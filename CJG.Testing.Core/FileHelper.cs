using Moq;
using System;
using System.IO;
using System.Linq;
using System.Web;

namespace CJG.Testing.Core
{
	public static class FileHelper
	{
		#region Methods
		/// <summary>
		/// Creates a mocked HttpPostedFileBase with fake PDF data.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="hexData"></param>
		/// <returns></returns>
		public static HttpPostedFileBase CreateFile(string fileName = "FakeFile.pdf", string hexData = "2550444625504446255044462550444625504446255044462550444625504446")
		{
			var data = StringToByteArray(hexData);
			var stream = new MemoryStream(data);

			var mockFile = new Mock<HttpPostedFileBase>();
			mockFile.Setup(m => m.FileName).Returns(fileName);
			mockFile.Setup(m => m.ContentLength).Returns(data.Length);
			mockFile.Setup(m => m.InputStream).Returns(stream);

			return mockFile.Object;
		}

		/// <summary>
		/// Converts a string into a byte array.
		/// </summary>
		/// <param name="hex"></param>
		/// <returns></returns>
		private static byte[] StringToByteArray(string hex)
		{
			return Enumerable.Range(0, hex.Length)
				.Where(x => x % 2 == 0)
				.Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
				.ToArray();
		}
		#endregion
	}
}
