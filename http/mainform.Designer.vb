<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class mainform
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
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

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意:  以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(mainform))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.staffnamecb = New System.Windows.Forms.ComboBox()
        Me.MPLable = New System.Windows.Forms.Label()
        Me.copyroomstatusb = New System.Windows.Forms.Button()
        Me.copyendorseb = New System.Windows.Forms.Button()
        Me.gctb = New System.Windows.Forms.TextBox()
        Me.roomdg = New System.Windows.Forms.DataGridView()
        Me.madedtp = New System.Windows.Forms.DateTimePicker()
        Me.WebTimer = New System.Windows.Forms.Timer(Me.components)
        Me.OLL = New System.Windows.Forms.Label()
        Me.OrderLB = New System.Windows.Forms.ListBox()
        Me.markb = New System.Windows.Forms.Button()
        Me.MMenu = New System.Windows.Forms.MenuStrip()
        Me.文件FToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.导入IToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.导出OToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.退出EToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.订单OToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.获取指定订单OToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.查找订单FToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.刷新列表RToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.复制房态ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.复制批注ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.删除订单DToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.邮件EToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.复制邮件模板ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.工具TToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.更新配置ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.设置SToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.语言LToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.帮助HToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.关于AToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.使用说明IToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.markallb = New System.Windows.Forms.Button()
        Me.oldetailtb = New System.Windows.Forms.TextBox()
        Me.htmlFileSystemWatcher = New System.IO.FileSystemWatcher()
        Me.detailtb = New System.Windows.Forms.TextBox()
        Me.dtb = New System.Windows.Forms.TextBox()
        Me.copydetailsb = New System.Windows.Forms.Button()
        CType(Me.roomdg, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MMenu.SuspendLayout()
        CType(Me.htmlFileSystemWatcher, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'staffnamecb
        '
        Me.staffnamecb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.staffnamecb.FormattingEnabled = True
        resources.ApplyResources(Me.staffnamecb, "staffnamecb")
        Me.staffnamecb.Name = "staffnamecb"
        Me.staffnamecb.Sorted = True
        '
        'MPLable
        '
        Me.MPLable.BackColor = System.Drawing.Color.White
        Me.MPLable.ForeColor = System.Drawing.Color.DarkSlateBlue
        resources.ApplyResources(Me.MPLable, "MPLable")
        Me.MPLable.Name = "MPLable"
        '
        'copyroomstatusb
        '
        resources.ApplyResources(Me.copyroomstatusb, "copyroomstatusb")
        Me.copyroomstatusb.Name = "copyroomstatusb"
        Me.copyroomstatusb.UseVisualStyleBackColor = True
        '
        'copyendorseb
        '
        resources.ApplyResources(Me.copyendorseb, "copyendorseb")
        Me.copyendorseb.Name = "copyendorseb"
        Me.copyendorseb.UseVisualStyleBackColor = True
        '
        'gctb
        '
        resources.ApplyResources(Me.gctb, "gctb")
        Me.gctb.Name = "gctb"
        '
        'roomdg
        '
        Me.roomdg.AllowUserToAddRows = False
        Me.roomdg.BackgroundColor = System.Drawing.Color.White
        resources.ApplyResources(Me.roomdg, "roomdg")
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle1.Font = New System.Drawing.Font("宋体", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.DarkRed
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.roomdg.DefaultCellStyle = DataGridViewCellStyle1
        Me.roomdg.Name = "roomdg"
        Me.roomdg.RowHeadersVisible = False
        Me.roomdg.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.roomdg.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        Me.roomdg.RowTemplate.Height = 23
        Me.roomdg.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        '
        'madedtp
        '
        resources.ApplyResources(Me.madedtp, "madedtp")
        Me.madedtp.Name = "madedtp"
        '
        'WebTimer
        '
        '
        'OLL
        '
        resources.ApplyResources(Me.OLL, "OLL")
        Me.OLL.Name = "OLL"
        '
        'OrderLB
        '
        Me.OrderLB.FormattingEnabled = True
        resources.ApplyResources(Me.OrderLB, "OrderLB")
        Me.OrderLB.Name = "OrderLB"
        '
        'markb
        '
        resources.ApplyResources(Me.markb, "markb")
        Me.markb.Name = "markb"
        Me.markb.UseVisualStyleBackColor = True
        '
        'MMenu
        '
        Me.MMenu.BackColor = System.Drawing.Color.White
        Me.MMenu.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.MMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.文件FToolStripMenuItem, Me.订单OToolStripMenuItem, Me.邮件EToolStripMenuItem, Me.工具TToolStripMenuItem, Me.帮助HToolStripMenuItem})
        resources.ApplyResources(Me.MMenu, "MMenu")
        Me.MMenu.Name = "MMenu"
        '
        '文件FToolStripMenuItem
        '
        Me.文件FToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.导入IToolStripMenuItem, Me.导出OToolStripMenuItem, Me.退出EToolStripMenuItem})
        Me.文件FToolStripMenuItem.Name = "文件FToolStripMenuItem"
        resources.ApplyResources(Me.文件FToolStripMenuItem, "文件FToolStripMenuItem")
        '
        '导入IToolStripMenuItem
        '
        resources.ApplyResources(Me.导入IToolStripMenuItem, "导入IToolStripMenuItem")
        Me.导入IToolStripMenuItem.Name = "导入IToolStripMenuItem"
        '
        '导出OToolStripMenuItem
        '
        resources.ApplyResources(Me.导出OToolStripMenuItem, "导出OToolStripMenuItem")
        Me.导出OToolStripMenuItem.Name = "导出OToolStripMenuItem"
        '
        '退出EToolStripMenuItem
        '
        Me.退出EToolStripMenuItem.Name = "退出EToolStripMenuItem"
        resources.ApplyResources(Me.退出EToolStripMenuItem, "退出EToolStripMenuItem")
        '
        '订单OToolStripMenuItem
        '
        Me.订单OToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.获取指定订单OToolStripMenuItem, Me.查找订单FToolStripMenuItem, Me.刷新列表RToolStripMenuItem, Me.复制房态ToolStripMenuItem, Me.复制批注ToolStripMenuItem, Me.删除订单DToolStripMenuItem})
        Me.订单OToolStripMenuItem.Name = "订单OToolStripMenuItem"
        resources.ApplyResources(Me.订单OToolStripMenuItem, "订单OToolStripMenuItem")
        '
        '获取指定订单OToolStripMenuItem
        '
        Me.获取指定订单OToolStripMenuItem.Name = "获取指定订单OToolStripMenuItem"
        resources.ApplyResources(Me.获取指定订单OToolStripMenuItem, "获取指定订单OToolStripMenuItem")
        '
        '查找订单FToolStripMenuItem
        '
        Me.查找订单FToolStripMenuItem.Name = "查找订单FToolStripMenuItem"
        resources.ApplyResources(Me.查找订单FToolStripMenuItem, "查找订单FToolStripMenuItem")
        '
        '刷新列表RToolStripMenuItem
        '
        Me.刷新列表RToolStripMenuItem.Name = "刷新列表RToolStripMenuItem"
        resources.ApplyResources(Me.刷新列表RToolStripMenuItem, "刷新列表RToolStripMenuItem")
        '
        '复制房态ToolStripMenuItem
        '
        Me.复制房态ToolStripMenuItem.Name = "复制房态ToolStripMenuItem"
        resources.ApplyResources(Me.复制房态ToolStripMenuItem, "复制房态ToolStripMenuItem")
        '
        '复制批注ToolStripMenuItem
        '
        Me.复制批注ToolStripMenuItem.Name = "复制批注ToolStripMenuItem"
        resources.ApplyResources(Me.复制批注ToolStripMenuItem, "复制批注ToolStripMenuItem")
        '
        '删除订单DToolStripMenuItem
        '
        Me.删除订单DToolStripMenuItem.Name = "删除订单DToolStripMenuItem"
        resources.ApplyResources(Me.删除订单DToolStripMenuItem, "删除订单DToolStripMenuItem")
        '
        '邮件EToolStripMenuItem
        '
        Me.邮件EToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.复制邮件模板ToolStripMenuItem})
        Me.邮件EToolStripMenuItem.Name = "邮件EToolStripMenuItem"
        resources.ApplyResources(Me.邮件EToolStripMenuItem, "邮件EToolStripMenuItem")
        '
        '复制邮件模板ToolStripMenuItem
        '
        Me.复制邮件模板ToolStripMenuItem.Name = "复制邮件模板ToolStripMenuItem"
        resources.ApplyResources(Me.复制邮件模板ToolStripMenuItem, "复制邮件模板ToolStripMenuItem")
        '
        '工具TToolStripMenuItem
        '
        Me.工具TToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.更新配置ToolStripMenuItem, Me.设置SToolStripMenuItem, Me.语言LToolStripMenuItem})
        Me.工具TToolStripMenuItem.Name = "工具TToolStripMenuItem"
        resources.ApplyResources(Me.工具TToolStripMenuItem, "工具TToolStripMenuItem")
        '
        '更新配置ToolStripMenuItem
        '
        Me.更新配置ToolStripMenuItem.Name = "更新配置ToolStripMenuItem"
        resources.ApplyResources(Me.更新配置ToolStripMenuItem, "更新配置ToolStripMenuItem")
        '
        '设置SToolStripMenuItem
        '
        Me.设置SToolStripMenuItem.Name = "设置SToolStripMenuItem"
        resources.ApplyResources(Me.设置SToolStripMenuItem, "设置SToolStripMenuItem")
        '
        '语言LToolStripMenuItem
        '
        resources.ApplyResources(Me.语言LToolStripMenuItem, "语言LToolStripMenuItem")
        Me.语言LToolStripMenuItem.Name = "语言LToolStripMenuItem"
        '
        '帮助HToolStripMenuItem
        '
        Me.帮助HToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.关于AToolStripMenuItem, Me.使用说明IToolStripMenuItem})
        Me.帮助HToolStripMenuItem.Name = "帮助HToolStripMenuItem"
        resources.ApplyResources(Me.帮助HToolStripMenuItem, "帮助HToolStripMenuItem")
        '
        '关于AToolStripMenuItem
        '
        Me.关于AToolStripMenuItem.Name = "关于AToolStripMenuItem"
        resources.ApplyResources(Me.关于AToolStripMenuItem, "关于AToolStripMenuItem")
        '
        '使用说明IToolStripMenuItem
        '
        resources.ApplyResources(Me.使用说明IToolStripMenuItem, "使用说明IToolStripMenuItem")
        Me.使用说明IToolStripMenuItem.Name = "使用说明IToolStripMenuItem"
        '
        'markallb
        '
        resources.ApplyResources(Me.markallb, "markallb")
        Me.markallb.Name = "markallb"
        Me.markallb.UseVisualStyleBackColor = True
        '
        'oldetailtb
        '
        Me.oldetailtb.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me.oldetailtb, "oldetailtb")
        Me.oldetailtb.Name = "oldetailtb"
        Me.oldetailtb.ReadOnly = True
        '
        'htmlFileSystemWatcher
        '
        Me.htmlFileSystemWatcher.EnableRaisingEvents = True
        Me.htmlFileSystemWatcher.Filter = "*.htm"
        Me.htmlFileSystemWatcher.SynchronizingObject = Me
        '
        'detailtb
        '
        Me.detailtb.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me.detailtb, "detailtb")
        Me.detailtb.Name = "detailtb"
        Me.detailtb.ReadOnly = True
        '
        'dtb
        '
        Me.dtb.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me.dtb, "dtb")
        Me.dtb.Name = "dtb"
        Me.dtb.ReadOnly = True
        '
        'copydetailsb
        '
        resources.ApplyResources(Me.copydetailsb, "copydetailsb")
        Me.copydetailsb.Name = "copydetailsb"
        Me.copydetailsb.UseVisualStyleBackColor = True
        '
        'mainform
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.Controls.Add(Me.copydetailsb)
        Me.Controls.Add(Me.dtb)
        Me.Controls.Add(Me.detailtb)
        Me.Controls.Add(Me.oldetailtb)
        Me.Controls.Add(Me.markallb)
        Me.Controls.Add(Me.MMenu)
        Me.Controls.Add(Me.markb)
        Me.Controls.Add(Me.OrderLB)
        Me.Controls.Add(Me.OLL)
        Me.Controls.Add(Me.madedtp)
        Me.Controls.Add(Me.roomdg)
        Me.Controls.Add(Me.gctb)
        Me.Controls.Add(Me.staffnamecb)
        Me.Controls.Add(Me.copyendorseb)
        Me.Controls.Add(Me.copyroomstatusb)
        Me.Controls.Add(Me.MPLable)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.Name = "mainform"
        CType(Me.roomdg, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MMenu.ResumeLayout(False)
        Me.MMenu.PerformLayout()
        CType(Me.htmlFileSystemWatcher, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents staffnamecb As System.Windows.Forms.ComboBox
    Friend WithEvents MPLable As System.Windows.Forms.Label
    Friend WithEvents copyroomstatusb As System.Windows.Forms.Button
    Friend WithEvents copyendorseb As System.Windows.Forms.Button
    Friend WithEvents gctb As System.Windows.Forms.TextBox
    Friend WithEvents roomdg As System.Windows.Forms.DataGridView
    Friend WithEvents madedtp As System.Windows.Forms.DateTimePicker
    Friend WithEvents WebTimer As Timer
    Friend WithEvents OLL As Label
    Friend WithEvents markb As Button
    Friend WithEvents MMenu As System.Windows.Forms.MenuStrip
    Friend WithEvents 文件FToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents 订单OToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents 工具TToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents 设置SToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents 帮助HToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents 关于AToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents 更新配置ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents 获取指定订单OToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OrderLB As System.Windows.Forms.ListBox
    Friend WithEvents 邮件EToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents 复制邮件模板ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents 使用说明IToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents 语言LToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents 查找订单FToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents 复制房态ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents 复制批注ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents 导入IToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents 导出OToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents 退出EToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents markallb As Button
    Friend WithEvents 删除订单DToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents oldetailtb As TextBox
    Friend WithEvents 刷新列表RToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents htmlFileSystemWatcher As FileSystemWatcher
    Friend WithEvents detailtb As TextBox
    Friend WithEvents dtb As TextBox
    Friend WithEvents copydetailsb As Button
End Class
