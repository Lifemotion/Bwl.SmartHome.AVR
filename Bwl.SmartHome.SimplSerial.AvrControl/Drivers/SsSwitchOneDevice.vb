Imports Bwl.Hardware.SimplSerial

Public Class SsSwitchOneDevice
    Inherits SsBaseDevice
    Private _switch1action As New SmartState

    Public Sub New(bus As SimplSerialBus, logger As Framework.Logger, guid As String, shc As SmartHomeClient)
        MyBase.New(bus, logger, guid, shc)
        _switch1action.ID = "Switch1"
        _switch1action.Type = SmartStateType.actionOnOff
        _switch1action.Value = "off"
        _switch1action.Caption = "Состояние"

        _serverObject.Config.Caption = "Выключатель " + guid
        _serverObject.Config.Category = SmartObjectCategory.generic
        _serverObject.Config.Groups = {"Выключатели"}
        _serverObject.Config.ShortName = "Выключатель " + _rnd.Next.ToString
        _serverObject.States.Add(_switch1action)
    End Sub

    Public Overrides Sub PollSimplSerial()
        Dim response = BusRequestByGuid(1, {124, 45, 67, 251, 0, 0})
        If response.Data.Length = 6 Then
            If response.Data(0) = 12 And response.Data(1) = 79 And response.Data(2) = 36 And response.Data(3) = 129 Then
                _lastSuccessRequest = Now
                Dim externalSwitch1 = response.Data(4)
                Dim internalSwitch1 = response.Data(5)
                Dim resulting = externalSwitch1 <> internalSwitch1
                For Each state In _serverObject.States
                    If state.ID = _switch1action.ID Then
                        If state.ValueChanged Then
                            If (resulting = True And state.Value = "off") Or (resulting = False And state.Value = "on") Then
                                If internalSwitch1 = 0 Then internalSwitch1 = 1 Else internalSwitch1 = 0
                                Dim response1 = BusRequestByGuid(1, {124, 45, 67, 251, 1, internalSwitch1})
                                If response1.Data.Length = 6 Then
                                    state.ValueChanged = False
                                    _shc.SmartHome.Objects.SetObject(_serverObject, SmartObjectSetMask.statesAll)
                                End If
                            End If
                        Else
                            If (resulting = True And state.Value = "off") Or (resulting = False And state.Value = "on") Then
                                If resulting Then state.Value = "on" Else state.Value = "off"
                                state.ValueChanged = False
                                _shc.SmartHome.Objects.SetObject(_serverObject, SmartObjectSetMask.statesAll)
                            End If
                        End If
                    End If
                Next
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
