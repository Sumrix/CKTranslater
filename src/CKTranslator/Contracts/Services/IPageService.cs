using System;

namespace CKTranslator.Contracts.Services
{
    public interface IPageService
    {
        Type GetPageType(string key);
    }
}
