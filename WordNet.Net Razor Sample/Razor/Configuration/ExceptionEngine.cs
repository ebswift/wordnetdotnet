/*
 * This file is a part of the Razor Framework.
 * 
 * Copyright (C) 2004 Mark (Code6) Belles 
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
using System.Windows.Forms;

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for ExceptionEngine.
	/// </summary>
	public class ExceptionEngine
	{
		/// <summary>
		/// Displays a message box containing information about an exception, for the currently executing application, plus optional additional information.
		/// </summary>
		/// <param name="owner">The window owning the dialog</param>
		/// <param name="caption">The caption to display on the dialog</param>
		/// <param name="icon">The icon to display on the dialog</param>
		/// <param name="buttons">The buttons to display on the dialog</param>
		/// <param name="systemException">The exception to display on the dialog</param>
		/// <param name="infoLines">Optional additional information to display on the dialog</param>
		/// <returns>The result of the dialog</returns>
		public static DialogResult DisplayException(IWin32Window owner, string caption, MessageBoxIcon icon, MessageBoxButtons buttons, System.Exception systemException, params string[] infoLines)
		{
			bool hasAdditionalInfo = false;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			/// begin with the application information that generated the exception
			sb.Append(string.Format("The application '{0}' has encountered the following exception or condition.\n\n", Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)));
			
			/// append the additional information if any was supplied
			if (infoLines != null)
			{
				hasAdditionalInfo = true;
				sb.Append("Additional Information:\n\n");
				foreach(string line in infoLines)
					sb.Append(string.Format("{0}\n", line));
			}
						
			if (systemException != null)
			{
				/// append the information contained in the exception
				sb.Append(string.Format("{0}Exception Information:\n\n", (hasAdditionalInfo ? "\n" : null)));
				sb.Append(systemException.ToString());
			}

			/// display a message and return the result
			return MessageBox.Show(owner, sb.ToString(), caption, buttons, icon);
		}
	}
}
