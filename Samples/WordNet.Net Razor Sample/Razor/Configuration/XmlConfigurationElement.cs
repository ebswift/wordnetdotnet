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
	/// The base class for elements in the Core.Configuration namespace. Provides a basic set of properties to describe the element.
	/// </summary>
//	[DesignTimeVisible(false)]
	[DefaultProperty("ElementName")]
	[TypeConverter(typeof(XmlConfigurationElementTypeConverter))] 
	public class XmlConfigurationElement : Component, ICloneable, ISupportInitialize, ISupportsEditing, IXmlConfigurationElementEvents
	{
		protected string _elementName;
		protected string _description;
		protected string _category;
		protected string _displayName;
		protected bool _hidden;
		protected bool _readonly;
		protected bool _persistent;
		protected bool _hasChanges;
		protected bool _isBeingInitialized;
		protected bool _isBeingEdited;		
		protected XmlConfigurationElement _editableProxy;
		protected bool _isEditableProxy;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		protected System.ComponentModel.Container components = null;
		
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

		private void SetDefaultValues()
		{
			_elementName = System.Guid.NewGuid().ToString();
			//			_description = null;
			//			_category = null;
			//			_displayName = null;
			//			_hidden = false;	
			//			_readonly = false;
			_persistent = true;
			//			_hasChanges = false;
			//			_isBeingInitialized = false;
			//			_isBeingEdited = false;		
			//			_editableProxy = null;
			//			_isEditableProxy = false;
		}

		#region Instance Constructors

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationElement class
		/// </summary>
		/// <param name="container"></param>
		public XmlConfigurationElement(System.ComponentModel.IContainer container)
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			container.Add(this);
			this.InitializeComponent();
			this.SetDefaultValues();
		}

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationElement class
		/// </summary>
		public XmlConfigurationElement()
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			this.InitializeComponent();	
			this.SetDefaultValues();
		}


		/// <summary>
		/// Initializes a new instance of the XmlConfigurationElement class
		/// </summary>
		/// <param elementName="elementName">The elementName of this element</param>
		public XmlConfigurationElement(string elementName) : this()
		{
			_elementName = elementName;
		}

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationElement class
		/// </summary>
		/// <param elementName="elementName">A elementName for this element</param>
		/// <param elementName="description">A description for this element</param>
		/// <param elementName="category">A category for this element</param>
		/// <param elementName="displayName">A display name for this element</param>
		public XmlConfigurationElement(string elementName, string description, string category, string displayName) : this(elementName)
		{
			_description = description;
			_category = category;
			_displayName = displayName;
		}

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationElement class
		/// </summary>
		/// <param name="element"></param>
		public XmlConfigurationElement(XmlConfigurationElement element) : this(element.ElementName)
		{			
			_description = element.Description;
			_category = element.Category;
			_displayName = element.DisplayName;
			_hidden = element.Hidden;
			_readonly = element.Readonly;
			_persistent = element.Persistent;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the elementName of this XmlConfigurationElement
		/// </summary>			
		[Description("A elementName that uniquely identifies this element in a collection. Also when combined in a recursive collection will form a path that can be used to refer to this element. Example: \"Environment\\General\\MyOption\"")]
		[Category("Element Properties")]	
		public virtual string ElementName
		{
			get	
			{ 
				if (_isBeingEdited)
					if (_editableProxy != null)
						return _editableProxy.ElementName;

				return _elementName; 
			}
			set
			{
				if (_isBeingEdited)
				{
					if (_editableProxy != null)
					{
						_editableProxy.ElementName = value;
						return;
					}
				}

				if (_elementName == value)
					return;

//				_hasChanges = true;
				_elementName = value;
				this.OnChanged(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.Changed));				
			}
		}

		/// <summary>
		/// Gets or sets the description of this XmlConfigurationElement
		/// </summary>
//		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("The description text displayed at runtime in the property grid for this option.")]
		[Category("Element Properties")]
		public virtual string Description
		{
			get
			{
				if (_isBeingEdited)
					if (_editableProxy != null)
						return _editableProxy.Description;

				return _description;
			}
			set
			{
				if (_isBeingEdited)
				{
					if (_editableProxy != null)
					{							
						_editableProxy.Description = value;
						return;
					}
				}
				
				if (_description == value)
					return;

//				_hasChanges = true;
				_description = value;
				this.OnChanged(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.Changed));				
			}
		}

		/// <summary>
		/// Gets or sets the category of this XmlConfigurationElement
		/// </summary>
		[Description("The category in which this option will be grouped when displayed in the property grid with the other options in the same collection.")]
		[Category("Element Properties")]
		public virtual string Category
		{
			get
			{
				if (_isBeingEdited)
					if (_editableProxy != null)
						return _editableProxy.Category;

				return _category;
			}
			set
			{
				if (_isBeingEdited)
				{
					if (_editableProxy != null)
					{
						_editableProxy.Category = value;
					}
				}
				
				if (_category == value)
					return;

//				_hasChanges = true;
				_category = value;				
				this.OnChanged(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.Changed));				
			}
		}

		/// <summary>
		/// Gets or sets the display elementName of this XmlConfigurationElement
		/// </summary>
		[Description("The text displayed at runtime in the property grid for the name of this option.")]
		[Category("Element Properties")]
		public virtual string DisplayName
		{
			get
			{
				if (_isBeingEdited)
					if (_editableProxy != null)
						return _editableProxy.DisplayName;
								
				if (_displayName == null || _displayName == string.Empty)
					return _elementName;

				return _displayName;
			}
			set
			{
				if (_isBeingEdited)
				{
					if (_editableProxy != null)
					{
						_editableProxy.DisplayName = value;
						return;
					}
				}
				
				if (_displayName == value)
					return;

//				_hasChanges = true;
				_displayName = value;
				this.OnChanged(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.Changed));				
			}
		}

		/// <summary>
		/// Gets or sets whether this option is hidden in the default editor.
		/// </summary>
		[Description("A flag that indicates whether this option will be displayed at runtime in the property grid.")]
		[Category("Element Properties")]
		public virtual bool Hidden
		{
			get
			{
				if (_isBeingEdited)
					if (_editableProxy != null)
						return _editableProxy.Hidden;

				return _hidden;
			}
			set
			{
				if (_isBeingEdited)
				{
					if (_editableProxy != null)
					{
						_editableProxy.Hidden = value;
						return;
					}
				}
				
				if (_hidden == value)
					return;

//				_hasChanges = true;
				_hidden = value;
				this.OnChanged(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.Changed));				
			}
		}

		/// <summary>
		/// Gets or sets whether this option is readonly in the default editor.
		/// </summary>
		[Description("A flag that indicates whether this option will be readonly at runtime in the property grid.")]
		[Category("Element Properties")]
		public virtual bool Readonly
		{
			get
			{
				if (_isBeingEdited)
					if (_editableProxy != null)
						return _editableProxy.Readonly;

				return _readonly;
			}
			set
			{
				if (_isBeingEdited)
				{
					if (_editableProxy != null)
					{
						_editableProxy.Readonly = value;
						return;
					}
				}
				
				if (_readonly == value)
					return;

//				_hasChanges = true;
				_readonly = value;
				this.OnChanged(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.Changed));				
			}
		}

		/// <summary>
		/// Gets or sets whether this option is persisted when the option is saved.
		/// </summary>
		[Description("A flag that describes whether this option is persistent (ie. written to the configuration file) or volatile.")]
		[Category("Element Properties")]
		public virtual bool Persistent
		{
			get
			{
				if (_isBeingEdited)
					if (_editableProxy != null)
						return _editableProxy.Persistent;

				return _persistent;
			}
			set
			{
				if (_isBeingEdited)
				{
					if (_editableProxy != null)
					{
						_editableProxy.Persistent = value;
						return;
					}
				}
				
				if (_persistent == value)
					return;

//				_hasChanges = true;
				_persistent = value;
				this.OnChanged(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.Changed));				
			}
		}

		/// <summary>
		/// Gets or sets the full path to this option
		/// </summary>
		[Browsable(false)]
//		[Description("The full path of keys combined to form a path to this element. This property exists for all objects that inherit from XmlConfigurationElement.")]
//		[Category("Element Properties")]
		public virtual string Fullpath
		{
			get
			{								
				return _elementName;
			}
//			set
//			{
//				_fullpath = value;				
//			}
		}

		[Browsable(false)]
		public virtual XmlConfiguration Configuration
		{
			get
			{				
				return null;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Determines the type of element that this object is.
		/// </summary>
		/// <returns></returns>
		public XmlConfigurationElementTypes GetElementType()
		{
			Type thisType = this.GetType();
			if (thisType != null)
			{
				if (thisType == typeof(XmlConfigurationElement))
					return XmlConfigurationElementTypes.XmlConfigurationElement;

				if (thisType == typeof(XmlConfigurationOption))
					return XmlConfigurationElementTypes.XmlConfigurationOption;

				if (thisType == typeof(XmlConfigurationCategory))
					return XmlConfigurationElementTypes.XmlConfigurationCategory;

				if (thisType == typeof(XmlConfiguration))
					return XmlConfigurationElementTypes.XmlConfiguration;
			};
			return XmlConfigurationElementTypes.Null;
		}

		protected virtual XmlConfigurationElement GetElementToEdit()
		{
			return (XmlConfigurationElement)this.Clone();
		}

		#endregion

		#region ICloneable Members

//		public virtual object Clone()
//		{
//			XmlConfigurationElement clone = new XmlConfigurationElement();
//			
//			clone.ElementName = _elementName;
//			clone.Description = _description;
//			clone.Category = _category;
//			clone.DisplayName = _displayName;
//			clone.Hidden = _hidden;
//			clone.Readonly = _readonly;
//			clone.Persistent = _persistent;
////			clone.Fullpath = _fullpath;
//			clone.AcceptChanges();
//			
//			return clone;
//		}
		
		public virtual object Clone()
		{
			object clone = CloningEngine.Clone(this, CloningEngine.DefaultBindingFlags);
			return clone;
		}

		#endregion

		#region ISupportInitialize Members

		public virtual void BeginInit()
		{
			_isBeingInitialized = true;
		}

		public virtual void EndInit()
		{
			_isBeingInitialized = false;
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
				return _isBeingEdited || _isEditableProxy;
			}
		}

		public virtual bool BeginEdit()
		{
			try
			{
				if (!_isBeingEdited)
				{
					/// Raise BeforeEdit event and provide a means of cancellation
					XmlConfigurationElementCancelEventArgs e = new XmlConfigurationElementCancelEventArgs(this, false);
					this.OnBeforeEdit(this, e);
					if (e.Cancel)
						return false;
					
					/// place the element in edit mode and clone ourself so that future changes will be redirected to the clone and not to ourself					
					_editableProxy = this.GetElementToEdit();		
					if (_editableProxy != null)
					{
						_editableProxy.Changed += new XmlConfigurationElementEventHandler(this.OnChanged);
						_editableProxy._isEditableProxy = true;
						_isBeingEdited = true;
						return true;
					}					
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}

		public virtual bool EndEdit()
		{
			bool success = false;
			try
			{
				if (_isBeingEdited)
				{		
					_isBeingEdited = false;

					this.BeginInit();

					// apply the changes
					this.ApplyChanges((ISupportsEditing)_editableProxy, SupportedEditingActions.Synchronize);

					this.EndInit();

					// destroy clone's event handler
					if (_editableProxy != null)
					{
						_editableProxy.Changed -= new XmlConfigurationElementEventHandler(this.OnChanged);
						_editableProxy = null;
					}

					try
					{
						/// make sure to kick this off so that no we are getting all events out
						if (this.HasChanges)
							this.OnChanged(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.Changed));
					}
					catch (System.Exception systemException)
					{
						System.Diagnostics.Trace.WriteLine(systemException);
					}
						
					/// reset the haschanges flag and accept the current changes
					this.AcceptChanges();
				}
				success = true;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			finally
			{
				/// raise the AfterEdit event
				this.OnAfterEdit(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.None));										
			}
			return success;			
		}

		public virtual bool CancelEdit()
		{
			try
			{
				if (_isBeingEdited)
				{
					_isBeingEdited = false;

					// destroy clone's event handler
					if (_editableProxy != null)
					{
						_editableProxy.Changed -= new XmlConfigurationElementEventHandler(this.OnChanged);
						_editableProxy = null;
					}

					// should not this accept changes? just like end edit?
					
					/// raise the AfterEdit event
					this.OnEditCancelled(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.None));										
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

		public virtual bool HasChanges
		{
			get
			{						
				return _hasChanges;
			}
			set
			{
				_hasChanges = value;
			}
		}

		public virtual void AcceptChanges()
		{
			this.HasChanges = false;
		}

		public virtual bool ApplyChanges(ISupportsEditing editableObject, Razor.Configuration.SupportedEditingActions actions)
		{
			if (actions == SupportedEditingActions.None)
				return true;

			XmlConfigurationElement element = editableObject as XmlConfigurationElement;			
			if (element != null)
			{
				/// do we match in full paths?
				if (string.Compare(this.Fullpath, element.Fullpath, true) == 0)
				{
					/// does the element have changes, if not we don't need to bother
					if (element.HasChanges)
					{				
						/// yes so apply it's changed features
						this.ElementName = element.ElementName;
						this.Description = element.Description;
						this.Category = element.Category;
						this.DisplayName = element.DisplayName;
						this.Hidden = element.Hidden;
						this.Readonly = element.Readonly;
						this.Persistent = element.Persistent;
					}
					return true;
				}
			}


			return false;
		}

		public virtual bool ApplyToSelf(ISupportsEditing editableObject, SupportedEditingActions actions)
		{
			XmlConfigurationElement element = editableObject as XmlConfigurationElement;			
			if (element != null)
			{
				// if this fullpath matches the element's full path, then these may apply to each other
				if (string.Compare(this.Fullpath, element.Fullpath, true) == 0)
				{
					if (element.HasChanges)
					{
						this.ElementName = element.ElementName;
						this.Description = element.Description;
						this.Category = element.Category;
						this.DisplayName = element.DisplayName;
						this.Hidden = element.Hidden;
						this.Readonly = element.Readonly;
						this.Persistent = element.Persistent;
					}
					return true;
				}
			}
			return false;
		}

		#endregion

		#region IXmlConfigurationElementEvents Members

		public event XmlConfigurationElementEventHandler Changed;

		public virtual void OnChanged(object sender, XmlConfigurationElementEventArgs e)
		{
			try
			{							
				// no events during initialization or editing
				if (_isBeingInitialized/*|| _isBeingEdited */) return;	

				// are we ourselves changing? if not then our sub options and sub categories don't change us!!!
				if (string.Compare(this.Fullpath, e.Element.Fullpath, true) == 0)
				{					
					_hasChanges = true;
				}											

				EventTracing.TraceMethodAndDelegate(this, this.Changed);

				if (this.Changed != null)
					this.Changed(sender, e);

			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		public virtual void ResetChanged()
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

		public virtual void TriggerChange()
		{
			this.OnChanged(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.Changed));
		}
	}
}
