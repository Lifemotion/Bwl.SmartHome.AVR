Imports Bwl
Imports Bwl.Hardware.SimplSerial
Imports Bwl.SmartHome
Imports Bwl.SmartHome.SimplSerial.AvrControl

Public Class SsEasyLeak
    Inherits SsBaseDevice
    Private _valveAction As New SmartStateScheme
    Private _leakState As New SmartStateScheme
    Private _lastValveState As Boolean
    Private _lastSensorState As Boolean

    Public Sub New(bus As SimplSerialBus, logger As Framework.Logger, guid As String, shc As SmartHomeClient)
        MyBase.New(bus, logger, guid, shc)
        _valveAction.ID = "valve"
        _valveAction.Type = SmartStateType.actionOnOff
        _valveAction.DefaultCaption = "Открыт"
        _leakState.ID = "leak"
        _leakState.Type = SmartStateType.stateYesNo
        _leakState.DefaultCaption = "Протечка"
        _objectScheme.ClassID = "SsEasyLeakDriver"
        _objectScheme.DefaultCaption = "EasyLeak " + guid
        _objectScheme.DefaultCategory = SmartObjectCategory.generic
        _objectScheme.DefaultGroups = {"Механика"}
        _objectScheme.DefaultShortName = ""
        _objectScheme.States.Add(_valveAction)
        _objectScheme.States.Add(_leakState)
        AddHandler _shc.SmartHome.Objects.StateChanged, AddressOf StateChangedHandler
    End Sub

    Private Sub StateChangedHandler(objGuid As String, stateId As String, lastValue As String, currentValue As String, changedBy As ChangedBy)
        If objGuid = Guid And (changedBy = ChangedBy.script Or changedBy = ChangedBy.user) Then
            If stateId = _valveAction.ID And changedBy = ChangedBy.user Then
                If currentValue = "on" Then
                    BusRequestByGuid(New SSRequest(0, 2, {0}))
                End If
                If currentValue = "off" Then
                    If _lastSensorState = False Then BusRequestByGuid(New SSRequest(0, 2, {1}))
                End If
            End If
        End If
    End Sub

    Public Overrides Sub PollSimplSerial()
        Dim response = BusRequestByGuid(New SSRequest(0, 1, {}))
        If response.Data.Length = 2 Then
            _lastSuccessRequest = Now
            Dim currentSensorState As Boolean = response.Data(0) <> 0
            Dim currentValveState As Boolean = response.Data(1) = 0
            If currentValveState <> _lastValveState Or currentSensorState <> _lastSensorState Then
                Try
                    _shc.SmartHome.Objects.SetValue(_guid, _valveAction.ID, If(currentValveState, "on", "off"), ChangedBy.device)
                    _shc.SmartHome.Objects.SetValue(_guid, _leakState.ID, If(currentSensorState, "yes", "on"), ChangedBy.device)
                    _lastValveState = currentValveState
                    _lastSensorState = currentSensorState
                Catch ex As Exception
                    _logger.AddWarning("Failed to send to server change from device " + _guid)
                End Try
            End If
        End If
    End Sub
End Class

Public Class SsEasyLeakDriver
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
        Return devicename.Contains("EasyLeak.Controller")
    End Function

    Public Function CreateDevice(guid As String) As ISsDevice Implements ISsDriver.CreateDevice
        Dim device As New SsEasyLeak(_bus, _logger, guid, _shc)
        Return device
    End Function
End Class