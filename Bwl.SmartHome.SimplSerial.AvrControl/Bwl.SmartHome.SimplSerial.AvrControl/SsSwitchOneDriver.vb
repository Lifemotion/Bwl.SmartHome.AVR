Imports Bwl.Hardware.SimplSerial

Public Class SsSwitchOneDriver
    Private _bus As SimplSerialBus
    Private _logger As Framework.Logger
    Private _guid As Guid
    Private _shc As SmartHomeClient

    Public Sub New(bus As SimplSerialBus, logger As Framework.Logger, guid As Guid, shc As SmartHomeClient)
        _bus = bus
        _logger = logger
        _guid = guid
        _shc = shc
    End Sub

    Public Sub UpdateServerObjects()

    End Sub

    Public Sub PollSimplSerial()

    End Sub

End Class
