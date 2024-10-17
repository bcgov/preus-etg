using System.Web;
using Autofac;
using CJG.Application.Services;
using CJG.Application.Services.Web;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.BCeID.WebService;
using CJG.Infrastructure.EF;
using CJG.Infrastructure.Entities;
using CJG.Infrastructure.ReportingService.Properties;
using NLog;

namespace CJG.Infrastructure.ReportingService
{
	internal class AppFactory
	{
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
			builder.RegisterType<SdsiReportJob>().As<ISdsiReportJob>();
			builder.RegisterType<ParticipantService>().As<IParticipantService>();
			builder.RegisterType<EligibleCostService>().As<IEligibleCostService>();
			builder.RegisterType<SiteMinderService>().As<ISiteMinderService>();
			builder.RegisterType<OrganizationService>().As<IOrganizationService>();
			builder.RegisterType<UserService>().As<IUserService>();
			builder.RegisterType<BCeIDService>().As<IBCeIDService>();
			builder.RegisterType<DataContext>().As<IDataContext>().InstancePerLifetimeScope();

			return builder.Build();
		}

		private readonly ILogger _logger;

		public IContainer Container { get; }
		
		public ILogger GetLogger()
		{
			return _logger;
		}
		
		public ISdsiReportJob GetGrantOpeningJob()
		{
			return Container.Resolve<ISdsiReportJob>();
		}
	}
}