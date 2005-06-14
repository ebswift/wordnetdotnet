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
using System.Drawing;

namespace Razor.Configuration 
{
	/// <summary>
	/// Defines an object which can be used to describe a "ElementName-and-Value" pair, using custom display names and descriptions.
	/// </summary>
	//	[DesignTimeVisible(false)]	
	[TypeConverter(typeof(XmlConfigurationOptionTypeConverter))]
	[Designer(typeof(XmlConfigurationOptionDesigner))]	
	[PropertyTab(typeof(XmlConfigurationOptionPropertyTab), PropertyTabScope.Component)]	
	public class XmlConfigurationOption : XmlConfigurationElement
	{
		internal object _value = string.Empty;
		protected string _valueAssemblyQualifiedName;
		protected string _editorAssemblyQualifiedName;
		protected bool _shouldSerializeValue;
		protected XmlConfigurationOptionCollection _parent;

		#region Instance Constructors

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationOption class
		/// </summary>
		public XmlConfigurationOption() : base() 
		{
			_valueAssemblyQualifiedName = _value.GetType().AssemblyQualifiedName;			
		}

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationOption class
		/// </summary>
		/// <param name="name">A name for this option</param>
		/// <param name="value">A value for this option</param>
		public XmlConfigurationOption(string elementName, object value) : base(elementName) 
		{
			if (value != null)
			{
				_value = value;	
				_valueAssemblyQualifiedName = _value.GetType().AssemblyQualifiedName;
			}
		}

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationOption class
		/// </summary>
		/// <param name="name">A name for this option</param>
		/// <param name="value">A value for this option</param>
		/// <param name="description">A description for this option</param>
		/// <param name="category">A category for this option</param>
		/// <param name="displayName">A display name for this option</param>
		public XmlConfigurationOption(string elementName, object value, string description, string category, string displayName) : base(elementName, description, category, displayName) 
		{
			if (value != null)
			{
				_value = value;	
				_valueAssemblyQualifiedName = _value.GetType().AssemblyQualifiedName;
			}
		}
			
		/// <summary>
		/// Initializes a new instance of the XmlConfigurationOption class
		/// </summary>
		/// <param name="element">The element to base this option on</param>
		public XmlConfigurationOption(XmlConfigurationElement element) : base(element)
		{
			_valueAssemblyQualifiedName = _value.GetType().AssemblyQualifiedName;
		}

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationOption class
		/// </summary>
		/// <param name="option">The option to base this option on</param>
		public XmlConfigurationOption(XmlConfigurationOption option) : base((XmlConfigurationElement)option)
		{
			_valueAssemblyQualifiedName = _value.GetType().AssemblyQualifiedName;
			if (option != null)
			{
				_value = option.Value;
				_valueAssemblyQualifiedName = option.ValueAssemblyQualifiedName;
				_editorAssemblyQualifiedName = option.EditorAssemblyQualifiedName;
				_shouldSerializeValue = option.ShouldSerializeValue;						
			}
		}

		#endregion

		#region Public Properties

		[Description("A flag that indicates whether the Value should be serialized/deserialized.")]
		[Category("Value Properties")]
		public bool ShouldSerializeValue
		{
			get
			{
				if (_isBeingEdited)
					if (_editableProxy != null)
                        return ((XmlConfigurationOption)_editableProxy).ShouldSerializeValue;

				return _shouldSerializeValue;
			}
			set
			{
				if (_isBeingEdited)
				{
					if (_editableProxy != null)
					{
						((XmlConfigurationOption)_editableProxy).ShouldSerializeValue = value;
						return;
					}
				}
				if (_shouldSerializeValue == value)
					return;
				
//				_hasChanges = true;
				_shouldSerializeValue = value;
				base.OnChanged(this, new XmlConfigurationOptionEventArgs(this, XmlConfigurationElementActions.Changed));				
			}
		}

//		[Description("The fully qualified name of the Assembly that contains the type name specified by ValueAssemblyQualifiedName.")]
//		[Category("Value Properties")]
//		public string ReferencedAssemblyName 
//		{
//			get 
//			{
//				if (_isBeingEdited)
//					return ((XmlConfigurationOption)_editableProxy).ReferencedAssemblyName;
//
//				return _referencedAssemblyName;
//			}
//			set 
//			{
//				if (_isBeingEdited)
//				{
//					((XmlConfigurationOption)_editableProxy).ReferencedAssemblyName = value;
//				}
//				else
//				{
//					if (string.Equals(_referencedAssemblyName, value))
//						return;
//
//					_referencedAssemblyName = value;
//					base.OnChanged(this, new XmlConfigurationOptionEventArgs(this, XmlConfigurationElementActions.Changed));
//				}
//			}
//		}

		/// <summary>
		/// Gets or sets the fully qualified name of the System.Type of the object stored in Value, including the name of the assembly from which the System.Type was loaded.
		/// </summary>		
		[Editor(typeof(TypeSelectionTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[Description("Gets or sets the fully qualified name of the System.Type of the object stored in Value, including the name of the assembly from which the System.Type was loaded.")]
		[Category("Value Properties")]		
		public string ValueAssemblyQualifiedName 
		{
			get 
			{		
				if (_isBeingEdited)
					if (_editableProxy != null)
						return ((XmlConfigurationOption)_editableProxy).ValueAssemblyQualifiedName;

				return _valueAssemblyQualifiedName;
			}
			set 
			{
				if (_isBeingEdited)
				{
					if (_editableProxy != null)
					{
						((XmlConfigurationOption)_editableProxy).ValueAssemblyQualifiedName = value;
						return;
					}
				}
				
				if (_valueAssemblyQualifiedName == value)
					return;									
				
//				_hasChanges = true;
				_valueAssemblyQualifiedName = value;
				base.OnChanged(this, new XmlConfigurationOptionEventArgs(this, XmlConfigurationElementActions.Changed));			
			}
		}

		/// <summary>
		/// Gets or sets the fully qualified name of the System.Type of the object's UITypeEditor, including the name of the assembly from which the System.Type was loaded.
		/// </summary>
		[Description("Gets or sets the fully qualified name of the System.Type of the object's UITypeEditor, including the name of the assembly from which the System.Type was loaded.")]
		[Category("Value Properties")]
		public string EditorAssemblyQualifiedName
		{
			get
			{
				if (_isBeingEdited)
					if (_editableProxy != null)
						return ((XmlConfigurationOption)_editableProxy).EditorAssemblyQualifiedName;
			
				return _editorAssemblyQualifiedName;
			}
			set
			{
				if (_isBeingEdited)
				{
					if (_editableProxy != null)
					{
						((XmlConfigurationOption)_editableProxy).EditorAssemblyQualifiedName = value;
						return;
					}
				}
				
				if (_editorAssemblyQualifiedName == value)
					return;
				
//				_hasChanges = true;
				_editorAssemblyQualifiedName = value;
				base.OnChanged(this, new XmlConfigurationOptionEventArgs(this, XmlConfigurationElementActions.Changed));				
			}
		}
		
		/// <summary>
		/// Gets or sets a value for this option.
		/// </summary>	
		[Description("The object that contains the actual data for this XmlConfigurationOption this is of the System.Type specified by the ValueType property.")]
		[Category("Value Properties")]
		public object Value 
		{
			get 
			{								
				if (_isBeingEdited)
					if (_editableProxy != null)
						return ((XmlConfigurationOption)_editableProxy).Value;

				return _value;
			}
			set 
			{
				if (_isBeingEdited)
				{
					if (_editableProxy != null)
					{
						((XmlConfigurationOption)_editableProxy).Value = value;
						return;
					}
				}
				
				bool compared = false;
				// try using the IComparable interface first
				System.IComparable existingValue = _value as System.IComparable;
				if (existingValue != null) 
				{
					System.IComparable newValue = value as System.IComparable;
					if (newValue != null) 
					{
						if (existingValue.GetType() == newValue.GetType()) 
						{
							try 
							{
								if (existingValue.CompareTo(newValue) == 0)
									return;
								compared = true;
							}
							catch(System.Exception) { /* fuck it */ }
						}
					}
				}
				
				// then try operator equality
				if (!compared)					
					if (Object.Equals(_value, value))
						return;

//				_hasChanges = true;
				_value = value;
				if (_value != null)
					_valueAssemblyQualifiedName = _value.GetType().AssemblyQualifiedName;
				base.OnChanged(this, new XmlConfigurationOptionEventArgs(this, XmlConfigurationElementActions.Changed));				
			}
		}

		/// <summary>
		/// Override the trigger change to reflect an option change instead of an elemental change
		/// </summary>
		public override void TriggerChange()
		{
			base.OnChanged(this, new XmlConfigurationOptionEventArgs(this, XmlConfigurationElementActions.Changed));
		}

//		public void SetValue(object value, bool setValueAssemblyQualifiedName)
//		{
//			if (_isBeingEdited)
//			{
//				((XmlConfigurationOption)_editableProxy).Value = value;
//			}
//			else
//			{
//				bool compared = false;
//				/// try using the IComparable interface first
//				System.IComparable existingValue = _value as System.IComparable;
//				if (existingValue != null) 
//				{
//					System.IComparable newValue = value as System.IComparable;
//					if (newValue != null) 
//					{
//						if (existingValue.GetType() == newValue.GetType()) 
//						{
//							try 
//							{
//								if (existingValue.CompareTo(newValue) == 0)
//									return;
//								compared = true;
//							}
//							catch(System.Exception) { /* fuck it */ }
//						}
//					}
//				}
//					
//				/// then try operator equality
//				if (!compared)					
//					if (Object.Equals(_value, value))
//						return;
//
//				_value = value;
//				if (setValueAssemblyQualifiedName)
//					if (_value != null)
//						_valueAssemblyQualifiedName = _value.GetType().AssemblyQualifiedName;
//				base.OnChanged(this, new XmlConfigurationOptionEventArgs(this, XmlConfigurationElementActions.Changed));
//			}
//		}

		/// <summary>
		/// Gets or sets the collection to which this option is a child.
		/// </summary>
		[Browsable(false)]
		public XmlConfigurationOptionCollection Parent 
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

		#endregion
		
		public override string Fullpath
		{
			get
			{
				if (_parent == null)
                    return base.Fullpath;
				else
				{
					string path = _parent.Fullpath;
					path = (path != null ? path + @"\" + base.Fullpath : base.Fullpath);
					return path;
				}
			}
		}

		/// <summary>
		/// Returns a clone of the current object
		/// </summary>
		/// <returns></returns>
//		public override object Clone()
//		{
//			XmlConfigurationOption clone = null;
//			XmlConfigurationElement element = (XmlConfigurationElement)base.Clone();
//			if (element != null)
//			{
//				clone = new XmlConfigurationOption(element);
//				clone.ResetBeforeEdit();
//				clone.ResetChanged();
//				clone.ResetAfterEdit();
//				clone.ResetEditCancelled();
//				clone.Value = this.Value;
//				clone.ValueAssemblyQualifiedName = this.ValueAssemblyQualifiedName;
////				clone.ReferencedAssemblyName = this.ReferencedAssemblyName;			
//				clone.EditorAssemblyQualifiedName = this.EditorAssemblyQualifiedName;
//			}	
//			return clone;
//		}
		
		public override object Clone()
		{
			object clone = CloningEngine.Clone(this, CloningEngine.DefaultBindingFlags);
			if (clone != null)
			{
				((XmlConfigurationOption)clone).ResetBeforeEdit();
				((XmlConfigurationOption)clone).ResetChanged();
				((XmlConfigurationOption)clone).ResetAfterEdit();
				((XmlConfigurationOption)clone).ResetEditCancelled();
				return clone;
			}	
			return null;
		}

		protected override XmlConfigurationElement GetElementToEdit()
		{
			XmlConfigurationOption option = (XmlConfigurationOption)this.Clone();
			option.Parent = this.Parent;
			return (XmlConfigurationElement)option;
		}


		public override bool ApplyChanges(ISupportsEditing editableObject, SupportedEditingActions actions)
		{
			/// if we can apply changes to the base then keep going
			if (base.ApplyChanges (editableObject, actions))
			{
				XmlConfigurationOption option = editableObject as XmlConfigurationOption;			
				if (option != null)
				{										
					if (option.HasChanges)
					{
						this.Value = option.Value;
						this.ValueAssemblyQualifiedName = option.ValueAssemblyQualifiedName;
						this.EditorAssemblyQualifiedName = option.EditorAssemblyQualifiedName;
					}				
				}
				return true;
			}
			return false;
		}

		public override bool ApplyToSelf(ISupportsEditing editableObject, SupportedEditingActions actions)
		{			
			if (base.ApplyToSelf(editableObject, actions))
			{
				XmlConfigurationOption option = editableObject as XmlConfigurationOption;			
				if (option != null)
				{				
					if (option.HasChanges)
					{
						this.Value = option.Value;
						this.ValueAssemblyQualifiedName = option.ValueAssemblyQualifiedName;
						this.EditorAssemblyQualifiedName = option.EditorAssemblyQualifiedName;
					}
				}
				return true;
			}
			return false;			
		}


		public override XmlConfiguration Configuration
		{
			get
			{
				if (_parent != null)
                    return _parent.Configuration;
				return base.Configuration;
			}
		}
	}
}
