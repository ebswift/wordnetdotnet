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
' User: Troy
' Date: 4/07/2006
' Time: 7:21 AM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Partial Public Class WordNetColourFormat
    Public Event CanvasNavigating(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserNavigatingEventArgs)

	' define the CSS for the display elements
	Private m_cssbody As String = "font-family:'Verdana';font-size:10pt;"
	Private m_csslexeme As String = "color: #0000FF; font-weight:bold; text-decoration: none"
	Private m_csspos As String = "font-size: 12pt; color: #FFFFFF; font-weight: bold; background-color: #808080"
	Private m_cssdefn As String = "color: #000000; text-decoration: none"
	Private m_cssquote As String = "color: #800000; text-decoration: none; font-style:italic"
	Private m_csshover As String = "background-color: yellow"
	
    Public Sub New()
        ' The Me.InitializeComponent call is required for Windows Forms designer support.
        Me.InitializeComponent()

        '
        ' TODO : Add constructor code after InitializeComponents
        '

		' setup our custom event for the application to handle
        AddHandler Canvas.Navigating, AddressOf NavigateCanvas
    End Sub

	''' <summary>
	''' Event handler for a user clicking on a word hyperlink inside the webbrowser.
	''' This raises an event that can be handled by the application.
	''' </summary>
	''' <param name="sender"></param>
	''' <param name="e"></param>
    Public Sub NavigateCanvas(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserNavigatingEventArgs)
        RaiseEvent CanvasNavigating(sender, e)
    End Sub

	''' <summary>
	''' Iterates each part of speech which intern calls the iteration to the search 
	''' object to create the hierarchical output.
	''' </summary>
	''' <param name="list"></param>
	''' <param name="tmppos"></param>
	''' <returns></returns>
    Public Function buildContents(ByVal list As ArrayList) As String
        Dim outstr As String = ""
        Dim headstyle As String
        Dim ss As Wnlib.Search

		' define the CSS
		' TODO: move this to properties (property sheet)
        headstyle = "<style>" & vbCrLf & _
            "<!--" & vbCrLf & _
            "*	{ " & CSSBody & " }" & vbCrLf & _
            ".Word a { " & CSSLexeme & " }" & vbCrLf & _
            ".pos        { " & CSSPOS & " }" & vbCrLf & _
            ".Defn a { " & CSSDefinition & " }" & vbCrLf & _
            ".Quote a { " & CSSQuote & " }" & vbCrLf & _
            "a 		{ text-decoration: none }" & vbCrLf & _
            "a:hover { " & CSSHover & " }" & vbCrLf & _
            "-->" & vbCrLf & _
            "</style>" & vbCrLf & vbCrLf

        outstr = "<html><body>" & headstyle

        'For i = 0 To list.Count - 1
        For Each ss In list
            If ss.senses.Count > 0 Then
                outstr += FormatPOS(ss.pos.name)
                outstr += contentIteration(ss)
            End If
        Next

        Return outstr & "</body></html>"
    End Function

	''' <summary>
	''' Takes a given Part Of Speech and iterates the Search object to build the 
	''' hierarchical output.
	''' </summary>
	''' <param name="sch"></param>
	''' <param name="tmppos"></param>
	''' <returns></returns>
    Public Function contentIteration(ByVal sch As Wnlib.Search) As String
        Dim ss As Wnlib.SynSet
        Dim lx As Wnlib.Lexeme
        Dim outstr As String = ""

        outstr += "<ul>"

        ' iterate the returned senses
        For Each ss In sch.senses
            outstr += "<li>"

            ' format the WORDs
            For Each lx In ss.words
                outstr += FormatLexeme(lx.word) & ", "
            Next

            ' remove last comma
            outstr = Mid(outstr, 1, outstr.Length - 2)

            outstr += ": "

            ' show the definition
            outstr += FormatDefn(ss.defn)

            ' children
            If Not ss.senses Is Nothing Then
                outstr += FormatWN(ss.senses)
            End If

            outstr += "</li>"
        Next ss

        outstr += "</ul>"

        Return outstr
    End Function

    ''' <summary>
    ''' Takes a definition (a phrase) and breaks each word apart, creating 
    ''' a hyperlink that can be clicked in order to drill down.
    ''' </summary>
    ''' <param name="tmpstr"></param>
    ''' <returns></returns>
    Private Function linkDefinition(ByVal tmpstr As String) As String
        Dim wrdstart As Integer = -1
        Dim wrdend As Integer = -1
        Dim wrd As String
        Dim tmpstr2 As String
        Dim i As Integer = 1
        Dim chr As Integer

        tmpstr = LTrim(tmpstr)

        If tmpstr = "" Then
            Return tmpstr
        End If

        While (1)
            chr = Asc(Mid(tmpstr, i, 1))
            ' include uppercase, lowercase, numbers and hyphen (respectively)
            If ((chr >= 65 And chr <= 90) Or (chr >= 97 And chr <= 122) Or (chr >= 48 And chr <= 57) Or chr = 45) And wrdstart = -1 Then
                wrdstart = i
                wrdend = -1
            Else
                If (Not ((chr >= 65 And chr <= 90) Or (chr >= 97 And chr <= 122) Or (chr >= 48 And chr <= 57) Or chr = 45)) And wrdstart > -1 Then
                    ' complete word has been found, so replace that instance with a hyperlinked version of self
                    wrdend = i - 1
                    wrd = Mid(tmpstr, wrdstart, (wrdend - wrdstart) + 1)
                    tmpstr2 = Mid(tmpstr, 1, wrdstart - 1)
                    tmpstr = tmpstr2 & Replace(tmpstr, wrd, "<a href='" & wrd & "'>" & wrd & "</a>", i - Len(wrd), 1)
                    wrdstart = -1
                    i += Len("<a href=''></a>") + Len(wrd)
                End If
            End If

            i += 1

            If i > Len(tmpstr) Then
                Exit While
            End If
        End While

        ' end of string has been found but last word has not been processed
        ' - so process it
        If wrdstart > -1 And wrdend = -1 Then
            wrdend = i - 1
            wrd = Mid(tmpstr, wrdstart, (wrdend - wrdstart) + 1)
            tmpstr2 = Mid(tmpstr, 1, wrdstart - 1)
            tmpstr = tmpstr2 & Replace(tmpstr, wrd, "<a href='" & wrd & "'>" & wrd & "</a>", i - Len(wrd), 1)
            wrdstart = -1
            i += Len("<a href=''></a>") + Len(wrd)
        End If

        Return tmpstr
    End Function

	''' <summary>
	''' Takes a sample sentence (defined by being enclosed in quotes) and
	''' defines it for CSS formatting.
	''' </summary>
	''' <param name="tmpstr">The complete definition including sample sentences wrapped in quotes</param>
	''' <returns></returns>
    Private Function formatSampleSentence(ByVal tmpstr As String) As String
        Dim i As Integer = 1
        Dim iflag As Boolean = True

        While i > 0
            i = InStr(i, tmpstr, Chr(34))

            If i > 0 Then
                If iflag Then
                    tmpstr = Mid(tmpstr, 1, i - 1) & Replace(tmpstr, Chr(34), Chr(34) & "<span class='Quote'>", i, 1)
                    i += Len(Chr(34) & "<span class='Quote'>") + 1
                Else
                    tmpstr = Mid(tmpstr, 1, i - 1) & Replace(tmpstr, Chr(34), "</span>" & Chr(34), i, 1)
                    i += Len("</span>" & Chr(34)) + 1
                End If
                iflag = Not iflag
            End If
        End While

        Return tmpstr
    End Function

	''' <summary>
	''' Takes the Part Of Speech heading and wraps it in a span for further 
	''' CSS formatting.
	''' </summary>
	''' <param name="pos">Part Of Speech description (noun, verb, adj, adv)</param>
	''' <returns></returns>
    Public Function FormatPOS(ByVal pos As String) As String
        Dim retstr As String = ""

        Select Case pos
            Case "noun"
                retstr = "Noun"

            Case "verb"
                retstr = "Verb"

            Case "adj"
                retstr = "Adjective"

            Case "adv"
                retstr = "Adverb"
        End Select

        ' prepare for CSS formatting
        retstr = "<div class='pos'>" & retstr & "</div>"

        Return retstr
    End Function

	''' <summary>
	''' Iterates and formats all of the children of the given top-level Search 
	''' object for a given Part Of Speech.
	''' </summary>
	''' <param name="ssarray">The SynSetList for a given Part Of Speech</param>
	''' <returns></returns>
    Private Function FormatWN(ByVal ssarray As Wnlib.SynSetList) As String
        Dim retstr As String = ""
        Dim ss As Wnlib.SynSet
        Dim lx As Wnlib.Lexeme

        For Each ss In ssarray
            retstr += "<li>"

            For Each lx In ss.words
                retstr += FormatLexeme(lx.word) & ", "
            Next

            ' remove last comma
            retstr = Mid(retstr, 1, retstr.Length - 2)

            retstr += ": "

            ' show the definition
            retstr += FormatDefn(ss.defn)

            ' recursive call to self for all children
            If Not ss.senses Is Nothing Then
                retstr += "<ul>"
                retstr += FormatWN(ss.senses)
                retstr += "</ul>"
            End If

            retstr += "</li>"
        Next

        Return retstr
    End Function

	''' <summary>
	''' Formats each lexeme by turning it into a hyperlink.
	''' </summary>
	''' <param name="lx">A lexeme (word)</param>
	''' <returns></returns>
    Private Function FormatLexeme(ByVal lx As String) As String
        Dim retstr As String = ""

        lx = Replace(lx, "_", " ")

        retstr = "<a href='" & lx & "' class='Word'>" & lx & "</a>"

        Return retstr
    End Function

	''' <summary>
	''' Formats the definition (defn) part of the Search object.
	''' </summary>
	''' <param name="defn">The definition part of the Search object</param>
	''' <returns></returns>
    Private Function FormatDefn(ByVal defn As String) As String
        Dim retstr As String = ""

        defn = Replace(defn, "_", " ")

        retstr = "<span class='Defn'>" & formatSampleSentence(linkDefinition(defn)) & "</span>"

        Return retstr
    End Function
    
    Public Property CSSLexeme() As String
    	Get
    		Return m_csslexeme
    	End Get
    	Set
    		m_csslexeme = value
    	End Set
    End Property
    
    Public Property CSSPOS() As String
    	Get
    		Return m_csspos
    	End Get
    	Set
    		m_csspos = value
    	End Set
    End Property
    
    Public Property CSSDefinition() As String
    	Get
    		Return m_cssdefn
    	End Get
    	Set
    		m_cssdefn = value
    	End Set
    End Property
    
    Public Property CSSQuote() As String
    	Get
    		Return m_cssquote
    	End Get
    	Set
    		m_cssquote = value
    	End Set
    End Property
    
    Public Property CSSBody() As String
    	Get
    		Return m_cssbody
    	End Get
    	Set
    		m_cssbody = value
    	End Set
    End Property
    
    Public Property CSSHover() As String
    	Get
    		Return m_csshover
    	End Get
    	Set
    		m_csshover = value
    	End Set
    End Property
End Class
