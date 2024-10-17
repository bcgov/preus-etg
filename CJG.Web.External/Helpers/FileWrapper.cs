using System.IO;

namespace CJG.Web.External.Helpers
{
	public class FileWrapper : IFileWrapper
	{
		public bool Exists(string path)
		{
			return File.Exists(path);
		}
	}

	public interface IFileWrapper
	{
		bool Exists(string path);
	}
}
