using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.EF;
using CJG.Infrastructure.Entities;
using CJG.Infrastructure.Identity;
using Moq;
using NLog;

namespace CJG.Testing.Core
{
	/// <summary>
	/// ServiceHelper sealed class, provides a one stop shop to initialize services and their related objects.
	/// This makes it easier to setup a unit test since it cuts the number of lines of code down.
	/// This also improves performance of testing since we don't Mock objects we don't need.
	/// </summary>
	public class ServiceHelper : MoqHelper
	{
		#region Variables
		public enum Roles
		{
			Assessor,
			[Description("System Administrator")]
			SystemAdministrator,
			[Description("Operations Manager")]
			OperationsManager,
			[Description("Measurement And Reporting")]
			MeasurementAndReporting,
			Director,
			[Description("Financial Clerk")]
			FinancialClerk,
			[Description("Director Of Finance")]
			DirectorOfFinance
		}

		/// <summary>
		/// Each role and their assigned privileges.
		/// </summary>
		public readonly static Dictionary<string, Privilege[]> RolePrivileges = new Dictionary<string, Privilege[]>()
		{
			{ "Assessor", new[] { Privilege.IA1, Privilege.IA4, Privilege.AM1, Privilege.AM2, Privilege.AM5, Privilege.PM1, Privilege.TP1 } },
			{ "System Administrator", new[] { Privilege.SM, Privilege.IA1, Privilege.IA2, Privilege.IA3, Privilege.IA4, Privilege.IA5, Privilege.AM1, Privilege.AM2, Privilege.AM3, Privilege.AM4, Privilege.AM5, Privilege.IA1 } },
			{ "Operations Manager", new[] { Privilege.IA1, Privilege.IA2, Privilege.IA4, Privilege.IA5, Privilege.UM1, Privilege.PM1, Privilege.PM2, Privilege.TP1 } },
			{ "Measurement And Reporting", new Privilege[0] },
			{ "Director", new[] { Privilege.IA1, Privilege.IA2, Privilege.IA3, Privilege.IA4, Privilege.IA5, Privilege.AM1, Privilege.AM2, Privilege.AM3, Privilege.AM4, Privilege.AM5, Privilege.IA1, Privilege.GM1 } },
			{ "Financial Clerk", new[] { Privilege.IA1, Privilege.PM1, Privilege.PM2, Privilege.AM5 } },
			{ "Director Of Finance", new[] { Privilege.IA1, Privilege.IA2, Privilege.PM1, Privilege.PM2 } }
		};
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a ServiceHelper object.
		/// </summary>
		public ServiceHelper()
		{
		}

		/// <summary>
		/// Creates a new instance of a ServiceHelper object.
		/// </summary>
		/// <param name="serviceType">The service type you want to initialize mocks for.</param>
		public ServiceHelper(Type serviceType)
		{
			this.InitializeMockFor(serviceType, serviceType.GetConstructors()[0].GetParameters());
		}

		/// <summary>
		/// Creates a new instance of a ServiceHelper object.
		/// Initializes all mocks required to create an instance of the specified service type.
		/// Initializes the HttpContextMock with the specified user.  This is important since all methods are validated against the current user.
		/// </summary>
		/// <param name="serviceType">The service type you want to initialize mocks for.</param>
		/// <param name="user">The current user that will be executing the code.</param>
		public ServiceHelper(Type serviceType, System.Security.Principal.IPrincipal user)
		{
			this.InitializeMockFor(serviceType, serviceType.GetConstructors()[0].GetParameters());
			ChangeUser(user);
		}

		/// <summary>
		/// Creates a new instance of a ServiceHelper object.
		/// Initializes all mocks required to create an instance of the specified service type.
		/// Initializes the HttpContextMock with the specified user.  This is important since all methods are validated against the current user.
		/// </summary>
		/// <param name="serviceType">The service type you want to initialize mocks for.</param>
		/// <param name="user">The current user that will be executing the code.</param>
		/// <param name="args">Use the constructor that contains parameters with the specified argument types.</param>
		public ServiceHelper(Type serviceType, System.Security.Principal.IPrincipal user, params object[] args)
		{
			this.InitializeMockFor(serviceType, args);
			ChangeUser(user);
		}

		/// <summary>
		/// Creates a new instance of a ServiceHelper object.
		/// Initializes all mocks required to create an instance of the specified service type.
		/// Initializes the HttpContextMock with the specified user.  This is important since all methods are validated against the current user.
		/// </summary>
		/// <param name="serviceType">The service type you want to initialize mocks for.</param>
		/// <param name="user">The current user that will be executing the code.</param>
		/// <param name="args">Use the constructor that contains parameters with the specified argument types.</param>
		public ServiceHelper(Type serviceType, User user, params object[] args) : this(serviceType, HttpHelper.CreateIdentity(user), args)
		{
			this.InitializeMockFor(serviceType, args);
			ChangeUser(user);
		}

		/// <summary>
		/// Creates a new instance of a ServiceHelper object.
		/// Initializes all mocks required to create an instance of the specified service type.
		/// Initializes the HttpContextMock with the specified user.  This is important since all methods are validated against the current user.
		/// </summary>
		/// <param name="serviceType">The service type you want to initialize mocks for.</param>
		/// <param name="user">The current user that will be executing the code.</param>
		/// <param name="role">The current users role.</param>
		/// <param name="args">Use the constructor that contains parameters with the specified argument types.</param>
		public ServiceHelper(Type serviceType, InternalUser user, string role, params object[] args)
		{
			this.InitializeMockFor(serviceType, args);
			ChangeUser(user, role);
		}

		/// <summary>
		/// Creates a new instance of a ServiceHelper object.
		/// Initializes all mocks required to create an instance of the specified service type.
		/// Initializes the HttpContextMock with the specified user.  This is important since all methods are validated against the current user.
		/// </summary>
		/// <param name="serviceType">The service type you want to initialize mocks for.</param>
		/// <param name="user">The current user that will be executing the code.</param>
		/// <param name="role">The current users role.</param>
		/// <param name="args">Use the constructor that contains parameters with the specified argument types.</param>
		public ServiceHelper(Type serviceType, InternalUser user, Roles role, params object[] args)
		{
			this.InitializeMockFor(serviceType, args);
			ChangeUser(user, role);
		}

		/// <summary>
		/// Creates a new instance of a ServiceHelper object.
		/// Initializes all mocks required to create an instance of the specified service type.
		/// Initializes the HttpContextMock with the specified user.  This is important since all methods are validated against the current user.
		/// </summary>
		/// <param name="serviceType">The service type you want to initialize mocks for.</param>
		/// <param name="user">The current user that will be executing the code.</param>
		/// <param name="privilege">The current users privilege.</param>
		/// <param name="args">Use the constructor that contains parameters with the specified argument types.</param>
		public ServiceHelper(Type serviceType, InternalUser user, Privilege privilege, params object[] args)
		{
			this.InitializeMockFor(serviceType, args);
			ChangeUser(user, privilege);
		}

		/// <summary>
		/// Creates a new instance of a ServiceHelper object.
		/// Initializes all mocks required to create an instance of the specified service type.
		/// Initializes the HttpContextMock with the specified user.  This is important since all methods are validated against the current user.
		/// </summary>
		/// <param name="serviceType">The service type you want to initialize mocks for.</param>
		/// <param name="user">The current user that will be executing the code.</param>
		/// <param name="privileges">The current users privileges.</param>
		/// <param name="args">Use the constructor that contains parameters with the specified argument types.</param>
		public ServiceHelper(Type serviceType, InternalUser user, Privilege[] privileges, params object[] args)
		{
			this.InitializeMockFor(serviceType, args);
			ChangeUser(user, privileges);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Initializes new instances for all the constructor parameters required for the specified service type.
		/// This will not overwrite previously initialized mocked objects.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="args">Use the constructor that contains parameters with the specified argument types.</param>
		public new void InitializeMockFor<T>(params object[] args) where T : class
		{
			var type = typeof(T);
			this.InitializeMockFor(type, args);
		}

		/// <summary>
		/// Initializes new instances for all the constructor parameters required for the specified service type.
		/// This will not overwrite previously initialized mocked objects.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="args">Use the constructor that contains parameters with the specified argument types.</param>
		public new void InitializeMockFor(Type type, params object[] args)
		{
			this.GetMock<IUserService>();
			this.GetMock<IStaticDataService>();
			this.GetMock<ISiteMinderService>();
			this.GetMock<ILogger>();

			base.InitializeMockFor(type, args);

			// Quick and dirty way to manually setup the services to make each test Arrange smaller.
			if (type == typeof(ApplicationWorkflowStateMachine) || typeof(IService).IsAssignableFrom(type))
			{
				if (type == typeof(GrantApplicationService)
					|| type == typeof(ApplicationWorkflowStateMachine)
					|| type == typeof(GrantAgreementService))
				{
					if (!this.IsMocked(typeof(DbSet<GrantApplication>)))
						MockDbSet<GrantApplication>();
					if (!this.IsMocked(typeof(DbSet<GrantApplicationStateChange>)))
						MockDbSet<GrantApplicationStateChange>();
				}
			}
		}

		#region DataContext
		/// <summary>
		/// Setup the CJGContext mock so that every repository will use it.
		/// </summary>
		/// <returns></returns>
		public Mock<IDataContext> MockContext()
		{
			var contextMock = this.SetMockAs<DataContext, IDataContext>(false);
			return contextMock;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		public Mock<DbSet<TEntity>> MockDbSet<TEntity>(TEntity entity)
			where TEntity : EntityBase
		{
			if (entity == null) {
				return MockDbSet(new Collection<TEntity>());
			} else {
				return MockDbSet(new Collection<TEntity> { entity });
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="collection"></param>
		/// <param name="overwrite"></param>
		/// <returns></returns>
		public Mock<DbSet<TEntity>> MockDbSet<TEntity>(ICollection <TEntity> collection = null)
			where TEntity : class
		{
			var dbSetMock = this.GetMock<DbSet<TEntity>>(false);
			if (dbSetMock != null) {
				// Add collection to the DbSet
				if (collection != null) {
					foreach (var item in collection) {
						dbSetMock.Object.Add(item);
					}
				}
			} else {
				if (collection == null) collection = new List<TEntity>();

				var queryable = collection.AsQueryable();

				// Mock DbSet
				dbSetMock = new Mock<DbSet<TEntity>>();
				dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(queryable.Provider);
				dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(queryable.Expression);
				dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
				dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
				dbSetMock.Setup(m => m.Add(It.IsAny<TEntity>())).Callback<TEntity>(o => collection.Add(o));
				dbSetMock.Setup(m => m.Remove(It.IsAny<TEntity>())).Callback<TEntity>(o => collection.Remove(o));
				dbSetMock.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => {
					var keys = Activator.CreateInstance<TEntity>().GetKeys();
					return collection.FirstOrDefault(o => {
						if (o == null) return false;

						// Look through the keys and ensure they match the specified ids values.
						for (var i = 0; i < keys.Length; i++) {
							if (!keys[i].GetValue(o).Equals(ids[i]))
								return false;
						}

						return true;
					});
				});
				dbSetMock.Setup(m => m.Include(It.IsAny<string>())).Returns(dbSetMock.Object);

				this.SetMock(dbSetMock);

				SetupDbContext(dbSetMock);
			}

			return dbSetMock;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="dbSetMock"></param>
		private void SetupDbContext<TEntity>(Mock<DbSet<TEntity>> dbSetMock)
			where TEntity : class
		{
			var dbContextMock = this.GetMock<IDataContext>();

			dbContextMock.Setup(m => m.Set<TEntity>()).Returns(() => { return dbSetMock.Object; });
			dbContextMock.Setup(m => m.Validate<TEntity>(It.IsAny<TEntity>())).Returns<TEntity>(entity => {
				var vEntity = entity as IValidatableObject;
				if (vEntity == null) return Enumerable.Empty<ValidationResult>();

				var items = new Dictionary<object, object> { { "DbContext", dbContextMock.Object } };
				DbEntityEntry entry = dbContextMock.Object.Context.Entry(entity);

				items.Add("Entry", entry);
				items.Add("HttpContext", this.GetMock<HttpContextBase>().Object);
				var results = new List<ValidationResult>();
				ValidationContext validationContext = new ValidationContext(entity, items);
			
				// Call the entity validation manually
				Validator.TryValidateObject(entity, validationContext, results, true);
				return results.Select(e => new ValidationResult(e.ErrorMessage, e.MemberNames));
			});
			dbContextMock.Setup(m => m.Commit());
			dbContextMock.Setup(m => m.CommitTransaction());
			dbContextMock.Setup(m => m.Update(It.IsAny<TEntity>())).Callback((TEntity ga) => dbSetMock.Object.Add(ga));
			
			// Setup Return expression
			var type = typeof(IDataContext);
			var parameter = Expression.Parameter(type, "i");
			var propertyType = typeof(DbSet<>).MakeGenericType(typeof(TEntity));
			var property = Expression.Property(parameter, type.GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(p => p.PropertyType == propertyType || propertyType.IsAssignableFrom(p.PropertyType)));
			var delegateType = typeof(Func<,>).MakeGenericType(type, propertyType);
			var expression = (Expression<Func<IDataContext, DbSet<TEntity>>>)Expression.Lambda(delegateType, property, parameter);
			dbContextMock.Setup(expression).Returns(dbSetMock.Object);
		}
		#endregion

		#region Create Services
		/// <summary>
		/// Creates a new instance of <typeparamref name="T"/> and initializes it with either the args specified or the mocked objects in this helper.
		/// This always uses the constructor that was initialized for this service to create the object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="user"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public T CreateFor<T>(User user, params object[] args) where T : class
		{
			ChangeUser(user);
			return this.Create<T>(args);
		}

		/// <summary>
		/// Creates a new instance of <typeparamref name="T"/> and initializes it with either the args specified or the mocked objects in this helper.
		/// This always uses the constructor that was initialized for this service to create the object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="user"></param>
		/// <param name="role"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public T CreateFor<T>(InternalUser user, Roles role, params object[] args) where T : class
		{
			ChangeUser(user, role);
			return this.Create<T>(args);
		}
		#endregion

		#region Change User
		/// <summary>
		/// Change the active user currently performing the actions.
		/// </summary>
		/// <param name="user"></param>
		protected void ChangeUser(IPrincipal user)
		{
			HttpHelper.MockHttpContext(this, user);
			var siteMinderService = this.GetMock<ISiteMinderService>();
			siteMinderService.Setup(m => m.CurrentUserGuid).Returns(user.GetBCeIdGuid() ?? Guid.Empty);
			siteMinderService.Setup(m => m.CurrentUserName).Returns(user.GetUserName());
			siteMinderService.Setup(m => m.CurrentUserType).Returns(user.GetAccountType().Value == AccountTypes.Internal ? BCeIDAccountTypeCodes.Void : BCeIDAccountTypeCodes.Business);
			this.GetMock<ISiteMinderService>().Setup(m => m.CurrentUserGuid).Returns(user.GetBCeIdGuid() ?? Guid.Empty);
			this.GetMock<ISiteMinderService>().Setup(m => m.CurrentUserName).Returns(user.GetUserName());
			this.GetMock<IUserService>().Setup(m => m.GetAccountType()).Returns(user.GetAccountType().Value);
		}

		/// <summary>
		/// Change the active user currently performing the actions.
		/// </summary>
		/// <param name="user"></param>
		public void ChangeUser(User user)
		{
			ChangeUser(HttpHelper.CreateIdentity(user));
			this.GetMock<IUserService>().Setup(m => m.GetUser(It.Is<int>((id) => id == user.Id))).Returns(user);
			this.GetMock<IUserService>().Setup(m => m.GetUser(It.Is<Guid>((guid) => guid == user.BCeIDGuid))).Returns(user);
		}

		/// <summary>
		/// Change the active user currently performing the actions.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="privileges"></param>
		public void ChangeUser(InternalUser user, params Privilege[] privileges)
		{
			ChangeUser(HttpHelper.CreateIdentity(user, privileges));
			this.GetMock<IUserService>().Setup(m => m.GetInternalUser(It.Is<int>((id) => id == user.Id))).Returns(user);
			this.GetMock<IUserService>().Setup(m => m.GetInternalUser(It.Is<string>((idir) => idir == user.IDIR))).Returns(user);
		}

		/// <summary>
		/// Change the active user currently performing the actions.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="role"></param>
		public void ChangeUser(InternalUser user, string role)
		{
			ChangeUser(HttpHelper.CreateIdentity(user, role));
			this.GetMock<IUserService>().Setup(m => m.GetInternalUser(It.Is<int>((id) => id == user.Id))).Returns(user);
			this.GetMock<IUserService>().Setup(m => m.GetInternalUser(It.Is<string>((idir) => idir == user.IDIR))).Returns(user);
		}

		/// <summary>
		/// Change the active user currently performing the actions.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="role"></param>
		public void ChangeUser(InternalUser user, Roles role)
		{
			ChangeUser(HttpHelper.CreateIdentity(user, role));
			this.GetMock<IUserService>().Setup(m => m.GetInternalUser(It.Is<int>((id) => id == user.Id))).Returns(user);
			this.GetMock<IUserService>().Setup(m => m.GetInternalUser(It.Is<string>((idir) => idir == user.IDIR))).Returns(user);
		}
		#endregion
		#endregion
	}
}
