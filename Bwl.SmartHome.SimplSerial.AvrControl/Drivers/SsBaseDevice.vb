Imports Bwl.Hardware.SimplSerial

Public MustInherit Class SsBaseDevice
    Implements ISsDevice
    Private _bus As SimplSerialBus
    Protected _logger As Framework.Logger
    Protected _shc As SmartHomeClient
    Protected _guid As String

    Protected _objectScheme As New SmartObjectScheme

    Protected _rnd As New Random
    Protected _lastSuccessRequest As DateTime

    Public MustOverride Sub PollSimplSerial() Implements ISsDevice.PollSimplSerial

    Public ReadOnly Property Guid As String Implements ISsDevice.Guid
        Get
            Return _guid
        End Get
    End Property

    Public Sub New(bus As SimplSerialBus, logger As Framework.Logger, guid As String, shc As SmartHomeClient)
        _bus = bus
        _logger = logger
        _shc = shc
        _guid = guid
    End Sub

    Public Sub UpdateServerObjects() Implements ISsDevice.UpdateServerObjects
        _shc.SmartHome.Objects.SetScheme(_guid, _objectScheme)
    End Sub

    Protected Function BusRequestByGuid(command As Byte, data() As Byte) As SSResponse
        Return BusRequestByGuid(New SSRequest(0, command, data))
    End Function

    Protected Function BusRequestByGuid(request As SSRequest) As SSResponse
        SyncLock Me
            Dim address = _rnd.Next(1, 30000)
            _bus.RequestSetAddress(System.Guid.Parse(_guid), address)
            request.Address = address
            Dim response = _bus.Request(request)
            Return response
        End SyncLock
    End Function

End Class
