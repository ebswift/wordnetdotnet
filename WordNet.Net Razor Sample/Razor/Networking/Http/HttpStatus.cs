using System;

namespace Razor.Networking.Http
{
/// <summary>
	/// Provides a way of combining status-codes with reason-phrases
	/// </summary>
	[Serializable()]
	public class HttpStatus
	{
		protected int _code;
		protected string _reason;	

		/// <summary>
		/// Returns a string in the format of 'Code SP Reason'
		/// </summary>
		public const string STRING_FORMAT = "{0} {1}";

		/// <summary>
		/// Initializes a new instance of the HttpStatus class
		/// </summary>
		/// <param name="code"></param>
		/// <param name="reason"></param>
		public HttpStatus(int code, string reason)
		{
			_code = code;
			_reason = reason;				
		}

		/// <summary>
		/// Returns the status-code
		/// </summary>
		public int Code
		{
			get
			{
				return _code;
			}
			set
			{
				_code = value;
			}
		}

		/// <summary>
		/// Returns the reason-phrase
		/// </summary>
		public string Reason
		{
			get
			{
				return _reason;
			}
			set
			{
				_reason = value;
			}
		}

		/// <summary>
		/// Returns a string in the format of 'Code SP Reason'
		/// </summary>
		public override string ToString()
		{
			return string.Format(STRING_FORMAT, _code, _reason);
		}

		/// <summary>
		/// Parses a string in the format of 'Code SP Reason' into an HttpStatus instance
		/// </summary>
		/// <param name="value">The string to parse. May include the CRLF.</param>
		/// <returns></returns>
		public static HttpStatus Parse(string value)
		{
			value = HttpUtils.StripCRLF(value);
			value = HttpUtils.TrimLeadingAndTrailingSpaces(value);
			
			string code = null;
			string reason = null;
			int indexOfSP = value.IndexOf(' ');
			code = value.Substring(0, indexOfSP);
			reason = value.Substring(++indexOfSP);

			return new HttpStatus(int.Parse(code), reason);
		}
	}
}
