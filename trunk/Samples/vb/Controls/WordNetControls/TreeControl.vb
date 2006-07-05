'/*
' * This file is a part of the WordNet.Net open source project.
' * 
' * Copyright (C) 2005 Malcolm Crowe, Troy Simpson 
' * 
' * Project Home: http://www.ebswift.com
' *
' * This library is free software; you can redistribute it and/or
' * modify it under the terms of the GNU Lesser General Public
' * License as published by the Free Software Foundation; either
' * version 2.1 of the License, or (at your option) any later version.
' *
' * This library is distributed in the hope that it will be useful,
' * but WITHOUT ANY WARRANTY; without even the implied warranty of
' * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
' * Lesser General Public License for more details.
' *
' * You should have received a copy of the GNU Lesser General Public
' * License along with this library; if not, write to the Free Software
' * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
' * 
' * */

'
' Created by SharpDevelop.
' User: simpsont
' Date: 10/02/2006
' Time: 10:25 AM

'TODO: icons need to be created/obtained and appropriately assigned to the various results types.

''' <summary>
''' Standalone TreeView control for WordNet.  
''' See the TreeView sample under the samples folder for an example of use.
''' </summary>
Public Partial Class TreeControl
	Public Event AfterSelect(sender As Object, e As System.Windows.Forms.TreeViewEventArgs)
	Public Shadows Event MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs)
	Public Event TreeRightClick(sender As Object, e As System.Windows.Forms.MouseEventArgs, tn As System.Windows.Forms.TreeNode)
	
	Public Sub New()
		' The Me.InitializeComponent call is required for Windows Forms designer support.
		Me.InitializeComponent()
		
		'
		' TODO : Add constructor code after InitializeComponents
        '

        AddHandler TreeView1.AfterSelect, AddressOf TreeView1AfterSelect
        AddHandler TreeView1.MouseDown, AddressOf TreeView1MouseDown
	End Sub

	''' <summary>
	''' Begins the iterative process of filling the tree.
	''' </summary>
	''' <param name="list">An ArrayList of search objects to iterate</param>
	''' <param name="usepos">A Boolean to specify whether a part of speech should be used as ArrayList top node.  Generally this is not used when searching POS relations because the POS is already specified</param>
    Public Sub fillTree(ByVal list As ArrayList, ByVal usepos As Boolean)
        Dim ss As Wnlib.Search
        Dim posnode As TreeNode = Nothing ' part of speech node used for overview search

        BeginUpdate()
        Clear()

        For Each ss In list
            If ss.senses.Count > 0 Then
				If usepos Then
    	        	posnode = New TreeNode(ss.pos.name)
	            	TreeView1.Nodes.Add(posnode)
				End If
				
            	fillTreeRoot(ss, posnode)
            End If
        Next

        EndUpdate()
	End Sub
	
	''' <summary>
	''' Fill the top level of the tree.
	''' </summary>
	''' <param name="sch"></param>
	''' <param name="opt">Currently a redundant parameter, but since it holds the search type, it can remain in case search type needs to be known.</param>
	''' <param name="tmppos"></param>
    Private Sub fillTreeRoot(ByVal sch As Wnlib.Search, ByRef posnode As TreeNode)
        ' do the treeview
        Dim ss As Wnlib.SynSet
        Dim parentnode As TreeNode

        ' iterate the returned senses
        For Each ss In sch.senses
            parentnode = newTreeNode(ss)

            ' if a part of speech node has been created
            ' then it becomes the top level
            If Not posnode Is Nothing Then
                posnode.Nodes.Add(parentnode)
            Else
                TreeView1.Nodes.Add(parentnode)
            End If

            ' do child senses
            If Not ss.senses Is Nothing Then
                fillTreeChild(ss.senses, parentnode)
            End If

            ' fill in sense frames
            Dim fr As Wnlib.SynSetFrame
            Dim frnode As TreeNode
            If ss.frames.Count > 0 Then
                For Each fr In ss.frames
                    frnode = New TreeNode(fr.fr.str)
                    frnode.ImageIndex = 38
                    parentnode.Nodes.Add(frnode)
                Next
            End If
        Next ss
    End Sub

	''' <summary>
	''' Fill all the child nodes.  This is done to allow recursive child calls.
	''' </summary>
	''' <param name="ssarray">Search Sense Array</param>
	''' <param name="parentnode">Parent which the child is going to attach</param>
    Private Sub fillTreeChild(ByVal ssarray As Wnlib.SynSetList, ByVal parentnode As TreeNode)
        Dim childnode As TreeNode
        Dim ss As Wnlib.SynSet

        For Each ss In ssarray
            ' define a new child node
            childnode = newTreeNode(ss)

            parentnode.Nodes.Add(childnode)

            ' increase the depth of the tree via recursion
            If Not ss.senses Is Nothing Then
                fillTreeChild(ss.senses, childnode)
            End If
        Next ss
    End Sub

	''' <summary>
	''' Standardises the addition of a new TreeNode to the TreeView.  Lexemes are iterated and comma-separated
	''' for display purposes.  An icon is attached to the node according to the type of the sense.
	''' </summary>
	''' <param name="ss"></param>
	''' <returns></returns>
    Private Function newTreeNode(ByVal ss As Wnlib.SynSet) As TreeNode
        Dim word As Wnlib.Lexeme
        Dim words As String
        Dim childnode As TreeNode

        words = ""

		' Build the words for display in the node.
		' Words are the lexemes in a search result.
        For Each word In ss.words
            If words <> "" Then
                words += ", "
            End If

            words += Replace(word.word, "_", " ")

            ' append the sense number when the sense is not 1
            If word.wnsns <> 1 Then
                words += "(" & word.wnsns & ")"
            End If
        Next word

        ' define a new child node
        childnode = New TreeNode
        childnode.Text = words

        ' assign an icon according to the ident number of the ptr
        ' (see static void classinit() in util.cs)
        If Not ss.thisptr Is Nothing Then
            childnode.ImageIndex = ss.thisptr.ptp.ident
            childnode.ToolTipText = ss.thisptr.ptp.label
        End If

        childnode.Tag = ss

        Return childnode
    End Function
	
	''' <summary>
	''' Gets or sets the currently selected node in the tree.
	''' </summary>
	Public Property SelectedNode As System.Windows.Forms.TreeNode
		Get
			Return TreeView1.SelectedNode
		End Get
		Set
			TreeView1.SelectedNode = Value
		End Set
	End Property

	''' <summary>
	''' Gets the TreeNode at position x, y.
	''' </summary>
	Public ReadOnly Property GetNodeAt(ByVal x As Integer, ByVal y As Integer) As System.Windows.Forms.TreeNode
		Get
			Return TreeView1.GetNodeAt(x, y)
		End Get
	End Property

	''' <summary>
	''' Passes along the AfterSelect event from the TreeView.
	''' </summary>
	''' <param name="sender"></param>
	''' <param name="e"></param>
    Sub TreeView1AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) 'Handles TreeView1.AfterSelect
        RaiseEvent AfterSelect(sender, e)
    End Sub
	
	''' <summary>
	''' If the action was a right-click find the node that was at the right-click position and pass it along
	''' with the custom TreeRightClick event.
	''' </summary>
	''' <param name="sender"></param>
	''' <param name="e"></param>
    Sub TreeView1MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) 'Handles TreeView1.MouseDown
        If e.Button = System.Windows.Forms.MouseButtons.Right Then
            ' get the treenode at the mouse location
            Dim t As TreeNode = TreeView1.GetNodeAt(e.X, e.Y)

            ' only proceed if there was a valid clicked node
            If t Is Nothing Then
                Exit Sub
            End If

            TreeView1.SelectedNode = t

            RaiseEvent TreeRightClick(sender, e, t)
        End If
    End Sub
	
	''' <summary>
	''' Clears the TreeView nodes.
	''' </summary>
	Public Sub Clear()
		TreeView1.Nodes.Clear()
	End Sub
	
	''' <summary>
	''' Passes through the BeginUpdate method to the TreeView.
	''' </summary>
	Public Sub BeginUpdate()
		TreeView1.BeginUpdate()
	End Sub

	''' <summary>
	''' Passes through the EndUpdate method to the TreeView
	''' </summary>
	Public Sub EndUpdate()
		TreeView1.EndUpdate()
	End Sub
End Class
