Imports System.Windows.Forms

Public Class BasicTicket

    Dim Ticket As String = ""
    Dim SplitWeigh As Boolean
    Dim PupWeigh As Boolean
    Dim StoredWt As Int32 = 0
    Dim TrailerWt As Int32 = 0
    Dim LtWt As Int32 = 0

    Public Sub New(ByVal StorWt As Integer, ByVal TrlrWt As Integer)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        UpdateTimer.Start()

        If StorWt > 0 Then

            SplitWeigh = True                'If True we have at least one splitweigh active
            lblTruck.Text = StorWt.ToString
            lblTruck.Visible = True
            lblTrailer.Text = ""
            lblTrailer.Visible = True
            Label5.Visible = True
            Label6.Visible = True
            Label7.Visible = False
            lblPup.Visible = False

            btnSplitWeigh.Text = "Cancel Split"

            btnPup.Visible = True
            btnPup.Text = "Add Pup"

        Else

            SplitWeigh = False
            lblTruck.Text = ""
            lblTruck.Visible = False
            lblTrailer.Text = ""
            lblTrailer.Visible = False
            Label5.Visible = False
            Label6.Visible = False
            Label7.Visible = False
            lblPup.Visible = False

            btnSplitWeigh.Text = "Split Weigh"

            btnPup.Visible = False
            btnPup.Text = "Add Pup"

        End If

        StoredWt = StorWt

        If TrlrWt > 0 Then

            PupWeigh = True                    'If True we have a Pup Split active
            Label7.Visible = True
            lblPup.Visible = True
            btnPup.Text = "Remove Pup"
            lblTrailer.Text = TrlrWt.ToString

        Else

            PupWeigh = False
            Label7.Visible = False
            lblPup.Visible = False

            btnPup.Text = "Add Pup"
            lblTrailer.Text = ""

        End If

        TrailerWt = TrlrWt

        Me.ShowDialog()

    End Sub

    Private Sub UpdateTimer_Tick(sender As Object, e As EventArgs) Handles UpdateTimer.Tick

        Dim TotalWt As Integer = My.Forms.Main.Wt + StoredWt + TrailerWt

        Try

            lblTotal.Text = TotalWt.ToString

            If SplitWeigh And PupWeigh Then

                lblTruck.Text = StoredWt.ToString
                lblTrailer.Text = TrailerWt.ToString
                lblPup.Text = My.Forms.Main.Wt.ToString

            ElseIf SplitWeigh And PupWeigh = False Then

                lblTruck.Text = StoredWt.ToString
                lblTrailer.Text = My.Forms.Main.Wt.ToString
                Me.lblPup.Text = "0"

            Else

                lblTruck.Text = "0"
                lblTrailer.Text = "0"
                Me.lblPup.Text = "0"

            End If

        Catch ex As Exception

            lblTotal.Text = TotalWt.ToString

        End Try

        Me.lblMotion.Visible = My.Forms.Main.Motion
        Me.lblTimeDate.Text = Now.ToString

    End Sub

    Private Sub Cancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

        Me.Close()

    End Sub

    Private Sub btnSplitWeigh_Click(sender As Object, e As EventArgs) Handles btnSplitWeigh.Click

        If SplitWeigh Then

            SplitWeigh = False
            StoredWt = 0
            TrailerWt = 0
            lblTruck.Text = ""
            lblTrailer.Text = ""
            lblTruck.Visible = False
            lblTrailer.Visible = False
            lblPup.Visible = False
            Label5.Visible = False
            Label6.Visible = False
            Label7.Visible = False
            btnSplitWeigh.Text = "Split Weigh"
            btnPup.Text = "Add Pup"
            btnPup.Visible = False

        Else

            SplitWeigh = True
            StoredWt = My.Forms.Main.Wt
            lblTruck.Text = StoredWt.ToString
            lblTruck.Visible = True
            lblTrailer.Visible = True
            lblTrailer.Text = ""
            TrailerWt = 0
            lblPup.Visible = False
            Label5.Visible = True
            Label6.Visible = True
            Label7.Visible = False
            btnSplitWeigh.Text = "Cancel Split"
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

        Else

            PupWeigh = False
            TrailerWt = 0
            Label7.Visible = False
            lblPup.Visible = False
            btnPup.Text = "Add Pup"

        End If

    End Sub

    Private Sub Print_Click(sender As Object, e As EventArgs) Handles btnPrint.Click

        Try

            My.Forms.PrintingTicket.Show()
            My.Application.DoEvents()

            Dim SetupTableAdapter As New ScaleDataSetTableAdapters.SetupTableAdapter

            Dim BasicScaleDataSet As New ScaleDataSet

            SetupTableAdapter.Fill(BasicScaleDataSet.Setup)

            'Cannot use 'as new' must use 'as' because this table exists in the structure, you aren't creatking a new one but referencing an existing table
            'In essence you are saying "Here is your structure" = "Here is where you are being referenced"
            Dim BasicDT As ScaleDataSet.BasicTicketDataTable = BasicScaleDataSet.BasicTicket
            Dim BasicRow As ScaleDataSet.BasicTicketRow = BasicDT.NewBasicTicketRow()

            BasicRow.Company = txtCompany.Text
            BasicRow.Hauler = txtHauler.Text
            BasicRow.TruckID = txtTruckID.Text
            BasicRow.Comments = txtComments.Text
            BasicRow.Weight = Convert.ToDouble(lblTotal.Text)

            BasicDT.AddBasicTicketRow(BasicRow)

            Dim Rpt As New Object

            If Convert.ToInt16(My.Settings.TicketsperPage) = 3 Then
                Rpt = New Basic_3Part
            ElseIf Convert.ToInt16(My.Settings.TicketsperPage) = 2 Then
                Rpt = New Basic_2Part
            Else
                Rpt = New Basic_Ticket
            End If

            Rpt.SetDataSource(BasicScaleDataSet)

            '' The New Way to Get the Prenter Name Set
            Dim printReportOptions As New CrystalDecisions.ReportAppServer.Controllers.PrintReportOptions
            Dim printOutputController As New CrystalDecisions.ReportAppServer.Controllers.PrintOutputController

            Dim rptClientDoc As CrystalDecisions.ReportAppServer.ClientDoc.ISCDReportClientDocument
            rptClientDoc = Rpt.ReportClientDocument
            printReportOptions.PrinterName = My.Settings.PrinterName
            printReportOptions.NumberOfCopies = Convert.ToInt16(My.Settings.PagesToPrint)
            rptClientDoc.PrintOutputController.PrintReport(printReportOptions)

            Rpt = Nothing
            GC.Collect()
            My.Forms.PrintingTicket.Close()
            Me.Close()

        Catch ex As Exception
            MsgBox("An Error Occured during Printing = " + ex.Message)
        End Try

    End Sub
End Class