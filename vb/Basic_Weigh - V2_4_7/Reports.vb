Imports System.ComponentModel
Imports CrystalDecisions.CrystalReports.Engine

Public Class Reports

    Dim Rpt As New CrystalDecisions.CrystalReports.Engine.ReportDocument

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Distinct_Transaction_CustomesTableAdapter.Fill(Me.ScaleDataSet.Distinct_Transaction_Customes)
        Me.Distinct_Transaction_LocationsTableAdapter.Fill(Me.ScaleDataSet.Distinct_Transaction_Locations)
        Me.Distinct_Transaction_ProductsTableAdapter.Fill(Me.ScaleDataSet.Distinct_Transaction_Products)
        cboCustomers.SelectedIndex = 0
        cboLocations.SelectedIndex = 0
        cboProducts.SelectedIndex = 0

        LoadData()

    End Sub

    Private Sub Reports_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'TODO: This line of code loads data into the 'ScaleDataSet.Distinct_Transactions_Hauler' table. You can move, or remove it, as needed.
        Me.Distinct_Transactions_HaulerTableAdapter.Fill(Me.ScaleDataSet.Distinct_Transactions_Hauler)

        StartDate.Value = Now
        EndDate.Value = Now

    End Sub

    Private Sub LoadData()

        Try


            Rpt.Load("Transaction_Report.rpt")

            Dim MyDataSet As New ScaleDataSet

            Using TransactionsTA As New ScaleDataSetTableAdapters.TransactionsTableAdapter

                Using SetupTA As New ScaleDataSetTableAdapters.SetupTableAdapter

                    Dim Start_Date As DateTime = StartDate.Value.Date + " 12:00:00AM"
                    Dim End_Date As DateTime = EndDate.Value.Date.AddDays(1) + " 12:00:00AM"

                    ' Each of these sets an object to the current value of the combobox selected item.  All denotes a null and that won't work with strings

                    Dim Customer As Object = Nothing ' cboCustomers.SelectedValue
                    Dim Commodity As Object = Nothing 'cboProducts.SelectedValue
                    Dim Location As Object = Nothing 'cboLocations.SelectedValue
                    Dim Hauler As Object = Nothing 'cboHauler.SelectedValue
                    If cboCustomers.SelectedIndex > 0 Then Customer = cboCustomers.Text
                    If cboProducts.SelectedIndex > 0 Then Commodity = cboProducts.Text
                    If cboLocations.SelectedIndex > 0 Then Location = cboLocations.Text
                    If cboHaulers.SelectedIndex > 0 Then Hauler = cboHaulers.Text

                    'This loads the setup table into the Mydataset Setup Table
                    SetupTA.Fill(MyDataSet.Setup)

                    'This loads the transactions into the MyDataSet Transactions table

                    ' *** If you get an Error - Cannot Send Null Values - Then checked the properties for this table adapter and set Customer, Location, and Destinations to allow Nulls.
                    ' TransactionsTA.FillByStartEndCustCommLoc(MyDataSet.Transactions, Start_Date, End_Date, Customer, Location, Commodity)
                    TransactionsTA.FillByStartEndCustCarrierCommLoc(MyDataSet.Transactions, Start_Date, End_Date, Customer, Location, Commodity, Hauler)
                    Rpt.SetDataSource(MyDataSet)
                    CrystalReportViewer1.ReportSource = Rpt
                    CrystalReportViewer1.Zoom(75)
                    CrystalReportViewer1.RefreshReport()

                End Using

            End Using

        Catch ex As Exception
            MsgBox("Error Viewing Report - " + ex.ToString)
        End Try

    End Sub



    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        LoadData()
    End Sub

    Private Sub Reports_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

        Rpt.Close()
        Rpt.Dispose()
        GC.Collect()

    End Sub
End Class