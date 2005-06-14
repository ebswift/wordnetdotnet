using System;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Provides means of validating names (Any named item that will translate to a work area in the Razor Interface)
	/// </summary>
	public class NameValidator
	{
		public static char[] InvalidCharacters = new char[] { ';', '/', '?', ':', '@', '&', '=', '+', '$', ','};

		public static bool IsNullOrBlank(string name)
		{
			if (name == null || name == string.Empty)
				return true;

			return false;
		}

		/// <summary>
		/// Determines if a name is valid (not null, empty, or containing invalid characters
		/// </summary>
		/// <param name="name">The name to validate</param>
		/// <returns></returns>
		public static bool IsValid(string name)
		{
			if (NameValidator.IsNullOrBlank(name))
				return false;
			
			// add support for the invalid characters
			if (name.IndexOfAny(InvalidCharacters) > -1)
				return false;



			return true;
		}
	}
}
