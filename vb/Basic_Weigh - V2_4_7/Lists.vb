Imports System.Windows.Forms

Public Class Lists

    Dim CurrentBindingSource As New BindingSource
    Dim BindingSourceDictionary As New Dictionary(Of String, BindingSource)
    Dim DataError As Boolean = False
    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        If SaveAll() = True Then
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End If

    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub Selection_Lists_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        'TODO: This line of code loads data into the 'ScaleDataSet.Location' table. You can move, or remove it, as needed.
        Me.LocationTableAdapter.Fill(Me.ScaleDataSet.Location)
        'TODO: This line of code loads data into the 'ScaleDataS Me.TruckTableAdapter.Fill(Me.ScaleDataSet.Truck)
        TruckTableAdapter.Fill(Me.ScaleDataSet.Truck)
        'TODO: This line of code loads data into the 'ScaleDataSet.Destination' table. You can move, or remove it, as needed.
        Me.CommodityTableAdapter.Fill(Me.ScaleDataSet.Commodity)
        'TODO: This line of code loads data into the 'ScaleDataSet.Customer' table. You can move, or remove it, as needed.
        Me.CustomerTableAdapter.Fill(Me.ScaleDataSet.Customer)
        'TODO: This line of code loads data into the 'ScaleDataSet.Carrier' table. You can move, or remove it, as needed.
        Me.CarrierTableAdapter.Fill(Me.ScaleDataSet.Carrier)
        BindingSourceDictionary.Clear()

        Me.LocationTableAdapter.Fill(Me.ScaleDataSet.Location)
        BindingSourceDictionary.Add("Locations", LocationBindingSource)
        Me.TruckTableAdapter.Fill(Me.ScaleDataSet.Truck)
        BindingSourceDictionary.Add("Truck IDs", TruckBindingSource)
        Me.CommodityTableAdapter.Fill(Me.ScaleDataSet.Commodity)
        BindingSourceDictionary.Add("Products", CommodityBindingSource)
        Me.CarrierTableAdapter.Fill(Me.ScaleDataSet.Carrier)
        BindingSourceDictionary.Add("Haulers", CarrierBindingSource)
        Me.CustomerTableAdapter.Fill(Me.ScaleDataSet.Customer)
        BindingSourceDictionary.Add("Customers", CustomerBindingSource)

        Me.TableList.Items.Clear()

        For Each key As String In BindingSourceDictionary.Keys
            Me.TableList.Items.Add(key)
        Next
        Me.TableList.Sorted = True

    End Sub

    Private Sub BindingNavigatorSaveItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BindingNavigatorSaveItem.Click
        SaveAll()
    End Sub


    Private Function SaveAll() As Boolean

        Try
            Me.Validate()
            Me.CurrentBindingSource.EndEdit()
            Me.TableAdapterManager.UpdateAll(Me.ScaleDataSet)
            SaveAll = True
        Catch ex As Exception
            MessageBox.Show(" Error " + ex.Message)
            SaveAll = False
        End Try

    End Function

    Private Sub TableList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TableList.SelectedIndexChanged

        Me.CurrentBindingSource = Me.BindingSourceDictionary(Me.TableList.SelectedItem)

        Me.CurrentDataGridView.DataSource = Me.CurrentBindingSource
        Me.CurrentDataGridView.Columns.Clear()

        For I As Integer = 0 To Me.ScaleDataSet.Tables(CurrentBindingSource.DataMember).Columns.Count - 1

            Dim RowName As String = Me.ScaleDataSet.Tables(CurrentBindingSource.DataMember).Columns(I).ColumnName
            Dim BoolColumn As New System.Windows.Forms.DataGridViewCheckBoxColumn
            Dim TestBool As Boolean

            If Me.ScaleDataSet.Tables(CurrentBindingSource.DataMember).Columns(I).DataType Is TestBool.GetType Then

                BoolColumn.DataPropertyName = RowName
                BoolColumn.HeaderText = RowName
                Me.CurrentDataGridView.Columns.Add(BoolColumn)

            Else

                Me.CurrentDataGridView.Columns.Add(Me.CurrentBindingSource.DataMember, RowName)
                Me.CurrentDataGridView.Columns(I).DataPropertyName = RowName

            End If

            If I = 0 Then
                Me.CurrentDataGridView.Columns(I).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            Else
                Me.CurrentDataGridView.Columns(I).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            End If

        Next

        Me.BindingNavigator.BindingSource = Me.CurrentBindingSource
        Me.lblTable.Text = Me.TableList.SelectedItem
        Me.CurrentDataGridView.Refresh()

    End Sub

    Private Sub CurrentDataGridView_BindingContextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CurrentDataGridView.BindingContextChanged
        Me.lblError.Text = ""
    End Sub

    Private Sub CurrentDataGridView_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles CurrentDataGridView.CellContentClick

    End Sub

    Private Sub CurrentDataGridView_CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles CurrentDataGridView.CellEnter
        If DataError = False Then Me.lblError.Text = ""
        DataError = False
    End Sub

    Private Sub CurrentDataGridView_DataError(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles CurrentDataGridView.DataError
        Me.lblError.Text = e.Exception.Message
        DataError = True
    End Sub



    Private Sub CurrentDataGridView_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles CurrentDataGridView.Validated
        Me.lblError.Text = ""
    End Sub


End Class
