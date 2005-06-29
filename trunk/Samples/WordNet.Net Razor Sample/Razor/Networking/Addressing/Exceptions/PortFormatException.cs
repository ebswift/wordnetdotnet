using System;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Summary description for PortFormatException.
	/// </summary>
	public class PortFormatException : FormatException
	{
		protected string _format;

		/// <summary>
		/// Initializes a new instance of the PortFormatException class
		/// </summary>
		/// <param name="format">The format to parse the port from</param>
		public PortFormatException(string format) : base(string.Format("The format of '{0}' is not valid. Acceptable characters are 0-9, no characters or punctuation.", format))
		{
			_format = format;
        }

		public string Format
		{
			get
			{
				return _format;
			}
		}
	}
}
