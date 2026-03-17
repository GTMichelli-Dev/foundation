Imports System.Windows.Forms

Public Class Completed_Trucks

    Dim ViewVoidedTickets As Boolean

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub Completed_Trucks_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        ViewVoidedTickets = False
        btnViewVoid.Text = "View Voided Tickets"
        Me.DateTimePicker1.Value = Now
        Me.DateTimePicker2.Value = Now
        Fill_Grid()

    End Sub

    Public Sub Fill_Grid()

        Dim StartDate As String = Me.DateTimePicker1.Value.Date + " 12:00:00AM"
        Dim EndDate As String = Me.DateTimePicker2.Value.Date.AddDays(1) + " 12:00:00AM"

        If ViewVoidedTickets = False Then
            Me.TransactionsTableAdapter.FillByDate_Out(Me.ScaleDataSet.Transactions, StartDate, EndDate)
        Else
            Me.TransactionsTableAdapter.FillByDateOutVoidedTicket(Me.ScaleDataSet.Transactions, StartDate, EndDate)
        End If

    End Sub

    Private Sub TransactionsDataGridView_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles TransactionsDataGridView.CellContentClick

        If e.ColumnIndex = Me.btnReprint.Index Then

            ModMain.TransactionRow = CType(CType(Me.TransactionsBindingSource.Current, DataRowView).Row, ScaleDataSet.TransactionsRow)
            ModMain.PrintOutboundTicket(ModMain.TransactionRow.Ticket)

        ElseIf e.ColumnIndex = Me.btnEdit.Index Then

            ModMain.TransactionRow = CType(CType(Me.TransactionsBindingSource.Current, DataRowView).Row, ScaleDataSet.TransactionsRow)
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        ElseIf e.ColumnIndex = Me.btnVoid.Index Then

            Try

                ModMain.TransactionRow = CType(CType(Me.TransactionsBindingSource.Current, DataRowView).Row, ScaleDataSet.TransactionsRow)

                If ViewVoidedTickets = False Then
                    ModMain.TransactionRow.Void = True
                Else
                    ModMain.TransactionRow.Void = False
                End If

                Me.Validate()
                Me.TransactionsBindingSource.EndEdit()
                Me.TableAdapterManager.UpdateAll(Me.ScaleDataSet)

                Fill_Grid()

            Catch ex As Exception
                MsgBox("Error Saving Information" + ex.Message)
            End Try

        End If

    End Sub

    Private Sub DateTimePicker1_CloseUp(ByVal sender As Object, ByVal e As System.EventArgs) Handles DateTimePicker1.CloseUp
        Fill_Grid()
    End Sub

    Private Sub dateTimePicker2_CloseUp(ByVal sender As Object, ByVal e As System.EventArgs) Handles DateTimePicker2.CloseUp
        Fill_Grid()
    End Sub

    Private Sub DateTimePicker1_QueryAccessibilityHelp(ByVal sender As Object, ByVal e As System.Windows.Forms.QueryAccessibilityHelpEventArgs) Handles DateTimePicker1.QueryAccessibilityHelp

    End Sub

    Private Sub DateTimePicker1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DateTimePicker1.ValueChanged

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

    Private Sub Label1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub btnViewVoid_Click(sender As Object, e As EventArgs) Handles btnViewVoid.Click

        If ViewVoidedTickets = True Then

            btnViewVoid.Text = "View Voided Tickets"
            ViewVoidedTickets = False
            Label1.Text = "Completed Trucks"
            Fill_Grid()

        Else

            btnViewVoid.Text = "View Active Tickets"
            ViewVoidedTickets = True
            Label1.Text = "Voided Tickets"
            Fill_Grid()

        End If
    End Sub
End Class
