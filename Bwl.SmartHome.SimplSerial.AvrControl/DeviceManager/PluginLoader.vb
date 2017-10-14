Imports System.Net
Imports System.Xml

Public Class PluginLoader
    Private _url As String = ""
    Private _pluginFolder As String = ""

    Sub New(url As String, pluginFolder As String)
        Me._url = url
        Me._pluginFolder = pluginFolder
    End Sub

    Public Sub Start()
        Dim th = New Threading.Thread(AddressOf PluginMonitor)
        th.Start()
    End Sub
    Private Sub PluginMonitor()
        Console.WriteLine("Plugin loader work...")
        While True
            Threading.Thread.Sleep(600)
            Try
                If IO.File.Exists("serverPlugins.xml") Then IO.File.Delete("serverPlugins.xml")
                Console.WriteLine("Pull plugins...")
                Using client = New WebClient()
                    client.DownloadFile("http://" + _url + "/plugins/plugins.xml", "serverPlugins.xml")
                End Using

                Dim xml = New XmlDocument()
                xml.Load("serverPlugins.xml")
                For Each n As XmlNode In xml.SelectNodes("/plugins/plugin")
                    Dim d = DateTime.ParseExact(n.SelectSingleNode("date").InnerText, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture)
                    If IO.File.Exists(_pluginFolder + IO.Path.DirectorySeparatorChar + n.SelectSingleNode("file").InnerText) Then
                        If DateTime.Compare(IO.File.GetCreationTime(_pluginFolder + IO.Path.DirectorySeparatorChar + n.SelectSingleNode("file").InnerText), d) > 1 Then
                            Using client = New WebClient()
                                client.DownloadFile("http://" + _url + "/plugins/" + n.SelectSingleNode("file").InnerText, "temp.dll")
                            End Using
                            IO.File.Delete(_pluginFolder + IO.Path.DirectorySeparatorChar + n.SelectSingleNode("file").InnerText)
                            IO.File.Move("temp.dll", _pluginFolder + IO.Path.DirectorySeparatorChar + n.SelectSingleNode("file").InnerText)
                        End If
                    Else
                        Using client = New WebClient()
                            client.DownloadFile("http://" + _url + "/plugins/" + n.SelectSingleNode("file").InnerText, _pluginFolder + IO.Path.DirectorySeparatorChar + n.SelectSingleNode("file").InnerText)
                        End Using
                    End If
                Next
            Catch ex As Exception
            End Try
            Threading.Thread.Sleep(600000)
        End While
    End Sub
End Class
