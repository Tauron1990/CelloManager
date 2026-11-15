using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CelloManager.Core.Comp;
using CelloManager.Core.Comp.OldData;
using CelloManager.Core.Data;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Kernel;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CelloManager.Core.Logic;

public sealed class SpoolManager
{
    private readonly ILogger<SpoolManager> _logger;
    private readonly SpoolRepository _repository;

    public SpoolManager(SpoolRepository repository, ILogger<SpoolManager> logger)
    {
        _repository = repository;
        _logger = logger;
        CurrentSpools = repository.Spools
                                  .Select(set => new ReadySpoolModel(set, repository))
                                  .Sort(ReadySpoolSorter.CategoryModelSorter)
                                  .Group(m => m.Category);

        KnowenCategorys = CurrentSpools.Select(g => g.Key);
    }

    public IObservable<IGroupChangeSet<ReadySpoolModel, string, string>> CurrentSpools { get; }

    public IObservable<int> Count => _repository.SpoolCount;

    public IObservable<IChangeSet<string, string>> KnowenCategorys { get; }

    public IObservable<bool> ValidateName(IObservable<ValidateNameRequest> requests) =>
        requests
           .CombineLatest(
                _repository.Spools
                           .QueryWhenChanged()
                           .Select(_ => Unit.Default)
                           .StartWith(Unit.Default),
                (r, _) => r)
           .Select(r => _repository.ValidateName(r.Name, r.Category));

    public IObservable<bool> ValidateModifyName(IObservable<OldValidateNameRequest> requests) =>
        requests
           .CombineLatest(
                _repository.Spools
                           .QueryWhenChanged()
                           .Select(_ => Unit.Default)
                           .StartWith(Unit.Default),
                (r, _) => r)
           .Select(r =>
                   {
                       string id = SpoolData.CreateId(r.Name ?? string.Empty, r.Category ?? string.Empty);
                       return string.Equals(id, r.Old.Data.Id, StringComparison.Ordinal) || _repository.ValidateName(r.Name, r.Category);
                   });

    public bool Exist(string name, string category)
        => _repository.LookUp(name, category).HasValue;

    public IObservable<bool> CanDelete(ReadySpoolModel model)
        => _repository.Spools.QueryWhenChanged().Select(l => l.Keys.Contains(model.Data.Id, StringComparer.Ordinal));


    public void CreateSpool(string? name, string? category, int amount, int needAmount)
    {
        if (!_repository.ValidateName(name, category))
        {
            _logger.LogDebug(new EventId(2001, "SpoolCreationRejected"), "Spool creation was ignored because the name was not accepted");
            return;
        }

        if (amount < 0)
        {
            amount = 0;
        }

        SpoolData data = SpoolData.New(name, category, amount);
        if (needAmount > 0)
        {
            data = data with { NeedAmount = needAmount };
        }

        _repository.UpdateSpool(data);
        _logger.LogInformation(new EventId(2000, "SpoolCreated"), "Spool added to the inventory cache");
    }

    public void UpdateSpool(SpoolData old, string? name, string? category, int amount, int needAmount)
    {
        if (needAmount <= 0)
        {
            needAmount = -1;
        }

        string id = SpoolData.CreateId(name ?? string.Empty, category ?? string.Empty);

        if (string.Equals(id, old.Id, StringComparison.Ordinal))
        {
            _repository.UpdateSpool(old with { Amount = amount, NeedAmount = needAmount });
            _logger.LogInformation(new EventId(2002, "SpoolUpdated"), "Spool updated in the inventory cache");
        }
        else if (_repository.ValidateName(name, category))
        {
            _repository.Edit(u =>
                             {
                                 u.Remove(old);
                                 u.AddOrUpdate(SpoolData.New(name, category, amount) with { NeedAmount = needAmount });
                             });
            _logger.LogInformation(new EventId(2003, "SpoolRenamed"), "Spool renamed in the inventory cache");
        }
        else
        {
            _logger.LogDebug(new EventId(2004, "SpoolUpdateRejected"), "Spool update was ignored because the new name was not accepted");
        }
    }

    [RequiresUnreferencedCode("The serializer options are not known at compile time.")]
    public async Task<Exception?> ExportToJson(IStorageFile path)
    {
        try
        {
            SpoolData[] array = [.. _repository.SpoolItems];

            Stream stream = await path.OpenWriteAsync().ConfigureAwait(false);
            await using (stream.ConfigureAwait(false))
            {
                await JsonSerializer.SerializeAsync(stream, array, SerializerContext.Default.Options).ConfigureAwait(false);
            }

            _logger.LogInformation(new EventId(2010, "InventoryExportCompleted"), "Inventory export completed for {SpoolCount} spools", array.Length);
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(new EventId(2011, "InventoryExportFailed"), e, "Inventory export failed");
            return e;
        }
    }

    [RequiresUnreferencedCode("The serializer options are not known at compile time.")]
    public async Task<Exception?> ImportFromJson(IStorageFile path)
    {
        try
        {
            int recordCount;
            Stream stream = await path.OpenReadAsync().ConfigureAwait(false);
            await using (stream.ConfigureAwait(false))
            {
                SpoolData[]? array = await JsonSerializer.DeserializeAsync<SpoolData[]>(stream, SerializerContext.Default.Options).ConfigureAwait(false);

                if (array is null)
                {
                    throw new InvalidOperationException("Lesen der Daten Fehlgeschlagen");
                }

                foreach (SpoolData spool in array)
                {
                    Optional<SpoolData> old = _repository.LookUp(spool.Name, spool.Category);

                    if (old.HasValue)
                    {
                        UpdateSpool(old.Value, spool.Name, spool.Category, spool.Amount, spool.NeedAmount);
                    }
                    else
                    {
                        CreateSpool(spool.Name, spool.Category, spool.Amount, spool.NeedAmount);
                    }
                }

                recordCount = array.Length;
            }

            _logger.LogInformation(new EventId(2012, "JsonImportCompleted"), "JSON inventory import processed {SpoolCount} records", recordCount);
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(new EventId(2013, "JsonImportFailed"), e, "JSON inventory import failed");
            return e;
        }
    }

    [RequiresUnreferencedCode("EF Core")]
    public async Task<Exception?> ImportFromLegacy(IStorageFile path)
    {
        try
        {
            if (!File.Exists(path.Path.LocalPath))
            {
                _logger.LogWarning(new EventId(2014, "LegacyImportFileMissing"), "Legacy inventory import skipped because the selected file is missing");
                return null;
            }

            int recordCount = 0;
            CoreDatabase database = new();
            await using (database.ConfigureAwait(false))
            {
                SqliteConnectionStringBuilder builder = new()
                                                        {
                                                            DataSource = path.Path.LocalPath,
                                                        };

                database.Database.SetConnectionString(builder.ConnectionString);

                await foreach (CelloSpoolEntity spool in database.CelloSpools)
                {
                    recordCount++;
                    Optional<SpoolData> old = _repository.LookUp(spool.Name, spool.Type);

                    if (old.HasValue)
                    {
                        UpdateSpool(old.Value, spool.Name, spool.Type, spool.Amount, spool.Neededamount);
                    }
                    else
                    {
                        CreateSpool(spool.Name, spool.Type, spool.Amount, spool.Neededamount);
                    }
                }
            }

            _logger.LogInformation(new EventId(2015, "LegacyImportCompleted"), "Legacy inventory import processed {SpoolCount} records", recordCount);
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(new EventId(2016, "LegacyImportFailed"), e, "Legacy inventory import failed");
            return e;
        }
    }

    public void Delete(ReadySpoolModel model)
    {
        _repository.Delete(model.Data);
        _logger.LogInformation(new EventId(2005, "SpoolDeleted"), "Spool removed from the inventory cache");
    }
}