Imports Bwl.Framework
Imports Bwl.Hardware.SimplSerial
Imports Bwl.SmartHome

Public Class SsValveDevice
    Inherits SsBaseDevice
    Private _valveAction As New SmartStateScheme
    Private _lastvalveState As Boolean

    Public Sub New(bus As SimplSerialBus, logger As Framework.Logger, guid As String, shc As SmartHomeClient)
        MyBase.New(bus, logger, guid, shc)
        _valveAction.ID = "valve"
        _valveAction.Type = SmartStateType.actionOnOff
        _valveAction.DefaultCaption = "Открыт"

        _objectScheme.ClassID = "SsValveDriver"
        _objectScheme.DefaultCaption = "Клапан " + guid
        _objectScheme.DefaultCategory = SmartObjectCategory.generic
        _objectScheme.DefaultGroups = {"Механика"}
        _objectScheme.DefaultShortName = ""
        _objectScheme.States.Add(_valveAction)
        AddHandler _shc.SmartHome.Objects.StateChanged, AddressOf StateChangedHandler
    End Sub

    Private Sub StateChangedHandler(objGuid As String, stateId As String, lastValue As String, currentValue As String, changedBy As ChangedBy)
        If objGuid = Guid And (changedBy = ChangedBy.script Or changedBy = ChangedBy.user) Then
            If stateId = _valveAction.ID Then
                For i = 1 To 5
                    Try
                        SetDeviceState(currentValue)

                        Return
                    Catch ex As Exception
                        _logger.AddWarning("Failed to set device state: " + ex.Message)
                    End Try
                Next
            End If
        End If
    End Sub

    Private Sub SetDeviceState(state As String)
        Dim stateByte As Byte = 0
        If state = "off" Then
            stateByte = &H1
        End If
        Dim response = BusRequestByGuid(New SSRequest(0, 1, {stateByte}))
    End Sub

    Public Overrides Sub PollSimplSerial()
        Dim response = BusRequestByGuid(New SSRequest(0, 2, {}))
        If response.Data.Length = 1 Then
            _lastSuccessRequest = Now
            Dim currentState As Boolean = response.Data(0) <> 0
            If currentState <> _lastvalveState Then
                Try
                    _shc.SmartHome.Objects.SetValue(_guid, _valveAction.ID, If(currentState, "off", "on"), ChangedBy.device)
                    _lastvalveState = currentState
                Catch ex As Exception
                    _logger.AddWarning("Failed to send to server change from device " + _guid)
                End Try
            End If
        End If
    End Sub
End Class

Public Class SsValveDriver
    Implements ISsDriver
    Protected _bus As SimplSerialBus
    Protected _logger As Framework.Logger
    Protected _shc As SmartHomeClient

    Public Sub New(bus As SimplSerialBus, logger As Framework.Logger, shc As SmartHomeClient)
        _bus = bus
        _logger = logger
        _shc = shc
    End Sub

    Public Function IsDeviceSupported(devicename As String) As Boolean Implements ISsDriver.IsDeviceSupported
        Return devicename.Contains("SmartDevice.ValveControl")
    End Function

    Public Function CreateDevice(guid As String) As ISsDevice Implements ISsDriver.CreateDevice
        Dim device As New SsValveDevice(_bus, _logger, guid, _shc)
        Return device
    End Function
End Class