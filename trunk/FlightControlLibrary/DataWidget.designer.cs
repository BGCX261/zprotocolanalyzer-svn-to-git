namespace zyc.AutoPilotTester
{
    partial class DataWidget
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lbName = new System.Windows.Forms.Label();
            this.tbData = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Location = new System.Drawing.Point(6, 7);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(29, 12);
            this.lbName.TabIndex = 0;
            this.lbName.Text = "Name";
            // 
            // tbData
            // 
            this.tbData.Location = new System.Drawing.Point(41, 3);
            this.tbData.Name = "tbData";
            this.tbData.Size = new System.Drawing.Size(100, 21);
            this.tbData.TabIndex = 1;
            this.tbData.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbData_KeyDown);
            this.tbData.Leave += new System.EventHandler(this.tbData_Leave);
            this.tbData.Enter += new System.EventHandler(this.tbData_Enter);
            // 
            // DataWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbData);
            this.Controls.Add(this.lbName);
            this.Name = "DataWidget";
            this.Size = new System.Drawing.Size(146, 27);
            this.SizeChanged += new System.EventHandler(this.DataWidget_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.TextBox tbData;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
