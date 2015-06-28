Imports System.Net
Imports System.IO
Imports System.Threading
Imports System.xml 

Public Class MinecraftUpdate

    Dim DownThread As Thread
    Private Delegate Sub voidDelegate(ByRef totalDownloadedByte As Long, totalBytes As Long)

    Private Sub MinecraftUpdate_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MetroLabel.Left = (Me.Width - MetroLabel.Width) \ 2
        MetroProgressBar.Left = (Me.Width - MetroProgressBar.Width) \ 2
        MetroProgressBarAll.Left = (Me.Width - MetroProgressBar.Width) \ 2
        Control.CheckForIllegalCrossThreadCalls = False
        DownThread = New Thread(New ThreadStart(AddressOf Update))
        DownThread.Start()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ' Update()

    End Sub

    Private Sub Update()

        Try
            Dim Myrq As HttpWebRequest = HttpWebRequest.Create(PeoLeser.Minecraft.CoreClass.GetINI("Launcher", "UpdateXML", "", Application.StartupPath + "\LightMCLauncher\launcher.ini"))
            Dim myrp As HttpWebResponse = Myrq.GetResponse
            Dim st As Stream = myrp.GetResponseStream
            Dim so As Stream = New FileStream(Application.StartupPath + "\LightMCLauncher\" + "update.xml", FileMode.Create)
            Dim by(1024) As Byte
            Dim osize As Integer = st.Read(by, 0, by.Length)
            While osize > 0
                Application.DoEvents()
                so.Write(by, 0, osize)
                osize = st.Read(by, 0, by.LongLength)
            End While
            so.Close()
            st.Close()

            Dim xmlDoc As New XmlDocument
            xmlDoc.Load(Application.StartupPath + "\LightMCLauncher\" + "update.xml")

            Dim UpdateLine As TextReader = File.OpenText(Application.StartupPath + "\LightMCLauncher\" + "update.xml")
            Dim UpdateLineAll As String = UpdateLine.ReadToEnd, LineCount As Int16 = 0
            LineCount = (UBound(Split(UpdateLineAll, vbCrLf)) - 2) / 6
            UpdateLine.Close()
            MetroProgressBarAll.Maximum = LineCount

            Dim UpdateInfo(LineCount, 3) As String
            For i = 1 To LineCount
                Try
                    UpdateInfo(i, 0) = xmlDoc.SelectSingleNode("update").SelectSingleNode("file" + i.ToString).SelectSingleNode("action").InnerText
                    UpdateInfo(i, 1) = xmlDoc.SelectSingleNode("update").SelectSingleNode("file" + i.ToString).SelectSingleNode("path").InnerText
                    UpdateInfo(i, 2) = xmlDoc.SelectSingleNode("update").SelectSingleNode("file" + i.ToString).SelectSingleNode("version").InnerText
                    UpdateInfo(i, 3) = xmlDoc.SelectSingleNode("update").SelectSingleNode("file" + i.ToString).SelectSingleNode("url").InnerText
                Catch ex As System.NullReferenceException
                End Try

            Next

            For i = 1 To LineCount
                Select Case UpdateInfo(i, 0)
                    Case "Creat"
                        If UpdateInfo(i, 2) <> PeoLeser.Minecraft.CoreClass.getMd5Hash(Application.StartupPath + "/" + UpdateInfo(i, 1)) Then
                            Dim Myrq2 As HttpWebRequest = HttpWebRequest.Create(PeoLeser.Minecraft.CoreClass.GetINI("Launcher", "UpdateURL", "", Application.StartupPath + "\LightMCLauncher\launcher.ini") + UpdateInfo(i, 3))
                            Dim myrp2 As HttpWebResponse = Myrq2.GetResponse
                            Dim totalBytes As Long = myrp2.ContentLength
                            Dim st2 As Stream = myrp2.GetResponseStream
                            Try
line1:                          Dim so2 As Stream = New FileStream(Application.StartupPath + "\" + UpdateInfo(i, 1), FileMode.Create)
                                Dim totalDownloadedByte As Long = 0
                                Dim by2(1024) As Byte
                                Dim osize2 As Integer = st2.Read(by2, 0, by2.Length)
                                While osize2 > 0
                                    totalDownloadedByte = osize2 + totalDownloadedByte
                                    Application.DoEvents()
                                    so2.Write(by2, 0, osize2)
                                    Me.Invoke(New voidDelegate(AddressOf UpdateUI), totalDownloadedByte, totalBytes)
                                    osize2 = st2.Read(by2, 0, by2.LongLength)
                                End While
                                so2.Close()
                                st2.Close()

                            Catch ex As DirectoryNotFoundException
                                'MsgBox(UpdateInfo(i, 1))
                                Directory.CreateDirectory(Strings.Left(UpdateInfo(i, 1), UpdateInfo(i, 1).LastIndexOf("\")))
                                GoTo line1
                            End Try

                        End If

                    Case "Delete"
                        Try
                            File.Delete(Application.StartupPath + "\" + UpdateInfo(i, 1))
                        Catch ex As Exception

                        End Try
                End Select

                MetroProgressBarAll.Value = i

            Next
        Catch ex As Exception
        End Try
        MetroProgressBarAll.Value = MetroProgressBarAll.Maximum
            MetroProgressBar.Value = MetroProgressBar.Maximum
        Me.Close()
    End Sub

    Private Sub UpdateUI(ByRef totalDownloadedByte As Long, totalBytes As Long)
        MetroProgressBar.Maximum = totalBytes
        MetroProgressBar.Value = totalDownloadedByte
    End Sub
End Class
