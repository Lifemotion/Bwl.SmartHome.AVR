Imports Bwl
Imports Bwl.Hardware.SimplSerial
Imports Bwl.SmartHome
Imports Bwl.SmartHome.SimplSerial.AvrControl

Public Class SsJalousieDevice
    Inherits SsBaseDevice
    Private _jalousie1action As New SmartStateScheme
    Private _jalousie2action As New SmartStateScheme
    Private _jalousie3action As New SmartStateScheme
    Private _jalousie4action As New SmartStateScheme

    Private _lastSwitchState As Boolean
    Public Sub New(bus As SimplSerialBus, logger As Framework.Logger, guid As String, shc As SmartHomeClient)
        MyBase.New(bus, logger, guid, shc)
        _jalousie1action.ID = "jalousieAction_0"
        _jalousie1action.Type = SmartStateType.actionButton
        _jalousie1action.DefaultCaption = "Закрыть"

        _jalousie2action.ID = "jalousieAction_1"
        _jalousie2action.Type = SmartStateType.actionButton
        _jalousie2action.DefaultCaption = "30%"

        _jalousie3action.ID = "jalousieAction_2"
        _jalousie3action.Type = SmartStateType.actionButton
        _jalousie3action.DefaultCaption = "60%"

        _jalousie4action.ID = "jalousieAction_3"
        _jalousie4action.Type = SmartStateType.actionButton
        _jalousie4action.DefaultCaption = "100%"

        _objectScheme.ClassID = "SsJalousieAction"
        _objectScheme.DefaultCaption = "Жалюзи " + guid
        _objectScheme.DefaultCategory = SmartObjectCategory.generic
        _objectScheme.DefaultGroups = {"Жалюзи"}
        _objectScheme.DefaultShortName = ""
        _objectScheme.States.Add(_jalousie1action)
        _objectScheme.States.Add(_jalousie2action)
        _objectScheme.States.Add(_jalousie3action)
        _objectScheme.States.Add(_jalousie4action)


        AddHandler _shc.SmartHome.Objects.StateChanged, AddressOf StateChangedHandler
    End Sub

    Private Sub StateChangedHandler(objGuid As String, stateId As String, lastValue As String, currentValue As String, changedBy As ChangedBy)
        If objGuid = Guid And (changedBy = ChangedBy.user) Then
            If (stateId.Contains("jalousieAction_")) Then
                Try
                    Dim cmd = Integer.Parse(stateId.Split("_")(1))
                    If cmd = 0 Then
                        BusRequestByGuid(New SSRequest(0, 2, {CByte(0)}))
                    End If
                    If cmd = 1 Then
                        BusRequestByGuid(New SSRequest(0, 2, {CByte(30)}))
                    End If
                    If cmd = 2 Then
                        BusRequestByGuid(New SSRequest(0, 2, {CByte(60)}))
                    End If
                    If cmd = 3 Then
                        BusRequestByGuid(New SSRequest(0, 2, {CByte(100)}))
                    End If
                Catch ex As Exception
                    _logger.AddError(ex.Message)
                End Try
            End If
            _shc.SmartHome.Objects.SetValue(Guid, stateId, "0", ChangedBy.device)
        End If
    End Sub

    Public Overrides Sub PollSimplSerial()

    End Sub
End Class

Public Class SsJalousieDriver
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
        Return devicename.Contains("SmartDevice.Jalousie")
    End Function

    Public Function CreateDevice(guid As String) As ISsDevice Implements ISsDriver.CreateDevice
        Dim device As New SsJalousieDevice(_bus, _logger, guid, _shc)
        Return device
    End Function
End Class
