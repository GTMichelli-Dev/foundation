<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Weigh_Screen
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
        Dim TicketLabel As System.Windows.Forms.Label
        Dim Date_InLabel As System.Windows.Forms.Label
        Dim Date_OutLabel As System.Windows.Forms.Label
        Dim In_WeightLabel As System.Windows.Forms.Label
        Dim CarrierLabel As System.Windows.Forms.Label
        Dim TruckLabel As System.Windows.Forms.Label
        Dim LotLabel As System.Windows.Forms.Label
        Dim LocationLabel As System.Windows.Forms.Label
        Dim DestinationLabel As System.Windows.Forms.Label
        Dim CustomerLabel As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Weigh_Screen))
        Me.Out_WeightLabel = New System.Windows.Forms.Label()
        Me.TicketLabel1 = New System.Windows.Forms.Label()
        Me.TransactionsBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ScaleDataSet = New Basic_Weigh.ScaleDataSet()
        Me.Date_InLabel1 = New System.Windows.Forms.Label()
        Me.Date_OutLabel1 = New System.Windows.Forms.Label()
        Me.CustomerBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.CarrierComboBox = New System.Windows.Forms.ComboBox()
        Me.CarrierBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.TruckComboBox = New System.Windows.Forms.ComboBox()
        Me.TruckBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.CommentsTextBox = New System.Windows.Forms.TextBox()
        Me.LocationComboBox = New System.Windows.Forms.ComboBox()
        Me.LocationBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ProductComboBox = New System.Windows.Forms.ComboBox()
        Me.CommodityBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ScaleDataSet2 = New Basic_Weigh.ScaleDataSet()
        Me.DestinationBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.pnlGTN = New System.Windows.Forms.GroupBox()
        Me.lblNet = New System.Windows.Forms.Label()
        Me.lblTare = New System.Windows.Forms.Label()
        Me.lblGross = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.TransactionsTableAdapter = New Basic_Weigh.ScaleDataSetTableAdapters.TransactionsTableAdapter()
        Me.TableAdapterManager = New Basic_Weigh.ScaleDataSetTableAdapters.TableAdapterManager()
        Me.lblMotionIn = New System.Windows.Forms.Label()
        Me.pnlSave = New System.Windows.Forms.Panel()
        Me.CustomerTableAdapter = New Basic_Weigh.ScaleDataSetTableAdapters.CustomerTableAdapter()
        Me.CarrierTableAdapter = New Basic_Weigh.ScaleDataSetTableAdapters.CarrierTableAdapter()
        Me.LocationTableAdapter = New Basic_Weigh.ScaleDataSetTableAdapters.LocationTableAdapter()
        Me.DestinationTableAdapter = New Basic_Weigh.ScaleDataSetTableAdapters.DestinationTableAdapter()
        Me.tmrUpdate = New System.Windows.Forms.Timer(Me.components)
        Me.CustomerComboBox = New System.Windows.Forms.ComboBox()
        Me.ckUseScale = New System.Windows.Forms.CheckBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.In_Weight = New System.Windows.Forms.NumericUpDown()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Out_Weight = New System.Windows.Forms.NumericUpDown()
        Me.pnlOutWt = New System.Windows.Forms.Panel()
        Me.lblSetDateOut = New System.Windows.Forms.Label()
        Me.Out_TimePicker = New System.Windows.Forms.DateTimePicker()
        Me.Out_DatePicker = New System.Windows.Forms.DateTimePicker()
        Me.lblMotionOut = New System.Windows.Forms.Label()
        Me.lblStored = New System.Windows.Forms.Label()
        Me.lblStoredWt = New System.Windows.Forms.Label()
        Me.lblWeight = New System.Windows.Forms.Label()
        Me.lblTotalWeight = New System.Windows.Forms.Label()
        Me.btnSplit = New System.Windows.Forms.Button()
        Me.lblPup = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.btnPup = New System.Windows.Forms.Button()
        Me.TruckTableAdapter = New Basic_Weigh.ScaleDataSetTableAdapters.TruckTableAdapter()
        Me.In_DatePicker = New System.Windows.Forms.DateTimePicker()
        Me.In_TimePicker = New System.Windows.Forms.DateTimePicker()
        Me.lblSetDateIn = New System.Windows.Forms.Label()
        Me.ScaleDataSet1 = New Basic_Weigh.ScaleDataSet()
        Me.ScaleDataSet1BindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ProductFilterBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.CommodityTableAdapter = New Basic_Weigh.ScaleDataSetTableAdapters.CommodityTableAdapter()
        TicketLabel = New System.Windows.Forms.Label()
        Date_InLabel = New System.Windows.Forms.Label()
        Date_OutLabel = New System.Windows.Forms.Label()
        In_WeightLabel = New System.Windows.Forms.Label()
        CarrierLabel = New System.Windows.Forms.Label()
        TruckLabel = New System.Windows.Forms.Label()
        LotLabel = New System.Windows.Forms.Label()
        LocationLabel = New System.Windows.Forms.Label()
        DestinationLabel = New System.Windows.Forms.Label()
        CustomerLabel = New System.Windows.Forms.Label()
        CType(Me.TransactionsBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ScaleDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CustomerBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CarrierBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TruckBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LocationBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CommodityBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ScaleDataSet2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DestinationBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlGTN.SuspendLayout()
        Me.pnlSave.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.In_Weight, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        CType(Me.Out_Weight, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlOutWt.SuspendLayout()
        CType(Me.ScaleDataSet1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ScaleDataSet1BindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ProductFilterBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TicketLabel
        '
        TicketLabel.AutoSize = True
        TicketLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        TicketLabel.Location = New System.Drawing.Point(10, 46)
        TicketLabel.Name = "TicketLabel"
        TicketLabel.Size = New System.Drawing.Size(72, 24)
        TicketLabel.TabIndex = 2
        TicketLabel.Text = "Ticket:"
        '
        'Date_InLabel
        '
        Date_InLabel.AutoSize = True
        Date_InLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Date_InLabel.Location = New System.Drawing.Point(10, 182)
        Date_InLabel.Name = "Date_InLabel"
        Date_InLabel.Size = New System.Drawing.Size(81, 24)
        Date_InLabel.TabIndex = 6
        Date_InLabel.Text = "Date In:"
        '
        'Date_OutLabel
        '
        Date_OutLabel.AutoSize = True
        Date_OutLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Date_OutLabel.Location = New System.Drawing.Point(26, 70)
        Date_OutLabel.Name = "Date_OutLabel"
        Date_OutLabel.Size = New System.Drawing.Size(97, 24)
        Date_OutLabel.TabIndex = 8
        Date_OutLabel.Text = "Date Out:"
        '
        'In_WeightLabel
        '
        In_WeightLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        In_WeightLabel.Location = New System.Drawing.Point(10, 243)
        In_WeightLabel.Name = "In_WeightLabel"
        In_WeightLabel.Size = New System.Drawing.Size(136, 27)
        In_WeightLabel.TabIndex = 10
        In_WeightLabel.Text = "In Weight:"
        '
        'CarrierLabel
        '
        CarrierLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        CarrierLabel.Location = New System.Drawing.Point(10, 359)
        CarrierLabel.Name = "CarrierLabel"
        CarrierLabel.Size = New System.Drawing.Size(136, 23)
        CarrierLabel.TabIndex = 16
        CarrierLabel.Text = "Hauler:"
        '
        'TruckLabel
        '
        TruckLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        TruckLabel.Location = New System.Drawing.Point(10, 396)
        TruckLabel.Name = "TruckLabel"
        TruckLabel.Size = New System.Drawing.Size(136, 23)
        TruckLabel.TabIndex = 18
        TruckLabel.Text = "Truck ID:"
        '
        'LotLabel
        '
        LotLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        LotLabel.Location = New System.Drawing.Point(10, 510)
        LotLabel.Name = "LotLabel"
        LotLabel.Size = New System.Drawing.Size(135, 23)
        LotLabel.TabIndex = 28
        LotLabel.Text = "Comments:"
        '
        'LocationLabel
        '
        LocationLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        LocationLabel.Location = New System.Drawing.Point(10, 473)
        LocationLabel.Name = "LocationLabel"
        LocationLabel.Size = New System.Drawing.Size(135, 23)
        LocationLabel.TabIndex = 30
        LocationLabel.Text = "Location:"
        '
        'DestinationLabel
        '
        DestinationLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DestinationLabel.Location = New System.Drawing.Point(10, 434)
        DestinationLabel.Name = "DestinationLabel"
        DestinationLabel.Size = New System.Drawing.Size(136, 23)
        DestinationLabel.TabIndex = 32
        DestinationLabel.Text = "Product:"
        '
        'CustomerLabel
        '
        CustomerLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        CustomerLabel.Location = New System.Drawing.Point(9, 321)
        CustomerLabel.Name = "CustomerLabel"
        CustomerLabel.Size = New System.Drawing.Size(136, 23)
        CustomerLabel.TabIndex = 14
        CustomerLabel.Text = "Customer:"
        '
        'Out_WeightLabel
        '
        Me.Out_WeightLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Out_WeightLabel.Location = New System.Drawing.Point(7, 133)
        Me.Out_WeightLabel.Name = "Out_WeightLabel"
        Me.Out_WeightLabel.Size = New System.Drawing.Size(182, 26)
        Me.Out_WeightLabel.TabIndex = 12
        Me.Out_WeightLabel.Text = "Out Weight:"
        '
        'TicketLabel1
        '
        Me.TicketLabel1.BackColor = System.Drawing.Color.White
        Me.TicketLabel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TicketLabel1.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.TransactionsBindingSource, "Ticket", True))
        Me.TicketLabel1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TicketLabel1.Location = New System.Drawing.Point(97, 47)
        Me.TicketLabel1.Name = "TicketLabel1"
        Me.TicketLabel1.Size = New System.Drawing.Size(100, 23)
        Me.TicketLabel1.TabIndex = 3
        Me.TicketLabel1.Text = "Label1"
        Me.TicketLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TransactionsBindingSource
        '
        Me.TransactionsBindingSource.DataMember = "Transactions"
        Me.TransactionsBindingSource.DataSource = Me.ScaleDataSet
        '
        'ScaleDataSet
        '
        Me.ScaleDataSet.DataSetName = "ScaleDataSet"
        Me.ScaleDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'Date_InLabel1
        '
        Me.Date_InLabel1.BackColor = System.Drawing.Color.White
        Me.Date_InLabel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Date_InLabel1.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.TransactionsBindingSource, "Date_In", True))
        Me.Date_InLabel1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Date_InLabel1.Location = New System.Drawing.Point(97, 182)
        Me.Date_InLabel1.Name = "Date_InLabel1"
        Me.Date_InLabel1.Size = New System.Drawing.Size(190, 23)
        Me.Date_InLabel1.TabIndex = 12
        Me.Date_InLabel1.Text = "Label1"
        Me.Date_InLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Date_OutLabel1
        '
        Me.Date_OutLabel1.BackColor = System.Drawing.Color.White
        Me.Date_OutLabel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Date_OutLabel1.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.TransactionsBindingSource, "Date_Out", True))
        Me.Date_OutLabel1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Date_OutLabel1.Location = New System.Drawing.Point(129, 72)
        Me.Date_OutLabel1.Name = "Date_OutLabel1"
        Me.Date_OutLabel1.Size = New System.Drawing.Size(190, 23)
        Me.Date_OutLabel1.TabIndex = 17
        Me.Date_OutLabel1.Text = "Label1"
        Me.Date_OutLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CustomerBindingSource
        '
        Me.CustomerBindingSource.DataMember = "Customer"
        Me.CustomerBindingSource.DataSource = Me.ScaleDataSet
        '
        'CarrierComboBox
        '
        Me.CarrierComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.CarrierComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.CarrierComboBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.TransactionsBindingSource, "Carrier", True))
        Me.CarrierComboBox.DataSource = Me.CarrierBindingSource
        Me.CarrierComboBox.DisplayMember = "Hauler_Name"
        Me.CarrierComboBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CarrierComboBox.FormattingEnabled = True
        Me.CarrierComboBox.Location = New System.Drawing.Point(158, 356)
        Me.CarrierComboBox.Name = "CarrierComboBox"
        Me.CarrierComboBox.Size = New System.Drawing.Size(540, 32)
        Me.CarrierComboBox.TabIndex = 2
        Me.CarrierComboBox.ValueMember = "Hauler_Name"
        '
        'CarrierBindingSource
        '
        Me.CarrierBindingSource.DataMember = "Carrier"
        Me.CarrierBindingSource.DataSource = Me.ScaleDataSet
        '
        'TruckComboBox
        '
        Me.TruckComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.TruckComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.TruckComboBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.TransactionsBindingSource, "Truck_Id", True))
        Me.TruckComboBox.DataSource = Me.TruckBindingSource
        Me.TruckComboBox.DisplayMember = "Truck_ID"
        Me.TruckComboBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TruckComboBox.FormattingEnabled = True
        Me.TruckComboBox.Location = New System.Drawing.Point(158, 394)
        Me.TruckComboBox.MaxLength = 15
        Me.TruckComboBox.Name = "TruckComboBox"
        Me.TruckComboBox.Size = New System.Drawing.Size(540, 32)
        Me.TruckComboBox.TabIndex = 3
        Me.TruckComboBox.ValueMember = "Truck_ID"
        '
        'TruckBindingSource
        '
        Me.TruckBindingSource.DataMember = "Truck"
        Me.TruckBindingSource.DataSource = Me.ScaleDataSet
        '
        'CommentsTextBox
        '
        Me.CommentsTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.TransactionsBindingSource, "Comment", True))
        Me.CommentsTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CommentsTextBox.Location = New System.Drawing.Point(158, 507)
        Me.CommentsTextBox.Name = "CommentsTextBox"
        Me.CommentsTextBox.Size = New System.Drawing.Size(540, 29)
        Me.CommentsTextBox.TabIndex = 6
        '
        'LocationComboBox
        '
        Me.LocationComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.LocationComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.LocationComboBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.TransactionsBindingSource, "Location", True))
        Me.LocationComboBox.DataSource = Me.LocationBindingSource
        Me.LocationComboBox.DisplayMember = "Location_Name"
        Me.LocationComboBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LocationComboBox.FormattingEnabled = True
        Me.LocationComboBox.Location = New System.Drawing.Point(158, 470)
        Me.LocationComboBox.Name = "LocationComboBox"
        Me.LocationComboBox.Size = New System.Drawing.Size(540, 32)
        Me.LocationComboBox.TabIndex = 5
        Me.LocationComboBox.ValueMember = "Location_Name"
        '
        'LocationBindingSource
        '
        Me.LocationBindingSource.DataMember = "Location"
        Me.LocationBindingSource.DataSource = Me.ScaleDataSet
        '
        'ProductComboBox
        '
        Me.ProductComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.ProductComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.ProductComboBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.TransactionsBindingSource, "Commodity", True))
        Me.ProductComboBox.DataSource = Me.CommodityBindingSource
        Me.ProductComboBox.DisplayMember = "Product_Name"
        Me.ProductComboBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ProductComboBox.FormattingEnabled = True
        Me.ProductComboBox.Location = New System.Drawing.Point(158, 432)
        Me.ProductComboBox.Name = "ProductComboBox"
        Me.ProductComboBox.Size = New System.Drawing.Size(540, 32)
        Me.ProductComboBox.TabIndex = 4
        Me.ProductComboBox.ValueMember = "Product_Name"
        '
        'CommodityBindingSource
        '
        Me.CommodityBindingSource.DataMember = "Commodity"
        Me.CommodityBindingSource.DataSource = Me.ScaleDataSet2
        '
        'ScaleDataSet2
        '
        Me.ScaleDataSet2.DataSetName = "ScaleDataSet"
        Me.ScaleDataSet2.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'DestinationBindingSource
        '
        Me.DestinationBindingSource.DataMember = "Destination"
        Me.DestinationBindingSource.DataSource = Me.ScaleDataSet
        '
        'pnlGTN
        '
        Me.pnlGTN.Controls.Add(Me.lblNet)
        Me.pnlGTN.Controls.Add(Me.lblTare)
        Me.pnlGTN.Controls.Add(Me.lblGross)
        Me.pnlGTN.Controls.Add(Me.Label3)
        Me.pnlGTN.Controls.Add(Me.Label2)
        Me.pnlGTN.Controls.Add(Me.Label1)
        Me.pnlGTN.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.pnlGTN.Location = New System.Drawing.Point(658, 154)
        Me.pnlGTN.Name = "pnlGTN"
        Me.pnlGTN.Size = New System.Drawing.Size(289, 137)
        Me.pnlGTN.TabIndex = 36
        Me.pnlGTN.TabStop = False
        Me.pnlGTN.Text = "Gross Tare Net"
        '
        'lblNet
        '
        Me.lblNet.BackColor = System.Drawing.Color.White
        Me.lblNet.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblNet.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold)
        Me.lblNet.Location = New System.Drawing.Point(117, 81)
        Me.lblNet.Name = "lblNet"
        Me.lblNet.Size = New System.Drawing.Size(139, 23)
        Me.lblNet.TabIndex = 21
        Me.lblNet.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblTare
        '
        Me.lblTare.BackColor = System.Drawing.Color.White
        Me.lblTare.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblTare.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold)
        Me.lblTare.Location = New System.Drawing.Point(117, 55)
        Me.lblTare.Name = "lblTare"
        Me.lblTare.Size = New System.Drawing.Size(139, 23)
        Me.lblTare.TabIndex = 20
        Me.lblTare.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblGross
        '
        Me.lblGross.BackColor = System.Drawing.Color.White
        Me.lblGross.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblGross.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold)
        Me.lblGross.Location = New System.Drawing.Point(117, 29)
        Me.lblGross.Name = "lblGross"
        Me.lblGross.Size = New System.Drawing.Size(139, 23)
        Me.lblGross.TabIndex = 19
        Me.lblGross.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold)
        Me.Label3.Location = New System.Drawing.Point(30, 81)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(62, 23)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Net"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold)
        Me.Label2.Location = New System.Drawing.Point(30, 55)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(62, 23)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Tare"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(30, 29)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(76, 23)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Gross"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Cancel_Button
        '
        Me.Cancel_Button.BackColor = System.Drawing.Color.Red
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Cancel_Button.Location = New System.Drawing.Point(3, 7)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(112, 68)
        Me.Cancel_Button.TabIndex = 7
        Me.Cancel_Button.Text = "Cancel"
        Me.Cancel_Button.UseVisualStyleBackColor = False
        '
        'OK_Button
        '
        Me.OK_Button.BackColor = System.Drawing.Color.Yellow
        Me.OK_Button.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OK_Button.Location = New System.Drawing.Point(3, 81)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(112, 68)
        Me.OK_Button.TabIndex = 8
        Me.OK_Button.Text = "Save"
        Me.OK_Button.UseVisualStyleBackColor = False
        '
        'Button1
        '
        Me.Button1.BackColor = System.Drawing.Color.Lime
        Me.Button1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.Location = New System.Drawing.Point(3, 155)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(112, 68)
        Me.Button1.TabIndex = 9
        Me.Button1.Text = "Save and Print"
        Me.Button1.UseVisualStyleBackColor = False
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
        'lblMotionIn
        '
        Me.lblMotionIn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMotionIn.ForeColor = System.Drawing.Color.Red
        Me.lblMotionIn.Location = New System.Drawing.Point(150, 275)
        Me.lblMotionIn.Name = "lblMotionIn"
        Me.lblMotionIn.Size = New System.Drawing.Size(146, 23)
        Me.lblMotionIn.TabIndex = 39
        Me.lblMotionIn.Text = "Motion On Scale"
        Me.lblMotionIn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.lblMotionIn.Visible = False
        '
        'pnlSave
        '
        Me.pnlSave.Controls.Add(Me.Button1)
        Me.pnlSave.Controls.Add(Me.OK_Button)
        Me.pnlSave.Controls.Add(Me.Cancel_Button)
        Me.pnlSave.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.pnlSave.Location = New System.Drawing.Point(775, 309)
        Me.pnlSave.Name = "pnlSave"
        Me.pnlSave.Size = New System.Drawing.Size(126, 231)
        Me.pnlSave.TabIndex = 11
        '
        'CustomerTableAdapter
        '
        Me.CustomerTableAdapter.ClearBeforeFill = True
        '
        'CarrierTableAdapter
        '
        Me.CarrierTableAdapter.ClearBeforeFill = True
        '
        'LocationTableAdapter
        '
        Me.LocationTableAdapter.ClearBeforeFill = True
        '
        'DestinationTableAdapter
        '
        Me.DestinationTableAdapter.ClearBeforeFill = True
        '
        'tmrUpdate
        '
        Me.tmrUpdate.Enabled = True
        Me.tmrUpdate.Interval = 200
        '
        'CustomerComboBox
        '
        Me.CustomerComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.CustomerComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.CustomerComboBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.TransactionsBindingSource, "Customer", True))
        Me.CustomerComboBox.DataSource = Me.CustomerBindingSource
        Me.CustomerComboBox.DisplayMember = "Customer_Name"
        Me.CustomerComboBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CustomerComboBox.FormattingEnabled = True
        Me.CustomerComboBox.Location = New System.Drawing.Point(158, 318)
        Me.CustomerComboBox.Name = "CustomerComboBox"
        Me.CustomerComboBox.Size = New System.Drawing.Size(540, 32)
        Me.CustomerComboBox.TabIndex = 1
        Me.CustomerComboBox.ValueMember = "Customer_Name"
        '
        'ckUseScale
        '
        Me.ckUseScale.Checked = True
        Me.ckUseScale.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ckUseScale.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ckUseScale.Location = New System.Drawing.Point(162, 219)
        Me.ckUseScale.Name = "ckUseScale"
        Me.ckUseScale.Size = New System.Drawing.Size(134, 24)
        Me.ckUseScale.TabIndex = 13
        Me.ckUseScale.Text = "Use Scale"
        Me.ckUseScale.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.In_Weight)
        Me.Panel1.Location = New System.Drawing.Point(158, 244)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(122, 26)
        Me.Panel1.TabIndex = 41
        '
        'In_Weight
        '
        Me.In_Weight.BackColor = System.Drawing.Color.White
        Me.In_Weight.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.TransactionsBindingSource, "In_Weight", True))
        Me.In_Weight.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.In_Weight.Location = New System.Drawing.Point(4, -1)
        Me.In_Weight.Maximum = New Decimal(New Integer() {200000, 0, 0, 0})
        Me.In_Weight.Minimum = New Decimal(New Integer() {10000, 0, 0, -2147483648})
        Me.In_Weight.Name = "In_Weight"
        Me.In_Weight.ReadOnly = True
        Me.In_Weight.Size = New System.Drawing.Size(135, 29)
        Me.In_Weight.TabIndex = 14
        Me.In_Weight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.Out_Weight)
        Me.Panel2.Location = New System.Drawing.Point(183, 130)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(122, 31)
        Me.Panel2.TabIndex = 42
        '
        'Out_Weight
        '
        Me.Out_Weight.BackColor = System.Drawing.Color.White
        Me.Out_Weight.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.TransactionsBindingSource, "Out_Weight", True))
        Me.Out_Weight.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Out_Weight.Location = New System.Drawing.Point(1, 1)
        Me.Out_Weight.Maximum = New Decimal(New Integer() {200000, 0, 0, 0})
        Me.Out_Weight.Minimum = New Decimal(New Integer() {10000, 0, 0, -2147483648})
        Me.Out_Weight.Name = "Out_Weight"
        Me.Out_Weight.ReadOnly = True
        Me.Out_Weight.Size = New System.Drawing.Size(137, 29)
        Me.Out_Weight.TabIndex = 18
        Me.Out_Weight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'pnlOutWt
        '
        Me.pnlOutWt.Controls.Add(Me.lblSetDateOut)
        Me.pnlOutWt.Controls.Add(Me.Out_TimePicker)
        Me.pnlOutWt.Controls.Add(Me.Out_DatePicker)
        Me.pnlOutWt.Controls.Add(Me.lblMotionOut)
        Me.pnlOutWt.Controls.Add(Me.Panel2)
        Me.pnlOutWt.Controls.Add(Me.Out_WeightLabel)
        Me.pnlOutWt.Controls.Add(Me.Date_OutLabel1)
        Me.pnlOutWt.Controls.Add(Date_OutLabel)
        Me.pnlOutWt.Location = New System.Drawing.Point(305, 111)
        Me.pnlOutWt.Name = "pnlOutWt"
        Me.pnlOutWt.Size = New System.Drawing.Size(342, 201)
        Me.pnlOutWt.TabIndex = 43
        Me.pnlOutWt.Visible = False
        '
        'lblSetDateOut
        '
        Me.lblSetDateOut.AutoSize = True
        Me.lblSetDateOut.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSetDateOut.Location = New System.Drawing.Point(135, 9)
        Me.lblSetDateOut.Name = "lblSetDateOut"
        Me.lblSetDateOut.Size = New System.Drawing.Size(169, 16)
        Me.lblSetDateOut.TabIndex = 61
        Me.lblSetDateOut.Text = "Change Date/Time Out:"
        '
        'Out_TimePicker
        '
        Me.Out_TimePicker.Location = New System.Drawing.Point(239, 38)
        Me.Out_TimePicker.Name = "Out_TimePicker"
        Me.Out_TimePicker.Size = New System.Drawing.Size(93, 20)
        Me.Out_TimePicker.TabIndex = 16
        Me.Out_TimePicker.Visible = False
        '
        'Out_DatePicker
        '
        Me.Out_DatePicker.Location = New System.Drawing.Point(129, 38)
        Me.Out_DatePicker.Name = "Out_DatePicker"
        Me.Out_DatePicker.Size = New System.Drawing.Size(104, 20)
        Me.Out_DatePicker.TabIndex = 15
        Me.Out_DatePicker.Visible = False
        '
        'lblMotionOut
        '
        Me.lblMotionOut.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMotionOut.ForeColor = System.Drawing.Color.Red
        Me.lblMotionOut.Location = New System.Drawing.Point(144, 164)
        Me.lblMotionOut.Name = "lblMotionOut"
        Me.lblMotionOut.Size = New System.Drawing.Size(167, 23)
        Me.lblMotionOut.TabIndex = 44
        Me.lblMotionOut.Text = "Motion On Scale"
        Me.lblMotionOut.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblMotionOut.Visible = False
        '
        'lblStored
        '
        Me.lblStored.AutoSize = True
        Me.lblStored.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStored.Location = New System.Drawing.Point(440, 78)
        Me.lblStored.Name = "lblStored"
        Me.lblStored.Size = New System.Drawing.Size(54, 16)
        Me.lblStored.TabIndex = 47
        Me.lblStored.Text = "Stored"
        Me.lblStored.Visible = False
        '
        'lblStoredWt
        '
        Me.lblStoredWt.BackColor = System.Drawing.Color.White
        Me.lblStoredWt.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStoredWt.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold)
        Me.lblStoredWt.Location = New System.Drawing.Point(420, 46)
        Me.lblStoredWt.Name = "lblStoredWt"
        Me.lblStoredWt.Size = New System.Drawing.Size(113, 23)
        Me.lblStoredWt.TabIndex = 48
        Me.lblStoredWt.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblStoredWt.Visible = False
        '
        'lblWeight
        '
        Me.lblWeight.BackColor = System.Drawing.Color.White
        Me.lblWeight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblWeight.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold)
        Me.lblWeight.Location = New System.Drawing.Point(539, 46)
        Me.lblWeight.Name = "lblWeight"
        Me.lblWeight.Size = New System.Drawing.Size(113, 23)
        Me.lblWeight.TabIndex = 49
        Me.lblWeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblWeight.Visible = False
        '
        'lblTotalWeight
        '
        Me.lblTotalWeight.BackColor = System.Drawing.Color.White
        Me.lblTotalWeight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblTotalWeight.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold)
        Me.lblTotalWeight.Location = New System.Drawing.Point(777, 46)
        Me.lblTotalWeight.Name = "lblTotalWeight"
        Me.lblTotalWeight.Size = New System.Drawing.Size(113, 23)
        Me.lblTotalWeight.TabIndex = 50
        Me.lblTotalWeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblTotalWeight.Visible = False
        '
        'btnSplit
        '
        Me.btnSplit.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.btnSplit.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSplit.Location = New System.Drawing.Point(294, 43)
        Me.btnSplit.Name = "btnSplit"
        Me.btnSplit.Size = New System.Drawing.Size(120, 31)
        Me.btnSplit.TabIndex = 51
        Me.btnSplit.Text = "Split Weigh"
        Me.btnSplit.UseVisualStyleBackColor = False
        '
        'lblPup
        '
        Me.lblPup.BackColor = System.Drawing.Color.White
        Me.lblPup.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblPup.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold)
        Me.lblPup.Location = New System.Drawing.Point(658, 46)
        Me.lblPup.Name = "lblPup"
        Me.lblPup.Size = New System.Drawing.Size(113, 23)
        Me.lblPup.TabIndex = 52
        Me.lblPup.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblPup.Visible = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(440, 23)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(35, 13)
        Me.Label5.TabIndex = 53
        Me.Label5.Text = "Truck"
        Me.Label5.Visible = False
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(554, 23)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(36, 13)
        Me.Label6.TabIndex = 54
        Me.Label6.Text = "Trailer"
        Me.Label6.Visible = False
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(672, 23)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(26, 13)
        Me.Label7.TabIndex = 55
        Me.Label7.Text = "Pup"
        Me.Label7.Visible = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(790, 23)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(34, 13)
        Me.Label8.TabIndex = 56
        Me.Label8.Text = "Total:"
        Me.Label8.Visible = False
        '
        'btnPup
        '
        Me.btnPup.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.btnPup.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPup.Location = New System.Drawing.Point(539, 75)
        Me.btnPup.Name = "btnPup"
        Me.btnPup.Size = New System.Drawing.Size(113, 30)
        Me.btnPup.TabIndex = 57
        Me.btnPup.Text = "Add Pup"
        Me.btnPup.UseVisualStyleBackColor = False
        Me.btnPup.Visible = False
        '
        'TruckTableAdapter
        '
        Me.TruckTableAdapter.ClearBeforeFill = True
        '
        'In_DatePicker
        '
        Me.In_DatePicker.Location = New System.Drawing.Point(97, 149)
        Me.In_DatePicker.Name = "In_DatePicker"
        Me.In_DatePicker.Size = New System.Drawing.Size(100, 20)
        Me.In_DatePicker.TabIndex = 10
        Me.In_DatePicker.Visible = False
        '
        'In_TimePicker
        '
        Me.In_TimePicker.Location = New System.Drawing.Point(203, 149)
        Me.In_TimePicker.Name = "In_TimePicker"
        Me.In_TimePicker.Size = New System.Drawing.Size(94, 20)
        Me.In_TimePicker.TabIndex = 11
        Me.In_TimePicker.Visible = False
        '
        'lblSetDateIn
        '
        Me.lblSetDateIn.AutoSize = True
        Me.lblSetDateIn.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSetDateIn.Location = New System.Drawing.Point(122, 120)
        Me.lblSetDateIn.Name = "lblSetDateIn"
        Me.lblSetDateIn.Size = New System.Drawing.Size(158, 16)
        Me.lblSetDateIn.TabIndex = 60
        Me.lblSetDateIn.Text = "Change Date/Time In:"
        '
        'ScaleDataSet1
        '
        Me.ScaleDataSet1.DataSetName = "ScaleDataSet"
        Me.ScaleDataSet1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'ScaleDataSet1BindingSource
        '
        Me.ScaleDataSet1BindingSource.DataSource = Me.ScaleDataSet1
        Me.ScaleDataSet1BindingSource.Position = 0
        '
        'ProductFilterBindingSource
        '
        Me.ProductFilterBindingSource.DataMember = "Product_Filter"
        Me.ProductFilterBindingSource.DataSource = Me.ScaleDataSet1
        '
        'CommodityTableAdapter
        '
        Me.CommodityTableAdapter.ClearBeforeFill = True
        '
        'Weigh_Screen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.LightSteelBlue
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(984, 588)
        Me.Controls.Add(Me.lblSetDateIn)
        Me.Controls.Add(Me.In_TimePicker)
        Me.Controls.Add(Me.In_DatePicker)
        Me.Controls.Add(Me.btnPup)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.lblPup)
        Me.Controls.Add(Me.btnSplit)
        Me.Controls.Add(Me.lblTotalWeight)
        Me.Controls.Add(Me.lblWeight)
        Me.Controls.Add(Me.lblStoredWt)
        Me.Controls.Add(Me.lblStored)
        Me.Controls.Add(Me.pnlOutWt)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.ckUseScale)
        Me.Controls.Add(Me.pnlSave)
        Me.Controls.Add(Me.lblMotionIn)
        Me.Controls.Add(Me.pnlGTN)
        Me.Controls.Add(DestinationLabel)
        Me.Controls.Add(Me.ProductComboBox)
        Me.Controls.Add(LocationLabel)
        Me.Controls.Add(Me.LocationComboBox)
        Me.Controls.Add(LotLabel)
        Me.Controls.Add(Me.CommentsTextBox)
        Me.Controls.Add(TruckLabel)
        Me.Controls.Add(Me.TruckComboBox)
        Me.Controls.Add(CarrierLabel)
        Me.Controls.Add(Me.CarrierComboBox)
        Me.Controls.Add(CustomerLabel)
        Me.Controls.Add(Me.CustomerComboBox)
        Me.Controls.Add(In_WeightLabel)
        Me.Controls.Add(Date_InLabel)
        Me.Controls.Add(Me.Date_InLabel1)
        Me.Controls.Add(TicketLabel)
        Me.Controls.Add(Me.TicketLabel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Weigh_Screen"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Weigh Truck"
        CType(Me.TransactionsBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ScaleDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CustomerBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CarrierBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TruckBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LocationBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CommodityBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ScaleDataSet2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DestinationBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlGTN.ResumeLayout(False)
        Me.pnlSave.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        CType(Me.In_Weight, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        CType(Me.Out_Weight, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlOutWt.ResumeLayout(False)
        Me.pnlOutWt.PerformLayout()
        CType(Me.ScaleDataSet1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ScaleDataSet1BindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ProductFilterBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ScaleDataSet As Basic_Weigh.ScaleDataSet
    Friend WithEvents TransactionsBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents TransactionsTableAdapter As Basic_Weigh.ScaleDataSetTableAdapters.TransactionsTableAdapter
    Friend WithEvents TableAdapterManager As Basic_Weigh.ScaleDataSetTableAdapters.TableAdapterManager
    Friend WithEvents TicketLabel1 As System.Windows.Forms.Label
    Friend WithEvents Date_InLabel1 As System.Windows.Forms.Label
    Friend WithEvents Date_OutLabel1 As System.Windows.Forms.Label
    Friend WithEvents CarrierComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents TruckComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents CommentsTextBox As System.Windows.Forms.TextBox
    Friend WithEvents LocationComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents ProductComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents pnlGTN As System.Windows.Forms.GroupBox
    Friend WithEvents lblNet As System.Windows.Forms.Label
    Friend WithEvents lblTare As System.Windows.Forms.Label
    Friend WithEvents lblGross As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Out_WeightLabel As System.Windows.Forms.Label
    Friend WithEvents lblMotionIn As System.Windows.Forms.Label
    Friend WithEvents pnlSave As System.Windows.Forms.Panel
    Friend WithEvents CustomerBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents CustomerTableAdapter As Basic_Weigh.ScaleDataSetTableAdapters.CustomerTableAdapter
    Friend WithEvents CarrierBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents CarrierTableAdapter As Basic_Weigh.ScaleDataSetTableAdapters.CarrierTableAdapter
    Friend WithEvents TruckBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents TruckTableAdapter As Basic_Weigh.ScaleDataSetTableAdapters.TruckTableAdapter
    Friend WithEvents LocationBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents LocationTableAdapter As Basic_Weigh.ScaleDataSetTableAdapters.LocationTableAdapter
    Friend WithEvents DestinationBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents DestinationTableAdapter As Basic_Weigh.ScaleDataSetTableAdapters.DestinationTableAdapter
    Friend WithEvents tmrUpdate As System.Windows.Forms.Timer
    Friend WithEvents CustomerComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents ckUseScale As System.Windows.Forms.CheckBox
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents In_Weight As System.Windows.Forms.NumericUpDown
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Out_Weight As System.Windows.Forms.NumericUpDown
    Friend WithEvents pnlOutWt As System.Windows.Forms.Panel
    Friend WithEvents lblMotionOut As System.Windows.Forms.Label
    Friend WithEvents lblStored As Label
    Friend WithEvents lblStoredWt As Label
    Friend WithEvents lblWeight As Label
    Friend WithEvents lblTotalWeight As Label
    Friend WithEvents btnSplit As Button
    Friend WithEvents lblPup As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents btnPup As Button
    Friend WithEvents In_DatePicker As DateTimePicker
    Friend WithEvents In_TimePicker As DateTimePicker
    Friend WithEvents Out_TimePicker As DateTimePicker
    Friend WithEvents Out_DatePicker As DateTimePicker
    Friend WithEvents lblSetDateIn As Label
    Friend WithEvents lblSetDateOut As Label
    Friend WithEvents ProductFilterBindingSource As BindingSource
    Friend WithEvents ScaleDataSet1 As ScaleDataSet
    Friend WithEvents ScaleDataSet1BindingSource As BindingSource
    Friend WithEvents ScaleDataSet2 As ScaleDataSet
    Friend WithEvents CommodityBindingSource As BindingSource
    Friend WithEvents CommodityTableAdapter As ScaleDataSetTableAdapters.CommodityTableAdapter
End Class
