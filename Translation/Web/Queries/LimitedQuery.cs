using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace NameTranslation.Web.Queries
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

        protected LimitedQuery(QueueTimer queryTimer, int batchSize, string logPath)
            : base(queryTimer, logPath)
        {
            this.batchSize = batchSize;
        }

        public override IEnumerable<TOuputPart> Execute(IEnumerable<TInputPart> param)
        {
            return param
                .Batch(this.batchSize)
                .SelectMany(base.Execute);
        }
    }
}