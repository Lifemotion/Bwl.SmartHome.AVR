Imports System.IO
Imports Bwl
Imports Bwl.Hardware.SimplSerial
Imports Bwl.SmartHome
Imports Bwl.SmartHome.SimplSerial.AvrControl

Public Class SsRemoteControlDevice
    Inherits SsBaseDevice
    Private _temperatureAction As New SmartStateScheme
    Private _humidityAction As New SmartStateScheme
    Private _replayAction As New SmartStateScheme
    Private _saveAction As New SmartStateScheme
    Private _cmdList As List(Of Byte()) = New List(Of Byte())
    Private _lastCapturedCmd As List(Of Byte) = New List(Of Byte)

    Public Sub New(bus As SimplSerialBus, logger As Framework.Logger, guid As String, shc As SmartHomeClient)
        MyBase.New(bus, logger, guid, shc)
        _temperatureAction.ID = "temperature"
        _temperatureAction.Type = SmartStateType.stateString
        _temperatureAction.DefaultCaption = "Температура"
        _humidityAction.ID = "humidity"
        _humidityAction.Type = SmartStateType.stateString
        _humidityAction.DefaultCaption = "Влажность"

        _replayAction.ID = "replay_cmd"
        _replayAction.Type = SmartStateType.actionButton
        _replayAction.DefaultCaption = "Воспроизвести захват"

        _saveAction.ID = "save_cmd"
        _saveAction.Type = SmartStateType.actionButton
        _saveAction.DefaultCaption = "Сохранить захват"

        _objectScheme.ClassID = "SsRemoteControlDriver"
        _objectScheme.DefaultCaption = "ИК модем " + guid
        _objectScheme.DefaultCategory = SmartObjectCategory.generic
        _objectScheme.DefaultGroups = {"Беспровдное"}
        _objectScheme.DefaultShortName = ""
        _objectScheme.States.Add(_temperatureAction)
        _objectScheme.States.Add(_humidityAction)
        _objectScheme.States.Add(_replayAction)
        _objectScheme.States.Add(_saveAction)
        LoadCommands()
        AddHandler _shc.SmartHome.Objects.StateChanged, AddressOf StateChangedHandler
    End Sub

    Private Sub StateChangedHandler(objGuid As String, stateId As String, lastValue As String, currentValue As String, changedBy As ChangedBy)
        If objGuid = Guid And (changedBy = ChangedBy.user) Then
            If (stateId.Contains("IrCmd_")) Then
                Try
                    Dim cmd = Integer.Parse(stateId.Split("_")(1)) - 1
                    If (cmd < _cmdList.Count) Then BusRequestByGuid(New SSRequest(0, 3, _cmdList(cmd)))
                Catch ex As Exception
                    _logger.AddError(ex.Message)
                End Try
            End If
            If stateId = _replayAction.ID Then
                BusRequestByGuid(New SSRequest(0, 3, _lastCapturedCmd.ToArray))
            End If
            If stateId = _saveAction.ID Then
                SaveCommand(_lastCapturedCmd.ToArray)
            End If
            _shc.SmartHome.Objects.SetValue(Guid, stateId, "0", ChangedBy.device)
        End If

    End Sub

    Public Overrides Sub PollSimplSerial()
        Dim response = BusRequestByGuid(New SSRequest(0, 1, {}))
        If response.Data.Length = 9 Then
            Dim _temeratureString = BitConverter.ToSingle(response.Data, 0).ToString + " C"
            Dim _humidityString = BitConverter.ToSingle(response.Data, 4).ToString + "%"
            _shc.SmartHome.Objects.SetValue(_guid, _temperatureAction.ID, _temeratureString, ChangedBy.device)
            _shc.SmartHome.Objects.SetValue(_guid, _humidityAction.ID, _humidityString, ChangedBy.device)
            If (response.Data(8) <> 0) Then
                response = BusRequestByGuid(New SSRequest(0, 2, {}))
                If response.Data.Length > 16 Then
                    _lastCapturedCmd.Clear()
                    _lastCapturedCmd.AddRange(response.Data)
                End If
            End If
        End If
    End Sub

    Private Sub SaveCommand(data As Byte())
        If data.Length > 16 Then
            Using sw As StreamWriter = File.AppendText(Guid + "\commands.txt")
                Dim Str As String = ""
                For Each b In data
                    Str += b.ToString + ":"
                Next
                sw.WriteLine(Str.Substring(0, Str.Length - 1))
            End Using
            _cmdList.Add(data)
            _objectScheme.States.Add(New SmartStateScheme("IrCmd_" + (_cmdList.Count).ToString, SmartStateType.actionButton, "Команда " + (_cmdList.Count).ToString))
        End If
    End Sub

    Private Sub LoadCommands()
        _cmdList.Clear()
        Dim cmdCount = 0
        Try
            Using sr As New StreamReader(Guid + "\commands.txt")
                Dim line As String = sr.ReadLine()
                While (line.Length > 10)
                    Dim list As IList(Of Byte) = New List(Of Byte)
                    Dim elemetns = line.Split(":")
                    For Each element In elemetns
                        list.Add(Byte.Parse(element))
                    Next
                    _cmdList.Add(list.ToArray)
                    cmdCount = cmdCount + 1
                    _objectScheme.States.Add(New SmartStateScheme("IrCmd_" + cmdCount.ToString, SmartStateType.actionButton, "Команда " + cmdCount.ToString))
                    line = sr.ReadLine()
                End While
            End Using
        Catch ex As Exception

        End Try
    End Sub

End Class

Public Class SsRemoteControlDriver
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
        Return devicename.Contains("SmartDevice.RemoteControl")
    End Function

    Public Function CreateDevice(guid As String) As ISsDevice Implements ISsDriver.CreateDevice
        If Not IO.Directory.Exists(guid) Then
            IO.Directory.CreateDirectory(guid)
        End If
        Dim device As New SsRemoteControlDevice(_bus, _logger, guid, _shc)
        Return device
    End Function
End Class
