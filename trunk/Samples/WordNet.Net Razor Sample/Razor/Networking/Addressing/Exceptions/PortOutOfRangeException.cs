using System;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Summary description for PortOutOfRangeException.
	/// </summary>
	public class PortOutOfRangeException : Exception
	{
		protected int _port;

		/// <summary>
		/// Initializes a new instance of the PortOutOfRangeException class
		/// </summary>
		/// <param name="port"></param>
		public PortOutOfRangeException(int port) : base(string.Format("The port number is out of range. Valid ports are {0} to {1}.", PortValidator.MinPortValue.ToString(), PortValidator.MaxPortValue.ToString()))
		{
			_port = port;
		}

		public int Port
		{
			get
			{
				return _port;
			}
		}
	}
}
