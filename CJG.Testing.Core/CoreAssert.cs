using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CJG.Testing.Core
{
	public static class CoreAssert
	{
		/// <summary>
		/// Asserts the specified exception was thrown.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="action"></param>
		public static Exception Throws<T>(Action action)
			where T : Exception
		{
			try
			{
				action.Invoke();
				Assert.Fail($"Exception {typeof(T).Name} was not thrown.");
			}
			catch (Exception ex)
			{
				ex.Should().BeOfType<T>();
				return ex;
			}

			return null;
		}

		/// <summary>
		/// Asserts an exception is thrown.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		public static Exception Throws(Action action)
		{
			try
			{
				action.Invoke();
				Assert.Fail($"Exception was not thrown.");
			}
			catch (Exception ex)
			{
				return ex;
			}

			return null;
		}
	}
}
