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
using System.IO;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using Razor.Configuration;

namespace Razor.Tracing
{	
	/// <summary>
	/// Defines the Style of the headers
	/// </summary>
	public enum InstanceTraceListenerHeaderStyles
	{
		Default,
		Full,
		Minimal,
		Custom
	}

	/// <summary>
	/// Defines the various headers that can be included with trace statements
	/// </summary>
	[Flags()]
	public enum InstanceTraceListenerHeaders
	{
		[Description("Time")]
		DateAndTime = 1,

		[Description("File")]
		Filename	= 2,
		
		[Description("Line")]
		LineNumber	= 4,

		[Description("Nspc")]
		NameSpace	= 8,

		[Description("Type")]
		TypeName	= 16,

		[Description("Mthd")]
		MethodName	= 32	
	}

	/// <summary>
	/// Summary description for FormattedTextWriterTraceListener.
	/// </summary>
	public class FormattedTextWriterTraceListener : TextWriterTraceListener
	{
		private InstanceTraceListenerHeaders _headers = FormattedTextWriterTraceListener.DefaultHeaders;
		private static Hashtable _headerDescriptions;
		private static Array _headerValues;
		private int _stackFramesToSkip = FormattedTextWriterTraceListener.DefaultNumberOfStackFramesToSkip;
		private bool _trimPathFromFile = false;
		private bool _trimNamespaceFromType = false;
		private string _flag = "*";
		private bool _hasWrittenDescriptor = false;

		#region Constructors

		static FormattedTextWriterTraceListener()
		{
			
			_headerDescriptions = new Hashtable();

			// cache the descriptions for the headers, we don't want to do this everytime something logs
			// so do it once, and dynamically so that if the enum changes we don't have to recode things
			_headerValues = Enum.GetValues(typeof(InstanceTraceListenerHeaders));
			foreach(object header in _headerValues)
			{
				string description = EnumHelper.GetEnumValueDescription(header, typeof(InstanceTraceListenerHeaders));
				_headerDescriptions.Add(header.ToString(), description);
			}
		}

		public FormattedTextWriterTraceListener() : base()
		{
		}

		public FormattedTextWriterTraceListener(Stream stream) : base(stream)
		{
		}

		public FormattedTextWriterTraceListener(string path) : base(path)
		{
		}

		public FormattedTextWriterTraceListener(Stream stream, string name) : base(stream, name)
		{
		}

		public FormattedTextWriterTraceListener(string path, string name) : base(path, name)
		{
		}

		public FormattedTextWriterTraceListener(TextWriter writer, string name) : base(writer, name)
		{
		}

		#endregion

		#region Overrides

		public override void WriteLine(object o)
		{
			if (o != null)
			{
				bool flagEntry = false;
				Type t = o.GetType();				 
				if (t != null)
					if (t.IsSubclassOf(typeof(System.Exception)))
						flagEntry = true;

				this.WriteHeaders(flagEntry);			
				base.WriteLine(o.ToString());
//				base.Flush();
			}
		}

//		public override void WriteLine(object o, string category)
//		{
//			this.WriteHeaders();
//			base.WriteLine(o, category);
//		}

		public override void WriteLine(string message)
		{
			this.WriteHeaders(false);
			base.WriteLine(message);
//			base.Flush();
		}

//		public override void WriteLine(string message, string category)
//		{
//			this.WriteHeaders();
//			base.WriteLine(message, category);
//		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets a masked set of values indicating the default combination of headers. (File, Line, Type, Method)
		/// </summary>
		public static InstanceTraceListenerHeaders DefaultHeaders
		{
			get
			{
				return (InstanceTraceListenerHeaders.Filename	|	
					InstanceTraceListenerHeaders.LineNumber | 
					//						InstanceTraceListenerHeaders.NameSpace	|
					InstanceTraceListenerHeaders.TypeName	| 
					InstanceTraceListenerHeaders.MethodName	);
			}
		}

		/// <summary>
		/// Gets a masked set of values indicating all of the headers
		/// </summary>
		public static InstanceTraceListenerHeaders FullHeaders
		{
			get
			{
				return (InstanceTraceListenerHeaders.DateAndTime|
					InstanceTraceListenerHeaders.Filename	|	
					InstanceTraceListenerHeaders.LineNumber | 
					InstanceTraceListenerHeaders.NameSpace	|
					InstanceTraceListenerHeaders.TypeName	| 
					InstanceTraceListenerHeaders.MethodName	);
			}
		}

		/// <summary>
		/// Gets a masked set of values indicating the minimal combination of headers. (File, Line)
		/// </summary>
		public static InstanceTraceListenerHeaders MinimalHeaders
		{
			get
			{
				return (InstanceTraceListenerHeaders.Filename	|	
					InstanceTraceListenerHeaders.LineNumber);
			}
		}

		/// <summary>
		/// Gets the default number of stack frames to skip to acquire the valid frame in which the caller executed the trace. This will need to skip over all of our calls.
		/// </summary>
		public static int DefaultNumberOfStackFramesToSkip
		{
			get
			{
				return 4;
			}
		}

		/// <summary>
		/// Gets the current masked set of values indicating which headers will be included with each trace.
		/// </summary>
		public InstanceTraceListenerHeaders Headers
		{
			get
			{
				return _headers;
			}
			set
			{
				_headers = 0;
				_headers = value;
			}
		}

		/// <summary>
		/// The number of stack frames to skip to hit our code base and align with the correct methods. Modifications to source of trace listener class will directly affect this property. Use the call stack and autos window to align this offset when breakage occurs.
		/// </summary>
		public int StackFramesToSkip
		{
			get
			{
				return _stackFramesToSkip;
			}
			set
			{
				_stackFramesToSkip = value;
			}
		}
		
		/// <summary>
		/// Gets or sets a flag indicating whether the path will be trimmed to just the filename
		/// </summary>
		public bool TrimPathFromFile
		{
			get
			{
				return _trimPathFromFile;
			}
			set
			{
				_trimPathFromFile = value;
			}
		}

		/// <summary>
		/// Gets or sets a flag indicating whether the namespace will be trimmed to just the typename
		/// </summary>
		public bool TrimNamespaceFromType
		{
			get
			{
				return _trimNamespaceFromType;
			}
			set
			{
				_trimNamespaceFromType = value;
			}	
		}

		/// <summary>
		/// Gets or sets the flag used when an entry is flagged during a write operation
		/// </summary>
		public string Flag
		{
			get
			{
				return _flag;
			}
			set
			{
				_flag = value;
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Writes a header line for each header flag that is specified in Headers
		/// </summary>
		protected virtual void WriteHeaders(bool flagEntry)
		{						
			StackFrame frame = new StackFrame(_stackFramesToSkip, true);	
		
			if (!_hasWrittenDescriptor)
			{
				_hasWrittenDescriptor = true;
				base.WriteLine("#==============================================================================="); 
				base.WriteLine("# Log file starting..."); 
				base.WriteLine("# This file contains diagnostic information."); 
				base.WriteLine("# Please do not edit this file."); 
				base.WriteLine(string.Format("# {0} at {1}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString()));
				base.WriteLine("#===============================================================================");  
			}

			base.WriteLine(string.Empty);
			foreach(object header in _headerValues)
			{
				if (FlagsHelper.IsFlagSet((int)_headers, (int)header))
				{
					string headerName = this.GetHeaderDescription((InstanceTraceListenerHeaders)header);
					string headerValue = this.GetHeaderValue((InstanceTraceListenerHeaders)header, frame);
					string line = this.FormatHeaderLine(flagEntry, headerName, headerValue);
					base.WriteLine(line);
				}
			}
			base.Write(this.GetFlag(flagEntry)/* + ">" */);
		}

		protected virtual string GetFlag(bool flagged)
		{
			return (flagged ? _flag : null);
		}

		/// <summary>
		/// Retrieves the description for a value from the cached descriptions
		/// </summary>
		/// <param name="header"></param>
		/// <returns></returns>
		private string GetHeaderDescription(InstanceTraceListenerHeaders header)
		{
			return (string)_headerDescriptions[header.ToString()];
		}

		/// <summary>
		/// Retrieves a specific value for the INDIVIDUAL header value. NOTE: Do not pass a flag combination here. It will break if you do!
		/// </summary>
		/// <param name="header"></param>
		/// <returns></returns>
		protected virtual string GetHeaderValue(InstanceTraceListenerHeaders header, StackFrame frame)
		{
			try
			{
				switch(header)
				{
				case InstanceTraceListenerHeaders.DateAndTime:
					return DateTime.Now.ToString();

				case InstanceTraceListenerHeaders.Filename:
					if (_trimPathFromFile)
						return System.IO.Path.GetFileName(frame.GetFileName());
					return frame.GetFileName();

				case InstanceTraceListenerHeaders.LineNumber:
					int line = frame.GetFileLineNumber() - 1;
					return line.ToString();

				case InstanceTraceListenerHeaders.NameSpace:
					return frame.GetMethod().DeclaringType.Namespace;

				case InstanceTraceListenerHeaders.TypeName:
					if (_trimNamespaceFromType)
						return frame.GetMethod().DeclaringType.Name;
					return frame.GetMethod().DeclaringType.FullName;
					
				case InstanceTraceListenerHeaders.MethodName:
					return "." + frame.GetMethod().Name + "()";
				};
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Debug.WriteLine(systemException);
			}
			return null;
		}

		/// <summary>
		/// Formats a line into a [Name(Value)] + newline format
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual string FormatHeaderLine(bool flagEntry, string name, string value)
		{
			// return string.Format("{0}[{1}({2})]", this.GetFlag(flagEntry), name, value);
			return string.Format("{0}${1} = \"{2}\"", this.GetFlag(flagEntry), name, value);
		}

		#endregion
	}
}
