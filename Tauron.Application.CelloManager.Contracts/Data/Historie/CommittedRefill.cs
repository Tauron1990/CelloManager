using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Tauron.Application.CelloManager.Data.Historie
{
    [PublicAPI]
    public sealed class CommittedRefill
    {
        public int Id { get; set; }
        public List<CommittedSpool> CommitedSpools { get; set; } = new List<CommittedSpool>();
        public DateTime SentTime { get; set; }

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