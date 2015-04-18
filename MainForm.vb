Imports Microsoft.Win32
Imports System.IO
Imports System
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.RegularExpressions

Public Class MainForm
    Dim CoreFunction As New CoreClass
    Private Sub Main_Load(sender As Object, e As EventArgs) Handles Me.Load
        iniSub(0)

        TimerRefreshInfo_Tick()
        TimerRefreshInfo.Enabled = True
        Try
            Shell("java", AppWinStyle.Hide)
        Catch NoJava As FileNotFoundException
            MessageBox.Show("Cannot find Java.exe!Please set java path manually.")
        End Try

        'Get Server Info
        Dim ip As String
        Dim ipHost As IPHostEntry = Dns.GetHostEntry("127.0.0.1")
        For Each ip1 As IPAddress In ipHost.AddressList
            ip = ip1.ToString
            Exit For
        Next
        Dim MCServerInfo As eMZi.Gaming.Minecraft.MinecraftServerInfo
        Dim MCServerIPAddress As IPAddress
        MCServerIPAddress = IPAddress.Parse(ip)
        Dim MCServerPoint As New IPEndPoint(MCServerIPAddress, 25565)
        Try
            MCServerInfo = eMZi.Gaming.Minecraft.MinecraftServerInfo.GetServerInformation(MCServerPoint)
            LabelServerVersion.Text = "服务器版本:" & MCServerInfo.MinecraftVersion
            LabelServerPlayerCount.Text = "在线人数:" & MCServerInfo.CurrentPlayerCount & "/" & MCServerInfo.MaxPlayerCount
        Catch UnavaliableServer As Exception
            LabelServerVersion.Text = "Server Version:Error"
            LabelServerPlayerCount.Text = "Current Player:Error"
        End Try

        'release update files
        Dim FileName2 As String = "MCUpdater.ini", FileName3 As String = "MCUpdater.exe"
        Dim bufint As Integer
        Dim bufbytes(0) As Byte
        Dim fs As FileStream

        Try
            Dim Asm As Reflection.Assembly = Reflection.Assembly.GetExecutingAssembly()
            Dim strm As Stream = Asm.GetManifestResourceStream( _
                         Asm.GetName().Name + "." + FileName3)
            fs = File.OpenWrite("MCUpdater.exe")

            Do
                bufint = strm.ReadByte()
                If bufint = -1 Then Exit Do
                bufbytes(0) = Convert.ToByte(bufint)
                fs.Write(bufbytes, 0, bufbytes.Length)
            Loop

            fs.Close()
            fs.Dispose()
            strm.Close()

        Catch ex As System.IO.IOException
            MessageBox.Show("Relesae MCUpdater.exe failed!" & ex.Message)
        Catch ex As Exception
            MessageBox.Show("Relesae MCUpdater.exe failed!" & ex.Message)
        End Try

        Try
            Dim Asm As Reflection.Assembly = Reflection.Assembly.GetExecutingAssembly()
            Dim strm As Stream = Asm.GetManifestResourceStream( _
                         Asm.GetName().Name + "." + FileName2)
            fs = File.OpenWrite("MCUpdater.ini")

            Do
                bufint = strm.ReadByte()
                If bufint = -1 Then Exit Do
                bufbytes(0) = Convert.ToByte(bufint)
                fs.Write(bufbytes, 0, bufbytes.Length)
            Loop

            fs.Close()
            fs.Dispose()
            strm.Close()

        Catch ex As System.IO.IOException
            MessageBox.Show("Relesae MCUpdater.ini failed!" & ex.Message)
        Catch ex As Exception
            MessageBox.Show("Relesae MCUpdater.ini failed!" & ex.Message)
        End Try

        'run update
        Process.Start("MCUpdater")
        TimerMCUexist.Enabled = True

        'hide files
        Dim fileInfo2 As New FileInfo(FileName2)
        fileInfo2.Attributes = FileAttributes.Hidden
        Dim fileInfo3 As New FileInfo(FileName3)
        fileInfo3.Attributes = FileAttributes.Hidden


    End Sub

    Private Sub MCLauncherEnd()
        TimerMCUend.Enabled = False
        'delete released files
        If IO.File.Exists("MCUpdater.exe") Then
            IO.File.Delete("MCUpdater.exe")
        End If
        If IO.File.Exists("MCUpdater.ini") Then
            IO.File.Delete("MCUpdater.ini")
        End If

        'hide xml file
        Dim StrMCUXml As String = "mcupdater.xml"
        Dim fileInfo As New FileInfo(StrMCUXml)
        If IO.File.Exists("mcupdater.xml") Then
            fileInfo.Attributes = FileAttributes.Hidden
        End If

        Process.GetCurrentProcess().Kill()

    End Sub

    Private Sub TimerMCUexist_Tick(sender As Object, e As EventArgs) Handles TimerMCUexist.Tick
        If Process.GetProcessesByName("MCUpdater").Length = 0 Then
            TimerMCUend.Enabled = True
        End If
    End Sub

    Private Sub TimerMCUend_Tick(sender As Object, e As EventArgs) Handles TimerMCUend.Tick
        TimerMCUexist.Enabled = False
        Me.Opacity = 100%
        ' Process.Start(strShell.ToString)
        ' Process.GetCurrentProcess().Kill()
        ' MessageBox.Show("""" & System.Environment.GetEnvironmentVariable("JAVA_HOME") & "\bin\javaw.exe" & """" & " -Xmx" & TextBoxMem.Text & "M" & strMcPara & strMcLibraries & "net.minecraft.launchwrapper.Launch  --username " & TextBoxUsername.Text & " --version " &  strForgeVersion & " --gameDir .minecraft\versions\" &  strForgeVersion & " --assetsDir .minecraft\assets --assetIndex 1.7.10 --uuid ${auth_uuid} --accessToken ${auth_access_token} --userProperties {} --userType Legacy --tweakClass cpw.mods.fml.common.launcher.FMLTweaker")
    End Sub

    Private Sub TimerAutoKill_Tick(sender As Object, e As EventArgs) Handles TimerAutoKill.Tick
        Process.GetCurrentProcess().Kill()
    End Sub


    Private Sub ButtonRunMc_Click(sender As Object, e As EventArgs) Handles ButtonRunMc.Click
        iniSub(1)
        'Shell(GetMcPath(), AppWinStyle.NormalNoFocus)
        RunMc()
        Application.Exit()
    End Sub


#Region "Set Reg"
    Private Sub SetReg()

        If Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True) Is Nothing Then
            Registry.CurrentUser.CreateSubKey("Software\LightMCLauncher")
        End If
        Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True).SetValue("Username", TextBoxUsername.Text)
        Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True).SetValue("Parameter", TextBoxParameter.Text)

    End Sub
#End Region

#Region "RunMC"
    Private Sub RunMc()
        Dim dirAimPathTemp As New IO.DirectoryInfo(Application.StartupPath & "\.minecraft\versions\"), strForgeVersion As String
        strForgeVersion = dirAimPathTemp.GetDirectories.GetValue(0).ToString

        Dim swRunMcBat As New IO.StreamWriter(System.Environment.GetEnvironmentVariable("temp") & "\runmc.bat", False, System.Text.Encoding.Default)
        Dim swRunMcVbs As New IO.StreamWriter(System.Environment.GetEnvironmentVariable("temp") & "\runmc.vbs", False, System.Text.Encoding.Default)
        swRunMcBat.WriteLine(CoreFunction.Core(0, TextBoxAvailableMem.Text, strForgeVersion, TextBoxUsername.Text, TextBoxParameter.Text) & Chr(13) + Chr(10) & "pause")
        swRunMcBat.Close()
        swRunMcVbs.WriteLine("set ws=wscript.createobject(" & """" & "wscript.shell" & """" & ") " & Chr(13) + Chr(10) & "ws.run " & """" & System.Environment.GetEnvironmentVariable("temp") & "\runmc.bat" & """" & ",0")
        swRunMcVbs.Close()
        Process.Start(System.Environment.GetEnvironmentVariable("temp") & "\runmc.vbs")

    End Sub
#End Region

#Region "get java path"
    Private Function GetJavaHome()
        Try
            Dim strJavaHome As String, strJavaVer As String
            strJavaVer = Registry.LocalMachine.OpenSubKey("SOFTWARE\javasoft\Java Runtime Environment", True).GetValue("CurrentVersion")
            strJavaVer = Registry.LocalMachine.OpenSubKey("SOFTWARE\javasoft\Java Runtime Environment\" & strJavaVer, True).GetValue("JavaHome")
            strJavaHome = strJavaVer & "\bin\java.exe"
            Return strJavaHome
        Catch NoJava As Exception
            MessageBox.Show("Cannot find Java.exe!")
        End Try
    End Function
#End Region

    Private Sub ButtonDefaultParameter_Click(sender As Object, e As EventArgs) Handles ButtonDefaultParameter.Click
        MessageBox.Show("-XX:-UseVMInterruptibleIO -XX:NewRatio=3 -XX:+UseStringCache -XX:+UseConcMarkSweepGC -XX:+UseParNewGC -XX:+CMSIncrementalPacing -XX:+AggressiveOpts -XX:+UseFastAccessorMethods -XX:+UseBiasedLocking -XX:PermSize=128m -XX:MaxPermSize=256m -XX:+CMSParallelRemarkEnabled -XX:MaxGCPauseMillis=50 -XX:+UseAdaptiveGCBoundary -XX:-UseGCOverheadLimit -XX:SurvivorRatio=8 -XX:TargetSurvivorRatio=90 -XX:MaxTenuringThreshold=15 -XX:+UseAdaptiveSizePolicy -XX:+DisableExplicitGC -Xnoclassgc -oss4M -ss4M -XX:CMSInitiatingOccupancyFraction=60 -XX:+UseCMSCompactAtFullCollection -XX:CMSFullGCsBeforeCompaction=1 -XX:SoftRefLRUPolicyMSPerMB=2048 -Xms800M -XX:ParallelGCThreads=" & System.Environment.ProcessorCount & " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true ")
    End Sub


    Private Sub TimerRefreshInfo_Tick() Handles TimerRefreshInfo.Tick
        'get MEM
        LabelTotalMemNum.Text = Int(My.Computer.Info.TotalPhysicalMemory / 1024 / 1024) & " M"
        LabelAvailableMemNum.Text = Int(My.Computer.Info.AvailablePhysicalMemory / 1024 / 1024) & " M"
        If Int(My.Computer.Info.AvailablePhysicalMemory / 1024 / 1024) > 4096 Then
            TextBoxAvailableMem.Text = 4096
        Else
            TextBoxAvailableMem.Text = Int(My.Computer.Info.AvailablePhysicalMemory / 1024 / 1024 * 0.9)
        End If

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'Dim update As New Form
        'update.Show()
        'Me.Hide()
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("http://tieba.baidu.com/home/main?id=bb13383737303438373634bb09&fr=userbar")
    End Sub

    Private Sub iniSub(ByVal SubMode As Integer)
        Dim iniPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\LightMCLauncher\user.ini"
        Select Case SubMode
            Case 0
                Try
                    TextBoxUsername.Text = CoreFunction.GetINI("User", "Username", "", iniPath)
                    TextBoxParameter.Text = CoreFunction.GetINI("User", "Parameter", "", iniPath)
                Catch ex As Exception

                End Try
            Case 1
                CoreFunction.WriteINI("User", "Username", TextBoxUsername.Text, iniPath)
                CoreFunction.WriteINI("User", "Parameter", TextBoxParameter.Text, iniPath)
        End Select
    End Sub
End Class


