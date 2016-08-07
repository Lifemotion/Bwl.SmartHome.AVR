Imports Bwl.Hardware.SimplSerial

Public MustInherit Class SsBaseDriver
    Implements ISsDriver
    Protected _bus As SimplSerialBus
    Protected _logger As Framework.Logger
    Protected _shc As SmartHomeClient
    Protected _serverObject As SmartObject
    Protected _rnd As New Random
    Protected _lastSuccessRequest As DateTime

    Public MustOverride Sub PollSimplSerial() Implements ISsDriver.PollSimplSerial

    Public ReadOnly Property Guid As String Implements ISsDriver.Guid
        Get
            Return _serverObject.Guid
        End Get
    End Property

    Public Sub New(bus As SimplSerialBus, logger As Framework.Logger, guid As String, shc As SmartHomeClient)
        _bus = bus
        _logger = logger
        _shc = shc

        _serverObject = New SmartObject(guid)
    End Sub

    Public Sub UpdateServerObjects() Implements ISsDriver.UpdateServerObjects
        If (Now - _lastSuccessRequest).TotalMinutes < 1 Then
            _shc.SmartHome.Objects.SetObject(_serverObject, SmartObjectSetMask.configOnlyReplaceEmpty Or SmartObjectSetMask.statesOnlyReplaceEmpty)
            Dim changedObject = _shc.SmartHome.Objects.GetObject(_serverObject.Guid)
            If changedObject IsNot Nothing Then _serverObject = changedObject
        End If
    End Sub

    Protected Function BusRequestByGuid(command As Byte, data() As Byte) As SSResponse
        Return BusRequestByGuid(New SSRequest(0, command, data))
    End Function

    Protected Function BusRequestByGuid(request As SSRequest) As SSResponse
        Dim address = _rnd.Next(1, 30000)
        _bus.RequestSetAddress(System.Guid.Parse(_serverObject.Guid), address)
        request.Address = address
        Dim response = _bus.Request(request)
        Return response
    End Function

End Class
