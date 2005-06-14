using System;
using System.Collections;
using System.Text;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Defines a class that can parse an array of bytes into tokens which can then be converted into strings.
	/// </summary>
	internal class HttpByteParser 
	{
		protected byte[] _bytes;
		protected int _index;

		/// <summary>
		/// Initializes a new instance of the HttpByteParser class
		/// </summary>
		public HttpByteParser()
		{
			_bytes = new byte[0];
		}

		/// <summary>
		/// Initializes a new instance of the HttpByteParser class
		/// </summary>
		/// <param name="bytes"></param>
		public HttpByteParser(byte[] bytes) 
		{
			_bytes = bytes;			
		}

		/// <summary>
		/// Returns the internal byte array
		/// </summary>
		public byte[] Bytes
		{
			get
			{
				return _bytes;
			}
		}

		/// <summary>
		/// Returns the index of the parser
		/// </summary>
		public int Index 
		{
			get 
			{ 
				return _index; 
			}
			set
			{
				_index = value;
			}
		}

		/// <summary>
		/// Sets the internal byte array used to parse string lines
		/// </summary>
		/// <param name="bytes"></param>
		public void SetBuffer(byte[] bytes)
		{
			_bytes = bytes;
		}

		

		/// <summary>
		/// Tries to parse out a line of text from the underlying byte array by searching for '\r\n' combinations.
		/// </summary>
		/// <returns></returns>
		public HttpByteParserToken GetNextToken() 
		{
			HttpByteParserToken line = null;
			// Start searching at our last index, and work our way towards the end of our bytes (|| until we strike a valid CRLF combination)						
			for (int i = _index; i < _bytes.Length; i++) 
			{
				// if the current byte is the LF char
				if (_bytes[i] == (byte)'\n') 
				{
					// try backing up and seeing if the one before was a CR char (the length of it will be our current position - where we started
					int length = i - _index;
					
					// if there is a non-zero length, and the char matches (back the length up one more so that we do not include the CR char
					if (length > 0 && _bytes[i-1] == (byte)'\r')
						length--;					

					// now that we have a line (and we have possibly excluded the '\r' from the line using the code above :P let's construct a new line around these bytes
					line = new HttpByteParserToken(_bytes, _index, length);					

                    // now push the index ahead so that we're sitting ahead of the '\n' that we stopped on
					_index = i+1;

					// return the object
					return line;
				}
			}
			
			return null;
		}
	}
}
