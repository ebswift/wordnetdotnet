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
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;

namespace Razor.Wizards
{
	/// <summary>
	/// Defines the various types of WizardButtons
	/// </summary>	
	public enum WizardButtons
	{	
		None,

		/// <summary>
		/// Displays "Help" on the button
		/// </summary>
		Help,

		/// <summary>
		/// Displays "Back" on the button
		/// </summary>
		Back,

		/// <summary>
		/// Displays "Next" on the button
		/// </summary>
		Next,

		/// <summary>
		/// Displays "Finish" on the button
		/// </summary>
		Finish,

		/// <summary>
		/// Displays "Cancel" on the button
		/// </summary>
		Cancel		
	}

	/// <summary>
	/// Defines the style, visibility, and layout applied to a WizardButton
	/// </summary>
	public class WizardButtonStyle
	{
		protected WizardButtons _button;
		protected bool _visible;
		protected bool _enabled;
		
		/// <summary>
		/// Initializes a new instance of the WizardButtonStyle class
		/// </summary>
		/// <param name="button">The button this style applies to</param>
		/// <param name="visible">A flag that controls the button's visibility</param>
		/// <param name="enabled">A flag that controls the button's layout</param>
		public WizardButtonStyle(WizardButtons button, bool visible, bool enabled)
		{
			_button = button;
			_visible = visible;
			_enabled = enabled;
		}

		/// <summary>
		/// Returns the button that this style applies to
		/// </summary>
		public WizardButtons Button
		{
			get
			{
				return _button;
			}
		}

		/// <summary>
		/// Returns whether the button is visible or invisible
		/// </summary>
		public bool Visible
		{
			get
			{
				return _visible;
			}
		}

		/// <summary>
		/// Returns whether the button is enabled or disabled
		/// </summary>
		public bool Enabled
		{
			get
			{
				return _enabled;
			}
		}
	}

	/// <summary>
	/// A strongly typed collection of WizardButtonStyles
	/// </summary>
	public class WizardButtonStyleList : CollectionBase
	{
		public WizardButtonStyleList()
		{

		}

		public void Add(WizardButtonStyle buttonStyle)
		{
			if (!this.Contains(buttonStyle))
				base.InnerList.Add(buttonStyle);
		}

		public void Remove(WizardButtonStyle buttonStyle)
		{
			if (this.Contains(buttonStyle))
				base.InnerList.Remove(buttonStyle);
		}

		public bool Contains(WizardButtonStyle buttonStyle)
		{
			foreach(WizardButtonStyle bs in base.InnerList)
				if (bs.Button == buttonStyle.Button)						
					return true;
			return false;
		}

		public WizardButtonStyle this[WizardButtons button]
		{
			get
			{
				foreach(WizardButtonStyle bs in base.InnerList)
					if (bs.Button == button)						
						return bs;
				return null;
			}
		}
	}
}
