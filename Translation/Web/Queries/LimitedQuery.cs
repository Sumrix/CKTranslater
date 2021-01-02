using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Translation.Web.Queries
{
    /// <summary>
    ///     Запрос выполняемый пачками
    /// </summary>
    /// <typeparam name="TInputPart">Тип входящих данных</typeparam>
    /// <typeparam name="TOuputPart">Тип исходящих данных</typeparam>
    public abstract class LimitedQuery<TInputPart, TOuputPart> :
        Query<IEnumerable<TInputPart>, IEnumerable<TOuputPart>>
    {
        private readonly int batchSize;

        protected LimitedQuery(QueueTimer queryTimer, int batchSize) : base(queryTimer)
        {
            this.batchSize = batchSize;
        }

        public override IEnumerable<TOuputPart> Execute(IEnumerable<TInputPart> input)
        {
            return input
                .Batch(this.batchSize)
                .SelectMany(base.Execute);
        }
    }
}