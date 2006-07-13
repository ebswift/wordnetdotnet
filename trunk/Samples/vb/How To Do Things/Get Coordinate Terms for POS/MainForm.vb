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

' Get Coordinate Terms for POS
' Returns a list of coordinate terms for a given part of speech.

Imports Wnlib

Public Partial Class MainForm
    Private dictpath As String = "C:\Program Files\WordNet\2.1\dict\"
    Private wnc As WordNetClasses.WN = New WordNetClasses.WN(dictpath)

	Public Sub New()
		' The Me.InitializeComponent call is required for Windows Forms designer support.
		Me.InitializeComponent()
		
		'
		' TODO : Add constructor code after InitializeComponents
		'
		
		cboPOS.SelectedIndex = 0
	End Sub
	
	Sub BtnGoClick(sender As Object, e As System.EventArgs)
        Dim se As Wnlib.Search = Nothing
        Dim list As ArrayList = New ArrayList
		Dim hascoords As Boolean = False
        Dim ss As SearchSet = Nothing
        Dim sense As SynSet
        Dim lx As Wnlib.Lexeme
        Dim i As Integer = 0
        Dim outcoords As ArrayList = New ArrayList
        Dim tmpstr As String = ""

        GetCoords(txtWord.Text, cboPOS.SelectedItem.ToString, hascoords, ss, list)

        txtResult.Text = ""

        For Each se In list
            For Each sense In se.senses
                For Each lx In sense.words
                    tmpstr = lx.word.Replace("_", " ")
                    If outcoords.IndexOf(LCase(tmpstr)) = -1 Then
                        outcoords.Add(LCase(tmpstr))
                    End If
                Next
            Next
        Next

        For i = 0 To outcoords.Count - 1
            txtResult.AppendText(outcoords(i).ToString & vbCrLf)
        Next
    End Sub

	Private Sub GetCoords(ByVal t As string, ByVal p As string, ByRef b As Boolean, ByRef obj As SearchSet, ByRef list As ArrayList)
		Dim pos As PartOfSpeech = Wnlib.PartOfSpeech.of(p)
		Dim ss As SearchSet = Wnlib.WNDB.is_defined(t,pos)
		Dim ms As MorphStr = new Wnlib.MorphStr(t,pos)
		AddSearchFor(t,pos, list) ' do a search
		Dim m As String = ""

		' loop through morphs (if there are any)
        While 1 = 1
            m = ms.next()
            If m = Nothing Then
                Exit While
            End If

            If m <> t Then
                ss = ss + WNDB.is_defined(m, pos)
                AddSearchFor(m, pos, list)
            End If
        End While
		b = ss.NonEmpty
		obj = ss
	End Sub

	Private Sub AddSearchFor(ByVal s As string, ByVal pos As PartOfSpeech, ByRef list As ArrayList)
        Dim se As Search = New Search(s, False, pos, New SearchType(False, "COORDS"), 0)
		list.Add(se)
	End Sub
End Class
