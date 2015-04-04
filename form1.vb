Imports Microsoft.Win32
Imports System.IO
Imports System
Imports System.Diagnostics
Imports System.ComponentModel
Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        '获取内存
        LabelTotalMemNum.Text = Int(My.Computer.Info.TotalPhysicalMemory / 1024 / 1024) & " M"
        LabelAvailableMemNum.Text = Int(My.Computer.Info.AvailablePhysicalMemory / 1024 / 1024) & " M"
        TextBoxAvailableMem.Text = Int(My.Computer.Info.AvailablePhysicalMemory / 1024 / 1024 * 0.9)
        '获取注册表
        If Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True) IsNot Nothing Then
            TextBoxUsername.Text = Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True).GetValue("Username")
            TextBoxParameter.Text = Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True).GetValue("Parameter")
        End If

        '释放更新文件
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
            MessageBox.Show("读取MCUpdater.exe失败! " & ex.Message)
        Catch ex As Exception
            MessageBox.Show("读取MCUpdater.exe失败! " & ex.Message)
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
            MessageBox.Show("读取MCUpdater.ini失败! " & ex.Message)
        Catch ex As Exception
            MessageBox.Show("读取MCUpdater.ini失败! " & ex.Message)
        End Try

        '运行自动更新
        Process.Start("MCUpdater")
        TimerMCUexist.Enabled = True

        '隐藏释放的文件
        Dim fileInfo2 As New FileInfo(FileName2)
        fileInfo2.Attributes = FileAttributes.Hidden
        Dim fileInfo3 As New FileInfo(FileName3)
        fileInfo3.Attributes = FileAttributes.Hidden


    End Sub

    Private Sub MCLauncherEnd()
        TimerMCUend.Enabled = False
        '删除释放的文件
        If IO.File.Exists("MCUpdater.exe") Then
            IO.File.Delete("MCUpdater.exe")
        End If
        If IO.File.Exists("MCUpdater.ini") Then
            IO.File.Delete("MCUpdater.ini")
        End If

        '隐藏升级信息文件
        Dim StrMCUXml As String = "mcupdater.xml"
        Dim fileInfo As New FileInfo(StrMCUXml)
        If IO.File.Exists("mcupdater.xml") Then
            fileInfo.Attributes = FileAttributes.Hidden
        End If

        '自杀
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
        ' MessageBox.Show("""" & System.Environment.GetEnvironmentVariable("JAVA_HOME") & "\bin\javaw.exe" & """" & " -Xmx" & TextBoxMem.Text & "M" & strMcPara & strMcLibraries & "net.minecraft.launchwrapper.Launch  --username " & TextBoxUsername.Text & " --version " & strForgePath & " --gameDir .minecraft\versions\" & strForgePath & " --assetsDir .minecraft\assets --assetIndex 1.7.10 --uuid ${auth_uuid} --accessToken ${auth_access_token} --userProperties {} --userType Legacy --tweakClass cpw.mods.fml.common.launcher.FMLTweaker")
    End Sub

    Private Sub TimerAutoKill_Tick(sender As Object, e As EventArgs) Handles TimerAutoKill.Tick
        '自杀
        Process.GetCurrentProcess().Kill()
    End Sub


    Private Sub ButtonRunMc_Click(sender As Object, e As EventArgs) Handles ButtonRunMc.Click
        SetReg()
        RunMc()
        Application.Exit()
    End Sub

#Region "设置MC启动命令行"
    Private Function GetMcPath()
        Dim dirAimPathTemp As New IO.DirectoryInfo(Environment.CurrentDirectory & "\.minecraft\versions\"), strAimPath As String, strForgePath As String, strDefaultPara As String
        '获取forge目录
        strForgePath = dirAimPathTemp.GetDirectories.GetValue(0).ToString
        strAimPath = dirAimPathTemp.ToString & strForgePath & "\"
        Dim strMcPara As String, strLibPath As String = Environment.CurrentDirectory & "\.minecraft\libraries\", strShell As String
        Dim strTmpLib As String = Environment.CurrentDirectory & "\.minecraft\libraries\"
        '设置附加参数
        strDefaultPara = " -XX:-UseVMInterruptibleIO -XX:NewRatio=3 -XX:+UseStringCache -XX:+UseConcMarkSweepGC -XX:+UseParNewGC -XX:+CMSIncrementalPacing -XX:+AggressiveOpts -XX:+UseFastAccessorMethods -XX:+UseBiasedLocking -XX:PermSize=128m -XX:MaxPermSize=256m -XX:+CMSParallelRemarkEnabled -XX:MaxGCPauseMillis=50 -XX:+UseAdaptiveGCBoundary -XX:-UseGCOverheadLimit -XX:SurvivorRatio=8 -XX:TargetSurvivorRatio=90 -XX:MaxTenuringThreshold=15 -XX:+UseAdaptiveSizePolicy -XX:+DisableExplicitGC -Xnoclassgc -oss4M -ss4M -XX:CMSInitiatingOccupancyFraction=60 -XX:+UseCMSCompactAtFullCollection -XX:CMSFullGCsBeforeCompaction=1 -XX:SoftRefLRUPolicyMSPerMB=2048 -Xms800M -XX:ParallelGCThreads=" & System.Environment.ProcessorCount & " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true "
        strMcPara = strDefaultPara & TextBoxParameter.Text & " -Djava.library.path=" & """" & ".minecraft\natives" & """" & " -cp "
        '设置命令行
        strShell = """" & GetJavaHome() & """" & " -Xmx" & TextBoxAvailableMem.Text & "M" & strMcPara & """" & GetLibrariesFiles(strLibPath) & Environment.CurrentDirectory & "\.minecraft\versions\" & strForgePath & "\" & strForgePath & ".jar" & """" & " net.minecraft.launchwrapper.Launch  --username " & TextBoxUsername.Text & " --version " & strForgePath & " --gameDir .minecraft\versions\" & strForgePath & " --assetsDir .minecraft\assets --assetIndex 1.7.10 --uuid ${auth_uuid} --accessToken ${auth_access_token} --userProperties {} --userType Legacy --tweakClass cpw.mods.fml.common.launcher.FMLTweaker"
        Return strShell
    End Function
#End Region

#Region "设置注册表"
    Private Sub SetReg()
        Dim regUsernameKey As RegistryKey
        If Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True) Is Nothing Then
            Registry.CurrentUser.CreateSubKey("Software\LightMCLauncher")
            Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True).SetValue("Username", TextBoxUsername.Text)
        Else
            If Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True).GetValue("Username") Is Nothing Then
                Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True).SetValue("Username", TextBoxUsername.Text)
            Else
                Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True).SetValue("Username", TextBoxUsername.Text)
            End If

            If Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True).GetValue("Parameter") Is Nothing Then
                Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True).SetValue("Parameter", TextBoxParameter.Text)
            Else
                Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True).SetValue("Parameter", TextBoxParameter.Text)
            End If
        End If

    End Sub
#End Region

#Region "运行MC"
    Private Sub RunMc()

        Dim swRunMc As StreamWriter
        swRunMc = File.CreateText(System.Environment.GetEnvironmentVariable("temp") & "\run.bat")
        swRunMc.WriteLine(GetMcPath() & Chr(13) + Chr(10) & "exit")
        swRunMc.Close()
        swRunMc = File.CreateText(System.Environment.GetEnvironmentVariable("temp") & "\run.vbs")
        swRunMc.WriteLine("set ws=wscript.createobject(" & """" & "wscript.shell" & """" & ") " & Chr(13) + Chr(10) & "ws.run " & """" & System.Environment.GetEnvironmentVariable("temp") & "\run.bat" & """" & ",0")
        swRunMc.Close()
        Process.Start(System.Environment.GetEnvironmentVariable("temp") & "\run.vbs")

    End Sub
#End Region

#Region "获取java路径"
    Private Function GetJavaHome()
        Dim strJavaHome As String, strJavaVer As String
        strJavaVer = Registry.LocalMachine.OpenSubKey("SOFTWARE\javasoft\Java Runtime Environment", True).GetValue("CurrentVersion")
        strJavaVer = Registry.LocalMachine.OpenSubKey("SOFTWARE\javasoft\Java Runtime Environment\" & strJavaVer, True).GetValue("JavaHome")
        strJavaHome = strJavaVer & "\bin\javaw.exe"
        Return strJavaHome
    End Function
#End Region

#Region "获取Libraries文件列表"
    Function GetLibrariesFiles(ByVal strLibPath As String)
        Dim strLibFiles() As String, i As Integer, strMcLibraries As String
        strLibFiles = IO.Directory.GetFiles(strLibPath, "*.jar", SearchOption.AllDirectories)
        Do Until i = strLibFiles.Length
            strMcLibraries = strMcLibraries & strLibFiles(i) & ";"
            i += 1
        Loop
        Return strMcLibraries
        Return strLibFiles.Length
    End Function
#End Region

    Private Sub ButtonRefreshMem_Click(sender As Object, e As EventArgs) Handles ButtonRefreshMem.Click
        LabelAvailableMemNum.Text = Int(My.Computer.Info.AvailablePhysicalMemory / 1024 / 1024) & " M"
        TextBoxAvailableMem.Text = Int(My.Computer.Info.AvailablePhysicalMemory / 1024 / 1024 * 0.9)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        SetReg()
    End Sub

    Private Sub ButtonDefaultParameter_Click(sender As Object, e As EventArgs) Handles ButtonDefaultParameter.Click
        MessageBox.Show("-XX:-UseVMInterruptibleIO -XX:NewRatio=3 -XX:+UseStringCache -XX:+UseConcMarkSweepGC -XX:+UseParNewGC -XX:+CMSIncrementalPacing -XX:+AggressiveOpts -XX:+UseFastAccessorMethods -XX:+UseBiasedLocking -XX:PermSize=128m -XX:MaxPermSize=256m -XX:+CMSParallelRemarkEnabled -XX:MaxGCPauseMillis=50 -XX:+UseAdaptiveGCBoundary -XX:-UseGCOverheadLimit -XX:SurvivorRatio=8 -XX:TargetSurvivorRatio=90 -XX:MaxTenuringThreshold=15 -XX:+UseAdaptiveSizePolicy -XX:+DisableExplicitGC -Xnoclassgc -oss4M -ss4M -XX:CMSInitiatingOccupancyFraction=60 -XX:+UseCMSCompactAtFullCollection -XX:CMSFullGCsBeforeCompaction=1 -XX:SoftRefLRUPolicyMSPerMB=2048 -Xms800M -XX:ParallelGCThreads=" & System.Environment.ProcessorCount & " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true ")
    End Sub
End Class
