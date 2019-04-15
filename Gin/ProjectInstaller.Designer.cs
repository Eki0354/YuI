namespace Gin
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.GinProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.GinInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // GinProcessInstaller
            // 
            this.GinProcessInstaller.Password = null;
            this.GinProcessInstaller.Username = null;
            // 
            // GinInstaller
            // 
            this.GinInstaller.Description = "YuIの背中を見守らせて";
            this.GinInstaller.DisplayName = "Gin";
            this.GinInstaller.ServiceName = "Gin";
            this.GinInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.GinProcessInstaller,
            this.GinInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller GinProcessInstaller;
        private System.ServiceProcess.ServiceInstaller GinInstaller;
    }
}