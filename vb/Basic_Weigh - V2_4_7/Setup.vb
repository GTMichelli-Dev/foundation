Imports System.Windows.Forms

Public Class Setup

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

        If MsgBox("Closing Program to Save Settings") = MsgBoxResult.Ok Then

            Try

                Me.Validate()
                Me.SetupBindingSource.EndEdit()
                Me.TableAdapterManager.UpdateAll(Me.ScaleDataSet)

                My.Settings.Baud = cbBaudRate.SelectedItem.ToString
                My.Settings.Word = cbWordLength.SelectedItem
                My.Settings.Parity = cbParity.SelectedItem.ToString
                My.Settings.StopBits = cbStopBits.SelectedItem.ToString

                'Locate which radio button Is selected And set the connection type variable before saving.
                If rdoNetwork.Checked Then

                    If ckSMAEnabled.Checked Then
                        My.Settings.ConnectionType = "SMA"
                    Else
                        My.Settings.ConnectionType = "Network"
                    End If

                ElseIf rdoSerial.Checked Then

                    My.Settings.ConnectionType = "Serial"

                Else

                    My.Settings.ConnectionType = "Simulate"

                End If

                My.Settings.ComPort = cbComPort.Text.ToString
                My.Settings.PrinterName = PrinterNameTextBox.Text
                My.Settings.IP_Address = txtIPAddress.Text.ToString
                My.Settings.Network_Port = txtNetworkPort.Text.ToString
                My.Settings.PagesToPrint = Convert.ToString(nuPagesToPrint.Value)
                My.Settings.TicketsperPage = Convert.ToString(nuTicketsperPage.Value)
                My.Settings.AudioFile = WaveFileTextBox.Text
                My.Settings.Save()

            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

        End If

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()

    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub


    Private Sub Setup_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.WaveFileTextBox.Text = My.Settings.AudioFile
        ' Clear the items in the Com Port ComboBox
        Me.cbComPort.Items.Clear()
        ' Load the data from the Setup Table
        Me.PrinterNameTextBox.Text = My.Settings.PrinterName

        Me.SetupTableAdapter.Fill(Me.ScaleDataSet.Setup)
        ' For Every Com Port existing create an entry in the combo box.

        For Each port As String In My.Computer.Ports.SerialPortNames

            Me.cbComPort.Items.Add(port)

            If port = My.Settings.ComPort Then

                Me.cbComPort.SelectedText = port

            End If

        Next

        Dim BaudIndex As Integer = Me.cbBaudRate.FindStringExact(My.Settings.Baud.ToString)
        Dim WordLengthIndex As Integer = Me.cbWordLength.FindStringExact(My.Settings.Word)
        Dim ParityIndex As Integer = Me.cbParity.FindStringExact(My.Settings.Parity.ToString)
        Dim StopBitsIndex As Integer = Me.cbStopBits.FindStringExact(My.Settings.StopBits.ToString)

        txtIPAddress.Text = My.Settings.IP_Address.ToString
        txtNetworkPort.Text = My.Settings.Network_Port.ToString

        Dim LoadedPagesToPrint = Convert.ToInt16(My.Settings.PagesToPrint.ToString)

        If LoadedPagesToPrint > 0 And LoadedPagesToPrint < 4 Then
            nuPagesToPrint.Value = LoadedPagesToPrint
        Else
            nuPagesToPrint.Value = 1
        End If
        Dim LoadedTicketsPerPage = Convert.ToInt16(My.Settings.TicketsperPage.ToString)

        If LoadedTicketsPerPage > 0 And LoadedTicketsPerPage < 4 Then
            nuTicketsperPage.Value = LoadedTicketsPerPage
        Else
            nuTicketsperPage.Value = 1
        End If
        If BaudIndex > -1 Then
            cbBaudRate.SelectedIndex = BaudIndex
        End If
        If WordLengthIndex > -1 Then
            cbWordLength.SelectedIndex = WordLengthIndex
        End If
        If ParityIndex > -1 Then
            cbParity.SelectedIndex = ParityIndex
        End If
        If StopBitsIndex > -1 Then
            cbStopBits.SelectedIndex = StopBitsIndex
        End If

        If My.Settings.ConnectionType.ToString = "Network" Then

            rdoNetwork.Select()

            cbComPort.Enabled = False
            cbBaudRate.Enabled = False
            cbParity.Enabled = False
            cbWordLength.Enabled = False
            cbStopBits.Enabled = False
            txtIPAddress.Enabled = True
            txtNetworkPort.Enabled = True
            ckSMAEnabled.Enabled = True
            ckSMAEnabled.Checked = False

        ElseIf My.Settings.ConnectionType.ToString = "SMA" Then

            rdoNetwork.Select()

            cbComPort.Enabled = False
            cbBaudRate.Enabled = False
            cbParity.Enabled = False
            cbWordLength.Enabled = False
            cbStopBits.Enabled = False
            txtIPAddress.Enabled = True
            txtNetworkPort.Enabled = True
            ckSMAEnabled.Enabled = True
            ckSMAEnabled.Checked = True

        ElseIf My.Settings.ConnectionType.ToString = "Serial" Then

            rdoSerial.Select()

            cbComPort.Enabled = True
            cbBaudRate.Enabled = True
            cbParity.Enabled = True
            cbWordLength.Enabled = True
            cbStopBits.Enabled = True
            txtIPAddress.Enabled = False
            txtNetworkPort.Enabled = False
            ckSMAEnabled.Enabled = False
            ckSMAEnabled.Checked = False


        ElseIf My.Settings.ConnectionType.ToString = "Simulate" Then

            rdoSimulate.Select()

            cbComPort.Enabled = False
            cbBaudRate.Enabled = False
            cbParity.Enabled = False
            cbWordLength.Enabled = False
            cbStopBits.Enabled = False
            txtIPAddress.Enabled = False
            txtNetworkPort.Enabled = False
            ckSMAEnabled.Enabled = False
            ckSMAEnabled.Checked = False

        Else  'If what was typed doesn't match this code default to Simulate

            rdoSimulate.Select()

            cbComPort.Enabled = False
            cbBaudRate.Enabled = False
            cbParity.Enabled = False
            cbWordLength.Enabled = False
            cbStopBits.Enabled = False
            txtIPAddress.Enabled = False
            txtNetworkPort.Enabled = False
            ckSMAEnabled.Enabled = False
            ckSMAEnabled.Checked = False

        End If

    End Sub

    Private Sub btnPrinter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrinter.Click

        Dim pd As New PrintDialog
        If pd.ShowDialog = Windows.Forms.DialogResult.OK Then

            Me.PrinterNameTextBox.Text = pd.PrinterSettings.PrinterName

        End If

    End Sub


    Private Sub btnSound_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSound.Click

        Me.OpenFileDialog1.FileName = Me.WaveFileTextBox.Text
        If Me.OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then

            Me.WaveFileTextBox.Text = Me.OpenFileDialog1.FileName

        End If
    End Sub


    Private Sub rdoSerial_CheckedChanged(sender As Object, e As EventArgs) Handles rdoSerial.CheckedChanged

        If rdoSerial.Checked = True Then

            cbComPort.Enabled = True
            cbBaudRate.Enabled = True
            cbParity.Enabled = True
            cbWordLength.Enabled = True
            cbStopBits.Enabled = True
            txtIPAddress.Enabled = False
            txtNetworkPort.Enabled = False
            My.Settings.ConnectionType = "Serial"

        End If
    End Sub

    Private Sub rdoNetwork_CheckedChanged(sender As Object, e As EventArgs) Handles rdoNetwork.CheckedChanged

        If rdoNetwork.Checked = True Then

            cbComPort.Enabled = False
            cbBaudRate.Enabled = False
            cbParity.Enabled = False
            cbWordLength.Enabled = False
            cbStopBits.Enabled = False
            txtIPAddress.Enabled = True
            txtNetworkPort.Enabled = True
            ckSMAEnabled.Enabled = True
            My.Settings.ConnectionType = "Network"

        End If
    End Sub

    Private Sub rdoSimulate_CheckedChanged(sender As Object, e As EventArgs) Handles rdoSimulate.CheckedChanged

        If rdoSimulate.Checked = True Then

            cbComPort.Enabled = False
            cbBaudRate.Enabled = False
            cbParity.Enabled = False
            cbWordLength.Enabled = False
            cbStopBits.Enabled = False
            txtIPAddress.Enabled = False
            txtNetworkPort.Enabled = False
            My.Settings.ConnectionType = "Simulate"

        End If
    End Sub
End Class
