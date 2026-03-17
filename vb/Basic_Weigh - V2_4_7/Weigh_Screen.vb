Imports System.Windows.Forms

Public Class Weigh_Screen

    Dim Ticket As String = ""
    Dim SplitWeigh As Boolean
    Dim PupWeigh As Boolean
    Dim StoredWt As Int32 = 0
    Dim TrailerWt As Int32 = 0
    Dim LtWt As Int32 = 0
    Dim Manual_In_DateTime As Boolean = False
    Dim Manual_Out_DateTime As Boolean = False
    Dim NewTicket As Boolean
    Dim SavedTruck As Boolean = False


    Public Sub New(ByVal StorWt As Integer, ByVal TrlrWt As Integer)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        If StorWt > 0 Then

            SplitWeigh = True
            lblStoredWt.Text = StorWt.ToString
            lblStoredWt.Visible = True
            lblStored.Visible = True
            Label5.Visible = True
            Label6.Visible = True
            Label7.Visible = False
            Label8.Visible = True
            lblPup.Visible = False
            lblWeight.Visible = True
            lblTotalWeight.Visible = True
            btnSplit.Text = "Cancel Split"
            btnPup.Visible = True
            btnPup.Text = "Add Pup"

        Else

            SplitWeigh = False
            lblStoredWt.Text = ""
            lblStoredWt.Visible = False
            lblStored.Visible = False
            Label5.Visible = False
            Label6.Visible = False
            Label7.Visible = False
            Label8.Visible = False
            lblPup.Visible = False
            lblWeight.Visible = False
            lblTotalWeight.Visible = False
            btnSplit.Text = "Split Weigh"
            btnPup.Visible = False

        End If

        StoredWt = StorWt

        If TrlrWt > 0 Then

            PupWeigh = True
            Label7.Visible = True
            lblPup.Visible = True
            btnPup.Text = "Remove Pup"
            lblWeight.Text = TrlrWt.ToString

        Else

            PupWeigh = False
            Label7.Visible = False
            lblPup.Visible = False

            btnPup.Text = "Add Pup"
            lblWeight.Text = ""

        End If

        In_DatePicker.Visible = False
        In_TimePicker.Visible = False
        Out_DatePicker.Visible = False
        Out_TimePicker.Visible = False
        lblSetDateIn.Visible = False
        lblSetDateOut.Visible = False

        TrailerWt = TrlrWt

        NewTicket = True

        Me.ShowDialog()

    End Sub


    Public Sub New(ByVal StorWt As Integer, ByVal TrlrWt As Integer, ByVal LtWt As Integer)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        If StorWt > 0 Then

            SplitWeigh = True
            lblStoredWt.Text = StorWt.ToString
            lblStoredWt.Visible = True
            lblStored.Visible = True
            Label5.Visible = True
            Label6.Visible = True
            Label7.Visible = False
            Label8.Visible = True
            lblPup.Visible = False
            lblWeight.Visible = True
            lblTotalWeight.Visible = True
            btnSplit.Text = "Cancel Split"
            btnPup.Visible = True
            btnPup.Text = "Add Pup"

        Else

            SplitWeigh = False
            lblStoredWt.Text = ""
            lblStoredWt.Visible = False
            lblStored.Visible = False
            Label5.Visible = False
            Label6.Visible = False
            Label7.Visible = False
            Label8.Visible = False
            lblPup.Visible = False
            lblWeight.Visible = False
            lblTotalWeight.Visible = False
            btnSplit.Text = "Split Weigh"
            btnPup.Visible = False

        End If

        StoredWt = StorWt

        If TrlrWt > 0 Then

            PupWeigh = True
            Label7.Visible = True
            lblPup.Visible = True
            btnPup.Text = "Remove Pup"
            lblWeight.Text = TrlrWt.ToString

        Else

            PupWeigh = False
            Label7.Visible = False
            lblPup.Visible = False

            btnPup.Text = "Add Pup"
            lblWeight.Text = ""

        End If

        In_DatePicker.Visible = False
        In_TimePicker.Visible = False
        Out_DatePicker.Visible = False
        Out_TimePicker.Visible = False
        lblSetDateIn.Visible = False
        lblSetDateOut.Visible = False

        TrailerWt = TrlrWt

        NewTicket = True

        Me.ShowDialog()

    End Sub

    Public Sub New(ByVal RecalledTicket As String)

        ' This call is required by the designer.
        InitializeComponent()

        Me.Out_Weight.ReadOnly = False
        Me.In_Weight.ReadOnly = False

        SplitWeigh = False
        lblStoredWt.Text = ""
        lblStoredWt.Visible = False
        lblStored.Visible = False
        Label5.Visible = False
        Label6.Visible = False
        Label8.Visible = False
        lblPup.Visible = False
        lblWeight.Visible = False
        lblTotalWeight.Visible = False
        btnSplit.Text = "Split Weigh"
        btnSplit.Visible = False
        btnPup.Visible = False
        PupWeigh = False
        Label7.Visible = False
        lblPup.Visible = False
        btnPup.Text = "Add Pup"
        lblWeight.Text = ""

        Manual_In_DateTime = True
        Manual_Out_DateTime = True
        In_DatePicker.Visible = True
        In_TimePicker.Visible = True
        Out_DatePicker.Visible = True
        Out_TimePicker.Visible = True
        lblSetDateIn.Visible = True
        lblSetDateOut.Visible = True



        Me.Ticket = RecalledTicket
        Main.EditThisTicket = True

        Me.TransactionsTableAdapter.FillByTicket(Me.ScaleDataSet.Transactions, Ticket)

        If Me.ScaleDataSet.Transactions.Count = 0 Then
            MsgBox("Could Not Find Ticket " + Ticket)
            Me.Close()
        Else
            Me.Ticket = RecalledTicket
            NewTicket = False
            Me.ShowDialog()
        End If

    End Sub
    Public Sub New(ByVal RecalledTicket As String, ByVal StorWt As Integer, ByVal TrlrWt As Integer, Optional Saved_Truck As Boolean = False)

        InitializeComponent()
        SavedTruck = Saved_Truck
        If StorWt > 0 Then
            SplitWeigh = True
            lblStoredWt.Text = StorWt.ToString
            lblStoredWt.Visible = True
            lblStored.Visible = True
            Label5.Visible = True
            Label6.Visible = True
            Label8.Visible = True
            lblPup.Visible = False
            lblWeight.Visible = True
            lblTotalWeight.Visible = True
            btnSplit.Text = "Cancel Split"
            btnPup.Visible = True
            btnPup.Text = "Add Pup"
        Else
            SplitWeigh = False
            lblStoredWt.Text = ""
            lblStoredWt.Visible = False
            lblStored.Visible = False
            Label5.Visible = False
            Label6.Visible = False
            Label8.Visible = False
            lblPup.Visible = False
            lblWeight.Visible = False
            lblTotalWeight.Visible = False
            btnSplit.Text = "Split Weigh"
            btnPup.Visible = False
        End If

        StoredWt = StorWt

        If TrlrWt > 0 Then
            PupWeigh = True
            Label7.Visible = True
            lblPup.Visible = True
            btnPup.Text = "Remove Pup"
            lblWeight.Text = TrlrWt.ToString
        Else
            PupWeigh = False
            Label7.Visible = False

            lblPup.Visible = False

            btnPup.Text = "Add Pup"
            lblWeight.Text = ""
        End If

        In_DatePicker.Visible = False
        In_TimePicker.Visible = False
        Out_DatePicker.Visible = False
        Out_TimePicker.Visible = False
        lblSetDateIn.Visible = False
        lblSetDateOut.Visible = False

        TrailerWt = TrlrWt

        Me.Ticket = RecalledTicket
        Me.TransactionsTableAdapter.FillByTicket(Me.ScaleDataSet.Transactions, Ticket)
        If Me.ScaleDataSet.Transactions.Count = 0 Then
            MsgBox("Could Not Find Ticket " + Ticket)
            Me.Close()
        Else
            Me.Ticket = RecalledTicket
            NewTicket = False
            Me.ShowDialog()
        End If

    End Sub


    Private Sub Weigh_Screen_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'TODO: This line of code loads data into the 'ScaleDataSet2.Commodity' table. You can move, or remove it, as needed.
        Me.CommodityTableAdapter.Fill(Me.ScaleDataSet2.Commodity)
        'TODO: This line of code loads data into the 'ScaleDataSet.Truck' table. You can move, or remove it, as needed.
        Me.TruckTableAdapter.Fill(Me.ScaleDataSet.Truck)

        Me.DestinationTableAdapter.Fill(Me.ScaleDataSet.Destination)

        Me.LocationTableAdapter.Fill(Me.ScaleDataSet.Location)



        Me.CarrierTableAdapter.Fill(Me.ScaleDataSet.Carrier)

        Me.CustomerTableAdapter.Fill(Me.ScaleDataSet.Customer)

        If Ticket = "" Then

            Dim SetupTableAdapter As New ScaleDataSetTableAdapters.SetupTableAdapter
            SetupTableAdapter.Fill(ScaleDataSet.Setup)
            Dim Q As New ScaleDataSetTableAdapters.QueriesTableAdapter

            'Ticket = ScaleDataSet.Setup(0).Ticket_Number.ToString

            Me.TransactionsTableAdapter.FillByTicket(Me.ScaleDataSet.Transactions, Ticket)
            While Me.ScaleDataSet.Transactions.Count > 0
                ScaleDataSet.Setup(0).Ticket_Number += 1
                Ticket = ScaleDataSet.Setup(0).Ticket_Number.ToString
                Me.TransactionsTableAdapter.FillByTicket(Me.ScaleDataSet.Transactions, Ticket)
            End While
            SetupTableAdapter.Update(Me.ScaleDataSet)
            TransactionRow = Me.ScaleDataSet.Transactions.NewTransactionsRow
            TransactionRow.Date_In = Now
            TransactionRow.In_Weight = 0
            TransactionRow.Ticket = Ticket
            Me.ScaleDataSet.Transactions.AddTransactionsRow(TransactionRow)


            Me.pnlGTN.Visible = False
            Me.pnlOutWt.Visible = False

        Else

            Me.TransactionsTableAdapter.FillByTicket(Me.ScaleDataSet.Transactions, Ticket)
            ModMain.TransactionRow = Me.ScaleDataSet.Transactions(0)
            Me.pnlGTN.Visible = True
            Me.In_Weight.Enabled = True
            Me.pnlOutWt.Visible = True

            If Main.EditThisTicket = False Then
                Me.Out_Weight.Value = 0
            Else
                ckUseScale.CheckState = CheckState.Unchecked
                If ModMain.TransactionRow.IsOut_WeightNull Then
                    Me.Out_Weight.Value = 0
                Else
                    Me.Out_Weight.Value = ModMain.TransactionRow.Out_Weight
                End If

            End If

        End If

        Me.TransactionsBindingSource.MoveFirst()
        In_TimePicker.Format = DateTimePickerFormat.Time
        Out_TimePicker.Format = DateTimePickerFormat.Time

        In_DatePicker.Format = DateTimePickerFormat.Short
        Out_DatePicker.Format = DateTimePickerFormat.Short
        In_TimePicker.ShowUpDown = True
        Out_TimePicker.ShowUpDown = True

    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

        If DataSaved() Then
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        End If
    End Sub


    Private Function DataSaved() As Boolean
        Try

            Dim InboundTicketDateTime As DateTime

            If Me.pnlGTN.Visible = True Then 'If we are out-weighing the truck we will apply the light weight to the actual light weight of the truck.

                ' First Create a Table Adapter to work with
                Using TruckTableAdapter As New ScaleDataSetTableAdapters.TruckTableAdapter

                    'Next Create a table to put the data into
                    Dim Tbl As New ScaleDataSet.TruckDataTable

                    'Execute the Query (First value is the table to drop results into, second is search criteria)
                    If TruckTableAdapter.FillByTruckID(Tbl, TruckComboBox.Text.ToString) > 0 Then

                        'If we have records then define a record row to put them in.
                        Dim row As ScaleDataSet.TruckRow
                        'Set the row equal the to first row of the table.
                        row = Tbl(0)
                        If Me.In_Weight.Value <= Me.Out_Weight.Value Then  'Check to see which is actually the Outweight
                            row.Light_Wt = Me.In_Weight.Value
                        Else
                            row.Light_Wt = Me.Out_Weight.Value
                        End If

                    Else
                        'No records found so we'll execute this query to add one.
                        Tbl.AddTruckRow(Mid(TruckComboBox.Text, 1, 15), Me.In_Weight.Value)

                    End If

                    TruckTableAdapter.Update(Tbl)

                End Using

            End If

            Me.Validate()


            Me.TransactionsBindingSource.EndEdit()

            Me.TableAdapterManager.UpdateAll(Me.ScaleDataSet)

            If Me.pnlGTN.Visible = False Then  'Only update ModMain.TransactionRow during an inbound truck.

                'After the database is written to I need to do a search by the Date_In so I can send that record to ModMain for printing.
                InboundTicketDateTime = TransactionRow.Date_In

                Me.TransactionsTableAdapter.FillByInboundDateTime(Me.ScaleDataSet.Transactions, InboundTicketDateTime)
                ModMain.TransactionRow = Me.ScaleDataSet.Transactions(0)

            End If

            Return True

        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try

    End Function

    Private Sub tmrUpdate_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrUpdate.Tick

        If My.Forms.Main.EditThisTicket = False Then

            Dim TotalWt As Integer = My.Forms.Main.Wt + StoredWt + TrailerWt

            If Me.pnlGTN.Visible Then

                If Me.ckUseScale.Checked Then

                    Try

                        If SplitWeigh And PupWeigh Then

                            Me.Out_Weight.Value = TotalWt
                            Me.lblTotalWeight.Text = TotalWt.ToString
                            Me.lblStoredWt.Text = StoredWt.ToString
                            Me.lblWeight.Text = TrailerWt.ToString
                            Me.lblPup.Text = My.Forms.Main.Wt.ToString

                        ElseIf SplitWeigh And PupWeigh = False Then

                            Me.Out_Weight.Value = TotalWt
                            Me.lblTotalWeight.Text = TotalWt.ToString
                            Me.lblStoredWt.Text = StoredWt.ToString
                            Me.lblWeight.Text = My.Forms.Main.Wt.ToString
                            Me.lblPup.Text = "0"

                        Else

                            Me.Out_Weight.Value = My.Forms.Main.Wt
                            Me.lblPup.Text = "0"
                            Me.lblWeight.Text = "0"

                        End If

                    Catch ex As Exception
                        Me.Out_Weight.Value = 0
                    End Try

                End If

                Me.lblMotionOut.Visible = My.Forms.Main.Motion
                Me.pnlSave.Enabled = Not Me.lblMotionOut.Visible

                If Manual_Out_DateTime = False Then
                    Me.Date_OutLabel1.Text = Now.ToString
                Else
                    Me.Date_OutLabel1.Text = Out_DatePicker.Text + " " + Out_TimePicker.Text
                End If

                Dim G, T, N As Integer
                If ModMain.TransactionRow.In_Weight > Me.Out_Weight.Value Then
                    G = ModMain.TransactionRow.In_Weight
                    T = Me.Out_Weight.Value
                Else
                    T = ModMain.TransactionRow.In_Weight
                    G = Me.Out_Weight.Value
                End If

                N = G - T
                Me.lblGross.Text = String.Format("{0:N0}", G) + " lbs."
                Me.lblTare.Text = String.Format("{0:N0}", T) + " lbs."
                Me.lblNet.Text = String.Format("{0:N0}", N) + " lbs."

            Else

                If Me.ckUseScale.Checked Then

                    Try
                        If SplitWeigh And PupWeigh Then

                            Me.In_Weight.Value = TotalWt
                            Me.lblTotalWeight.Text = TotalWt.ToString
                            Me.lblStoredWt.Text = StoredWt.ToString
                            Me.lblWeight.Text = TrailerWt.ToString
                            Me.lblPup.Text = My.Forms.Main.Wt.ToString

                        ElseIf SplitWeigh And PupWeigh = False Then

                            Me.In_Weight.Value = TotalWt
                            Me.lblTotalWeight.Text = TotalWt.ToString
                            Me.lblStoredWt.Text = StoredWt.ToString
                            Me.lblWeight.Text = My.Forms.Main.Wt.ToString
                            Me.lblPup.Text = "0"

                        Else

                            Me.In_Weight.Value = My.Forms.Main.Wt
                            Me.lblPup.Text = "0"
                            Me.lblWeight.Text = "0"

                        End If

                    Catch ex As Exception

                        Me.In_Weight.Value = 0

                    End Try

                End If

                Me.lblMotionIn.Visible = My.Forms.Main.Motion
                Me.pnlSave.Enabled = Not Me.lblMotionOut.Visible

                If Manual_In_DateTime = False Then
                    Me.Date_InLabel1.Text = Now.ToString
                Else
                    Me.Date_InLabel1.Text = In_DatePicker.Text + " " + In_TimePicker.Text
                End If

            End If

        Else

            Dim G, T, N As Integer
            If ModMain.TransactionRow.In_Weight > Me.Out_Weight.Value Then
                G = Me.In_Weight.Value
                T = Me.Out_Weight.Value
            Else
                T = Me.In_Weight.Value
                G = Me.Out_Weight.Value
            End If

            N = G - T
            Me.lblGross.Text = String.Format("{0:N0}", G) + " lbs."
            Me.lblTare.Text = String.Format("{0:N0}", T) + " lbs."
            Me.lblNet.Text = String.Format("{0:N0}", N) + " lbs."

        End If

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        If DataSaved() Then

            If Me.pnlGTN.Visible = True Then

                ModMain.PrintOutboundTicket(ModMain.TransactionRow.Ticket)

            Else

                ModMain.PrintInboundTicket(ModMain.TransactionRow.Ticket)

            End If

            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        End If
    End Sub


    Private Sub CustomerComboBox_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles CommentsTextBox.KeyDown, LocationComboBox.KeyDown, TruckComboBox.KeyDown, ProductComboBox.KeyDown, CustomerComboBox.KeyDown, CarrierComboBox.KeyDown
        If e.KeyCode = Keys.Return Then
            e.SuppressKeyPress = True
            SelectNextControl(sender, True, True, True, True)
        End If
    End Sub

    Private Sub ckUseScale_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ckUseScale.CheckedChanged

        If Me.pnlOutWt.Visible Then
            Me.Out_Weight.ReadOnly = ckUseScale.Checked
        Else
            Me.In_Weight.ReadOnly = ckUseScale.Checked
        End If

    End Sub


    Private Sub In_Weight_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles In_Weight.LostFocus, Out_Weight.LostFocus

        Dim I As Integer = sender.value
        I = I / 20
        I = I * 20
        sender.value = I

    End Sub

    Private Sub btnSplit_Click(sender As Object, e As EventArgs) Handles btnSplit.Click

        If SplitWeigh Then

            SplitWeigh = False
            StoredWt = 0
            lblStoredWt.Text = ""
            lblStoredWt.Visible = False
            lblStored.Visible = False
            lblWeight.Visible = False
            Label5.Visible = False
            Label6.Visible = False
            Label7.Visible = False
            Label8.Visible = False
            lblTotalWeight.Visible = False
            btnSplit.Text = "Split Weigh"
            btnPup.Text = "Add Pup"
            btnPup.Visible = False

        Else

            SplitWeigh = True
            StoredWt = My.Forms.Main.Wt
            lblStoredWt.Text = StoredWt.ToString
            lblStoredWt.Visible = True
            lblStored.Visible = True
            lblWeight.Visible = True
            Label5.Visible = True
            Label6.Visible = True
            Label7.Visible = False
            Label8.Visible = True
            lblTotalWeight.Visible = True
            btnSplit.Text = "Cancel Split"
            btnPup.Text = "Add Pup"
            btnPup.Visible = True

        End If

    End Sub


    Private Sub btnPup_Click(sender As Object, e As EventArgs) Handles btnPup.Click

        If PupWeigh = False Then

            PupWeigh = True
            Label7.Visible = True
            lblPup.Visible = True
            TrailerWt = My.Forms.Main.Wt
            lblPup.Text = TrailerWt.ToString
            btnPup.Text = "Remove Pup"
            lblWeight.Text = TrailerWt.ToString

        Else

            PupWeigh = False
            TrailerWt = 0
            Label7.Visible = False
            lblPup.Visible = False
            btnPup.Text = "Add Pup"
            lblWeight.Text = ""

        End If

    End Sub

    Private Sub Cancel_Button_Click(sender As Object, e As EventArgs) Handles Cancel_Button.Click

        If lblNet.Text = "" Then
            Me.TransactionsTableAdapter.DeleteByTicketID(TicketLabel1.Text)
        Else
            Me.TransactionsBindingSource.EndEdit()
        End If

    End Sub

    Private Sub In_DatePicker_ValueChanged(sender As Object, e As EventArgs) Handles In_DatePicker.ValueChanged

        Manual_In_DateTime = True
        Me.Date_InLabel1.Text = In_DatePicker.Text + " " + In_TimePicker.Text

    End Sub

    Private Sub In_TimePicker_ValueChanged(sender As Object, e As EventArgs) Handles In_TimePicker.ValueChanged

        Manual_In_DateTime = True
        Me.Date_InLabel1.Text = In_DatePicker.Text + " " + In_TimePicker.Text

    End Sub

    Private Sub Out_DatePicker_ValueChanged(sender As Object, e As EventArgs) Handles Out_DatePicker.ValueChanged
        Manual_Out_DateTime = True
        Me.Date_OutLabel1.Text = Out_DatePicker.Text + " " + Out_TimePicker.Text
    End Sub

    Private Sub Out_TimePicker_ValueChanged(sender As Object, e As EventArgs) Handles Out_TimePicker.ValueChanged
        Manual_Out_DateTime = True
        Me.Date_OutLabel1.Text = Out_DatePicker.Text + " " + Out_TimePicker.Text
    End Sub

    Private Sub CarrierComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CarrierComboBox.SelectedIndexChanged

    End Sub
End Class
