using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using JetBrains.Annotations;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.CelloManager.Data.Historie
{
    [PublicAPI]
    public sealed class CommittedRefillEntity : GenericBaseEntity<int>
    {
        private DateTime _sentTime;
        private bool _isCompleted;
        private DateTime _compledTime;
        public ICollection<CommittedSpoolEntity> CommitedSpools { get; } = new ObservableCollection<CommittedSpoolEntity>();

        public DateTime SentTime
        {
            get => _sentTime;
            set => SetWithNotify(ref _sentTime, value);
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set => SetWithNotify(ref _isCompleted, value);
        }

        public DateTime CompledTime
        {
            get => _compledTime;
            set => SetWithNotify(ref _compledTime, value);
        }

        public void BuildString(StringBuilder builder)
        {
            builder.Append("Start").AppendLine()
            .Append("ID  : ").Append(Id).AppendLine()
            .Append("Zeit: ").Append(SentTime).AppendLine();

            foreach (var spool in CommitedSpools)
            {
                builder.Append("     ");
                spool.BuildString(builder);
            }

            builder.Append("End").AppendLine();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            BuildString(builder);

            return builder.ToString();
        }
    }
}