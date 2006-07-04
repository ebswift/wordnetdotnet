'
' Created by SharpDevelop.
' User: Troy
' Date: 4/07/2006
' Time: 7:21 AM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Partial Class WordNetColourFormat
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
        Me.Canvas = New System.Windows.Forms.WebBrowser
        Me.SuspendLayout()
        '
        'Canvas
        '
        Me.Canvas.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Canvas.Location = New System.Drawing.Point(0, 0)
        Me.Canvas.MinimumSize = New System.Drawing.Size(20, 20)
        Me.Canvas.Name = "Canvas"
        Me.Canvas.Size = New System.Drawing.Size(405, 347)
        Me.Canvas.TabIndex = 0
        '
        'WordNetColourFormat
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Canvas)
        Me.Name = "WordNetColourFormat"
        Me.Size = New System.Drawing.Size(405, 347)
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents Canvas As System.Windows.Forms.WebBrowser
End Class
