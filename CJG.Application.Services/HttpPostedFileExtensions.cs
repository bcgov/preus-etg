﻿using CJG.Application.Business.Models;
using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	public static class HttpPostedFileExtensions
	{
		/// <summary>
		/// Validated the uploaded file to see if it is not too big or of the wrong file type.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="maxBytes"></param>
		/// <param name="allowedExtensions"></param>
		/// <returns></returns>
		public static HttpPostedFileBase Validate(this HttpPostedFileBase file, int maxBytes = 0, string[] allowedExtensions = null)
		{
			if (maxBytes > 0 && file.ContentLength > maxBytes)
				throw new InvalidOperationException($"The maximum size of a file you can upload is '{(int)(maxBytes / (1024 * 1024))}MB'.");

			var fileExtension = Path.GetExtension(file.FileName);
			if (allowedExtensions != null && !allowedExtensions.Contains(fileExtension, StringComparer.InvariantCultureIgnoreCase))
				throw new InvalidOperationException($"You cannot upload a file of type '{fileExtension}'.");

			return file;
		}

		public static void CreateNewVersion(this Attachment attachment, HttpPostedFileBase file, string description, string permittedFileTypesKey = "PermittedAttachmentTypes", IEnumerable<string> bypassExtraTypes = null)
		{
			var result = file.UploadFile(description, file.FileName, permittedFileTypesKey: permittedFileTypesKey, bypassExtraTypes: bypassExtraTypes);
			attachment.CreateNewVersion(result.FileName, result.Description, result.FileExtension, result.AttachmentData);
		}

		public static Attachment UploadFile(this HttpPostedFileBase file, string description, string newFileName = "", string permittedFileTypesKey = "PermittedAttachmentTypes", IEnumerable<string> bypassExtraTypes = null)
		{
			int maxUploadSize = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);
			string[] permittedAttachmentTypes = ConfigurationManager.AppSettings[permittedFileTypesKey].Split('|');

			using (var memoryStream = new MemoryStream())
			{
				file.Validate(maxUploadSize, permittedAttachmentTypes).InputStream.CopyTo(memoryStream);

				var fileName = string.IsNullOrEmpty(newFileName) ? Path.GetFileNameWithoutExtension(file.FileName) : newFileName;
				var fileExtension = Path.GetExtension(file.FileName);
				if (IsValidFileType(memoryStream, fileExtension, bypassExtraTypes))
					return new Attachment(fileName, description, fileExtension, memoryStream.ToArray());

				if (file.ContentLength < 17)
					throw new InvalidOperationException("The file type is not valid.");

				throw new InvalidOperationException($"The file does not match the extension type '{fileExtension}'.");
			}
		}

		public static Tuple<string, bool, Attachment> UploadPostedFile(this HttpPostedFileBase applicationDocument, string description)
		{
			return Tuple.Create("", false, applicationDocument.UploadFile(description));
		}

		public static Tuple<string, bool, Attachment> UploadPostedFile(this HttpPostedFileBase applicationDocument, string fileDescription, string fileName)
		{
			try
			{
				return Tuple.Create("", false, applicationDocument.UploadFile(fileDescription, fileName));
			}
			catch (InvalidOperationException ex)
			{
				return Tuple.Create(ex.Message, true, default(Attachment));
			}
		}

		public static Attachment UploadPostedFile(this HttpPostedFileBase applicationDocument, AttachmentModel attachment)
		{
			return applicationDocument.UploadFile(attachment.Description, attachment.Name);
		}

		private static bool IsValidFileType(MemoryStream memoryStream, string fileExtension, IEnumerable<string> bypassExtraTypes = null)
		{
			var isValidFileType = false;

			// first check for JPG/JPEG
			using (var reader = new BinaryReader(memoryStream))
			{
				reader.BaseStream.Position = 0x0;

				var bypassFileTypes = new List<string> { ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg" };
				if (bypassExtraTypes != null)
					bypassFileTypes.AddRange(bypassExtraTypes);

				if (bypassFileTypes.Contains(fileExtension, StringComparer.InvariantCultureIgnoreCase))
					return true;

				// the file needs to be a minimum of 16 bytes in length, to accommodate the file type signatures
				if (reader.BaseStream.Length >= 16)
				{
					var soi = reader.ReadUInt16();  // Start of Image (SOI) marker (FFD8)
					var marker = reader.ReadUInt16(); // JFIF marker (FFE0) or EXIF marker(FF01)

					isValidFileType = soi == 0xd8ff && (marker & 0xe0ff) == 0xe0ff;

					if (!isValidFileType)
					{
						string[] fileTypeSignatureHexValues =
						{
							"25-50-44-46",  // PDF
							"89-50-4E-47-0D-0A-1A-0A", // PNG
							"47-49-46-38-39-61", // GIF
							"47-49-46-38-37-61", // GIF
							//"50-4B-03-04", // DOCX/XLSX
							//"D0-CF-11-E0-A1-B1-1A-E1" // DOC/XLS
						};

						reader.BaseStream.Position = 0x0; // the offset you are reading the data from  
						byte[] data = reader.ReadBytes(0x10); // read 16 bytes into an array  
						var dataAsHex = BitConverter.ToString(data);

						// get a substring long enough to accommodate the signature for a PNG
						var fileSignatureHexValue = dataAsHex.Substring(0, 23);
						var fileTypeSignatureCounter = 0;

						foreach (var fileTypeSignatureHexValue in fileTypeSignatureHexValues)
						{
							if (fileSignatureHexValue.StartsWith(fileTypeSignatureHexValue))
							{
								isValidFileType = FileTypeMatchesExtension(fileTypeSignatureCounter, fileExtension.ToLower());
								break;
							}

							fileTypeSignatureCounter++;
						}
					}
					else
					{
						isValidFileType = new string[] { ".jpg", ".jpeg" }.Contains(fileExtension, StringComparer.InvariantCultureIgnoreCase);
					}
				}
			}

			return isValidFileType;
		}

		private static bool FileTypeMatchesExtension(int fileTypeSignatureCounter, string fileExtension)
		{
			switch (fileTypeSignatureCounter)
			{
				case 0:
					return fileExtension == ".pdf";
				case 1:
					return fileExtension == ".png";
				case 2:
				case 3:
					return fileExtension == ".gif";
				default:
					return false;
			}
		}
	}
}
