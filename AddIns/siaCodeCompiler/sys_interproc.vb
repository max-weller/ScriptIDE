﻿Imports System.Runtime.InteropServices
Imports System.Text
'Imports IfacesEnumsStructsClasses
Imports System.Windows.Forms
Imports ScriptIDE.Core.WindowsUtilities


Public Class sys_interproc
  Inherits NativeWindow

  Event InitCommandDefinition(ByVal e As commandDefEventArgs)

  Event Message(ByVal source As String, ByVal cmdString As String, ByVal para As String)
  Event DataRequest(ByVal source As String, ByVal cmdString As String, ByVal para As String, ByRef returnValue As String)

  Dim winTitle As String
  Const mSplit = "|²^³|"

  Dim hgcSource As GCHandle
  Dim responseTick As Long = -1
  Dim responseData As String = ""
  Dim responseCmd As String = ""
  Dim m_interprocWins As New List(Of String)

  Dim commandDef As New Dictionary(Of String, String)

  Enum PDEF
    Typ
    winTitle
    cmd
    tick
    para
  End Enum

  ReadOnly Property AllInterprocWindows() As Collections.ObjectModel.ReadOnlyCollection(Of String)
    Get
      Return m_interprocWins.AsReadOnly
    End Get
  End Property

  Sub trace(ByVal ParamArray data() As String)
  End Sub

  Class commandDefEventArgs
    Private p_commandDef As Dictionary(Of String, String)
    Sub Add(ByVal cmdType As String, ByVal cmdString As String, ByVal paraDef As String, ByVal helpString As String)
      p_commandDef.Add(cmdType.Trim.ToUpper + ":" + cmdString.ToUpper, cmdType.Trim.ToUpper + "|##|" + cmdString + "|##|" + paraDef + "|##|" + helpString)
    End Sub
    Public Sub New(ByRef commandDefDictionary As Dictionary(Of String, String))
      p_commandDef = commandDefDictionary
    End Sub
  End Class

  <DebuggerStepThrough()> _
  Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
    MyBase.WndProc(m)
    Dim myMsg As Integer = m.Msg
    If m.Msg = WindowsMessages.WM_COPYDATA Then
      Dim CD As New COPYDATASTRUCT
      CD = Marshal.PtrToStructure(m.LParam, CD.GetType)

      Dim strDest As String = Space(CD.cdData / 2)

      Dim hgcDest As GCHandle = _
      GCHandle.Alloc(strDest, GCHandleType.Pinned)

      Call InterprocApi.CopyMemory(hgcDest.AddrOfPinnedObject.ToInt32, CD.lpData, CD.cdData)
      hgcDest.Free()

      Dim parts() As String = Split(strDest, mSplit)
      Dim cmdName = parts(PDEF.cmd).ToUpper
      Select Case parts(0)
        Case "DGET "
          If cmdName = "GETCOMMANDSPEC" Then
            Dim TEMP(commandDef.Count - 1) As String
            commandDef.Values.CopyTo(TEMP, 0)
            Dim cmds = Join(TEMP, vbNewLine)
            SendDataTitle(parts(PDEF.winTitle), "DRESP", cmdName, cmds, parts(PDEF.tick))
            Exit Sub
          End If
          If commandDef.ContainsKey("QUERY:" + cmdName) = False Then
            TT.Write("sys_interproc - ERROR: unknown query: " + strDest)
            SendDataTitle(parts(PDEF.winTitle), "DRESP", cmdName, "ERR: unknown command", parts(PDEF.tick))
          Else
            Dim reqData As String = ""
            RaiseEvent DataRequest(parts(PDEF.winTitle), cmdName, parts(PDEF.para), reqData)
            SendDataTitle(parts(PDEF.winTitle), "DRESP", cmdName, reqData, parts(PDEF.tick))
          End If

        Case "CMD  "
          If commandDef.ContainsKey("CMD:" + cmdName) = False Then
            TT.Write("sys_interproc - ERROR: unknown command: " + strDest)
          Else
            RaiseEvent Message(parts(PDEF.winTitle), cmdName, parts(PDEF.para))
          End If
        Case "DRESP"
          responseData = parts(PDEF.para)
          responseCmd = cmdName
          responseTick = parts(PDEF.tick)

      End Select
    End If

  End Sub

  Public Sub New(ByVal titleName As String)
    winTitle = titleName.ToLower

    Dim cp As New CreateParams()
    cp.Caption = "sys_interproc@" + winTitle + ""
    Me.CreateHandle(cp)

    refreshInterprocWinList()
    Dim tmr As New Timer()
    tmr.Interval = 77
    AddHandler tmr.Tick, AddressOf callInitCommandDefinition
    tmr.Start()
  End Sub

  Sub callInitCommandDefinition(ByVal d As Object, ByVal d2 As Object)
    Dim eventArgs As New commandDefEventArgs(commandDef)
    RaiseEvent InitCommandDefinition(eventArgs)
    d.stop()
  End Sub

  Function getWindow(ByVal titleName As String) As IntPtr
    Dim hwnd As IntPtr
    Dim searchFor As String = "sys_interproc@" + titleName + ""
    hwnd = InterprocApi.FindWindowByCaption(IntPtr.Zero, searchFor)
    Return hwnd
  End Function

  Private Shared Sub idle(Optional ByVal sleepTime As Integer = 10)
    System.Threading.Thread.Sleep(sleepTime)
    Application.DoEvents()
  End Sub

  Function FetchData(ByVal titleName As String, ByVal cmdString As String, ByVal para As String, Optional ByVal timeout As Integer = 5000) As String
    Return GetData(titleName, cmdString, para, timeout)
  End Function
  Function GetData(ByVal titleName As String, ByVal cmdString As String, ByVal para As String, Optional ByVal timeout As Integer = 5000) As String
    Dim ok As Boolean
    Dim myticks As Long = Now.Ticks
    cmdString = cmdString.ToUpper()
    ok = SendDataTitle(titleName, "DGET ", cmdString, para, Now.Ticks)
    If Not ok Then Return ""

    TT.Write("sys_interproc - FetchData:", titleName, cmdString)

    Do
      idle()
      If myticks = responseTick AndAlso cmdString = responseCmd Then
        TT.Write("sys_interproc - FetchData:", "RESULT=" + responseData)
        Return responseData
      End If
      If myticks + (timeout * 10) < Now.Ticks Then TT.Write("sys_interproc - FetchData:", "TIMEOUT") : Return ""
    Loop
  End Function

  Function SendCommand(ByVal titleName As String, ByVal cmdString As String, ByVal para As String) As Boolean
    Return SendDataTitle(titleName, "CMD  ", cmdString, para, Now.Ticks)
  End Function
  Function SendDataTitle(ByVal titleName As String, ByVal cmdType As String, ByVal cmdString As String, ByVal para As String, ByVal ticks As Long) As Boolean
    Dim hwnd As IntPtr = getWindow(titleName.ToLower)
    If hwnd = IntPtr.Zero Then
      Return False
    End If
    'TT.Write("sys_interproc - SendDataTitle:", titleName + "/" + hwnd.ToString, cmdType, cmdString, para)
    Return SendDataHwnd(hwnd, cmdType, cmdString, para, ticks)
  End Function
  Function SendDataHwnd(ByVal hwnd As IntPtr, ByVal cmdType As String, ByVal cmdString As String, ByVal para As String, ByVal ticks As Long) As Boolean
    Try
      Dim struct As New COPYDATASTRUCT
      Dim strSource As String = cmdType + "|²^³|" + winTitle + "|²^³|" + cmdString + "|²^³|" + ticks.ToString + "|²^³|" + para
      Dim bTarget(strSource.Length * 2) As Byte

      Dim hgcString As GCHandle = GCHandle.Alloc(strSource, GCHandleType.Pinned)
      If hgcSource <> Nothing Then hgcSource.Free()
      Dim hgcDest As GCHandle = GCHandle.Alloc(bTarget, GCHandleType.Pinned)

      InterprocApi.CopyMemory( _
          hgcDest.AddrOfPinnedObject.ToInt32(), _
          hgcString.AddrOfPinnedObject.ToInt32(), _
          strSource.Length * 2 _
      )

      hgcString.Free()

      struct.cdData = strSource.Length * 2
      struct.lpData = hgcDest.AddrOfPinnedObject.ToInt32()

      'hgcSource = GCHandle.Alloc(struct, GCHandleType.Pinned)
      Dim structPointer As IntPtr = Marshal.AllocCoTaskMem(1024)
      Marshal.StructureToPtr(struct, structPointer, True)
      InterprocApi.SendMessage(New HandleRef(Me, hwnd), WindowsMessages.WM_COPYDATA, Me.Handle, structPointer)
      'InterprocApi.PostMessage(New HandleRef(Me, hwnd), WndMsg.WM_COPYDATA, Me.Handle, structPointer)
      'InterprocApi.PostMessage(hwnd, WndMsg.WM_COPYDATA, Me.Handle, hgcSource.AddrOfPinnedObject)

      Return True
    Catch ex As Exception
      Return False
    End Try
  End Function

  Private Function appPath() As String
    Return fp(My.Computer.FileSystem.GetParentPath(Application.ExecutablePath))
  End Function
  Private Function fp(ByVal path As String, Optional ByVal fileName As String = "")
    fp = path + IIf(path.EndsWith("\"), "", "\") + If(fileName.StartsWith("\"), fileName.Substring(1), fileName)
  End Function


  Sub EnsureAppRunning(ByVal winTitle As String, Optional ByVal appName As String = "", Optional ByVal appFileSpec As String = "")
    If getWindow(winTitle.ToLower) = IntPtr.Zero Then
      If appName = "" Then appName = winTitle
      If appFileSpec = "" Then
        Dim fileName As String = "C:\yPara\mwRegistry\" + appName + ".mwreg"
        If IO.File.Exists(fileName) Then
          appFileSpec = IO.File.ReadAllText(fileName)
        ElseIf IO.File.Exists(appPath() + appName + ".exe") Then
          appFileSpec = appPath() + appName + ".exe"
        End If
      End If
      If appFileSpec <> "" Then Process.Start(appFileSpec, "/IPROC_ENSUREAPPRUNNING")
    End If
  End Sub

  Public Sub refreshInterprocWinList()
    On Error Resume Next

    m_interprocWins.Clear()
    Call InterprocApi.EnumWindows(AddressOf EnumWindowsProc, True)

  End Sub


  Private Function EnumWindowsProc(ByVal hwnd As IntPtr, ByVal lParam As Integer) As Integer
    On Error Resume Next
    Dim WindowText As New StringBuilder(256)

    Dim title = InterprocApi.GetWindowText(hwnd)

    'Debug.Print("window Found >" + title + "<")
    If title.StartsWith("sys_interproc@") Then
      'Stop
      TT.Write("sys_interproc - window Found:", ">" + title + "<")
      m_interprocWins.Add(title.Substring(14))
    End If

    EnumWindowsProc = (hwnd <> 0)
  End Function
End Class

MustInherit Class InterprocApi
  Public Delegate Function dEnumWindowsProc(ByVal hwnd As Integer, ByVal lParam As Integer) As Integer

  Public Declare Function EnumWindows Lib "user32" (ByVal lpEnumFunc As dEnumWindowsProc, ByVal lParam As Integer) As Long

  <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
  Private Shared Function GetWindowTextLength(ByVal hwnd As IntPtr) As Integer
  End Function
  <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
  Private Shared Function GetWindowText(ByVal hwnd As IntPtr, _
                       ByVal lpString As StringBuilder, _
                       ByVal cch As Integer) As Integer
  End Function
  Public Shared Function GetWindowText(ByVal hWnd As IntPtr) As String
    Dim length As Integer
    If hWnd.ToInt32 <= 0 Then
      Return Nothing
    End If
    length = GetWindowTextLength(hWnd)
    If length = 0 Then
      Return ""
    End If
    Dim sb As New System.Text.StringBuilder("", length + 1)

    GetWindowText(hWnd, sb, sb.Capacity)
    Return sb.ToString()
  End Function

  <DllImport("KERNEL32.DLL")> _
  Public Shared Sub CopyMemory(ByVal Destination As System.Int32, ByVal Source As System.Int32, ByVal Length As System.Int32)
  End Sub
  <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
  Public Shared Function PostMessage( _
     ByVal hWnd As IntPtr, _
     ByVal Msg As UInteger, _
     ByVal wParam As IntPtr, _
     ByVal lParam As IntPtr) As Boolean
  End Function
  <DllImport("user32.dll", EntryPoint:="FindWindow", SetLastError:=True, CharSet:=CharSet.Auto)> _
  Public Shared Function FindWindowByCaption( _
       ByVal zero As IntPtr, _
       ByVal lpWindowName As String) As IntPtr
  End Function

  <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
  Public Shared Function SendMessage( _
     ByVal hWnd As HandleRef, _
     ByVal Msg As UInteger, _
     ByVal wParam As IntPtr, _
     ByVal lParam As IntPtr) As IntPtr
  End Function
  '<DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
  'Public Shared Function FindWindow( _
  '   ByVal lpClassName As String, _
  '   ByVal lpWindowName As String) As IntPtr
  'End Function
  'Public Declare Sub CopyMemoryByRef Lib "kernel32.dll" Alias "RtlMoveMemory" ( _
  '    ByRef Destination As System.Int32, ByRef Source As System.Int32, ByVal Length As System.Int32)
End Class

<StructLayout(LayoutKind.Sequential)> _
Class COPYDATASTRUCT
  Public dwData As Int32
  Public cdData As Int32
  Public lpData As IntPtr
End Class

'Public Enum WndMsg
'  WM_ACTIVATE = &H6
'  WM_ACTIVATEAPP = &H1C
'  WM_AFXFIRST = &H360
'  WM_AFXLAST = &H37F
'  WM_APP = &H8000
'  WM_ASKCBFORMATNAME = &H30C
'  WM_CANCELJOURNAL = &H4B
'  WM_CANCELMODE = &H1F
'  WM_CAPTURECHANGED = &H215
'  WM_CHANGECBCHAIN = &H30D
'  WM_CHANGEUISTATE = &H127
'  WM_CHAR = &H102
'  WM_CHARTOITEM = &H2F
'  WM_CHILDACTIVATE = &H22
'  WM_CLEAR = &H303
'  WM_CLOSE = &H10
'  WM_COMMAND = &H111
'  WM_COMPACTING = &H41
'  WM_COMPAREITEM = &H39
'  WM_CONTEXTMENU = &H7B
'  WM_COPY = &H301
'  WM_COPYDATA = &H4A
'  WM_CREATE = &H1
'  WM_CTLCOLORBTN = &H135
'  WM_CTLCOLORDLG = &H136
'  WM_CTLCOLOREDIT = &H133
'  WM_CTLCOLORLISTBOX = &H134
'  WM_CTLCOLORMSGBOX = &H132
'  WM_CTLCOLORSCROLLBAR = &H137
'  WM_CTLCOLORSTATIC = &H138
'  WM_CUT = &H300
'  WM_DEADCHAR = &H103
'  WM_DELETEITEM = &H2D
'  WM_DESTROY = &H2
'  WM_DESTROYCLIPBOARD = &H307
'  WM_DEVICECHANGE = &H219
'  WM_DEVMODECHANGE = &H1B
'  WM_DISPLAYCHANGE = &H7E
'  WM_DRAWCLIPBOARD = &H308
'  WM_DRAWITEM = &H2B
'  WM_DROPFILES = &H233
'  WM_ENABLE = &HA
'  WM_ENDSESSION = &H16
'  WM_ENTERIDLE = &H121
'  WM_ENTERMENULOOP = &H211
'  WM_ENTERSIZEMOVE = &H231
'  WM_ERASEBKGND = &H14
'  WM_EXITMENULOOP = &H212
'  WM_EXITSIZEMOVE = &H232
'  WM_FONTCHANGE = &H1D
'  WM_GETDLGCODE = &H87
'  WM_GETFONT = &H31
'  WM_GETHOTKEY = &H33
'  WM_GETICON = &H7F
'  WM_GETMINMAXINFO = &H24
'  WM_GETOBJECT = &H3D
'  WM_GETTEXT = &HD
'  WM_GETTEXTLENGTH = &HE
'  WM_HANDHELDFIRST = &H358
'  WM_HANDHELDLAST = &H35F
'  WM_HELP = &H53
'  WM_HOTKEY = &H312
'  WM_HSCROLL = &H114
'  WM_HSCROLLCLIPBOARD = &H30E
'  WM_ICONERASEBKGND = &H27
'  WM_IME_CHAR = &H286
'  WM_IME_COMPOSITION = &H10F
'  WM_IME_COMPOSITIONFULL = &H284
'  WM_IME_CONTROL = &H283
'  WM_IME_ENDCOMPOSITION = &H10E
'  WM_IME_KEYDOWN = &H290
'  WM_IME_KEYLAST = &H10F
'  WM_IME_KEYUP = &H291
'  WM_IME_NOTIFY = &H282
'  WM_IME_REQUEST = &H288
'  WM_IME_SELECT = &H285
'  WM_IME_SETCONTEXT = &H281
'  WM_IME_STARTCOMPOSITION = &H10D
'  WM_INITDIALOG = &H110
'  WM_INITMENU = &H116
'  WM_INITMENUPOPUP = &H117
'  WM_INPUTLANGCHANGE = &H51
'  WM_INPUTLANGCHANGEREQUEST = &H50
'  WM_KEYDOWN = &H100
'  WM_KEYFIRST = &H100
'  WM_KEYLAST = &H108
'  WM_KEYUP = &H101
'  WM_KILLFOCUS = &H8
'  WM_LBUTTONDBLCLK = &H203
'  WM_LBUTTONDOWN = &H201
'  WM_LBUTTONUP = &H202
'  WM_MBUTTONDBLCLK = &H209
'  WM_MBUTTONDOWN = &H207
'  WM_MBUTTONUP = &H208
'  WM_MDIACTIVATE = &H222
'  WM_MDICASCADE = &H227
'  WM_MDICREATE = &H220
'  WM_MDIDESTROY = &H221
'  WM_MDIGETACTIVE = &H229
'  WM_MDIICONARRANGE = &H228
'  WM_MDIMAXIMIZE = &H225
'  WM_MDINEXT = &H224
'  WM_MDIREFRESHMENU = &H234
'  WM_MDIRESTORE = &H223
'  WM_MDISETMENU = &H230
'  WM_MDITILE = &H226
'  WM_MEASUREITEM = &H2C
'  WM_MENUCHAR = &H120
'  WM_MENUCOMMAND = &H126
'  WM_MENUDRAG = &H123
'  WM_MENUGETOBJECT = &H124
'  WM_MENURBUTTONUP = &H122
'  WM_MENUSELECT = &H11F
'  WM_MOUSEACTIVATE = &H21
'  WM_MOUSEFIRST = &H200
'  WM_MOUSEHOVER = &H2A1
'  WM_MOUSELAST = &H20D
'  WM_MOUSELEAVE = &H2A3
'  WM_MOUSEMOVE = &H200
'  WM_MOUSEWHEEL = &H20A
'  WM_MOUSEHWHEEL = &H20E
'  WM_MOVE = &H3
'  WM_MOVING = &H216
'  WM_NCACTIVATE = &H86
'  WM_NCCALCSIZE = &H83
'  WM_NCCREATE = &H81
'  WM_NCDESTROY = &H82
'  WM_NCHITTEST = &H84
'  WM_NCLBUTTONDBLCLK = &HA3
'  WM_NCLBUTTONDOWN = &HA1
'  WM_NCLBUTTONUP = &HA2
'  WM_NCMBUTTONDBLCLK = &HA9
'  WM_NCMBUTTONDOWN = &HA7
'  WM_NCMBUTTONUP = &HA8
'  WM_NCMOUSEMOVE = &HA0
'  WM_NCPAINT = &H85
'  WM_NCRBUTTONDBLCLK = &HA6
'  WM_NCRBUTTONDOWN = &HA4
'  WM_NCRBUTTONUP = &HA5
'  WM_NEXTDLGCTL = &H28
'  WM_NEXTMENU = &H213
'  WM_NOTIFY = &H4E
'  WM_NOTIFYFORMAT = &H55
'  WM_NULL = &H0
'  WM_PAINT = &HF
'  WM_PAINTCLIPBOARD = &H309
'  WM_PAINTICON = &H26
'  WM_PALETTECHANGED = &H311
'  WM_PALETTEISCHANGING = &H310
'  WM_PARENTNOTIFY = &H210
'  WM_PASTE = &H302
'  WM_PENWINFIRST = &H380
'  WM_PENWINLAST = &H38F
'  WM_POWER = &H48
'  WM_POWERBROADCAST = &H218
'  WM_PRINT = &H317
'  WM_PRINTCLIENT = &H318
'  WM_QUERYDRAGICON = &H37
'  WM_QUERYENDSESSION = &H11
'  WM_QUERYNEWPALETTE = &H30F
'  WM_QUERYOPEN = &H13
'  WM_QUEUESYNC = &H23
'  WM_QUIT = &H12
'  WM_RBUTTONDBLCLK = &H206
'  WM_RBUTTONDOWN = &H204
'  WM_RBUTTONUP = &H205
'  WM_RENDERALLFORMATS = &H306
'  WM_RENDERFORMAT = &H305
'  WM_SETCURSOR = &H20
'  WM_SETFOCUS = &H7
'  WM_SETFONT = &H30
'  WM_SETHOTKEY = &H32
'  WM_SETICON = &H80
'  WM_SETREDRAW = &HB
'  WM_SETTEXT = &HC
'  WM_SETTINGCHANGE = &H1A
'  WM_SHOWWINDOW = &H18
'  WM_SIZE = &H5
'  WM_SIZECLIPBOARD = &H30B
'  WM_SIZING = &H214
'  WM_SPOOLERSTATUS = &H2A
'  WM_STYLECHANGED = &H7D
'  WM_STYLECHANGING = &H7C
'  WM_SYNCPAINT = &H88
'  WM_SYSCHAR = &H106
'  WM_SYSCOLORCHANGE = &H15
'  WM_SYSCOMMAND = &H112
'  WM_SYSDEADCHAR = &H107
'  WM_SYSKEYDOWN = &H104
'  WM_SYSKEYUP = &H105
'  WM_TCARD = &H52
'  WM_TIMECHANGE = &H1E
'  WM_TIMER = &H113
'  WM_UNDO = &H304
'  WM_UNINITMENUPOPUP = &H125
'  WM_USER = &H400
'  WM_USERCHANGED = &H54
'  WM_VKEYTOITEM = &H2E
'  WM_VSCROLL = &H115
'  WM_VSCROLLCLIPBOARD = &H30A
'  WM_WINDOWPOSCHANGED = &H47
'  WM_WINDOWPOSCHANGING = &H46
'  WM_WININICHANGE = &H1A
'  WM_XBUTTONDBLCLK = &H20D
'  WM_XBUTTONDOWN = &H20B
'  WM_XBUTTONUP = &H20C

'End Enum

Public MustInherit Class WindowStyles
  Public Const WS_OVERLAPPED As UInteger = 0
  Public Const WS_POPUP As UInteger = 2147483648
  Public Const WS_CHILD As UInteger = 1073741824
  Public Const WS_MINIMIZE As UInteger = 536870912
  Public Const WS_VISIBLE As UInteger = 268435456
  Public Const WS_DISABLED As UInteger = 134217728
  Public Const WS_CLIPSIBLINGS As UInteger = 67108864
  Public Const WS_CLIPCHILDREN As UInteger = 33554432
  Public Const WS_MAXIMIZE As UInteger = 16777216
  Public Const WS_CAPTION As UInteger = 12582912
  Public Const WS_BORDER As UInteger = 8388608
  Public Const WS_DLGFRAME As UInteger = 4194304
  Public Const WS_VSCROLL As UInteger = 2097152
  Public Const WS_HSCROLL As UInteger = 1048576
  Public Const WS_SYSMENU As UInteger = 524288
  Public Const WS_THICKFRAME As UInteger = 262144
  Public Const WS_GROUP As UInteger = 131072
  Public Const WS_TABSTOP As UInteger = 65536

  Public Const WS_MINIMIZEBOX As UInteger = 131072
  Public Const WS_MAXIMIZEBOX As UInteger = 65536

  Public Const WS_TILED As UInteger = WS_OVERLAPPED
  Public Const WS_ICONIC As UInteger = WS_MINIMIZE
  Public Const WS_SIZEBOX As UInteger = WS_THICKFRAME
  Public Const WS_TILEDWINDOW As UInteger = WS_OVERLAPPEDWINDOW

  ' Common Window Styles

  Public Const WS_OVERLAPPEDWINDOW As UInteger = (WS_OVERLAPPED Or WS_CAPTION Or WS_SYSMENU Or WS_THICKFRAME Or WS_MINIMIZEBOX Or WS_MAXIMIZEBOX)

  Public Const WS_POPUPWINDOW As UInteger = (WS_POPUP Or WS_BORDER Or WS_SYSMENU)

  Public Const WS_CHILDWINDOW As UInteger = WS_CHILD

  'Extended Window Styles

  Public Const WS_EX_DLGMODALFRAME As UInteger = 1
  Public Const WS_EX_NOPARENTNOTIFY As UInteger = 4
  Public Const WS_EX_TOPMOST As UInteger = 8
  Public Const WS_EX_ACCEPTFILES As UInteger = 16
  Public Const WS_EX_TRANSPARENT As UInteger = 32

  '#if(WINVER >= 0x0400)
  Public Const WS_EX_MDICHILD As UInteger = 64
  Public Const WS_EX_TOOLWINDOW As UInteger = 128
  Public Const WS_EX_WINDOWEDGE As UInteger = 256
  Public Const WS_EX_CLIENTEDGE As UInteger = 512
  Public Const WS_EX_CONTEXTHELP As UInteger = 1024

  Public Const WS_EX_RIGHT As UInteger = 4096
  Public Const WS_EX_LEFT As UInteger = 0
  Public Const WS_EX_RTLREADING As UInteger = 8192
  Public Const WS_EX_LTRREADING As UInteger = 0
  Public Const WS_EX_LEFTSCROLLBAR As UInteger = 16384
  Public Const WS_EX_RIGHTSCROLLBAR As UInteger = 0

  Public Const WS_EX_CONTROLPARENT As UInteger = 65536
  Public Const WS_EX_STATICEDGE As UInteger = 131072
  Public Const WS_EX_APPWINDOW As UInteger = 262144

  Public Const WS_EX_OVERLAPPEDWINDOW As UInteger = (WS_EX_WINDOWEDGE Or WS_EX_CLIENTEDGE)
  Public Const WS_EX_PALETTEWINDOW As UInteger = (WS_EX_WINDOWEDGE Or WS_EX_TOOLWINDOW Or WS_EX_TOPMOST)
  '#endif /* WINVER >= 0x0400 */

  '#if(_WIN32_WINNT >= 0x0500)
  Public Const WS_EX_LAYERED As UInteger = 524288
  '#endif /* _WIN32_WINNT >= 0x0500 */

  '#if(WINVER >= 0x0500)
  Public Const WS_EX_NOINHERITLAYOUT As UInteger = 1048576
  ' Disable inheritence of mirroring by children
  Public Const WS_EX_LAYOUTRTL As UInteger = 4194304
  ' Right to left mirroring
  '#endif /* WINVER >= 0x0500 */
  '#if(_WIN32_WINNT >= 0x0500)
  Public Const WS_EX_COMPOSITED As UInteger = 33554432
  Public Const WS_EX_NOACTIVATE As UInteger = 134217728
  '#endif /* _WIN32_WINNT >= 0x0500 */
End Class
