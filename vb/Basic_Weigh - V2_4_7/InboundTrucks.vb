Imports System.Windows.Forms

Public Class InboundTrucks

    Dim FormType As Boolean  '  True = Inbound Trucks, False = Saved Trucks

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub TransactionsBindingNavigatorSaveItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TransactionsBindingNavigatorSaveItem.Click
        Try
            Me.Validate()
            Me.TransactionsBindingSource.EndEdit()
            Me.TableAdapterManager.UpdateAll(Me.ScaleDataSet)

        Catch ex As Exception
            MsgBox("Error Saving Information" + ex.Message)

        End Try

    End Sub

    Private Sub InboundTrucks_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'TODO: This line of code loads data into the 'ScaleDataSet.Truck' table. You can move, or remove it, as needed.
        Me.TruckTableAdapter.Fill(Me.ScaleDataSet.Truck)

        'TODO: This line of code loads data into the 'ScaleDataSet.Transactions' table. You can move, or remove it, as needed.
        Me.TransactionsTableAdapter.FillByInbound(Me.ScaleDataSet.Transactions)

        ModMain.TransactionRow = Nothing
        btnSelectType.Text = "Switch To Saved Trucks"
        btnSelectType.BackColor = Color.WhiteSmoke
        Label1.Text = "Trucks In Yard"

        TransactionsDataGridView.Visible = True
        TransactionsDataGridView.Enabled = True
        FormType = True

        TrucksDataGridView.Visible = False
        TrucksDataGridView.Enabled = False

    End Sub

    Private Sub TransactionsDataGridView_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles TransactionsDataGridView.CellContentClick
        If e.ColumnIndex = Me.btnSelect.Index Then
            ModMain.TransactionRow = CType(CType(Me.TransactionsBindingSource.Current, DataRowView).Row, ScaleDataSet.TransactionsRow)
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        ElseIf e.ColumnIndex = Me.btnReprint.Index Then
            ModMain.TransactionRow = CType(CType(Me.TransactionsBindingSource.Current, DataRowView).Row, ScaleDataSet.TransactionsRow)
            ModMain.PrintInboundTicket(ModMain.TransactionRow.Ticket)
        End If
    End Sub


    Private Sub btnSelectType_Click(sender As Object, e As EventArgs) Handles btnSelectType.Click

        If FormType = True Then

            btnSelectType.Text = "Switch To Trucks In Yard"

            TransactionsDataGridView.Visible = False
            TransactionsDataGridView.Enabled = False
            FormType = False

            TrucksDataGridView.Visible = True
            TrucksDataGridView.Enabled = True
            Label1.Text = "Saved Trucks"

        Else

            btnSelectType.Text = "Switch To Saved Trucks"

            TransactionsDataGridView.Visible = True
            TransactionsDataGridView.Enabled = True
            FormType = True

            TrucksDataGridView.Visible = False
            TrucksDataGridView.Enabled = False
            Label1.Text = "Trucks In Yard"


        End If

    End Sub

    Private Sub TrucksDataGridView_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles TrucksDataGridView.CellContentClick

        Dim SelectedTruck As String
        Dim TruckLightWt As Integer

        If e.ColumnIndex = Me.btnSelectTruck.Index Then

            Dim selectedtruckrow As ScaleDataSet.TruckRow  'Create a row to put the record into

            selectedtruckrow = CType(CType(TruckBindingSource.Current, DataRowView).Row, ScaleDataSet.TruckRow)

            SelectedTruck = selectedtruckrow.Truck_ID.ToString  'Set my variables to the values of that truck record
            TruckLightWt = selectedtruckrow.Light_Wt

            Dim SetupTableAdapter As New ScaleDataSetTableAdapters.SetupTableAdapter
            SetupTableAdapter.Fill(ScaleDataSet.Setup)
            Dim Q As New ScaleDataSetTableAdapters.QueriesTableAdapter

            Dim NextTicket As Integer = 0

            If Me.TransactionsTableAdapter.FillByLastTicket(Me.ScaleDataSet.Transactions) > 0 Then
                Integer.TryParse(Me.ScaleDataSet.Transactions(0).Ticket, NextTicket)
            End If 'Find the new row that was added.
            NextTicket += 1

            'While Me.ScaleDataSet.Transactions.Count > 0
            '    ScaleDataSet.Setup(0).Ticket_Number += 1
            '    Ticket = ScaleDataSet.Setup(0).Ticket_Number.ToString
            '    Me.TransactionsTableAdapter.FillByTicket(Me.ScaleDataSet.Transactions, Ticket)
            'End While
            'SetupTableAdapter.Update(Me.ScaleDataSet)
            TransactionRow = Me.ScaleDataSet.Transactions.NewTransactionsRow 'Create a new transaction row
            TransactionRow.Date_In = Now                                     'Add data to the row
            TransactionRow.Date_Out = Now
            TransactionRow.Truck_ID = SelectedTruck
            TransactionRow.In_Weight = TruckLightWt
            TransactionRow.Ticket =NextTicket.ToString()
            Me.ScaleDataSet.Transactions.AddTransactionsRow(TransactionRow)  'Add the row to the database




            ModMain.TransactionRow.Ticket = NextTicket.ToString()
            Me.TransactionsTableAdapter.Update(ScaleDataSet.Transactions)



            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        End If

    End Sub

    Private Sub BindingNavigatorDeleteItem_Click(sender As Object, e As EventArgs) Handles BindingNavigatorDeleteItem.Click

    End Sub
End Class
