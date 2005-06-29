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
using System.Diagnostics;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Xml;

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for XmlConfiguration.
	/// </summary>
	[DefaultProperty("Categories")]	
	[TypeConverter(typeof(XmlConfigurationTypeConverter))]
	public class XmlConfiguration : XmlConfigurationElement
	{				
		/// <summary>
		/// The configuration will have unsaved changes by default until someone saves it or explicitly sets this to false
		/// </summary>
		protected bool _hasUnpersistedChanges = true;

		/// <summary>
		/// The path where this configuration is persisted
		/// </summary>
		protected string _path = string.Empty;

		/// <summary>
		/// The collection of categories contained in this configuration
		/// </summary>
		protected XmlConfigurationCategoryCollection _categories;

		/// <summary>
		/// Gets an array of valid path separators used by the configuration classes
		/// </summary>
		public static readonly char[] CategoryPathSeparators = {'\\', '/'};

		/// <summary>
		/// Gets the default path separator: a backslash
		/// </summary>
		public static readonly string DefaultPathSeparator = "\\";

		public event System.EventHandler TimeToSave;

		#region Instance Constructors

		/// <summary>
		/// Initializes a new instance of the XmlConfiguration class
		/// </summary>
		public XmlConfiguration() : base()
		{
		
		}

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationOption class
		/// </summary>
		/// <param name="element">The element to base this option on</param>
		public XmlConfiguration(XmlConfigurationElement element) : base(element)
		{

		}

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationOption class
		/// </summary>
		/// <param name="option">The option to base this option on</param>
		public XmlConfiguration(XmlConfiguration configuration) : base((XmlConfigurationElement)configuration)
		{
			_path = configuration.Path;
			_categories = configuration.Categories;			
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets a collection of categories (sub-categories for this XmlConfigurationElement)
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor(typeof(CollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[Description("The collection of categories contained in this configuration.")]
		[Category("Configuration Properties")]
		public XmlConfigurationCategoryCollection Categories
		{
			get
			{
				if (_categories == null)
				{
					_categories = new XmlConfigurationCategoryCollection();
					_categories.Parent = this;					
					_categories.BeforeEdit += new XmlConfigurationElementCancelEventHandler(base.OnBeforeEdit);
					_categories.Changed += new XmlConfigurationElementEventHandler(base.OnChanged);			
					_categories.AfterEdit += new XmlConfigurationElementEventHandler(base.OnAfterEdit);
					_categories.Changed += new XmlConfigurationElementEventHandler(Categories_Changed);
					if (_isBeingInitialized)
						_categories.BeginInit();
				}
				return _categories;
			}
			set
			{
				_categories = (XmlConfigurationCategoryCollection)value.Clone();
				_categories.Parent = this;
				_categories.BeforeEdit += new XmlConfigurationElementCancelEventHandler(base.OnBeforeEdit);
				_categories.Changed += new XmlConfigurationElementEventHandler(base.OnChanged);			
				_categories.AfterEdit += new XmlConfigurationElementEventHandler(base.OnAfterEdit);	
				_categories.Changed += new XmlConfigurationElementEventHandler(Categories_Changed);
				if (_isBeingInitialized)
					_categories.BeginInit();
			}
		}

		/// <summary>
		/// Gets or sets the path where this configuration is saved (Example: 'C:\MyApp\MyConfig.xml')
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[Description("The location of the file.")]
		[Category("Configuration Properties")]
		public string Path
		{
			get
			{
				return _path;
			}
			set
			{
				try
				{
					if (string.Compare(_path, value) == 0)
						return;
				}
				catch(System.Exception){ /*screw it*/ }

				_path = value;
				base.OnChanged(this, new XmlConfigurationEventArgs(this, XmlConfigurationElementActions.Changed));
			}
		}

		/// <summary>
		/// Determines whether changes have been made since the last time the object was persisted
		/// </summary>
		/// <returns></returns>
		public bool HasUnpersistedChanges()
		{
			return _hasUnpersistedChanges;
		}

		/// <summary>
		/// Sets whether changes have been made since the last time the object was persisted, used internally by the ConfigurationEngine class
		/// </summary>
		/// <param name="value"></param>
		public void SetHasUnpersistedChanges(bool value)
		{
			_hasUnpersistedChanges = value;
		}

		#endregion

		public override bool HasChanges
		{
			get
			{
				bool anyCategory = false;
				
				/// check this configuration's categories recursively for changes
				if (_categories != null)
					anyCategory = _categories.HasChanges;

				return base.HasChanges || anyCategory;
			}
			set
			{
				base.HasChanges = value;
			}
		}

//		public bool Load()
//		{
//			return true;
//		}
//
//		public bool Load(string path)
//		{
//			return true;
//		}
//
//		public bool Save()
//		{
//			return true;
//		}
//
//		public bool Save(string path)
//		{
//			return true;
//		}

		public void TraceCategories()
		{
			this.TraceCategories(_categories);
		}

		internal void TraceCategories(XmlConfigurationCategoryCollection categories)
		{
			System.Diagnostics.Trace.WriteLine(categories.Fullpath);
			foreach(XmlConfigurationCategory category in categories)
				this.TraceCategories(category.Categories);
		}

		/// <summary>
		/// Writes the contents of this configuration to a string in the format of an XmlDocument
		/// </summary>
		/// <returns></returns>
		public string ToXml()
		{
			MemoryStream stream = new MemoryStream();
			XmlConfigurationWriter writer = new XmlConfigurationWriter();
			writer.Write(this, stream, true);			
			string xml = System.Text.Encoding.ASCII.GetString(stream.GetBuffer());
			stream.Close();
			return xml;
		}

		/// <summary>
		/// Clones this configuration
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			XmlConfiguration clone = null;
			XmlConfigurationElement element = (XmlConfigurationElement)base.Clone();
			if (element != null)
			{
				clone = new XmlConfiguration(element);
				clone.ResetBeforeEdit();
				clone.ResetChanged();
				clone.ResetAfterEdit();
				clone.Path = this.Path;
				clone.SetHasUnpersistedChanges(this.HasUnpersistedChanges());
				clone.Categories =  (XmlConfigurationCategoryCollection)this.Categories.Clone();
			}	
			return clone;
		}
		
		public override void BeginInit()
		{
			base.BeginInit ();

			if (_categories != null)
				_categories.BeginInit();
		}

		public override void EndInit()
		{
			if (_categories != null)
				_categories.EndInit();

			base.EndInit ();
		}

		public override bool BeginEdit()
		{
			if (base.BeginEdit())
			{
				if (_categories != null)
					_categories.BeginEdit();
				
				return true;
			}
			return false;
		}

		public override bool CancelEdit()
		{
			if (_categories != null)
				_categories.CancelEdit();

			return base.CancelEdit ();
		}

		public override bool EndEdit()
		{			
			if (_categories != null)
				_categories.EndEdit();
			
			bool result = base.EndEdit();						
			this.AcceptChanges();
			this.ItIsNowTimeToSave();
			return result;
		}
		
		protected override XmlConfigurationElement GetElementToEdit()
		{
			XmlConfiguration configuration = (XmlConfiguration)this.Clone();
			return (XmlConfigurationElement)configuration;
		}

		public override void AcceptChanges()
		{
			if (_categories != null)
				_categories.AcceptChanges();
			base.AcceptChanges();
		}


		public override bool ApplyChanges(ISupportsEditing editableObject, SupportedEditingActions actions)
		{
			if (base.ApplyChanges (editableObject, actions))
			{
				XmlConfiguration configuration = editableObject as XmlConfiguration;			
				if (configuration != null)
				{
					if (_isBeingEdited)
						this.BeginInit();									
										
					if (_categories != null)
						_categories.ApplyChanges((ISupportsEditing)configuration.Categories, actions);

					if (_isBeingEdited)
						this.EndInit();
				}
				return true;
			}
			return false;
		}

		public override bool ApplyToSelf(ISupportsEditing editableObject, SupportedEditingActions actions)
		{
			if (base.ApplyToSelf(editableObject, actions))
			{
				XmlConfiguration configuration = editableObject as XmlConfiguration;			
				if (configuration != null)
				{																				
					if (_categories != null)
						_categories.ApplyToSelf((ISupportsEditing)configuration.Categories, actions);
				}
				return true;
			}

			this.ItIsNowTimeToSave();

			return false;
		}


		public override XmlConfiguration Configuration
		{
			get
			{
				return this;
			}
		}

		#region Static Methods

		public static string DescribeElementEnteringEdit(XmlConfigurationElementCancelEventArgs e)
		{
			try
			{
				string elementType = null;
				XmlConfigurationElementTypes et = e.Element.GetElementType();
				switch(et)
				{
				case XmlConfigurationElementTypes.XmlConfiguration:				elementType = "configuration"; break;
				case XmlConfigurationElementTypes.XmlConfigurationCategory:		elementType = "category"; break;		
				case XmlConfigurationElementTypes.XmlConfigurationElement:		elementType = "element"; break;
				case XmlConfigurationElementTypes.XmlConfigurationOption:		elementType = "option"; break;
				};
				
				return string.Format("The {0} '{1}' is entering edit mode at {2} on {3}. The current user is {4}.", elementType, e.Element.Fullpath, DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), System.Environment.UserName);				
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		public static string DescribeElementChanging(XmlConfigurationElementEventArgs e)
		{
			try
			{
				string elementType = null;
				XmlConfigurationElementTypes et = e.Element.GetElementType();
				switch(et)
				{
				case XmlConfigurationElementTypes.XmlConfiguration:				elementType = "configuration"; break;
				case XmlConfigurationElementTypes.XmlConfigurationCategory:		elementType = "category"; break;		
				case XmlConfigurationElementTypes.XmlConfigurationElement:		elementType = "element"; break;
				case XmlConfigurationElementTypes.XmlConfigurationOption:		elementType = "option"; break;
				};
				
				return string.Format("The {5} '{0}' was '{1}' in the '{2}' configuration at {3} on {4}. The {5} is{6}being edited. The current user is {7}.", e.Element.Fullpath, e.Action.ToString(), e.Element.Configuration.DisplayName, DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), elementType, (e.Element.IsBeingEdited ? " " : " not "), System.Environment.UserName);				
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		public static string DescribeElementLeavingEdit(XmlConfigurationElementEventArgs e)
		{
			try
			{
				string elementType = null;
				XmlConfigurationElementTypes et = e.Element.GetElementType();
				switch(et)
				{
				case XmlConfigurationElementTypes.XmlConfiguration:				elementType = "configuration"; break;
				case XmlConfigurationElementTypes.XmlConfigurationCategory:		elementType = "category"; break;		
				case XmlConfigurationElementTypes.XmlConfigurationElement:		elementType = "element"; break;
				case XmlConfigurationElementTypes.XmlConfigurationOption:		elementType = "option"; break;
				};
				
				return string.Format("The {0} '{1}' is leaving edit mode at {2} on {3}. The current user is {4}.", elementType, e.Element.Fullpath, DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), System.Environment.UserName);				
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		public static string DescribeElementCancellingEdit(XmlConfigurationElementEventArgs e)
		{
			try
			{
				string elementType = null;
				XmlConfigurationElementTypes et = e.Element.GetElementType();
				switch(et)
				{
				case XmlConfigurationElementTypes.XmlConfiguration:				elementType = "configuration"; break;
				case XmlConfigurationElementTypes.XmlConfigurationCategory:		elementType = "category"; break;		
				case XmlConfigurationElementTypes.XmlConfigurationElement:		elementType = "element"; break;
				case XmlConfigurationElementTypes.XmlConfigurationOption:		elementType = "option"; break;
				};
				
				return string.Format("The {0} '{1}' has cancelled edit mode at {2} on {3}. The current user is {4}.", elementType, e.Element.Fullpath, DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), System.Environment.UserName);				
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		#endregion

		/// <summary>
		/// Searches the tree of categories for the matching category by the specified key or path
		/// </summary>
		/// <param name="keyOrPath">The key or path of combined keys that uniquely identifies the category</param>
		/// <returns></returns>
		public XmlConfigurationCategory FindCategory(string keyOrPath)
		{
			/// chunk up the path into the separate categories
			string[] categories = keyOrPath.Split(XmlConfiguration.CategoryPathSeparators);
			
			/// strip the config key from the path
			if (categories[0] == this.ElementName)
				keyOrPath = string.Join(XmlConfiguration.DefaultPathSeparator, categories, 1, categories.Length - 1);

			return this.Categories.FindCategory(keyOrPath);
		}

		/// <summary>
		/// Occurs when a category has changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Categories_Changed(object sender, XmlConfigurationElementEventArgs e)
		{
			if (!e.Element.IsBeingEdited)
			{
				_hasUnpersistedChanges = true;
			}
		}

		/// <summary>
		/// Fires the TimeToSave event
		/// </summary>
		public void ItIsNowTimeToSave()
		{
			// raise the event that it is time to save
			this.OnTimeToSave(this, EventArgs.Empty);
		}

		/// <summary>
		/// Raises the TimeToSave event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnTimeToSave(object sender, System.EventArgs e)
		{
			try
			{
				if (this.TimeToSave != null)
					this.TimeToSave(sender, e);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}
	}
}
