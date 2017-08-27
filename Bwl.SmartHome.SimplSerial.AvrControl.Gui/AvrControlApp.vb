Imports Bwl.Framework
Imports Bwl.Hardware.SimplSerial
Imports Microsoft.Win32

Public Class AvrControlApp
    Inherits SmartHomeClientBase
    Private _bus As New SimplSerialBus
    Private _portSetting As New StringSetting(_storage, "BusPort", "")
    Private _deviceManager As New DeviceManager(_bus, _logger, _client)

    Private Sub TestApp_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _deviceManager.Drivers.Add(New SsSwitchOneDriver(_bus, _logger, _client))
        _deviceManager.Drivers.Add(New SsMultiSwitchDriver(_bus, _logger, _client))
        _deviceManager.Drivers.Add(New SsRemoteControlDriver(_bus, _logger, _client))
        _deviceManager.Drivers.Add(New SsValveDriver(_bus, _logger, _client))
        _deviceManager.Drivers.Add(New SsRgbOneDriver(_bus, _logger, _client))
        For Each df In _deviceManager.Drivers
            lbDrivers.Items.Add(df.GetType.Name)
        Next
        Text += " " + Application.ProductVersion
        niTray.Text = "BWL SH: AvrControl"
        niTray.Icon = Icon
#If Not DEBUG Then
        Dim invisible As New Threading.Thread(Sub() Me.Invoke(Sub() Hide()))
        invisible.Start()
#End If
    End Sub

    Private Sub tPoll_Tick(sender As Object, e As EventArgs) Handles tPoll.Tick
        If _bus.IsConnected = False And _portSetting.Value > "" Then
            Try
                _bus.SerialDevice.DeviceAddress = _portSetting.Value
                _bus.SerialDevice.DeviceSpeed = 9600
                _bus.Connect()
                _logger.AddMessage("Bus Connected on port " + _portSetting.Value)
                _deviceManager.SearchDevices()
                _deviceManager.SearchDevices()
            Catch ex As Exception
            End Try
        End If

        If _bus.IsConnected Then
            _deviceManager.PollDevices()
        End If
    End Sub

    Private Sub bFindDevices_Click(sender As Object, e As EventArgs) Handles bFindDevices.Click
        Try
            _deviceManager.SearchDevices()
            _deviceManager.SearchDevices()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub tStates_Tick(sender As Object, e As EventArgs) Handles tStates.Tick
        lbDevices.Items.Clear()
        For Each drives In _deviceManager.Devices
            lbDevices.Items.Add(drives.Guid + " " + drives.GetType.Name)
        Next
    End Sub

    Private Sub tUpdateObjects_Tick(sender As Object, e As EventArgs) Handles tUpdateObjects.Tick
        _deviceManager.UpdateObjects
    End Sub

    Private Sub tFind_Tick(sender As Object, e As EventArgs) Handles tFind.Tick
        Try
            _deviceManager.SearchDevices()
        Catch ex As Exception
        End Try
    End Sub

End Class
