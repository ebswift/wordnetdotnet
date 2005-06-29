using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Summary description for AddressBookDirectoryBrowseWindow.
	/// </summary>
	public class AddressBookDirectoryBrowseWindow : System.Windows.Forms.Form
	{
		protected AddressBookDirectory _directory;
		protected AddressBookDirectory _selectedDirectory;
		protected bool _isAssitingChecking;
		protected bool _checkRootItems;
		private System.Windows.Forms.Label _labelInstructions;
		private System.Windows.Forms.Button _buttonCancel;
		private System.Windows.Forms.Button _buttonOK;
		private System.Windows.Forms.TreeView _treeView;
		private System.Windows.Forms.ImageList _imageList;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Initializes a new instance of the AddressBookDirectoryBrowseWindow class
		/// </summary>
		/// <param name="directory"></param>
		public AddressBookDirectoryBrowseWindow(AddressBookDirectory directory)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_treeView.CheckBoxes = true;
			_treeView.AfterCheck += new TreeViewEventHandler(OnAfterNodeCheck);
			
			_directory = directory;

			this.AcceptButton = _buttonOK;
			this.CancelButton = _buttonCancel;

			_buttonOK.Click += new EventHandler(OnButtonOKClicked);
			_buttonCancel.Click += new EventHandler(OnButtonCancelClicked);			
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AddressBookDirectoryBrowseWindow));
			this._buttonCancel = new System.Windows.Forms.Button();
			this._buttonOK = new System.Windows.Forms.Button();
			this._treeView = new System.Windows.Forms.TreeView();
			this._labelInstructions = new System.Windows.Forms.Label();
			this._imageList = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// _buttonCancel
			// 
			this._buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._buttonCancel.Location = new System.Drawing.Point(205, 236);
			this._buttonCancel.Name = "_buttonCancel";
			this._buttonCancel.TabIndex = 0;
			this._buttonCancel.Text = "Cancel";
			// 
			// _buttonOK
			// 
			this._buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._buttonOK.Location = new System.Drawing.Point(125, 236);
			this._buttonOK.Name = "_buttonOK";
			this._buttonOK.TabIndex = 1;
			this._buttonOK.Text = "OK";
			// 
			// _treeView
			// 
			this._treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._treeView.ImageList = this._imageList;
			this._treeView.Location = new System.Drawing.Point(10, 50);
			this._treeView.Name = "_treeView";
			this._treeView.Size = new System.Drawing.Size(270, 176);
			this._treeView.TabIndex = 2;
			// 
			// _labelInstructions
			// 
			this._labelInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._labelInstructions.BackColor = System.Drawing.SystemColors.Control;
			this._labelInstructions.Location = new System.Drawing.Point(10, 10);
			this._labelInstructions.Name = "_labelInstructions";
			this._labelInstructions.Size = new System.Drawing.Size(270, 35);
			this._labelInstructions.TabIndex = 4;
			// 
			// _imageList
			// 
			this._imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this._imageList.ImageSize = new System.Drawing.Size(16, 16);
			this._imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_imageList.ImageStream")));
			this._imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// AddressBookDirectoryBrowseWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 272);
			this.Controls.Add(this._labelInstructions);
			this.Controls.Add(this._treeView);
			this.Controls.Add(this._buttonOK);
			this.Controls.Add(this._buttonCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AddressBookDirectoryBrowseWindow";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Browse";
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		/// <summary>
		/// Overrides the load event to display the address book directory
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);

			this.DisplayAddressBookDirectory(_directory);
		}

		/// <summary>
		/// Intercepts the AfterNodeCheck event and assists checking or unchecking the remaining nodes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnAfterNodeCheck(object sender, TreeViewEventArgs e)
		{
			if (_isAssitingChecking)
				return;

			this.AssistNodeChecking(e.Node);
		}

		/// <summary>
		/// Handles the OK button click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnButtonOKClicked(object sender, EventArgs e)
		{
			// create the directory that represents the items that were selcted
			_selectedDirectory = this.CreateSelectedDirectory();

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		/// <summary>
		/// Handles the Cancel button click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnButtonCancelClicked(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		#endregion

		#region Public Properties
		
		/// <summary>
		/// Gets or sets a flag that indicates whether root items are checked when the items are initially displayed
		/// </summary>
		public bool CheckRootItems
		{
			get
			{
				return _checkRootItems;
			}
			set
			{
				_checkRootItems = value;
			}
		}

		/// <summary>
		/// Gets or sets the instructions for the dialog
		/// </summary>
		public string Instructions
		{
			get
			{
				return _labelInstructions.Text;
			}
			set
			{
				_labelInstructions.Text = value;
			}
		}

		/// <summary>
		/// Returns an address book directory containing the selected items
		/// </summary>
		public virtual AddressBookDirectory SelectedDirectory
		{
			get
			{
				return _selectedDirectory;
			}
		}
		
		#endregion

		#region Protected Methods

		/// <summary>
		/// Displays the address book directory using the tree view
		/// </summary>
		/// <param name="directory"></param>
		protected virtual void DisplayAddressBookDirectory(AddressBookDirectory directory)
		{
			try
			{
				_treeView.SuspendLayout();
				_treeView.Nodes.Clear();

				AddressBookDirectoryTreeNode directoryNode = new AddressBookDirectoryTreeNode(directory);
				_treeView.Nodes.Add(directoryNode);
				
				foreach(AddressBook addressBook in directory.Books)
				{
					AddressBookTreeNode bookNode = new AddressBookTreeNode(addressBook);
					directoryNode.Nodes.Add(bookNode);

					foreach(AddressBookItem addressBookItem in addressBook.Items)
					{
						AddressBookItemTreeNode itemNode = new AddressBookItemTreeNode(addressBookItem);
						bookNode.Nodes.Add(itemNode);
					}
				}
				
				directoryNode.Expand();
				directoryNode.Checked = _checkRootItems;
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
			finally
			{
				_treeView.ResumeLayout(true);
			}
		}

		/// <summary>
		/// Creates a new address book directory containing only the items that were selected
		/// </summary>
		/// <returns></returns>
		protected virtual AddressBookDirectory CreateSelectedDirectory()
		{
			// create a new directory
			AddressBookDirectory directory = new AddressBookDirectory(_directory.Name);			
			
			// start with the root node which is the directory
			foreach(AddressBookDirectoryTreeNode directoryNode in _treeView.Nodes)
			{
				// check each book node
				foreach(AddressBookTreeNode bookNode in directoryNode.Nodes)
				{
					// if it is selected
					if (bookNode.Checked)
					{
						// take the book for the selected node
						AddressBook book = bookNode.Book;

						// clear it's items
						book.Items.Clear();

						// add it to the directory
						directory.Books.Add(book);

						// check each item node
						foreach(AddressBookItemTreeNode itemNode in bookNode.Nodes)
						{
							// if it is selected
							if (itemNode.Checked)
							{					
								// add it to the book
								book.Items.Add(itemNode.Item);
							}
						}												
					}
				}
			}

			return directory;
		}
			
		#endregion
					
		#region Node Checking Methods

		/// <summary>
		/// Assists checking the node's children and parent nodes based on the nodes state and siblings 
		/// </summary>
		/// <param name="target"></param>
		protected virtual void AssistNodeChecking(TreeNode target)
		{
			try
			{
				_isAssitingChecking = true;
				
				this.CheckDownwards(target);
				this.CheckUpwards(target);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
				MessageBox.Show(this, ex.ToString(), "Exception Encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				_isAssitingChecking = false;
			}

		}
		
		/// <summary>
		/// Determines if a node has siblings that are checked
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		protected virtual bool HasSiblingsThatAreChecked(TreeNode target)
		{
			if (target != null)
			{
				if (target.Parent != null)
				{
					foreach(TreeNode sibling in target.Parent.Nodes)
					{
						if (target != sibling)
						{
							if (sibling.Checked)
								return true;
						}
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Determines if a node has siblings that have children that are checked
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		protected virtual bool HasSiblingsThatHaveChildrenThatAreChecked(TreeNode target)
		{
			if (target != null)
			{
				if (target.Parent != null)
				{
					foreach(TreeNode sibling in target.Parent.Nodes)
					{
						if (target != sibling)
						{
							bool hasAChildChecked = false;
							foreach(TreeNode child in sibling.Nodes)
							{
								if (child.Checked)
								{
									hasAChildChecked = true;
									break;
								}
							}

							if (!hasAChildChecked)
							{
								foreach(TreeNode child in sibling.Nodes)
								{
									hasAChildChecked = this.HasSiblingsThatHaveChildrenThatAreChecked(child);
									if (hasAChildChecked)
										return true;
								}
							}
						}
					}
				}
			}
			return false;
		}
		
		/// <summary>
		/// Checks the parent nodes of a node recursively upwards based on the node's state
		/// </summary>
		/// <param name="target"></param>
		protected virtual void CheckUpwards(TreeNode target)
		{
			if (target != null)
			{
				if (target.Parent != null)
				{
					bool siblingChecked = this.HasSiblingsThatAreChecked(target);
					bool siblingHasChildrenThatAreChecked = this.HasSiblingsThatHaveChildrenThatAreChecked(target);

					if (target.Parent.Checked && !siblingHasChildrenThatAreChecked && !siblingChecked)
						target.Parent.Checked = false;

					if (!target.Parent.Checked && target.Checked)
						target.Parent.Checked = true;

					this.CheckUpwards(target.Parent);
				}
			}
		}

		/// <summary>
		/// Checks the child nodes of a node recursively downwards based on the node's state
		/// </summary>
		/// <param name="target"></param>
		protected virtual void CheckDownwards(TreeNode target)
		{
			if (target != null)
			{
				foreach(TreeNode child in target.Nodes)
				{
					if (child.Checked != target.Checked)
						child.Checked = target.Checked;

					this.CheckDownwards(child);
				}
			}
		}
		
		#endregion

		#region Internal TreeNode Classes

		#region AddressBookDirectoryTreeNode

		private class AddressBookDirectoryTreeNode : TreeNode 
		{
			protected AddressBookDirectory _directory;

			public AddressBookDirectoryTreeNode(AddressBookDirectory directory) : base(directory.Name, 0 /* image index */, 0 /* selected image index */)
			{
				_directory = directory;				
			}

			public AddressBookDirectory Directory
			{
				get
				{
					return _directory;
				}
			}
		}
		
		#endregion

		#region AddressBookTreeNode

		private class AddressBookTreeNode : TreeNode
		{
			protected AddressBook _addressBook;

			public AddressBookTreeNode(AddressBook addressBook) : base(addressBook.Name, 1 /* image index */, 1 /* selected image index */)
			{
				_addressBook = addressBook;
			}

			public AddressBook Book
			{
				get
				{
					return _addressBook;
				}
			}
		}
		
		#endregion

		#region AddressBookItemTreeNode

		private class AddressBookItemTreeNode : TreeNode
		{
			protected AddressBookItem _item;

			public AddressBookItemTreeNode(AddressBookItem item) : base(item.Name, 2 /* image index */, 2 /* selected image index */)
			{
				_item = item;
			}

			public AddressBookItem Item
			{
				get
				{
					return _item;
				}
			}
		}
		
		#endregion

		#endregion
	}	
}
