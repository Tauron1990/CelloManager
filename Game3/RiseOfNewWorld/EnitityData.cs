﻿using System.Collections.Immutable;

namespace Game.Engine.Core;

public sealed record ComponentData(string TypeName, string Data);

public sealed record EntityData(ImmutableList<ComponentData> Components);

public sealed record EntityCollectionData(int Id, ImmutableList<EntityData> Entitys);

public sealed record EntityDatabaseData(ImmutableList<EntityCollectionData> Collections);