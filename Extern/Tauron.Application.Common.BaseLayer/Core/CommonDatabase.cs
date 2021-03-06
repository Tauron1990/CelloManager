﻿using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.Common.BaseLayer.Core
{
    [PublicAPI]
    public class CommonDatabase<TDbContext> : IDatabase
        where TDbContext : DbContext, new()
    {
        public CommonDatabase(TDbContext context)
        {
            Context = context;
        }

        protected TDbContext Context { get; }

        public void Dispose()
        {
            Context.Dispose();
        }

        public virtual string Id { get; } = "Common";

        public void Remove<TEntity>(TEntity entity)
            where TEntity : BaseEntity
        {
            Context.Remove(entity);
        }

        public void Update<TEntity>(TEntity entity)
            where TEntity : BaseEntity
        {
            Context.Update(entity);
        }

        public IQueryable<TEntity> Query<TEntity>()
            where TEntity : BaseEntity
        {
            return Context.Set<TEntity>();
        }

        public void Add<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            Context.Add(entity);
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        public TEntity Find<TEntity, TKey>(TKey key) where TEntity : GenericBaseEntity<TKey>
        {
            return Context.Find<TEntity>(key);
        }
    }
}