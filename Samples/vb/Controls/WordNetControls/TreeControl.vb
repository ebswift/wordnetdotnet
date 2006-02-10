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
' 
' TreeControl
' Standalone TreeView control for WordNet.
' See the TreeView sample under the samples folder for an example of use
Public Partial Class TreeControl
	Public Event AfterSelect(sender As Object, e As System.Windows.Forms.TreeViewEventArgs)
	Public Shadows Event MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs)
	
	Public Sub New()
		' The Me.InitializeComponent call is required for Windows Forms designer support.
		Me.InitializeComponent()
		
		'
		' TODO : Add constructor code after InitializeComponents
		'
	End Sub
	
        Public Sub fillTreeRoot(ByVal sch As Wnlib.Search, ByVal opt As Wnlib.Opt, Optional ByVal tmppos As String = "")
            ' do the treeview
            Dim ss As Wnlib.SynSet
            Dim parentnode As TreeNode
            Dim posnode As TreeNode = Nothing ' part of speech node used for overview search

            ' a part of speech has been given as a parameter
            ' so create a new top level node
            If tmppos <> "" Then
                posnode = New TreeNode(tmppos)
                TreeView1.Nodes.Add(posnode)
            End If

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
                        frnode.ImageIndex = 23 'TODO: change this number
                        parentnode.Nodes.Add(frnode)
                    Next
                End If

                'skip:
            Next ss
        End Sub

        ' separate sub to fill all the children -
        ' this is done to allow recursive child calls
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

        Private Function newTreeNode(ByVal ss As Wnlib.SynSet) As TreeNode
            Dim word As Wnlib.Lexeme
            Dim words As String
            Dim childnode As TreeNode

            words = ""

            ' these are the returned lexemes
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
            'childnode.Tag = ss.defn

            ' assign an icon according to the ident number of the ptr
            ' (see static void classinit() in util.cs)
            If Not ss.thisptr Is Nothing Then
                childnode.ImageIndex = ss.thisptr.ptp.ident
            End If

            childnode.Tag = ss

            Return childnode
        End Function
	
	Public Property SelectedNode As System.Windows.Forms.TreeNode
		Get
			Return TreeView1.SelectedNode
		End Get
		Set
			TreeView1.SelectedNode = Value
		End Set
	End Property

	Public ReadOnly Property GetNodeAt(ByVal x As Integer, ByVal y As Integer) As System.Windows.Forms.TreeNode
		Get
			Return TreeView1.GetNodeAt(x, y)
		End Get
	End Property

	Sub TreeView1AfterSelect(sender As Object, e As System.Windows.Forms.TreeViewEventArgs)
		RaiseEvent AfterSelect(sender, e)
	End Sub
	
	Sub TreeView1MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs)
		RaiseEvent MouseDown(sender, e)
	End Sub
	
	Public Sub Clear()
		TreeView1.Nodes.Clear()
	End Sub
	
	Public Sub BeginUpdate()
		TreeView1.BeginUpdate()
	End Sub

	Public Sub EndUpdate()
		TreeView1.EndUpdate()
	End Sub
End Class
