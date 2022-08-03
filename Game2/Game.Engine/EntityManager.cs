using System.Collections.Immutable;
using System.Reflection;
using EcsRx.Collections.Database;
using EcsRx.Components;
using EcsRx.Extensions;
using Game.Engine.Core;
using Newtonsoft.Json;

namespace Game.Engine;

public static class EntityManager
{
    private static readonly Assembly AllowedAssembly = typeof(EntityManager).Assembly;
    private static readonly string SavePath = Path.GetFullPath("Saves");

    public static IEnumerable<string> GetSaveFiles()
        => Directory.EnumerateFiles(SavePath).Select(Path.GetFileNameWithoutExtension)!;

    public static void Save(IEntityDatabase database, string name)
    {
        if (!Directory.Exists(SavePath))
            Directory.CreateDirectory(SavePath);
        var targetName = Path.Combine(
            SavePath,
            name + ".sav");

        var data = new EntityDatabaseData(
            (
                from collection in database.Collections
                select new EntityCollectionData(
                    collection.Id,
                    (
                        from entity in collection
                        select new EntityData(
                            (
                                from component in entity.Components
                                let typeName = component.GetType().AssemblyQualifiedName
                                where !string.IsNullOrWhiteSpace(typeName)
                                select new ComponentData(
                                    typeName,
                                    JsonConvert.SerializeObject(component))
                            ).ToImmutableList())
                    ).ToImmutableList())
            ).ToImmutableList());

        File.WriteAllText(
            targetName,
            JsonConvert.SerializeObject(
                data,
                Formatting.Indented));
    }

    public static void Load(IEntityDatabase database, string name)
    {
        var targetName = Path.Combine(
            SavePath,
            name + ".sav");
        var data = JsonConvert.DeserializeObject<EntityDatabaseData>(File.ReadAllText(targetName));

        if (data is null)
            throw new InvalidOperationException("Keine Daten für den Speicherstand gefunden");

        foreach (var collection in database.Collections.ToArray()) database.RemoveCollection(collection.Id);

        foreach (var entity in from collection in data.Collections
                 let newCollection = database.CreateCollection(collection.Id)
                 from entity in collection.Entitys
                 let newEntity = newCollection.CreateEntity()
                 from component in entity.Components
                 select (newEntity, component))
        {
            var (newEntity, component) = entity;

            var targetType = Type.GetType(component.TypeName);
            if (targetType is null)
                throw new InvalidOperationException("Komponente nicht gefunden");
            if (targetType.Assembly != AllowedAssembly)
                throw new InvalidOperationException($"Komponent ist nicht erlaubt: {targetType}");

            if (JsonConvert.DeserializeObject(
                    component.Data,
                    targetType) is IComponent newComponent)
                newEntity.AddComponent(newComponent);
        }
    }
}