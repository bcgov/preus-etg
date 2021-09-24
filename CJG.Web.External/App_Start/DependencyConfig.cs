using Autofac;
using Autofac.Extras.AggregateService;
using Autofac.Integration.Mvc;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.EF;
using CJG.Infrastructure.Entities;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Helpers;
using Microsoft.AspNet.Identity;
using NLog;
using System;
using System.Web;
using System.Web.Mvc;

namespace CJG.Web.External
{
	public class DependencyConfig
	{
		static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

		public static void Configure(ContainerBuilder builder, System.Reflection.Assembly assembly)
		{
			builder.RegisterControllers(assembly)
				.OnActivated(e =>
				{
					dynamic viewBag = ((Controller)e.Instance).ViewBag;
					viewBag.Version = typeof(MvcApplication).Assembly.GetName().Version;
				});
			builder.RegisterModule(new AutofacWebTypesModule());

			builder.RegisterSource(new ViewRegistrationSource());

			RegisterObjects(builder);
			RegisterIdentity(builder);
			RegisterRepositories(builder);
			RegisterServices(builder);

			builder.RegisterType<FileWrapper>().As<IFileWrapper>().InstancePerRequest();

			//Configure DI for Identity components
			builder.Register(c => HttpContext.Current.GetOwinContext().Authentication).InstancePerRequest();

			builder.RegisterAggregateService<IControllerService>();

			//Configure custom filter attribute
			builder.RegisterFilterProvider();

			var container = builder.Build();
			var dependencyResolver = new AutofacDependencyResolver(container);
			DependencyResolver.SetResolver(dependencyResolver);
		}

		public static void RegisterObjects(ContainerBuilder builder)
		{
			builder.RegisterInstance(_logger).As<ILogger>().SingleInstance();
		}

		public static void RegisterIdentity(ContainerBuilder builder)
		{
			builder.RegisterType<UserManagerAdapter>().As<IUserManagerAdapter>().InstancePerRequest();
			builder.RegisterType<ApplicationUserStore>().As<IUserStore<ApplicationUser>>().InstancePerRequest();
			builder.RegisterType<ApplicationRoleStore>().As<IRoleStore<ApplicationRole, string>>().InstancePerRequest();
			builder.RegisterType<ApplicationUserManager>().AsSelf().InstancePerRequest();
			builder.RegisterType<ApplicationRoleManager>().AsSelf().InstancePerRequest();
			builder.RegisterType<ApplicationSignInManager>().AsSelf().InstancePerRequest();
		}

		public static void RegisterRepositories(ContainerBuilder builder)
		{
			builder.RegisterType<Helpers.Settings.NotificationSettings>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<Helpers.Settings.EmailSenderSettings>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<Helpers.Settings.TrainingProviderSettings>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<DataContext>().As<IDataContext>().InstancePerRequest();
			builder.RegisterType<Infrastructure.BCeID.WebService.BCeIDService>().AsImplementedInterfaces().InstancePerRequest();
		}

		public static void RegisterServices(ContainerBuilder builder)
		{
			builder.RegisterInstance(PerHttpSafeResolve<IGrantApplicationService>());

			builder.RegisterAssemblyTypes(typeof(Application.Services.Service).Assembly)
				.Where(t => t.Name.EndsWith("Service"))
				.AsImplementedInterfaces()
				.InstancePerRequest();
		}

		public static Func<T> PerHttpSafeResolve<T>()
		{
			return () => DependencyResolver.Current.GetService<T>();
		}
	}
}
