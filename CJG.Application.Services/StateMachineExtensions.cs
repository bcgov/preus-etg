using CJG.Core.Entities;
using static Stateless.StateMachine<CJG.Core.Entities.ApplicationStateInternal, CJG.Core.Entities.ApplicationWorkflowTrigger>;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="StateMachineExtensions"/> static class, provides extension methods for <typeparamref name="StateMachine"/>.
	/// </summary>
	public static class StateMachineExtensions
    {
        /// <summary>
        /// Adds all permitted triggers for the currently specified <typeparamref name="ApplicationStateInternal"/>.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static StateConfiguration Permits(this StateConfiguration config, ApplicationStateInternal state)
        {
            foreach (var trigger in config.State.GetValidWorkflowTriggers())
            {
                var resulting_state = state.GetResultingState(trigger);
                if (config.State != resulting_state)
                    config.Permit(trigger, resulting_state);
                else
                    config.PermitReentry(trigger);
            }

            return config;
        }
    }
}
