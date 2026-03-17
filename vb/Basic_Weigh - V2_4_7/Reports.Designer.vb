<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Reports
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.StartDate = New System.Windows.Forms.DateTimePicker()
        Me.EndDate = New System.Windows.Forms.DateTimePicker()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.cboProducts = New System.Windows.Forms.ComboBox()
        Me.DistinctTransactionProductsBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ScaleDataSet = New Basic_Weigh.ScaleDataSet()
        Me.cboLocations = New System.Windows.Forms.ComboBox()
        Me.DistinctTransactionLocationsBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.cboCustomers = New System.Windows.Forms.ComboBox()
        Me.DistinctTransactionCustomesBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.Distinct_Transaction_CustomesTableAdapter = New Basic_Weigh.ScaleDataSetTableAdapters.Distinct_Transaction_CustomesTableAdapter()
        Me.Distinct_Transaction_ProductsTableAdapter = New Basic_Weigh.ScaleDataSetTableAdapters.Distinct_Transaction_ProductsTableAdapter()
        Me.Distinct_Transaction_LocationsTableAdapter = New Basic_Weigh.ScaleDataSetTableAdapters.Distinct_Transaction_LocationsTableAdapter()
        Me.cboHaulers = New System.Windows.Forms.ComboBox()
        Me.DistinctTransactionsHaulerBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Distinct_Transactions_HaulerTableAdapter = New Basic_Weigh.ScaleDataSetTableAdapters.Distinct_Transactions_HaulerTableAdapter()
        Me.CrystalReportViewer1 = New CrystalDecisions.Windows.Forms.CrystalReportViewer()
        Me.Transaction_Report1 = New Basic_Weigh.Transaction_Report()
        CType(Me.DistinctTransactionProductsBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ScaleDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DistinctTransactionLocationsBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DistinctTransactionCustomesBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DistinctTransactionsHaulerBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 24.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(505, 11)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(312, 46)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Reports Screen"
        '
        'StartDate
        '
        Me.StartDate.Location = New System.Drawing.Point(149, 87)
        Me.StartDate.Margin = New System.Windows.Forms.Padding(4)
        Me.StartDate.Name = "StartDate"
        Me.StartDate.Size = New System.Drawing.Size(265, 22)
        Me.StartDate.TabIndex = 1
        '
        'EndDate
        '
        Me.EndDate.Location = New System.Drawing.Point(1027, 87)
        Me.EndDate.Margin = New System.Windows.Forms.Padding(4)
        Me.EndDate.Name = "EndDate"
        Me.EndDate.Size = New System.Drawing.Size(265, 22)
        Me.EndDate.TabIndex = 2
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(37, 91)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(93, 18)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Date From:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(937, 90)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(73, 18)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Date To:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(117, 143)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(123, 25)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "Customers:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(798, 143)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(104, 25)
        Me.Label5.TabIndex = 10
        Me.Label5.Text = "Products:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(1082, 143)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(194, 25)
        Me.Label6.TabIndex = 11
        Me.Label6.Text = "Storage Locations:"
        '
        'btnUpdate
        '
        Me.btnUpdate.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnUpdate.Location = New System.Drawing.Point(572, 79)
        Me.btnUpdate.Margin = New System.Windows.Forms.Padding(4)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(216, 46)
        Me.btnUpdate.TabIndex = 12
        Me.btnUpdate.Text = "Update Report"
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'cboProducts
        '
        Me.cboProducts.DataSource = Me.DistinctTransactionProductsBindingSource
        Me.cboProducts.DisplayMember = "Text"
        Me.cboProducts.FormattingEnabled = True
        Me.cboProducts.Location = New System.Drawing.Point(693, 175)
        Me.cboProducts.Margin = New System.Windows.Forms.Padding(4)
        Me.cboProducts.Name = "cboProducts"
        Me.cboProducts.Size = New System.Drawing.Size(320, 24)
        Me.cboProducts.TabIndex = 14
        Me.cboProducts.ValueMember = "Value"
        '
        'DistinctTransactionProductsBindingSource
        '
        Me.DistinctTransactionProductsBindingSource.DataMember = "Distinct_Transaction_Products"
        Me.DistinctTransactionProductsBindingSource.DataSource = Me.ScaleDataSet
        '
        'ScaleDataSet
        '
        Me.ScaleDataSet.DataSetName = "ScaleDataSet"
        Me.ScaleDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'cboLocations
        '
        Me.cboLocations.DataSource = Me.DistinctTransactionLocationsBindingSource
        Me.cboLocations.DisplayMember = "Text"
        Me.cboLocations.FormattingEnabled = True
        Me.cboLocations.Location = New System.Drawing.Point(1027, 175)
        Me.cboLocations.Margin = New System.Windows.Forms.Padding(4)
        Me.cboLocations.Name = "cboLocations"
        Me.cboLocations.Size = New System.Drawing.Size(320, 24)
        Me.cboLocations.TabIndex = 15
        Me.cboLocations.ValueMember = "Value"
        '
        'DistinctTransactionLocationsBindingSource
        '
        Me.DistinctTransactionLocationsBindingSource.DataMember = "Distinct_Transaction_Locations"
        Me.DistinctTransactionLocationsBindingSource.DataSource = Me.ScaleDataSet
        '
        'cboCustomers
        '
        Me.cboCustomers.DataSource = Me.DistinctTransactionCustomesBindingSource
        Me.cboCustomers.DisplayMember = "Text"
        Me.cboCustomers.FormattingEnabled = True
        Me.cboCustomers.Location = New System.Drawing.Point(16, 175)
        Me.cboCustomers.Margin = New System.Windows.Forms.Padding(4)
        Me.cboCustomers.Name = "cboCustomers"
        Me.cboCustomers.Size = New System.Drawing.Size(320, 24)
        Me.cboCustomers.TabIndex = 16
        Me.cboCustomers.ValueMember = "Value"
        '
        'DistinctTransactionCustomesBindingSource
        '
        Me.DistinctTransactionCustomesBindingSource.DataMember = "Distinct_Transaction_Customes"
        Me.DistinctTransactionCustomesBindingSource.DataSource = Me.ScaleDataSet
        '
        'Distinct_Transaction_CustomesTableAdapter
        '
        Me.Distinct_Transaction_CustomesTableAdapter.ClearBeforeFill = True
        '
        'Distinct_Transaction_ProductsTableAdapter
        '
        Me.Distinct_Transaction_ProductsTableAdapter.ClearBeforeFill = True
        '
        'Distinct_Transaction_LocationsTableAdapter
        '
        Me.Distinct_Transaction_LocationsTableAdapter.ClearBeforeFill = True
        '
        'cboHaulers
        '
        Me.cboHaulers.DataSource = Me.DistinctTransactionsHaulerBindingSource
        Me.cboHaulers.DisplayMember = "Text"
        Me.cboHaulers.FormattingEnabled = True
        Me.cboHaulers.Location = New System.Drawing.Point(351, 175)
        Me.cboHaulers.Margin = New System.Windows.Forms.Padding(4)
        Me.cboHaulers.Name = "cboHaulers"
        Me.cboHaulers.Size = New System.Drawing.Size(320, 24)
        Me.cboHaulers.TabIndex = 19
        Me.cboHaulers.ValueMember = "Value"
        '
        'DistinctTransactionsHaulerBindingSource
        '
        Me.DistinctTransactionsHaulerBindingSource.DataMember = "Distinct_Transactions_Hauler"
        Me.DistinctTransactionsHaulerBindingSource.DataSource = Me.ScaleDataSet
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(465, 143)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(93, 25)
        Me.Label7.TabIndex = 18
        Me.Label7.Text = "Haulers:"
        '
        'Distinct_Transactions_HaulerTableAdapter
        '
        Me.Distinct_Transactions_HaulerTableAdapter.ClearBeforeFill = True
        '
        'CrystalReportViewer1
        '
        Me.CrystalReportViewer1.ActiveViewIndex = -1
        Me.CrystalReportViewer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.CrystalReportViewer1.Cursor = System.Windows.Forms.Cursors.Default
        Me.CrystalReportViewer1.Location = New System.Drawing.Point(16, 213)
        Me.CrystalReportViewer1.Name = "CrystalReportViewer1"
        Me.CrystalReportViewer1.Size = New System.Drawing.Size(1339, 539)
        Me.CrystalReportViewer1.TabIndex = 20
        '
        'Reports
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.ClientSize = New System.Drawing.Size(1367, 764)
        Me.Controls.Add(Me.CrystalReportViewer1)
        Me.Controls.Add(Me.cboHaulers)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.cboCustomers)
        Me.Controls.Add(Me.cboLocations)
        Me.Controls.Add(Me.cboProducts)
        Me.Controls.Add(Me.btnUpdate)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.EndDate)
        Me.Controls.Add(Me.StartDate)
        Me.Controls.Add(Me.Label1)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "Reports"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Reports"
        Me.TopMost = True
        CType(Me.DistinctTransactionProductsBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ScaleDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DistinctTransactionLocationsBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DistinctTransactionCustomesBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DistinctTransactionsHaulerBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents StartDate As DateTimePicker
    Friend WithEvents EndDate As DateTimePicker
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents btnUpdate As Button
    Friend WithEvents cboProducts As ComboBox
    Friend WithEvents cboLocations As ComboBox
    Friend WithEvents cboCustomers As ComboBox
    Friend WithEvents ScaleDataSet As ScaleDataSet
    Friend WithEvents DistinctTransactionCustomesBindingSource As BindingSource
    Friend WithEvents Distinct_Transaction_CustomesTableAdapter As ScaleDataSetTableAdapters.Distinct_Transaction_CustomesTableAdapter
    Friend WithEvents DistinctTransactionProductsBindingSource As BindingSource
    Friend WithEvents Distinct_Transaction_ProductsTableAdapter As ScaleDataSetTableAdapters.Distinct_Transaction_ProductsTableAdapter
    Friend WithEvents DistinctTransactionLocationsBindingSource As BindingSource
    Friend WithEvents Distinct_Transaction_LocationsTableAdapter As ScaleDataSetTableAdapters.Distinct_Transaction_LocationsTableAdapter
    Friend WithEvents cboHaulers As ComboBox
    Friend WithEvents Label7 As Label
    Friend WithEvents DistinctTransactionsHaulerBindingSource As BindingSource
    Friend WithEvents Distinct_Transactions_HaulerTableAdapter As ScaleDataSetTableAdapters.Distinct_Transactions_HaulerTableAdapter
    Friend WithEvents CrystalReportViewer1 As CrystalDecisions.Windows.Forms.CrystalReportViewer
    Friend WithEvents Transaction_Report1 As Transaction_Report
End Class
