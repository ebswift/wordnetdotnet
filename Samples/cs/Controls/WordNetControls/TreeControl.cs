using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
namespace WordNetControls
{
	///*
	// * This file is a part of the WordNet.Net open source project.
	// * 
	// * Copyright (C) 2005 Malcolm Crowe, Troy Simpson 
	// * 
	// * Project Home: http://www.ebswift.com
	// *
	// * This library is free software; you can redistribute it and/or
	// * modify it under the terms of the GNU Lesser General Public
	// * License as published by the Free Software Foundation; either
	// * version 2.1 of the License, or (at your option) any later version.
	// *
	// * This library is distributed in the hope that it will be useful,
	// * but WITHOUT ANY WARRANTY; without even the implied warranty of
	// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
	// * Lesser General Public License for more details.
	// *
	// * You should have received a copy of the GNU Lesser General Public
	// * License along with this library; if not, write to the Free Software
	// * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
	// * 
	// * */

	//
	// Created by SharpDevelop.
	// User: simpsont
	// Date: 10/02/2006
	// Time: 10:25 AM

	//TODO: icons need to be created/obtained and appropriately assigned to the various results types.

	/// <summary>
	/// Standalone TreeView control for WordNet.  
	/// See the TreeView sample under the samples folder for an example of use.
	/// </summary>
	public partial class TreeControl
	{
		public event AfterSelectEventHandler AfterSelect;
		public delegate void AfterSelectEventHandler(object sender, System.Windows.Forms.TreeViewEventArgs e);
		public new event MouseDownEventHandler MouseDown;
		public delegate void MouseDownEventHandler(object sender, System.Windows.Forms.MouseEventArgs e);
		public event TreeRightClickEventHandler TreeRightClick;
		public delegate void TreeRightClickEventHandler(object sender, System.Windows.Forms.MouseEventArgs e, System.Windows.Forms.TreeNode tn);

		public TreeControl()
		{
			// The Me.InitializeComponent call is required for Windows Forms designer support.
			this.InitializeComponent();

			//
			// TODO : Add constructor code after InitializeComponents
			//

			TreeView1.AfterSelect += TreeView1AfterSelect;
			TreeView1.MouseDown += TreeView1MouseDown;
		}

		/// <summary>
		/// Begins the iterative process of filling the tree.
		/// </summary>
		/// <param name="list">An ArrayList of search objects to iterate</param>
		/// <param name="usepos">A Boolean to specify whether a part of speech should be used as ArrayList top node.  Generally this is not used when searching POS relations because the POS is already specified</param>
		public void fillTree(ArrayList list, bool usepos)
		{
			//Wnlib.Search ss = default(Wnlib.Search);
			TreeNode posnode = null;
			// part of speech node used for overview search

			BeginUpdate();
			Clear();

			foreach ( Wnlib.Search ss in list) {
				if (ss.senses.Count > 0) {
					if (usepos) {
						posnode = new TreeNode(ss.pos.name);
						TreeView1.Nodes.Add(posnode);
					}

					fillTreeRoot(ss, ref posnode);
				}
			}

			EndUpdate();
		}

		/// <summary>
		/// Fill the top level of the tree.
		/// </summary>
		/// <param name="sch"></param>
		/// <param name="opt">Currently a redundant parameter, but since it holds the search type, it can remain in case search type needs to be known.</param>
		/// <param name="tmppos"></param>
		private void fillTreeRoot(Wnlib.Search sch, ref TreeNode posnode)
		{
			// do the treeview
			//Wnlib.SynSet ss = default(Wnlib.SynSet);
			TreeNode parentnode = null;

			// iterate the returned senses
			foreach ( Wnlib.SynSet ss in sch.senses) {
				parentnode = newTreeNode(ss);

				// if a part of speech node has been created
				// then it becomes the top level
				if ((posnode != null)) {
					posnode.Nodes.Add(parentnode);
				} else {
					TreeView1.Nodes.Add(parentnode);
				}

				// do child senses
				if ((ss.senses != null)) {
					fillTreeChild(ss.senses, parentnode);
				}

				// fill in sense frames
				//Wnlib.SynSetFrame fr = default(Wnlib.SynSetFrame);
				TreeNode frnode = null;
				if (ss.frames.Count > 0) {
					foreach ( Wnlib.SynSetFrame fr in ss.frames) {
						frnode = new TreeNode(fr.fr.str);
						frnode.ImageIndex = 38;
						parentnode.Nodes.Add(frnode);
					}
				}
			}
		}

		/// <summary>
		/// Fill all the child nodes.  This is done to allow recursive child calls.
		/// </summary>
		/// <param name="ssarray">Search Sense Array</param>
		/// <param name="parentnode">Parent which the child is going to attach</param>
		private void fillTreeChild(Wnlib.SynSetList ssarray, TreeNode parentnode)
		{
			TreeNode childnode = null;
			//Wnlib.SynSet ss = default(Wnlib.SynSet);

			foreach ( Wnlib.SynSet ss in ssarray) {
				// define a new child node
				childnode = newTreeNode(ss);

				parentnode.Nodes.Add(childnode);

				// increase the depth of the tree via recursion
				if ((ss.senses != null)) {
					fillTreeChild(ss.senses, childnode);
				}
			}
		}

		/// <summary>
		/// Standardises the addition of a new TreeNode to the TreeView.  Lexemes are iterated and comma-separated
		/// for display purposes.  An icon is attached to the node according to the type of the sense.
		/// </summary>
		/// <param name="ss"></param>
		/// <returns></returns>
		private TreeNode newTreeNode(Wnlib.SynSet ss)
		{
			//Wnlib.Lexeme word = default(Wnlib.Lexeme);
			string words = null;
			TreeNode childnode = null;

			words = "";

			// Build the words for display in the node.
			// Words are the lexemes in a search result.
			foreach ( Wnlib.Lexeme word in ss.words) {
				if (!string.IsNullOrEmpty(words)) {
					words += ", ";
				}

				words += Strings.Replace(word.word, "_", " ");

				// append the sense number when the sense is not 1
				if (word.wnsns != 1) {
					words += "(" + word.wnsns + ")";
				}
			}

			// define a new child node
			childnode = new TreeNode();
			childnode.Text = words;

			// assign an icon according to the ident number of the ptr
			// (see static void classinit() in util.cs)
			if ((ss.thisptr != null)) {
				childnode.ImageIndex = ss.thisptr.ptp.ident;
				childnode.ToolTipText = ss.thisptr.ptp.label;
			}

			childnode.Tag = ss;

			return childnode;
		}

		/// <summary>
		/// Gets or sets the currently selected node in the tree.
		/// </summary>
		public System.Windows.Forms.TreeNode SelectedNode {
			get { return TreeView1.SelectedNode; }
			set { TreeView1.SelectedNode = value; }
		}

		/// <summary>
		/// Gets the TreeNode at position x, y.
		/// </summary>
		public System.Windows.Forms.TreeNode GetNodeAt(int x, int y) {
			return TreeView1.GetNodeAt(x, y);
		}

		/// <summary>
		/// Passes along the AfterSelect event from the TreeView.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		//Handles TreeView1.AfterSelect
		public void TreeView1AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (AfterSelect != null) {
				AfterSelect(sender, e);
			}
		}

		/// <summary>
		/// If the action was a right-click find the node that was at the right-click position and pass it along
		/// with the custom TreeRightClick event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		//Handles TreeView1.MouseDown
		public void TreeView1MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right) {
				// get the treenode at the mouse location
				TreeNode t = TreeView1.GetNodeAt(e.X, e.Y);

				// only proceed if there was a valid clicked node
				if (t == null) {
					return;
				}

				TreeView1.SelectedNode = t;

				if (TreeRightClick != null) {
					TreeRightClick(sender, e, t);
				}
			}
		}

		/// <summary>
		/// Clears the TreeView nodes.
		/// </summary>
		public void Clear()
		{
			TreeView1.Nodes.Clear();
		}

		/// <summary>
		/// Passes through the BeginUpdate method to the TreeView.
		/// </summary>
		public void BeginUpdate()
		{
			TreeView1.BeginUpdate();
		}

		/// <summary>
		/// Passes through the EndUpdate method to the TreeView
		/// </summary>
		public void EndUpdate()
		{
			TreeView1.EndUpdate();
		}
	}
}
