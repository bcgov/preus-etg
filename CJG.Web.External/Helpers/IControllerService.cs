using CJG.Core.Interfaces.Service;
using NLog;

namespace CJG.Web.External.Helpers
{
    /// <summary>
    /// <typeparamref name="IControllerService"/> interface, provides an aggregated way to use dependency injection on a number of commonly used services.
    /// </summary>
    public interface IControllerService
    {
        IUserService UserService { get; }
        ILogger Logger { get; }
        ISiteMinderService SiteMinderService { get; }
        IStaticDataService StaticDataService { get; }
    }
}