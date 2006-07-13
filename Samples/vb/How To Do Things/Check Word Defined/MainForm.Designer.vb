'
' Created by SharpDevelop.
' User: Troy
' Date: 13/07/2006
' Time: 7:39 AM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Partial Class MainForm
	Inherits System.Windows.Forms.Form
	
	''' <summary>
	''' Designer variable used to keep track of non-visual components.
	''' </summary>
	Private components As System.ComponentModel.IContainer
	
	''' <summary>
	''' Disposes resources used by the form.
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
		Me.txtWord = New System.Windows.Forms.TextBox
		Me.label1 = New System.Windows.Forms.Label
		Me.lblResult = New System.Windows.Forms.Label
		Me.btnGo = New System.Windows.Forms.Button
		Me.label2 = New System.Windows.Forms.Label
		Me.cboPOS = New System.Windows.Forms.ComboBox
		Me.label3 = New System.Windows.Forms.Label
		Me.SuspendLayout
		'
		'txtWord
		'
		Me.txtWord.Location = New System.Drawing.Point(118, 6)
		Me.txtWord.Name = "txtWord"
		Me.txtWord.Size = New System.Drawing.Size(139, 21)
		Me.txtWord.TabIndex = 0
		'
		'label1
		'
		Me.label1.Location = New System.Drawing.Point(12, 9)
		Me.label1.Name = "label1"
		Me.label1.Size = New System.Drawing.Size(100, 23)
		Me.label1.TabIndex = 1
		Me.label1.Text = "Word to Check:"
		'
		'lblResult
		'
		Me.lblResult.Location = New System.Drawing.Point(118, 32)
		Me.lblResult.Name = "lblResult"
		Me.lblResult.Size = New System.Drawing.Size(201, 23)
		Me.lblResult.TabIndex = 2
		'
		'btnGo
		'
		Me.btnGo.Location = New System.Drawing.Point(335, 6)
		Me.btnGo.Name = "btnGo"
		Me.btnGo.Size = New System.Drawing.Size(43, 23)
		Me.btnGo.TabIndex = 3
		Me.btnGo.Text = "Go"
		Me.btnGo.UseVisualStyleBackColor = true
		AddHandler Me.btnGo.Click, AddressOf Me.BtnGoClick
		'
		'label2
		'
		Me.label2.Location = New System.Drawing.Point(12, 35)
		Me.label2.Name = "label2"
		Me.label2.Size = New System.Drawing.Size(100, 23)
		Me.label2.TabIndex = 4
		Me.label2.Text = "Is Defined:"
		'
		'cboPOS
		'
		Me.cboPOS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cboPOS.FormattingEnabled = true
		Me.cboPOS.Items.AddRange(New Object() {"noun", "verb", "adj", "adv"})
		Me.cboPOS.Location = New System.Drawing.Point(264, 6)
		Me.cboPOS.Name = "cboPOS"
		Me.cboPOS.Size = New System.Drawing.Size(65, 21)
		Me.cboPOS.TabIndex = 5
		'
		'label3
		'
		Me.label3.Location = New System.Drawing.Point(12, 58)
		Me.label3.Name = "label3"
		Me.label3.Size = New System.Drawing.Size(366, 37)
		Me.label3.TabIndex = 6
		Me.label3.Text = "This example displays a true/false for if a given word exists in the WordNet data"& _ 
		"base for the given part of speech."
		'
		'MainForm
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(390, 99)
		Me.Controls.Add(Me.label3)
		Me.Controls.Add(Me.cboPOS)
		Me.Controls.Add(Me.label2)
		Me.Controls.Add(Me.btnGo)
		Me.Controls.Add(Me.lblResult)
		Me.Controls.Add(Me.label1)
		Me.Controls.Add(Me.txtWord)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		Me.MaximizeBox = false
		Me.MinimizeBox = false
		Me.Name = "MainForm"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "Check Word Defined"
		Me.ResumeLayout(false)
		Me.PerformLayout
	End Sub
	Private label3 As System.Windows.Forms.Label
	Private cboPOS As System.Windows.Forms.ComboBox
	Private label2 As System.Windows.Forms.Label
	Private btnGo As System.Windows.Forms.Button
	Private lblResult As System.Windows.Forms.Label
	Private txtWord As System.Windows.Forms.TextBox
	Private label1 As System.Windows.Forms.Label
End Class
