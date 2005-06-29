using System;

namespace Razor.Networking.AutoUpdate.Common
{
	/// <summary>
	/// Summary description for AutoUpdateHref.
	/// </summary>
	public class AutoUpdateHref
	{
		protected string _text;
		protected string _href;		

		public AutoUpdateHref()
		{
			
		}

		public AutoUpdateHref(string text, string href)
		{
			_text = text;
			_href = href;
		}

		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		public string Href
		{
			get
			{
				return _href;
			}
			set
			{
				_href = value;
			}
		}
	}
}
