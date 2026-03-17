Imports CrystalDecisions
Imports CrystalDecisions.CrystalReports
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared


Module ModMain

    Public TransactionRow As ScaleDataSet.TransactionsRow

    Public Sub PrintOutboundTicket(ByVal Ticket As String)

        Try

            Using Rpt As New CrystalDecisions.CrystalReports.Engine.ReportDocument

                My.Forms.PrintingTicket.Show()
                My.Application.DoEvents()


                Dim SetupTableAdapter As New ScaleDataSetTableAdapters.SetupTableAdapter
                Dim TransactionsTableAdapter As New ScaleDataSetTableAdapters.TransactionsTableAdapter
                Dim ReportScaleDataSet As New ScaleDataSet
                TransactionsTableAdapter.FillByTicket(ReportScaleDataSet.Transactions, Ticket)
                SetupTableAdapter.Fill(ReportScaleDataSet.Setup)

                ' Dim Rpt As Object

                If Convert.ToInt16(My.Settings.TicketsperPage) = 3 Then

                    'Rpt = New Ticket_3_Part
                    Rpt.Load("Ticket_3_Part.rpt")
                    Rpt.Subreports(0).SetDataSource(ReportScaleDataSet)
                    Rpt.Subreports(1).SetDataSource(ReportScaleDataSet)
                    Rpt.Subreports(2).SetDataSource(ReportScaleDataSet)

                ElseIf Convert.ToInt16(My.Settings.TicketsperPage) = 2 Then

                    'Rpt = New Ticket_2_Part
                    Rpt.Load("Ticket_2_Part.rpt")
                    Rpt.Subreports(0).SetDataSource(ReportScaleDataSet)
                    Rpt.Subreports(1).SetDataSource(ReportScaleDataSet)

                Else

                    'Rpt = New Outbound_Ticket
                    Rpt.Load("Outbound_Ticket")

                End If

                Rpt.SetDataSource(ReportScaleDataSet)

                '' The New Way to Get the Printer Name Set
                Dim printReportOptions As New CrystalDecisions.ReportAppServer.Controllers.PrintReportOptions
                Dim printOutputController As New CrystalDecisions.ReportAppServer.Controllers.PrintOutputController

                Dim rptClientDoc As CrystalDecisions.ReportAppServer.ClientDoc.ISCDReportClientDocument
                rptClientDoc = Rpt.ReportClientDocument
                printReportOptions.PrinterName = My.Settings.PrinterName
                Int32.TryParse(My.Settings.PagesToPrint, printReportOptions.NumberOfCopies)
                printReportOptions.JobTitle = "Outbound Ticket"

                rptClientDoc.PrintOutputController.PrintReport(printReportOptions)

                Rpt.Close()
                Rpt.Dispose()

                'Rpt = Nothing
                'GC.Collect()

            End Using

        Catch ex As Exception

            MsgBox("Error Printing Ticket - " + ex.Message)

        End Try

        GC.Collect()

        My.Forms.PrintingTicket.Close()

    End Sub


    Public Sub PrintInboundTicket(ByVal Ticket As String)
        Try

            My.Forms.PrintingTicket.Show()
            My.Application.DoEvents()

            Using Rpt As New CrystalDecisions.CrystalReports.Engine.ReportDocument

                Dim SetupTableAdapter As New ScaleDataSetTableAdapters.SetupTableAdapter
                Dim TransactionsTableAdapter As New ScaleDataSetTableAdapters.TransactionsTableAdapter
                Dim ReportScaleDataSet As New ScaleDataSet
                TransactionsTableAdapter.FillByTicket(ReportScaleDataSet.Transactions, Ticket)
                SetupTableAdapter.Fill(ReportScaleDataSet.Setup)

                'Dim Rpt As New Object              -- This was the original method used and it caused an error when enough tickets were generated.

                If Convert.ToInt16(My.Settings.TicketsperPage) = 3 Then

                    'Rpt = New InTicket_3Part
                    Rpt.Load("InTicket_3Part.rpt")

                ElseIf Convert.ToInt16(My.Settings.TicketsperPage) = 2 Then

                    'Rpt = New InTicket_2Part
                    Rpt.Load("InTicket_2Part.rpt")

                Else

                    'Rpt = New Inbound_Ticket
                    Rpt.Load("Inbound_Ticket.rpt")

                End If

                Rpt.SetDataSource(ReportScaleDataSet)

                '' The New Way to Get the Prenter Name Set
                Dim printReportOptions As New CrystalDecisions.ReportAppServer.Controllers.PrintReportOptions
                Dim printOutputController As New CrystalDecisions.ReportAppServer.Controllers.PrintOutputController

                Dim rptClientDoc As CrystalDecisions.ReportAppServer.ClientDoc.ISCDReportClientDocument
                rptClientDoc = Rpt.ReportClientDocument
                printReportOptions.PrinterName = My.Settings.PrinterName
                Int32.TryParse(My.Settings.PagesToPrint, printReportOptions.NumberOfCopies)
                printReportOptions.JobTitle = "Inbound Ticket"
                rptClientDoc.PrintOutputController.PrintReport(printReportOptions)

                Rpt.Close()
                Rpt.Dispose()

                'Rpt = Nothing
                'GC.Collect()

            End Using

        Catch ex As Exception

            MsgBox("Error Printing Ticket - " + ex.Message)

        End Try

        GC.Collect()
        My.Forms.PrintingTicket.Close()

    End Sub

End Module
