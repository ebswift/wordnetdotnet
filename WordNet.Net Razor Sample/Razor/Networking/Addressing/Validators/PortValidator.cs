using System;
using System.Diagnostics;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Summary description for PortValidator.
	/// </summary>
	public class PortValidator
	{
		public const int MinPortValue = 0;
		public const int MaxPortValue = 65535;
		
		public static bool IsValid(int port)
		{
			if (port < MinPortValue || port > MaxPortValue)
				return false;
			return true;
		}

		public static int Parse(string format)
		{
			int port = 0;
			try
			{
				port = Int32.Parse(format);				
			}
			catch(ArgumentNullException)
			{
				throw new PortFormatException(format);
			}
			catch(FormatException)
			{
				throw new PortFormatException(format);
			}
			return port;
		}
	}
}
