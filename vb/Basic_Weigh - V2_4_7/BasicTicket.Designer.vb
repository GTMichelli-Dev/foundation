<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BasicTicket
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
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtCompany = New System.Windows.Forms.TextBox()
        Me.BasicTicketBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ScaleDataSet = New Basic_Weigh.ScaleDataSet()
        Me.txtHauler = New System.Windows.Forms.TextBox()
        Me.txtTruckID = New System.Windows.Forms.TextBox()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtComments = New System.Windows.Forms.TextBox()
        Me.btnSplitWeigh = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.btnPup = New System.Windows.Forms.Button()
        Me.lblTotal = New System.Windows.Forms.Label()
        Me.lblTruck = New System.Windows.Forms.Label()
        Me.lblTrailer = New System.Windows.Forms.Label()
        Me.lblPup = New System.Windows.Forms.Label()
        Me.UpdateTimer = New System.Windows.Forms.Timer(Me.components)
        Me.lblMotion = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.lblTimeDate = New System.Windows.Forms.Label()
        CType(Me.BasicTicketBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ScaleDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(389, 20)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(174, 31)
        Me.lblTitle.TabIndex = 1
        Me.lblTitle.Text = "Basic Ticket"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(28, 306)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(154, 31)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Company :"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(65, 356)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(117, 31)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Hauler :"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(39, 400)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(143, 31)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Truck ID :"
        '
        'txtCompany
        '
        Me.txtCompany.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BasicTicketBindingSource, "Company", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.txtCompany.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCompany.Location = New System.Drawing.Point(192, 309)
        Me.txtCompany.Name = "txtCompany"
        Me.txtCompany.Size = New System.Drawing.Size(677, 38)
        Me.txtCompany.TabIndex = 1
        '
        'BasicTicketBindingSource
        '
        Me.BasicTicketBindingSource.DataMember = "BasicTicket"
        Me.BasicTicketBindingSource.DataSource = Me.ScaleDataSet
        '
        'ScaleDataSet
        '
        Me.ScaleDataSet.DataSetName = "ScaleDataSet"
        Me.ScaleDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'txtHauler
        '
        Me.txtHauler.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BasicTicketBindingSource, "Hauler", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.txtHauler.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtHauler.Location = New System.Drawing.Point(192, 353)
        Me.txtHauler.Name = "txtHauler"
        Me.txtHauler.Size = New System.Drawing.Size(677, 38)
        Me.txtHauler.TabIndex = 2
        '
        'txtTruckID
        '
        Me.txtTruckID.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BasicTicketBindingSource, "TruckID", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.txtTruckID.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtTruckID.Location = New System.Drawing.Point(192, 397)
        Me.txtTruckID.Name = "txtTruckID"
        Me.txtTruckID.Size = New System.Drawing.Size(677, 38)
        Me.txtTruckID.TabIndex = 3
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.Color.PaleGreen
        Me.btnPrint.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(317, 499)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(124, 45)
        Me.btnPrint.TabIndex = 5
        Me.btnPrint.Text = "Print"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'btnCancel
        '
        Me.btnCancel.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.btnCancel.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCancel.Location = New System.Drawing.Point(513, 499)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(122, 45)
        Me.btnCancel.TabIndex = 6
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(12, 445)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(170, 31)
        Me.Label4.TabIndex = 11
        Me.Label4.Text = "Comments :"
        '
        'txtComments
        '
        Me.txtComments.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BasicTicketBindingSource, "Comments", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.txtComments.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtComments.Location = New System.Drawing.Point(192, 442)
        Me.txtComments.Name = "txtComments"
        Me.txtComments.Size = New System.Drawing.Size(677, 38)
        Me.txtComments.TabIndex = 4
        '
        'btnSplitWeigh
        '
        Me.btnSplitWeigh.BackColor = System.Drawing.Color.Yellow
        Me.btnSplitWeigh.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSplitWeigh.Location = New System.Drawing.Point(409, 102)
        Me.btnSplitWeigh.Name = "btnSplitWeigh"
        Me.btnSplitWeigh.Size = New System.Drawing.Size(138, 41)
        Me.btnSplitWeigh.TabIndex = 7
        Me.btnSplitWeigh.Text = "Split Weigh"
        Me.btnSplitWeigh.UseVisualStyleBackColor = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(261, 165)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(58, 24)
        Me.Label5.TabIndex = 19
        Me.Label5.Text = "Truck"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(448, 165)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(63, 24)
        Me.Label6.TabIndex = 20
        Me.Label6.Text = "Trailer"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(635, 165)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(44, 24)
        Me.Label7.TabIndex = 21
        Me.Label7.Text = "Pup"
        '
        'btnPup
        '
        Me.btnPup.BackColor = System.Drawing.Color.Yellow
        Me.btnPup.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPup.Location = New System.Drawing.Point(409, 236)
        Me.btnPup.Name = "btnPup"
        Me.btnPup.Size = New System.Drawing.Size(138, 41)
        Me.btnPup.TabIndex = 8
        Me.btnPup.Text = "Add Pup"
        Me.btnPup.UseVisualStyleBackColor = False
        '
        'lblTotal
        '
        Me.lblTotal.BackColor = System.Drawing.Color.White
        Me.lblTotal.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BasicTicketBindingSource, "Weight", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, Nothing, "N0"))
        Me.lblTotal.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotal.Location = New System.Drawing.Point(397, 66)
        Me.lblTotal.Name = "lblTotal"
        Me.lblTotal.Size = New System.Drawing.Size(160, 25)
        Me.lblTotal.TabIndex = 23
        Me.lblTotal.Text = "0"
        Me.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblTruck
        '
        Me.lblTruck.BackColor = System.Drawing.Color.White
        Me.lblTruck.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTruck.Location = New System.Drawing.Point(215, 199)
        Me.lblTruck.Name = "lblTruck"
        Me.lblTruck.Size = New System.Drawing.Size(160, 25)
        Me.lblTruck.TabIndex = 24
        Me.lblTruck.Text = "0"
        Me.lblTruck.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblTrailer
        '
        Me.lblTrailer.BackColor = System.Drawing.Color.White
        Me.lblTrailer.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTrailer.Location = New System.Drawing.Point(397, 199)
        Me.lblTrailer.Name = "lblTrailer"
        Me.lblTrailer.Size = New System.Drawing.Size(160, 25)
        Me.lblTrailer.TabIndex = 25
        Me.lblTrailer.Text = "0"
        Me.lblTrailer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblPup
        '
        Me.lblPup.BackColor = System.Drawing.Color.White
        Me.lblPup.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPup.Location = New System.Drawing.Point(576, 199)
        Me.lblPup.Name = "lblPup"
        Me.lblPup.Size = New System.Drawing.Size(160, 25)
        Me.lblPup.TabIndex = 26
        Me.lblPup.Text = "0"
        Me.lblPup.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'UpdateTimer
        '
        Me.UpdateTimer.Interval = 200
        '
        'lblMotion
        '
        Me.lblMotion.AutoSize = True
        Me.lblMotion.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMotion.ForeColor = System.Drawing.Color.Red
        Me.lblMotion.Location = New System.Drawing.Point(581, 66)
        Me.lblMotion.Name = "lblMotion"
        Me.lblMotion.Size = New System.Drawing.Size(101, 25)
        Me.lblMotion.TabIndex = 27
        Me.lblMotion.Text = "MOTION"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(12, 110)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(147, 25)
        Me.Label8.TabIndex = 28
        Me.Label8.Text = "Date / Time :"
        '
        'lblTimeDate
        '
        Me.lblTimeDate.AutoSize = True
        Me.lblTimeDate.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTimeDate.Location = New System.Drawing.Point(165, 110)
        Me.lblTimeDate.Name = "lblTimeDate"
        Me.lblTimeDate.Size = New System.Drawing.Size(77, 25)
        Me.lblTimeDate.TabIndex = 29
        Me.lblTimeDate.Text = "Label9"
        '
        'BasicTicket
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(957, 571)
        Me.Controls.Add(Me.lblTimeDate)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.lblMotion)
        Me.Controls.Add(Me.lblPup)
        Me.Controls.Add(Me.lblTrailer)
        Me.Controls.Add(Me.lblTruck)
        Me.Controls.Add(Me.lblTotal)
        Me.Controls.Add(Me.btnPup)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.btnSplitWeigh)
        Me.Controls.Add(Me.txtComments)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnPrint)
        Me.Controls.Add(Me.txtTruckID)
        Me.Controls.Add(Me.txtHauler)
        Me.Controls.Add(Me.txtCompany)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblTitle)
        Me.Name = "BasicTicket"
        Me.Text = "BasicTicket"
        CType(Me.BasicTicketBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ScaleDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblTitle As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents txtCompany As TextBox
    Friend WithEvents txtHauler As TextBox
    Friend WithEvents txtTruckID As TextBox
    Friend WithEvents btnPrint As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents Label4 As Label
    Friend WithEvents txtComments As TextBox
    Friend WithEvents btnSplitWeigh As Button
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents btnPup As Button
    Friend WithEvents lblTotal As Label
    Friend WithEvents lblTruck As Label
    Friend WithEvents lblTrailer As Label
    Friend WithEvents lblPup As Label
    Friend WithEvents UpdateTimer As Timer
    Friend WithEvents lblMotion As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents lblTimeDate As Label
    Friend WithEvents BasicTicketBindingSource As BindingSource
    Friend WithEvents ScaleDataSet As ScaleDataSet
End Class
