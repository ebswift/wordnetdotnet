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

Imports System
Imports System.componentmodel
Imports System.Reflection
Imports System.Windows.Forms
Imports System.io
Imports WordNetClasses.WN
Imports Microsoft.VisualBasic
Imports System.Collections
Imports System.Drawing

Namespace wnb

    Public Class StartForm
        Inherits System.Windows.Forms.Form

        Friend mnuFile As System.Windows.Forms.MenuItem
        Friend btnAdv As System.Windows.Forms.Button
        Friend btnNoun As System.Windows.Forms.Button
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents mnuHelp As System.Windows.Forms.MenuItem
        Friend btnAdj As System.Windows.Forms.Button
        Friend lblSearchInfo As System.Windows.Forms.Label
        Friend WithEvents MainMenu1 As System.Windows.Forms.MainMenu
        Friend WithEvents mnuShowGloss As System.Windows.Forms.MenuItem
        Friend WithEvents mnuExit As System.Windows.Forms.MenuItem
        Friend WithEvents mnuLGPL As System.Windows.Forms.MenuItem
        Friend WithEvents mnuOptions As System.Windows.Forms.MenuItem
        Friend WithEvents btnSearch As System.Windows.Forms.Button
        Friend WithEvents mnuShowHelp As System.Windows.Forms.MenuItem
        Friend WithEvents mnuWordWrap As System.Windows.Forms.MenuItem
        Friend WithEvents mnuAdvancedOptions As System.Windows.Forms.MenuItem
        Friend WithEvents mnuSaveDisplay As System.Windows.Forms.MenuItem
        Friend txtSenses As System.Windows.Forms.TextBox
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents txtSearchWord As System.Windows.Forms.TextBox
        Friend WithEvents mnuWordNetLicense As System.Windows.Forms.MenuItem
        Friend btnOverview As System.Windows.Forms.Button
        Friend btnVerb As System.Windows.Forms.Button
        Private WithEvents mnuHistory As System.Windows.Forms.MenuItem
        Friend WithEvents mnuClearDisplay As System.Windows.Forms.MenuItem
        Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
        Friend WithEvents MenuItem17 As System.Windows.Forms.MenuItem

#Region "FormVariables"
        Private f3 As AdvancedOptions
        Private dictpath As String = "C:\Program Files\WordNet\2.1\dict\"
        Private wnc As WordNetClasses.WN = New WordNetClasses.WN(dictpath)
        Friend WithEvents wnColour As WordNetControls.WordNetColourFormat
        Private pbobject As Object = New Object

#End Region

#Region " Windows Form Designer generated code "

        Public Sub New()
            MyBase.New()

            'This call is required by the Windows Form Designer.
            InitializeComponent()

            'Add any initialization after the InitializeComponent() call

            AddHandler wnColour.CanvasNavigating, AddressOf CanvasNavigating
            wnColour.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Top

'            txtOutput.Anchor = Anchor.Top Or Anchor.Left Or Anchor.Bottom Or Anchor.Right
            f3 = New AdvancedOptions
            AddHandler mnuSaveDisplay.Click, AddressOf mnuSaveDisplay_Click
            AddHandler mnuClearDisplay.Click, AddressOf mnuClearDisplay_Click
            AddHandler mnuExit.Click, AddressOf mnuExit_Click
            AddHandler mnuWordWrap.Click, AddressOf mnuWordWrap_Click
            AddHandler mnuShowHelp.Click, AddressOf mnuShowHelp_Click
            AddHandler mnuShowGloss.Click, AddressOf mnuShowGloss_Click
            AddHandler mnuAdvancedOptions.Click, AddressOf mnuAdvancedOptions_Click
            AddHandler mnuWordNetLicense.Click, AddressOf mnuWordNetLicense_Click
            AddHandler mnuLGPL.Click, AddressOf mnuLGPL_Click
            AddHandler btnOverview.Click, AddressOf btnOverview_Click
            AddHandler btnNoun.Click, AddressOf btnWordType_Click
            AddHandler btnVerb.Click, AddressOf btnWordType_Click
            AddHandler btnAdj.Click, AddressOf btnWordType_Click
            AddHandler btnAdv.Click, AddressOf btnWordType_Click
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
        Friend WithEvents Panel1 As System.Windows.Forms.Panel
        Friend WithEvents StatusBar1 As System.Windows.Forms.StatusBar
        Friend WithEvents Panel3 As System.Windows.Forms.Panel
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        	Me.components = New System.ComponentModel.Container
        	Me.MenuItem17 = New System.Windows.Forms.MenuItem
        	Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog
        	Me.mnuClearDisplay = New System.Windows.Forms.MenuItem
        	Me.mnuHistory = New System.Windows.Forms.MenuItem
        	Me.btnVerb = New System.Windows.Forms.Button
        	Me.btnOverview = New System.Windows.Forms.Button
        	Me.mnuWordNetLicense = New System.Windows.Forms.MenuItem
        	Me.txtSearchWord = New System.Windows.Forms.TextBox
        	Me.Label1 = New System.Windows.Forms.Label
        	Me.txtSenses = New System.Windows.Forms.TextBox
        	Me.mnuSaveDisplay = New System.Windows.Forms.MenuItem
        	Me.mnuAdvancedOptions = New System.Windows.Forms.MenuItem
        	Me.mnuWordWrap = New System.Windows.Forms.MenuItem
        	Me.mnuShowHelp = New System.Windows.Forms.MenuItem
        	Me.btnSearch = New System.Windows.Forms.Button
        	Me.mnuOptions = New System.Windows.Forms.MenuItem
        	Me.mnuShowGloss = New System.Windows.Forms.MenuItem
        	Me.mnuLGPL = New System.Windows.Forms.MenuItem
        	Me.mnuExit = New System.Windows.Forms.MenuItem
        	Me.MainMenu1 = New System.Windows.Forms.MainMenu(Me.components)
        	Me.mnuFile = New System.Windows.Forms.MenuItem
        	Me.mnuHelp = New System.Windows.Forms.MenuItem
        	Me.lblSearchInfo = New System.Windows.Forms.Label
        	Me.btnAdj = New System.Windows.Forms.Button
        	Me.Label3 = New System.Windows.Forms.Label
        	Me.btnNoun = New System.Windows.Forms.Button
        	Me.btnAdv = New System.Windows.Forms.Button
        	Me.Panel1 = New System.Windows.Forms.Panel
        	Me.Panel3 = New System.Windows.Forms.Panel
        	Me.wnColour = New WordNetControls.WordNetColourFormat
        	Me.StatusBar1 = New System.Windows.Forms.StatusBar
        	Me.Panel1.SuspendLayout
        	Me.Panel3.SuspendLayout
        	Me.SuspendLayout
        	'
        	'MenuItem17
        	'
        	Me.MenuItem17.Index = 1
        	Me.MenuItem17.Text = "-"
        	'
        	'SaveFileDialog1
        	'
        	Me.SaveFileDialog1.Filter = "Text files (*.txt)|*.txt"
        	'
        	'mnuClearDisplay
        	'
        	Me.mnuClearDisplay.Index = 1
        	Me.mnuClearDisplay.Text = "Clear Current Display"
        	'
        	'mnuHistory
        	'
        	Me.mnuHistory.Index = -1
        	Me.mnuHistory.Text = ""
        	'
        	'btnVerb
        	'
        	Me.btnVerb.FlatStyle = System.Windows.Forms.FlatStyle.System
        	Me.btnVerb.Location = New System.Drawing.Point(350, 36)
        	Me.btnVerb.Name = "btnVerb"
        	Me.btnVerb.Size = New System.Drawing.Size(40, 18)
        	Me.btnVerb.TabIndex = 4
        	Me.btnVerb.Text = "Verb"
        	Me.btnVerb.Visible = false
        	'
        	'btnOverview
        	'
        	Me.btnOverview.FlatStyle = System.Windows.Forms.FlatStyle.System
        	Me.btnOverview.Location = New System.Drawing.Point(223, 36)
        	Me.btnOverview.Name = "btnOverview"
        	Me.btnOverview.Size = New System.Drawing.Size(75, 18)
        	Me.btnOverview.TabIndex = 15
        	Me.btnOverview.Text = "Overview"
        	Me.btnOverview.Visible = false
        	'
        	'mnuWordNetLicense
        	'
        	Me.mnuWordNetLicense.Index = 0
        	Me.mnuWordNetLicense.Text = "WordNet License (Princeton)"
        	'
        	'txtSearchWord
        	'
        	Me.txtSearchWord.AcceptsReturn = true
        	Me.txtSearchWord.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
        	        	        	Or System.Windows.Forms.AnchorStyles.Left)  _
        	        	        	Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        	Me.txtSearchWord.Location = New System.Drawing.Point(80, 6)
        	Me.txtSearchWord.Name = "txtSearchWord"
        	Me.txtSearchWord.Size = New System.Drawing.Size(216, 21)
        	Me.txtSearchWord.TabIndex = 1
        	'
        	'Label1
        	'
        	Me.Label1.Location = New System.Drawing.Point(0, 6)
        	Me.Label1.Name = "Label1"
        	Me.Label1.Size = New System.Drawing.Size(80, 19)
        	Me.Label1.TabIndex = 0
        	Me.Label1.Text = "Search Word:"
        	'
        	'txtSenses
        	'
        	Me.txtSenses.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        	Me.txtSenses.Location = New System.Drawing.Point(544, 6)
        	Me.txtSenses.Name = "txtSenses"
        	Me.txtSenses.Size = New System.Drawing.Size(40, 21)
        	Me.txtSenses.TabIndex = 8
        	Me.txtSenses.Text = "0"
        	'
        	'mnuSaveDisplay
        	'
        	Me.mnuSaveDisplay.Index = 0
        	Me.mnuSaveDisplay.Text = "Save Current Display"
        	'
        	'mnuAdvancedOptions
        	'
        	Me.mnuAdvancedOptions.Index = 3
        	Me.mnuAdvancedOptions.Text = "Advanced search options"
        	'
        	'mnuWordWrap
        	'
        	Me.mnuWordWrap.Checked = true
        	Me.mnuWordWrap.Index = 0
        	Me.mnuWordWrap.Text = "Word Wrap"
        	'
        	'mnuShowHelp
        	'
        	Me.mnuShowHelp.Index = 1
        	Me.mnuShowHelp.Text = "Show help with each search"
        	'
        	'btnSearch
        	'
        	Me.btnSearch.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        	Me.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.System
        	Me.btnSearch.Location = New System.Drawing.Point(304, 6)
        	Me.btnSearch.Name = "btnSearch"
        	Me.btnSearch.Size = New System.Drawing.Size(56, 19)
        	Me.btnSearch.TabIndex = 13
        	Me.btnSearch.Text = "Search"
        	'
        	'mnuOptions
        	'
        	Me.mnuOptions.Index = 1
        	Me.mnuOptions.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuWordWrap, Me.mnuShowHelp, Me.mnuShowGloss, Me.mnuAdvancedOptions})
        	Me.mnuOptions.Text = "Options"
        	'
        	'mnuShowGloss
        	'
        	Me.mnuShowGloss.Index = 2
        	Me.mnuShowGloss.Text = "Show descriptive gloss"
        	'
        	'mnuLGPL
        	'
        	Me.mnuLGPL.Index = 2
        	Me.mnuLGPL.Text = "License"
        	'
        	'mnuExit
        	'
        	Me.mnuExit.Index = 2
        	Me.mnuExit.Text = "Exit"
        	'
        	'MainMenu1
        	'
        	Me.MainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuFile, Me.mnuOptions, Me.mnuHelp})
        	'
        	'mnuFile
        	'
        	Me.mnuFile.Index = 0
        	Me.mnuFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuSaveDisplay, Me.mnuClearDisplay, Me.mnuExit})
        	Me.mnuFile.Text = "File"
        	'
        	'mnuHelp
        	'
        	Me.mnuHelp.Index = 2
        	Me.mnuHelp.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuWordNetLicense, Me.MenuItem17, Me.mnuLGPL})
        	Me.mnuHelp.Text = "Help"
        	'
        	'lblSearchInfo
        	'
        	Me.lblSearchInfo.Location = New System.Drawing.Point(2, 40)
        	Me.lblSearchInfo.Name = "lblSearchInfo"
        	Me.lblSearchInfo.Size = New System.Drawing.Size(296, 14)
        	Me.lblSearchInfo.TabIndex = 2
        	'
        	'btnAdj
        	'
        	Me.btnAdj.FlatStyle = System.Windows.Forms.FlatStyle.System
        	Me.btnAdj.Location = New System.Drawing.Point(396, 36)
        	Me.btnAdj.Name = "btnAdj"
        	Me.btnAdj.Size = New System.Drawing.Size(64, 18)
        	Me.btnAdj.TabIndex = 5
        	Me.btnAdj.Text = "Adjective"
        	Me.btnAdj.Visible = false
        	'
        	'Label3
        	'
        	Me.Label3.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        	Me.Label3.Location = New System.Drawing.Point(480, 6)
        	Me.Label3.Name = "Label3"
        	Me.Label3.Size = New System.Drawing.Size(100, 19)
        	Me.Label3.TabIndex = 7
        	Me.Label3.Text = "Senses:"
        	'
        	'btnNoun
        	'
        	Me.btnNoun.FlatStyle = System.Windows.Forms.FlatStyle.System
        	Me.btnNoun.Location = New System.Drawing.Point(304, 36)
        	Me.btnNoun.Name = "btnNoun"
        	Me.btnNoun.Size = New System.Drawing.Size(40, 18)
        	Me.btnNoun.TabIndex = 3
        	Me.btnNoun.Text = "Noun"
        	Me.btnNoun.Visible = false
        	'
        	'btnAdv
        	'
        	Me.btnAdv.FlatStyle = System.Windows.Forms.FlatStyle.System
        	Me.btnAdv.Location = New System.Drawing.Point(466, 36)
        	Me.btnAdv.Name = "btnAdv"
        	Me.btnAdv.Size = New System.Drawing.Size(48, 18)
        	Me.btnAdv.TabIndex = 6
        	Me.btnAdv.Text = "Adverb"
        	Me.btnAdv.Visible = false
        	'
        	'Panel1
        	'
        	Me.Panel1.Controls.Add(Me.btnAdj)
        	Me.Panel1.Controls.Add(Me.btnAdv)
        	Me.Panel1.Controls.Add(Me.txtSenses)
        	Me.Panel1.Controls.Add(Me.btnNoun)
        	Me.Panel1.Controls.Add(Me.Label1)
        	Me.Panel1.Controls.Add(Me.btnSearch)
        	Me.Panel1.Controls.Add(Me.btnVerb)
        	Me.Panel1.Controls.Add(Me.btnOverview)
        	Me.Panel1.Controls.Add(Me.lblSearchInfo)
        	Me.Panel1.Controls.Add(Me.txtSearchWord)
        	Me.Panel1.Controls.Add(Me.Label3)
        	Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        	Me.Panel1.Location = New System.Drawing.Point(0, 0)
        	Me.Panel1.Name = "Panel1"
        	Me.Panel1.Size = New System.Drawing.Size(592, 60)
        	Me.Panel1.TabIndex = 19
        	'
        	'Panel3
        	'
        	Me.Panel3.Controls.Add(Me.wnColour)
        	Me.Panel3.Controls.Add(Me.StatusBar1)
        	Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
        	Me.Panel3.Location = New System.Drawing.Point(0, 60)
        	Me.Panel3.Name = "Panel3"
        	Me.Panel3.Size = New System.Drawing.Size(592, 339)
        	Me.Panel3.TabIndex = 21
        	'
        	'wnColour
        	'
        	Me.wnColour.CSSBody = "font-family:'Verdana';font-size:10pt;"
        	Me.wnColour.CSSDefinition = "color: #000000; text-decoration: none"
        	Me.wnColour.CSSHover = "background-color: yellow"
        	Me.wnColour.CSSLexeme = "color: #0000FF; font-weight:bold; text-decoration: none"
        	Me.wnColour.CSSPOS = "font-size: 12pt; color: #FFFFFF; font-weight: bold; background-color: #808080"
        	Me.wnColour.CSSQuote = "color: #800000; text-decoration: none"
        	Me.wnColour.Location = New System.Drawing.Point(0, 3)
        	Me.wnColour.Name = "wnColour"
        	Me.wnColour.Size = New System.Drawing.Size(589, 315)
        	Me.wnColour.TabIndex = 12
        	'
        	'StatusBar1
        	'
        	Me.StatusBar1.Location = New System.Drawing.Point(0, 320)
        	Me.StatusBar1.Name = "StatusBar1"
        	Me.StatusBar1.Size = New System.Drawing.Size(592, 19)
        	Me.StatusBar1.TabIndex = 11
        	Me.StatusBar1.Text = "WordNet.Net Colour Formatting Sample"
        	'
        	'StartForm
        	'
        	Me.AutoScaleBaseSize = New System.Drawing.Size(5, 14)
        	Me.ClientSize = New System.Drawing.Size(592, 399)
        	Me.Controls.Add(Me.Panel3)
        	Me.Controls.Add(Me.Panel1)
        	Me.Menu = Me.MainMenu1
        	Me.Name = "StartForm"
        	Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        	Me.Text = "WordNet.Net Colour Formatting Sample"
        	Me.Panel1.ResumeLayout(false)
        	Me.Panel1.PerformLayout
        	Me.Panel3.ResumeLayout(false)
        	Me.ResumeLayout(false)
        End Sub

#End Region

        Public Sub CanvasNavigating(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserNavigatingEventArgs)
            Dim tmpstr As String
            tmpstr = e.Url.ToString()
            tmpstr = Replace(tmpstr, "about:blank", "")
            If tmpstr = "" Then
                Exit Sub
            End If

            e.Cancel = True

            Dim myWriter As New StringWriter
            ' Decode the encoded string.

            txtSearchWord.Text = Replace(tmpstr, "%20", " ")
            btnSearch_Click(Nothing, Nothing)
        End Sub


        Public Shared Sub Main(ByVal args As String())
            Application.EnableVisualStyles()
            Application.DoEvents()
            Application.Run(New StartForm)
        End Sub

        Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            LoadAbout()
        End Sub

        ''' <summary>
        ''' Loads the 'about' summary text.
        ''' </summary>
        Private Sub LoadAbout()
            Dim myFile As System.IO.StreamReader = New System.IO.StreamReader(MyPath() & "\license.txt")
            Dim mystring As String = myFile.ReadToEnd()

            myFile.Close()

			mystring = replace(mystring, vbcrlf, "<br>")
            wnColour.Canvas.DocumentText = mystring

            showFeedback(mystring)
        End Sub

        ''' <summary>
        ''' Gets the path of the application.
        ''' </summary>
        ''' <returns>String representing the path of the application.</returns>
        Private Function MyPath() As String
            'get the app path
            Dim fullAppName As String = [Assembly].GetExecutingAssembly().GetName().CodeBase
            'This strips off the exe name
            Dim FullAppPath As String = Path.GetDirectoryName(fullAppName)

            FullAppPath = Mid(FullAppPath, Len("file:\\"))

            ' following is only during testing
#If CONFIG = "Debug" Then
            FullAppPath = Mid(FullAppPath, 1, InStrRev(FullAppPath, "\"))
#End If

            Return FullAppPath
        End Function

        ''' <summary>
        ''' This is an overview search - the basis for any advanced search.
        ''' </summary>
        Private Sub Overview()
            'overview for 'search'
            Dim t As String
            Dim wnc As WordNetClasses.WN = New WordNetClasses.WN(dictpath)

            se = Nothing ' prevent the output from being overwritten by old results in showresults
            t = txtSearchWord.Text
            lblSearchInfo.Text = "Searches for " + t + ":"
            lblSearchInfo.Visible = True
            btnOverview.Visible = False

            '            txtOutput.Text = ""
            '            txtOutput.Visible = False
            StatusBar1.Text = "Overview of " + t
            Refresh()

            Try
                Dim b As Boolean ' sets the visibility of noun, verb, adj, adv when showing buttons for a word

                list = New ArrayList
                wnc.OverviewFor(t, "noun", b, bobj2, list)
                btnNoun.Visible = b

                wnc.OverviewFor(t, "verb", b, bobj3, list)
                btnVerb.Visible = b

                wnc.OverviewFor(t, "adj", b, bobj4, list)
                btnAdj.Visible = b

                wnc.OverviewFor(t, "adv", b, bobj5, list)
                btnAdv.Visible = b

                txtSearchWord.Text = t
                txtSenses.Text = "0"

                Dim outstr As String = ""

                outstr = wnColour.buildContents(list)

                wnColour.Canvas.DocumentText = outstr
            Catch ex As Exception
                MessageBox.Show(ex.Message & vbCrLf & vbCrLf & "Princeton's WordNet not pre-installed to default location?")
            End Try

            FixDisplay(Nothing)
        End Sub

        Dim se As Wnlib.Search

        Private Sub DoSearch(ByVal opt As Wnlib.Opt)
            If opt.sch.ptp.mnemonic = "OVERVIEW" Then
                Overview()
                Exit Sub
            End If

            '            txtOutput.Text = ""
            Refresh()

            list = New ArrayList
            se = New Wnlib.Search(txtSearchWord.Text, True, opt.pos, opt.sch, Int16.Parse(txtSenses.Text))
            Dim a As Integer = se.buf.IndexOf("\n")
            If (a >= 0) Then
                If (a = 0) Then
                    se.buf = se.buf.Substring(a + 1)
                    a = se.buf.IndexOf("\n")
                End If
                StatusBar1.Text = se.buf.Substring(0, a)
                se.buf = se.buf.Substring(a + 1)
            End If
            list.Add(se)
            If (Wnlib.WNOpt.opt("-h").flag) Then
                help = New Wnlib.WNHelp(opt.sch, opt.pos).help
            End If
            FixDisplay(opt)
        End Sub

        Dim list As ArrayList = New ArrayList
        Dim help As String = ""

        ''' <summary>
        ''' Helper for displaying output and associated housekeeping.
        ''' </summary>
        ''' <param name="opt"></param>
        Public Sub FixDisplay(ByVal opt As Wnlib.Opt)
            pbobject = ""
            ShowResults(opt)

            txtSearchWord.Focus()
        End Sub

        ''' <summary>
        ''' Displays the results of the search.
        ''' </summary>
        ''' <param name="opt">The opt object holds the user-defined search options</param>
        Private Sub ShowResults(ByVal opt As Wnlib.Opt)
            Dim tmpstr As String = ""

            If list.Count = 0 Then
                showFeedback("Search for " & txtSearchWord.Text & " returned 0 results.")
                Exit Sub
            End If

            Dim tmptbox As Overview = New Overview

            If Not pbobject.GetType Is tmptbox.GetType Then
                Dim tb As Overview = New Overview
                tb.useList(list, help, tmpstr)
                If Not help Is Nothing And help <> "" Then
                    tmpstr = help & vbCrLf & vbCrLf & tmpstr
                End If
                tmpstr = Replace(tmpstr, vbLf, vbCrLf)
                tmpstr = Replace(tmpstr, vbCrLf, "", 1, 1)
                tmpstr = Replace(tmpstr, "_", " ")
                showFeedback(tmpstr)

                If tmpstr = "" Or tmpstr = "<font color='green'><br />" & vbCr & " " & txtSearchWord.Text & " has no senses </font>" Then
                    showFeedback("Search for " & txtSearchWord.Text & " returned 0 results.")
                End If
                pbobject = tb
            End If

            txtSearchWord.Focus()

            If Not se Is Nothing Then
                Dim outstr As String = ""

                If se.morphs.Count > 0 Then
                    ' use morphs instead of se
                    Dim wrd As String

                    For Each wrd In se.morphs.Keys
                        ' Build the output with the search results.
                        ' Node hierarchy is automatically constructed
                        list = Nothing
                        list(0) = se.morphs(wrd)
                        outstr = wnColour.buildContents(list)
                    Next
                Else
                    ' there are no morphs - all senses exist in se
                    ' Build the output with the search results.
                    ' Node hierarchy is automatically constructed
                    list(0) = se
                    outstr = wnColour.buildContents(list)
                End If

                wnColour.Canvas.DocumentText = outstr
            End If
        End Sub

        Public bobj2 As Wnlib.SearchSet
        Public bobj3 As Wnlib.SearchSet
        Public bobj4 As Wnlib.SearchSet
        Public bobj5 As Wnlib.SearchSet

        ''' <summary>
        ''' Perform the overview search.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
            Overview()
            txtSearchWord.Focus()
        End Sub

        ''' <summary>
        ''' When the enter key is pressed in the search text field, perform an overview search.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub txtSearchWord_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtSearchWord.KeyDown
            If e.KeyCode = Keys.Enter Then
                e.Handled = True
                btnSearch_Click(Nothing, Nothing)
            End If
        End Sub

        Dim opts As ArrayList = Nothing

        ''' <summary>
        ''' Handles the sense buttons to build and display the appropriate dropdown menu for 
        ''' searches on word relationships.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub btnWordType_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Dim b As Button = sender
            Dim ss As Wnlib.SearchSet = Nothing
            Dim btext As String = b.Text

            If btext = "Adjective" Then
                btext = "Adj"
            End If

            Select Case btext
                Case "Noun"
                    ss = CType(bobj2, Wnlib.SearchSet)

                Case "Verb"
                    ss = CType(bobj3, Wnlib.SearchSet)

                Case "Adj"
                    ss = CType(bobj4, Wnlib.SearchSet)

                Case "Adverb"
                    ss = CType(bobj5, Wnlib.SearchSet)
            End Select

            Dim pos As Wnlib.PartOfSpeech = Wnlib.PartOfSpeech.of(btext.ToLower)
            Dim i As Integer
            opts = New ArrayList
            Dim cm As ContextMenu = New ContextMenu
            Dim tmplst As ArrayList = New ArrayList

            For i = 0 To Wnlib.Opt.Count - 1
                Dim opt As Wnlib.Opt = Wnlib.Opt.at(i)

                If ss(opt.sch.ptp.ident) And opt.pos Is pos Then
                    If tmplst.IndexOf(opt.label) = -1 And opt.label <> "Grep" Then
                        Dim mi As MenuItem = New MenuItem
                        mi.Text = opt.label
                        AddHandler mi.Click, AddressOf searchMenu_Click
                        opts.Add(opt)
                        cm.MenuItems.Add(mi)

                        tmplst.Add(opt.label)
                    End If
                End If
            Next i
            cm.Show(b.Parent, New Point(sender.left, b.Bottom))
        End Sub

        ''' <summary>
        ''' Handles all word relationship menu selections.  Performs a relationship search.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub searchMenu_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            ' one of the options for button2_click was selected
            Dim mi As MenuItem = sender
            Dim opt As Wnlib.Opt = Nothing
            Dim i As Integer
            Dim tmpstr As String

            '            txtOutput.Text = ""
            tmpstr = mi.Text
            tmpstr = Replace(tmpstr, "Syns", "Synonyms")
            tmpstr = Replace(tmpstr, " x ", " by ")
            tmpstr = Replace(tmpstr, "Freq:", "Frequency:")
            StatusBar1.Text = tmpstr
            Refresh()

            For i = 0 To mi.Parent.MenuItems.Count - 1
                If mi.Text = mi.Parent.MenuItems(i).Text Then
                    opt = opts(i)
                End If
            Next i
            DoSearch(opt)
            btnOverview.Visible = True

            Refresh()
        End Sub

        ''' <summary>
        ''' Toggles the wordwrap menu option and sets wrapping on the output text field accordingly.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub mnuWordWrap_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        	' TODO: Fix wordwrap for the webbrowser control
            sender.checked = Not sender.checked

            '            txtOutput.WordWrap = sender.checked
            '            showFeedback(txtOutput.Text)
        End Sub

        ''' <summary>
        ''' Option for controlling whether descriptive help is displayed alongside relationship searches.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub mnuShowHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            sender.Checked = (Not sender.Checked)
            Wnlib.WNOpt.opt("-h").flag = sender.Checked
        End Sub

        ''' <summary>
        ''' Toggles whether a glossary is displayed for relationship searches.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub mnuShowGloss_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            sender.Checked = Not sender.Checked
            Wnlib.WNOpt.opt("-g").flag = Not sender.Checked
        End Sub

        ''' <summary>
        ''' Re-displays the overview search.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub btnOverview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Overview()
        End Sub

        ''' <summary>
        ''' Exits the application.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub mnuExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Application.Exit()
        End Sub

        ''' <summary>
        ''' Clears and resets the entire application interface.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub mnuClearDisplay_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            wnColour.Canvas.DocumentText = ""
            txtSearchWord.Text = ""
            lblSearchInfo.Text = ""
            StatusBar1.Text = "WordNetDT"
            btnNoun.Visible = False
            btnVerb.Visible = False
            btnAdj.Visible = False
            btnAdv.Visible = False
            btnOverview.Visible = False
            btnSearch.Visible = True
        End Sub

        ''' <summary>
        ''' Displays Princeton's WordNet license.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub mnuWordNetLicense_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuWordNetLicense.Click
            Dim myFile As System.IO.StreamReader = New System.IO.StreamReader(MyPath() & "\wordnetlicense.txt")
            Dim mystring As String = myFile.ReadToEnd()

            myFile.Close()

            showFeedback(mystring)
        End Sub

        ''' <summary>
        ''' Displays the 'about' text.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub mnuLGPL_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            LoadAbout()
        End Sub

        ''' <summary>
        ''' Saves the text in the output field.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub mnuSaveDisplay_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            SaveFileDialog1.FileName = txtSearchWord.Text

            If SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                Dim f As StreamWriter = New StreamWriter(SaveFileDialog1.FileName, False)

                f.Write(wnColour.Canvas.DocumentText)
                f.Close()
            End If
        End Sub

        ''' <summary>
        ''' Display the passed text in the output textbox and set the focus to the search input field.
        ''' </summary>
        ''' <param name="mystring">The text to display in the output field</param>
        Private Sub showFeedback(ByVal mystring As String)
            '            txtOutput.Text = mystring
            txtSearchWord.Focus()
        End Sub

        ''' <summary>
        ''' Displays the advanced options dialog.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub mnuAdvancedOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            f3.ShowDialog()
        End Sub
    End Class

	''' <summary>
	''' Displays the basic overview text which is the 'buf' result returned from the WordNet.Net library.
	''' </summary>
    Public Class Overview
        Private cont As ArrayList
        Private totLines As Integer
        Private sw As String
        Private helpLines As Integer

        Sub usePassage(ByVal passage As String, ByRef tmpstr As String)
            tmpstr += passage
        End Sub

        Public Sub useList(ByVal w As ArrayList, ByVal help As String, ByRef tmpstr As String)
            cont = New ArrayList
            totLines = 0
            sw = Nothing

            If Not (help Is Nothing) AndAlso Not (help = "") Then
                usePassage(help, tmpstr)
            End If
            helpLines = totLines
            Dim j As Integer = 0
            While j < w.Count
                Dim se As Wnlib.Search = CType(w(j), Wnlib.Search)
                sw = se.word
                usePassage(se.buf, tmpstr)
                System.Math.Min(System.Threading.Interlocked.Increment(j), j - 1)
            End While
        End Sub
    End Class
End Namespace
