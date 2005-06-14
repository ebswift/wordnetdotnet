using System;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Summary description for AddressBookDirectoryImporterException.
	/// </summary>
	public class AddressBookDirectoryImporterException : Exception
	{
		protected string _filename;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="innerException"></param>
		public AddressBookDirectoryImporterException(string filename, Exception innerException) : base(string.Format("The file '{0}' could not be imported. It may be corrupt, or it may not be a valid Address Book Directory file.", filename))
		{
			_filename = filename;
		}

		/// <summary>
		/// Returns the full path to the file that caused the exception
		/// </summary>
		public string Filename
		{
			get
			{
				return _filename;
			}
		}
	}


}
