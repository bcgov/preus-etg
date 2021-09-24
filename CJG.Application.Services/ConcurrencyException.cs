using CJG.Core.Entities;
using System;
using System.Runtime.Serialization;

namespace CJG.Application.Services
{
	[Serializable]
	public class ConcurrencyException : Exception
	{
		public const string DefaultMessage = "<strong>\"{0}\"</strong> action is no longer valid "
			+ "for the current grant file state of <strong>\"{1}\".</strong> "
			+ "The state may have been changed by actions of another user";

		public ConcurrencyException() : base("The state may have been changed by actions of another user.")
		{
		}

		public ConcurrencyException(ApplicationWorkflowTrigger action, ApplicationStateExternal state) : base(String.Format(DefaultMessage, action.GetDescription(), state.GetDescription()))
		{

		}

		public ConcurrencyException(ApplicationWorkflowTrigger action, ApplicationStateInternal state) : base(String.Format(DefaultMessage, action.GetDescription(), state.GetDescription()))
		{

		}

		public ConcurrencyException(string action, ApplicationStateInternal state) : base(String.Format(DefaultMessage, action, state.GetDescription()))
		{

		}

		public ConcurrencyException(string message) : base(message)
		{
		}

		public ConcurrencyException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected ConcurrencyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		#region Methods
		public static string GetHtmlMessage(string action, ApplicationStateInternal state)
		{

			return String.Format(ConcurrencyException.DefaultMessage, action, state.GetDescription());
		}

		public static string GetHtmlMessage(ApplicationWorkflowTrigger action, ApplicationStateExternal state)
		{

			return String.Format(ConcurrencyException.DefaultMessage, action.GetDescription(), state.GetDescription());
		}

		public static string GetHtmlMessage(ApplicationWorkflowTrigger action, ApplicationStateInternal state)
		{

			return String.Format(ConcurrencyException.DefaultMessage, action.GetDescription(), state.GetDescription());
		}

		public static string GetMessage(ApplicationWorkflowTrigger action, ApplicationStateExternal state)
		{

			return GetHtmlMessage(action, state)
				.Replace("<strong>", String.Empty)
				.Replace("</strong>", String.Empty);
		}

		public static string GetMessage(ApplicationWorkflowTrigger action, ApplicationStateInternal state)
		{

			return GetHtmlMessage(action, state)
				.Replace("<strong>", String.Empty)
				.Replace("</strong>", String.Empty);
		}
		#endregion
	}
}