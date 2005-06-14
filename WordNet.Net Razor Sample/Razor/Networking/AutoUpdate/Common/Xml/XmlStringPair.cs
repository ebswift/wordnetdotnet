using System;

namespace Razor.Networking.AutoUpdate.Common.Xml
{
	/// <summary>
	/// Summary description for XmlStringPair.
	/// </summary>
	internal class XmlStringPair
	{
		protected string _name;
		protected string _value;

		/// <summary>
		/// Initializes a new instance of the XmlStringPair class
		/// </summary>
		public XmlStringPair()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the XmlStringPair class
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public XmlStringPair(string name, string value)
		{
			_name = name;
			_value = value;
		}
        
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}
	}
}
