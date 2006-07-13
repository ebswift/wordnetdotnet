'
' Created by SharpDevelop.
' User: Troy
' Date: 13/07/2006
' Time: 9:02 PM
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
		Me.label1 = New System.Windows.Forms.Label
		Me.txtWord = New System.Windows.Forms.TextBox
		Me.cboPOS = New System.Windows.Forms.ComboBox
		Me.btnGo = New System.Windows.Forms.Button
		Me.txtResult = New System.Windows.Forms.TextBox
		Me.label2 = New System.Windows.Forms.Label
		Me.SuspendLayout
		'
		'label1
		'
		Me.label1.Location = New System.Drawing.Point(12, 9)
		Me.label1.Name = "label1"
		Me.label1.Size = New System.Drawing.Size(83, 23)
		Me.label1.TabIndex = 0
		Me.label1.Text = "Search Word:"
		'
		'txtWord
		'
		Me.txtWord.Location = New System.Drawing.Point(101, 6)
		Me.txtWord.Name = "txtWord"
		Me.txtWord.Size = New System.Drawing.Size(126, 21)
		Me.txtWord.TabIndex = 1
		'
		'cboPOS
		'
		Me.cboPOS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cboPOS.FormattingEnabled = true
		Me.cboPOS.Items.AddRange(New Object() {"noun", "verb", "adj", "adv"})
		Me.cboPOS.Location = New System.Drawing.Point(233, 6)
		Me.cboPOS.Name = "cboPOS"
		Me.cboPOS.Size = New System.Drawing.Size(75, 21)
		Me.cboPOS.TabIndex = 3
		'
		'btnGo
		'
		Me.btnGo.Location = New System.Drawing.Point(314, 6)
		Me.btnGo.Name = "btnGo"
		Me.btnGo.Size = New System.Drawing.Size(45, 23)
		Me.btnGo.TabIndex = 4
		Me.btnGo.Text = "Go"
		Me.btnGo.UseVisualStyleBackColor = true
		AddHandler Me.btnGo.Click, AddressOf Me.BtnGoClick
		'
		'txtResult
		'
		Me.txtResult.Location = New System.Drawing.Point(12, 52)
		Me.txtResult.Multiline = true
		Me.txtResult.Name = "txtResult"
		Me.txtResult.ReadOnly = true
		Me.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
		Me.txtResult.Size = New System.Drawing.Size(346, 99)
		Me.txtResult.TabIndex = 5
		'
		'label2
		'
		Me.label2.Location = New System.Drawing.Point(12, 32)
		Me.label2.Name = "label2"
		Me.label2.Size = New System.Drawing.Size(100, 17)
		Me.label2.TabIndex = 6
		Me.label2.Text = "Coordinate Terms:"
		'
		'MainForm
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(370, 163)
		Me.Controls.Add(Me.label2)
		Me.Controls.Add(Me.txtResult)
		Me.Controls.Add(Me.btnGo)
		Me.Controls.Add(Me.cboPOS)
		Me.Controls.Add(Me.txtWord)
		Me.Controls.Add(Me.label1)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		Me.MaximizeBox = false
		Me.MinimizeBox = false
		Me.Name = "MainForm"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "Get Coordinate Terms for POS"
		Me.ResumeLayout(false)
		Me.PerformLayout
	End Sub
	Private txtResult As System.Windows.Forms.TextBox
	Private label2 As System.Windows.Forms.Label
	Private btnGo As System.Windows.Forms.Button
	Private cboPOS As System.Windows.Forms.ComboBox
	Private txtWord As System.Windows.Forms.TextBox
	Private label1 As System.Windows.Forms.Label
End Class
