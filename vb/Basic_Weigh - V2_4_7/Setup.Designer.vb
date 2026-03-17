<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Setup
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim PrinterNameLabel As System.Windows.Forms.Label
        Dim ComPortLabel As System.Windows.Forms.Label
        Dim WaveFileLabel As System.Windows.Forms.Label
        Dim Ticket_NumberLabel As System.Windows.Forms.Label
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.PrinterNameTextBox = New System.Windows.Forms.TextBox()
        Me.SetupBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ScaleDataSet = New Basic_Weigh.ScaleDataSet()
        Me.btnPrinter = New System.Windows.Forms.Button()
        Me.cbComPort = New System.Windows.Forms.ComboBox()
        Me.WaveFileTextBox = New System.Windows.Forms.TextBox()
        Me.btnSound = New System.Windows.Forms.Button()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.Ticket_NumberNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.TextBox3 = New System.Windows.Forms.TextBox()
        Me.TextBox4 = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.cbBaudRate = New System.Windows.Forms.ComboBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.cbWordLength = New System.Windows.Forms.ComboBox()
        Me.cbParity = New System.Windows.Forms.ComboBox()
        Me.cbStopBits = New System.Windows.Forms.ComboBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.rdoSimulate = New System.Windows.Forms.RadioButton()
        Me.rdoNetwork = New System.Windows.Forms.RadioButton()
        Me.rdoSerial = New System.Windows.Forms.RadioButton()
        Me.txtIPAddress = New System.Windows.Forms.TextBox()
        Me.lblIPAddress = New System.Windows.Forms.Label()
        Me.txtNetworkPort = New System.Windows.Forms.TextBox()
        Me.lblNetworkPort = New System.Windows.Forms.Label()
        Me.ckSMAEnabled = New System.Windows.Forms.CheckBox()
        Me.lblTicketsperPage = New System.Windows.Forms.Label()
        Me.nuTicketsperPage = New System.Windows.Forms.NumericUpDown()
        Me.nuPagesToPrint = New System.Windows.Forms.NumericUpDown()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.SetupTableAdapter = New Basic_Weigh.ScaleDataSetTableAdapters.SetupTableAdapter()
        Me.TableAdapterManager = New Basic_Weigh.ScaleDataSetTableAdapters.TableAdapterManager()
        PrinterNameLabel = New System.Windows.Forms.Label()
        ComPortLabel = New System.Windows.Forms.Label()
        WaveFileLabel = New System.Windows.Forms.Label()
        Ticket_NumberLabel = New System.Windows.Forms.Label()
        CType(Me.SetupBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ScaleDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Ticket_NumberNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        CType(Me.nuTicketsperPage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nuPagesToPrint, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PrinterNameLabel
        '
        PrinterNameLabel.AutoSize = True
        PrinterNameLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        PrinterNameLabel.Location = New System.Drawing.Point(11, 206)
        PrinterNameLabel.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        PrinterNameLabel.Name = "PrinterNameLabel"
        PrinterNameLabel.Size = New System.Drawing.Size(102, 16)
        PrinterNameLabel.TabIndex = 2
        PrinterNameLabel.Text = "Printer Name:"
        '
        'ComPortLabel
        '
        ComPortLabel.AutoSize = True
        ComPortLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        ComPortLabel.Location = New System.Drawing.Point(207, 316)
        ComPortLabel.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        ComPortLabel.Name = "ComPortLabel"
        ComPortLabel.Size = New System.Drawing.Size(75, 16)
        ComPortLabel.TabIndex = 4
        ComPortLabel.Text = "Com Port:"
        '
        'WaveFileLabel
        '
        WaveFileLabel.AutoSize = True
        WaveFileLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        WaveFileLabel.Location = New System.Drawing.Point(11, 236)
        WaveFileLabel.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        WaveFileLabel.Name = "WaveFileLabel"
        WaveFileLabel.Size = New System.Drawing.Size(92, 16)
        WaveFileLabel.TabIndex = 6
        WaveFileLabel.Text = "Alert Sound:"
        '
        'Ticket_NumberLabel
        '
        Ticket_NumberLabel.AutoSize = True
        Ticket_NumberLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Ticket_NumberLabel.Location = New System.Drawing.Point(11, 175)
        Ticket_NumberLabel.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Ticket_NumberLabel.Name = "Ticket_NumberLabel"
        Ticket_NumberLabel.Size = New System.Drawing.Size(166, 16)
        Ticket_NumberLabel.TabIndex = 9
        Ticket_NumberLabel.Text = "Current Ticket Number:"
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(487, 471)
        Me.OK_Button.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(100, 28)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(597, 471)
        Me.Cancel_Button.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(100, 28)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Cancel"
        '
        'PrinterNameTextBox
        '
        Me.PrinterNameTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PrinterNameTextBox.Location = New System.Drawing.Point(139, 203)
        Me.PrinterNameTextBox.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.PrinterNameTextBox.Name = "PrinterNameTextBox"
        Me.PrinterNameTextBox.Size = New System.Drawing.Size(448, 22)
        Me.PrinterNameTextBox.TabIndex = 3
        '
        'SetupBindingSource
        '
        Me.SetupBindingSource.DataMember = "Setup"
        Me.SetupBindingSource.DataSource = Me.ScaleDataSet
        '
        'ScaleDataSet
        '
        Me.ScaleDataSet.DataSetName = "ScaleDataSet"
        Me.ScaleDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'btnPrinter
        '
        Me.btnPrinter.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPrinter.Location = New System.Drawing.Point(598, 203)
        Me.btnPrinter.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.btnPrinter.Name = "btnPrinter"
        Me.btnPrinter.Size = New System.Drawing.Size(48, 22)
        Me.btnPrinter.TabIndex = 4
        Me.btnPrinter.Text = "..."
        Me.btnPrinter.UseVisualStyleBackColor = True
        '
        'cbComPort
        '
        Me.cbComPort.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbComPort.FormattingEnabled = True
        Me.cbComPort.Location = New System.Drawing.Point(187, 336)
        Me.cbComPort.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.cbComPort.Name = "cbComPort"
        Me.cbComPort.Size = New System.Drawing.Size(109, 24)
        Me.cbComPort.TabIndex = 5
        '
        'WaveFileTextBox
        '
        Me.WaveFileTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WaveFileTextBox.Location = New System.Drawing.Point(139, 233)
        Me.WaveFileTextBox.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.WaveFileTextBox.Name = "WaveFileTextBox"
        Me.WaveFileTextBox.Size = New System.Drawing.Size(448, 22)
        Me.WaveFileTextBox.TabIndex = 7
        '
        'btnSound
        '
        Me.btnSound.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSound.Location = New System.Drawing.Point(598, 233)
        Me.btnSound.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.btnSound.Name = "btnSound"
        Me.btnSound.Size = New System.Drawing.Size(48, 22)
        Me.btnSound.TabIndex = 8
        Me.btnSound.Text = "..."
        Me.btnSound.UseVisualStyleBackColor = True
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        Me.OpenFileDialog1.Filter = "Wav Files (*.wav)|*.wav|All Files (*.*)|*.*"
        '
        'Ticket_NumberNumericUpDown
        '
        Me.Ticket_NumberNumericUpDown.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.SetupBindingSource, "Ticket_Number", True))
        Me.Ticket_NumberNumericUpDown.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Ticket_NumberNumericUpDown.Location = New System.Drawing.Point(185, 173)
        Me.Ticket_NumberNumericUpDown.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.Ticket_NumberNumericUpDown.Maximum = New Decimal(New Integer() {99999999, 0, 0, 0})
        Me.Ticket_NumberNumericUpDown.Name = "Ticket_NumberNumericUpDown"
        Me.Ticket_NumberNumericUpDown.Size = New System.Drawing.Size(86, 22)
        Me.Ticket_NumberNumericUpDown.TabIndex = 10
        Me.Ticket_NumberNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.Label1.Location = New System.Drawing.Point(276, 6)
        Me.Label1.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(171, 29)
        Me.Label1.TabIndex = 11
        Me.Label1.Text = "Setup Screen"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(11, 56)
        Me.Label2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(123, 16)
        Me.Label2.TabIndex = 12
        Me.Label2.Text = "Ticket Header 1:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(11, 85)
        Me.Label3.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(123, 16)
        Me.Label3.TabIndex = 13
        Me.Label3.Text = "Ticket Header 2:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(11, 114)
        Me.Label4.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(123, 16)
        Me.Label4.TabIndex = 14
        Me.Label4.Text = "Ticket Header 3:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(11, 143)
        Me.Label5.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(123, 16)
        Me.Label5.TabIndex = 15
        Me.Label5.Text = "Ticket Header 4:"
        '
        'TextBox1
        '
        Me.TextBox1.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.SetupBindingSource, "Header1", True))
        Me.TextBox1.Location = New System.Drawing.Point(142, 53)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(555, 22)
        Me.TextBox1.TabIndex = 16
        '
        'TextBox2
        '
        Me.TextBox2.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.SetupBindingSource, "Header2", True))
        Me.TextBox2.Location = New System.Drawing.Point(142, 82)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(555, 22)
        Me.TextBox2.TabIndex = 17
        '
        'TextBox3
        '
        Me.TextBox3.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.SetupBindingSource, "Header3", True))
        Me.TextBox3.Location = New System.Drawing.Point(142, 111)
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.Size = New System.Drawing.Size(555, 22)
        Me.TextBox3.TabIndex = 18
        '
        'TextBox4
        '
        Me.TextBox4.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.SetupBindingSource, "Header4", True))
        Me.TextBox4.Location = New System.Drawing.Point(142, 140)
        Me.TextBox4.Name = "TextBox4"
        Me.TextBox4.Size = New System.Drawing.Size(555, 22)
        Me.TextBox4.TabIndex = 19
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(225, 269)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(307, 25)
        Me.Label6.TabIndex = 20
        Me.Label6.Text = "Communication Parameters:"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(320, 316)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(85, 16)
        Me.Label7.TabIndex = 21
        Me.Label7.Text = "Baud Rate:"
        '
        'cbBaudRate
        '
        Me.cbBaudRate.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbBaudRate.FormattingEnabled = True
        Me.cbBaudRate.Items.AddRange(New Object() {"1200", "2400", "4800", "9600", "19200", "38400"})
        Me.cbBaudRate.Location = New System.Drawing.Point(301, 336)
        Me.cbBaudRate.Name = "cbBaudRate"
        Me.cbBaudRate.Size = New System.Drawing.Size(121, 24)
        Me.cbBaudRate.TabIndex = 22
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(546, 317)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(52, 16)
        Me.Label8.TabIndex = 23
        Me.Label8.Text = "Parity:"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(627, 317)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(74, 16)
        Me.Label9.TabIndex = 24
        Me.Label9.Text = "Stop Bits:"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(428, 317)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(99, 16)
        Me.Label10.TabIndex = 25
        Me.Label10.Text = "Word Length:"
        '
        'cbWordLength
        '
        Me.cbWordLength.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbWordLength.FormattingEnabled = True
        Me.cbWordLength.Items.AddRange(New Object() {"7", "8"})
        Me.cbWordLength.Location = New System.Drawing.Point(428, 336)
        Me.cbWordLength.Name = "cbWordLength"
        Me.cbWordLength.Size = New System.Drawing.Size(96, 24)
        Me.cbWordLength.TabIndex = 26
        '
        'cbParity
        '
        Me.cbParity.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbParity.FormattingEnabled = True
        Me.cbParity.Items.AddRange(New Object() {"Even", "None", "Odd", "Space"})
        Me.cbParity.Location = New System.Drawing.Point(530, 336)
        Me.cbParity.Name = "cbParity"
        Me.cbParity.Size = New System.Drawing.Size(94, 24)
        Me.cbParity.TabIndex = 27
        '
        'cbStopBits
        '
        Me.cbStopBits.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbStopBits.FormattingEnabled = True
        Me.cbStopBits.Items.AddRange(New Object() {"1", "2"})
        Me.cbStopBits.Location = New System.Drawing.Point(630, 336)
        Me.cbStopBits.Name = "cbStopBits"
        Me.cbStopBits.Size = New System.Drawing.Size(67, 24)
        Me.cbStopBits.TabIndex = 28
        '
        'Panel1
        '
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel1.Controls.Add(Me.rdoSimulate)
        Me.Panel1.Controls.Add(Me.rdoNetwork)
        Me.Panel1.Controls.Add(Me.rdoSerial)
        Me.Panel1.Location = New System.Drawing.Point(12, 317)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(167, 183)
        Me.Panel1.TabIndex = 29
        '
        'rdoSimulate
        '
        Me.rdoSimulate.AutoSize = True
        Me.rdoSimulate.Location = New System.Drawing.Point(3, 137)
        Me.rdoSimulate.Name = "rdoSimulate"
        Me.rdoSimulate.Size = New System.Drawing.Size(86, 20)
        Me.rdoSimulate.TabIndex = 2
        Me.rdoSimulate.TabStop = True
        Me.rdoSimulate.Text = "Simulate"
        Me.rdoSimulate.UseVisualStyleBackColor = True
        '
        'rdoNetwork
        '
        Me.rdoNetwork.AutoSize = True
        Me.rdoNetwork.Location = New System.Drawing.Point(3, 80)
        Me.rdoNetwork.Name = "rdoNetwork"
        Me.rdoNetwork.Size = New System.Drawing.Size(163, 20)
        Me.rdoNetwork.TabIndex = 1
        Me.rdoNetwork.TabStop = True
        Me.rdoNetwork.Text = "Network Connection"
        Me.rdoNetwork.UseVisualStyleBackColor = True
        '
        'rdoSerial
        '
        Me.rdoSerial.AutoSize = True
        Me.rdoSerial.Location = New System.Drawing.Point(3, 18)
        Me.rdoSerial.Name = "rdoSerial"
        Me.rdoSerial.Size = New System.Drawing.Size(148, 20)
        Me.rdoSerial.TabIndex = 0
        Me.rdoSerial.TabStop = True
        Me.rdoSerial.Text = "Serial Connection"
        Me.rdoSerial.UseVisualStyleBackColor = True
        '
        'txtIPAddress
        '
        Me.txtIPAddress.Location = New System.Drawing.Point(187, 399)
        Me.txtIPAddress.Name = "txtIPAddress"
        Me.txtIPAddress.Size = New System.Drawing.Size(157, 22)
        Me.txtIPAddress.TabIndex = 30
        '
        'lblIPAddress
        '
        Me.lblIPAddress.AutoSize = True
        Me.lblIPAddress.Location = New System.Drawing.Point(187, 380)
        Me.lblIPAddress.Name = "lblIPAddress"
        Me.lblIPAddress.Size = New System.Drawing.Size(84, 16)
        Me.lblIPAddress.TabIndex = 31
        Me.lblIPAddress.Text = "IP Address"
        '
        'txtNetworkPort
        '
        Me.txtNetworkPort.Location = New System.Drawing.Point(351, 399)
        Me.txtNetworkPort.Name = "txtNetworkPort"
        Me.txtNetworkPort.Size = New System.Drawing.Size(100, 22)
        Me.txtNetworkPort.TabIndex = 32
        '
        'lblNetworkPort
        '
        Me.lblNetworkPort.AutoSize = True
        Me.lblNetworkPort.Location = New System.Drawing.Point(351, 380)
        Me.lblNetworkPort.Name = "lblNetworkPort"
        Me.lblNetworkPort.Size = New System.Drawing.Size(96, 16)
        Me.lblNetworkPort.TabIndex = 33
        Me.lblNetworkPort.Text = "Network Port"
        '
        'ckSMAEnabled
        '
        Me.ckSMAEnabled.AutoSize = True
        Me.ckSMAEnabled.Location = New System.Drawing.Point(487, 400)
        Me.ckSMAEnabled.Name = "ckSMAEnabled"
        Me.ckSMAEnabled.Size = New System.Drawing.Size(121, 20)
        Me.ckSMAEnabled.TabIndex = 34
        Me.ckSMAEnabled.Text = "SMA Enabled"
        Me.ckSMAEnabled.UseVisualStyleBackColor = True
        '
        'lblTicketsperPage
        '
        Me.lblTicketsperPage.AutoSize = True
        Me.lblTicketsperPage.Location = New System.Drawing.Point(498, 175)
        Me.lblTicketsperPage.Name = "lblTicketsperPage"
        Me.lblTicketsperPage.Size = New System.Drawing.Size(131, 16)
        Me.lblTicketsperPage.TabIndex = 35
        Me.lblTicketsperPage.Text = "Tickets per Page:"
        '
        'nuTicketsperPage
        '
        Me.nuTicketsperPage.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.nuTicketsperPage.Location = New System.Drawing.Point(637, 173)
        Me.nuTicketsperPage.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.nuTicketsperPage.Maximum = New Decimal(New Integer() {3, 0, 0, 0})
        Me.nuTicketsperPage.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nuTicketsperPage.Name = "nuTicketsperPage"
        Me.nuTicketsperPage.Size = New System.Drawing.Size(59, 22)
        Me.nuTicketsperPage.TabIndex = 36
        Me.nuTicketsperPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.nuTicketsperPage.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'nuPagesToPrint
        '
        Me.nuPagesToPrint.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.SetupBindingSource, "Ticket_Number", True))
        Me.nuPagesToPrint.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.nuPagesToPrint.Location = New System.Drawing.Point(425, 173)
        Me.nuPagesToPrint.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.nuPagesToPrint.Maximum = New Decimal(New Integer() {3, 0, 0, 0})
        Me.nuPagesToPrint.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nuPagesToPrint.Name = "nuPagesToPrint"
        Me.nuPagesToPrint.Size = New System.Drawing.Size(54, 22)
        Me.nuPagesToPrint.TabIndex = 38
        Me.nuPagesToPrint.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.nuPagesToPrint.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(286, 175)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(136, 16)
        Me.Label11.TabIndex = 37
        Me.Label11.Text = "Number of Pages :"
        '
        'SetupTableAdapter
        '
        Me.SetupTableAdapter.ClearBeforeFill = True
        '
        'TableAdapterManager
        '
        Me.TableAdapterManager.BackupDataSetBeforeUpdate = False
        Me.TableAdapterManager.CarrierTableAdapter = Nothing
        Me.TableAdapterManager.CustomerTableAdapter = Nothing
        Me.TableAdapterManager.DestinationTableAdapter = Nothing
        Me.TableAdapterManager.LocationTableAdapter = Nothing
        Me.TableAdapterManager.SetupTableAdapter = Me.SetupTableAdapter
        Me.TableAdapterManager.TransactionsTableAdapter = Nothing
        Me.TableAdapterManager.TruckTableAdapter = Nothing
        Me.TableAdapterManager.UpdateOrder = Basic_Weigh.ScaleDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete
        '
        'Setup
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(717, 512)
        Me.Controls.Add(Me.nuPagesToPrint)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.nuTicketsperPage)
        Me.Controls.Add(Me.lblTicketsperPage)
        Me.Controls.Add(Me.ckSMAEnabled)
        Me.Controls.Add(Me.lblNetworkPort)
        Me.Controls.Add(Me.txtNetworkPort)
        Me.Controls.Add(Me.lblIPAddress)
        Me.Controls.Add(Me.txtIPAddress)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.cbStopBits)
        Me.Controls.Add(Me.cbParity)
        Me.Controls.Add(Me.cbWordLength)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.cbBaudRate)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.TextBox4)
        Me.Controls.Add(Me.TextBox3)
        Me.Controls.Add(Me.TextBox2)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Cancel_Button)
        Me.Controls.Add(Me.OK_Button)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Ticket_NumberLabel)
        Me.Controls.Add(Me.Ticket_NumberNumericUpDown)
        Me.Controls.Add(Me.btnSound)
        Me.Controls.Add(WaveFileLabel)
        Me.Controls.Add(Me.WaveFileTextBox)
        Me.Controls.Add(ComPortLabel)
        Me.Controls.Add(Me.cbComPort)
        Me.Controls.Add(Me.btnPrinter)
        Me.Controls.Add(PrinterNameLabel)
        Me.Controls.Add(Me.PrinterNameTextBox)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Setup"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Setup"
        CType(Me.SetupBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ScaleDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Ticket_NumberNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.nuTicketsperPage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nuPagesToPrint, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents ScaleDataSet As Basic_Weigh.ScaleDataSet
    Friend WithEvents SetupBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents SetupTableAdapter As Basic_Weigh.ScaleDataSetTableAdapters.SetupTableAdapter
    Friend WithEvents TableAdapterManager As Basic_Weigh.ScaleDataSetTableAdapters.TableAdapterManager
    Friend WithEvents PrinterNameTextBox As System.Windows.Forms.TextBox
    Friend WithEvents btnPrinter As System.Windows.Forms.Button
    Friend WithEvents cbComPort As System.Windows.Forms.ComboBox
    Friend WithEvents WaveFileTextBox As System.Windows.Forms.TextBox
    Friend WithEvents btnSound As System.Windows.Forms.Button
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents Ticket_NumberNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents TextBox3 As TextBox
    Friend WithEvents TextBox4 As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents cbBaudRate As ComboBox
    Friend WithEvents Label8 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents cbWordLength As ComboBox
    Friend WithEvents cbParity As ComboBox
    Friend WithEvents cbStopBits As ComboBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents rdoSimulate As RadioButton
    Friend WithEvents rdoNetwork As RadioButton
    Friend WithEvents rdoSerial As RadioButton
    Friend WithEvents txtIPAddress As TextBox
    Friend WithEvents lblIPAddress As Label
    Friend WithEvents txtNetworkPort As TextBox
    Friend WithEvents lblNetworkPort As Label
    Friend WithEvents ckSMAEnabled As CheckBox
    Friend WithEvents lblTicketsperPage As Label
    Friend WithEvents nuTicketsperPage As NumericUpDown
    Friend WithEvents nuPagesToPrint As NumericUpDown
    Friend WithEvents Label11 As Label
End Class
