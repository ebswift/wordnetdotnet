using System;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Represents an error when a name is not valid in a given context
	/// </summary>
	public class NameNotValidException : Exception
	{
		protected string _name;

		/// <summary>
		/// Initializes a new instances of the NameNotUniqueException class
		/// </summary>
		/// <param name="name"></param>
		public NameNotValidException(string name) : base("The new name '" + name + "' is either blank or contains reserved characters which are not escaped.\nBlank names are not allowed.\nReserved characters include ;/?:@&=+$,\nIf reserved characters must be included they must be encoded according to the % hex hex format described in RFC 2396: URI Generic Syntax.")
		{
			_name = name;
		}

		/// <summary>
		/// Returns the name in question
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}			
	}
}
