using Autofac;
using CJG.Core.Interfaces.Service;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace CJG.Web.External.Helpers
{
    /// <summary>
    /// <typeparamref name="RedirectOnAttribute"/> attribute class, provides a way to control what happens when the exception of the specified type occurs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RedirectOnAttribute : Attribute
    {
        #region Variables
        #endregion

        #region Properties
        public Type ExceptionType { get; }
        public string Action { get; } = "Index";
        public string Controller { get; } = "Home";
        public AlertType AlertType { get; set; } = AlertType.Warning;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="RedirectOnAttribute"/> class.
        /// </summary>
        /// <param name="exceptionType"></param>
        public RedirectOnAttribute(Type exceptionType)
        {
            this.ExceptionType = exceptionType;
        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="RedirectOnAttribute"/> class.
        /// </summary>
        /// <param name="exceptionType"></param>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        public RedirectOnAttribute(Type exceptionType, string action, string controller) : this(exceptionType)
        {
            this.Action = action;
            this.Controller = controller;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a <typeparamref name="RedirectToRouteResult"/> object and initializes it with appropriate route data.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ActionResult RedirectResult(ExceptionContext context)
        {
            var grantApplicationId = context.RouteData.Values["grantApplicationId"].ToInt();
            var trainingProgramId = context.RouteData.Values["trainingProgramId"].ToInt();

            if (!grantApplicationId.HasValue && trainingProgramId.HasValue)
            {
                var container = DependencyResolver.Current.GetAutofacContainer();
                var service = container.Resolve<Func<ITrainingProgramService>>()();

                var trainingProgram = service.Get(trainingProgramId.Value);

                if (trainingProgram != null)
                    grantApplicationId = trainingProgram.GrantApplicationId;
            }

            var routeValues = new RouteValueDictionary
            {
                { "action", this.Action },
                { "controller", this.Controller },
                { "grantApplicationId", grantApplicationId },
                { "trainingProgramId", trainingProgramId },
                { "id", context.RouteData.Values["id"].ToInt() }
            };
            return new RedirectToRouteResult(routeValues);
        }
        #endregion
    }
}