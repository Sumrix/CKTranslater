using System.Threading.Tasks;

namespace CKTranslator.Contracts.Services
{
    public interface IActivationService
    {
        Task ActivateAsync(object activationArgs);

        void Activate(object activationArgs);
    }
}
