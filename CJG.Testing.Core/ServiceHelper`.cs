using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;

namespace CJG.Testing.Core
{
	public class ServiceHelper<T> : ServiceHelper
		where T : class, IService
	{

		#region Constructors
		/// <summary>
		/// Creates a new instance of a ServiceHelper object.
		/// </summary>
		public ServiceHelper()
		{
			this.InitializeMockFor(typeof(T), typeof(T).GetConstructors()[0].GetParameters());
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ServiceHelper"/> object.
		/// Initializes all mocks required to create an instance of the specified service type.
		/// Initializes the HttpContextMock with the specified user.  This is important since all methods are validated against the current user.
		/// </summary>
		/// <param name="serviceType">The service type you want to initialize mocks for.</param>
		/// <param name="user">The current user that will be executing the code.</param>
		public ServiceHelper(System.Security.Principal.IPrincipal user)
		{
			this.InitializeMockFor(typeof(T), typeof(T).GetConstructors()[0].GetParameters());
			ChangeUser(user);
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ServiceHelper"/> object.
		/// Initializes all mocks required to create an instance of the specified service type.
		/// Initializes the HttpContextMock with the specified user.  This is important since all methods are validated against the current user.
		/// </summary>
		/// <param name="serviceType">The service type you want to initialize mocks for.</param>
		/// <param name="user">The current user that will be executing the code.</param>
		/// <param name="args">Use the constructor that contains parameters with the specified argument types.</param>
		public ServiceHelper(System.Security.Principal.IPrincipal user, params object[] args)
		{
			this.InitializeMockFor(typeof(T), args);
			ChangeUser(user);
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ServiceHelper"/> object.
		/// Initializes all mocks required to create an instance of the specified service type.
		/// Initializes the HttpContextMock with the specified user.  This is important since all methods are validated against the current user.
		/// </summary>
		/// <param name="serviceType">The service type you want to initialize mocks for.</param>
		/// <param name="applicationAdministrator">The current user that will be executing the code.</param>
		/// <param name="args">Use the constructor that contains parameters with the specified argument types.</param>
		public ServiceHelper(User applicationAdministrator, params object[] args) : this(HttpHelper.CreateIdentity(applicationAdministrator), args)
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ServiceHelper"/> object.
		/// Initializes all mocks required to create an instance of the specified service type.
		/// Initializes the HttpContextMock with the specified user.  This is important since all methods are validated against the current user.
		/// </summary>
		/// <param name="serviceType">The service type you want to initialize mocks for.</param>
		/// <param name="user">The current user that will be executing the code.</param>
		/// <param name="role">The current users role.</param>
		/// <param name="args">Use the constructor that contains parameters with the specified argument types.</param>
		public ServiceHelper(InternalUser user, string role, params object[] args) : this(HttpHelper.CreateIdentity(user, role), args)
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ServiceHelper"/> object.
		/// Initializes all mocks required to create an instance of the specified service type.
		/// Initializes the HttpContextMock with the specified user.  This is important since all methods are validated against the current user.
		/// </summary>
		/// <param name="serviceType">The service type you want to initialize mocks for.</param>
		/// <param name="user">The current user that will be executing the code.</param>
		/// <param name="role">The current users role.</param>
		/// <param name="args">Use the constructor that contains parameters with the specified argument types.</param>
		public ServiceHelper(InternalUser user, Roles role, params object[] args) : this(HttpHelper.CreateIdentity(user, role), args)
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ServiceHelper"/> object.
		/// Initializes all mocks required to create an instance of the specified service type.
		/// Initializes the HttpContextMock with the specified user.  This is important since all methods are validated against the current user.
		/// </summary>
		/// <param name="serviceType">The service type you want to initialize mocks for.</param>
		/// <param name="user">The current user that will be executing the code.</param>
		/// <param name="privilege">The current users privilege.</param>
		/// <param name="args">Use the constructor that contains parameters with the specified argument types.</param>
		public ServiceHelper(InternalUser user, Privilege privilege, params object[] args) : this(HttpHelper.CreateIdentity(user, privilege), args)
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ServiceHelper"/> object.
		/// Initializes all mocks required to create an instance of the specified service type.
		/// Initializes the HttpContextMock with the specified user.  This is important since all methods are validated against the current user.
		/// </summary>
		/// <param name="serviceType">The service type you want to initialize mocks for.</param>
		/// <param name="user">The current user that will be executing the code.</param>
		/// <param name="privileges">The current users privileges.</param>
		/// <param name="args">Use the constructor that contains parameters with the specified argument types.</param>
		public ServiceHelper(InternalUser user, Privilege[] privileges, params object[] args) : this(HttpHelper.CreateIdentity(user, privileges), args)
		{
		}
		#endregion

		#region Methods

		/// <summary>
		/// Creates a new instance of <typeparamref name="T"/> and initializes it with either the args specified or the mocked objects in this helper.
		/// This always uses the constructor that was initialized for this service to create the object.
		/// </summary>
		/// <param name="args"></param>
		/// <returns>A new instance of <typeparamref name="T"/> object.</returns>
		public T Create(params object[] args)
		{
			return base.Create<T>(args);
		}
		#endregion
	}
}
