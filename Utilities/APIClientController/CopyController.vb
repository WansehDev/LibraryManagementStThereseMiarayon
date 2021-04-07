﻿Imports Newtonsoft.Json

Module CopyController

    Private URL As String = "http://localhost:8080/api/v1/book"
    Public Function getCopies(ByRef bookId As Int64) As List(Of BookCopyDTO)
        Dim response As String = HttpRequestController.HttpRequestGet(URL + "/all/copies/" + bookId.ToString)
        Dim responseObj As List(Of BookCopyDTO) = JsonConvert.DeserializeObject(Of List(Of BookCopyDTO))(response)
        Return responseObj
    End Function
    Public Sub updateCopiesOfBook(ByRef bookId As Int64, ByRef copies As List(Of BookCopyDTO))
        Dim newUrl = URL + "/admin/" + bookId.ToString() + "/copies"
        Dim mes = JsonConvert.SerializeObject(copies)
        HttpRequestController.HttpRequestPut(newUrl, mes, Authorization.authToken)
    End Sub
End Module