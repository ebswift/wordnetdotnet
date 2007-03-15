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
	
	''' <summary>
	''' Get the coordinate terms for the given word and part of speech.
	''' </summary>
	''' <param name="sender"></param>
	''' <param name="e"></param>
	Sub BtnGoClick(sender As Object, e As System.EventArgs)
        Dim list As ArrayList = New ArrayList ' used to store an array of searches populated by GetCoords
        Dim se As Wnlib.Search = Nothing ' used to iterate searches in list
        Dim morphsearch As System.Collections.DictionaryEntry = Nothing ' used to iterate morph searches from se
        Dim hascoords As Boolean = False ' used to determine whether there are any coordinate terms for the word and part of speech
        Dim ss As SearchSet = Nothing ' used to retrieve the SearchSet from GetCoords
        Dim sense As SynSet ' used to iterate each sense in se
        Dim i As Integer = 0 ' iterator to build our output
        Dim outcoords As ArrayList = New ArrayList ' used to build a list of words for the output
        Dim tmpstr As String = "" ' helper string for building the output

		' Get the coordinate terms
        GetCoords(txtWord.Text, cboPOS.SelectedItem.ToString, hascoords, ss, list)

        txtResult.Text = ""

		' iterate the searches in the list.
		' because we are only collecting coordinate terms
		' we do not take this to any depth beyond the parent
		' and its morphs.
        For Each se In list
            For Each sense In se.senses
                ProcessSense(sense, outcoords)
            Next

            ' loop morphs - they can be treated as just another search (because that's what they are)
            For Each morphsearch In se.morphs
                For Each sense In CType(morphsearch.Value, Search).senses
                    ProcessSense(sense, outcoords)
                Next
            Next
        Next

		' output the list of coordinate terms
        For i = 0 To outcoords.Count - 1
            txtResult.AppendText(outcoords(i).ToString & vbCrLf)
        Next
    End Sub

	''' <summary>
	''' Adds our sense lexemes to the output array
	''' </summary>
	''' <param name="sense"></param>
	''' <param name="outarr"></param>
    Private Sub ProcessSense(ByVal sense As SynSet, ByRef outarr As ArrayList)
        Dim lx As Lexeme
        Dim tmpstr As String

        For Each lx In sense.words
            tmpstr = lx.word.Replace("_", " ")
            If outarr.IndexOf(LCase(tmpstr)) = -1 Then
                outarr.Add(LCase(tmpstr))
            End If
        Next
    End Sub

	''' <summary>
	''' Processes Part of Speech, retrieves the SearchSet and makes the call to AddSearchFor.
	''' </summary>
	''' <param name="wrd">Word to search for</param>
	''' <param name="p">Part of Speech in string form</param>
	''' <param name="b">This tells the calling method whether any results were found</param>
	''' <param name="outss">SearchSet retrieved by the call to is_defined</param>
	''' <param name="list">ArrayList to add search results to</param>
	Private Sub GetCoords(ByVal wrd As string, ByVal p As string, ByRef b As Boolean, ByRef outss As SearchSet, ByRef list As ArrayList)
		Dim pos As PartOfSpeech = Wnlib.PartOfSpeech.of(p)
		Dim ss As SearchSet = Wnlib.WNDB.is_defined(wrd,pos)

        AddSearchFor(wrd, pos, list, "COORDS")
        b = ss.NonEmpty
		outss = ss
	End Sub

	''' <summary>
	''' Adds a new Search to the array 'list'.
	''' </summary>
	''' <param name="wrd">The word to add the search for</param>
	''' <param name="pos">Part Of Speech to search for</param>
	''' <param name="list">The array of Search objects</param>
	''' <param name="schtype">The type of search to perform as defined in the empty Opt constructor</param>
	Private Sub AddSearchFor(ByVal wrd As string, ByVal pos As PartOfSpeech, ByRef list As ArrayList, ByVal schtype As String)
        Dim se As Search = New Search(wrd, True, pos, New SearchType(False, schtype), 0)
        list.Add(se)
	End Sub
End Class
