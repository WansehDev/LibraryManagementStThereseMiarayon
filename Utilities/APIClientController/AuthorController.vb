﻿Imports System.IO
Imports System.Net
Imports System.Text
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports WindowsApp1.HttpRequestController

Module AuthorController


    Private URL As String = "http://localhost:8080/api/v1/author"


    Public Function getAuthors() As List(Of AuthorDTO)
        Dim response As String = HttpRequestController.HttpRequestGet(URL)
        Dim output = JsonConvert.DeserializeObject(Of List(Of AuthorDTO))(response)

        Return output
    End Function

    Public Function getAuthor(authorId As String) As AuthorDTO
        Dim newURL As String = URL + "/" + authorId

        Dim response As String = HttpRequestController.HttpRequestGet(newURL)
        Return JsonConvert.DeserializeObject(Of AuthorDTO)(response)

    End Function

    Function getFullName(entity As JObject) As Dictionary(Of
            String, String)
        Dim entityDict As New Dictionary(Of
            String, String)
        entityDict.Add("id", entity("id"))
        entityDict.Add("f_name", entity("f_name"))
        entityDict.Add("m_name", entity("m_name"))
        entityDict.Add("l_name", entity("l_name"))
        Return entityDict
    End Function

    Sub addAuthor(mes As String)
        HttpRequestController.HttpRequestPost(URL, mes)
    End Sub

    Function findAuthorByName(attrs As Dictionary(Of
            String, String))

        Dim newUrl As String = URL + "/q?"

        For Each key As String In attrs.Keys
            newUrl = newUrl + key + "=" + attrs.Item(key) + "&"
        Next

        Return JsonConvert.DeserializeObject(Of AuthorDTO)(HttpRequestController.HttpRequestGet(newUrl))

    End Function

    Sub updateAuthor(id As String, attrs As Dictionary(Of
            String, String))

        Dim newUrl As String = URL + "/" + id + "/?"

        For Each key As String In attrs.Keys
            newUrl = newUrl + key + "=" + attrs.Item(key) + "&"
        Next
        HttpRequestController.HttpRequestPut(newUrl)
    End Sub

    Sub deleteAuthor(id As String)
        Dim newURL As String = URL + "/" + id
        HttpRequestController.HttpRequestDelete(newURL)
    End Sub
End Module
