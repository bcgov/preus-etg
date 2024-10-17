using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
    public interface IParticipantEmploymentStatusService : IService
    {
        IEnumerable<ParticipantEmploymentStatus> GetParticipantEmploymentStatuses(int[] ids);
    }
}