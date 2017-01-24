Imports Bwl.Hardware.SimplSerial

Public Class SHWork
    Private _client As SmartHomeClient
    Private _sst As SimplSerialBus

    Private _computerObjectScheme As New SmartObjectScheme
    Private _TvStandby As New SmartStateScheme("TvStandby", SmartStateType.actionButton, "Телевизор")
    Private _TvVolumeMinus As New SmartStateScheme("TvVolumeMinus", SmartStateType.actionButton, "Убавить звук")
    Private _TvVolumePlus As New SmartStateScheme("TvVolumePlus", SmartStateType.actionButton, "Прибавить звук")
    Private _TvNextChannel As New SmartStateScheme("TvNextChannel", SmartStateType.actionButton, "Следующий канал")
    Private _TvPreviousChannel As New SmartStateScheme("TvPreviousChannel", SmartStateType.actionButton, "Предыдущий канал")
    Private _TvMute As New SmartStateScheme("TvMute", SmartStateType.actionButton, "Отключить звук")

    Sub New(_bus As SimplSerialBus)
        Me._sst = _bus
        _computerObjectScheme.ClassID = "BwlRPiControlApp"
        _computerObjectScheme.DefaultCaption = "Raspberry Pi " + My.Computer.Name
        _computerObjectScheme.DefaultCategory = SmartObjectCategory.generic
        _computerObjectScheme.DefaultGroups = {"SSerialDevices"}
        _computerObjectScheme.States.Add(_TvStandby)
        _computerObjectScheme.States.Add(_TvVolumeMinus)
        _computerObjectScheme.States.Add(_TvVolumePlus)
        _computerObjectScheme.States.Add(_TvNextChannel)
        _computerObjectScheme.States.Add(_TvPreviousChannel)
        _computerObjectScheme.States.Add(_TvMute)

        _client = New SmartHomeClient()
        AddHandler _client.SmartHome.Objects.StateChanged, AddressOf StateChangedHandler
        AddHandler _client.SendObjectsSchemesTimer, AddressOf SendObjectsTimerHandler
        _client.SendObjectsTimerHandler()

    End Sub

    Private Sub SendObjectsTimerHandler()
        Try
            _client.SmartHome.Objects.SetScheme(_client._guid, _computerObjectScheme)
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
    End Sub

    Private Sub ResetButton(stateID As String)
        Dim th = New Threading.Thread(Sub() Me.ResetButtonExecutor(stateID))
        th.Start()
    End Sub

    Private Sub ResetButtonExecutor(stateID As String)
        Threading.Thread.Sleep(500)
        _client.SmartHome.Objects.SetValue(_client._guid, stateID, "0", ChangedBy.device)
    End Sub

    Private Sub StateChangedHandler(objGuid As String, stateId As String, lastValue As String, currentValue As String, changedBy As ChangedBy)
        If objGuid = _client._guid And changedBy = ChangedBy.user Or changedBy = ChangedBy.script Then
            Dim data() As Byte = {12, 47, 55, 100, 1, 0, 12, 1}
            If stateId = _TvStandby.ID Then
                data(6) = 12
                data(7) = 1
                Dim req = _sst.Request(0, 1, data)
                If (req.Data.Length > 0) Then
                    Console.WriteLine("TV On/Off")
                    ResetButton(stateId)

                End If

            End If

            If stateId = _TvVolumeMinus.ID Then
                data(6) = 42
                data(7) = 0
                Dim req = _sst.Request(0, 1, data)
                If (req.Data.Length > 0) Then
                    Console.WriteLine("TV Vol-")
                    ResetButton(stateId)
                End If
            End If

            If stateId = _TvVolumePlus.ID Then
                data(6) = 26
                data(7) = 0
                Dim req = _sst.Request(0, 1, data)
                If (req.Data.Length > 0) Then
                    Console.WriteLine("TV Vol+")
                    ResetButton(stateId)
                End If
            End If

            If stateId = _TvNextChannel.ID Then
                data(6) = 2
                data(7) = 0
                Dim req = _sst.Request(0, 1, data)
                If (req.Data.Length > 0) Then
                    Console.WriteLine("TV next channel")
                    ResetButton(stateId)
                End If
            End If

            If stateId = _TvPreviousChannel.ID Then
                data(6) = 34
                data(7) = 0
                Dim req = _sst.Request(0, 1, data)
                If (req.Data.Length > 0) Then
                    Console.WriteLine("TV Previous channel")
                    ResetButton(stateId)
                End If
            End If

            If stateId = _TvMute.ID Then
                data(6) = 44
                data(7) = 1
                Dim req = _sst.Request(0, 1, data)
                If (req.Data.Length > 0) Then
                    Console.WriteLine("TV mute")
                    ResetButton(stateId)
                End If
            End If
        End If
    End Sub

End Class
