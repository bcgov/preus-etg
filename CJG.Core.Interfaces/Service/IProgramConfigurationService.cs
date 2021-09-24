using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface IProgramConfigurationService: IService
	{
		ProgramConfiguration Get(int id);
		IEnumerable<ProgramConfiguration> GetAll();
		IEnumerable<ProgramConfiguration> GetAll(bool isActive);
		void Delete(ProgramConfiguration programConfiguration);
		void Remove(ProgramConfiguration programConfiguration);
		ProgramConfiguration Generate(GrantStream grantStream);

		void SyncWDAService(ProgramConfiguration programConfiguration);
	}
}
