using System;
using System.Collections.Generic;
using System.Linq;

namespace Tauron.Application.CelloManager.Logic.Historie
{
    public class CommittedRefill
    {
        public CommittedRefill(IReadOnlyCollection<CommittedSpool> commitedSpools, DateTime sentTime, DateTime compledTime, int id)
        {
            CommitedSpools = commitedSpools;
            SentTime       = sentTime;
            CompledTime    = compledTime;
            Id             = id;
        }

        public CommittedRefill()
        {
            CommitedSpools = new CommittedSpool[0];
            SentTime = DateTime.Now;
            Id = -1;
        }

        public IReadOnlyCollection<CommittedSpool> CommitedSpools { get; }
        public DateTime                            SentTime       { get; }
        public DateTime                            CompledTime    { get; }
        public int                                 Id             { get; }

        public int Count
        {
            get { return CommitedSpools.Sum(s => s.OrderedCount); }
        }
    }
}