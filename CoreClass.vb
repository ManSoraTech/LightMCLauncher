' ________    _______       ________      ___           ________      ________     
'|\   __  \  |\  ___ \     |\   __  \    |\  \         |\   __  \    |\   __  \    
'\ \  \|\  \ \ \   __/|    \ \  \|\  \   \ \  \        \ \  \|\  \   \ \  \|\ /_   
' \ \   ____\ \ \  \_|/__   \ \  \\\  \   \ \  \        \ \   __  \   \ \   __  \  
'  \ \  \___|  \ \  \_|\ \   \ \  \\\  \   \ \  \____    \ \  \ \  \   \ \  \|\  \ 
'   \ \__\      \ \_______\   \ \_______\   \ \_______\   \ \__\ \__\   \ \_______\
'    \|__|       \|_______|    \|_______|    \|_______|    \|__|\|__|    \|_______|
'powered by 拖拉姬

Imports System.IO
Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Security.Cryptography
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace PeoLeser.Minecraft
    Public Class CoreClass

        Public Shared Function Core(ByVal FunctionMode As Integer, Optional ByVal AvailableMem As Integer = 4096, Optional ByVal FullVersion As String = "", Optional ByVal Username As String = "user", Optional ByVal CustomParameter As String = "")
            Dim json As String() = File.ReadAllText(Directory.GetCurrentDirectory() + "\.minecraft\versions\" + FullVersion + "\" + FullVersion + ".json").Split(New String() {Chr(34)}, StringSplitOptions.RemoveEmptyEntries)
            Select Case FunctionMode
                Case 0

                    Dim strProxy As String = ""
                    If MainForm.chbSocks5.CheckState = CheckState.Checked Then
                        'strProxy = " -DsocksProxyHost=" + Chr(34) + MainForm.tbxSocks5Host.Text + Chr(34) + " -DsocksProxyPort=" + Chr(34) + MainForm.tbxSocks5Port.Text + Chr(34)
                    End If
                    Dim dirAimPathTemp As New IO.DirectoryInfo(Application.StartupPath & "\.minecraft\versions\"), strAimPath As String, strForgeVersion As String, strDefaultPara As String
                    'get Forge path
                    strForgeVersion = dirAimPathTemp.GetDirectories.GetValue(0).ToString
                    strAimPath = dirAimPathTemp.ToString & strForgeVersion & "\"
                    Dim strMcPara As String, strLibPath As String = Application.StartupPath & "\.minecraft\libraries\", strShell As String
                    Dim strTmpLib As String = Application.StartupPath & "\.minecraft\libraries\"
                    'set parameter
                    'strDefaultPara = " -XX:-UseVMInterruptibleIO -XX:NewRatio=3 -XX:+UseConcMarkSweepGC -XX:+UseParNewGC -XX:+CMSIncrementalPacing -XX:+AggressiveOpts -XX:+UseFastAccessorMethods -XX:+UseBiasedLocking -XX:+CMSParallelRemarkEnabled -XX:MaxGCPauseMillis=50 -XX:+UseAdaptiveGCBoundary -XX:-UseGCOverheadLimit -XX:SurvivorRatio=8 -XX:TargetSurvivorRatio=90 -XX:MaxTenuringThreshold=15 -XX:+DisableExplicitGC -Xnoclassgc -oss4M -ss4M -XX:CMSInitiatingOccupancyFraction=60 -XX:SoftRefLRUPolicyMSPerMB=2048 -Xms800M -XX:ParallelGCThreads=" & System.Environment.ProcessorCount & " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true "
                    strDefaultPara = " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true "
                    strMcPara = strproxy + strDefaultPara & CustomParameter & "-Djava.library.path=" & Chr(34) & ".minecraft\native" & Chr(34)
                    Dim MainClass As String, MinecraftJar As String
                    MinecraftJar = Application.StartupPath + "\.minecraft\versions\" + FullVersion + "\" + FullVersion + ".jar"
                    MainClass = json(Array.IndexOf(json, "mainClass") + 2)

                    strShell = strMcPara + " -Xmx" & AvailableMem.ToString & "M" & " -cp " & Chr(34) & Core("1", , FullVersion, , ) + ";" + MinecraftJar & Chr(34) & " " & MainClass & " " & Core("2", , FullVersion, Username, )
                    'strShell = Chr(34) + JavaPath + Chr(34) + " -Xmx" & AvailableMem.ToString & "M" & strMcPara & " -cp " & Chr(34) & Core("1", , FullVersion, , ) + ";" + MinecraftJar & Chr(34) & " " & MainClass & " " & Core("2", , FullVersion, Username, )
                    Return strShell


                Case 1 'GetMinecraftLibrariesFiles
                    Dim i As Integer = 0
                    Dim MinecraftLibrariesFiles As String = ""
                    Dim RightString As String, LeftString As String, MiddleNumber As Integer, natives As String
                    For i = 0 To UBound(json)
                        Try
                            'If json(i) = "name" And json(i + 4) = "natives" And json(i - 2) <> "os" Then
                            '    For p = i + 2 To UBound(json)
                            '        If json(p) = "windows" Then
                            '            natives = json(p + 2)
                            '            Try
                            '                MiddleNumber = InStr(json(i + 2), ":")
                            '                LeftString = Replace(Left(json(i + 2), MiddleNumber - 1), ".", "\")
                            '                RightString = Right(json(i + 2), json(i + 2).Length - MiddleNumber)
                            '                MinecraftLibrariesFiles += ";" + Application.StartupPath + "\.minecraft\libraries\" + LeftString + "\" + Replace(RightString, ":", "\") + "\" + Replace(RightString, ":", "-") + "-" + Replace(natives, "${arch}", System.Runtime.InteropServices.Marshal.SizeOf(IntPtr.Zero) * 8) + ".jar"
                            '            Catch ex As ArgumentException
                            '            End Try
                            '        End If
                            '    Next

                            'End If


                            If json(i) = "name" And (json(i + 4) <> "rules" And json(i + 4) <> "natives") Then
                                MiddleNumber = InStr(json(i + 2), ":")
                                LeftString = Replace(Left(json(i + 2), MiddleNumber - 1), ".", "\")
                                RightString = Right(json(i + 2), json(i + 2).Length - MiddleNumber)
                                MinecraftLibrariesFiles += ";" + Application.StartupPath + "\.minecraft\libraries\" + LeftString + "\" + Replace(RightString, ":", "\") + "\" + Replace(RightString, ":", "-") + ".jar"
                            End If
                        Catch ex As IndexOutOfRangeException
                        End Try
                    Next

                    MinecraftLibrariesFiles = Strings.Right(MinecraftLibrariesFiles, MinecraftLibrariesFiles.Length - 1)
                    'MinecraftLibrariesFiles = Right(MinecraftLibrariesFiles, Len(MinecraftLibrariesFiles) - 1)
                    Return MinecraftLibrariesFiles

                Case 2 'GetMinecraftArguments
                    Dim MinecraftArguments As String, version As String = json(Array.IndexOf(json, "assets") + 2)

                    For Each json2 As String In json
                        If json2.Contains("--") Then
                            MinecraftArguments = MinecraftArguments + " " + json2
                            Exit For
                        End If

                    Next
                    MinecraftArguments = MinecraftArguments.Replace("${game_directory}", Application.StartupPath + "\.minecraft\versions\" & FullVersion)
                    MinecraftArguments = MinecraftArguments.Replace("${assets_root}", ".minecraft\assets")
                    MinecraftArguments = MinecraftArguments.Replace("${game_assets}", ".minecraft\assets")
                    MinecraftArguments = MinecraftArguments.Replace("${user_type}", "Legacy")
                    MinecraftArguments = MinecraftArguments.Replace("${user_properties}", "{}")
                    MinecraftArguments = MinecraftArguments.Replace("${auth_player_name}", Username)
                    MinecraftArguments = MinecraftArguments.Replace("${version_name}", FullVersion)
                    If MinecraftArguments.Contains("${assets_index_name}") Then
                        MinecraftArguments = MinecraftArguments.Replace("${assets_index_name}", version)
                    End If

                    Return MinecraftArguments
            End Select
        End Function

        Public Shared Function GetShell(Optional ByVal AvailableMem As Integer = 4096, Optional ByVal FullVersion As String = "", Optional ByVal Username As String = "user", Optional ByVal CustomParameter As String = "")
            Dim MinecraftLibrariesFiles As String = ""
            Dim strDefaultPara As String
            Dim strMcPara As String
            strDefaultPara = " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true "
            strMcPara = strDefaultPara & CustomParameter & "-Djava.library.path=" & Chr(34) & ".minecraft\native" & Chr(34)

            Dim strFullJson As String = Replace(Replace(File.ReadAllText(Directory.GetCurrentDirectory() + "\.minecraft\versions\" + FullVersion + "\" + FullVersion + ".json"), vbCrLf, ""), vbTab, "")
            Dim objFullJson As Object = DirectCast(JsonConvert.DeserializeObject(strFullJson), JObject)

            For i = 0 To objFullJson.item("libraries").count - 1
                Try
                    Dim RightString As String, LeftString As String, MiddleNumber As Integer, natives As String
                    Dim json As String = objFullJson.item("libraries").item(i).item("name").ToString
                    'MsgBox(json)
                    MiddleNumber = InStr(json, ":")
                    LeftString = Replace(Left(json, MiddleNumber - 1), ".", "\")
                    RightString = Right(json, json.Length - MiddleNumber)
                    MinecraftLibrariesFiles += ";" + Application.StartupPath + "\.minecraft\libraries\" + LeftString + "\" + Replace(RightString, ":", "\") + "\" + Replace(RightString, ":", "-") + ".jar"
                Catch ex As ArgumentOutOfRangeException
                    MsgBox(ex.Message)
                End Try
            Next
            MinecraftLibrariesFiles = Strings.Right(MinecraftLibrariesFiles, MinecraftLibrariesFiles.Length - 1)

            Dim MinecraftArguments = objFullJson.item("minecraftArguments").ToString  '获取MinecraftArguments

            Dim version As String = objFullJson.item("assets").ToString
            MinecraftArguments = MinecraftArguments.Replace("${game_directory}", Application.StartupPath + "\.minecraft\versions\" & FullVersion)
            MinecraftArguments = MinecraftArguments.Replace("${assets_root}", ".minecraft\assets")
            MinecraftArguments = MinecraftArguments.Replace("${game_assets}", ".minecraft\assets")
            MinecraftArguments = MinecraftArguments.Replace("${user_type}", "Legacy")
            MinecraftArguments = MinecraftArguments.Replace("${user_properties}", "{}")
            MinecraftArguments = MinecraftArguments.Replace("${auth_player_name}", Username)
            MinecraftArguments = MinecraftArguments.Replace("${version_name}", FullVersion)
            If MinecraftArguments.Contains("${assets_index_name}") Then
                MinecraftArguments = MinecraftArguments.Replace("${assets_index_name}", Version)
            End If

            Dim mainClass As String = objFullJson.item("mainClass").ToString
            Dim MinecraftJar As String = Application.StartupPath + "\.minecraft\versions\" + FullVersion + "\" + FullVersion + ".jar"
            Dim strShell As String

            strShell = strMcPara + " -Xmx" & AvailableMem.ToString & "M" & " -cp " & Chr(34) + MinecraftLibrariesFiles + ";" + MinecraftJar & Chr(34) & " " & mainClass & " " + MinecraftArguments

            Return strShell
        End Function

        Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Int32, ByVal lpFileName As String) As Int32
        Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Int32

        Public Shared Function GetINI(ByVal Section As String, ByVal AppName As String, ByVal lpDefault As String, ByVal FileName As String) As String
            Dim Str As String = LSet(Str, 256)
            GetPrivateProfileString(Section, AppName, lpDefault, Str, Len(Str), FileName)
            Return Left(Str, InStr(Str, Chr(0)) - 1)
        End Function

        Public Shared Function WriteINI(ByVal Section As String, ByVal AppName As String, ByVal lpDefault As String, ByVal FileName As String) As Long
            WriteINI = WritePrivateProfileString(Section, AppName, lpDefault, FileName)
        End Function

        Public Shared Function getMd5Hash(ByVal filePath As String) As String
            Try
                '创建MD5实例  
                Dim md5Hasher As MD5 = MD5.Create()

                '以字节形式读取文件  
                Dim originalDate As Byte() = My.Computer.FileSystem.ReadAllBytes(filePath)

                '计算MD5，Imports System.Security.Cryptography  
                Dim data As Byte() = md5Hasher.ComputeHash(originalDate)

                '创建可变字符串，Imports System.Text  
                Dim sBuilder As New System.Text.StringBuilder()

                ' 连接每一个 byte 的 hash 码，并格式化为十六进制字符串  
                Dim i As Integer
                For i = 0 To data.Length - 1
                    sBuilder.Append(data(i).ToString("x2"))
                Next i

                Return sBuilder.ToString
            Catch ex As FileNotFoundException
                Return "1"
            Catch ex As DirectoryNotFoundException
                Return "1"
            End Try
        End Function
    End Class
End Namespace



Namespace eMZi.Gaming.Minecraft

    'thanks to mcmny@mcbbs & eMZi@github

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
                Return New Dictionary(Of Char, String)() From {
                 {"0"c, "#000000"},
                 {"1"c, "#0000AA"},
                 {"2"c, "#00AA00"},
                 {"3"c, "#00AAAA"},
                 {"4"c, "#AA0000"},
                 {"5"c, "#AA00AA"},
                 {"6"c, "#FFAA00"},
                 {"7"c, "#AAAAAA"},
                 {"8"c, "#555555"},
                 {"9"c, "#5555FF"},
                 {"a"c, "#55FF55"},
                 {"b"c, "#55FFFF"},
                 {"c"c, "#FF5555"},
                 {"d"c, "#FF55FF"},
                 {"e"c, "#FFFF55"},
                 {"f"c, "#FFFFFF"}
                }
            End Get
        End Property


        Private Shared ReadOnly Property MinecraftStyles() As Dictionary(Of Char, String)
            Get
                Return New Dictionary(Of Char, String)() From {
                 {"k"c, "none;font-weight:normal;font-style:normal"},
                 {"m"c, "line-through;font-weight:normal;font-style:normal"},
                 {"l"c, "none;font-weight:900;font-style:normal"},
                 {"n"c, "underline;font-weight:normal;font-style:normal;"},
                 {"o"c, "none;font-weight:normal;font-style:italic;"},
                 {"r"c, "none;font-weight:normal;font-style:normal;color:#FFFFFF;"}
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
