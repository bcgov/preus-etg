using System.Web;
using Autofac;
using CJG.Application.Services.Web;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.EF;
using CJG.Infrastructure.Entities;
using CJG.Infrastructure.GrantOpeningService.Properties;
using NLog;

namespace CJG.Infrastructure.GrantOpeningService
{
	internal class AppFactory
    {
	    public IContainer Container { get; }

	    private readonly ILogger _logger;

		public AppFactory()
        {
            _logger = LogManager.GetCurrentClassLogger();
            Container = CreateContainer();
        }

        private IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(_logger).As<ILogger>();
            builder.RegisterInstance(new InternalHttpContext(new HttpContextWrapper(new HttpContext(new HttpRequest("", Settings.Default.SiteUrl, null), new HttpResponse(null))))).As<HttpContextBase>();
            builder.RegisterType<DataContext>().As<IDataContext>().InstancePerLifetimeScope();

            builder.RegisterType<Application.Services.SettingService>().As<ISettingService>();
            builder.RegisterType<Application.Services.GrantStreamService>().As<IGrantStreamService>();
            builder.RegisterType<Application.Services.GrantOpeningService>().As<IGrantOpeningService>();
			
			builder.RegisterType<GrantOpeningJob>().As<IGrantOpeningJob>();

            return builder.Build();
        }

        public ILogger GetLogger()
        {
            return _logger;
        }
        
        public IGrantOpeningJob GetGrantOpeningJob()
        {
            return Container.Resolve<IGrantOpeningJob>();
        }
    }
}