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

Public Class Main

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles Me.Load

        Try
        Shell("java", AppWinStyle.Hide)
        Catch NoJava As FileNotFoundException
            MessageBox.Show("Cannot find Java.exe!Please set java path manually.")
        End Try

        'Get Reg
        If Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True) IsNot Nothing Then
            TextBoxUsername.Text = Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True).GetValue("Username")
            TextBoxParameter.Text = Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True).GetValue("Parameter")
        End If

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
        Dim b As New IPEndPoint(MCServerIPAddress, 25565)
        Try
            MCServerInfo = eMZi.Gaming.Minecraft.MinecraftServerInfo.GetServerInformation(b)
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
        ' MessageBox.Show("""" & System.Environment.GetEnvironmentVariable("JAVA_HOME") & "\bin\javaw.exe" & """" & " -Xmx" & TextBoxMem.Text & "M" & strMcPara & strMcLibraries & "net.minecraft.launchwrapper.Launch  --username " & TextBoxUsername.Text & " --version " & strForgePath & " --gameDir .minecraft\versions\" & strForgePath & " --assetsDir .minecraft\assets --assetIndex 1.7.10 --uuid ${auth_uuid} --accessToken ${auth_access_token} --userProperties {} --userType Legacy --tweakClass cpw.mods.fml.common.launcher.FMLTweaker")
    End Sub

    Private Sub TimerAutoKill_Tick(sender As Object, e As EventArgs) Handles TimerAutoKill.Tick
        Process.GetCurrentProcess().Kill()
    End Sub


    Private Sub ButtonRunMc_Click(sender As Object, e As EventArgs) Handles ButtonRunMc.Click
        SetReg()
        'Shell(GetMcPath(), AppWinStyle.NormalNoFocus)
        RunMc()
        Application.Exit()
    End Sub

#Region "GetMcPath|set MC launch command"
    Private Function GetMcPath()
        Dim dirAimPathTemp As New IO.DirectoryInfo(Environment.CurrentDirectory & "\.minecraft\versions\"), strAimPath As String, strForgePath As String, strDefaultPara As String
        'get Forge path
        strForgePath = dirAimPathTemp.GetDirectories.GetValue(0).ToString
        strAimPath = dirAimPathTemp.ToString & strForgePath & "\"
        Dim strMcPara As String, strLibPath As String = Environment.CurrentDirectory & "\.minecraft\libraries\", strShell As String
        Dim strTmpLib As String = Environment.CurrentDirectory & "\.minecraft\libraries\"
        'set parameter
        strDefaultPara = " -XX:-UseVMInterruptibleIO -XX:NewRatio=3 -XX:+UseStringCache -XX:+UseConcMarkSweepGC -XX:+UseParNewGC -XX:+CMSIncrementalPacing -XX:+AggressiveOpts -XX:+UseFastAccessorMethods -XX:+UseBiasedLocking -XX:PermSize=128m -XX:MaxPermSize=256m -XX:+CMSParallelRemarkEnabled -XX:MaxGCPauseMillis=50 -XX:+UseAdaptiveGCBoundary -XX:-UseGCOverheadLimit -XX:SurvivorRatio=8 -XX:TargetSurvivorRatio=90 -XX:MaxTenuringThreshold=15 -XX:+UseAdaptiveSizePolicy -XX:+DisableExplicitGC -Xnoclassgc -oss4M -ss4M -XX:CMSInitiatingOccupancyFraction=60 -XX:+UseCMSCompactAtFullCollection -XX:CMSFullGCsBeforeCompaction=1 -XX:SoftRefLRUPolicyMSPerMB=2048 -Xms800M -XX:ParallelGCThreads=" & System.Environment.ProcessorCount & " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true "
        strMcPara = strDefaultPara & TextBoxParameter.Text & " -Djava.library.path=" & Chr(34) & ".minecraft\natives" & Chr(34) & " -cp "
        'set command
        strShell = Chr(34) & GetJavaHome() & Chr(34) & " -Xmx" & TextBoxAvailableMem.Text & "M" & strMcPara & Chr(34) & GetLibrariesFiles() & Environment.CurrentDirectory & "\.minecraft\versions\" & strForgePath & "\" & strForgePath & ".jar" & Chr(34) & " net.minecraft.launchwrapper.Launch  --username " & TextBoxUsername.Text & " --version " & strForgePath & " --gameDir .minecraft\versions\" & strForgePath & " --assetsDir .minecraft\assets --assetIndex 1.7.10 --uuid ${auth_uuid} --accessToken ${auth_access_token} --userProperties {} --userType Legacy --tweakClass cpw.mods.fml.common.launcher.FMLTweaker"
        Return strShell
    End Function
#End Region

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

        Dim swRunMc As StreamWriter
        swRunMc = File.CreateText(System.Environment.GetEnvironmentVariable("temp") & "\runmc.bat")
        swRunMc.WriteLine(GetMcPath() & Chr(13) + Chr(10) & "exit")
        swRunMc.Close()
        swRunMc = File.CreateText(System.Environment.GetEnvironmentVariable("temp") & "\runmc.vbs")
        swRunMc.WriteLine("set ws=wscript.createobject(" & """" & "wscript.shell" & """" & ") " & Chr(13) + Chr(10) & "ws.run " & """" & System.Environment.GetEnvironmentVariable("temp") & "\runmc.bat" & """" & ",0")
        swRunMc.Close()
        Process.Start(System.Environment.GetEnvironmentVariable("temp") & "\run.vbs")

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

#Region "get Libraries list"
    Function GetLibrariesFiles()
        'Function GetLibrariesFiles(ByVal strLibPath As String)
        'Dim strLibFiles() As String, i As Integer, strMcLibraries As String
        'strLibFiles = IO.Directory.GetFiles(strLibPath, "*.jar", SearchOption.AllDirectories)
        'Do Until i = strLibFiles.Length
        '    strMcLibraries = strMcLibraries & strLibFiles(i) & ";"
        '    i += 1
        'Loop
        'Return strMcLibraries
        Dim libraries2(100) As String
        Dim mi As Integer = 0
        Dim libraries1 As Integer = 0
        Dim a As String
        Dim json As String
        Dim json1 As String()
        json = File.ReadAllText(Directory.GetCurrentDirectory() + "\.minecraft\versions\" + "1.7.10-Forge10.13.2.1291" + "\" + "1.7.10-Forge10.13.2.1291" + ".json")
        json1 = json.Split(New String() {Chr(34)}, StringSplitOptions.RemoveEmptyEntries)
        For Each m As String In json1
            If m = "name" Then
                mi = 4
            End If
            If mi > 0 Then
                mi = mi - 1
            End If
            If mi = 1 And m.Contains(".") And m.Contains(":") Then

                Dim mlj As String
                Dim mname As String
                Dim mmh As Integer = InStr(m, ":")
                mlj = Strings.Left(m, mmh - 1)
                mlj = Directory.GetCurrentDirectory() + "\.minecraft\libraries\" + mlj.Replace(".", "\")
                mname = Strings.Right(m, m.Length - mmh + 1)
                mlj = mlj + mname.Replace(":", "\") + "\"
                mname = Strings.Right(m, m.Length - mmh)
                mname = mname.Replace(":", "-") + ".jar;"


                libraries2(libraries1) = mlj.Replace(Directory.GetCurrentDirectory() + "\.minecraft\libraries\", "") + mname.Replace(";", "")

                libraries1 = libraries1 + 1
                a = a + mlj + mname
            End If
        Next
        Return a
    End Function
#End Region


    Private Sub ButtonDefaultParameter_Click(sender As Object, e As EventArgs) Handles ButtonDefaultParameter.Click
        MessageBox.Show("-XX:-UseVMInterruptibleIO -XX:NewRatio=3 -XX:+UseStringCache -XX:+UseConcMarkSweepGC -XX:+UseParNewGC -XX:+CMSIncrementalPacing -XX:+AggressiveOpts -XX:+UseFastAccessorMethods -XX:+UseBiasedLocking -XX:PermSize=128m -XX:MaxPermSize=256m -XX:+CMSParallelRemarkEnabled -XX:MaxGCPauseMillis=50 -XX:+UseAdaptiveGCBoundary -XX:-UseGCOverheadLimit -XX:SurvivorRatio=8 -XX:TargetSurvivorRatio=90 -XX:MaxTenuringThreshold=15 -XX:+UseAdaptiveSizePolicy -XX:+DisableExplicitGC -Xnoclassgc -oss4M -ss4M -XX:CMSInitiatingOccupancyFraction=60 -XX:+UseCMSCompactAtFullCollection -XX:CMSFullGCsBeforeCompaction=1 -XX:SoftRefLRUPolicyMSPerMB=2048 -Xms800M -XX:ParallelGCThreads=" & System.Environment.ProcessorCount & " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true ")
    End Sub


    Private Sub TimerRefreshInfo_Tick(sender As Object, e As EventArgs) Handles TimerRefreshInfo.Tick
        'get MEM
        LabelTotalMemNum.Text = Int(My.Computer.Info.TotalPhysicalMemory / 1024 / 1024) & " M"
        LabelAvailableMemNum.Text = Int(My.Computer.Info.AvailablePhysicalMemory / 1024 / 1024) & " M"
        TextBoxAvailableMem.Text = Int(My.Computer.Info.AvailablePhysicalMemory / 1024 / 1024 * 0.9)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("http://tieba.baidu.com/home/main?id=bb13383737303438373634bb09&fr=userbar")
    End Sub
End Class

Namespace eMZi.Gaming.Minecraft

    ' thanks to mcmny@mcbbs & eMZi@github

    Public NotInheritable Class MinecraftServerInfo

        Public Property ServerMotd() As String
            ' Gets the server's MOTD
            Get
                Return m_ServerMotd
            End Get
            Private Set(ByVal value As String)
                m_ServerMotd = value
            End Set
        End Property
        Private m_ServerMotd As String


        Public ReadOnly Property ServerMotdHtml() As String
            ' Gets the server's MOTD converted into HTML
            Get
                Return Me.MotdHtml()
            End Get
        End Property


        Public Property MaxPlayerCount() As Integer
            ' Gets the server's max player count
            Get
                Return m_MaxPlayerCount
            End Get
            Private Set(ByVal value As Integer)
                m_MaxPlayerCount = value
            End Set
        End Property
        Private m_MaxPlayerCount As Integer

        Public Property CurrentPlayerCount() As Integer
            ' Gets the server's current player count
            Get
                Return m_CurrentPlayerCount
            End Get
            Private Set(ByVal value As Integer)
                m_CurrentPlayerCount = value
            End Set
        End Property
        Private m_CurrentPlayerCount As Integer

        Public Property MinecraftVersion() As String
            ' Gets the server's Minecraft version
            Get
                Return m_MinecraftVersion
            End Get
            Private Set(ByVal value As String)
                m_MinecraftVersion = value
            End Set
        End Property
        Private m_MinecraftVersion As String


        Private Shared ReadOnly Property MinecraftColors() As Dictionary(Of Char, String)
            ' Gets HTML colors associated with specific formatting codes
            Get
                Return New Dictionary(Of Char, String)() From { _
                 {"0"c, "#000000"}, _
                 {"1"c, "#0000AA"}, _
                 {"2"c, "#00AA00"}, _
                 {"3"c, "#00AAAA"}, _
                 {"4"c, "#AA0000"}, _
                 {"5"c, "#AA00AA"}, _
                 {"6"c, "#FFAA00"}, _
                 {"7"c, "#AAAAAA"}, _
                 {"8"c, "#555555"}, _
                 {"9"c, "#5555FF"}, _
                 {"a"c, "#55FF55"}, _
                 {"b"c, "#55FFFF"}, _
                 {"c"c, "#FF5555"}, _
                 {"d"c, "#FF55FF"}, _
                 {"e"c, "#FFFF55"}, _
                 {"f"c, "#FFFFFF"} _
                }
            End Get
        End Property


        Private Shared ReadOnly Property MinecraftStyles() As Dictionary(Of Char, String)
            Get
                Return New Dictionary(Of Char, String)() From { _
                 {"k"c, "none;font-weight:normal;font-style:normal"}, _
                 {"m"c, "line-through;font-weight:normal;font-style:normal"}, _
                 {"l"c, "none;font-weight:900;font-style:normal"}, _
                 {"n"c, "underline;font-weight:normal;font-style:normal;"}, _
                 {"o"c, "none;font-weight:normal;font-style:italic;"}, _
                 {"r"c, "none;font-weight:normal;font-style:normal;color:#FFFFFF;"} _
                }
            End Get
        End Property


        Private Sub New(ByVal motd As String, ByVal maxplayers As Integer, ByVal playercount As Integer, ByVal mcversion As String)
            Me.ServerMotd = motd
            Me.MaxPlayerCount = maxplayers
            Me.CurrentPlayerCount = playercount
            Me.MinecraftVersion = mcversion
        End Sub


        Private Function MotdHtml() As String
            Dim regex As New Regex("§([k-oK-O])(.*?)(§[0-9a-fA-Fk-oK-OrR]|$)")
            Dim s As String = Me.ServerMotd
            While regex.IsMatch(s)
                s = regex.Replace(s, Function(m)
                                         Dim ast As String = "text-decoration:" & MinecraftStyles(m.Groups(1).Value(0))
                                         Dim html As String = "<span style=""" & ast & """>" & m.Groups(2).Value & "</span>" & m.Groups(3).Value
                                         Return html

                                     End Function)
            End While
            regex = New Regex("§([0-9a-fA-F])(.*?)(§[0-9a-fA-FrR]|$)")
            While regex.IsMatch(s)
                s = regex.Replace(s, Function(m)
                                         Dim ast As String = "color:" & MinecraftColors(m.Groups(1).Value(0))
                                         Dim html As String = "<span style=""" & ast & """>" & m.Groups(2).Value & "</span>" & m.Groups(3).Value
                                         Return html

                                     End Function)
            End While
            Return s
        End Function


        Public Shared Function GetServerInformation(ByVal endpoint As IPEndPoint) As MinecraftServerInfo
            If endpoint Is Nothing Then
                Throw New ArgumentNullException("endpoint")
            End If
            Try
                Dim packetdat As String() = Nothing
                Using client As New TcpClient()
                    client.Connect(endpoint)
                    Using ns As NetworkStream = client.GetStream()
                        ns.Write(New Byte() {&HFE, &H1}, 0, 2)
                        Dim buff As Byte() = New Byte(2047) {}
                        Dim br As Integer = ns.Read(buff, 0, buff.Length)
                        If buff(0) <> &HFF Then
                            Throw New InvalidDataException("Received invalid packet")
                        End If
                        Dim packet As String = Encoding.BigEndianUnicode.GetString(buff, 3, br - 3)
                        If Not packet.StartsWith("§") Then
                            Throw New InvalidDataException("Received invalid data")
                        End If
                        packetdat = packet.Split(ControlChars.NullChar)
                        ns.Close()
                    End Using
                    client.Close()
                End Using
                Return New MinecraftServerInfo(packetdat(3), Integer.Parse(packetdat(5)), Integer.Parse(packetdat(4)), (packetdat(2)))
            Catch ex As SocketException
                'MessageBox.Show("Connected server failed")
                'Throw New Exception("There was a connection problem, look into InnerException for details", ex)
            Catch ex As InvalidDataException
                Throw New Exception("The data received was invalid, look into InnerException for details", ex)
            Catch ex As Exception
                Throw New Exception("There was a problem, look into InnerException for details", ex)
            End Try
        End Function

        Public Shared Function GetServerInformation(ByVal ip As IPAddress, ByVal port As Integer) As MinecraftServerInfo
            Return GetServerInformation(New IPEndPoint(ip, port))
        End Function
    End Class
End Namespace
