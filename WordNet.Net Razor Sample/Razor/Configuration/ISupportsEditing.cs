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

namespace Razor.Configuration
{
	// ps i still think i'm spelling that wrong, i think it might be Changable instead of including the e...
	/// <summary>
	/// Defines the actions that can be taken when changes are applied from one ISupportsEditing to another
	/// </summary>
	public enum SupportedEditingActions
	{
		/// <summary>
		/// No editing should occur
		/// </summary>
		None = 0,

		/// <summary>
		/// Existing elements will be overwritten with the incoming data
		/// </summary>
		Overwrite = 1,

		/// <summary>
		/// Elements that exist in the incoming element, but not in the existing element, will be added to the existing element
		/// </summary>
		Add = 2,

		/// <summary>
		/// Elements that exist in the existing element, but not in the incoming element, will be removed from the existing element
		/// </summary>
		Remove = 4,

		/// <summary>
		/// The existing element will clone the incoming element and become an identical copy of the incoming element
		/// </summary>
		Synchronize = SupportedEditingActions.Overwrite | SupportedEditingActions.Add | SupportedEditingActions.Remove
	}	

	/// <summary>
	/// Defines methods and properties to be implemented by any object that can have changes, and apply changes to itself as a configuration member.
	/// </summary>
	public interface ISupportsEditing
	{
		/// <summary>
		/// Occurs when BeginEdit is called
		/// </summary>
		event XmlConfigurationElementCancelEventHandler BeforeEdit;

		/// <summary>
		/// Occurs when EndEdit is called
		/// </summary>
		event XmlConfigurationElementEventHandler AfterEdit;

		/// <summary>
		/// Occurs when CancelEdit is called
		/// </summary>
		event XmlConfigurationElementEventHandler EditCancelled;

		/// <summary>
		/// Gets a flag that indicates whether the object is currently being edited
		/// </summary>
		bool IsBeingEdited { get; }

		/// <summary>
		/// Places the object into edit mode. All changes to this object will be held until EndEdit is called, all events are suspending while the object is being edited)
		/// </summary>
		bool BeginEdit();

		/// <summary>
		/// Removes the object from edit mode. All changes made since entering edit mode will be applied, and events will be fired.
		/// </summary>
		bool EndEdit();

		/// <summary>
		/// Removes the object from edit mode. All changes made since entering edit mode will be discarded.
		/// </summary>
		bool CancelEdit();

		/// <summary>
		/// Raises the BeforeEdit event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnBeforeEdit(object sender, XmlConfigurationElementCancelEventArgs e);

		/// <summary>
		/// Raises the AfterEdit event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnAfterEdit(object sender, XmlConfigurationElementEventArgs e);

		/// <summary>
		/// Raises the CancelEdit event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnEditCancelled(object sender, XmlConfigurationElementEventArgs e);

		/// <summary>
		/// Deletes all subscriptions to the BeforeEdit multicast delegate
		/// </summary>
		void ResetBeforeEdit();
		
		/// <summary>
		/// Deletes all subscriptions to the AfterEdit multicast delegate
		/// </summary>
		void ResetAfterEdit();

		/// <summary>
		/// Deletes all subscriptions to the CancelEdit multicast delegate
		/// </summary>
		void ResetEditCancelled();

		/// <summary>
		/// Gets whether the object currently has changes
		/// </summary>
		bool HasChanges { get; set; }

		/// <summary>
		/// Accepts the current changes and forces HasChanges to return false until more changes have been incurred on this object
		/// </summary>
		void AcceptChanges();

		/// <summary>
		/// Applies the values of the incoming object that have changes to the values of the object implementing the ISupportsEditing interface.
		/// </summary>
		/// <param name="editableObject"></param>
		bool ApplyChanges(ISupportsEditing editableObject, SupportedEditingActions actions); 

		bool ApplyToSelf(ISupportsEditing editableObject, SupportedEditingActions actions);
	}
}
