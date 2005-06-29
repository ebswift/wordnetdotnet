using System;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Summary description for HttpChunkSizeLine.
	/// </summary>
	internal class HttpChunkSizeLine
	{
		protected int _size;
		protected string _extension;

		public const string STRING_FORMAT = "{0}\r\n";
		public const string EXT_STRING_FORMAT = "; {0}";

		/// <summary>
		/// Initializes a new instance of the HttpChunkSizeLine class
		/// </summary>
		/// <param name="size"></param>
		/// <param name="extension"></param>
		public HttpChunkSizeLine(int size, string extension)
		{
			_size = size;
			_extension = extension;
		}

		/// <summary>
		/// Gets or sets the size of the chunk
		/// </summary>
		public int Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}
		}

		/// <summary>
		/// Gets or sets the extension (A name/value pair delimited by a '=' separator) of the chunk
		/// </summary>
		public string Extension
		{
			get
			{
				return _extension;
			}
			set
			{	
				_extension = value;
			}
		}

		/// <summary>
		/// Determines if the chunk is empty, if so then it is the last chunk, optional trailers may follow
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return (_size == 0);
			}
		}

		/// <summary>
		/// Returns a flag that indicates whether this chunk has an extension present or not
		/// </summary>
		public bool HasExtension
		{
			get
			{
				return (_extension != null && _extension != string.Empty);
			}
		}

		/// <summary>
		/// Returns the extension formatted for a chunk (Adds a leading ';' and space to the 'Name=Value' format)
		/// </summary>
		public string ExtensionFormated
		{
			get
			{
				// if the extension is empty there's not a lot to format...
				if (HttpUtils.IsEmptryString(this.Extension))
					return string.Empty;

				return string.Format(EXT_STRING_FORMAT, this.Extension);
			}
		}

		/// <summary>
		/// Returns a string in the format '0xHEX [";" ext-name=ext-value]'
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(
				STRING_FORMAT, _size.ToString("X"), 
				(this.HasExtension ? string.Format(EXT_STRING_FORMAT, _extension) : null));
		}

		/// <summary>
		/// Parses a chunk's size line into an HttpChunkSizeLine instance
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static HttpChunkSizeLine Parse(string value)
		{
			// strip the CRLF from each line
			value = HttpUtils.StripCRLF(value);

			int size = 0;
			string extension = null;

			// if the size line includes a ';' then there is also an extension present
			int indexOfSep = value.IndexOf(';');
			if (indexOfSep > 0)
			{
				// split the line on the ';'
				string szSize = value.Substring(0, indexOfSep);
				extension = value.Substring(++indexOfSep);

				szSize = HttpUtils.TrimLeadingAndTrailingSpaces(szSize);
				extension = HttpUtils.TrimLeadingAndTrailingSpaces(extension);

				// parse the size and extension from the line
				size = int.Parse(szSize, System.Globalization.NumberStyles.HexNumber);
			}
			else
			{
				// no extension because a lack of the ';' separator, just parse the size
				value = HttpUtils.TrimLeadingAndTrailingSpaces(value);
				size = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
			}
			
			// return a new chunk instance
			return new HttpChunkSizeLine(size, extension);
		}
	}
}
