using System;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Summary description for AddressValidator.
	/// </summary>
	public class AddressValidator
	{
		public static bool IsValid(string address)
		{
			if (address == null || address == string.Empty)
				return false;
			return true;
		}
	}
}
