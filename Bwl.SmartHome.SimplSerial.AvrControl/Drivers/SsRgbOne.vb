Imports Bwl.Hardware.SimplSerial

Public Class SsRgbOne
    Inherits SsBaseDevice
    Public Class RGB
        Public Property R As Integer
        Public Property G As Integer
        Public Property B As Integer
        Public Property ColorNumber As Integer
        Public Sub NextColor()
            ColorNumber += 1
            Select Case ColorNumber
                Case 0 : R = 10 : G = 10 : B = 10
                Case 1 : R = 100 : G = 100 : B = 100
                Case 2 : R = 250 : G = 250 : B = 250
                Case 3 : R = 250 : G = 0 : B = 0
                Case 4 : R = 250 : G = 255 : B = 0
                Case 5 : R = 0 : G = 255 : B = 0
                Case 6 : R = 0 : G = 255 : B = 255
                Case 7 : R = 0 : G = 0 : B = 255
                Case 8 : R = 255 : G = 0 : B = 255
                Case Else : ColorNumber = -1 : NextColor()
            End Select
        End Sub
    End Class

    Private _rgb1onoff As New SmartStateScheme
    Private _rgb2onoff As New SmartStateScheme
    Private _rgb1color As New SmartStateScheme
    Private _rgb2color As New SmartStateScheme

    Private _rgb1 As New RGB With {.R = 100, .G = 100, .B = 100}
    Private _rgb1on As Boolean
    Private _rgb2 As New RGB With {.R = 100, .G = 100, .B = 100}
    Private _rgb2on As Boolean

    Public Sub New(bus As SimplSerialBus, logger As Framework.Logger, guid As String, shc As SmartHomeClient)
        MyBase.New(bus, logger, guid, shc)
        _objectScheme.ClassID = "SsRgbOneDriver"
        _objectScheme.DefaultCaption = "RGB " + guid
        _objectScheme.DefaultCategory = SmartObjectCategory.generic
        _objectScheme.DefaultGroups = {"Лампы"}
        _objectScheme.DefaultShortName = ""

        _rgb1onoff.ID = "RGB1Switch"
        _rgb1onoff.Type = SmartStateType.actionOnOff
        _rgb1onoff.DefaultCaption = "RGB 1"
        _objectScheme.States.Add(_rgb1onoff)
        _rgb1color.ID = "RGB1Color"
        _rgb1color.Type = SmartStateType.actionOnOff
        _rgb1color.DefaultCaption = "Сменить цвет 1"
        _objectScheme.States.Add(_rgb1color)

        _rgb2onoff.ID = "RGB2Switch"
        _rgb2onoff.Type = SmartStateType.actionOnOff
        _rgb2onoff.DefaultCaption = "RGB 2"
        _objectScheme.States.Add(_rgb2onoff)
        _rgb2color.ID = "RGB2Color"
        _rgb2color.Type = SmartStateType.actionOnOff
        _rgb2color.DefaultCaption = "Сменить цвет 2"
        _objectScheme.States.Add(_rgb2color)

        AddHandler _shc.SmartHome.Objects.StateChanged, AddressOf StateChangedHandler
    End Sub

    Private Sub StateChangedHandler(objGuid As String, stateId As String, lastValue As String, currentValue As String, changedBy As ChangedBy)
        If objGuid = Guid And (changedBy = ChangedBy.script Or changedBy = ChangedBy.user) Then
            If stateId = _rgb1onoff.ID Then
                _rgb1on = currentValue = "on"
                SetDeviceState(If(_rgb1on, _rgb1, New RGB), If(_rgb2on, _rgb2, New RGB))
                '   _shc.SmartHome.Objects.SetValue(_guid, _rgb1color.ID, "-", ChangedBy.device)
            End If
            If stateId = _rgb1color.ID Then
                _rgb1.NextColor()
                SetDeviceState(If(_rgb1on, _rgb1, New RGB), If(_rgb2on, _rgb2, New RGB))
                '   _shc.SmartHome.Objects.SetValue(_guid, _rgb1color.ID, "-", ChangedBy.device)
            End If
            If stateId = _rgb1onoff.ID Then
                _rgb2on = currentValue = "on"
                SetDeviceState(If(_rgb1on, _rgb1, New RGB), If(_rgb2on, _rgb2, New RGB))
            End If
            If stateId = _rgb2color.ID Then
                _rgb2.NextColor()
                SetDeviceState(If(_rgb1on, _rgb1, New RGB), If(_rgb2on, _rgb2, New RGB))
            End If
        End If
    End Sub

    Private Sub SetDeviceState(rgb1 As RGB, rgb2 As RGB)
        For i = 1 To 5
            Try
                Dim response1 = BusRequestByGuid(1, {52, 135, 17, 245, rgb1.R, rgb1.G, rgb1.B, rgb2.R, rgb2.G, rgb2.B})
                If response1.Data.Length = 4 Then
                    'ok
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
        Return
        Dim response = BusRequestByGuid(2, {52, 135, 17, 245})
        If response.Data.Length = 12 Then
            If response.Data(0) = 80 And response.Data(1) = 36 And response.Data(2) = 2 And response.Data(3) = 224 Then
                _lastSuccessRequest = Now
                Dim buttons = response.Data(10)

                '  If _lastSwitchStateUnknown Or relays1 <> _lastSwitchState1 Then
                Try
                    '   _shc.SmartHome.Objects.SetValue(_guid, _switchAllAction.ID, If(relays1 = 255, "on", "off"), ChangedBy.device)
                    '_shc.SmartHome.Objects.SetValue(_guid, _switch1Action.ID, If((relays1 And 1) > 0, "on", "off"), ChangedBy.device)
                    '_shc.SmartHome.Objects.SetValue(_guid, _switch2Action.ID, If((relays1 And 2) > 0, "on", "off"), ChangedBy.device)
                    '_shc.SmartHome.Objects.SetValue(_guid, _switch3Action.ID, If((relays1 And 4) > 0, "on", "off"), ChangedBy.device)
                    '_shc.SmartHome.Objects.SetValue(_guid, _switch4Action.ID, If((relays1 And 8) > 0, "on", "off"), ChangedBy.device)
                    '_shc.SmartHome.Objects.SetValue(_guid, _switch5Action.ID, If((relays1 And 16) > 0, "on", "off"), ChangedBy.device)
                    '_shc.SmartHome.Objects.SetValue(_guid, _switch6Action.ID, If((relays1 And 32) > 0, "on", "off"), ChangedBy.device)
                    '_shc.SmartHome.Objects.SetValue(_guid, _switch7Action.ID, If((relays1 And 64) > 0, "on", "off"), ChangedBy.device)
                    '_shc.SmartHome.Objects.SetValue(_guid, _switch8Action.ID, If((relays1 And 128) > 0, "on", "off"), ChangedBy.device)
                    '   _lastSwitchState1 = relays1
                    '_lastSwitchState2 = relays2
                    '_lastSwitchStateUnknown = False
                Catch ex As Exception
                    _logger.AddWarning("Failed to send to server change from device " + _guid)
                End Try
                '  End If
            End If
        End If
    End Sub

End Class

Public Class SsRgbOneDriver
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
        Return devicename.Contains("BwlSH.SS.RGB")
    End Function

    Public Function CreateDevice(guid As String) As ISsDevice Implements ISsDriver.CreateDevice
        Dim device As New SsRgbOne(_bus, _logger, guid, _shc)
        Return device
    End Function
End Class
