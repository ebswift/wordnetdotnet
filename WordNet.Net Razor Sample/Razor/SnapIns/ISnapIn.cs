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
using System.Security.Cryptography;
using Razor.Features;

namespace Razor.SnapIns
{		
	/// <summary>
	/// Defines the methods and events of a SnapIn class used by the SnapInHostingEngine class
	/// </summary>
	public interface ISnapIn 
	{		
		event EventHandler InstallCommonOptions;
		event EventHandler InstallLocalUserOptions;
		event EventHandler UpgradeCommonOptions;
		event EventHandler UpgradeLocalUserOptions;
		event EventHandler ReadCommonOptions;
		event EventHandler ReadLocalUserOptions;
		event EventHandler WriteCommonOptions;
		event EventHandler WriteLocalUserOptions;		
		event EventHandler UninstallCommonOptions;
		event EventHandler UninstallLocalUserOptions;
		event EventHandler Start;
		event EventHandler Stop;	
				
		void OnInstallCommonOptions(object sender, System.EventArgs e);		
		void OnInstallLocalUserOptions(object sender, System.EventArgs e);
		void OnUpgradeCommonOptions(object sender, System.EventArgs e);
		void OnUpgradeLocalUserOptions(object sender, System.EventArgs e);
		void OnReadCommonOptions(object sender, System.EventArgs e);
		void OnReadLocalUserOptions(object sender, System.EventArgs e);
		void OnWriteCommonOptions(object sender, System.EventArgs e);
		void OnWriteLocalUserOptions(object sender, System.EventArgs e);
		void OnUninstallCommonOptions(object sender, System.EventArgs e);		
		void OnUninstallLocalUserOptions(object sender, System.EventArgs e);
		void OnStart(object sender, System.EventArgs e);
		void OnStop(object sender, System.EventArgs e);						
	}
}
