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

' Check Word Defined
'
' Returns a boolean as to whether a word exists for a given
' part of speech (noun, verb, adjective, adverb)

Imports WordNetClasses.WN

Public Partial Class MainForm
    Private dictpath As String = "C:\Program Files\WordNet\2.1\dict\"

	Public Sub New()
		' The Me.InitializeComponent call is required for Windows Forms designer support.
		Me.InitializeComponent()
		
		'
		' TODO : Add constructor code after InitializeComponents
		'

		' you must set the path to the WordNet database before performing 
		' any operation with the library.
		Wnlib.WNCommon.path = dictpath
		cboPOS.SelectedIndex = 0 ' ensure our combobox can't have a null value
	End Sub
	
	Sub BtnGoClick(sender As Object, e As System.EventArgs)
        lblResult.Text = Wnlib.WNDB.is_defined(txtWord.Text, Wnlib.PartOfSpeech.of(cboPOS.SelectedItem.ToString)).NonEmpty.ToString
    End Sub
End Class
