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
	/// Summary description for XmlConfigurationCategoryCollection.
	/// </summary>
	public class XmlConfigurationCategoryCollection : System.Collections.CollectionBase, ICloneable, ISupportsEditing, IXmlConfigurationElementEvents, ISupportInitialize
	{
		private XmlConfigurationElement _parent;
		private bool _hasChanges;
		protected bool _isBeingEdited;
		private bool _isBeingInitialized;

		#region Instance Constructors

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationCategoryCollection class
		/// </summary>
		public XmlConfigurationCategoryCollection()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#endregion

		#region Public Methods

		public int Add(XmlConfigurationCategory category)
		{
			if (this.Contains(category))
				throw new ArgumentException("ElementName already exists. ElementName in collection: " + category.ElementName + " ElementName being added: " + category.ElementName);
				
			category.Parent = this;
			category.BeforeEdit += new XmlConfigurationElementCancelEventHandler(this.OnBeforeEdit);
			category.Changed += new XmlConfigurationElementEventHandler(this.OnChanged);			
			category.AfterEdit += new XmlConfigurationElementEventHandler(this.OnAfterEdit);
			category.EditCancelled += new XmlConfigurationElementEventHandler(this.OnEditCancelled);
			int index = base.InnerList.Add(category);
			this.OnChanged(this, new XmlConfigurationCategoryEventArgs(category, XmlConfigurationElementActions.Added));
			return index;
		}

		public void Add(XmlConfigurationCategory[] categories)
		{
			if (categories == null)
				throw new ArgumentNullException("categories");			

			foreach(XmlConfigurationCategory category in categories) 
			{
				try 
				{
					this.Add(category);
				}
				catch(System.Exception systemException) 
				{
					System.Diagnostics.Trace.WriteLine(systemException);
				}
			}
		}

		public void Add(string elementName)
		{
			XmlConfigurationCategory category = null;

			if (!this.Contains(elementName))
			{
				category = new XmlConfigurationCategory(elementName);
				this.Add(category);
			}
		}

		public void Insert(int index, XmlConfigurationCategory category)
		{
			if (this.Contains(category))
				throw new ArgumentException("ElementName already exists. ElementName in collection: " + category.ElementName + " ElementName being added: " + category.ElementName);
			
			category.Parent = this;
			category.BeforeEdit += new XmlConfigurationElementCancelEventHandler(this.OnBeforeEdit);
			category.Changed += new XmlConfigurationElementEventHandler(this.OnChanged);			
			category.AfterEdit += new XmlConfigurationElementEventHandler(this.OnAfterEdit);
			category.EditCancelled += new XmlConfigurationElementEventHandler(this.OnEditCancelled);
			base.InnerList.Insert(index, category);
			this.OnChanged(this, new XmlConfigurationCategoryEventArgs(category, XmlConfigurationElementActions.Added));			
		}

		public void Remove(XmlConfigurationCategory category)
		{
			if (this.Contains(category))
			{											
				category.BeforeEdit -= new XmlConfigurationElementCancelEventHandler(this.OnBeforeEdit);
				category.Changed -= new XmlConfigurationElementEventHandler(this.OnChanged);			
				category.AfterEdit -= new XmlConfigurationElementEventHandler(this.OnAfterEdit);
				category.EditCancelled -= new XmlConfigurationElementEventHandler(this.OnEditCancelled);
				base.InnerList.Remove(category);
				this.OnChanged(this, new XmlConfigurationCategoryEventArgs(category, XmlConfigurationElementActions.Removed));
			}		
		}

		public void Remove(string elementName)
		{
			if (this.Contains(elementName))
				foreach(XmlConfigurationCategory category in base.InnerList)
					if (category.ElementName == elementName)
						this.Remove(category);					
		}

		public bool Contains(XmlConfigurationCategory category)
		{
			foreach(XmlConfigurationCategory c in base.InnerList)
				if (c.ElementName == category.ElementName)
					return true;
			
			return false;
		}

		public bool Contains(string elementName)
		{
			foreach(XmlConfigurationCategory c in base.InnerList)
				if (c.ElementName == elementName)
					return true;
			return false;
		}

		#endregion

		#region Public Properties

		public XmlConfigurationCategory this[int index]
		{
			get
			{
				return base.InnerList[index] as XmlConfigurationCategory;
			}
			set
			{
				base.InnerList[index] = value;
			}
		}

		public XmlConfigurationCategory this[string keyOrPath]
		{
			get
			{
				string[] categories = keyOrPath.Split(XmlConfiguration.CategoryPathSeparators);
				foreach(XmlConfigurationCategory category in base.InnerList)
				{
					if (category.ElementName == categories[0])
					{
						if (categories.Length == 1)
						{
							return category;
						}
						else
						{
							keyOrPath = string.Join(XmlConfiguration.DefaultPathSeparator, categories, 1, categories.Length - 1);
							return category.Categories[keyOrPath];
						}
					}					
				}
				return null;
			}
		}

		public XmlConfigurationCategory this[string keyOrPath, bool createIfNotFound]
		{
			get
			{
				string[] categories = keyOrPath.Split(XmlConfiguration.CategoryPathSeparators);
				foreach(XmlConfigurationCategory category in base.InnerList)
				{
					if (category.ElementName == categories[0])
					{
						if (categories.Length == 1)
							return category;
						else
							keyOrPath = string.Join(XmlConfiguration.DefaultPathSeparator, categories, 1, categories.Length - 1);

						XmlConfigurationCategory subCategory = category.Categories[keyOrPath];
						if (subCategory != null)
							return subCategory;
						else
							break;
					}
				}

				if (createIfNotFound)
					if (categories.Length > 0)
					{
						this.Add(categories[0]);
						XmlConfigurationCategory newCategory = this[categories[0]];
						if (categories.Length == 1)
							return newCategory;
						else
							keyOrPath = string.Join(XmlConfiguration.DefaultPathSeparator, categories, 1, categories.Length - 1);

						return newCategory.Categories[keyOrPath, createIfNotFound];
					}
				return null;
			}
		}

		/// find category recursively by key/path/elementname
		public XmlConfigurationCategory FindCategory(string keyOrPath)
		{
			try
			{
				/// chunk up the path into the separate categories
				string[] categories = keyOrPath.Split(XmlConfiguration.CategoryPathSeparators);
				
				/// at the first level, look for the first category in our list, if we find it and it's the last one then return it
				foreach(XmlConfigurationCategory category in base.InnerList)
				{
					if (category.ElementName == categories[0])
					{
						if (categories.Length == 1)
						{
							return category;						
						}
						else
						{
							/// chomp the first category off
							keyOrPath = string.Join(XmlConfiguration.DefaultPathSeparator, categories, 1, categories.Length - 1);
							return category.FindCategory(keyOrPath);
						}
					}
				}							
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
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
				{
					return _parent.Fullpath;
				}				
			}
		}
		
		/// <summary>
		/// Gets or sets the element to which this collection is a child (Either a XmlConfiguration or a XmlConfigurationCategory)
		/// </summary>
		[Browsable(false)]
		public XmlConfigurationElement Parent
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

		[Browsable(false)]
		public XmlConfiguration Configuration
		{
			get
			{
				if (_parent != null)
				{
					if (_parent.GetElementType() == XmlConfigurationElementTypes.XmlConfiguration)
						return ((XmlConfiguration)_parent).Configuration;

					if (_parent.GetElementType() == XmlConfigurationElementTypes.XmlConfigurationCategory)
						return ((XmlConfigurationCategory)_parent).Configuration;
				}
				return null;
			}
		}

		#endregion

		#region ICloneable Members

		/// <summary>
		/// Clones this category collection
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			XmlConfigurationCategoryCollection clone = new XmlConfigurationCategoryCollection();
			clone.ResetBeforeEdit();
			clone.ResetChanged();
			clone.ResetAfterEdit();
			clone.ResetEditCancelled();
			clone.Parent = _parent;

			foreach(XmlConfigurationCategory category in base.InnerList)
			{
				XmlConfigurationCategory clonedCategory = (XmlConfigurationCategory)category.Clone();				
				clonedCategory.Parent = clone;
				clone.Add(clonedCategory);
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
				if (!_isBeingEdited)
				{									
					/// place the element in edit mode and clone ourself so that future changes will be redirected to the clone and not to ourself
					_isBeingEdited = true;

					foreach(XmlConfigurationCategory category in base.InnerList)
					{
						try
						{
							category.BeginEdit();
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

		public bool EndEdit()
		{
			try
			{
				if (_isBeingEdited)
				{									
					/// place the element in edit mode and clone ourself so that future changes will be redirected to the clone and not to ourself
					_isBeingEdited = false;
					
					foreach(XmlConfigurationCategory category in base.InnerList)
					{
						try
						{
							category.EndEdit();
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

		public virtual bool CancelEdit()
		{
			try
			{
				if (_isBeingEdited)
				{
					_isBeingEdited = false;

					foreach(XmlConfigurationCategory category in base.InnerList)
					{
						try
						{
							category.CancelEdit();
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
				bool anyCategory = false;
				foreach(XmlConfigurationCategory category in base.InnerList)
					if (category.HasChanges)
						anyCategory = true;
				return _hasChanges || anyCategory;
			}
			set
			{
				_hasChanges = value;
			}
		}

		public void AcceptChanges()
		{
			foreach(XmlConfigurationCategory category in base.InnerList)
				category.AcceptChanges();				
			_hasChanges = false;
		}

		public bool ApplyChanges(ISupportsEditing editableObject, Razor.Configuration.SupportedEditingActions actions)
		{
			XmlConfigurationCategoryCollection categories = editableObject as XmlConfigurationCategoryCollection;
			if (categories != null)
			{					
				foreach(XmlConfigurationCategory category in categories)
				{					
					XmlConfigurationCategory myCategory = this[category.ElementName];
					if (myCategory != null)
					{
						try
						{
							myCategory.ApplyChanges((ISupportsEditing)category, actions);
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
			XmlConfigurationCategoryCollection categories = editableObject as XmlConfigurationCategoryCollection;
			if (categories != null)
			{	
				foreach(XmlConfigurationCategory category in categories)
				{					
					XmlConfigurationCategory myCategory = this[category.ElementName];
					if (myCategory != null)
					{
						try
						{
							myCategory.ApplyToSelf((ISupportsEditing)category, actions);
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

			foreach(XmlConfigurationCategory category in base.InnerList)
				category.BeginInit();
		}

		public virtual void EndInit()
		{
			_isBeingInitialized = false;

			foreach(XmlConfigurationCategory category in base.InnerList)
				category.EndInit();
		}

		#endregion
	}
}
