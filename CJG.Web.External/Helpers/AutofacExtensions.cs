using Autofac;
using Autofac.Integration.Mvc;
using System.Web.Mvc;

namespace CJG.Web.External.Helpers
{
    /// <summary>
    /// <typeparamref name="AutofacExtensions"/> static class, provides extension methods related to Autofac.
    /// </summary>
    public static class AutofacExtensions
    {
        /// <summary>
        /// Get the Autofac resolve from the generic .NET dependency resolver
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public static ILifetimeScope GetAutofacContainer(this IDependencyResolver resolver)
        {
            return ((AutofacDependencyResolver)DependencyResolver.Current).ApplicationContainer;
        }
    }
}