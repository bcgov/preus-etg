using Moq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace CJG.Testing.Core
{
	public class MoqHelper
	{
		#region Variables
		private readonly static ConcurrentDictionary<Type, ParameterInfo[]> MockedParameters = new ConcurrentDictionary<Type, ParameterInfo[]>();
		private readonly Dictionary<Type, Mock> Mocked = new Dictionary<Type, Mock>();
		#endregion

		#region Constructors
		#endregion

		#region Methods
		public bool IsMocked(Type type)
		{
			return Mocked.ContainsKey(type);
		}

		#region Initialize Mock
		/// <summary>
		/// Initializes new instances for all the constructor parameters required for the specified service type.
		/// This will not overwrite previously initialized mocked objects.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="constructor">The index of the constructor to use.  Default is the first.</param>
		public void InitializeMockFor<T>(int constructor = 0) where T : class
		{
			var type = typeof(T);
			InitializeMockFor(type, constructor);
		}

		/// <summary>
		/// Initializes new instances for all the constructor parameters required for the specified service type.
		/// This will not overwrite previously initialized mocked objects.
		/// </summary>
		/// <param name="type">The type you want to initialize mocks for.</param>
		/// <param name="constructor">The index of the constructor to use.  Default is the first.</param>
		public void InitializeMockFor(Type type, int constructor = 0)
		{
			InitializeMockFor(type, type.GetConstructors()[constructor].GetParameters());
		}

		/// <summary>
		/// Initializes new instances for all the constructor parameters required for the specified service type.
		/// This will not overwrite previously initialized mocked objects.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="args">Use the constructor that contains parameters with the specified argument types.</param>
		public void InitializeMockFor<T>(params object[] args) where T : class
		{
			var type = typeof(T);
			InitializeMockFor(type, args);
		}

		/// <summary>
		/// Initializes new instances for all the constructor parameters required for the specified service type.
		/// This will not overwrite previously initialized mocked objects.
		/// </summary>
		/// <param name="type">The type you want to initialize mocks for.</param>
		/// <param name="args">Use the constructor that contains parameters with the specified argument types.</param>
		public void InitializeMockFor(Type type, params object[] args)
		{
			if (type.IsInterface)
			{
				if (!Mocked.ContainsKey(type))
				{
					// Create an instance of the generic Mock of the constructor parameter type.
					var gt = typeof(Mock<>).MakeGenericType(type);
					var mock = (Mock)Activator.CreateInstance(gt);
					Mocked.Add(type, mock);
				}
				//throw new InvalidOperationException("Cannot initialize a mock for an interface.");
			}
			else
			{
				// get the constructor params for the specified service type.
				var cis = type.GetConstructors();

				// Find the constructor with the specified parameters.
				var ci = GetConstructorContaining(type, args.Select(a => a.GetType()).ToArray());

				// We must find an appropriate constructor.
				if (ci == null)
				{
					if (!Mocked.ContainsKey(type))
					{
						// Create an instance of the generic Mock of the constructor parameter type.
						var gt = typeof(Mock<>).MakeGenericType(type);
						var mock = (Mock)Activator.CreateInstance(gt);
						Mocked.Add(type, mock);
					}
					//throw new InvalidOperationException($"A constructor for '{type.Name}' with the specified argument types does not exist.");
				}
				else
				{
					InitializeMockFor(type, ci.GetParameters(), args);
				}
			}
		}

		/// <summary>
		/// Initializes new instances for all the constructor parameters required for the specified type.
		/// This will not overwrite previously initialized mocked objects.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="constructorParameters"></param>
		/// <param name="args">Use the constructor that contains parameters with the specified argument types.</param>
		private void InitializeMockFor(Type type, ParameterInfo[] constructorParameters, params object[] args)
		{
			if (!MockedParameters.ContainsKey(type))
			{
				// Get the constructor params for the specified service type.
				MockedParameters.AddOrUpdate(type, constructorParameters, (key, oldValue) => constructorParameters);
			}

			var arg_types = args?.Select(a => a.GetType());

			// Initialize the service type with the specified constructor.
			foreach (var pi in MockedParameters[type])
			{
				if (pi.ParameterType.IsValueType) continue;

				// Create an instance of the generic Mock of the constructor parameter type.
				var gt = typeof(Mock<>).MakeGenericType(pi.ParameterType);

				// Service constructor parameter has already been mocked.
				// If an argument provided matches the constructor arguments use it as a mocked value.
				// Don't mock HttpContextBase, it needs to be setup manually.
				if (Mocked.ContainsKey(pi.ParameterType))
				{
					var arg = args.FirstOrDefault(a => a.GetType() == pi.ParameterType);
					if (arg != null)
					{
						if (arg.GetType() == gt)
							Mocked.Add(pi.ParameterType, (Mock)arg);
						else
						{
							try
							{
								Mocked.Add(pi.ParameterType, Mock.Get(arg));
							}
							catch
							{
								// Ignore all errors, we just won't mock these arguments.
							}
						}
					}
					continue;
				}
				else if (pi.ParameterType == typeof(HttpContextBase))
					continue;

				Mocked.Add(pi.ParameterType, (Mock)Activator.CreateInstance(gt));
			}
		}
		#endregion

		/// <summary>
		/// Get the constructor containing all of the specified argument types.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="argTypes"></param>
		/// <returns></returns>
		private ConstructorInfo GetConstructorContaining(Type type, Type[] argTypes)
		{
			// get the constructor params for the specified service type.
			var cis = type.GetConstructors();
			var arg_types = argTypes?.Select(a => a.FullName.StartsWith("Castle.Proxies.") ? a.BaseType : a).ToArray();
			return cis.FirstOrDefault(c => argTypes.All(t => c.GetParameters().Any(pi => pi.ParameterType == t)))
				?? cis.FirstOrDefault(c => c.GetParameters().Count() == argTypes.Count());
		}

		#region Get Mock
		/// <summary>
		/// Get the mocked object for the specified type.
		/// If it doesn't exist it will create a new mock fo the specified type and add it to this collection.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="createIfNotExist"></param>
		/// <returns></returns>
		public Mock<T> GetMock<T>(bool createIfNotExist = true) where T : class
		{
			if (!Mocked.ContainsKey(typeof(T)))
			{
				if (!createIfNotExist)
					return null;

				Mocked[typeof(T)] = new Mock<T>();
			}

			return Mocked[typeof(T)] as Mock<T>;
		}

		/// <summary>
		/// Get the mocked object for the specified type.
		/// If it doesn't exist it will create a new mock fo the specified type and add it to this collection.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="createIfNotExist"></param>
		/// <returns></returns>
		public Mock GetMock(Type type, bool createIfNotExist = true)
		{
			if (!Mocked.ContainsKey(type))
			{
				if (!createIfNotExist)
					return null;

				// Create an instance of the generic Mock of the constructor parameter type.
				var gt = typeof(Mock<>).MakeGenericType(type);

				Mocked[type] = (Mock)Activator.CreateInstance(gt);
			}

			return Mocked[type];
		}

		/// <summary>
		/// Get the correct 'Object' property from the mocked object.
		/// </summary>
		/// <param name="mock"></param>
		/// <returns></returns>
		private static object GetMockObject(object mock)
		{
			var type = mock.GetType();
			var object_type = type.GetGenericArguments().First();
			var mock_object = type.GetProperties().Where(p => p.Name == "Object" && p.PropertyType == object_type).First();

			try
			{ 
				return mock_object.GetValue(mock);
			}
			catch 
			{
				var helper = new MoqHelper();
				var mocked = helper.CreateMock(object_type);
				return mocked.Object;
			}
		}
		#endregion

		#region Set Mock
		/// <summary>
		/// Initialize a mocked object and place it in the dictionary.
		/// This will overwrite a previously initialized mock.
		/// This requires a concrete class, not an interface.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="mock"></param>
		/// <param name="overwrite"></param>
		public Mock<T> SetMock<T>(Mock<T> mock, bool overwrite = true) where T : class
		{
			if (!Mocked.ContainsKey(typeof(T)) || overwrite)
				Mocked[typeof(T)] = mock;

			return mock;
		}

		/// <summary>
		/// Initialize a mocked object and place it in the dictionary.
		/// This will overwrite a previously initialized mock.
		/// This requires a concrete class, not an interface.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="args"></param>
		/// <returns>A new instance of a Mock of type T.</returns>
		public Mock<T> SetMock<T>(params object[] args) where T : class
		{
			var mock = CreateMock<T>(args);
			SetMock(mock);
			return mock;
		}

		/// <summary>
		/// Initialize a mocked object and place it in the dictionary.
		/// This will overwrite a previously initialized mock.
		/// This requires a concrete class, not an interface.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TInterface"></typeparam>
		/// <param name="args"></param>
		/// <returns></returns>
		public Mock<TInterface> SetMockAs<T, TInterface>(params object[] args)
			where T : class
			where TInterface : class
		{
			var mock = CreateMock<T>(args).As<TInterface>();
			SetMock(mock);
			return mock;
		}
		#endregion

		#region Create
		/// <summary>
		/// Creates a new instance of <typeparamref name="T"/> and initializes it with either the args specified or the mocked objects in this helper.
		/// This always uses the constructor that was initialized for this service to create the object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="args"></param>
		/// <returns>A new instance of <typeparamref name="T"/> object.</returns>
		public T Create<T>(params object[] args) where T : class
		{
			var type = typeof(T);

			// We're only interested in types we've initialized for.
			if (!MockedParameters.ContainsKey(type))
			{
				this.InitializeMockFor<T>(args);
			}

			// Get constructor parameters for T.
			var ordered_args = new List<object>();

			foreach (var pi in MockedParameters[type])
			{
				var arg_match = args.FirstOrDefault(a => a.GetType() == pi.ParameterType || a.GetType().GetGenericArguments().FirstOrDefault() == pi.ParameterType);

				// If the args have a type that matches the constructor parameter use it.
				// Otherwise use the mocked objects.
				if (arg_match != null)
				{
					var arg_type = arg_match.GetType();
					if (arg_type.IsGenericType && arg_type.GetGenericTypeDefinition().BaseType.GetGenericTypeDefinition() == typeof(Mock<>))
						ordered_args.Add(GetMockObject(arg_match));
					else
						// If a Mocked object we need to use the Object property.
						ordered_args.Add(arg_match);
				}
				else
				{
					ordered_args.Add(GetMockObject(GetMock(pi.ParameterType)));
				}
			}

			return (T)Activator.CreateInstance(type, ordered_args.ToArray());
		}

		/// <summary>
		/// Create a new mock for the specified type and also mock all required constructor parameters.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TInterface"></typeparam>
		/// <param name="args"></param>
		/// <returns></returns>
		public Mock<TInterface> CreateMockAs<T, TInterface>(params object[] args)
			where T : class
			where TInterface : class
		{
			return this.SetMockAs<T, TInterface>(args);
		}

		/// <summary>
		/// Create a new mock for the specified type and also mock all required constructor parameters.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="args"></param>
		/// <returns></returns>
		public Mock<T> CreateMock<T>(params object[] args) where T : class
		{
			var type = typeof(T);

			return (Mock<T>)CreateMock(type, args);
		}

		/// <summary>
		/// Create a new mock for the specified type and also mock all required constructor parameters.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public Mock CreateMock(Type type, params object[] args)
		{
			// Create an instance of the generic Mock of the constructor parameter type.
			var gt = typeof(Mock<>).MakeGenericType(type);

			// Initialize and mock all required constructor parameters.
			if (!MockedParameters.ContainsKey(type))
				InitializeMockFor(type, args);

			if (!type.IsInterface && MockedParameters.ContainsKey(type))
			{
				var pis = MockedParameters[type];

				var ordered_args = new List<object>();

				foreach (var pi in pis)
				{
					var supplied_arg = args.FirstOrDefault(a => a.GetType() == pi.ParameterType);

					if (supplied_arg != null)
						ordered_args.Add(supplied_arg);
					else
						ordered_args.Add(GetMockObject(GetMock(pi.ParameterType)));
				}

				var mock = (Mock)Activator.CreateInstance(gt, ordered_args.ToArray());
				mock.CallBase = true;
				return mock;
			}

			return this.GetMock(type);
		}
		#endregion
		#endregion
	}
}
