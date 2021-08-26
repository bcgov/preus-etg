using Autofac;
using CJG.Application.Services;
using CJG.Application.Services.Web;
using CJG.Core.Interfaces.Service;
using CJG.Core.Interfaces.Service.Settings;
using CJG.Infrastructure.EF;
using CJG.Infrastructure.Entities;
using CJG.Infrastructure.NotificationService.Properties;
using NLog;
using System;
using System.Globalization;
using System.Web;

namespace CJG.Infrastructure.NotificationService
{
	/// <summary>
	/// <typeparamref name="AppFactory"/> class, provides a way to setup and configure the dependency injection for the <typeparamref name="INotificationJob"/>.
	/// </summary>
	internal class AppFactory
	{
		#region Variables
		private readonly ILogger _logger;

		/// <summary>
		/// get - The dependency injection container.
		/// </summary>
		public IContainer Container { get; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="AppFactory"/>.
		/// </summary>
		public AppFactory()
		{
			_logger = LogManager.GetCurrentClassLogger();

			Container = CreateContainer();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Setup and configure dependency injection.
		/// </summary>
		/// <returns></returns>
		private IContainer CreateContainer()
		{
			var builder = new ContainerBuilder();
			builder.RegisterInstance(new InternalHttpContext(new HttpContextWrapper(new HttpContext(new HttpRequest("", Settings.Default.SiteUrl, null), new HttpResponse(null))))).As<HttpContextBase>();
			builder.RegisterInstance(_logger).As<ILogger>();
			builder.RegisterType<NotificationJob>().As<INotificationJob>();
			builder.RegisterType<CompletionReportService>().As<ICompletionReportService>();
			builder.RegisterType<GrantAgreementService>().As<IGrantAgreementService>();
			builder.RegisterType<Application.Services.NotificationService>().As<INotificationService>();
			builder.RegisterType<DataContext>().As<IDataContext>().InstancePerLifetimeScope();
			builder.RegisterType<EmailSenderService>().As<IEmailSenderService>();
			builder.Register(c=> new NotificationSettings(Settings.Default)).As<INotificationSettings>();
			builder.Register(c=> new EmailSenderSettings(Settings.Default)).As<IEmailSenderSettings>();
			builder.RegisterType<SettingService>().As<ISettingService>();
			return builder.Build();
		}

		/// <summary>
		/// Get the current logger for this <typeparamref name="AppFactory"/>.
		/// </summary>
		/// <returns></returns>
		public ILogger GetLogger()
		{
			return _logger;
		}

		/// <summary>
		/// Get the <typeparamref name="INotificationJob"/> from dependency injection container.
		/// </summary>
		/// <returns></returns>
		public INotificationJob GetNotificationJob()
		{
			return Container.Resolve<INotificationJob>();
		}

		/// <summary>
		/// Get the Setting AppDateTime from the datasource or configuration.
		/// This provides a way for the application to control the current date.
		/// </summary>
		/// <returns></returns>
		public DateTime? GetAppDateTime()
		{
			DateTime now;
			var culture = CultureInfo.CreateSpecificCulture("en-CA");
			var settingService = Container.Resolve<ISettingService>();
			if (settingService != null)
			{
				var appDateTime = settingService.Get("AppDateTime")?.Value ?? DateTime.UtcNow.ToString();
				if (appDateTime != null && DateTime.TryParse(appDateTime, culture, DateTimeStyles.AssumeUniversal, out now))
					return now;
			}

			return null;
		}
		#endregion
	}
}