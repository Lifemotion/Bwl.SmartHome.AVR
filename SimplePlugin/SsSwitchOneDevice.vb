Imports Bwl
Imports Bwl.Hardware.SimplSerial
Imports Bwl.SmartHome
Imports Bwl.SmartHome.SimplSerial.AvrControl

Public Class SsSwitchOneDevice
    Inherits SsBaseDevice
    Private _switch1action As New SmartStateScheme
    Private _lastSwitchState As Boolean
    Private _lastSwitchStateUnknown As Boolean = True

    Public Sub New(bus As SimplSerialBus, logger As Framework.Logger, guid As String, shc As SmartHomeClient)
        MyBase.New(bus, logger, guid, shc)
        _switch1action.ID = "Switch1"
        _switch1action.Type = SmartStateType.actionOnOff
        _switch1action.DefaultCaption = "Включен"

        _objectScheme.ClassID = "SsSwitchOneDriver"
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
        Dim response = BusRequestByGuid(1, {124, 45, 67, 251, 0, 0})
        If response.Data.Length = 6 Then
            If response.Data(0) = 12 And response.Data(1) = 79 And response.Data(2) = 36 And response.Data(3) = 129 Then
                _lastSuccessRequest = Now
                Dim externalSwitch1 = response.Data(4)
                Dim internalSwitch1 = response.Data(5)
                Dim resulting = externalSwitch1 <> internalSwitch1
                If (_lastSwitchState = True And state = "off") Or (_lastSwitchState = False And state = "on") Then
                    If internalSwitch1 = 0 Then internalSwitch1 = 1 Else internalSwitch1 = 0
                    Dim response1 = BusRequestByGuid(1, {124, 45, 67, 251, 1, internalSwitch1})
                    If response1.Data.Length = 6 Then
                        'ok
                        _lastSwitchState = True
                    Else
                        Throw New Exception("Bad response 2: " + response1.ToString)
                    End If
                End If
            Else
                Throw New Exception("Bad magic: " + response.ToString)
            End If
        Else
            Throw New Exception("Bad response 1: " + response.ToString)
        End If
    End Sub

    Public Overrides Sub PollSimplSerial()
        Dim response = BusRequestByGuid(1, {124, 45, 67, 251, 0, 0})
        If response.Data.Length = 6 Then
            If response.Data(0) = 12 And response.Data(1) = 79 And response.Data(2) = 36 And response.Data(3) = 129 Then
                _lastSuccessRequest = Now
                Dim externalSwitch1 = response.Data(4)
                Dim internalSwitch1 = response.Data(5)
                Dim resulting = externalSwitch1 <> internalSwitch1
                If _lastSwitchStateUnknown Then
                    _lastSwitchState = resulting
                    _lastSwitchStateUnknown = False
                Else
                    If resulting <> _lastSwitchState Then
                        Try
                            _shc.SmartHome.Objects.SetValue(_guid, _switch1action.ID, If(resulting, "on", "off"), ChangedBy.device)
                            _lastSwitchState = resulting
                        Catch ex As Exception
                            _logger.AddWarning("Failed to send to server change from device " + _guid)
                        End Try
                    End If
                End If
            End If
        End If
    End Sub
End Class

Public Class SsSwitchOneDriver
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
        Return devicename.Contains("SS.SwitchOne")
    End Function

    Public Function CreateDevice(guid As String) As ISsDevice Implements ISsDriver.CreateDevice
        Dim device As New SsSwitchOneDevice(_bus, _logger, guid, _shc)
        Return device
    End Function
End Class
