using System;
using System.Diagnostics;
using System.Collections;
using System.Text;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Summary description for HttpHeaderList.
	/// </summary>
	[Serializable()]
	public class HttpHeaderList : CollectionBase
	{
		public HttpHeaderList()
		{
			
		}

		public void Add(HttpHeader header)
		{
			if (this.Contains(header))
				throw new HttpHeaderAlreadyExistsException(header);

			base.InnerList.Add(header);
		}

		public void AddRange(params HttpHeader[] headers)
		{
			if (headers == null)
				throw new ArgumentNullException("headers");

			foreach(HttpHeader header in headers)
				this.Add(header);
		}

		public void Remove(HttpHeader header)
		{
			if (this.Contains(header))
				base.InnerList.Remove(header);
		}

		public void Remove(string headerName)
		{
			for(int i = 0; i < base.InnerList.Count; i++)
			{
				HttpHeader header = (HttpHeader)base.InnerList[i];
				if (string.Compare(header.Name, headerName, true) == 0)
				{
					base.InnerList.RemoveAt(i);
					return;
				}
			}
		}

		public new void RemoveAt(int index)
		{
			base.InnerList.RemoveAt(index);			
		}

		public bool Contains(HttpHeader header)
		{
			if (header == null)
				throw new ArgumentNullException("header");

			return this.Contains(header.Name);
		}

		public bool Contains(string headerName)
		{
			foreach(HttpHeader header in base.InnerList)
				if (string.Compare(header.Name, headerName, true) == 0)
					return true;
			return false;
		}

		public HttpHeader this[int index]
		{
			get
			{
				return (HttpHeader)base.InnerList[index];
			}
		}

		public HttpHeader this[string headerName]
		{
			get
			{
				foreach(HttpHeader header in base.InnerList)
					if (string.Compare(header.Name, headerName, true) == 0)
						return header;			
				return null;
			}
		}

		public string[][] GetUnknownHeaders()
		{
			int unknownHeaders = 0;
			for(int i = 0; i < base.InnerList.Count; i++)
				if (!this[i].IsKnownHeader)
					unknownHeaders++;

			string[][] headers = new string[unknownHeaders][];

			for(int i = 0; i < unknownHeaders; i++)
			{
				HttpHeader hdr = this[i];
				if (!hdr.IsKnownHeader)
				{
					string[] header = hdr.ToArray();
					headers[i] = new string[2];
					headers[i][0] = header[0];
					headers[i][1] = header[1];
				}
			}
		
			return headers;			
		}
        
		public string[][] ToArray()
		{
			string[][] headers = new string[base.InnerList.Count][];
			
			for(int i = 0; i < base.InnerList.Count; i++)
			{
				HttpHeader hdr = this[i];
				string[] header = hdr.ToArray();
				headers[i] = new string[2];
				headers[i][0] = header[0];
				headers[i][1] = header[1];
			}
			
			return headers;
		}

		public override string ToString()
		{			
			StringBuilder sb = new StringBuilder();

			// append each header
			foreach(HttpHeader header in base.InnerList)
			{				
				if (!HttpUtils.IsEmptryString(header.Value))
					sb.Append(header.ToString());
			}

			// wrap up the headers with another crlf combo
			sb.Append(HttpControlChars.CRLF);
			
			return sb.ToString();
		}
	}

	#region HttpHeaderAlreadyExistsException

	/// <summary>
	/// Defines an exception that is throw when a header is added to a list of headers that is already in the list
	/// </summary>
	public class HttpHeaderAlreadyExistsException : Exception 
	{
		protected HttpHeader _header;

		public HttpHeaderAlreadyExistsException(HttpHeader header) : base(string.Format("A header with the name '{0}' already exists.", header.Name))
		{
			_header = header;
		}

		public HttpHeader Header
		{
			get
			{
				return _header;
			}
		}
	}

	#endregion
}
