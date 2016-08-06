Public Interface ISsDriver
    ReadOnly Property Guid As String
    Sub PollSimplSerial()
    Sub UpdateServerObjects()
End Interface
