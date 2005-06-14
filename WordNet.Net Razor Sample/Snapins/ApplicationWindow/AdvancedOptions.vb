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

Imports Wnlib
Imports System.Windows.Forms
Imports System.Reflection

Public Class AdvancedOptions
    Inherits System.Windows.Forms.Form
    WithEvents Friend groupBox1 As System.Windows.Forms.GroupBox
    WithEvents Friend groupBox3 As System.Windows.Forms.GroupBox
    WithEvents Friend button1 As System.Windows.Forms.Button
    WithEvents Friend groupBox2 As System.Windows.Forms.GroupBox
    WithEvents Friend radioAoff As System.Windows.Forms.RadioButton
    WithEvents Friend radioSBoth As System.Windows.Forms.RadioButton
    WithEvents Friend radioOoff As System.Windows.Forms.RadioButton
    WithEvents Friend radioOon As System.Windows.Forms.RadioButton
    WithEvents Friend radioOBoth As System.Windows.Forms.RadioButton
    WithEvents Friend radioSon As System.Windows.Forms.RadioButton
    WithEvents Friend radioSoff As System.Windows.Forms.RadioButton
    WithEvents Friend radioAon As System.Windows.Forms.RadioButton
    WithEvents Friend radioABoth As System.Windows.Forms.RadioButton
    
#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        RBInit("-a", radioAoff, radioAon, radioABoth)
        RBInit("-o", radioOoff, radioOon, radioOBoth)
        RBInit("-s", radioSoff, radioSon, radioSBoth)
    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Me.radioABoth = New System.Windows.Forms.RadioButton
			Me.radioAon = New System.Windows.Forms.RadioButton
			Me.radioSoff = New System.Windows.Forms.RadioButton
			Me.radioSon = New System.Windows.Forms.RadioButton
			Me.radioOBoth = New System.Windows.Forms.RadioButton
			Me.radioOon = New System.Windows.Forms.RadioButton
			Me.radioOoff = New System.Windows.Forms.RadioButton
			Me.radioSBoth = New System.Windows.Forms.RadioButton
			Me.radioAoff = New System.Windows.Forms.RadioButton
			Me.groupBox2 = New System.Windows.Forms.GroupBox
			Me.button1 = New System.Windows.Forms.Button
			Me.groupBox3 = New System.Windows.Forms.GroupBox
			Me.groupBox1 = New System.Windows.Forms.GroupBox
			Me.groupBox2.SuspendLayout
			Me.groupBox3.SuspendLayout
			Me.groupBox1.SuspendLayout
			Me.SuspendLayout
			'
			'radioABoth
			'
			Me.radioABoth.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.radioABoth.Location = New System.Drawing.Point(16, 48)
			Me.radioABoth.Name = "radioABoth"
			Me.radioABoth.Size = New System.Drawing.Size(200, 16)
			Me.radioABoth.TabIndex = 6
			Me.radioABoth.Tag = "ON"
			Me.radioABoth.Text = "Show with searches and overview"
			'
			'radioAon
			'
			Me.radioAon.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.radioAon.Location = New System.Drawing.Point(16, 32)
			Me.radioAon.Name = "radioAon"
			Me.radioAon.Size = New System.Drawing.Size(128, 16)
			Me.radioAon.TabIndex = 5
			Me.radioAon.Tag = "on"
			Me.radioAon.Text = "Show with searches"
			'
			'radioSoff
			'
			Me.radioSoff.Checked = true
			Me.radioSoff.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.radioSoff.Location = New System.Drawing.Point(16, 16)
			Me.radioSoff.Name = "radioSoff"
			Me.radioSoff.Size = New System.Drawing.Size(112, 16)
			Me.radioSoff.TabIndex = 4
			Me.radioSoff.TabStop = true
			Me.radioSoff.Tag = "off"
			Me.radioSoff.Text = "Don't show"
			'
			'radioSon
			'
			Me.radioSon.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.radioSon.Location = New System.Drawing.Point(16, 32)
			Me.radioSon.Name = "radioSon"
			Me.radioSon.Size = New System.Drawing.Size(128, 16)
			Me.radioSon.TabIndex = 5
			Me.radioSon.Tag = "on"
			Me.radioSon.Text = "Show with searches"
			'
			'radioOBoth
			'
			Me.radioOBoth.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.radioOBoth.Location = New System.Drawing.Point(16, 48)
			Me.radioOBoth.Name = "radioOBoth"
			Me.radioOBoth.Size = New System.Drawing.Size(200, 16)
			Me.radioOBoth.TabIndex = 6
			Me.radioOBoth.Tag = "ON"
			Me.radioOBoth.Text = "Show with searches and overview"
			'
			'radioOon
			'
			Me.radioOon.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.radioOon.Location = New System.Drawing.Point(16, 32)
			Me.radioOon.Name = "radioOon"
			Me.radioOon.Size = New System.Drawing.Size(128, 16)
			Me.radioOon.TabIndex = 5
			Me.radioOon.Tag = "on"
			Me.radioOon.Text = "Show with searches"
			'
			'radioOoff
			'
			Me.radioOoff.Checked = true
			Me.radioOoff.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.radioOoff.Location = New System.Drawing.Point(16, 16)
			Me.radioOoff.Name = "radioOoff"
			Me.radioOoff.Size = New System.Drawing.Size(112, 16)
			Me.radioOoff.TabIndex = 4
			Me.radioOoff.TabStop = true
			Me.radioOoff.Tag = "off"
			Me.radioOoff.Text = "Don't show"
			'
			'radioSBoth
			'
			Me.radioSBoth.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.radioSBoth.Location = New System.Drawing.Point(16, 48)
			Me.radioSBoth.Name = "radioSBoth"
			Me.radioSBoth.Size = New System.Drawing.Size(200, 16)
			Me.radioSBoth.TabIndex = 6
			Me.radioSBoth.Tag = "ON"
			Me.radioSBoth.Text = "Show with searches and overview"
			'
			'radioAoff
			'
			Me.radioAoff.Checked = true
			Me.radioAoff.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.radioAoff.Location = New System.Drawing.Point(16, 16)
			Me.radioAoff.Name = "radioAoff"
			Me.radioAoff.Size = New System.Drawing.Size(112, 16)
			Me.radioAoff.TabIndex = 4
			Me.radioAoff.TabStop = true
			Me.radioAoff.Tag = "off"
			Me.radioAoff.Text = "Don't show"
			'
			'groupBox2
			'
			Me.groupBox2.Controls.Add(Me.radioOBoth)
			Me.groupBox2.Controls.Add(Me.radioOon)
			Me.groupBox2.Controls.Add(Me.radioOoff)
			Me.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupBox2.Location = New System.Drawing.Point(8, 80)
			Me.groupBox2.Name = "groupBox2"
			Me.groupBox2.Size = New System.Drawing.Size(232, 72)
			Me.groupBox2.TabIndex = 9
			Me.groupBox2.TabStop = false
			Me.groupBox2.Tag = "-o"
			Me.groupBox2.Text = "Synset location in database file"
			'
			'button1
			'
			Me.button1.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.button1.Location = New System.Drawing.Point(96, 240)
			Me.button1.Name = "button1"
			Me.button1.Size = New System.Drawing.Size(64, 24)
			Me.button1.TabIndex = 11
			Me.button1.Text = "OK"
			'
			'groupBox3
			'
			Me.groupBox3.Controls.Add(Me.radioSBoth)
			Me.groupBox3.Controls.Add(Me.radioSon)
			Me.groupBox3.Controls.Add(Me.radioSoff)
			Me.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupBox3.Location = New System.Drawing.Point(8, 152)
			Me.groupBox3.Name = "groupBox3"
			Me.groupBox3.Size = New System.Drawing.Size(232, 72)
			Me.groupBox3.TabIndex = 10
			Me.groupBox3.TabStop = false
			Me.groupBox3.Tag = "-s"
			Me.groupBox3.Text = "Sense number"
			'
			'groupBox1
			'
			Me.groupBox1.Controls.Add(Me.radioABoth)
			Me.groupBox1.Controls.Add(Me.radioAon)
			Me.groupBox1.Controls.Add(Me.radioAoff)
			Me.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupBox1.Location = New System.Drawing.Point(8, 8)
			Me.groupBox1.Name = "groupBox1"
			Me.groupBox1.Size = New System.Drawing.Size(232, 72)
			Me.groupBox1.TabIndex = 8
			Me.groupBox1.TabStop = false
			Me.groupBox1.Tag = "-a"
			Me.groupBox1.Text = "Lexical file information"
			'
			'AdvancedOptions
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(248, 270)
			Me.ControlBox = false
			Me.Controls.Add(Me.button1)
			Me.Controls.Add(Me.groupBox3)
			Me.Controls.Add(Me.groupBox2)
			Me.Controls.Add(Me.groupBox1)
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "AdvancedOptions"
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
			Me.Text = "Advanced Options"
			Me.TopMost = true
			Me.groupBox2.ResumeLayout(false)
			Me.groupBox3.ResumeLayout(false)
			Me.groupBox1.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub

#End Region

    Private Sub RBInit(ByVal t As String, ByVal a As RadioButton, ByVal b As RadioButton, ByVal c As RadioButton)
        Dim v As Boolean = WNOpt.opt(t).flag
        Dim V1 As Boolean = WNOpt.opt(t.ToUpper()).flag
        If (v) Then
            If (V1) Then
                c.Checked = True
            Else
                b.Checked = True
            End If
        ElseIf Not V1 Then
            a.Checked = True
        End If
    End Sub

    Private Sub radioAoff_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radioAoff.CheckedChanged
        Dim b As RadioButton = CType(sender, RadioButton)
        If b.Parent Is Nothing Then
        	Exit Sub
        End If
        
        Dim g As GroupBox = CType(b.Parent, GroupBox)
        Dim t As String = g.Tag
        Select Case b.Tag
            Case "off"
                WNOpt.opt(t).flag = False
                WNOpt.opt(t.ToUpper()).flag = False

            Case "on"
                WNOpt.opt(t).flag = True
                WNOpt.opt(t.ToUpper()).flag = False

            Case "ON"
                WNOpt.opt(t).flag = True
                WNOpt.opt(t.ToUpper()).flag = True

        End Select
    End Sub

    Private Sub button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button1.Click
        Me.Hide()
    End Sub

    Private Sub AdvancedOptions_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub AdvancedOptions_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Validated
        Application.EnableVisualStyles()
    End Sub
End Class
