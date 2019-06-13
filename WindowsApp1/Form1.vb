Public Class Form1

    Const WM_CAP As Short = &H400S
    Const WM_CAP_DRIVER_CONNECT As Integer = WM_CAP + 10
    Const WM_CAP_DRIVER_DISCONNECT As Integer = WM_CAP + 11

    Const WM_CAP_SET_PREVIEW As Integer = WM_CAP + 50
    Const WM_CAP_SET_PREVIEWRATE As Integer = WM_CAP + 52
    Const WM_CAP_SET_SCALE As Integer = WM_CAP + 53
    Const WS_CHILD As Integer = &H40000000
    Const WS_VISIBLE As Integer = &H10000000
    Const SWP_NOMOVE As Short = &H2S
    Const SWP_NOZORDER As Short = &H4S
    Const HWND_BOTTOM As Short = 1

    Dim iDevice As Integer = 0
    Dim hHwnd As Integer

    Declare Function SendMessage Lib "user32" Alias "SendMessageA" _
     (ByVal hwnd As Integer, ByVal wMsg As Integer, ByVal wParam As Boolean,
     ByRef lParam As Integer) As Boolean

    Declare Function SetWindowPos Lib "user32" Alias "SetWindowPos" (ByVal hwnd As Integer,
        ByVal hWndInsertAfter As Integer, ByVal x As Integer, ByVal y As Integer,
        ByVal cx As Integer, ByVal cy As Integer, ByVal wFlags As Integer) As Integer

    Declare Function DestroyWindow Lib "user32" (ByVal hndw As Integer) As Boolean

    Declare Function capCreateCaptureWindowA Lib "avicap32.dll" _
         (ByVal lpszWindowName As String, ByVal dwStyle As Integer,
         ByVal x As Integer, ByVal y As Integer, ByVal nWidth As Integer,
         ByVal nHeight As Short, ByVal hWndParent As Integer,
         ByVal nID As Integer) As Integer

    Declare Function capGetDriverDescriptionA Lib "avicap32.dll" (ByVal wDriver As Short,
        ByVal lpszName As String, ByVal cbName As Integer, ByVal lpszVer As String,
        ByVal cbVer As Integer) As Boolean

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If Button2.Enabled Then
            ClosePreviewWindow()
        End If
    End Sub
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LoadDeviceList()
        Button1.Enabled = True
        Me.AutoScrollMinSize = New Size(100, 100)
        Button2.Enabled = False
        PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
        If Button2.Enabled = False Then
            PictureBox1.Hide()
            Me.Width = 650
            Me.Height = 450
        End If
    End Sub
    Private Sub LoadDeviceList()
        Dim strName As String = Space(100)
        Dim strVer As String = Space(100)
        Dim bReturn As Boolean
        Dim x As Short = 0
        Do
            bReturn = capGetDriverDescriptionA(x, strName, 100, strVer, 100)
            x += CType(1, Short)
        Loop Until bReturn = False
    End Sub

    Private Sub OpenPreviewWindow()
        hHwnd = capCreateCaptureWindowA(iDevice.ToString, WS_VISIBLE Or WS_CHILD, 0, 0, 1280,
            720, PictureBox1.Handle.ToInt32, 0)
        If SendMessage(hHwnd, WM_CAP_DRIVER_CONNECT, iDevice, 0) Then
            SendMessage(hHwnd, WM_CAP_SET_SCALE, True, 0)
            SendMessage(hHwnd, WM_CAP_SET_PREVIEWRATE, 2, 0)
            SendMessage(hHwnd, WM_CAP_SET_PREVIEW, True, 0)
            SetWindowPos(hHwnd, HWND_BOTTOM, 0, 0, PictureBox1.Width, PictureBox1.Height,
                    SWP_NOMOVE Or SWP_NOZORDER)
            Button2.Enabled = True
            Button1.Enabled = True
        Else
            DestroyWindow(hHwnd)
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        OpenPreviewWindow()
        PictureBox1.Show()
    End Sub

    Private Sub ClosePreviewWindow()
        SendMessage(hHwnd, WM_CAP_DRIVER_DISCONNECT, iDevice, 0)
        DestroyWindow(hHwnd)
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        ClosePreviewWindow()
        Button2.Enabled = False
    End Sub

End Class