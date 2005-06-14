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

namespace Razor.Configuration 
{
	/// <summary>
	/// Summary description for XmlConfigurationOptionCollection.
	/// </summary>
	public class XmlConfigurationOptionCollection : System.Collections.CollectionBase, ICloneable, ISupportsEditing, IXmlConfigurationElementEvents, ISupportInitialize
	{
		private XmlConfigurationCategory _parent;
		private bool _hasChanges;
		protected bool _isBeingEdited = false;	
		private bool _isBeingInitialized;
		
		#region Instance Constructors

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationOptionCollection
		/// </summary>
		public XmlConfigurationOptionCollection() 
		{
		
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds the option to this collection
		/// </summary>
		/// <param name="option"></param>
		/// <returns></returns>
		public int Add(XmlConfigurationOption option) 
		{				
			if (this.Contains(option))
				throw new ArgumentException("ElementName already exists. ElementName in collection: " + option.ElementName + " ElementName being added: " + option.ElementName);
			
			option.Parent = this;
			option.BeforeEdit += new XmlConfigurationElementCancelEventHandler(this.OnBeforeEdit);
			option.Changed += new XmlConfigurationElementEventHandler(this.OnChanged);
			option.AfterEdit += new XmlConfigurationElementEventHandler(this.OnAfterEdit);
			option.EditCancelled += new XmlConfigurationElementEventHandler(this.OnEditCancelled);
			int index = base.InnerList.Add(option);
			this.OnChanged(this, new XmlConfigurationOptionEventArgs(option, XmlConfigurationElementActions.Added));			
			return index;
		}

		/// <summary>
		/// Adds the array of options to this collection
		/// </summary>
		/// <param name="options"></param>
		public void Add(XmlConfigurationOption[] options) 
		{
			if (options == null)
				throw new ArgumentNullException("options");			

			foreach(XmlConfigurationOption opt in options) 
			{
				try 
				{
					this.Add(opt);
				}
				catch(System.Exception systemException) 
				{
					System.Diagnostics.Trace.WriteLine(systemException);
				}
			}
		}

		/// <summary>
		/// Removes the option from this collection
		/// </summary>
		/// <param name="option"></param>
		public void Remove(XmlConfigurationOption option) 
		{
			if (this.Contains(option))
			{											
				option.BeforeEdit -= new XmlConfigurationElementCancelEventHandler(this.OnBeforeEdit);
				option.Changed -= new XmlConfigurationElementEventHandler(this.OnChanged);
				option.AfterEdit -= new XmlConfigurationElementEventHandler(this.OnAfterEdit);
				option.EditCancelled -= new XmlConfigurationElementEventHandler(this.OnEditCancelled);
				base.InnerList.Remove(option);
				this.OnChanged(this, new XmlConfigurationOptionEventArgs(option, XmlConfigurationElementActions.Removed));
			}
		}

		/// <summary>
		/// Determines whether an option exists using the specified option's elementName
		/// </summary>
		/// <param name="option"></param>
		/// <returns></returns>
		public bool Contains(XmlConfigurationOption option) 
		{
			foreach(XmlConfigurationOption opt in base.InnerList)
				if (opt.ElementName == option.ElementName)
					return true;
			return false;
		}				

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the option at the specified index
		/// </summary>
		[Browsable(false)]
		public XmlConfigurationOption this[int index] 
		{
			get 
			{
				return base.InnerList[index] as XmlConfigurationOption;
			}
			set 
			{
				base.InnerList[index] = value;
			}
		}

		/// <summary>
		/// Gets the option using the specified elementName
		/// </summary>
		[Browsable(false)]
		public XmlConfigurationOption this[string elementName]
		{
			get
			{
				foreach(XmlConfigurationOption opt in base.InnerList)
					if (opt.ElementName == elementName)
						return opt;
				return null;
			}
		}

		/// <summary>
		/// Gets or adds the option using the specified elementName
		/// </summary>
		[Browsable(false)]
		public XmlConfigurationOption this[string elementName, bool createIfNotFound]
		{
			get
			{	
				XmlConfigurationOption option = this[elementName];				
				
				if (option == null)
					if (createIfNotFound)
					{
						option = new XmlConfigurationOption(elementName, null);
						this.Add(option);
						option = this[elementName];
					}
				return option;			
			}
		}

		/// <summary>
		/// Gets or adds the option using the specified elementName and default value
		/// </summary>
		[Browsable(false)]
		public XmlConfigurationOption this[string elementName, bool createIfNotFound, object defaultValue]
		{
			get
			{
				XmlConfigurationOption option = this[elementName];				
				
				if (option == null)
					if (createIfNotFound)
					{
						option = new XmlConfigurationOption(elementName, defaultValue);
						this.Add(option);
						option = this[elementName];
					}
				return option;			
			}
		}

		/// <summary>
		/// Gets or sets the category of which this option collection is a child
		/// </summary>
		[Browsable(false)]
		public XmlConfigurationCategory Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		/// <summary>
		/// Gets or sets the full path to this option collection is a child
		/// </summary>
		[Browsable(false)]
		public string Fullpath
		{
			get
			{
				if (_parent == null)
					return null;
				else
					return _parent.Fullpath;				
			}
		}
		
		[Browsable(false)]
		public XmlConfiguration Configuration
		{
			get
			{
				if (_parent != null)
					return _parent.Configuration;
				return null;
			}
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			XmlConfigurationOptionCollection clone = new XmlConfigurationOptionCollection();
			clone.ResetBeforeEdit();
			clone.ResetChanged();
			clone.ResetAfterEdit();
			clone.ResetEditCancelled();

			foreach(XmlConfigurationOption option in base.InnerList)
			{
				XmlConfigurationOption clonedOption = (XmlConfigurationOption)option.Clone();
				clonedOption.Parent = clone;
				clone.Add(clonedOption);
			} 

			return clone;
		}

		#endregion

		#region ISupportsEditing Members

		public event XmlConfigurationElementCancelEventHandler BeforeEdit;
		public event XmlConfigurationElementEventHandler AfterEdit;
		public event XmlConfigurationElementEventHandler EditCancelled;

		public bool IsBeingEdited
		{
			get
			{
				return _isBeingEdited;
			}
		}

		public bool BeginEdit()
		{
			try
			{
				/// place the element in edit mode and clone ourself so that future changes will be redirected to the clone and not to ourself
				_isBeingEdited = true;

				foreach(XmlConfigurationOption option in base.InnerList)
				{
					try
					{
						option.BeginEdit();
					}
					catch(System.Exception systemException)
					{
						System.Diagnostics.Trace.WriteLine(systemException);
					}
				}
				return true;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}

		public bool EndEdit()
		{
			try
			{
				if (_isBeingEdited)
				{									
					/// place the element in edit mode and clone ourself so that future changes will be redirected to the clone and not to ourself
					_isBeingEdited = false;
					
					foreach(XmlConfigurationOption option in base.InnerList)
					{
						try
						{
							option.EndEdit();
						}
						catch(System.Exception systemException)
						{
							System.Diagnostics.Trace.WriteLine(systemException);
						}
					}
					return true;
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}

		public virtual bool CancelEdit()
		{
			try
			{
				if (_isBeingEdited)
				{
					_isBeingEdited = false;

					foreach(XmlConfigurationOption option in base.InnerList)
					{
						try
						{
							option.CancelEdit();
						}
						catch(System.Exception systemException)
						{
							System.Diagnostics.Trace.WriteLine(systemException);
						}
					}
				}
				return true;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}

		public void OnBeforeEdit(object sender, XmlConfigurationElementCancelEventArgs e)
		{
			try
			{
				EventTracing.TraceMethodAndDelegate(this, this.BeforeEdit);

				if (this.BeforeEdit != null)
					this.BeforeEdit(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		public void OnAfterEdit(object sender, XmlConfigurationElementEventArgs e)
		{
			try
			{
				EventTracing.TraceMethodAndDelegate(this, this.AfterEdit);

				if (this.AfterEdit != null)
					this.AfterEdit(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		public void OnEditCancelled(object sender, XmlConfigurationElementEventArgs e)
		{
			try
			{
				EventTracing.TraceMethodAndDelegate(this, this.EditCancelled);

				if (this.EditCancelled != null)
					this.EditCancelled(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		public void ResetBeforeEdit()
		{
			lock(this)
			{
				if (this.BeforeEdit != null)
				{
					System.Delegate[] invocationList = this.BeforeEdit.GetInvocationList();
					if (invocationList != null)
					{
						foreach(System.Delegate subscriber in invocationList)
							this.BeforeEdit -= (XmlConfigurationElementCancelEventHandler)subscriber;
					}
				}
			}
		}

		public void ResetAfterEdit()
		{
			lock(this)
			{
				if (this.AfterEdit != null)
				{
					System.Delegate[] invocationList = this.AfterEdit.GetInvocationList();
					if (invocationList != null)
					{
						foreach(System.Delegate subscriber in invocationList)
							this.AfterEdit -= (XmlConfigurationElementEventHandler)subscriber;
					}
				}
			}
		}

		public void ResetEditCancelled()
		{
			lock(this)
			{
				if (this.EditCancelled != null)
				{
					System.Delegate[] invocationList = this.EditCancelled.GetInvocationList();
					if (invocationList != null)
					{
						foreach(System.Delegate subscriber in invocationList)
							this.EditCancelled -= (XmlConfigurationElementEventHandler)subscriber;
					}
				}
			}
		}

		public bool HasChanges
		{
			get
			{
				bool anyOption = false;
				foreach(XmlConfigurationOption option in base.InnerList)
					if (option.HasChanges)
						anyOption = true;
				return _hasChanges || anyOption;
			}
			set
			{
				_hasChanges = value;
			}
		}

		public void AcceptChanges()
		{
			foreach(XmlConfigurationOption option in base.InnerList)
				option.AcceptChanges();
			_hasChanges = false;
		}

		public bool ApplyChanges(ISupportsEditing editableObject, Razor.Configuration.SupportedEditingActions actions)
		{
			XmlConfigurationOptionCollection options = editableObject as XmlConfigurationOptionCollection;
			if (options != null)
			{	
				foreach(XmlConfigurationOption option in options)
				{					
					XmlConfigurationOption myOption = this[option.ElementName];
					if (myOption != null)
					{
						try
						{
							myOption.ApplyChanges((ISupportsEditing)option, actions);
						}
						catch(System.Exception systemException)
						{
							System.Diagnostics.Trace.WriteLine(systemException);
						}
					}
				}

			}
			return true;
		}

		public bool ApplyToSelf(ISupportsEditing editableObject, SupportedEditingActions actions)
		{
			XmlConfigurationOptionCollection options = editableObject as XmlConfigurationOptionCollection;
			if (options != null)
			{	
				foreach(XmlConfigurationOption option in options)
				{					
					XmlConfigurationOption myOption = this[option.ElementName];
					if (myOption != null)
					{
						try
						{
							myOption.ApplyToSelf((ISupportsEditing)option, actions);
						}
						catch(System.Exception systemException)
						{
							System.Diagnostics.Trace.WriteLine(systemException);
						}
					}
				}
			}
			return true;
		}

		#endregion

		#region IXmlConfigurationElementEvents Members

		public event XmlConfigurationElementEventHandler Changed;

		public void OnChanged(object sender, XmlConfigurationElementEventArgs e)
		{
			try
			{
				// no events during initialization
				if (_isBeingInitialized) return;

				_hasChanges = true;
				
				EventTracing.TraceMethodAndDelegate(this, this.Changed);

				if (this.Changed != null)
					this.Changed(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		public void ResetChanged()
		{
			lock(this)
			{
				if (this.Changed != null)
				{
					System.Delegate[] invocationList = this.Changed.GetInvocationList();
					if (invocationList != null)
					{
						foreach(System.Delegate subscriber in invocationList)
							this.Changed -= (XmlConfigurationElementEventHandler)subscriber;
					}
				}
			}
		}

		#endregion

		#region ISupportInitialize Members

		public virtual void BeginInit()
		{
			_isBeingInitialized = true;

			foreach(XmlConfigurationOption option in base.InnerList)
				option.BeginInit();
		}

		public virtual void EndInit()
		{
			_isBeingInitialized = false;

			foreach(XmlConfigurationOption option in base.InnerList)
				option.EndInit();
		}

		#endregion
	}
}
