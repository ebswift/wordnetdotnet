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

Public Class WebForm1
    Inherits System.Web.UI.Page

    ' *** set the dictpath to the location of your WordNet files
    Private dictpath As String = "C:\Program Files\WordNet\2.1\dict\"
    Private wnc As WordNetClasses.WN = New WordNetClasses.WN(dictpath)
    Dim pbobject As Object = New Object


    Protected WithEvents Adv As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Adj As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Verb As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Image1 As System.Web.UI.WebControls.Image
    Protected WithEvents lblVerb As System.Web.UI.WebControls.Label
    Protected WithEvents lblAdj As System.Web.UI.WebControls.Label
    Protected WithEvents lblAdv As System.Web.UI.WebControls.Label
    Protected WithEvents Noun As System.Web.UI.WebControls.DropDownList
    Protected WithEvents lblNoun As System.Web.UI.WebControls.Label
    Protected WithEvents chkWordWrap As System.Web.UI.WebControls.CheckBox

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Protected WithEvents Button1 As System.Web.UI.WebControls.Button
    Protected WithEvents Button6 As System.Web.UI.WebControls.Button
    Protected WithEvents TextBox1 As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label1 As System.Web.UI.WebControls.Label
    Protected WithEvents Label3 As System.Web.UI.WebControls.Label
    Protected WithEvents Label2 As System.Web.UI.WebControls.Label
    Protected WithEvents TextBox2 As System.Web.UI.WebControls.TextBox
    Protected WithEvents StatusBar1 As System.Web.UI.WebControls.Label
    Protected WithEvents lblResult As System.Web.UI.WebControls.Label

    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()

        'Wnlib.WNDB.path = "e:\html\domains\ebswift.com\LocalFiles\WordNet\dict\"
    End Sub

#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Put user code to initialize the page here
        If Not context.Items("searchword") Is Nothing Then
            TextBox1.Text = Context.Items("searchword")
        End If
        If Not context.Request.Params("Word") Is Nothing And Request.RequestType = "GET" Then
            If context.Request.Params("Word") <> TextBox1.Text Then
                TextBox1.Text = context.Request.Params("Word")
                Button6_Click(Nothing, Nothing)
            End If
        End If
    End Sub

    ' search button
    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        Try
            Context.Items.Add("searchword", TextBox1.Text)
        Catch
        End Try

        Dim t As String

        Button1.Visible = False
        If Not Session("opts") Is Nothing Then
            Session("opts").Clear()
        End If

        t = TextBox1.Text
        Label2.Text = "Searches for " + t + ":"
        Label2.Visible = True
        Overview()
    End Sub

    ' overview search
    Private Sub Overview()
        Dim x As Button
        Dim t As String
        Dim b As Boolean ' sets the visibility of noun, verb, adj, adv when showing buttons for a word

        Try
            t = TextBox1.Text
            list = New ArrayList
            wnc.OverviewFor(t, "noun", b, Session("bobj2"), list)
            lblNoun.Visible = b
            Noun.Visible = b
            SetupDropdown(Noun)
            wnc.OverviewFor(t, "verb", b, Session("bobj3"), list)
            lblVerb.Visible = b
            Verb.Visible = b
            SetupDropdown(Verb)
            wnc.OverviewFor(t, "adj", b, Session("bobj4"), list)
            lblAdj.Visible = b
            Adj.Visible = b
            SetupDropdown(Adj)
            wnc.OverviewFor(t, "adv", b, Session("bobj5"), list)
            lblAdv.Visible = b
            Adv.Visible = b
            SetupDropdown(Adv)
            TextBox1.Text = t
            StatusBar1.Text = "Overview of " + t
            TextBox2.Text = "0"
            FixDisplay()
        Catch
            Session.Abandon() ' this is to try and catch session timeouts - taken from WordNetASPNet; it appears to have been working stable since this code was introduced
        End Try
    End Sub

    ' sets up the dropdown lists for relation searches
    Private Sub SetupDropdown(ByVal dl As DropDownList)
        Dim ss As Wnlib.SearchSet

        Select Case dl.ID
            Case "Noun"
                ss = Session("bobj2")

            Case "Verb"
                ss = Session("bobj3")

            Case "Adj"
                ss = Session("bobj4")

            Case "Adv"
                ss = Session("bobj5")
        End Select

        If ss Is Nothing Then
            Exit Sub
        End If

        Dim pos As Wnlib.PartOfSpeech = Wnlib.PartOfSpeech.of(dl.ID.ToLower())
        Dim i As Integer
        Session("opts") = New ArrayList

        dl.Items.Clear()

        dl.Items.Add("")
        Dim tmplst As ArrayList = New ArrayList

        For i = 0 To Wnlib.Opt.Count - 1
            Dim opt As Wnlib.Opt = opt.at(i)
            If ss(opt.sch.ptp.ident) And opt.pos Is pos Then
                If tmplst.IndexOf(opt.label) = -1 And opt.label <> "Grep" Then
                    dl.Items.Add(opt.label)
                    Session("opts").Add(opt)

                    tmplst.Add(opt.label)
                End If
            End If
        Next i
    End Sub

    Public Sub GetOpts(ByVal dl As DropDownList)
        ' gets the opts when we don't want to tamper with the dropdowns
        Dim ss As Wnlib.SearchSet

        Select Case dl.ID
            Case "Noun"
                ss = Session("bobj2")

            Case "Verb"
                ss = Session("bobj3")

            Case "Adj"
                ss = Session("bobj4")

            Case "Adv"
                ss = Session("bobj5")
        End Select

        If ss Is Nothing Then
            Exit Sub
        End If

        Dim pos As Wnlib.PartOfSpeech = Wnlib.PartOfSpeech.of(dl.ID.ToLower())
        Dim i As Integer
        Session("opts") = New ArrayList
        Dim tmplst As ArrayList = New ArrayList

        For i = 0 To Wnlib.Opt.Count - 1
            Dim opt As Wnlib.Opt = opt.at(i)
            If tmplst.IndexOf(opt.label) = -1 And opt.label <> "Grep" Then
                If ss(opt.sch.ptp.ident) And opt.pos Is pos Then
                    Session("opts").Add(opt)
                    tmplst.Add(opt.label)
                End If
            End If
        Next i
    End Sub

    Dim list As ArrayList = New ArrayList
    Dim help As String = ""

    ' first step in showing results
    Public Sub FixDisplay()
        pbobject = ""
        ShowResult()
    End Sub

    ' second step in showing results
    Public Sub ShowResult()
        Dim tmpstr As String = ""

        Try
            If list.Count = 0 Then
                Exit Sub
            End If

            ' this exists only for the type comparison
            Dim tmptbox As Overview = New Overview

            If Not pbobject.GetType Is tmptbox.GetType Then
                Dim tb As Overview = New Overview
                lblResult.Text = ""
                tb.useList(list, help, tmpstr)

                tmpstr = Replace(tmpstr, "_", " ")
                showFeedback(tmpstr, True)
                lblResult.Visible = True
                TextBox1.Enabled = True
                pbobject = tb
            End If
        Catch ex As Exception
            WebHelper.RaiseAlert("ShowResult: " & ex.Message)
        End Try
    End Sub

    ' handles everything that is going to be displayed in the main output
    Private Sub showFeedback(ByVal mystring As String, ByVal reformat As Boolean)
        ' formatting here does not show indentation...
        mystring = Replace(mystring, vbLf, "<br />")

        If reformat Then
            Dim headstyle As String
            ' define the size of the table used to counter word wrapping
            Dim nowraptbl As String = "<TABLE id=table1 width=10000 border=0><TBODY><TR><TD>"
            ' define the closing of the above table
            Dim closetbl As String = "</TD></TR></TBODY></TABLE>"

            headstyle = "<style>" & vbCrLf & _
                "<!--" & vbCrLf & _
                "*	{ font-family:'Verdana'; font-size:10pt }" & vbCrLf & _
                ".Word   { color: #0000FF; font-weight:bold }" & vbCrLf & _
                ".Word a { color: #0000FF; font-weight:bold; text-decoration: none }" & vbCrLf & _
                ".Type        { font-size: 12pt; color: #FFFFFF; font-weight: bold; background-color: #808080 }" & vbCrLf & _
                ".Defn   { color: #000000 }" & vbCrLf & _
                ".Defn a { color: #000000; text-decoration: none }" & vbCrLf & _
                ".Quote   { color: #800000; font-style:italic }" & vbCrLf & _
                ".Quote a { color: #800000; text-decoration: none }" & vbCrLf & _
                "a 		{ text-decoration: none }" & vbCrLf & _
                "-->" & vbCrLf & _
                "</style>" & vbCrLf & vbCrLf

            lblResult.Text += headstyle

            ' word wrapping is off, so place inside a very large table (we cheat)
            If Not chkWordWrap.Checked Then
                lblResult.Text += nowraptbl
            End If

            ' write the output
            lblResult.Text += mystring

            ' if word wrapping is off then close our large table
            If Not chkWordWrap.Checked Then
                lblResult.Text += closetbl
            End If
        Else
            lblResult.Text = mystring
        End If
    End Sub

    ' advanced options button
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            Context.Items.Add("searchword", TextBox1.Text)
        Catch
        End Try
        Server.Transfer("AdvancedSearch.aspx", True)
    End Sub

    ' handles a word relation selection
    Private Sub SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Noun.SelectedIndexChanged, Verb.SelectedIndexChanged, Adj.SelectedIndexChanged, Adv.SelectedIndexChanged
        Dim opt As Wnlib.Opt

        If sender.selectedindex <= 0 Then
            Exit Sub
        End If

        GetOpts(sender)

        opt = Session("opts")(sender.selectedindex - 1)

        If sender.id <> "Noun" Then
            If Noun.Items.Count > 0 Then
                Noun.SelectedIndex = 0
            End If
        End If
        If sender.id <> "Verb" Then
            If Verb.Items.Count > 0 Then
                Verb.SelectedIndex = 0
            End If
        End If
        If sender.id <> "Adj" Then
            If Adj.Items.Count > 0 Then
                Adj.SelectedIndex = 0
            End If
        End If
        If sender.id <> "Adv" Then
            If Adv.Items.Count > 0 Then
                Adv.SelectedIndex = 0
            End If
        End If

        DoSearch(opt)
        Button1.Visible = True
    End Sub

    ' word relation search
    Private Sub DoSearch(ByVal opt As Wnlib.Opt)
        If opt.sch.ptp.mnemonic = "OVERVIEW" Then
            Overview()
            Exit Sub
        End If

        list = New ArrayList
        Dim se As Wnlib.Search = New Wnlib.Search(TextBox1.Text, True, opt.pos, opt.sch, Int16.Parse(TextBox2.Text))
        Dim a As Integer = InStr(se.buf, Chr(10))

        If (a >= 0) Then
            If (a = 1) Then
                se.buf = se.buf.Substring(a)
                a = se.buf.IndexOf(Chr(10))
            End If

            StatusBar1.Text = se.buf.Substring(0, a)
        End If

        list.Add(se)
        If (Wnlib.WNOpt.opt("-h").flag) Then
            help = New Wnlib.WNHelp(opt.sch, opt.pos).help
        End If
        FixDisplay()
    End Sub

    ' back to overview after viewing a relation
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Button6_Click(Nothing, Nothing)
    End Sub

    ' change wordwrap settings
    Private Sub chkWordWrap_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkWordWrap.CheckedChanged
        Dim tmpstr As String = lblResult.Text

        If chkWordWrap.Checked Then
            tmpstr = Replace(tmpstr, "<TABLE id=table1 width=10000 border=0><TBODY><TR><TD>", "")
            tmpstr = Replace(tmpstr, "</TD></TR></TBODY></TABLE>", "")
        Else
            tmpstr = "<TABLE id=table1 width=10000 border=0><TBODY><TR><TD>" & tmpstr & "</TD></TR></TBODY></TABLE>"
        End If

        lblResult.Text = ""
        showFeedback(tmpstr, False)
    End Sub
End Class

' things that help display on the web
Public Class WebHelper
    Public Shared Sub RaiseAlert(ByVal msg As String)
        ' sends an alert to the user via a popup messagebox
        If InStr(msg, vbCrLf) Then
            msg = Replace(msg, vbCrLf, "\n")
        End If

        System.Web.HttpContext.Current.Response.Write("<script language=javascript>alert(" & Chr(34) & msg & Chr(34) & ")</script>")
    End Sub
End Class

Public Class Overview
    ' really basic thrown together class which makes the plain text search results
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
