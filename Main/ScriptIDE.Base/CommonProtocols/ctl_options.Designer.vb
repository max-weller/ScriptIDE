﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ctl_options
  Inherits System.Windows.Forms.UserControl

  'UserControl overrides dispose to clean up the component list.
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
    Dim IGColPattern1 As TenTec.Windows.iGridLib.iGColPattern = New TenTec.Windows.iGridLib.iGColPattern
    Dim IGColPattern2 As TenTec.Windows.iGridLib.iGColPattern = New TenTec.Windows.iGridLib.iGColPattern
    Dim IGColPattern3 As TenTec.Windows.iGridLib.iGColPattern = New TenTec.Windows.iGridLib.iGColPattern
    Dim IGColPattern4 As TenTec.Windows.iGridLib.iGColPattern = New TenTec.Windows.iGridLib.iGColPattern
    Dim IGColPattern5 As TenTec.Windows.iGridLib.iGColPattern = New TenTec.Windows.iGridLib.iGColPattern
    Me.igFtpCredCol3CellStyle = New TenTec.Windows.iGridLib.iGCellStyle(True)
    Me.igFtpCredCol3ColHdrStyle = New TenTec.Windows.iGridLib.iGColHdrStyle(True)
    Me.btnDelFtpCred = New System.Windows.Forms.Button
    Me.btnAddSFtpCred = New System.Windows.Forms.Button
    Me.igFtpCred = New TenTec.Windows.iGridLib.iGrid
    Me.GroupBox1 = New System.Windows.Forms.GroupBox
    Me.GroupBox2 = New System.Windows.Forms.GroupBox
    Me.btnPreviewSound = New System.Windows.Forms.Button
    Me.cmbSaveSound = New System.Windows.Forms.ComboBox
    Me.igFtpCredCol4CellStyle = New TenTec.Windows.iGridLib.iGCellStyle(True)
    Me.igFtpCredCol4ColHdrStyle = New TenTec.Windows.iGridLib.iGColHdrStyle(True)
    Me.btnAddFtpCred = New System.Windows.Forms.Button
    Me.btnSelPrivKey = New System.Windows.Forms.Button
    CType(Me.igFtpCred, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.GroupBox1.SuspendLayout()
    Me.GroupBox2.SuspendLayout()
    Me.SuspendLayout()
    '
    'btnDelFtpCred
    '
    Me.btnDelFtpCred.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnDelFtpCred.Location = New System.Drawing.Point(297, 249)
    Me.btnDelFtpCred.Name = "btnDelFtpCred"
    Me.btnDelFtpCred.Size = New System.Drawing.Size(73, 25)
    Me.btnDelFtpCred.TabIndex = 31
    Me.btnDelFtpCred.Text = "Löschen"
    Me.btnDelFtpCred.UseVisualStyleBackColor = True
    '
    'btnAddSFtpCred
    '
    Me.btnAddSFtpCred.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnAddSFtpCred.Location = New System.Drawing.Point(218, 249)
    Me.btnAddSFtpCred.Name = "btnAddSFtpCred"
    Me.btnAddSFtpCred.Size = New System.Drawing.Size(73, 25)
    Me.btnAddSFtpCred.TabIndex = 30
    Me.btnAddSFtpCred.Text = "Neu SSH"
    Me.btnAddSFtpCred.UseVisualStyleBackColor = True
    '
    'igFtpCred
    '
    Me.igFtpCred.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.igFtpCred.AutoResizeCols = True
    IGColPattern1.Text = "Host"
    IGColPattern1.Width = 115
    IGColPattern2.Text = "User"
    IGColPattern2.Width = 71
    IGColPattern3.Text = "Pass"
    IGColPattern3.Width = 69
    IGColPattern4.CellStyle = Me.igFtpCredCol3CellStyle
    IGColPattern4.ColHdrStyle = Me.igFtpCredCol3ColHdrStyle
    IGColPattern4.Text = "Port"
    IGColPattern4.Width = 36
    IGColPattern5.CellStyle = Me.igFtpCredCol4CellStyle
    IGColPattern5.ColHdrStyle = Me.igFtpCredCol4ColHdrStyle
    IGColPattern5.Text = "Protocol"
    IGColPattern5.Width = 57
    Me.igFtpCred.Cols.AddRange(New TenTec.Windows.iGridLib.iGColPattern() {IGColPattern1, IGColPattern2, IGColPattern3, IGColPattern4, IGColPattern5})
    Me.igFtpCred.Header.Height = 19
    Me.igFtpCred.Location = New System.Drawing.Point(18, 27)
    Me.igFtpCred.Name = "igFtpCred"
    Me.igFtpCred.Size = New System.Drawing.Size(352, 221)
    Me.igFtpCred.TabIndex = 29
    '
    'GroupBox1
    '
    Me.GroupBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.GroupBox1.Controls.Add(Me.btnSelPrivKey)
    Me.GroupBox1.Controls.Add(Me.btnAddFtpCred)
    Me.GroupBox1.Controls.Add(Me.btnDelFtpCred)
    Me.GroupBox1.Controls.Add(Me.btnAddSFtpCred)
    Me.GroupBox1.Controls.Add(Me.igFtpCred)
    Me.GroupBox1.Location = New System.Drawing.Point(0, 71)
    Me.GroupBox1.Name = "GroupBox1"
    Me.GroupBox1.Size = New System.Drawing.Size(383, 280)
    Me.GroupBox1.TabIndex = 32
    Me.GroupBox1.TabStop = False
    Me.GroupBox1.Text = "FTP-Anmeldedaten verwalten"
    '
    'GroupBox2
    '
    Me.GroupBox2.Controls.Add(Me.btnPreviewSound)
    Me.GroupBox2.Controls.Add(Me.cmbSaveSound)
    Me.GroupBox2.Location = New System.Drawing.Point(0, 6)
    Me.GroupBox2.Name = "GroupBox2"
    Me.GroupBox2.Size = New System.Drawing.Size(364, 54)
    Me.GroupBox2.TabIndex = 33
    Me.GroupBox2.TabStop = False
    Me.GroupBox2.Text = "Sound bei Datei speichern"
    '
    'btnPreviewSound
    '
    Me.btnPreviewSound.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnPreviewSound.Location = New System.Drawing.Point(278, 19)
    Me.btnPreviewSound.Name = "btnPreviewSound"
    Me.btnPreviewSound.Size = New System.Drawing.Size(73, 25)
    Me.btnPreviewSound.TabIndex = 31
    Me.btnPreviewSound.Text = "Vorschau"
    Me.btnPreviewSound.UseVisualStyleBackColor = True
    '
    'cmbSaveSound
    '
    Me.cmbSaveSound.FormattingEnabled = True
    Me.cmbSaveSound.Location = New System.Drawing.Point(18, 22)
    Me.cmbSaveSound.Name = "cmbSaveSound"
    Me.cmbSaveSound.Size = New System.Drawing.Size(251, 21)
    Me.cmbSaveSound.TabIndex = 0
    '
    'btnAddFtpCred
    '
    Me.btnAddFtpCred.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnAddFtpCred.Location = New System.Drawing.Point(139, 249)
    Me.btnAddFtpCred.Name = "btnAddFtpCred"
    Me.btnAddFtpCred.Size = New System.Drawing.Size(73, 25)
    Me.btnAddFtpCred.TabIndex = 32
    Me.btnAddFtpCred.Text = "Neu FTP"
    Me.btnAddFtpCred.UseVisualStyleBackColor = True
    '
    'btnSelPrivKey
    '
    Me.btnSelPrivKey.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
    Me.btnSelPrivKey.Location = New System.Drawing.Point(18, 249)
    Me.btnSelPrivKey.Name = "btnSelPrivKey"
    Me.btnSelPrivKey.Size = New System.Drawing.Size(83, 25)
    Me.btnSelPrivKey.TabIndex = 33
    Me.btnSelPrivKey.Text = "Private key..."
    Me.btnSelPrivKey.UseVisualStyleBackColor = True
    '
    'ctl_options
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.AutoScroll = True
    Me.Controls.Add(Me.GroupBox2)
    Me.Controls.Add(Me.GroupBox1)
    Me.Name = "ctl_options"
    Me.Size = New System.Drawing.Size(384, 358)
    CType(Me.igFtpCred, System.ComponentModel.ISupportInitialize).EndInit()
    Me.GroupBox1.ResumeLayout(False)
    Me.GroupBox2.ResumeLayout(False)
    Me.ResumeLayout(False)

  End Sub
  Friend WithEvents btnDelFtpCred As System.Windows.Forms.Button
  Friend WithEvents btnAddSFtpCred As System.Windows.Forms.Button
  Friend WithEvents igFtpCred As TenTec.Windows.iGridLib.iGrid
  Friend WithEvents igFtpCredCol3CellStyle As TenTec.Windows.iGridLib.iGCellStyle
  Friend WithEvents igFtpCredCol3ColHdrStyle As TenTec.Windows.iGridLib.iGColHdrStyle
  Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
  Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
  Friend WithEvents btnPreviewSound As System.Windows.Forms.Button
  Friend WithEvents cmbSaveSound As System.Windows.Forms.ComboBox
  Friend WithEvents igFtpCredCol4CellStyle As TenTec.Windows.iGridLib.iGCellStyle
  Friend WithEvents igFtpCredCol4ColHdrStyle As TenTec.Windows.iGridLib.iGColHdrStyle
  Friend WithEvents btnAddFtpCred As System.Windows.Forms.Button
  Friend WithEvents btnSelPrivKey As System.Windows.Forms.Button

End Class
