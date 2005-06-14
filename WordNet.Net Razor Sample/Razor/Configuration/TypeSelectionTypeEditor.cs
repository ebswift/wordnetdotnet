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
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Reflection;

namespace Razor.Configuration 
{
	/// <summary>
	/// Summary description for TypeSelectionTypeEditor.
	/// </summary>
	public class TypeSelectionTypeEditor : UITypeEditor 
	{
		public TypeSelectionTypeEditor() 
		{
			//
			// TODO: Add constructor logic here
			//
		}

		//		[System.Security.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name="FullTrust")]
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) 
		{
			return UITypeEditorEditStyle.DropDown;
		}

		//		[System.Security.Per.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name="FullTrust")]
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) 
		{
			//			System.Windows.Forms.MessageBox.Show(null, context.Instance.GetType().FullName, "caption");
			XmlConfigurationOption option = context.Instance as XmlConfigurationOption;
			if (option != null) 
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null) 
				{
					TypeSelectionTypeEditorControl typeSelector = new TypeSelectionTypeEditorControl();
					typeSelector.WindowsFormsEditorService = edSvc;                
					edSvc.DropDownControl(typeSelector);
					
					try 
					{
						Type t = typeSelector.SelectedType;
						if (t != null)
						{
							option.Value = null;
							option.ValueAssemblyQualifiedName = t.AssemblyQualifiedName;
							return t.AssemblyQualifiedName;
						}

//						Assembly a = typeSelector.RefrencedAssembly;
//						if (t != null && a != null) 
//						{
//							System.IO.FileInfo f = new System.IO.FileInfo(a.Location);
//							option.Value = null;
//							option.ReferencedAssemblyName = f.Name;
//							return t.FullName;
//						}						
					}
					catch(System.Exception) {}
					return value;
				}			
			}
			return base.EditValue (context, provider, value);
		}

		
	}
}
