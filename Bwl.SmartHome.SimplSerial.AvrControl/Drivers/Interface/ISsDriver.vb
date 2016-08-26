Imports Bwl.SmartHome.SimplSerial.AvrControl

Public Interface ISsDriver
    Function CreateDevice(guid As String) As ISsDevice
    Function IsDeviceSupported(devicename As String) As Boolean
End Interface
