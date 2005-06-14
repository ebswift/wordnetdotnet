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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for XmlConfigurationView.
	/// </summary>
	public class XmlConfigurationView : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// Defines the indexes of the images associated with categories displayed in the treeview
		/// </summary>
		public enum CategoryImageIndexes
		{
			/// <summary>
			/// No image
			/// </summary>
			NoImage = 3,
			
			/// <summary>
			/// The category is unselected with sub categories (displayed as a closed folder)
			/// </summary>
			UnselectedWithSubCategories = 0,

			/// <summary>
			/// The category is selected with sub categories (displayed as an open folder)
			/// </summary>
			SelectedWithSubCategories = 1,

			/// <summary>
			/// The category is unselected without sub categories (displayed with no image)
			/// </summary>
			UnselectedWithoutSubCategories = NoImage,

			/// <summary>
			/// The category is selected without sub categories (displayed with an arrow)
			/// </summary>
			SelectedWithoutSubCategories = 2,			
		}

		/// <summary>
		/// The configurations that are currently selected into the control
		/// </summary>
		private XmlConfigurationCollection _selectedConfigurations;
		private bool _placeElementsIntoEditMode;

		public event XmlConfigurationElementEventHandler ConfigurationChanged;

		/// <summary>
		/// The various controls that are used to display the configurations
		/// </summary>
		private System.Windows.Forms.TabControl tabControlMain;
		private System.Windows.Forms.TabPage tabPagePropertyPages;
		private System.Windows.Forms.Panel rootPanel;
		private System.Windows.Forms.Panel optionsPanel;
		private System.Windows.Forms.Splitter _splitter;
		private System.Windows.Forms.Panel categoriesPanel;
		private System.Windows.Forms.TreeView _treeView;
		private System.Windows.Forms.TabPage tabPageXml;
		private System.Windows.Forms.TabControl tabControlXmlViews;
		private System.Windows.Forms.ImageList _imageList;
		private System.Windows.Forms.ContextMenu _contextMenu;
		private System.Windows.Forms.Label _labelCategory;
		private System.Windows.Forms.PropertyGrid _propertyGrid;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationView class
		/// </summary>
		public XmlConfigurationView()
		{
			this.InitializeComponent();

			_selectedConfigurations = new XmlConfigurationCollection();

			// property grid
			_propertyGrid.HelpVisible = true;
			_propertyGrid.ToolbarVisible = true;
			
			// splitter
			_splitter.MouseEnter += new EventHandler(OnMouseEnterSplitter);
			_splitter.MouseLeave += new EventHandler(OnMouseLeaveSplitter);

			// treeview
			_treeView.AfterSelect += new TreeViewEventHandler(OnAfterNodeSelected);
			_treeView.ImageList = _imageList;

			_placeElementsIntoEditMode = true;

			_contextMenu.Popup += new EventHandler(OnGridContextMenuPoppedUp);
//			this.ClearNodes();
//			this.ClearXmlTabPages();
		}

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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(XmlConfigurationView));
			this.tabControlMain = new System.Windows.Forms.TabControl();
			this.tabPagePropertyPages = new System.Windows.Forms.TabPage();
			this.rootPanel = new System.Windows.Forms.Panel();
			this.optionsPanel = new System.Windows.Forms.Panel();
			this._contextMenu = new System.Windows.Forms.ContextMenu();
			this._splitter = new System.Windows.Forms.Splitter();
			this.categoriesPanel = new System.Windows.Forms.Panel();
			this._treeView = new System.Windows.Forms.TreeView();
			this.tabPageXml = new System.Windows.Forms.TabPage();
			this.tabControlXmlViews = new System.Windows.Forms.TabControl();
			this._imageList = new System.Windows.Forms.ImageList(this.components);
			this._labelCategory = new System.Windows.Forms.Label();
			this._propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.tabControlMain.SuspendLayout();
			this.tabPagePropertyPages.SuspendLayout();
			this.rootPanel.SuspendLayout();
			this.optionsPanel.SuspendLayout();
			this.categoriesPanel.SuspendLayout();
			this.tabPageXml.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControlMain
			// 
			this.tabControlMain.Controls.Add(this.tabPagePropertyPages);
			this.tabControlMain.Controls.Add(this.tabPageXml);
			this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlMain.Location = new System.Drawing.Point(0, 0);
			this.tabControlMain.Multiline = true;
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.SelectedIndex = 0;
			this.tabControlMain.Size = new System.Drawing.Size(470, 260);
			this.tabControlMain.TabIndex = 5;
			// 
			// tabPagePropertyPages
			// 
			this.tabPagePropertyPages.Controls.Add(this.rootPanel);
			this.tabPagePropertyPages.Location = new System.Drawing.Point(4, 22);
			this.tabPagePropertyPages.Name = "tabPagePropertyPages";
			this.tabPagePropertyPages.Size = new System.Drawing.Size(462, 234);
			this.tabPagePropertyPages.TabIndex = 0;
			this.tabPagePropertyPages.Text = "Property Pages";
			// 
			// rootPanel
			// 
			this.rootPanel.Controls.Add(this.optionsPanel);
			this.rootPanel.Controls.Add(this._splitter);
			this.rootPanel.Controls.Add(this.categoriesPanel);
			this.rootPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rootPanel.Location = new System.Drawing.Point(0, 0);
			this.rootPanel.Name = "rootPanel";
			this.rootPanel.Size = new System.Drawing.Size(462, 234);
			this.rootPanel.TabIndex = 1;
			// 
			// optionsPanel
			// 
			this.optionsPanel.Controls.Add(this._propertyGrid);
			this.optionsPanel.Controls.Add(this._labelCategory);
			this.optionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.optionsPanel.DockPadding.Left = 3;
			this.optionsPanel.Location = new System.Drawing.Point(205, 0);
			this.optionsPanel.Name = "optionsPanel";
			this.optionsPanel.Size = new System.Drawing.Size(257, 234);
			this.optionsPanel.TabIndex = 4;
			// 
			// _splitter
			// 
			this._splitter.BackColor = System.Drawing.SystemColors.Control;
			this._splitter.Location = new System.Drawing.Point(200, 0);
			this._splitter.Name = "_splitter";
			this._splitter.Size = new System.Drawing.Size(5, 234);
			this._splitter.TabIndex = 3;
			this._splitter.TabStop = false;
			// 
			// categoriesPanel
			// 
			this.categoriesPanel.Controls.Add(this._treeView);
			this.categoriesPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.categoriesPanel.DockPadding.Right = 3;
			this.categoriesPanel.Location = new System.Drawing.Point(0, 0);
			this.categoriesPanel.Name = "categoriesPanel";
			this.categoriesPanel.Size = new System.Drawing.Size(200, 234);
			this.categoriesPanel.TabIndex = 2;
			// 
			// _treeView
			// 
			this._treeView.BackColor = System.Drawing.SystemColors.Window;
			this._treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._treeView.ImageIndex = -1;
			this._treeView.Location = new System.Drawing.Point(0, 0);
			this._treeView.Name = "_treeView";
			this._treeView.SelectedImageIndex = -1;
			this._treeView.ShowLines = false;
			this._treeView.Size = new System.Drawing.Size(197, 234);
			this._treeView.Sorted = true;
			this._treeView.TabIndex = 0;
			// 
			// tabPageXml
			// 
			this.tabPageXml.Controls.Add(this.tabControlXmlViews);
			this.tabPageXml.Location = new System.Drawing.Point(4, 22);
			this.tabPageXml.Name = "tabPageXml";
			this.tabPageXml.Size = new System.Drawing.Size(462, 234);
			this.tabPageXml.TabIndex = 1;
			this.tabPageXml.Text = "Xml";
			this.tabPageXml.Visible = false;
			// 
			// tabControlXmlViews
			// 
			this.tabControlXmlViews.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlXmlViews.Location = new System.Drawing.Point(0, 0);
			this.tabControlXmlViews.Name = "tabControlXmlViews";
			this.tabControlXmlViews.SelectedIndex = 0;
			this.tabControlXmlViews.Size = new System.Drawing.Size(462, 234);
			this.tabControlXmlViews.TabIndex = 0;
			// 
			// _imageList
			// 
			this._imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this._imageList.ImageSize = new System.Drawing.Size(16, 16);
			this._imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_imageList.ImageStream")));
			this._imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// _labelCategory
			// 
			this._labelCategory.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this._labelCategory.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._labelCategory.Dock = System.Windows.Forms.DockStyle.Top;
			this._labelCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelCategory.ForeColor = System.Drawing.SystemColors.HighlightText;
			this._labelCategory.Location = new System.Drawing.Point(3, 0);
			this._labelCategory.Name = "_labelCategory";
			this._labelCategory.Size = new System.Drawing.Size(254, 20);
			this._labelCategory.TabIndex = 4;
			// 
			// _propertyGrid
			// 
			this._propertyGrid.BackColor = System.Drawing.SystemColors.Control;
			this._propertyGrid.CommandsVisibleIfAvailable = true;
			this._propertyGrid.ContextMenu = this._contextMenu;
			this._propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this._propertyGrid.LargeButtons = false;
			this._propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this._propertyGrid.Location = new System.Drawing.Point(3, 20);
			this._propertyGrid.Name = "_propertyGrid";
			this._propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this._propertyGrid.Size = new System.Drawing.Size(254, 214);
			this._propertyGrid.TabIndex = 5;
			this._propertyGrid.Text = "PropertyGrid";
			this._propertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this._propertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// XmlConfigurationView
			// 
			this.Controls.Add(this.tabControlMain);
			this.Name = "XmlConfigurationView";
			this.Size = new System.Drawing.Size(470, 260);
			this.tabControlMain.ResumeLayout(false);
			this.tabPagePropertyPages.ResumeLayout(false);
			this.rootPanel.ResumeLayout(false);
			this.optionsPanel.ResumeLayout(false);
			this.categoriesPanel.ResumeLayout(false);
			this.tabPageXml.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Overrides

		/// <summary>
		/// Override the parent changing event, and bind to the parent forms
		/// </summary>
		/// <param name="e"></param>
		protected override void OnParentChanged(EventArgs e)
		{			
			if (this.ParentForm != null)
				this.ParentForm.Closed += new EventHandler(OnParentFormClosed);

			base.OnParentChanged (e);
		}

		#endregion		

		#region Public Methods

		private delegate void AddConfigurationInvoker(XmlConfiguration configuration);

		/// <summary>
		/// Adds the specified configuration to the selected configurations displayed by this control
		/// </summary>
		/// <param name="configuration"></param>
		public void AddConfiguration(XmlConfiguration configuration)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new AddConfigurationInvoker(this.AddConfiguration), new object[] {configuration});
				return;
			}

			try
			{
				if (configuration != null)
				{				
					configuration.Changed += new XmlConfigurationElementEventHandler(OnConfigurationChanged);
					if (_placeElementsIntoEditMode)
						configuration.BeginEdit();		
					_selectedConfigurations.Add(configuration);		
					this.AddNodesForCategories(_treeView, null, configuration.Categories);							
					this.AddXmlTabForConfiguration(configuration);
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}
        
		private delegate void RemoveConfigurationInvoker(XmlConfiguration configuration, bool keepLocationIfPossible);

		/// <summary>
		/// Removes the specified configuration from the selected configurations diplayed by this control, optionally attempts to restore the location
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="keepLocationIfPossible"></param>
		public void RemoveConfiguration(XmlConfiguration configuration, bool keepLocationIfPossible)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new RemoveConfigurationInvoker(this.RemoveConfiguration), new object[] {configuration, keepLocationIfPossible});
				return;
			}

			try
			{
				if (configuration != null)
				{
					if (_selectedConfigurations.Contains(configuration))
					{
						string path = null;
						TreeNode n = _treeView.SelectedNode;
						if (n != null)
							path = n.FullPath;
						
						_selectedConfigurations.Remove(configuration);

						this.RefreshDisplay(path, keepLocationIfPossible);
					}
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		private delegate bool SelectPathInvoker(string path);

		/// <summary>
		/// Attempts to select the node specified by the given path
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public bool SelectPath(string path)
		{
			if (this.InvokeRequired)
			{
				return (bool)this.Invoke(new SelectPathInvoker(this.SelectPath), new object[] {path});				
			}

			try
			{
				return this.SelectPathFromNodes(_treeView.Nodes, path);			
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}

		/// <summary>
		/// Refreshes the categories and the selected category and the options contained therein
		/// </summary>
		public void RefreshDisplay()
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new MethodInvoker(this.RefreshDisplay), new object[] {});
				return;
			}

			try
			{
				string path = null;
				TreeNode n = _treeView.SelectedNode;
				if (n != null)
					path = n.FullPath;
				this.RefreshDisplay(path, true);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		private delegate void RefreshDisplayInvoker(string path, bool keepLocationIfPossible);

		/// <summary>
		/// Refreshes the display, and conditionally selects the specified path if possible
		/// </summary>
		/// <param name="path"></param>
		/// <param name="keepLocationIfPossible"></param>
		public void RefreshDisplay(string path, bool keepLocationIfPossible)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new RefreshDisplayInvoker(this.RefreshDisplay), new object[] {path, keepLocationIfPossible});
				return;
			}

			try
			{
				this.ClearNodes();			
				this.ClearXmlTabPages();
								
				foreach(XmlConfiguration configuration in _selectedConfigurations)
				{
					this.AddNodesForCategories(_treeView, null, configuration.Categories);						
					this.AddXmlTabForConfiguration(configuration);
				}			

				if (keepLocationIfPossible)
					this.SelectPath(path);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Debug.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Refreshes the Xml view of the selected configurations
		/// </summary>
		public void RefreshXmlDisplay()
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new MethodInvoker(this.RefreshXmlDisplay), new object[] {});
				return;
			}

			try
			{
				this.ClearXmlTabPages();
				
				foreach(XmlConfiguration configuration in _selectedConfigurations)
					this.AddXmlTabForConfiguration(configuration);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Debug.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Applies the changes of the selected configuration to the original configuration
		/// </summary>
		public void ApplyChanges()
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new MethodInvoker(this.ApplyChanges), new object[] {});
				return;
			}

			foreach(XmlConfiguration configuration in _selectedConfigurations)
			{
				if (configuration.IsBeingEdited)
					configuration.EndEdit();

				if (_placeElementsIntoEditMode)
					configuration.BeginEdit();
			}
		}

		/// <summary>
		/// Cancels the changes to the selected configuration
		/// </summary>
		public void CancelEdit()
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new MethodInvoker(this.CancelEdit), new object[] {});
				return;
			}

			foreach(XmlConfiguration configuration in _selectedConfigurations)
				if (configuration.IsBeingEdited)
					configuration.CancelEdit();														 
		}

		/// <summary>
		/// Displays a warning if any of the current configurations do not have sufficient security access permissions for write access on the local computer for the current user
		/// </summary>
		public void DisplayWarningIfLocalFilePermissionsAreInsufficient()
		{
			if (_selectedConfigurations == null)
				return;

			try
			{
				bool anyAreDeniedWriteAccess = false;
				foreach(XmlConfiguration configuration in _selectedConfigurations)
				{						
					string path = configuration.Path;
					if (path != null && path != string.Empty)
					{
						if (System.IO.File.Exists(path))
						{
							using(SecurityAccessRight right = new SecurityAccessRight(path))
							{
								bool noWrite = right.AssertWriteAccess();
								if (!noWrite)
									anyAreDeniedWriteAccess = true;	
							}
						}
						else
						{
							string dir = System.IO.Path.GetDirectoryName(path);
							using(SecurityAccessRight right = new SecurityAccessRight(dir))
							{
								bool noWrite = right.AssertWriteAccess();
								if (!noWrite)
									anyAreDeniedWriteAccess = true;
							}								
						}
					}
				}

				if (anyAreDeniedWriteAccess)
				{
					ExceptionEngine.DisplayException(
						this,
						"Security restriction detected - Write access is denied to one or more configuration files",
						MessageBoxIcon.Information,
						MessageBoxButtons.OK,
						null,
						"One or more of the configuration files that store the options you are about to configure, has been denied write access for the current user '" + System.Environment.UserName + "'.",
						"You may continue and make changes to the options as normal, however some options may not be saved when the appliation exits.",
						"Please contact your system administrator for questions regarding Windows security and access rights.");
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		#endregion		

		#region Private Methods

		/// <summary>
		/// Clears the nodes from the treeview
		/// </summary>
		private void ClearNodes()
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new MethodInvoker(this.ClearNodes), new object[] {});
				return;
			}

			try
			{
				this._treeView.Nodes.Clear();
				this._propertyGrid.SelectedObject = null;
				_labelCategory.Text = null;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Debug.WriteLine(systemException);
			}			
		}

		/// <summary>
		/// Clears the tab pages from the xml tab control
		/// </summary>
		private void ClearXmlTabPages()
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new MethodInvoker(this.ClearXmlTabPages), new object[] {});
				return;
			}

			try
			{
				foreach(TabPage page in this.tabControlXmlViews.TabPages)
					this.tabControlXmlViews.TabPages.Remove(page);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Debug.WriteLine(systemException);
			}
		}

		private delegate bool SelectPathFromNodesInvoker(TreeNodeCollection nodes, string path);

		/// <summary>
		/// Recursively expands and selects nodes down to the last node found by searching the specified path
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		private bool SelectPathFromNodes(TreeNodeCollection nodes, string path)
		{
			if (this.InvokeRequired)
			{
				return (bool)this.Invoke(new SelectPathFromNodesInvoker(this.SelectPathFromNodes), new object[] {nodes, path});
			}

			string[] paths = path.Split(new char[] {'\\'});

			if (paths.Length > 0)
			{
				if (nodes != null)
				{
					foreach(TreeNode n in nodes)
					{
						if (string.Compare(n.Text, paths[0], true) == 0)
						{
							if (paths.Length == 1)
							{
								n.Expand();
								_treeView.SelectedNode = n;
								return true;
							}
							else
							{
								n.Expand();
								path = string.Join("\\", paths, 1, paths.Length-1);
								return this.SelectPathFromNodes(n.Nodes, path);
							}
						}
					}
				}
			}
			return false;
		}

		private delegate void AddNodesForCategoriesInvoker(TreeView tree, TreeNodeCollection nodes, XmlConfigurationCategoryCollection categories);

		/// <summary>
		/// Recursively adds nodes for the specified categories
		/// </summary>
		/// <param name="tree"></param>
		/// <param name="nodes"></param>
		/// <param name="categories"></param>
		private void AddNodesForCategories(TreeView tree, TreeNodeCollection nodes, XmlConfigurationCategoryCollection categories)
		{			
			if (this.InvokeRequired)
			{
				this.Invoke(new AddNodesForCategoriesInvoker(this.AddNodesForCategories), new object[] {tree, nodes, categories});
				return;
			}

			tree.BeginUpdate();
			try
			{
				foreach(XmlConfigurationCategory category in categories)
				{					
					if (!category.Hidden)
					{
						// try and find an existing node that we can merge with
						CategoryTreeNode n = this.FindNodeForCategory(nodes, category);												
						
						if (n == null)
							n = this.AddNodeForCategory(tree, nodes, category);
												
						if (n != null)
						{
							if (!n.IsBoundToCategory(category))
								n.BindToCategory(category);

							this.AddNodesForCategories(tree, n.Nodes, category.Categories);						
						}
					}
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			finally
			{
				tree.EndUpdate();
			}
		}

		private delegate CategoryTreeNode FindNodeForCategoryInvoker(TreeNodeCollection nodes, XmlConfigurationCategory category);

		/// <summary>
		/// Finds an existing node for the specified category
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="category"></param>
		/// <returns></returns>
		private CategoryTreeNode FindNodeForCategory(TreeNodeCollection nodes, XmlConfigurationCategory category)
		{
			if (this.InvokeRequired)
			{
				return this.Invoke(new FindNodeForCategoryInvoker(this.FindNodeForCategory), new object[] {nodes, category}) as CategoryTreeNode;
			}

			if (nodes == null)
				// asume root
				nodes = _treeView.Nodes;

			if (nodes != null)
			{
				if (category != null)
				{
					foreach(CategoryTreeNode n in nodes)
					{
						//						if (n.IsBoundToCategory(category))
						//							return n;
						if (string.Compare(n.Text, category.DisplayName, true) == 0)
							return n;
					}
				}
			}
			return null;
		}

		private delegate CategoryTreeNode AddNodeForCategoryInvoker(TreeView tree, TreeNodeCollection nodes, XmlConfigurationCategory category);

		/// <summary>
		/// Adds a category node into the treeview for the specified category
		/// </summary>
		/// <param name="tree"></param>
		/// <param name="nodes"></param>
		/// <param name="category"></param>
		/// <returns></returns>
		private CategoryTreeNode AddNodeForCategory(TreeView tree, TreeNodeCollection nodes, XmlConfigurationCategory category)
		{
			if (this.InvokeRequired)
			{
				return this.Invoke(new AddNodeForCategoryInvoker(this.AddNodeForCategory), new object[] {tree, nodes, category}) as CategoryTreeNode;
			}

			bool isRootCategory = (nodes == null);

			if (nodes == null)
				if (tree != null)
					nodes = tree.Nodes;

			CategoryTreeNode n = new CategoryTreeNode(category.DisplayName);
			n.BindToCategory(category);
			nodes.Add(n);

			if (isRootCategory)
			{
				n.ImageIndex = (int)CategoryImageIndexes.UnselectedWithSubCategories;
				n.SelectedImageIndex = (int)CategoryImageIndexes.SelectedWithSubCategories;
			}
			else
			{
				/// ok, off the root, now base images on whether the category has sub categories or not, 
				/// and whether the category is selected
				if (category.Categories.Count > 0)
				{
					n.ImageIndex = (int)CategoryImageIndexes.UnselectedWithSubCategories;
					n.SelectedImageIndex = (int)CategoryImageIndexes.SelectedWithSubCategories;
				}
				else
				{
					n.ImageIndex = (int)CategoryImageIndexes.NoImage;
					n.SelectedImageIndex = (int)CategoryImageIndexes.SelectedWithoutSubCategories;
				}
			}

			return n;
		}

		private delegate void AddXmlTabForConfigurationInvoker(XmlConfiguration configuration);

		/// <summary>
		/// Adds an Xml tab for the specified configuration
		/// </summary>
		/// <param name="configuration"></param>
		private void AddXmlTabForConfiguration(XmlConfiguration configuration)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new AddXmlTabForConfigurationInvoker(this.AddXmlTabForConfiguration), new object[] {configuration});
				return;
			}

			try
			{
				this.tabControlXmlViews.SuspendLayout();

				XmlConfigurationXmlBehindViewer view = new XmlConfigurationXmlBehindViewer();
				view.Xml = configuration.ToXml();

				TabPage page = new TabPage(configuration.ElementName);
				page.Controls.Add(view);
				view.Parent = page;
				view.Dock = DockStyle.Fill;
				
				this.tabControlXmlViews.TabPages.Add(page);				
				this.tabControlXmlViews.ResumeLayout(true);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		private delegate void SelectPropertyDescriptorForNodeIntoPropertyGridInvoker(CategoryTreeNode node);

		/// <summary>
		/// Selects the node's type descriptor into the property grid
		/// </summary>
		/// <param name="n"></param>
		private void SelectPropertyDescriptorForNodeIntoPropertyGrid(CategoryTreeNode n)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new SelectPropertyDescriptorForNodeIntoPropertyGridInvoker(this.SelectPropertyDescriptorForNodeIntoPropertyGrid), new object[] {n});
				return;
			}
			
			try
			{
				_propertyGrid.SelectedObject = null;
				_labelCategory.Text = null;

				if (n != null)
				{
					if (n.Categories.Count > 0)
					{

						XmlConfigurationOptionCollectionTypeDescriptor td = new XmlConfigurationOptionCollectionTypeDescriptor(n.Categories);
						if (td != null)
							_propertyGrid.SelectedObject = td;

						foreach(DictionaryEntry entry in n.Categories)
						{
							XmlConfigurationCategory category = entry.Value as XmlConfigurationCategory;
							if (category != null)
							{
								_labelCategory.Text = category.DisplayName;
								break;
							}
						}
					}
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}	
		}

		#endregion				

		#region Public Properties
		
		public bool PlaceElementsIntoEditMode
		{
			get
			{
				return _placeElementsIntoEditMode;
			}
			set
			{
				_placeElementsIntoEditMode = value;
			}
		}


		/// <summary>
		/// Gets or sets the selected configuration for the Configuration Window. 
		/// This configuration will reflect any and all changes made using the gui.
		/// Use this.OriginalConfiguration to obtain the original configuration unchanged.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public XmlConfigurationCollection SelectedConfigurations
		{
			get
			{
				return _selectedConfigurations;
			}
			set
			{								
				this.ClearNodes();
				this.ClearXmlTabPages();
				_selectedConfigurations = new XmlConfigurationCollection();
	
				if (value != null)
				{
					foreach(XmlConfiguration configuration in value)
						this.AddConfiguration(configuration);
				}			
			}
		}

		#endregion				

		#region Private Event Handlers and Event Invokers

		/// <summary>
		/// Occurs when a configuration changes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnConfigurationChanged(object sender, XmlConfigurationElementEventArgs e)
		{
			try
			{
				if (this.ConfigurationChanged != null)
					this.ConfigurationChanged(sender, e);

//				System.Diagnostics.Trace.WriteLine(XmlConfiguration.DescribeElementChanging(e));
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Occurs when our parent form closes, we should unwire from the configuration events		
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnParentFormClosed(object sender, EventArgs e)
		{
			if (_selectedConfigurations != null)
				foreach(XmlConfiguration configuration in _selectedConfigurations)
					if (_placeElementsIntoEditMode)
						configuration.Changed -= new XmlConfigurationElementEventHandler(OnConfigurationChanged);
		}

		/// <summary>
		/// Occurs when the mouse enters the splitter
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseEnterSplitter(object sender, System.EventArgs e)
		{
			_splitter.BackColor = SystemColors.ControlDark;
		}

		/// <summary>
		/// Occurs when the mouse leaves the splitter
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseLeaveSplitter(object sender, System.EventArgs e)
		{
			_splitter.BackColor = SystemColors.Control;
		}

		/// <summary>
		/// Occurs after a tree node is selected
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAfterNodeSelected(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			this.SelectPropertyDescriptorForNodeIntoPropertyGrid(e.Node as CategoryTreeNode);		
		}

		#endregion
		
		/// <summary>
		/// Occurs when the "Has changes" menu item is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnToggleOptionHasChangesClicked(object sender, EventArgs e)
		{
			// grab one of our custom menu items
			XmlConfigurationOptionPropertyDescriptorMenuItem menuItem = sender as XmlConfigurationOptionPropertyDescriptorMenuItem;
			if (menuItem != null)
			{
				// and the option it points to
				XmlConfigurationOption option = menuItem.Option;
				if (option != null)
				{
					// toggle the check
					option.HasChanges = !menuItem.Checked;

					// if changed, then trigger the changed event for the option
					if (option.HasChanges)
						option.TriggerChange();
				}
			}			
		}

		/// <summary>
		/// Occurs when the context menu is popped up for the property grid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnGridContextMenuPoppedUp(object sender, EventArgs e)
		{
			// grab the grid
			PropertyGrid grid = this._propertyGrid;
			if (grid != null)
			{
				// grab the selected item
				if (grid.SelectedGridItem != null)
				{
					GridItem item = grid.SelectedGridItem;
					if (item != null)
					{
						// grab the descriptor as one of our option descriptors
						XmlConfigurationOptionPropertyDescriptor descriptor = item.PropertyDescriptor as XmlConfigurationOptionPropertyDescriptor;
						if (descriptor != null)
						{
							XmlConfigurationOption option = descriptor.Option;
							if (option != null)
							{
								// construct a new menu item for it
								XmlConfigurationOptionPropertyDescriptorMenuItem menuItem = new XmlConfigurationOptionPropertyDescriptorMenuItem("Has changes", new EventHandler(OnToggleOptionHasChangesClicked), option);
								// determine its checked state
								menuItem.Checked = option.HasChanges;
								// rinse and repeat								
								_contextMenu.MenuItems.Clear();
								_contextMenu.MenuItems.Add(menuItem);
							}
						}						
					}
				}
			}
		}
	}
}
