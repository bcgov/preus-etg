using CJG.Core.Entities;

namespace CJG.Core.Interfaces.Service
{
    public interface IGrantOpeningManageScheduledService : IService
    {
        void ManageStateTransitions(int fiscalYearId);

        void ManageStateTransition(GrantOpening grantOpening);
    }
}
