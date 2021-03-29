﻿Imports System.IO
Imports System.Net
Imports System.Text

Module HttpRequestController
    Public Function HttpRequestGet(URL As String) As String
        Dim request As WebRequest = WebRequest.Create(URL)
        Dim dataStream As Stream = request.GetResponse.GetResponseStream()
        Dim sr As New StreamReader(dataStream)
        Return sr.ReadToEnd()
    End Function
    Public Function HttpRequestGetStream(URL As String) As Stream
        Dim request As WebRequest = WebRequest.Create(URL)
        Dim res = CType(request.GetResponse(), HttpWebResponse)
        Return res.GetResponseStream()
    End Function

    Public Function HttpRequestPost(URL As String, mes As String) As String

        Dim request As WebRequest
        request = WebRequest.CreateHttp(URL)
        Dim response As WebResponse
        Dim postData As String = mes
        Dim jsonBytes As Byte() = Encoding.UTF8.GetBytes(postData)


        request.Method = "POST"
        request.ContentType = "application/json; charset=utf-8"
        request.ContentLength = jsonBytes.Length

        Dim stream As Stream = request.GetRequestStream()
        stream.Write(jsonBytes, 0, jsonBytes.Length)
        stream.Close()

        response = request.GetResponse()
        Dim sr As New StreamReader(response.GetResponseStream())
        Dim responseContent As String = String.Empty

        Using res = DirectCast(request.GetResponse(), HttpWebResponse),
            responseStream = res.GetResponseStream(),
            reader = New StreamReader(responseStream)
            responseContent = reader.ReadToEnd()
        End Using
        Return responseContent
    End Function
    Public Sub HttpRequestPut(URL As String)

        Dim request As WebRequest
        request = WebRequest.Create(URL)

        request.Method = "PUT"

        Dim stream As Stream = request.GetRequestStream()

        Dim response = request.GetResponse()
        Dim sr As New StreamReader(response.GetResponseStream())
        Dim responseContent As String = String.Empty

        Using res = DirectCast(request.GetResponse(), HttpWebResponse),
            responseStream = res.GetResponseStream(),
            reader = New StreamReader(responseStream)
            responseContent = reader.ReadToEnd()
        End Using
    End Sub

    Sub HttpRequestPut(URL As String, mes As String)
        Dim request As WebRequest
        request = WebRequest.CreateHttp(URL)
        Dim response As WebResponse
        Dim postData As String = mes
        Dim jsonBytes As Byte() = Encoding.UTF8.GetBytes(postData)


        request.Method = "PUT"
        request.ContentType = "application/json; charset=utf-8"
        request.ContentLength = jsonBytes.Length

        Dim stream As Stream = request.GetRequestStream()
        stream.Write(jsonBytes, 0, jsonBytes.Length)
        stream.Close()

        response = request.GetResponse()
        Dim sr As New StreamReader(response.GetResponseStream())
        Dim responseContent As String = String.Empty

        Using res = DirectCast(request.GetResponse(), HttpWebResponse),
            responseStream = res.GetResponseStream(),
            reader = New StreamReader(responseStream)
            responseContent = reader.ReadToEnd()
        End Using
    End Sub

    Sub HttpRequestDelete(URL As String)
        Dim request As WebRequest = WebRequest.Create(URL)
        request.Method = "DELETE"

        Dim response = request.GetResponse()
        Dim sr As New StreamReader(response.GetResponseStream())
        Dim responseContent As String = String.Empty

        Using res = DirectCast(request.GetResponse(), HttpWebResponse),
            responseStream = res.GetResponseStream(),
            reader = New StreamReader(responseStream)
            responseContent = reader.ReadToEnd()
        End Using
    End Sub
End Module