using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface ICipsCodesService : IService
    {
        IEnumerable<ClassificationOfInstructionalProgram> GetAll();
		ClassificationOfInstructionalProgram Get(int id);
		IEnumerable<ClassificationOfInstructionalProgram> GetListOfCipsCodes(int id);
		ClassificationOfInstructionalProgram Add(ClassificationOfInstructionalProgram cipsCode);
		ClassificationOfInstructionalProgram Update(ClassificationOfInstructionalProgram cipsCode);
		bool ParentIdExists(int? parentId);
		IEnumerable<ClassificationOfInstructionalProgram> GetCipsCodeChildren(int parentId = 0, int level = 0);
	}
}
