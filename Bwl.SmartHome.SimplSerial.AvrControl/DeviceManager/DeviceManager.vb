Imports Bwl.Hardware.SimplSerial
Imports System.Reflection
Imports System.IO

Public Class DeviceManager
    Public ReadOnly Property Drivers As New List(Of ISsDriver)
    Public ReadOnly Property Devices As New List(Of ISsDevice)

    Private _pluginFolder As String = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "plugins"
    Private _plugins As List(Of String) = New List(Of String)

    Protected _bus As SimplSerialBus
    Protected _logger As Framework.Logger
    Protected _shc As SmartHomeClient
    Protected _rnd As New Random
    Private _pluginLoaderThread As Threading.Thread

    Public Sub New(bus As SimplSerialBus, logger As Framework.Logger, shc As SmartHomeClient)
        _bus = bus
        _logger = logger
        _shc = shc
    End Sub

    Public Sub RunPluginMonitor(server As String)
        Dim th = New Threading.Thread(AddressOf LoadPlugins)
        th.Start()
        Dim remotePluginFolderMonitor = New PluginLoader(server, _pluginFolder)
        remotePluginFolderMonitor.Start()
    End Sub
    Private Sub LoadPlugins()
        If Directory.Exists(_pluginFolder) = False Then
            Directory.CreateDirectory(_pluginFolder)
        End If
        While True
            Try
                Dim files = Directory.GetFiles(_pluginFolder)
                For Each file In files
                    If _plugins.Contains(file) = False Then
                        Dim asm = Assembly.LoadFrom(file)
                        Dim types = asm.GetExportedTypes()
                        For Each type In types
                            If type.Name.ToLower().Contains("driver") And Not _plugins.Contains(type.Name) Then
                                Dim plugin As ISsDriver = Activator.CreateInstance(type, _bus, _logger, _shc)
                                Drivers.Add(plugin)
                                _plugins.Add(type.Name)
                                Console.WriteLine("Loading plugin: " + type.Name)
                            End If
                        Next
                    End If
                Next
            Catch ex As Exception
            End Try
            Threading.Thread.Sleep(900000)
        End While
    End Sub

    Public Sub SearchDevices()
        Dim guids = _bus.FindDevices()
        _logger.AddDebug("Found on bus: " + guids.Length.ToString)
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
