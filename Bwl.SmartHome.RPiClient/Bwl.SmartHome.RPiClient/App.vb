Imports Bwl.Hardware.SimplSerial
Module App
    Private _ssb As SimplSerialBus

    Sub Main()
        While (System.IO.Ports.SerialPort.GetPortNames().Length < 1)
            Console.WriteLine("Ожидание подключения COM порта...")
            Threading.Thread.Sleep(3000)
        End While

        Console.WriteLine(IO.Ports.SerialPort.GetPortNames()(0))
        _ssb = New SimplSerialBus()
        _ssb.SerialDevice.DeviceAddress = IO.Ports.SerialPort.GetPortNames()(0)
        _ssb.SerialDevice.DeviceSpeed = 9600
        _ssb.Connect()
        Dim req = _ssb.RequestDeviceInfo(0)

        Dim _shw As SHWork = New SHWork(_ssb)
        Console.WriteLine("SmartHomeClient запущен")
        Console.ReadKey()
    End Sub

End Module
