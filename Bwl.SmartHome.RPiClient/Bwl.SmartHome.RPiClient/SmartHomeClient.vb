Imports Bwl.Framework
Imports Bwl.Network.ClientServer

Public Class SmartHomeClient
    Protected _logger As Logger = New Logger()
    Protected _storage As SettingsStorageRoot
    Public _guid As StringSetting
    Public Property SmartHome As New SmartHome
    Public Event SendObjectsSchemesTimer()

    Private _sendObjectsTimer As New System.Timers.Timer
    Private WithEvents _client As MessageTransport

    Public Sub New()
        Dim rnd As New Random
        _storage = New SettingsStorageRoot()
        _guid = New StringSetting(_storage, "ComputerObjectGuid", GuidTool.GuidToString)
        _client = New MessageTransport(_storage.CreateChildStorage("Transport"), _logger.CreateChildLogger("Transport"), "NetClient", "194.87.144.245:3210", "SmartHomeClient" + rnd.Next.ToString, "SmartHomeServer", "SmartHomeServer", True)
        SmartHome.Objects = New RemoteSmartObjects(_client)
        _sendObjectsTimer.Interval = 30000
        _sendObjectsTimer.AutoReset = True
        _sendObjectsTimer.Start()
        AddHandler _sendObjectsTimer.Elapsed, AddressOf SendObjectsTimerHandler
        Try
            _client.OpenAndRegister()
            'SendObjectsTimerHandler()
        Catch ex As Exception
        End Try
    End Sub

    Public Sub SendObjectsTimerHandler()
        If _client IsNot Nothing AndAlso _client.IsConnected Then
            RaiseEvent SendObjectsSchemesTimer()
        End If
    End Sub

    Public ReadOnly Property Transport As MessageTransport
        Get
            Return _client
        End Get
    End Property
End Class