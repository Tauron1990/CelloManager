namespace CelloManager.Core.Logic;

public sealed record OldValidateNameRequest(ReadySpoolModel Old, string? Name, string? Category);