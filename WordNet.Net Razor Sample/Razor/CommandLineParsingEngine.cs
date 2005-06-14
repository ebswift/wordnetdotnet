/*
 * This file is a part of the Razor Framework.
 * 
 * Copyright (C) 2003 Mark (Code6) Belles 
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 * */

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Razor
{
	/// <summary>
	/// Provides methods for parsing a command line argument set, into a collection of name/value pairs using a variety of switches and combinations
	/// </summary>
	public class CommandLineParsingEngine : IEnumerable
	{
		private StringDictionary _arguments;

		/// <summary>
		/// Initializes a new instance of the CommandLineParsingEngine class
		/// </summary>
		public CommandLineParsingEngine()
		{			
		}

		/// <summary>
		/// Initializes a new instance of the CommandLineParsingEngine class
		/// </summary>
		/// <param name="args">A command line argument set to parse</param>
		public CommandLineParsingEngine(string[] args)
		{
			this.Parse(args);
		}
		
		#region Implementation of IEnumerable
		
		public System.Collections.IEnumerator GetEnumerator()
		{
			return _arguments.GetEnumerator();
		}
		
		#endregion

		/// <summary>
		/// Parses the command line argument set into a collection of name/value pairs
		/// </summary>
		/// <param name="args"></param>
		public void Parse(string[] args)
		{
			_arguments = new StringDictionary();

			Regex Spliter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			Regex Remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

			string param = null;
			string[] paramElements;

			// Valid parameters forms:
			// {-,/,--}param{ ,=,:}((",')value(",'))
			// Examples: -param1 value1 --param2 /param3:"Test-:-work" /param4=happy -param5 '--=nice=--'
			foreach(string arg in args)
			{
				// Look for new parameters (-,/ or --) and a possible enclosed value (=,:)
				paramElements = Spliter.Split(arg, 3);

				switch(paramElements.Length)
				{
					// Found a value (for the last parameter found (space separator))
				case 1:

					if (param!=null)
					{
						if(!_arguments.ContainsKey(param))
						{
							paramElements[0] = Remover.Replace(paramElements[0],"$1");
							_arguments.Add(param,paramElements[0]);
						}
						param = null;
					}
					// else Error: no parameter waiting for a value (skipped)
					break;
					// Found just a parameter

				case 2:
					// The last parameter is still waiting. With no value, set it to true.
					if (param!=null)
					{
						if (!_arguments.ContainsKey(param)) _arguments.Add(param,"true");
					}
					param = paramElements[1];
					break;
					// param with enclosed value

				case 3:
					// The last parameter is still waiting. With no value, set it to true.
					if (param!=null)
					{
						if (!_arguments.ContainsKey(param)) _arguments.Add(param,"true");
					}
					param = paramElements[1];
					// Remove possible enclosing characters (",')
					if (!_arguments.ContainsKey(param))
					{
						paramElements[2] = Remover.Replace(paramElements[2],"$1");
						_arguments.Add(param,paramElements[2]);
					}
					param = null;
					break;
				}
			}
			// In case a parameter is still waiting
			if (param!=null)
			{
				if (!_arguments.ContainsKey(param)) _arguments.Add(param,"true");
			}
		}

		/// <summary>
		/// indexer for referencing command line parameters by a named key
		/// </summary>
		public string this [string paramName]
		{
			get
			{
				return this.ToString(paramName);
			}
		}			
	
		public bool Exists(string paramName)
		{
			return (_arguments[paramName] == null ? false : true);			
		}

		public string ToString(string paramName)
		{
			if (this.Exists(paramName))
                return _arguments[paramName].ToString();
			return string.Empty;
		}

		public bool ToBoolean(string paramName)
		{
			if (this.Exists(paramName))
                return Convert.ToBoolean(_arguments[paramName]);
			return false;
		}

		public Int16 ToInt16(string paramName)
		{
			if (this.Exists(paramName))
				return Convert.ToInt16(_arguments[paramName]);
			return 0;
		}

		public Int32 ToInt32(string paramName)
		{
			if (this.Exists(paramName))
				return Convert.ToInt32(_arguments[paramName]);
			return 0;
		}

		public Int64 ToInt64(string paramName)
		{
			if (this.Exists(paramName))
				return Convert.ToInt64(_arguments[paramName]);
			return 0;
		}

		public Single ToSingle(string paramName)
		{
			if (this.Exists(paramName))
				return Convert.ToSingle(_arguments[paramName]);
			return 0;
		}
	}
}
