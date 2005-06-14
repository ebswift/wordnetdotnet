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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

using Razor.Attributes;
using Razor.Features;

namespace Razor.SnapIns
{
	/// <summary>
	/// Summary description for SnapInControl.
	/// </summary>
	[SnapInVisibility(false)]
	public class SnapInControl : System.Windows.Forms.UserControl, ISnapIn
	{		
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initializes a new instance of the SnapInControl class. The default constructor Windows Forms Designer.
		/// </summary>
		public SnapInControl()
		{			
			this.InitializeComponent();
		}

//		/// <summary>
//		/// Initializes a new instance of the SnapInControl class. The default constructor for the Hosting Engine.
//		/// </summary>
//		/// <param name="hostingEngine">The hosting engine that will own this instance</param>
//		public SnapInControl(SnapInHostingEngine hostingEngine) : this()
//		{
//			_hostingEngine = hostingEngine;			
//		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	
		#region Suggested Overridable Methods
		
		protected virtual void RestartMyServices()
		{
			try
			{
				this.StopMyServices();
				this.StartMyServices();
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		protected virtual void StartMyServices()
		{

		}

		protected virtual void StopMyServices()
		{

		}

		protected virtual void InstallMyCommonOptions() 
		{
			
		}

		protected virtual void InstallMyLocalUserOptions() 
		{
			
		}
		
		protected virtual void ReadMyCommonOptions()
		{

		}

		protected virtual void ReadMyLocalUserOptions()
		{

		}

		protected virtual void WriteMyCommonOptions()
		{

		}

		protected virtual void WriteMyLocalUserOptions()
		{

		}

		protected virtual void UninstallMyCommonOptions()
		{

		}

		protected virtual void UninstallMyLocalUserOptions()
		{

		}

		protected virtual void UpgradeMyCommonOptions()
		{

		}

		protected virtual void UpgradeMyLocalUserOptions()
		{

		}

		#endregion

		#region ISnapIn Members

		//		/// <summary>
		//		/// Gets or sets the State of the SnapIn. This can be a combination of 0 or more flags from the SnapInStates flags enumeration. Only the SnapInHostingEngine has permission to set this property.
		//		/// </summary>
		//		public SnapInStates State
		//		{
		//			get
		//			{
		//				return _state;
		//			}
		//			set
		//			{
		//				StackFrame sf = new StackFrame(1);
		//				if (sf != null)
		//				{
		//					MethodBase mb = sf.GetMethod();
		//					if (mb != null)
		//					{
		//						if (mb.DeclaringType != typeof(SnapInHostingEngine))
		//						{
		//							throw new PriveledgeCodeAccessedException(new Type[] {typeof(SnapInHostingEngine)});
		//						}
		//					}
		//				}
		//				_state = value;
		//			}
		//		}

		/// <summary>
		/// Fires when common options should be installed
		/// </summary>
		public event EventHandler InstallCommonOptions;

		/// <summary>
		/// Fires when local user options should be installed
		/// </summary>
		public event EventHandler InstallLocalUserOptions;

		/// <summary>
		/// Fires when common options should be upgraded
		/// </summary>
		public event EventHandler UpgradeCommonOptions;

		/// <summary>
		/// Fires when local user options should be upgraded
		/// </summary>
		public event EventHandler UpgradeLocalUserOptions;

		/// <summary>
		/// Fires when common options should be read
		/// </summary>
		public event EventHandler ReadCommonOptions;

		/// <summary>
		/// Fires when local user options should be read
		/// </summary>
		public event EventHandler ReadLocalUserOptions;

		/// <summary>
		/// Fires when common options should be written
		/// </summary>
		public event EventHandler WriteCommonOptions;

		/// <summary>
		/// Fires when local user options should be written
		/// </summary>
		public event EventHandler WriteLocalUserOptions;

		/// <summary>
		/// Fires when common options should be uninstalled
		/// </summary>
		public event EventHandler UninstallCommonOptions;

		/// <summary>
		/// Fires when local user options should be uninstalled
		/// </summary>
		public event EventHandler UninstallLocalUserOptions;

		/// <summary>
		/// Fires when the SnapIn should try ang start. May fire multiple times, depending upon the return result.
		/// </summary>
		public event EventHandler Start;

		/// <summary>
		/// Fires when the SnapIn should try and stop. May fire multiple times, depending upon the return result.
		/// </summary>
		public event EventHandler Stop;

		/// <summary>
		/// Raises the InstallCommonOptions Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnInstallCommonOptions(object sender, EventArgs e)
		{
			try
			{
				if (this.InstallCommonOptions != null)
					this.InstallCommonOptions(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Raises the InstallLocalUserOptions Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnInstallLocalUserOptions(object sender, EventArgs e)
		{
			try
			{
				if (this.InstallLocalUserOptions != null)
					this.InstallLocalUserOptions(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Raises the UpgradeCommonOptions Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnUpgradeCommonOptions(object sender, EventArgs e)
		{
			try
			{
				if (this.UpgradeCommonOptions != null)
					this.UpgradeCommonOptions(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Raises the UpgradeLocalUserOptions Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnUpgradeLocalUserOptions(object sender, EventArgs e)
		{
			try
			{
				if (this.UpgradeLocalUserOptions != null)
					this.UpgradeLocalUserOptions(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Raises the ReadCommonOptions Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnReadCommonOptions(object sender, EventArgs e)
		{
			try
			{
				if (this.ReadCommonOptions != null)
					this.ReadCommonOptions(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Raises the ReadLocalUserOptions Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnReadLocalUserOptions(object sender, EventArgs e)
		{
			try
			{
				if (this.ReadLocalUserOptions != null)
					this.ReadLocalUserOptions(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Raises the WriteCommonOptions Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnWriteCommonOptions(object sender, EventArgs e)
		{
			try
			{
				if (this.WriteCommonOptions != null)
					this.WriteCommonOptions(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Raises the WriteLocalUserOptions Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnWriteLocalUserOptions(object sender, EventArgs e)
		{
			try
			{
				if (this.WriteLocalUserOptions != null)
					this.WriteLocalUserOptions(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Raises the UninstallCommonOptions Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnUninstallCommonOptions(object sender, EventArgs e)
		{
			try
			{
				if (this.UninstallCommonOptions != null)
					this.UninstallCommonOptions(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Raises the UninstallLocalUserOptions Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnUninstallLocalUserOptions(object sender, EventArgs e)
		{
			try
			{
				if (this.UninstallLocalUserOptions != null)
					this.UninstallLocalUserOptions(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Raises the Start Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnStart(object sender, EventArgs e)
		{
			try
			{
				if (this.Start != null)
					this.Start(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Raises the Stop Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnStop(object sender, EventArgs e)
		{
			try
			{
				if (this.Stop != null)
					this.Stop(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		#endregion
	}
}
