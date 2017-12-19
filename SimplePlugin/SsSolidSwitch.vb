Imports Bwl
Imports Bwl.Hardware.SimplSerial
Imports Bwl.SmartHome
Imports Bwl.SmartHome.SimplSerial.AvrControl

Public Class SsSolidSwitch
    Inherits SsBaseDevice
    Private _switch1action As New SmartStateScheme
    Private _lastSwitchState As Boolean
    Public Sub New(bus As SimplSerialBus, logger As Framework.Logger, guid As String, shc As SmartHomeClient)
        MyBase.New(bus, logger, guid, shc)
        _switch1action.ID = "Switch1"
        _switch1action.Type = SmartStateType.actionOnOff
        _switch1action.DefaultCaption = "Включен"
        _objectScheme.ClassID = "SsSolidSwitch"
        _objectScheme.DefaultCaption = "Выключатель " + guid
        _objectScheme.DefaultCategory = SmartObjectCategory.generic
        _objectScheme.DefaultGroups = {"Выключатели"}
        _objectScheme.DefaultShortName = ""
        _objectScheme.States.Add(_switch1action)

        AddHandler _shc.SmartHome.Objects.StateChanged, AddressOf StateChangedHandler
    End Sub

    Private Sub StateChangedHandler(objGuid As String, stateId As String, lastValue As String, currentValue As String, changedBy As ChangedBy)
        If objGuid = Guid And (changedBy = ChangedBy.script Or changedBy = ChangedBy.user) Then
            If stateId = _switch1action.ID Then
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
        Dim response = BusRequestByGuid(1, {If(state = "on", 1, 0)})
        If response.ResponseState = ResponseState.ok Then
            _lastSwitchState = If(state = "on", True, False)
            Dim str = "Switch set"
            For Each b In response.Data
                str += " " + b.ToString
            Next
            _logger.AddMessage(str)
        Else
        End If
    End Sub

    Public Overrides Sub PollSimplSerial()
        Dim response = BusRequestByGuid(2, {})
        _lastSuccessRequest = Now
        Dim resulting As Boolean = response.Data(0) = 1
        If _lastSwitchState <> resulting Then
            Try
                _shc.SmartHome.Objects.SetValue(_guid, _switch1action.ID, If(resulting, "on", "off"), ChangedBy.device)
                _lastSwitchState = resulting
            Catch ex As Exception
                _logger.AddWarning("Failed to send to server change from device " + _guid)
            End Try
        End If
    End Sub
End Class

Public Class SsSolidSwitchDriver
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
        Return devicename.Contains("SmartDevice.Switch")
    End Function

    Public Function CreateDevice(guid As String) As ISsDevice Implements ISsDriver.CreateDevice
        Dim device As New SsSolidSwitch(_bus, _logger, guid, _shc)
        Return device
    End Function
End Class
