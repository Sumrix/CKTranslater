using System.Collections.Generic;
using System.Linq;

using MoreLinq;
using Translation;

namespace Translation.Web.Queries
{
    public abstract class LimitedQuery<TInputPart, TOuputPart> : 
        Query<IEnumerable<TInputPart>, IEnumerable<TOuputPart>>
    {
        protected abstract int BatchSize { get; }

        public LimitedQuery(QueueTimer queryTimer) : base(queryTimer)
        {
        }

        public override IEnumerable<TOuputPart> Execute(IEnumerable<TInputPart> input) => 
            input
            .Batch(this.BatchSize)
            .SelectMany(base.Execute);
    }
}
