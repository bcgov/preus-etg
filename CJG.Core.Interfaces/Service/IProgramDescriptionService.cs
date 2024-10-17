using System;
using CJG.Core.Entities;
using System.Collections.Generic;
using System.Linq.Expressions;
using CJG.Application.Business.Models;

namespace CJG.Core.Interfaces.Service
{
    public interface IProgramDescriptionService : IService
    {
        void Add(ProgramDescription programDescription);
        void Update(ProgramDescription programDescription);
		void ClearApplicationNaics(Guid userBCeID, int orgdId);
	}
}
