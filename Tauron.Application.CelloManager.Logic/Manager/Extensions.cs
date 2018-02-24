using System;
using Tauron.Application.CelloManager.Data.Manager;

namespace Tauron.Application.CelloManager.Logic.Manager
{
    public static class Extensions
    {
        public static CelloSpool CreateCelloSpool(this CelloSpoolEntity entity) => new CelloSpool(entity.Name, entity.Type, entity.Amount, entity.Neededamount, entity.Id);

        public static CelloSpoolEntity CreateEntity(this CelloSpool spool)
        {
            return new CelloSpoolEntity
                   {
                       Amount = spool.Amount,
                       Name = spool.Name,
                       Neededamount = spool.Neededamount,
                       Timestamp = DateTime.Now,
                       Type = spool.Type
                   };
        }
    }
}