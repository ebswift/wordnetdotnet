using System;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Represents an error when a name is not unique in a given context
	/// </summary>
	public class NameNotUniqueException : Exception
	{
		protected string _name;

		/// <summary>
		/// Initializes a new instance of the NameNotUniqueException class
		/// </summary>
		/// <param name="address"></param>
		public NameNotUniqueException(string name) : base("The name '" + name + "' is not unique.")
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
