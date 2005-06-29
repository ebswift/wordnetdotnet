using System;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Summary description for AddressNotValidException.
	/// </summary>
	public class AddressNotValidException : Exception
	{
		protected string _address;
		public static char[] InvalidCharacters = new char[] 
			{
				'\'',
				'~',
				'!',
				'@',
				'#',
				'$',
				'%',
				'^',
				'&',
				'*',
				'(',
				')',
				'=',
				'+',
				'[',
				']',
				'{',
				'}',
				'\\',
				'|',
				';',
				':',
				',',
				'"',
				'<',
				'>',
				'/',
				'?'
			};

		public static string GetInvalidCharacters()
		{
			string s = null;
			foreach(char ch in InvalidCharacters)
				s += ch.ToString();
			return s;
		}

		public AddressNotValidException(string address) : base("The new address '" + address + "' is either blank or contains characters which are not allowed.\n\nCharacters which are not allowed include " + AddressNotValidException.GetInvalidCharacters())
		{
			_address = address;			
		}

		public string Address
		{
			get
			{
				return _address;
			}
		}
	}
}
