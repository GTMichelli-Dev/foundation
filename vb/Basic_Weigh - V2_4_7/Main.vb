Imports System.ComponentModel
Imports SMA_Command_LIB
Imports StreamScaleIP
Imports System.IO


Public Class Main

    Dim Instring As String = ""
    Public Wt As Integer
    Public Motion As Boolean
    Public MyScaleStatus As String
    Public EditThisTicket As Boolean

    Public Scale_Connection_Type As String
    Public Scale_IP_Address As String
    Public Scale_Network_Port As Int32
    Public Scale_Baud_Rate As Int32
    Public Scale_Word_Length As Int32
    Public Scale_Parity As Int32
    Public Scale_Stop_Bits As Int32
    Public Scale_Com_Port As String

    Public Display_Camera_URL As String
    Public Display_Local_Camera As String
    Public Display_Bitmap As String

    Public Pages_To_Print As Int32
    Public Tickets_Per_Page As Int32

    Public Hide_Tables As Boolean
    Public Stop_Light As Boolean
    Public Alert_Audio_File As String
    Public Ticket_Printer As String

    Dim SetupTableAdapter As New ScaleDataSetTableAdapters.SetupTableAdapter
    Dim SetupDataTable As New ScaleDataSet.SetupDataTable
    Dim ManualLightChange As Boolean
    Dim RedLight As Boolean
    Dim CancelAlarm As Boolean = False
    Dim SplitWeigh As Boolean = False
    Dim PupWeigh As Boolean = False

    Dim I As Integer
    Dim LastWeight As Integer = 0

    'This starts the sma scale interface
    Dim MySMA As New SMA_Command_LIB.SMA(My.Settings.IP_Address.ToString, Convert.ToInt32(My.Settings.Network_Port.ToString))
    'This starts the network scale interface
    Dim WithEvents StreamingScale As StreamScaleIP.StreamingScale


    Private Sub Main_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            btnChangeLight.Visible = My.Settings.StopLight
            PictureBox1.Visible = My.Settings.StopLight

            If My.Settings.HideTables = True Then

                Button4.Text = ""
                Button4.Enabled = False

            Else

                Button4.Text = "Edit Tables"
                Button4.Enabled = True

            End If

            ' Added by Matt Burkett  07/27/2016
            ' Cannot make the setting come across properly for the playlist so I hard coded it.
            Me.AxVLCPlugin21.playlist.stop()
            Me.AxVLCPlugin21.playlist.items.clear()
            Me.AxVLCPlugin21.Toolbar = False

            ' The first setting with text in it will be the one used to display the chosen picture or camera image

            If Len(My.Settings.Camera_Url.ToString) > 0 Then
                'To Capture a HikVision IP camera
                Me.AxVLCPlugin21.playlist.add("rtsp://" + My.Settings.Camera_Url)
                Me.pct_Image.Visible = False

            ElseIf Len(My.Settings.Local_Camera.ToString) > 0 Then

                'To Capture a USB camera  Place the camera name in the setting
                Me.AxVLCPlugin21.playlist.add(My.Settings.Local_Camera.ToString)
                Me.pct_Image.Visible = False


            ElseIf Len(My.Settings.BitMap.ToString) > 0 Then

                Me.AxVLCPlugin21.Visible = False     'Place the full path to the bitmap in the setting
                Me.pct_Image.Image = Image.FromFile(My.Settings.BitMap.ToString)
                Me.pct_Image.Visible = True

            Else

                Me.AxVLCPlugin21.Visible = False
                Me.pct_Image.Visible = False

            End If

            Me.AxVLCPlugin21.playlist.play()

            Me.Text = My.Application.Info.Title + " Version: " + Application.ProductVersion
            ChangeLight()

            SetupTableAdapter.Fill(SetupDataTable)

            EstablishConnection()

        Catch ex As Exception

            MessageBox.Show("An error occured: " + ex.Message)

        End Try

    End Sub


    Private Sub EstablishConnection()

        Try


            If My.Settings.ConnectionType = "Serial" Then

                'If the values are left blank the system will default to 9600, 8, N, 1
                Me.SerialPort1.PortName = My.Settings.ComPort
                If Len(My.Settings.Baud.ToString) > 0 Then Me.SerialPort1.BaudRate = Convert.ToInt32(My.Settings.Baud.ToString)
                If Len(My.Settings.Word.ToString) > 0 Then Me.SerialPort1.DataBits = My.Settings.Word

                If Len(My.Settings.Parity.ToString) > 0 Then    ' Set parity

                    'Look at the first character for the Parity setting.  If its an unusual parity setting it will be set correctly.
                    'The system will default to No Parity
                    If InStr("E", Mid(My.Settings.Parity.ToString.ToUpper, 1)) > 0 Then
                        Me.SerialPort1.Parity = IO.Ports.Parity.Even
                    ElseIf InStr("O", Mid(My.Settings.Parity.ToString.ToUpper, 1)) > 0 Then
                        Me.SerialPort1.Parity = IO.Ports.Parity.Odd
                    ElseIf InStr("M", Mid(My.Settings.Parity.ToString.ToUpper, 1)) > 0 Then
                        Me.SerialPort1.Parity = IO.Ports.Parity.Mark
                    ElseIf InStr("S", Mid(My.Settings.Parity.ToString.ToUpper, 1)) > 0 Then
                        Me.SerialPort1.Parity = IO.Ports.Parity.Space
                    Else
                        Me.SerialPort1.Parity = IO.Ports.Parity.None
                    End If

                End If

                If Len(My.Settings.StopBits.ToString) > 0 Then

                    If My.Settings.StopBits.ToString = "0" Then Me.SerialPort1.StopBits = IO.Ports.StopBits.None
                    If My.Settings.StopBits.ToString = "1" Then Me.SerialPort1.StopBits = IO.Ports.StopBits.One
                    If My.Settings.StopBits.ToString = "1.5" Then Me.SerialPort1.StopBits = IO.Ports.StopBits.OnePointFive
                    If My.Settings.StopBits.ToString = "2" Then Me.SerialPort1.StopBits = IO.Ports.StopBits.Two

                End If

                Me.SerialPort1.Open()

                'This control is visible normally.  If it reaches this point the serial port opened correctly.  So its turned off.

                Me.hsScaleBar.Visible = False

            ElseIf My.Settings.ConnectionType = "Network" Then

                Try
                    'Connect to Network
                    StreamingScale = New StreamingScale("TruckScale", My.Settings.IP_Address.ToString, Convert.ToInt32(My.Settings.Network_Port.ToString))
                    StreamingScale.Connect(300)
                    Me.hsScaleBar.Visible = False

                Catch ex As Exception
                    MessageBox.Show("Error Connecting VIA Network Connection ..." + ex.Message)
                End Try

            ElseIf My.Settings.ConnectionType = "SMA" Then

                'SMA Connection to ZM Series
                MySMA.Start(200) 'Starts the connection to the scale.
                Me.hsScaleBar.Visible = False

            ElseIf My.Settings.ConnectionType = "Simulate" Then

                Me.hsScaleBar.Visible = True

            End If

            tmrUpdate.Start()

        Catch ex As Exception
            Dim Error_Message As String

            If My.Settings.ConnectionType = "Serial" Then
                Error_Message = "Unable to open Port: " + My.Settings.ComPort
            Else
                Error_Message = "Unable to open Port: " + My.Settings.Network_Port
            End If

            Dim ComError As New Com_Error(Error_Message, ex.Message)
            ComError.ShowDialog()

            ComError.Close()

        End Try

    End Sub
    Private Sub SerialPort1_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
        Try

            Instring = Me.SerialPort1.ReadLine
            Dim WtString As String = ""

            For Each OneChr As Char In Instring

                If InStr("-1234567890", OneChr) Then
                    WtString += OneChr
                End If

            Next

            Try
                If WtString.Trim <> "" Then Me.Wt = CType(WtString, Integer)

            Catch ex As Exception
                Me.Wt = 0
            End Try
            Motion = InStr(Instring, "M")

            I += 1
            If I > 999 Then I = 1

        Catch ex As Exception


        End Try

    End Sub



    Private Sub tmrUpdate_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrUpdate.Tick

        If EditThisTicket = False Then

            If My.Settings.ConnectionType = "Network" Then

                If StreamingScale IsNot Nothing Then

                    Wt = StreamingScale.CurrentScaleData.CurWeight
                    Motion = StreamingScale.CurrentScaleData.Motion

                    If StreamingScale.ConnectionStatus = StreamingScale.enumConnectionStatus.Connecting Then MyScaleStatus = "Connecting..."

                    If StreamingScale.ConnectionStatus = StreamingScale.enumConnectionStatus.NotConnected Then MyScaleStatus = "Lost Connection"

                    If StreamingScale.ConnectionStatus = StreamingScale.enumConnectionStatus.Ok Then

                        If Motion Then
                            MyScaleStatus = "MOTION"
                        Else
                            MyScaleStatus = ""
                        End If

                    End If

                    Me.lblCnt.Text = StreamingScale.CurrentScaleData.CurrentStatusChar

                End If

            ElseIf My.Settings.ConnectionType = "SMA" Then

                Wt = MySMA.CurrentScaleData.CurWeight
                Motion = MySMA.CurrentScaleData.Motion

                If MySMA.ConnectionStatus = SMA.enumConnectionStatus.Connecting Then MyScaleStatus = "Connecting..."

                If MySMA.ConnectionStatus = SMA.enumConnectionStatus.NotConnected Then MyScaleStatus = "Lost Connection"

                If MySMA.ConnectionStatus = SMA.enumConnectionStatus.Ok Then

                    If Motion Then
                        MyScaleStatus = "MOTION"
                    Else
                        MyScaleStatus = ""
                    End If

                End If

                Me.lblCnt.Text = MySMA.CurrentScaleData.CurrentStatusChar


            ElseIf My.Settings.ConnectionType = "Serial" Then

                If Motion Then

                    MyScaleStatus = "Motion"

                Else

                    MyScaleStatus = ""

                End If

            End If
            Try

                If Wt > 2000 And tmrAlarm.Enabled = False Then

                    If Len(My.Settings.AudioFile) > 0 Then
                        Try
                            If CancelAlarm = False Then

                                My.Computer.Audio.Play(My.Settings.AudioFile)
                                Me.tmrAlarm.Enabled = True
                                Me.tmrAlarm.Start()
                                Me.btnAlarm.BackColor = Color.Red

                            End If
                        Catch ex As Exception

                        End Try

                    End If

                ElseIf Wt <= 500 Then

                    StopAlarm()
                    CancelAlarm = False

                End If

                If SplitWeigh Then

                    Me.lblSplitMotion.Text = MyScaleStatus

                Else

                    Me.lblMotion.Text = MyScaleStatus
                    Me.lblSplitMotion.Text = ""

                End If

                If LastWeight <> Wt Then
                    If LastWeight < 2000 And Wt >= 2000 Then
                        ManualLightChange = False
                    ElseIf LastWeight > 2000 And Wt <= 2000 Then
                        ManualLightChange = False
                    End If
                End If

                LastWeight = Wt

                If Wt >= 2000 And ManualLightChange = False Then

                    ChangeLightToGreen()

                ElseIf Wt < 2000 And ManualLightChange = False Then

                    ChangeLightToRed()

                End If

                If SplitWeigh And PupWeigh Then

                    Me.lblPup.Text = Me.Wt.ToString

                ElseIf SplitWeigh And PupWeigh = False Then

                    Me.lblSplitWeigh.Text = Me.Wt.ToString
                    Me.lblPup.Text = "0"

                Else

                    Me.lblWeight.Text = Me.Wt.ToString
                    Me.lblSplitWeigh.Text = "0"
                    Me.lblPup.Text = "0"

                End If

                Dim TotalWt As Integer = Convert.ToInt32(Me.lblWeight.Text) + Convert.ToInt32(lblSplitWeigh.Text) + Convert.ToInt32(lblPup.Text)
                Me.lblTotalWeigh.Text = TotalWt.ToString

            Catch ex As Exception

                If SplitWeigh Then

                    Me.lblSplitMotion.Text = "Connection Error"

                Else

                    Me.lblMotion.Text = "Connection Error"

                End If

            End Try

        End If

    End Sub

    Public Sub ChangeLightToRed()
        'Me.SerialPort1.RtsEnable = True

        ChangeLight()
    End Sub

    Public Sub ChangeLightToGreen()

        'Me.SerialPort1.RtsEnable = False
        ChangeLight()

    End Sub

    Private Sub btnZero_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnZero.Click
        Me.hsScaleBar.Value = 0

        If My.Settings.ConnectionType = "Serial" Then

            'For I As Integer = 1 To 3

            '  ** Remove these remarks for Rice Lake indicators **
            Me.SerialPort1.WriteLine("KZERO" + vbCr + vbLf)

            'Next

        ElseIf My.Settings.ConnectionType = "Network" Then

            StreamingScale.ZeroCommand = "KZERO"
            StreamingScale.ZeroScale()

        ElseIf My.Settings.ConnectionType = "SMA" Then

            MySMA.ZeroScale()

        End If

    End Sub


    Public Sub ChangeLight()
        Try

            'RedLight = Me.SerialPort1.RtsEnable

            If RedLight = False Then
                Me.PictureBox1.BackgroundImage = My.Resources.Red
            Else
                Me.PictureBox1.BackgroundImage = My.Resources.Green
            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Sub btnChangeLight_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnChangeLight.Click

        ManualLightChange = True
        StopAlarm()

        'Me.SerialPort1.RtsEnable = Not Me.SerialPort1.RtsEnable
        ChangeLight()

    End Sub

    Private Sub btnWeigh_In_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWeigh_In.Click

        StopAlarm()

        Dim StoredWeight As Integer = 0
        Dim TrailerWeight As Integer = 0

        If SplitWeigh Then
            StoredWeight = Convert.ToInt32(lblWeight.Text.ToString)
        End If

        If PupWeigh Then
            TrailerWeight = Convert.ToInt32(lblSplitWeigh.Text.ToString)
        End If

        Dim WS As New Weigh_Screen(StoredWeight, TrailerWeight)
        WS.Close()
        WS = Nothing
        Reset()

    End Sub

    Private Sub Reset()
        SplitWeigh = False
        lblSplitWeigh.Visible = False
        lblTotalWeigh.Visible = False
        btnSplit.Text = "Split Weigh"
        SplitWeigh = False
        PupWeigh = False
        lblPup.Visible = False
        btnPup.Visible = False
        btnPup.Text = "Add Pup"
        lblPup.Text = ""
        lblSplitMotion.Text = ""
        EditThisTicket = False
    End Sub
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click

        StopAlarm()
        Dim L As New Lists
        L.ShowDialog()

    End Sub

    Public Sub StopAlarm()

        CancelAlarm = True
        btnAlarm.BackColor = Color.Yellow
        My.Computer.Audio.Stop()
        Me.tmrAlarm.Stop()
        Me.tmrAlarm.Enabled = False

    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        StopAlarm()

        Dim StoredWeight As Integer = 0
        Dim TrailerWt As Integer = 0
        If SplitWeigh Then
            StoredWeight = Convert.ToInt32(lblWeight.Text.ToString)
        End If

        If PupWeigh Then
            TrailerWt = Convert.ToInt32(lblSplitWeigh.Text.ToString)
        End If

        Dim InboundTrucks As New InboundTrucks

        If InboundTrucks.ShowDialog = Windows.Forms.DialogResult.OK Then

            If Not ModMain.TransactionRow Is Nothing Then

                Dim Ws As New Weigh_Screen(TransactionRow.Ticket, StoredWeight, TrailerWt, True)
                Ws.Close()
                Ws = Nothing
                Reset()

            End If

        End If

    End Sub

    Private Sub SettingsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SettingsToolStripMenuItem.Click

        tmrUpdate.Stop()
        tmrAlarm.Stop()

        Dim Setup As New Setup
        Setup.ShowDialog()

        SerialPort1.Close()
        MySMA.Disconnect()

        If StreamingScale IsNot Nothing Then
            StreamingScale.Disconnect()
            StreamingScale.Dispose()
            StreamingScale = Nothing
        End If

        EstablishConnection()

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click

        StopAlarm()
        Dim CT As New Completed_Trucks

        If CT.ShowDialog = Windows.Forms.DialogResult.OK Then

            If Not ModMain.TransactionRow Is Nothing Then

                Dim Ws As New Weigh_Screen(TransactionRow.Ticket)
                Ws.Close()
                Ws = Nothing
                Reset()

            End If

        End If

    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        StopAlarm()

    End Sub

    Private Sub tmrAlarm_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrAlarm.Tick

        If Len(My.Settings.AudioFile) > 0 Then

            Try

                My.Computer.Audio.Stop()

                If CancelAlarm = False Then My.Computer.Audio.Play(My.Settings.AudioFile)

            Catch ex As Exception

            End Try

        End If
    End Sub


    Private Sub hsScaleBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles hsScaleBar.Scroll

        Me.Wt = (Convert.ToInt32(Me.hsScaleBar.Value / 10) * 10)

    End Sub


    Private Sub TruckScaleCamer1_Enter(sender As Object, e As EventArgs)

    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click

        Me.Close()

    End Sub

    Private Sub Main_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

        ShutDown()

    End Sub

    Private Sub ShutDown()

        StopAlarm()
        My.Settings.Save()
        If MySMA.IsRunning Then
            MySMA.Disconnect()
        End If
        If SerialPort1.IsOpen Then
            SerialPort1.Close()
        End If
        If StreamingScale IsNot Nothing Then
            StreamingScale.Disconnect()
            StreamingScale.Dispose()
            StreamingScale = Nothing
        End If
    End Sub

    Private Sub AxVLCPlugin21_Enter(sender As Object, e As EventArgs) Handles AxVLCPlugin21.Enter

    End Sub

    Private Sub btnSplit_Click(sender As Object, e As EventArgs) Handles btnSplit.Click

        If SplitWeigh Then

            lblSplitWeigh.Visible = False
            lblTotalWeigh.Visible = False
            btnSplit.Text = "Split Weigh"
            SplitWeigh = False
            lblSplitMotion.Text = ""
            PupWeigh = False
            lblPup.Text = ""
            lblPup.Visible = False
            btnPup.Visible = False

        Else

            lblSplitWeigh.Visible = True
            lblTotalWeigh.Visible = True
            btnSplit.Text = "Cancel Split"
            SplitWeigh = True
            lblMotion.Text = "Stored"
            PupWeigh = False
            lblPup.Visible = False
            btnPup.Text = "Add Pup"
            btnPup.Visible = True

        End If

    End Sub

    Private Sub btnPup_Click(sender As Object, e As EventArgs) Handles btnPup.Click

        If PupWeigh Then

            PupWeigh = False
            lblPup.Visible = False
            btnPup.Text = "Add Pup"
            btnPup.Visible = True

        Else

            PupWeigh = True
            lblPup.Visible = True
            btnPup.Text = "Remove Pup"
            btnPup.Visible = True

        End If

    End Sub


    Private Sub Button_Reports_Click(sender As Object, e As EventArgs) Handles btnReports.Click

        StopAlarm()
        Dim Rpt As New Reports
        Rpt.ShowDialog()

    End Sub

    Private Sub Button_Basic_Click_1(sender As Object, e As EventArgs) Handles btnBasicTicket.Click

        StopAlarm()
        Dim StoredWeight As Integer = 0
        Dim TrailerWt As Integer = 0
        If SplitWeigh Then
            StoredWeight = Convert.ToInt32(lblWeight.Text.ToString)
        End If

        If PupWeigh Then
            TrailerWt = Convert.ToInt32(lblSplitWeigh.Text.ToString)
        End If

        Dim BasicEntry As New BasicTicket(StoredWeight, TrailerWt)

        BasicEntry.Close()
        BasicEntry = Nothing
        Reset()

    End Sub

    Private Sub btnAlarm_Click(sender As Object, e As EventArgs) Handles btnAlarm.Click

        StopAlarm()

    End Sub



    Private Sub EditTablesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditTablesToolStripMenuItem.Click

        StopAlarm()
        Dim L As New Lists
        L.ShowDialog()

    End Sub



End Class