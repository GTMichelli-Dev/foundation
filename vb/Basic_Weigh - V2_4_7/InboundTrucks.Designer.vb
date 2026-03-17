<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class InboundTrucks
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(InboundTrucks))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.TransactionsBindingNavigator = New System.Windows.Forms.BindingNavigator(Me.components)
        Me.BindingNavigatorCountItem = New System.Windows.Forms.ToolStripLabel()
        Me.BindingNavigatorDeleteItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMoveFirstItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMovePreviousItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.BindingNavigatorPositionItem = New System.Windows.Forms.ToolStripTextBox()
        Me.BindingNavigatorSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.BindingNavigatorMoveNextItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMoveLastItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.TransactionsBindingNavigatorSaveItem = New System.Windows.Forms.ToolStripButton()
        Me.TransactionsDataGridView = New System.Windows.Forms.DataGridView()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.TrucksDataGridView = New System.Windows.Forms.DataGridView()
        Me.btnSelectTruck = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.btnSelectType = New System.Windows.Forms.Button()
        Me.TruckIDDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.LightWtDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TruckBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ScaleDataSet = New Basic_Weigh.ScaleDataSet()
        Me.TransactionsBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.TransactionsTableAdapter = New Basic_Weigh.ScaleDataSetTableAdapters.TransactionsTableAdapter()
        Me.TableAdapterManager = New Basic_Weigh.ScaleDataSetTableAdapters.TableAdapterManager()
        Me.TruckTableAdapter = New Basic_Weigh.ScaleDataSetTableAdapters.TruckTableAdapter()
        Me.btnSelect = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.btnReprint = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Truck_ID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Customer = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Carrier = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Commodity = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn14 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.TransactionsBindingNavigator, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TransactionsBindingNavigator.SuspendLayout()
        CType(Me.TransactionsDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        CType(Me.TrucksDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TruckBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ScaleDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TransactionsBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TransactionsBindingNavigator
        '
        Me.TransactionsBindingNavigator.AddNewItem = Nothing
        Me.TransactionsBindingNavigator.BindingSource = Me.TransactionsBindingSource
        Me.TransactionsBindingNavigator.CountItem = Me.BindingNavigatorCountItem
        Me.TransactionsBindingNavigator.DeleteItem = Me.BindingNavigatorDeleteItem
        Me.TransactionsBindingNavigator.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.TransactionsBindingNavigator.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BindingNavigatorMoveFirstItem, Me.BindingNavigatorMovePreviousItem, Me.BindingNavigatorSeparator, Me.BindingNavigatorPositionItem, Me.BindingNavigatorCountItem, Me.BindingNavigatorSeparator1, Me.BindingNavigatorMoveNextItem, Me.BindingNavigatorMoveLastItem, Me.BindingNavigatorSeparator2, Me.BindingNavigatorDeleteItem, Me.TransactionsBindingNavigatorSaveItem})
        Me.TransactionsBindingNavigator.Location = New System.Drawing.Point(0, 0)
        Me.TransactionsBindingNavigator.MoveFirstItem = Me.BindingNavigatorMoveFirstItem
        Me.TransactionsBindingNavigator.MoveLastItem = Me.BindingNavigatorMoveLastItem
        Me.TransactionsBindingNavigator.MoveNextItem = Me.BindingNavigatorMoveNextItem
        Me.TransactionsBindingNavigator.MovePreviousItem = Me.BindingNavigatorMovePreviousItem
        Me.TransactionsBindingNavigator.Name = "TransactionsBindingNavigator"
        Me.TransactionsBindingNavigator.PositionItem = Me.BindingNavigatorPositionItem
        Me.TransactionsBindingNavigator.Size = New System.Drawing.Size(1199, 27)
        Me.TransactionsBindingNavigator.TabIndex = 1
        Me.TransactionsBindingNavigator.Text = "BindingNavigator1"
        '
        'BindingNavigatorCountItem
        '
        Me.BindingNavigatorCountItem.Name = "BindingNavigatorCountItem"
        Me.BindingNavigatorCountItem.Size = New System.Drawing.Size(35, 24)
        Me.BindingNavigatorCountItem.Text = "of {0}"
        Me.BindingNavigatorCountItem.ToolTipText = "Total number of items"
        '
        'BindingNavigatorDeleteItem
        '
        Me.BindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorDeleteItem.Image = CType(resources.GetObject("BindingNavigatorDeleteItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorDeleteItem.Name = "BindingNavigatorDeleteItem"
        Me.BindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorDeleteItem.Size = New System.Drawing.Size(24, 24)
        Me.BindingNavigatorDeleteItem.Text = "Delete"
        '
        'BindingNavigatorMoveFirstItem
        '
        Me.BindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMoveFirstItem.Image = CType(resources.GetObject("BindingNavigatorMoveFirstItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorMoveFirstItem.Name = "BindingNavigatorMoveFirstItem"
        Me.BindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorMoveFirstItem.Size = New System.Drawing.Size(24, 24)
        Me.BindingNavigatorMoveFirstItem.Text = "Move first"
        '
        'BindingNavigatorMovePreviousItem
        '
        Me.BindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMovePreviousItem.Image = CType(resources.GetObject("BindingNavigatorMovePreviousItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorMovePreviousItem.Name = "BindingNavigatorMovePreviousItem"
        Me.BindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorMovePreviousItem.Size = New System.Drawing.Size(24, 24)
        Me.BindingNavigatorMovePreviousItem.Text = "Move previous"
        '
        'BindingNavigatorSeparator
        '
        Me.BindingNavigatorSeparator.Name = "BindingNavigatorSeparator"
        Me.BindingNavigatorSeparator.Size = New System.Drawing.Size(6, 27)
        '
        'BindingNavigatorPositionItem
        '
        Me.BindingNavigatorPositionItem.AccessibleName = "Position"
        Me.BindingNavigatorPositionItem.AutoSize = False
        Me.BindingNavigatorPositionItem.Name = "BindingNavigatorPositionItem"
        Me.BindingNavigatorPositionItem.Size = New System.Drawing.Size(50, 23)
        Me.BindingNavigatorPositionItem.Text = "0"
        Me.BindingNavigatorPositionItem.ToolTipText = "Current position"
        '
        'BindingNavigatorSeparator1
        '
        Me.BindingNavigatorSeparator1.Name = "BindingNavigatorSeparator1"
        Me.BindingNavigatorSeparator1.Size = New System.Drawing.Size(6, 27)
        '
        'BindingNavigatorMoveNextItem
        '
        Me.BindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMoveNextItem.Image = CType(resources.GetObject("BindingNavigatorMoveNextItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorMoveNextItem.Name = "BindingNavigatorMoveNextItem"
        Me.BindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorMoveNextItem.Size = New System.Drawing.Size(24, 24)
        Me.BindingNavigatorMoveNextItem.Text = "Move next"
        '
        'BindingNavigatorMoveLastItem
        '
        Me.BindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMoveLastItem.Image = CType(resources.GetObject("BindingNavigatorMoveLastItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorMoveLastItem.Name = "BindingNavigatorMoveLastItem"
        Me.BindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorMoveLastItem.Size = New System.Drawing.Size(24, 24)
        Me.BindingNavigatorMoveLastItem.Text = "Move last"
        '
        'BindingNavigatorSeparator2
        '
        Me.BindingNavigatorSeparator2.Name = "BindingNavigatorSeparator2"
        Me.BindingNavigatorSeparator2.Size = New System.Drawing.Size(6, 27)
        '
        'TransactionsBindingNavigatorSaveItem
        '
        Me.TransactionsBindingNavigatorSaveItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.TransactionsBindingNavigatorSaveItem.Image = CType(resources.GetObject("TransactionsBindingNavigatorSaveItem.Image"), System.Drawing.Image)
        Me.TransactionsBindingNavigatorSaveItem.Name = "TransactionsBindingNavigatorSaveItem"
        Me.TransactionsBindingNavigatorSaveItem.Size = New System.Drawing.Size(24, 24)
        Me.TransactionsBindingNavigatorSaveItem.Text = "Save Data"
        '
        'TransactionsDataGridView
        '
        Me.TransactionsDataGridView.AllowUserToAddRows = False
        Me.TransactionsDataGridView.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TransactionsDataGridView.AutoGenerateColumns = False
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.TransactionsDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.TransactionsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.TransactionsDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.btnSelect, Me.btnReprint, Me.DataGridViewTextBoxColumn1, Me.Truck_ID, Me.Customer, Me.Carrier, Me.Commodity, Me.DataGridViewTextBoxColumn3, Me.DataGridViewTextBoxColumn14, Me.DataGridViewTextBoxColumn2})
        Me.TransactionsDataGridView.DataSource = Me.TransactionsBindingSource
        Me.TransactionsDataGridView.Location = New System.Drawing.Point(10, 50)
        Me.TransactionsDataGridView.Name = "TransactionsDataGridView"
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.TransactionsDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TransactionsDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.TransactionsDataGridView.Size = New System.Drawing.Size(1176, 487)
        Me.TransactionsDataGridView.TabIndex = 2
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 27)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Padding = New System.Windows.Forms.Padding(5, 5, 5, 5)
        Me.Panel1.Size = New System.Drawing.Size(1199, 53)
        Me.Panel1.TabIndex = 3
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 27.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(5, 5)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(1189, 43)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Trucks In Yard"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.TrucksDataGridView)
        Me.Panel2.Controls.Add(Me.btnSelectType)
        Me.Panel2.Controls.Add(Me.TransactionsDataGridView)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Panel2.Location = New System.Drawing.Point(0, 80)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Padding = New System.Windows.Forms.Padding(10, 10, 10, 10)
        Me.Panel2.Size = New System.Drawing.Size(1199, 581)
        Me.Panel2.TabIndex = 4
        '
        'TrucksDataGridView
        '
        Me.TrucksDataGridView.AllowUserToAddRows = False
        Me.TrucksDataGridView.AllowUserToDeleteRows = False
        Me.TrucksDataGridView.AllowUserToOrderColumns = True
        Me.TrucksDataGridView.AutoGenerateColumns = False
        Me.TrucksDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders
        Me.TrucksDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.TrucksDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.btnSelectTruck, Me.TruckIDDataGridViewTextBoxColumn, Me.LightWtDataGridViewTextBoxColumn})
        Me.TrucksDataGridView.DataSource = Me.TruckBindingSource
        Me.TrucksDataGridView.Location = New System.Drawing.Point(302, 54)
        Me.TrucksDataGridView.Name = "TrucksDataGridView"
        Me.TrucksDataGridView.ReadOnly = True
        Me.TrucksDataGridView.Size = New System.Drawing.Size(386, 505)
        Me.TrucksDataGridView.TabIndex = 4
        Me.TrucksDataGridView.Visible = False
        '
        'btnSelectTruck
        '
        Me.btnSelectTruck.FillWeight = 120.0!
        Me.btnSelectTruck.Frozen = True
        Me.btnSelectTruck.HeaderText = "Select Truck"
        Me.btnSelectTruck.Name = "btnSelectTruck"
        Me.btnSelectTruck.ReadOnly = True
        Me.btnSelectTruck.Text = "SELECT"
        Me.btnSelectTruck.UseColumnTextForButtonValue = True
        Me.btnSelectTruck.Width = 120
        '
        'btnSelectType
        '
        Me.btnSelectType.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSelectType.Location = New System.Drawing.Point(369, 6)
        Me.btnSelectType.Name = "btnSelectType"
        Me.btnSelectType.Size = New System.Drawing.Size(255, 42)
        Me.btnSelectType.TabIndex = 3
        Me.btnSelectType.Text = "Weigh Out Saved Truck"
        Me.btnSelectType.UseVisualStyleBackColor = True
        '
        'TruckIDDataGridViewTextBoxColumn
        '
        Me.TruckIDDataGridViewTextBoxColumn.DataPropertyName = "Truck_ID"
        Me.TruckIDDataGridViewTextBoxColumn.Frozen = True
        Me.TruckIDDataGridViewTextBoxColumn.HeaderText = "Truck_ID"
        Me.TruckIDDataGridViewTextBoxColumn.Name = "TruckIDDataGridViewTextBoxColumn"
        Me.TruckIDDataGridViewTextBoxColumn.ReadOnly = True
        Me.TruckIDDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        '
        'LightWtDataGridViewTextBoxColumn
        '
        Me.LightWtDataGridViewTextBoxColumn.DataPropertyName = "Light_Wt"
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopRight
        Me.LightWtDataGridViewTextBoxColumn.DefaultCellStyle = DataGridViewCellStyle6
        Me.LightWtDataGridViewTextBoxColumn.HeaderText = "Light_Wt"
        Me.LightWtDataGridViewTextBoxColumn.Name = "LightWtDataGridViewTextBoxColumn"
        Me.LightWtDataGridViewTextBoxColumn.ReadOnly = True
        '
        'TruckBindingSource
        '
        Me.TruckBindingSource.DataMember = "Truck"
        Me.TruckBindingSource.DataSource = Me.ScaleDataSet
        '
        'ScaleDataSet
        '
        Me.ScaleDataSet.DataSetName = "ScaleDataSet"
        Me.ScaleDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'TransactionsBindingSource
        '
        Me.TransactionsBindingSource.DataMember = "Transactions"
        Me.TransactionsBindingSource.DataSource = Me.ScaleDataSet
        '
        'TransactionsTableAdapter
        '
        Me.TransactionsTableAdapter.ClearBeforeFill = True
        '
        'TableAdapterManager
        '
        Me.TableAdapterManager.BackupDataSetBeforeUpdate = False
        Me.TableAdapterManager.CarrierTableAdapter = Nothing
        Me.TableAdapterManager.CommodityTableAdapter = Nothing
        Me.TableAdapterManager.CustomerTableAdapter = Nothing
        Me.TableAdapterManager.DestinationTableAdapter = Nothing
        Me.TableAdapterManager.LocationTableAdapter = Nothing
        Me.TableAdapterManager.SetupTableAdapter = Nothing
        Me.TableAdapterManager.TransactionsTableAdapter = Me.TransactionsTableAdapter
        Me.TableAdapterManager.TruckTableAdapter = Nothing
        Me.TableAdapterManager.UpdateOrder = Basic_Weigh.ScaleDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete
        '
        'TruckTableAdapter
        '
        Me.TruckTableAdapter.ClearBeforeFill = True
        '
        'btnSelect
        '
        Me.btnSelect.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.btnSelect.DataPropertyName = "btnSelect"
        Me.btnSelect.HeaderText = ""
        Me.btnSelect.Name = "btnSelect"
        Me.btnSelect.ReadOnly = True
        Me.btnSelect.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.btnSelect.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        Me.btnSelect.Width = 19
        '
        'btnReprint
        '
        Me.btnReprint.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.btnReprint.DataPropertyName = "btnReprint"
        Me.btnReprint.HeaderText = ""
        Me.btnReprint.Name = "btnReprint"
        Me.btnReprint.ReadOnly = True
        Me.btnReprint.Width = 5
        '
        'DataGridViewTextBoxColumn1
        '
        Me.DataGridViewTextBoxColumn1.DataPropertyName = "Ticket"
        Me.DataGridViewTextBoxColumn1.HeaderText = "Ticket"
        Me.DataGridViewTextBoxColumn1.MinimumWidth = 50
        Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
        Me.DataGridViewTextBoxColumn1.Width = 80
        '
        'Truck_ID
        '
        Me.Truck_ID.DataPropertyName = "Truck_ID"
        Me.Truck_ID.HeaderText = "Truck_ID"
        Me.Truck_ID.Name = "Truck_ID"
        Me.Truck_ID.Width = 139
        '
        'Customer
        '
        Me.Customer.DataPropertyName = "Customer"
        Me.Customer.HeaderText = "Customer"
        Me.Customer.Name = "Customer"
        Me.Customer.Width = 146
        '
        'Carrier
        '
        Me.Carrier.DataPropertyName = "Carrier"
        Me.Carrier.HeaderText = "Hauler"
        Me.Carrier.Name = "Carrier"
        Me.Carrier.Width = 113
        '
        'Commodity
        '
        Me.Commodity.DataPropertyName = "Commodity"
        Me.Commodity.HeaderText = "Commodity"
        Me.Commodity.Name = "Commodity"
        Me.Commodity.Width = 164
        '
        'DataGridViewTextBoxColumn3
        '
        Me.DataGridViewTextBoxColumn3.DataPropertyName = "Date_In"
        DataGridViewCellStyle2.Format = "g"
        DataGridViewCellStyle2.NullValue = Nothing
        Me.DataGridViewTextBoxColumn3.DefaultCellStyle = DataGridViewCellStyle2
        Me.DataGridViewTextBoxColumn3.HeaderText = "Date_In"
        Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
        Me.DataGridViewTextBoxColumn3.Width = 124
        '
        'DataGridViewTextBoxColumn14
        '
        Me.DataGridViewTextBoxColumn14.DataPropertyName = "Location"
        Me.DataGridViewTextBoxColumn14.HeaderText = "Location"
        Me.DataGridViewTextBoxColumn14.MinimumWidth = 100
        Me.DataGridViewTextBoxColumn14.Name = "DataGridViewTextBoxColumn14"
        Me.DataGridViewTextBoxColumn14.Width = 133
        '
        'DataGridViewTextBoxColumn2
        '
        Me.DataGridViewTextBoxColumn2.DataPropertyName = "In_Weight"
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopRight
        DataGridViewCellStyle3.Format = "N0"
        DataGridViewCellStyle3.NullValue = Nothing
        Me.DataGridViewTextBoxColumn2.DefaultCellStyle = DataGridViewCellStyle3
        Me.DataGridViewTextBoxColumn2.FillWeight = 50.0!
        Me.DataGridViewTextBoxColumn2.HeaderText = "In Weight"
        Me.DataGridViewTextBoxColumn2.MinimumWidth = 50
        Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
        '
        'InboundTrucks
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.BackColor = System.Drawing.Color.LightSteelBlue
        Me.ClientSize = New System.Drawing.Size(1199, 661)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.TransactionsBindingNavigator)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimizeBox = False
        Me.Name = "InboundTrucks"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "InboundTrucks"
        CType(Me.TransactionsBindingNavigator, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TransactionsBindingNavigator.ResumeLayout(False)
        Me.TransactionsBindingNavigator.PerformLayout()
        CType(Me.TransactionsDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        CType(Me.TrucksDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TruckBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ScaleDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TransactionsBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ScaleDataSet As Basic_Weigh.ScaleDataSet
    Friend WithEvents TransactionsBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents TransactionsTableAdapter As Basic_Weigh.ScaleDataSetTableAdapters.TransactionsTableAdapter
    Friend WithEvents TableAdapterManager As Basic_Weigh.ScaleDataSetTableAdapters.TableAdapterManager
    Friend WithEvents TransactionsBindingNavigator As System.Windows.Forms.BindingNavigator
    Friend WithEvents BindingNavigatorCountItem As System.Windows.Forms.ToolStripLabel
    Friend WithEvents BindingNavigatorDeleteItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMoveFirstItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMovePreviousItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorSeparator As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BindingNavigatorPositionItem As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents BindingNavigatorSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BindingNavigatorMoveNextItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMoveLastItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents TransactionsBindingNavigatorSaveItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents TransactionsDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents btnSelectType As Button
    Friend WithEvents DataGridViewTextBoxColumn8 As DataGridViewTextBoxColumn
    Friend WithEvents TrucksDataGridView As DataGridView
    Friend WithEvents TruckBindingSource As BindingSource
    Friend WithEvents TruckTableAdapter As ScaleDataSetTableAdapters.TruckTableAdapter
    Friend WithEvents DataGridViewTextBoxColumn13 As DataGridViewTextBoxColumn
    Friend WithEvents btnSelectTruck As DataGridViewButtonColumn
    Friend WithEvents TruckIDDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents LightWtDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents btnSelect As DataGridViewButtonColumn
    Friend WithEvents btnReprint As DataGridViewButtonColumn
    Friend WithEvents DataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents Truck_ID As DataGridViewTextBoxColumn
    Friend WithEvents Customer As DataGridViewTextBoxColumn
    Friend WithEvents Carrier As DataGridViewTextBoxColumn
    Friend WithEvents Commodity As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn3 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn14 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn2 As DataGridViewTextBoxColumn
End Class
