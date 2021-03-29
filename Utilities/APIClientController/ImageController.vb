﻿Imports System.IO
Imports System.Net.Http

Module ImageController
    Private URL As String = "http://localhost:8080/api/v1/images"

    Public Function uploadImage(imgFlNm As String, id As String) As String
        Dim newURL = URL + "/upload/?file=" + id  ' set the filename

        Dim response As New HttpResponseMessage
        Dim path As String = imgFlNm
        Dim uri As New Uri(newURL)

        Using form As New MultipartFormDataContent,
            fs As New FileStream(path, FileMode.Open),
            content As New StreamContent(fs),
            httpClient As New HttpClient

            form.Add(content, "file", id)
            ' TODO: add authentication
            'httpClient.DefaultRequestHeaders.Add("Token", "Super Secret")
            'httpClient.DefaultRequestHeaders.Add("name", id)

            response = httpClient.PostAsync(uri, form).GetAwaiter.GetResult()
            System.Diagnostics.Debug.WriteLine("Is Successfull: " & response.IsSuccessStatusCode)
        End Using
    End Function

    Public Function getImage(fName As String) As Image
        Dim newURL = URL + "/getImage/" + fName
        Dim img = Image.FromStream(HttpRequestController.HttpRequestGetStream(newURL))
        Return img
    End Function

    Public Sub removeImage(fName As String)
        Dim newURL = URL + "/" + fName
        HttpRequestController.HttpRequestDelete(newURL)
    End Sub

End Module