Imports Bwl
Imports Bwl.Hardware.SimplSerial
Imports Bwl.SmartHome
Imports Bwl.SmartHome.SimplSerial.AvrControl

Public Class SsMultiSwitchDevice
    Inherits SsBaseDevice
    Private _switchAllAction As New SmartStateScheme
    Private _switch1Action As New SmartStateScheme
    Private _switch2Action As New SmartStateScheme
    Private _switch3Action As New SmartStateScheme
    Private _switch4Action As New SmartStateScheme
    Private _switch5Action As New SmartStateScheme
    Private _switch6Action As New SmartStateScheme
    Private _switch7Action As New SmartStateScheme
    Private _switch8Action As New SmartStateScheme

    Private _lastSwitchState1 As Byte
    Private _lastSwitchState2 As Byte
    Private _lastSwitchStateUnknown As Boolean = True

    Public Sub New(bus As SimplSerialBus, logger As Framework.Logger, guid As String, shc As SmartHomeClient)
        MyBase.New(bus, logger, guid, shc)
        _objectScheme.ClassID = "SsMultiSwitchDriver"
        _objectScheme.DefaultCaption = "Выключатель " + guid
        _objectScheme.DefaultCategory = SmartObjectCategory.generic
        _objectScheme.DefaultGroups = {"Выключатели"}
        _objectScheme.DefaultShortName = ""

        _switchAllAction.ID = "SwitchAll"
        _switchAllAction.Type = SmartStateType.actionOnOff
        _switchAllAction.DefaultCaption = "Все"
        _objectScheme.States.Add(_switchAllAction)

        _switch1Action.ID = "Switch1"
        _switch1Action.Type = SmartStateType.actionOnOff
        _switch1Action.DefaultCaption = "Лампа 1"
        _objectScheme.States.Add(_switch1Action)
        _switch2Action.ID = "Switch2"
        _switch2Action.Type = SmartStateType.actionOnOff
        _switch2Action.DefaultCaption = "Лампа 2"
        _objectScheme.States.Add(_switch2Action)
        _switch3Action.ID = "Switch3"
        _switch3Action.Type = SmartStateType.actionOnOff
        _switch3Action.DefaultCaption = "Лампа 3"
        _objectScheme.States.Add(_switch3Action)
        _switch4Action.ID = "Switch4"
        _switch4Action.Type = SmartStateType.actionOnOff
        _switch4Action.DefaultCaption = "Лампа 4"
        _objectScheme.States.Add(_switch4Action)
        _switch5Action.ID = "Switch5"
        _switch5Action.Type = SmartStateType.actionOnOff
        _switch5Action.DefaultCaption = "Лампа 5"
        _objectScheme.States.Add(_switch5Action)
        _switch6Action.ID = "Switch6"
        _switch6Action.Type = SmartStateType.actionOnOff
        _switch6Action.DefaultCaption = "Лампа 6"
        _objectScheme.States.Add(_switch6Action)
        _switch7Action.ID = "Switch7"
        _switch7Action.Type = SmartStateType.actionOnOff
        _switch7Action.DefaultCaption = "Лампа 7"
        _objectScheme.States.Add(_switch7Action)
        _switch8Action.ID = "Switch8"
        _switch8Action.Type = SmartStateType.actionOnOff
        _switch8Action.DefaultCaption = "Лампа 8"
        _objectScheme.States.Add(_switch8Action)

        AddHandler _shc.SmartHome.Objects.StateChanged, AddressOf StateChangedHandler
    End Sub

    Private Sub StateChangedHandler(objGuid As String, stateId As String, lastValue As String, currentValue As String, changedBy As ChangedBy)
        If objGuid = Guid And (changedBy = ChangedBy.script Or changedBy = ChangedBy.user) Then
            If stateId = _switchAllAction.ID Then SetDeviceState(currentValue, 255, 0)
            If stateId = _switch1Action.ID Then SetDeviceState(currentValue, 1, 0)
            If stateId = _switch2Action.ID Then SetDeviceState(currentValue, 2, 0)
            If stateId = _switch3Action.ID Then SetDeviceState(currentValue, 4, 0)
            If stateId = _switch4Action.ID Then SetDeviceState(currentValue, 8, 0)
            If stateId = _switch5Action.ID Then SetDeviceState(currentValue, 16, 0)
            If stateId = _switch6Action.ID Then SetDeviceState(currentValue, 32, 0)
            If stateId = _switch7Action.ID Then SetDeviceState(currentValue, 64, 0)
            If stateId = _switch8Action.ID Then SetDeviceState(currentValue, 128, 0)
        End If
    End Sub

    Private Sub SetDeviceState(state As String, r1 As Byte, r2 As Byte)
        For i = 1 To 5
            Try
                Dim relays1, relays2 As Byte
                ' If (_lastSwitchState1 > 0 And state = "off") Or (_lastSwitchState1 = 0 And state = "on") Then
                If state = "on" Then
                    relays1 = _lastSwitchState1 Or r1
                    relays2 = _lastSwitchState2 Or r1
                Else
                    relays1 = _lastSwitchState1 And Not r1
                    relays2 = _lastSwitchState1 And Not r2
                End If
                Dim response1 = BusRequestByGuid(2, {57, 129, 33, 221, relays1, relays2})
                    If response1.Data.Length = 4 Then
                        'ok
                        _lastSwitchState1 = relays1
                        _lastSwitchState1 = relays2
                    Else
                        Throw New Exception("Bad response 2: " + response1.ToString)
                    End If
                ' End If
                Return
            Catch ex As Exception
                _logger.AddWarning("Failed to set device state: " + ex.Message)
            End Try
        Next
    End Sub

    Public Overrides Sub PollSimplSerial()
        Dim response = BusRequestByGuid(1, {57, 129, 33, 221})
        If response.Data.Length = 7 Then
            If response.Data(0) = 78 And response.Data(1) = 32 And response.Data(2) = 1 And response.Data(3) = 227 Then
                _lastSuccessRequest = Now
                Dim relays1 = response.Data(4)
                Dim relays2 = response.Data(5)
                Dim buttons = response.Data(6)
                If _lastSwitchStateUnknown Or relays1 <> _lastSwitchState1 Then
                    Try
                        _shc.SmartHome.Objects.SetValue(_guid, _switchAllAction.ID, If(relays1 = 255, "on", "off"), ChangedBy.device)
                        '_shc.SmartHome.Objects.SetValue(_guid, _switch1Action.ID, If((relays1 And 1) > 0, "on", "off"), ChangedBy.device)
                        '_shc.SmartHome.Objects.SetValue(_guid, _switch2Action.ID, If((relays1 And 2) > 0, "on", "off"), ChangedBy.device)
                        '_shc.SmartHome.Objects.SetValue(_guid, _switch3Action.ID, If((relays1 And 4) > 0, "on", "off"), ChangedBy.device)
                        '_shc.SmartHome.Objects.SetValue(_guid, _switch4Action.ID, If((relays1 And 8) > 0, "on", "off"), ChangedBy.device)
                        '_shc.SmartHome.Objects.SetValue(_guid, _switch5Action.ID, If((relays1 And 16) > 0, "on", "off"), ChangedBy.device)
                        '_shc.SmartHome.Objects.SetValue(_guid, _switch6Action.ID, If((relays1 And 32) > 0, "on", "off"), ChangedBy.device)
                        '_shc.SmartHome.Objects.SetValue(_guid, _switch7Action.ID, If((relays1 And 64) > 0, "on", "off"), ChangedBy.device)
                        '_shc.SmartHome.Objects.SetValue(_guid, _switch8Action.ID, If((relays1 And 128) > 0, "on", "off"), ChangedBy.device)
                        _lastSwitchState1 = relays1
                        _lastSwitchState2 = relays2
                        _lastSwitchStateUnknown = False
                    Catch ex As Exception
                        _logger.AddWarning("Failed to send to server change from device " + _guid)
                    End Try
                End If
            End If
        End If
    End Sub

End Class

Public Class SsMultiSwitchDriver
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
        Return devicename.Contains("BwlSH.SS.MultiSwitch")
    End Function

    Public Function CreateDevice(guid As String) As ISsDevice Implements ISsDriver.CreateDevice
        Dim device As New SsMultiSwitchDevice(_bus, _logger, guid, _shc)
        Return device
    End Function
End Class
