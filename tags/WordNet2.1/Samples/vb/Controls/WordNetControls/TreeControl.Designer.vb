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
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Partial Class TreeControl
	Inherits System.Windows.Forms.UserControl
	
	''' <summary>
	''' Designer variable used to keep track of non-visual components.
	''' </summary>
	Private components As System.ComponentModel.IContainer
	
	''' <summary>
	''' Disposes resources used by the control.
	''' </summary>
	''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		If disposing Then
			If components IsNot Nothing Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(disposing)
	End Sub
	
	''' <summary>
	''' This method is required for Windows Forms designer support.
	''' Do not change the method contents inside the source code editor. The Forms designer might
	''' not be able to load this method if it was changed manually.
	''' </summary>
	Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TreeControl))
        Me.TreeView1 = New System.Windows.Forms.TreeView
        Me.wnIcons = New System.Windows.Forms.ImageList(Me.components)
        Me.SuspendLayout()
        '
        'TreeView1
        '
        Me.TreeView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TreeView1.ImageIndex = 0
        Me.TreeView1.ImageList = Me.wnIcons
        Me.TreeView1.Location = New System.Drawing.Point(0, 0)
        Me.TreeView1.Name = "TreeView1"
        Me.TreeView1.SelectedImageIndex = 0
        Me.TreeView1.ShowNodeToolTips = True
        Me.TreeView1.Size = New System.Drawing.Size(350, 381)
        Me.TreeView1.TabIndex = 18
        '
        'wnIcons
        '
        Me.wnIcons.ImageStream = CType(resources.GetObject("wnIcons.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.wnIcons.TransparentColor = System.Drawing.Color.Transparent
        Me.wnIcons.Images.SetKeyName(0, "")
        Me.wnIcons.Images.SetKeyName(1, "")
        Me.wnIcons.Images.SetKeyName(2, "MISC18.ICO")
        Me.wnIcons.Images.SetKeyName(3, "BINOCULR.ICO")
        Me.wnIcons.Images.SetKeyName(4, "MISC08.ICO")
        Me.wnIcons.Images.SetKeyName(5, "MISC28.ICO")
        Me.wnIcons.Images.SetKeyName(6, "")
        Me.wnIcons.Images.SetKeyName(7, "WATER.ICO")
        Me.wnIcons.Images.SetKeyName(8, "EAR.ICO")
        Me.wnIcons.Images.SetKeyName(9, "MISC33.ICO")
        Me.wnIcons.Images.SetKeyName(10, "MISC44.ICO")
        Me.wnIcons.Images.SetKeyName(11, "SECUR06.ICO")
        Me.wnIcons.Images.SetKeyName(12, "MOUSE01.ICO")
        Me.wnIcons.Images.SetKeyName(13, "PC02.ICO")
        Me.wnIcons.Images.SetKeyName(14, "KEY04.ICO")
        Me.wnIcons.Images.SetKeyName(15, "MOUSE04.ICO")
        Me.wnIcons.Images.SetKeyName(16, "DRAG3PG.ICO")
        Me.wnIcons.Images.SetKeyName(17, "NET07.ICO")
        Me.wnIcons.Images.SetKeyName(18, "RULERS.ICO")
        Me.wnIcons.Images.SetKeyName(19, "POINT06.ICO")
        Me.wnIcons.Images.SetKeyName(20, "POINT10.ICO")
        Me.wnIcons.Images.SetKeyName(21, "HOUSE.ICO")
        Me.wnIcons.Images.SetKeyName(22, "MAIL08.ICO")
        Me.wnIcons.Images.SetKeyName(23, "MISC22.ICO")
        Me.wnIcons.Images.SetKeyName(24, "GRAPH01.ICO")
        Me.wnIcons.Images.SetKeyName(25, "CRDFLE04.ICO")
        Me.wnIcons.Images.SetKeyName(26, "BULLSEYE.ICO")
        Me.wnIcons.Images.SetKeyName(27, "MISC29.ICO")
        Me.wnIcons.Images.SetKeyName(28, "MOUSE01.ICO")
        Me.wnIcons.Images.SetKeyName(29, "PC02.ICO")
        Me.wnIcons.Images.SetKeyName(30, "CRDFLE12.ICO")
        Me.wnIcons.Images.SetKeyName(31, "GRAPH11.ICO")
        Me.wnIcons.Images.SetKeyName(32, "CLIP07.ICO")
        Me.wnIcons.Images.SetKeyName(33, "PLANE.ICO")
        Me.wnIcons.Images.SetKeyName(34, "CLOUD.ICO")
        Me.wnIcons.Images.SetKeyName(35, "FOLDER04.ICO")
        Me.wnIcons.Images.SetKeyName(36, "SECUR05.ICO")
        Me.wnIcons.Images.SetKeyName(37, "BOOK02.ICO")
        Me.wnIcons.Images.SetKeyName(38, "")
        '
        'TreeControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TreeView1)
        Me.Name = "TreeControl"
        Me.Size = New System.Drawing.Size(350, 381)
        Me.ResumeLayout(False)

    End Sub
	Friend wnIcons As System.Windows.Forms.ImageList
	Friend TreeView1 As System.Windows.Forms.TreeView
	
End Class
