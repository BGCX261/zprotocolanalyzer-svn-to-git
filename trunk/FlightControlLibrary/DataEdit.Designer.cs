namespace zyc.AutoPilotTester
{
    partial class DataEdit
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
            this.label1 = new System.Windows.Forms.Label();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.tbData = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "数据类型/名称/数值";
            // 
            // cbType
            // 
            this.cbType.Items.AddRange(new object[] {
            "U8",
            "U16",
            "U32",
            "S16",
            "S32",
            "F32"});
            this.cbType.Location = new System.Drawing.Point(126, 4);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(56, 20);
            this.cbType.TabIndex = 1;
            this.cbType.Text = "U8";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(188, 4);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(80, 21);
            this.tbName.TabIndex = 2;
            // 
            // tbData
            // 
            this.tbData.Location = new System.Drawing.Point(274, 4);
            this.tbData.Name = "tbData";
            this.tbData.Size = new System.Drawing.Size(80, 21);
            this.tbData.TabIndex = 3;
            // 
            // DataEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbData);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.cbType);
            this.Controls.Add(this.label1);
            this.Name = "DataEdit";
            this.Size = new System.Drawing.Size(360, 27);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbData;
    }
}
