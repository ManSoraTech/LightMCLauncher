Imports System.IO
Imports System.Net
Imports System.Threading

Public Class LauncherUpdate

    Dim DownThread As Thread
    Private Delegate Sub Show()
    Private Delegate Sub voidDelegate(ByRef totalDownloadedByte As Long, totalBytes As Long)
    Dim LauncherURL As String = PeoLeser.Minecraft.CoreClass.GetINI("Launcher", "LauncherURL", "", Application.StartupPath + "\LightMCLauncher\launcher.ini")

    Private Sub LauncherUpdate_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        Control.CheckForIllegalCrossThreadCalls = False
        MetroLabel.Text = "Checking Launcher update..."
        MetroLabel.Left = (Me.Width - MetroLabel.Width) \ 2
        MetroProgressBar.Left = (Me.Width - MetroProgressBar.Width) \ 2

        PeoLeser.Minecraft.CoreClass.WriteINI("Launcher", "Update", "False", Application.StartupPath + "\LightMCLauncher\launcher.ini")
        Try
            If PeoLeser.Minecraft.CoreClass.GetINI("Launcher", "Update", "", Application.StartupPath + "\LightMCLauncher\launcher.ini") = "True" Then
                Process.Start(Application.StartupPath + "\LightMCLauncher\" + Strings.Right(LauncherURL, LauncherURL.Length - LauncherURL.LastIndexOf("/") - 1))
                Application.Exit()
            Else
                PeoLeser.Minecraft.CoreClass.WriteINI("Launcher", "LauncherFile", Strings.Right(Application.ExecutablePath, Application.ExecutablePath.Length - Application.StartupPath.Length - 1), Application.StartupPath + "\LightMCLauncher\launcher.ini")
                DownThread = New Thread(New ThreadStart(AddressOf LauncherDown))
                'DownThread.IsBackground = True
                DownThread.Start()
                'DownThread.Join()
            End If
        Catch ex As FileNotFoundException
            PeoLeser.Minecraft.CoreClass.WriteINI("Launcher", "Update", "False", Application.StartupPath + "\LightMCLauncher\launcher.ini")
        End Try



    End Sub

    Private Sub LauncherDown()

        Dim GetUpdateXml As New WebClient()
        Try
            GetUpdateXml.DownloadFile(PeoLeser.Minecraft.CoreClass.GetINI("Launcher", "SettingURL", "", Application.StartupPath + "\LightMCLauncher\launcher.ini"), Application.StartupPath + "\LightMCLauncher\launcher.ini")
        Catch
        End Try

        Try
            Dim url As String = PeoLeser.Minecraft.CoreClass.GetINI("Launcher", "LauncherCheckURL", "", Application.StartupPath + "\LightMCLauncher\launcher.ini")
            Dim httpReq As HttpWebRequest
            Dim httpResp As HttpWebResponse
            Dim httpURL As New Uri(url)
            httpReq = CType(WebRequest.Create(httpURL), HttpWebRequest)
            httpReq.Method = "GET"
            httpResp = CType(httpReq.GetResponse(), HttpWebResponse)
            httpReq.KeepAlive = False
            Dim reader As StreamReader = New StreamReader(httpResp.GetResponseStream, System.Text.Encoding.Default)
            Dim respHTML As String = reader.ReadToEnd()

            Dim LauncherProperties As FileVersionInfo = FileVersionInfo.GetVersionInfo(Strings.Right(Application.ExecutablePath, Application.ExecutablePath.Length - Application.StartupPath.Length - 1))

            If respHTML <> LauncherProperties.FileVersion Then

                MetroLabel.Text = "The Minecraft Launcher is updating..."
                MetroLabel.Left = (Me.Width - MetroLabel.Width) \ 2
                MetroProgressBar.ProgressBarStyle = 1
                ' Dim LauncherURL As String = PeoLeser.Minecraft.CoreClass.GetINI("Launcher", "LauncherURL", "", Application.StartupPath + "\LightMCLauncher\launcher.ini")
                Dim Myrq As HttpWebRequest = HttpWebRequest.Create(LauncherURL)
                Dim myrp As HttpWebResponse = Myrq.GetResponse
                Dim totalBytes As Long = myrp.ContentLength
                Dim st As Stream = myrp.GetResponseStream
                Dim so As Stream = New FileStream(Application.StartupPath + "\LightMCLauncher\launcherupdate.exe", FileMode.Create)
                Dim totalDownloadedByte As Long = 0
                Dim by(1024) As Byte
                Dim osize As Integer = st.Read(by, 0, by.Length)
                While osize > 0
                    totalDownloadedByte = osize + totalDownloadedByte
                    Application.DoEvents()
                    so.Write(by, 0, osize)
                    Me.Invoke(New voidDelegate(AddressOf UpdateUI), totalDownloadedByte, totalBytes)
                    osize = st.Read(by, 0, by.LongLength)
                End While
                so.Close()
                st.Close()

                PeoLeser.Minecraft.CoreClass.WriteINI("Launcher", "Update", "True", Application.StartupPath + "\LightMCLauncher\launcher.ini")
                Process.Start(Application.StartupPath + "\LightMCLauncher\" + Strings.Right(LauncherURL, LauncherURL.Length - LauncherURL.LastIndexOf("/") - 1))
                Application.Exit()
                DownThread.Abort()
            Else
                MetroProgressBar.ProgressBarStyle = 1
                MetroProgressBar.Value = MetroProgressBar.Maximum

                Me.Invoke(New Show(AddressOf ShowMainForm))
                DownThread.Abort()

            End If

        Catch ex As WebException
            Me.Invoke(New Show(AddressOf ShowMainForm))
            DownThread.Abort()
        End Try

    End Sub

    Private Sub ShowMainForm()
        MainForm.Show()
    End Sub

    Private Sub UpdateUI(ByRef totalDownloadedByte As Long, totalBytes As Long)
        MetroProgressBar.Maximum = totalBytes
        MetroProgressBar.Value = totalDownloadedByte
    End Sub

    Private Sub MetroLabel_Click(sender As Object, e As EventArgs) Handles MetroLabel.Click

    End Sub

    Private Sub LauncherUpdate_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
