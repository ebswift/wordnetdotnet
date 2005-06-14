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
using System.Windows.Forms.Design;
using System.Xml;
using System.Reflection;
using System.IO;

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for TypeSelectionTypeEditorControl.
	/// </summary>
	public class TypeSelectionTypeEditorControl : System.Windows.Forms.UserControl
	{
		private const int TYPE_STRUCT = 21;
		private const int TYPE_INTERFACE = 17;
		private const int TYPE_CLASS = 13;
		private const int TYPE_ENUM = 25;

		#region ReflectionTreeNode Class

		public class ReflectionTreeNode: TreeNode
		{
			private Assembly _assembly;
			private Type _type;

			public ReflectionTreeNode(string text, int imageIndex, int selectedImageIndex, TreeNode[] children): base(text, imageIndex, selectedImageIndex, children)
			{			
			}

			public ReflectionTreeNode(string text, int imageIndex, int selectedImageIndex): base(text, imageIndex, selectedImageIndex)
			{			
			}

			public ReflectionTreeNode(string text, TreeNode[] children): base(text, children)
			{			
			}

			public ReflectionTreeNode(string text): base(text)
			{			
			}

			public ReflectionTreeNode(): base()
			{			
			}

			public Assembly Assembly
			{
				get
				{
					return _assembly;
				}
				set
				{
					_assembly = value;
				}
			}

			public Type Type
			{
				get
				{
					return _type;
				}
				set
				{
					_type = value;
				}
			}
		}

		#endregion

		private Hashtable _nodes;
		private IWindowsFormsEditorService _edSvc;
		private System.Windows.Forms.TabControl _tabControl;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.ImageList _imageList;
		private System.Windows.Forms.TreeView _treeViewGACTypes;
		private System.Windows.Forms.Button buttonGacOk;
		private System.Windows.Forms.Button buttonBrowse;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Initializes a new instance of the TypeSelectionTypeEditorControl class
		/// </summary>
		public TypeSelectionTypeEditorControl()
		{
			this.InitializeComponent();
			this.Load += new EventHandler(TypeSelectionControl_Load);
			_nodes = new Hashtable();
			_imageList = this.CreateImageListFromResourceStream(this.GetType(), new Size(16,16), true, new Point(0,0));			
			_treeViewGACTypes.ImageList = _imageList;
		}

		/// <summary>
		/// Occurs when the control is loaded for the first time
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TypeSelectionControl_Load(object sender, EventArgs e)
		{			
			this.DisplayTypes();
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
			this._tabControl = new System.Windows.Forms.TabControl();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this._treeViewGACTypes = new System.Windows.Forms.TreeView();
			this._imageList = new System.Windows.Forms.ImageList(this.components);
			this.buttonGacOk = new System.Windows.Forms.Button();
			this.buttonBrowse = new System.Windows.Forms.Button();
			this._tabControl.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tabControl
			// 
			this._tabControl.Controls.Add(this.tabPage2);
			this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tabControl.Location = new System.Drawing.Point(0, 0);
			this._tabControl.Name = "_tabControl";
			this._tabControl.SelectedIndex = 0;
			this._tabControl.Size = new System.Drawing.Size(215, 300);
			this._tabControl.TabIndex = 0;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.buttonBrowse);
			this.tabPage2.Controls.Add(this.buttonGacOk);
			this.tabPage2.Controls.Add(this._treeViewGACTypes);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(207, 274);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Types";
			// 
			// _treeViewGACTypes
			// 
			this._treeViewGACTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._treeViewGACTypes.ImageIndex = -1;
			this._treeViewGACTypes.Location = new System.Drawing.Point(3, 3);
			this._treeViewGACTypes.Name = "_treeViewGACTypes";
			this._treeViewGACTypes.SelectedImageIndex = -1;
			this._treeViewGACTypes.Size = new System.Drawing.Size(200, 235);
			this._treeViewGACTypes.Sorted = true;
			this._treeViewGACTypes.TabIndex = 0;
			// 
			// _imageList
			// 
			this._imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this._imageList.ImageSize = new System.Drawing.Size(16, 16);
			this._imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// buttonGacOk
			// 
			this.buttonGacOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonGacOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonGacOk.Location = new System.Drawing.Point(130, 245);
			this.buttonGacOk.Name = "buttonGacOk";
			this.buttonGacOk.TabIndex = 5;
			this.buttonGacOk.Text = "OK";
			this.buttonGacOk.Click += new System.EventHandler(this.buttonGacOk_Click);
			// 
			// buttonBrowse
			// 
			this.buttonBrowse.Location = new System.Drawing.Point(5, 245);
			this.buttonBrowse.Name = "buttonBrowse";
			this.buttonBrowse.TabIndex = 6;
			this.buttonBrowse.Text = "Browse...";
			this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
			// 
			// TypeSelectionTypeEditorControl
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this._tabControl);
			this.Name = "TypeSelectionTypeEditorControl";
			this.Size = new System.Drawing.Size(215, 300);
			this._tabControl.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Gets or sets the service currently controlling this control via the UITypeEditor that spawned this control.
		/// </summary>
		public IWindowsFormsEditorService WindowsFormsEditorService
		{
			get
			{
				return _edSvc;
			}
			set
			{
				_edSvc = value;
			}
		}
		
		/// <summary>
		/// Loads the standa
		/// </summary>
		private void DisplayTypes()
		{
			try
			{
				_treeViewGACTypes.BeginUpdate();

				string[] references = new string[] {"mscorlib, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "system", "system.drawing", "system.windows.forms"}; //, "adodb.dll"};
				foreach(string reference in references)

					this.LoadTypesFromReference (reference);						
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			finally
			{
				_treeViewGACTypes.EndUpdate();
			}
		}

		/// <summary>
		/// Loads all of the types found from the specified reference
		/// </summary>
		/// <param name="reference">The reference to load types from</param>
		public void LoadTypesFromReference(string reference)
		{
			try
			{
				Assembly assembly = this.LoadReference(reference);
				if (assembly != null)
				{
					// get the exported types from the assembly
					Type[] types = assembly.GetExportedTypes();				
					if (types != null)
					{				
						foreach(Type t in types)
						{
							if (t.IsPublic && (!t.IsAbstract))
							{
//								object[] attributes = t.GetCustomAttributes(typeof(SerializableAttribute), false);
//								bool isSerializable = false;
//								if ((attributes != null) && (attributes.Length > 0))
//								{
//									foreach(object attribute in attributes)
//										if (attribute is SerializableAttribute)
//										{
//											isSerializable = true;
//											break;
//										}
//								}
//								bool supportsISerializable = (t.GetInterface(typeof(System.Runtime.Serialization.ISerializable).FullName) != null);
//								if (isSerializable || supportsISerializable)
//								{
									ReflectionTreeNode namespaceParent = this.GetNodeByPath(null, t.Namespace);
									ReflectionTreeNode node = new ReflectionTreeNode(t.Name);
									node.Assembly = assembly;
									node.Type = t;
									int imageIndex = this.GetTypeImageIndex(t);
									node.ImageIndex = imageIndex;
									node.SelectedImageIndex = imageIndex;
									
									if (namespaceParent != null)
										namespaceParent.Nodes.Add(node);
//								}
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

		private Assembly LoadReference(string reference)
		{
			FileInfo file = new FileInfo(reference);
			Assembly assembly = null;
			try
			{
				if (!file.Exists)
					assembly = Assembly.LoadWithPartialName(reference);				
				else
					assembly = Assembly.LoadFrom(reference);
			}
			catch(Exception e)
			{
				MessageBox.Show(null, "Could not reference '" + reference + ".\nAn exception was encountered: " + e.Message, "Invalid Reference",MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			
			return assembly;
		}

		private int GetTypeImageIndex(Type t)
		{
			if (t == null)
				return -1;
			
			int imageIndex = -1;
							
			if (t.IsClass)
				imageIndex = TYPE_CLASS;
			else if (t.IsEnum)
				imageIndex = TYPE_ENUM;
			else if (t.IsInterface)
				imageIndex = TYPE_INTERFACE;
			else
				imageIndex = TYPE_STRUCT;
			
			if (t.IsNotPublic)
				imageIndex += 2;
			
			return imageIndex;
		}

		private ReflectionTreeNode GetNodeByPath(ReflectionTreeNode parent, string fullPath)
		{
			// bail if there is no path to look up
			if (fullPath == null)
				return null;

			// look in the cache first, before trying to do a ton of work to create the node
			if (_nodes.Contains(fullPath))
				return (ReflectionTreeNode)_nodes[fullPath];

			// snag hold of a node collection from the parent
			TreeNodeCollection nodes = (parent != null ? parent.Nodes : _treeViewGACTypes.Nodes);

			// split the node path down into it's individual paths
			String[] paths = fullPath.Split(new char[] {'.'});

			// make a node that will be the target or null when the full path is resolved
			ReflectionTreeNode targetNode = null;

			// start making the paths
			//            foreach(string path in paths)
			//			{				
			foreach(ReflectionTreeNode node in nodes)
			{
				if (node.Text == paths[0])
				{
					targetNode = node;
					break;
				}
			}

			if (targetNode == null)
			{
				// create it
				targetNode = new ReflectionTreeNode(paths[0]);
				targetNode.ImageIndex = 11;
				targetNode.SelectedImageIndex = 11;

				// add the node to the tree
				nodes.Add(targetNode);

				// cache it by it's full path
				_nodes.Add(targetNode.FullPath, targetNode);
			}
	
			// we found a path, recurse into it
			string subPath = string.Join(".", paths, 1, paths.Length - 1);

			if (subPath != null)
				if (subPath != string.Empty)
					targetNode = GetNodeByPath(targetNode, subPath);
			//			}

			return targetNode;
		}

		/// <summary>
		/// Gets a ReflectionTreeNode from a type's namespace
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="fullPath"></param>
		/// <returns></returns>
		private ReflectionTreeNode GetNode(ReflectionTreeNode parent, string fullPath)
		{
			if (fullPath == null)
				return null;
			
			// split up the namespace
			string[] namespaces = fullPath.Split(new char[] {'.'});
			
			ReflectionTreeNode node = null;

			if (namespaces.Length > 0)
			{
				string elementName = namespaces[0];
				
				if (parent == null)
				{
					if (_nodes.Contains(elementName))
						node = (ReflectionTreeNode)_nodes[elementName];
				}
				else
				{

				}

				if (node == null)
				{
					node = new ReflectionTreeNode(elementName);
					node.ImageIndex = 11;
					node.SelectedImageIndex = 11;
					
					if (parent == null)
						_treeViewGACTypes.Nodes.Add(node);
					else
						parent.Nodes.Add(node);

					_nodes.Add(elementName, node);
					// cache the node
					
				}
				
				string remainingPath = string.Join(".", namespaces, 1, namespaces.Length - 1);

				if (remainingPath != null)
					if (remainingPath != string.Empty)
						node = GetNode(node, remainingPath);
			}
						
			return node;
		}	

		private ImageList CreateImageListFromResourceStream(Type assemblyType, Size imageSize, bool makeTransparent, Point transparentPixel)
		{			
			ImageList imageList = new ImageList();
			imageList.ColorDepth = ColorDepth.Depth32Bit;
			imageList.ImageSize = imageSize;
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager("Razor.Configuration.Images", System.Reflection.Assembly.GetExecutingAssembly());
			Bitmap bitmap = (Bitmap)resources.GetObject("TreeIcons");
			if (bitmap != null)
			{
				if (makeTransparent)
				{
					Color backColor = bitmap.GetPixel(transparentPixel.X, transparentPixel.Y);
					bitmap.MakeTransparent(backColor);
				}
				imageList.Images.AddStrip(bitmap);
			}			    
			return imageList;
		}

				

		private void buttonGacOk_Click(object sender, System.EventArgs e)
		{
			if (_edSvc != null)
					_edSvc.CloseDropDown();
		}
		
		private void buttonBrowse_Click(object sender, System.EventArgs e)
		{
//			try
//			{
//				System.Windows.Forms.OpenFileDialog dialog = new OpenFileDialog();
//				dialog.a
//			}
//			catch(System.Exception systemException)
//			{
//				
//			}
		}

		/// <summary>
		/// Gets the Assembly containing the selected Type.
		/// </summary>
		public Assembly RefrencedAssembly
		{
			get
			{
				try
				{
					ReflectionTreeNode node = _treeViewGACTypes.SelectedNode as ReflectionTreeNode;
					if (node != null)
						return node.Assembly;					
				}
				catch(System.Exception)
				{
				}
				return null;
			}
		}

		/// <summary>
		/// Gets the Type selected in the selection control.
		/// </summary>
		public Type SelectedType
		{
			get
			{
				try
				{
					ReflectionTreeNode node = _treeViewGACTypes.SelectedNode as ReflectionTreeNode;
					if (node != null)
						return node.Type;					
				}
				catch(System.Exception)
				{
				}
				return null;
			}
		}
	}
}
