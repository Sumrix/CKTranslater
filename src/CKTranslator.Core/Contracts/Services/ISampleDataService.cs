using System.Collections.Generic;
using System.Threading.Tasks;

using CKTranslator.Core.Models;

namespace CKTranslator.Core.Contracts.Services
{
    // Remove this class once your pages/features are using your data.
    public interface ISampleDataService
    {
        Task SaveOrderAsync(SampleOrder order);

        Task<IEnumerable<SampleOrder>> GetGridDataAsync();
    }
}
