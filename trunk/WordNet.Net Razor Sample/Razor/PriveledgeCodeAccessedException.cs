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

namespace Razor
{
	/// <summary>
	/// Summary description for PriveledgeCodeAccessedException.
	/// </summary>
	public class PriveledgeCodeAccessedException : System.Exception 
	{
		private Type[] _typesAllowed;
		private string _message;

		public PriveledgeCodeAccessedException(Type[] typesAllowed) : base()
		{
			_typesAllowed = typesAllowed;
			System.Diagnostics.StackFrame sf = new System.Diagnostics.StackFrame(1);
			if (sf != null)
			{
				System.Reflection.MethodBase mb = sf.GetMethod();
				if (mb != null)
				{
					_message = "The code calling '" + mb.Name + "' does not have permission to execute '" + mb.Name + "'. Only the Type(s) listed as follows have been granted access to this method or property: ";
					bool isFirst = true;
					foreach(Type t in _typesAllowed)
					{
						_message += (isFirst == false ? ", " : null) + t.FullName;
						isFirst = false;
					}
				}
			}
		}

		public Type[] TypesAllowed
		{
			get
			{
				return _typesAllowed;
			}
		}

		public override string Message
		{
			get
			{
				return _message;
			}
		}
	}
}
