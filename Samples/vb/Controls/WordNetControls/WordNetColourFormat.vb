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

    Public Sub New()
        ' The Me.InitializeComponent call is required for Windows Forms designer support.
        Me.InitializeComponent()

        AddHandler Canvas.Navigating, AddressOf NavigateCanvas
        '
        ' TODO : Add constructor code after InitializeComponents
        '
    End Sub

    Public Sub NavigateCanvas(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserNavigatingEventArgs)
        RaiseEvent CanvasNavigating(sender, e)
    End Sub

    ' iterates each pos
    Public Function buildContents(ByVal list As ArrayList, ByVal opt As Wnlib.Opt, Optional ByVal tmppos As String = "") As String
        Dim outstr As String = ""
        Dim i As Integer
        Dim headstyle As String

        headstyle = "<style>" & vbCrLf & _
            "<!--" & vbCrLf & _
            "*	{ font-family:'Verdana'; font-size:10pt }" & vbCrLf & _
            ".Word   { color: #0000FF; font-weight:bold }" & vbCrLf & _
            ".Word a { color: #0000FF; font-weight:bold; text-decoration: none }" & vbCrLf & _
            ".pos        { font-size: 12pt; color: #FFFFFF; font-weight: bold; background-color: #808080 }" & vbCrLf & _
            ".Defn   { color: #000000 }" & vbCrLf & _
            ".Defn a { color: #000000; text-decoration: none }" & vbCrLf & _
            ".Quote   { color: #800000; font-style:italic }" & vbCrLf & _
            ".Quote a { color: #800000; text-decoration: none }" & vbCrLf & _
            "a 		{ text-decoration: none }" & vbCrLf & _
            "-->" & vbCrLf & _
            "</style>" & vbCrLf & vbCrLf

        outstr = headstyle

        For i = 0 To list.Count - 1
            If CType(list(i), Wnlib.Search).senses.Count > 0 Then
                outstr += FormatPOS(CType(list(i), Wnlib.Search).pos.name)
                outstr += contentIteration(CType(list(i), Wnlib.Search), opt, tmppos)
            End If
        Next i

        Return outstr
    End Function

    ' turns all words in a definition into hyperlinks
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

    Private Function italicizeQuotes(ByVal tmpstr As String) As String
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

    ' formats each pos
    Public Function contentIteration(ByVal sch As Wnlib.Search, ByVal opt As Wnlib.Opt, Optional ByVal tmppos As String = "") As String
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

    Private Function FormatDefn(ByVal defn As String) As String
        Dim retstr As String = ""

        defn = Replace(defn, "_", " ")

        retstr = "<span class='Defn'>" & italicizeQuotes(linkDefinition(defn)) & "</span>"

        Return retstr
    End Function

    Private Function FormatLexeme(ByVal lx As String) As String
        Dim retstr As String = ""

        lx = Replace(lx, "_", " ")

        retstr = "<a href='" & lx & "' class='Word'>" & lx & "</a>"

        Return retstr
    End Function

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
End Class
