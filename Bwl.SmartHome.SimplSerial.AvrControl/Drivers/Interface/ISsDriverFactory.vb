Imports Bwl.SmartHome.SimplSerial.AvrControl

Public Interface ISsDriverFactory
    Function CreateDevice(guid As String) As ISsDriver
    Function IsDeviceSupported(devicename As String) As Boolean
End Interface
