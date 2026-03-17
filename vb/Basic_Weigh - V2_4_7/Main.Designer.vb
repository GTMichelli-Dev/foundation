<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Main
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
        Me.SerialPort1 = New System.IO.Ports.SerialPort(Me.components)
        Me.tmrUpdate = New System.Windows.Forms.Timer(Me.components)
        Me.lblWeight = New System.Windows.Forms.Label()
        Me.lblCnt = New System.Windows.Forms.Label()
        Me.btnZero = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.btnAlarm = New System.Windows.Forms.Button()
        Me.btnSplit = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.btnBasicTicket = New System.Windows.Forms.Button()
        Me.btnReports = New System.Windows.Forms.Button()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.btnWeigh_In = New System.Windows.Forms.Button()
        Me.btnChangeLight = New System.Windows.Forms.Button()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.lblMotion = New System.Windows.Forms.Label()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditTablesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tmrAlarm = New System.Windows.Forms.Timer(Me.components)
        Me.hsScaleBar = New System.Windows.Forms.HScrollBar()
        Me.SerialPort2 = New System.IO.Ports.SerialPort(Me.components)
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.lblSplitWeigh = New System.Windows.Forms.Label()
        Me.lblTotalWeigh = New System.Windows.Forms.Label()
        Me.lblSplitMotion = New System.Windows.Forms.Label()
        Me.lblPup = New System.Windows.Forms.Label()
        Me.btnPup = New System.Windows.Forms.Button()
        Me.pct_Image = New System.Windows.Forms.PictureBox()
        Me.AxVLCPlugin21 = New AxAXVLC.AxVLCPlugin2()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel3.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.pct_Image, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.AxVLCPlugin21, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SerialPort1
        '
        '
        'tmrUpdate
        '
        Me.tmrUpdate.Enabled = True
        Me.tmrUpdate.Interval = 500
        '
        'lblWeight
        '
        Me.lblWeight.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.lblWeight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblWeight.Font = New System.Drawing.Font("Microsoft Sans Serif", 27.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblWeight.Location = New System.Drawing.Point(243, 33)
        Me.lblWeight.Name = "lblWeight"
        Me.lblWeight.Size = New System.Drawing.Size(160, 44)
        Me.lblWeight.TabIndex = 0
        Me.lblWeight.Text = "------"
        Me.lblWeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblCnt
        '
        Me.lblCnt.AutoSize = True
        Me.lblCnt.Location = New System.Drawing.Point(164, 65)
        Me.lblCnt.Name = "lblCnt"
        Me.lblCnt.Size = New System.Drawing.Size(13, 13)
        Me.lblCnt.TabIndex = 1
        Me.lblCnt.Text = "0"
        '
        'btnZero
        '
        Me.btnZero.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.btnZero.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.btnZero.Location = New System.Drawing.Point(118, 92)
        Me.btnZero.Name = "btnZero"
        Me.btnZero.Size = New System.Drawing.Size(108, 46)
        Me.btnZero.TabIndex = 4
        Me.btnZero.Text = "Zero Scale"
        Me.btnZero.UseVisualStyleBackColor = False
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.White
        Me.Panel1.Controls.Add(Me.btnAlarm)
        Me.Panel1.Controls.Add(Me.btnSplit)
        Me.Panel1.Controls.Add(Me.Panel2)
        Me.Panel1.Controls.Add(Me.btnChangeLight)
        Me.Panel1.Controls.Add(Me.PictureBox1)
        Me.Panel1.Controls.Add(Me.btnZero)
        Me.Panel1.Controls.Add(Me.lblCnt)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel1.Location = New System.Drawing.Point(0, 24)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(229, 697)
        Me.Panel1.TabIndex = 6
        '
        'btnAlarm
        '
        Me.btnAlarm.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.btnAlarm.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAlarm.Location = New System.Drawing.Point(118, 142)
        Me.btnAlarm.Name = "btnAlarm"
        Me.btnAlarm.Size = New System.Drawing.Size(108, 46)
        Me.btnAlarm.TabIndex = 11
        Me.btnAlarm.Text = "Cancel Alarm"
        Me.btnAlarm.UseVisualStyleBackColor = False
        '
        'btnSplit
        '
        Me.btnSplit.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.btnSplit.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.btnSplit.Location = New System.Drawing.Point(118, 9)
        Me.btnSplit.Name = "btnSplit"
        Me.btnSplit.Size = New System.Drawing.Size(108, 46)
        Me.btnSplit.TabIndex = 10
        Me.btnSplit.Text = "Split Weigh"
        Me.btnSplit.UseVisualStyleBackColor = False
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.Panel2.Controls.Add(Me.Panel3)
        Me.Panel2.Location = New System.Drawing.Point(4, 204)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(221, 441)
        Me.Panel2.TabIndex = 9
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.White
        Me.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel3.Controls.Add(Me.btnBasicTicket)
        Me.Panel3.Controls.Add(Me.btnReports)
        Me.Panel3.Controls.Add(Me.Button4)
        Me.Panel3.Controls.Add(Me.Button3)
        Me.Panel3.Controls.Add(Me.Button2)
        Me.Panel3.Controls.Add(Me.btnWeigh_In)
        Me.Panel3.Location = New System.Drawing.Point(9, 15)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(202, 405)
        Me.Panel3.TabIndex = 10
        '
        'btnBasicTicket
        '
        Me.btnBasicTicket.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.btnBasicTicket.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnBasicTicket.Location = New System.Drawing.Point(3, 138)
        Me.btnBasicTicket.Name = "btnBasicTicket"
        Me.btnBasicTicket.Size = New System.Drawing.Size(192, 60)
        Me.btnBasicTicket.TabIndex = 10
        Me.btnBasicTicket.Text = "Basic Ticket"
        Me.btnBasicTicket.UseVisualStyleBackColor = False
        '
        'btnReports
        '
        Me.btnReports.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.btnReports.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnReports.Location = New System.Drawing.Point(3, 270)
        Me.btnReports.Name = "btnReports"
        Me.btnReports.Size = New System.Drawing.Size(192, 60)
        Me.btnReports.TabIndex = 12
        Me.btnReports.Text = "Reports"
        Me.btnReports.UseVisualStyleBackColor = False
        '
        'Button4
        '
        Me.Button4.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.Button4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button4.Location = New System.Drawing.Point(3, 336)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(192, 60)
        Me.Button4.TabIndex = 13
        Me.Button4.Text = "Edit Tables"
        Me.Button4.UseVisualStyleBackColor = False
        '
        'Button3
        '
        Me.Button3.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.Button3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button3.Location = New System.Drawing.Point(3, 204)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(192, 60)
        Me.Button3.TabIndex = 11
        Me.Button3.Text = "Completed Tickets"
        Me.Button3.UseVisualStyleBackColor = False
        '
        'Button2
        '
        Me.Button2.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.Button2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button2.Location = New System.Drawing.Point(3, 72)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(192, 60)
        Me.Button2.TabIndex = 9
        Me.Button2.Text = "Weigh Out"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'btnWeigh_In
        '
        Me.btnWeigh_In.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.btnWeigh_In.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnWeigh_In.Location = New System.Drawing.Point(3, 6)
        Me.btnWeigh_In.Name = "btnWeigh_In"
        Me.btnWeigh_In.Size = New System.Drawing.Size(192, 60)
        Me.btnWeigh_In.TabIndex = 8
        Me.btnWeigh_In.Text = "Weigh In"
        Me.btnWeigh_In.UseVisualStyleBackColor = False
        '
        'btnChangeLight
        '
        Me.btnChangeLight.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.btnChangeLight.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnChangeLight.Location = New System.Drawing.Point(4, 142)
        Me.btnChangeLight.Name = "btnChangeLight"
        Me.btnChangeLight.Size = New System.Drawing.Size(108, 46)
        Me.btnChangeLight.TabIndex = 7
        Me.btnChangeLight.Text = "Change Light"
        Me.btnChangeLight.UseVisualStyleBackColor = False
        '
        'PictureBox1
        '
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PictureBox1.Location = New System.Drawing.Point(12, 9)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(100, 112)
        Me.PictureBox1.TabIndex = 6
        Me.PictureBox1.TabStop = False
        '
        'lblMotion
        '
        Me.lblMotion.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMotion.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblMotion.Location = New System.Drawing.Point(243, 81)
        Me.lblMotion.Name = "lblMotion"
        Me.lblMotion.Size = New System.Drawing.Size(143, 23)
        Me.lblMotion.TabIndex = 5
        Me.lblMotion.Text = "Connecting"
        Me.lblMotion.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'MenuStrip1
        '
        Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(1284, 24)
        Me.MenuStrip1.TabIndex = 7
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExitToolStripMenuItem, Me.SettingsToolStripMenuItem, Me.EditTablesToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(130, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'SettingsToolStripMenuItem
        '
        Me.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
        Me.SettingsToolStripMenuItem.Size = New System.Drawing.Size(130, 22)
        Me.SettingsToolStripMenuItem.Text = "Settings"
        '
        'EditTablesToolStripMenuItem
        '
        Me.EditTablesToolStripMenuItem.Name = "EditTablesToolStripMenuItem"
        Me.EditTablesToolStripMenuItem.Size = New System.Drawing.Size(130, 22)
        Me.EditTablesToolStripMenuItem.Text = "Edit Tables"
        '
        'tmrAlarm
        '
        Me.tmrAlarm.Interval = 30000
        '
        'hsScaleBar
        '
        Me.hsScaleBar.Location = New System.Drawing.Point(243, 0)
        Me.hsScaleBar.Maximum = 140010
        Me.hsScaleBar.Name = "hsScaleBar"
        Me.hsScaleBar.Size = New System.Drawing.Size(929, 20)
        Me.hsScaleBar.SmallChange = 10
        Me.hsScaleBar.TabIndex = 8
        '
        'lblSplitWeigh
        '
        Me.lblSplitWeigh.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.lblSplitWeigh.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblSplitWeigh.Font = New System.Drawing.Font("Microsoft Sans Serif", 27.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSplitWeigh.Location = New System.Drawing.Point(409, 33)
        Me.lblSplitWeigh.Name = "lblSplitWeigh"
        Me.lblSplitWeigh.Size = New System.Drawing.Size(160, 44)
        Me.lblSplitWeigh.TabIndex = 11
        Me.lblSplitWeigh.Text = "------"
        Me.lblSplitWeigh.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblSplitWeigh.Visible = False
        '
        'lblTotalWeigh
        '
        Me.lblTotalWeigh.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.lblTotalWeigh.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblTotalWeigh.Font = New System.Drawing.Font("Microsoft Sans Serif", 27.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotalWeigh.Location = New System.Drawing.Point(741, 33)
        Me.lblTotalWeigh.Name = "lblTotalWeigh"
        Me.lblTotalWeigh.Size = New System.Drawing.Size(160, 44)
        Me.lblTotalWeigh.TabIndex = 12
        Me.lblTotalWeigh.Text = "------"
        Me.lblTotalWeigh.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblTotalWeigh.Visible = False
        '
        'lblSplitMotion
        '
        Me.lblSplitMotion.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSplitMotion.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblSplitMotion.Location = New System.Drawing.Point(728, 81)
        Me.lblSplitMotion.Name = "lblSplitMotion"
        Me.lblSplitMotion.Size = New System.Drawing.Size(143, 23)
        Me.lblSplitMotion.TabIndex = 13
        Me.lblSplitMotion.Text = "Connecting"
        Me.lblSplitMotion.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblPup
        '
        Me.lblPup.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.lblPup.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblPup.Font = New System.Drawing.Font("Microsoft Sans Serif", 27.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPup.Location = New System.Drawing.Point(575, 33)
        Me.lblPup.Name = "lblPup"
        Me.lblPup.Size = New System.Drawing.Size(160, 44)
        Me.lblPup.TabIndex = 14
        Me.lblPup.Text = "------"
        Me.lblPup.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblPup.Visible = False
        '
        'btnPup
        '
        Me.btnPup.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.btnPup.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPup.Location = New System.Drawing.Point(441, 81)
        Me.btnPup.Name = "btnPup"
        Me.btnPup.Size = New System.Drawing.Size(96, 29)
        Me.btnPup.TabIndex = 15
        Me.btnPup.Text = "Add Pup"
        Me.btnPup.UseVisualStyleBackColor = False
        Me.btnPup.Visible = False
        '
        'pct_Image
        '
        Me.pct_Image.InitialImage = Nothing
        Me.pct_Image.Location = New System.Drawing.Point(235, 116)
        Me.pct_Image.Name = "pct_Image"
        Me.pct_Image.Size = New System.Drawing.Size(1017, 553)
        Me.pct_Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pct_Image.TabIndex = 16
        Me.pct_Image.TabStop = False
        Me.pct_Image.Visible = False
        '
        'AxVLCPlugin21
        '
        Me.AxVLCPlugin21.Enabled = True
        Me.AxVLCPlugin21.Location = New System.Drawing.Point(308, 141)
        Me.AxVLCPlugin21.Name = "AxVLCPlugin21"
        Me.AxVLCPlugin21.OcxState = CType(resources.GetObject("AxVLCPlugin21.OcxState"), System.Windows.Forms.AxHost.State)
        Me.AxVLCPlugin21.Size = New System.Drawing.Size(1197, 665)
        Me.AxVLCPlugin21.TabIndex = 9
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1284, 721)
        Me.Controls.Add(Me.pct_Image)
        Me.Controls.Add(Me.btnPup)
        Me.Controls.Add(Me.lblPup)
        Me.Controls.Add(Me.lblSplitMotion)
        Me.Controls.Add(Me.lblTotalWeigh)
        Me.Controls.Add(Me.lblSplitWeigh)
        Me.Controls.Add(Me.lblMotion)
        Me.Controls.Add(Me.AxVLCPlugin21)
        Me.Controls.Add(Me.hsScaleBar)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.lblWeight)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "Main"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Main"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        CType(Me.pct_Image, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.AxVLCPlugin21, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents SerialPort1 As System.IO.Ports.SerialPort
    Friend WithEvents tmrUpdate As System.Windows.Forms.Timer
    Friend WithEvents lblWeight As System.Windows.Forms.Label
    Friend WithEvents lblCnt As System.Windows.Forms.Label
    Friend WithEvents btnZero As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents lblMotion As System.Windows.Forms.Label
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents btnChangeLight As System.Windows.Forms.Button
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents btnWeigh_In As System.Windows.Forms.Button
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Button4 As System.Windows.Forms.Button
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents SettingsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tmrAlarm As System.Windows.Forms.Timer
    Friend WithEvents hsScaleBar As System.Windows.Forms.HScrollBar
    Friend WithEvents SerialPort2 As IO.Ports.SerialPort
    Friend WithEvents AxVLCPlugin21 As AxAXVLC.AxVLCPlugin2
    Friend WithEvents Timer1 As Timer
    Friend WithEvents lblSplitWeigh As Label
    Friend WithEvents lblTotalWeigh As Label
    Friend WithEvents btnSplit As Button
    Friend WithEvents lblSplitMotion As Label
    Friend WithEvents lblPup As Label
    Friend WithEvents btnPup As Button
    Friend WithEvents btnReports As Button
    Friend WithEvents pct_Image As PictureBox
    Friend WithEvents btnBasicTicket As Button
    Friend WithEvents btnAlarm As Button
    Friend WithEvents EditTablesToolStripMenuItem As ToolStripMenuItem
End Class
