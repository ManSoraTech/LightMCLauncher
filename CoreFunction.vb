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

Public Class CoreFunction
    Shared Function Core(ByVal FunctionMode As String, AvailableMem As Integer, FullVersion As String, Username As String, CustomParameter As String)
        Dim json As String() = File.ReadAllText(Directory.GetCurrentDirectory() + "\.minecraft\versions\" + FullVersion + "\" + FullVersion + ".json").Split(New String() {Chr(34)}, StringSplitOptions.RemoveEmptyEntries)
        Select Case FunctionMode
            Case "0"

                Dim dirAimPathTemp As New IO.DirectoryInfo(Application.StartupPath & "\.minecraft\versions\"), strAimPath As String, strForgeVersion As String, strDefaultPara As String
                'get Forge path
                strForgeVersion = dirAimPathTemp.GetDirectories.GetValue(0).ToString
                strAimPath = dirAimPathTemp.ToString & strForgeVersion & "\"
                Dim strMcPara As String, strLibPath As String = Application.StartupPath & "\.minecraft\libraries\", strShell As String
                Dim strTmpLib As String = Application.StartupPath & "\.minecraft\libraries\"
                'set parameter
                strDefaultPara = " -XX:-UseVMInterruptibleIO -XX:NewRatio=3 -XX:+UseStringCache -XX:+UseConcMarkSweepGC -XX:+UseParNewGC -XX:+CMSIncrementalPacing -XX:+AggressiveOpts -XX:+UseFastAccessorMethods -XX:+UseBiasedLocking -XX:PermSize=128m -XX:MaxPermSize=256m -XX:+CMSParallelRemarkEnabled -XX:MaxGCPauseMillis=50 -XX:+UseAdaptiveGCBoundary -XX:-UseGCOverheadLimit -XX:SurvivorRatio=8 -XX:TargetSurvivorRatio=90 -XX:MaxTenuringThreshold=15 -XX:+UseAdaptiveSizePolicy -XX:+DisableExplicitGC -Xnoclassgc -oss4M -ss4M -XX:CMSInitiatingOccupancyFraction=60 -XX:+UseCMSCompactAtFullCollection -XX:CMSFullGCsBeforeCompaction=1 -XX:SoftRefLRUPolicyMSPerMB=2048 -Xms800M -XX:ParallelGCThreads=" & System.Environment.ProcessorCount & " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true "
                strMcPara = strDefaultPara & CustomParameter & " -Djava.library.path=" & Chr(34) & ".minecraft\Native" & Chr(34)
                Dim MainClass As String, MinecraftJar As String
                MinecraftJar = Application.StartupPath + "\.minecraft\versions\" + FullVersion + "\" + FullVersion + ".jar"
                MainClass = json(Array.IndexOf(json, "mainClass") + 2)

                strShell = Chr(34) & "java.exe" & Chr(34) & " -Xmx" & AvailableMem.ToString & "M" & strMcPara & " -cp " & Chr(34) & Core("1", 0, FullVersion, "", "") & MinecraftJar & Chr(34) & " " & MainClass & " " & Core("2", 0, FullVersion, Username, "")
                Return strShell


            Case "1" 'GetMinecraftLibrariesFiles
                Dim i As Integer = 0
                Dim MinecraftLibrariesFiles As String
                For Each Keyword As String In json
                    Dim RightString As String, LeftString As String, MiddleNumber As Integer
                    i += 1
                    If Keyword = "name" Then
                        MiddleNumber = InStr(json(i + 1), ":")
                        LeftString = Strings.Left(json(i + 1), MiddleNumber - 1)
                        RightString = Strings.Right(json(i + 1), json(i + 1).Length - MiddleNumber)
                        LeftString = Replace(LeftString, ".", "\")
                        Dim RightString2() = RightString.Split({Chr(58)})
                        MinecraftLibrariesFiles = Application.StartupPath + "\.minecraft\libraries\" + LeftString + "\" + RightString2(0) + "\" + RightString2(1) + "\" + RightString2(0) + "-" + RightString2(1) + ".jar" + ";" + MinecraftLibrariesFiles

                    End If
                Next
                Return MinecraftLibrariesFiles

            Case "2" 'GetMinecraftArguments
                Dim MinecraftArguments As String, version As String = json(Array.IndexOf(json, "assets") + 2)

                For Each json2 As String In json
                    If json2.Contains("--") Then
                        MinecraftArguments = MinecraftArguments + " " + json2
                        Exit For
                    End If

                Next
                MinecraftArguments = MinecraftArguments.Replace("${game_directory}", ".minecraft\versions\" & FullVersion)
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
End Class
