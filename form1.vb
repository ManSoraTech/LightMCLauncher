Imports System.IO
Imports System
Imports System.Diagnostics
Imports System.ComponentModel
Public Class Form1

    Dim dirAimPathTemp As New IO.DirectoryInfo(Environment.CurrentDirectory & "\.minecraft\versions\"), strAimPath As String, strForgePath As String, strMcLibraries As String


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        '获取可用内存
        TextBoxMem.Text = Int(My.Computer.Info.AvailablePhysicalMemory / 1024 / 1024)
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


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        '获取目标forge目录
        strForgePath = dirAimPathTemp.GetDirectories.GetValue(0).ToString
        strAimPath = dirAimPathTemp.ToString & strForgePath & "\"
        Dim strMcPara As String, strMcLibraries As String, strTmpLib As String, strShell As String
        strTmpLib = Environment.CurrentDirectory & "\.minecraft\libraries\"
        strMcPara = " -XX:-UseVMInterruptibleIO -XX:NewRatio=3 -XX:+UseStringCache -XX:+UseConcMarkSweepGC -XX:+UseParNewGC -XX:+CMSIncrementalPacing -XX:+AggressiveOpts -XX:+UseFastAccessorMethods -XX:+UseBiasedLocking -XX:PermSize=128m -XX:MaxPermSize=256m -XX:+CMSParallelRemarkEnabled -XX:MaxGCPauseMillis=50 -XX:+UseAdaptiveGCBoundary -XX:-UseGCOverheadLimit -XX:SurvivorRatio=8 -XX:TargetSurvivorRatio=90 -XX:MaxTenuringThreshold=15 -XX:+UseAdaptiveSizePolicy -XX:+DisableExplicitGC -Xnoclassgc -oss4M -ss4M -XX:CMSInitiatingOccupancyFraction=60 -XX:+UseCMSCompactAtFullCollection -XX:CMSFullGCsBeforeCompaction=1 -XX:SoftRefLRUPolicyMSPerMB=2048 -Xms2G -XX:ParallelGCThreads=" & System.Environment.ProcessorCount & " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true -Djava.library.path=" & """" & ".minecraft\natives" & """" & " -cp"
        strMcLibraries = """" & strTmpLib & "net\minecraftforge\forge\1.7.10-10.13.2.1291\forge-1.7.10-10.13.2.1291.jar;" & strTmpLib & "net\minecraft\launchwrapper\1.11\launchwrapper-1.11.jar;" & strTmpLib & "org\ow2\asm\asm-all\5.0.3\asm-all-5.0.3.jar;" & strTmpLib & "com\typesafe\akka\akka-actor_2.11\2.3.3\akka-actor_2.11-2.3.3.jar;" & strTmpLib & "com\typesafe\config\1.2.1\config-1.2.1.jar;" & strTmpLib & "org\scala-lang\scala-actors-migration_2.11\1.1.0\scala-actors-migration_2.11-1.1.0.jar;" & strTmpLib & "org\scala-lang\scala-compiler\2.11.1\scala-compiler-2.11.1.jar;" & strTmpLib & "org\scala-lang\plugins\scala-continuations-library_2.11\1.0.2\scala-continuations-library_2.11-1.0.2.jar;" & strTmpLib & "org\scala-lang\plugins\scala-continuations-plugin_2.11.1\1.0.2\scala-continuations-plugin_2.11.1-1.0.2.jar;" & strTmpLib & "org\scala-lang\scala-library\2.11.1\scala-library-2.11.1.jar;" & strTmpLib & "org\scala-lang\scala-parser-combinators_2.11\1.0.1\scala-parser-combinators_2.11-1.0.1.jar;" & strTmpLib & "org\scala-lang\scala-reflect\2.11.1\scala-reflect-2.11.1.jar;" & strTmpLib & "org\scala-lang\scala-swing_2.11\1.0.1\scala-swing_2.11-1.0.1.jar;" & strTmpLib & "org\scala-lang\scala-xml_2.11\1.0.2\scala-xml_2.11-1.0.2.jar;" & strTmpLib & "net\sf\jopt-simple\jopt-simple\4.5\jopt-simple-4.5.jar;" & strTmpLib & "lzma\lzma\0.0.1\lzma-0.0.1.jar;" & strTmpLib & "com\mojang\realms\1.3.5\realms-1.3.5.jar;" & strTmpLib & "org\apache\commons\commons-compress\1.8.1\commons-compress-1.8.1.jar;" & strTmpLib & "org\apache\httpcomponents\httpclient\4.3.3\httpclient-4.3.3.jar;" & strTmpLib & "commons-logging\commons-logging\1.1.3\commons-logging-1.1.3.jar;" & strTmpLib & "org\apache\httpcomponents\httpcore\4.3.2\httpcore-4.3.2.jar;" & strTmpLib & "java3d\vecmath\1.3.1\vecmath-1.3.1.jar;" & strTmpLib & "net\sf\trove4j\trove4j\3.0.3\trove4j-3.0.3.jar;" & strTmpLib & "com\ibm\icu\icu4j-core-mojang\51.2\icu4j-core-mojang-51.2.jar;" & strTmpLib & "com\paulscode\codecjorbis\20101023\codecjorbis-20101023.jar;" & strTmpLib & "com\paulscode\codecwav\20101023\codecwav-20101023.jar;" & strTmpLib & "com\paulscode\libraryjavasound\20101123\libraryjavasound-20101123.jar;" & strTmpLib & "com\paulscode\librarylwjglopenal\20100824\librarylwjglopenal-20100824.jar;" & strTmpLib & "com\paulscode\soundsystem\20120107\soundsystem-20120107.jar;" & strTmpLib & "io\netty\netty-all\4.0.10.Final\netty-all-4.0.10.Final.jar;" & strTmpLib & "com\google\guava\guava\16.0\guava-16.0.jar;" & strTmpLib & "org\apache\commons\commons-lang3\3.2.1\commons-lang3-3.2.1.jar;" & strTmpLib & "commons-io\commons-io\2.4\commons-io-2.4.jar;" & strTmpLib & "commons-codec\commons-codec\1.9\commons-codec-1.9.jar;" & strTmpLib & "net\java\jinput\jinput\2.0.5\jinput-2.0.5.jar;" & strTmpLib & "net\java\jutils\jutils\1.0.0\jutils-1.0.0.jar;" & strTmpLib & "com\google\code\gson\gson\2.2.4\gson-2.2.4.jar;" & strTmpLib & "com\mojang\authlib\1.5.16\authlib-1.5.16.jar;" & strTmpLib & "org\apache\logging\log4j\log4j-api\2.0-beta9\log4j-api-2.0-beta9.jar;" & strTmpLib & "org\apache\logging\log4j\log4j-core\2.0-beta9\log4j-core-2.0-beta9.jar;" & strTmpLib & "org\lwjgl\lwjgl\lwjgl\2.9.1\lwjgl-2.9.1.jar;" & strTmpLib & "org\lwjgl\lwjgl\lwjgl_util\2.9.1\lwjgl_util-2.9.1.jar;" & strTmpLib & "tv\twitch\twitch\5.16\twitch-5.16.jar;" & strTmpLib & "tv\twitch\twitch\4.5\twitch-4.5.jar;" & Environment.CurrentDirectory & "\.minecraft\versions\" & "\" & strForgePath & "\" & strForgePath & ".jar" & """"
        strShell = """" & System.Environment.GetEnvironmentVariable("JAVA_HOME") & "\bin\javaw.exe" & """" & " -Xmx" & TextBoxMem.Text & "M" & strMcPara & strMcLibraries & " net.minecraft.launchwrapper.Launch  --username " & TextBoxUsername.Text & " --version " & strForgePath & " --gameDir .minecraft\versions\" & strForgePath & " --assetsDir .minecraft\assets --assetIndex 1.7.10 --uuid ${auth_uuid} --accessToken ${auth_access_token} --userProperties {} --userType Legacy --tweakClass cpw.mods.fml.common.launcher.FMLTweaker"
        TextBoxUsername.Text = strShell
  
        '  Debug.WriteLine(CMD(strShell))
    End Sub
    Function CMD(ByVal Data As String) As String
        Try
            Dim p As New Process()
            p.StartInfo.FileName = "cmd.exe"
            p.StartInfo.UseShellExecute = False
            p.StartInfo.RedirectStandardInput = True
            p.StartInfo.RedirectStandardOutput = True
            p.StartInfo.RedirectStandardError = True
            p.StartInfo.CreateNoWindow = True
            p.Start()
            Application.DoEvents()
            p.StandardInput.WriteLine(Data)
            p.StandardInput.WriteLine("Exit")
            Dim strRst As String = p.StandardOutput.ReadToEnd()
            p.Close()
            Return strRst
        Catch ex As Exception
            Return ""
        End Try
    End Function
End Class
