Public Class MwRegistry
  Const mwRegistryPath = "C:\yPara\mwRegistry\"


  Private Shared Function fp(ByVal path As String, Optional ByVal fileName As String = "")
    fp = path + IIf(path.EndsWith("\"), "", "\") + fileName
  End Function
  Private Shared Function appPath() As String
    Return fp(My.Computer.FileSystem.GetParentPath(Application.ExecutablePath))
  End Function

  Shared Sub mwRegisterSelf()
    Dim exeName As String = IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath)
    IO.Directory.CreateDirectory(mwRegistryPath)
    IO.File.WriteAllText(mwRegistryPath + exeName + ".mwreg", Application.ExecutablePath)
  End Sub

  Shared Function ExePath(ByVal appName As String) As String
    On Error Resume Next
    Dim fileName As String = mwRegistryPath + appName + ".mwreg"
    If IO.File.Exists(fileName) Then Return IO.File.ReadAllText(fileName)
    If IO.File.Exists(appPath() + appName + ".exe") Then Return appPath() + appName + ".exe"
  End Function

End Class
