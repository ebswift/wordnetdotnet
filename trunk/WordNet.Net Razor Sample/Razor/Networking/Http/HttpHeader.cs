using System;
using System.Diagnostics;
using System.Collections;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Summary description for HttpHeader.
	/// </summary>
	[Serializable()]
	public class HttpHeader
	{		
		protected string _name;
		protected string _value;
		        
		/// <summary>
		/// Returns a string in the format 'message-header = token'
		/// </summary>
		public const string STRING_FORMAT = "{0}: {1}{2}";

		/// <summary>
		/// Initializes a new instance of the HttpHeader class
		/// </summary>
		/// <param name="name">The name of the header</param>
		/// <param name="value">The value of the header</param>
		public HttpHeader(string name, string value)
		{			
			this.Name = name;
			this.Value = value;
		}

		/// <summary>
		/// Gets or sets the name of the header
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				HttpUtils.ValidateToken(@"Name", value);
				value = HttpUtils.TrimLeadingAndTrailingSpaces(value);
				_name = value;
			}
		}

		/// <summary>
		/// Gets or sets the value of the header
		/// </summary>
		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				HttpUtils.ValidateToken(@"Value", value);
				value = HttpUtils.TrimLeadingAndTrailingSpaces(value);
				_value = value;
			}
		}

		/// <summary>
		/// Determines if this is a known header
		/// </summary>
		public bool IsKnownHeader
		{
			get
			{
				return (System.Web.HttpWorkerRequest.GetKnownRequestHeaderIndex(_name) != -1);
			}
		}

		/// <summary>
		/// Returns the header as an array of strings like {HeaderName, HeaderValue}
		/// </summary>
		/// <returns></returns>
		public string[] ToArray()
		{
			return new string[] {_name, _value};
		}

		/// <summary>
		/// Returns a string in the format 'message-header : token'
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(STRING_FORMAT, _name, _value, HttpControlChars.CRLF);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static HttpHeader Parse(string value)
		{			
			// strip the crlf if it's there
			value = HttpUtils.StripCRLF(value);

			// split it on the header token
			string[] parts = value.Split(':');

			// trim the leading and trailing spaces from the fields
			string a = HttpUtils.TrimLeadingAndTrailingSpaces(parts[0]);
			string b = HttpUtils.TrimLeadingAndTrailingSpaces(parts[1]);

			// return a new header
			return new HttpHeader(a, b);
		}
	}
}
