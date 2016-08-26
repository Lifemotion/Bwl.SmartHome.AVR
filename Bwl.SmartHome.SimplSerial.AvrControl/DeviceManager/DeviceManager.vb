Imports Bwl.Hardware.SimplSerial

Public Class DeviceManager
    Public ReadOnly Property Drivers As New List(Of ISsDriver)
    Public ReadOnly Property Devices As New List(Of ISsDevice)

    Protected _bus As SimplSerialBus
    Protected _logger As Framework.Logger
    Protected _shc As SmartHomeClient
    Protected _rnd As New Random

    Public Sub New(bus As SimplSerialBus, logger As Framework.Logger, shc As SmartHomeClient)
        _bus = bus
        _logger = logger
        _shc = shc
    End Sub

    Public Sub SearchDevices()
        Dim guids = _bus.FindDevices()
        For Each guid In guids
            Dim found As Boolean = False
            For Each driver In Devices
                If driver.Guid = guid.ToString Then
                    found = True
                    Exit For
                End If
            Next
            If Not found Then
                Dim address = _rnd.Next(1, 30000)
                _bus.RequestSetAddress(guid, address)
                Dim devinfo = _bus.RequestDeviceInfo(address)
                If devinfo.DeviceName > "" Then
                    Dim supported As Boolean = False
                    For Each df In Drivers
                        If df.IsDeviceSupported(devinfo.DeviceName) Then
                            Dim newdew = df.CreateDevice(guid.ToString)
                            _logger.AddMessage("Found new device " + guid.ToString + ", created with driver " + df.GetType.Name)
                            supported = True
                            Me.Devices.Add(newdew)
                        End If
                    Next
                    If Not supported Then
                        _logger.AddMessage("Found new device " + guid.ToString + ", no driver found")
                    End If
                End If
            End If
        Next
    End Sub

    Public Sub PollDevices()
        For Each driver In Devices
            Try
                driver.PollSimplSerial()
            Catch ex As Exception
                _logger.AddWarning(ex.Message)
            End Try
        Next
    End Sub

    Public Sub UpdateObjects()
        For Each driver In Devices
            Try
                driver.UpdateServerObjects()
            Catch ex As Exception
                _logger.AddWarning(ex.Message)
            End Try
        Next
    End Sub

End Class
