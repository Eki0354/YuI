namespace BetaUpdateApp
{
    partial class UMForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cB_config = new System.Windows.Forms.CheckBox();
            this.cB_db = new System.Windows.Forms.CheckBox();
            this.cB_app = new System.Windows.Forms.CheckBox();
            this.button_update = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.cB_udApp = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cB_udApp);
            this.groupBox1.Controls.Add(this.cB_config);
            this.groupBox1.Controls.Add(this.cB_db);
            this.groupBox1.Controls.Add(this.cB_app);
            this.groupBox1.Location = new System.Drawing.Point(13, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(353, 197);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "更新选项";
            // 
            // cB_config
            // 
            this.cB_config.AutoSize = true;
            this.cB_config.Location = new System.Drawing.Point(32, 158);
            this.cB_config.Name = "cB_config";
            this.cB_config.Size = new System.Drawing.Size(106, 22);
            this.cB_config.TabIndex = 2;
            this.cB_config.Text = "配置文件";
            this.cB_config.UseVisualStyleBackColor = true;
            // 
            // cB_db
            // 
            this.cB_db.AutoSize = true;
            this.cB_db.Location = new System.Drawing.Point(32, 116);
            this.cB_db.Name = "cB_db";
            this.cB_db.Size = new System.Drawing.Size(124, 22);
            this.cB_db.TabIndex = 1;
            this.cB_db.Text = "数据库文件";
            this.cB_db.UseVisualStyleBackColor = true;
            // 
            // cB_app
            // 
            this.cB_app.AutoSize = true;
            this.cB_app.Checked = true;
            this.cB_app.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cB_app.Location = new System.Drawing.Point(32, 79);
            this.cB_app.Name = "cB_app";
            this.cB_app.Size = new System.Drawing.Size(106, 22);
            this.cB_app.TabIndex = 0;
            this.cB_app.Text = "程序主体";
            this.cB_app.UseVisualStyleBackColor = true;
            // 
            // button_update
            // 
            this.button_update.Location = new System.Drawing.Point(60, 235);
            this.button_update.Name = "button_update";
            this.button_update.Size = new System.Drawing.Size(91, 32);
            this.button_update.TabIndex = 1;
            this.button_update.Text = "更新";
            this.button_update.UseVisualStyleBackColor = true;
            this.button_update.Click += new System.EventHandler(this.button_update_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_cancel.Location = new System.Drawing.Point(228, 235);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(91, 32);
            this.button_cancel.TabIndex = 2;
            this.button_cancel.Text = "取消";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // cB_udApp
            // 
            this.cB_udApp.AutoSize = true;
            this.cB_udApp.Checked = true;
            this.cB_udApp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cB_udApp.Location = new System.Drawing.Point(32, 38);
            this.cB_udApp.Name = "cB_udApp";
            this.cB_udApp.Size = new System.Drawing.Size(106, 22);
            this.cB_udApp.TabIndex = 3;
            this.cB_udApp.Text = "更新程序";
            this.cB_udApp.UseVisualStyleBackColor = true;
            // 
            // UMForm
            // 
            this.AcceptButton = this.button_update;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_cancel;
            this.ClientSize = new System.Drawing.Size(378, 279);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_update);
            this.Controls.Add(this.groupBox1);
            this.Name = "UMForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UMForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.UMForm_FormClosed);
            this.Load += new System.EventHandler(this.UMForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cB_config;
        private System.Windows.Forms.CheckBox cB_db;
        private System.Windows.Forms.CheckBox cB_app;
        private System.Windows.Forms.Button button_update;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.CheckBox cB_udApp;
    }
}