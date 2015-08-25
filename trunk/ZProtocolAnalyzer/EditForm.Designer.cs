namespace zyc.ZProtocolAnalyzer
{
    partial class EditForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.dataWidget3 = new zyc.AutoPilotTester.DataWidget();
            this.dataWidget2 = new zyc.AutoPilotTester.DataWidget();
            this.dataWidget1 = new zyc.AutoPilotTester.DataWidget();
            this.button4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(211, 455);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "完成";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 455);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(25, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "+";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(43, 455);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(25, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "-";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 45);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(402, 402);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // dataWidget3
            // 
            this.dataWidget3.Data = 0F;
            this.dataWidget3.DataString = "";
            this.dataWidget3.ID = 0;
            this.dataWidget3.Index = 0;
            this.dataWidget3.Location = new System.Drawing.Point(74, 453);
            this.dataWidget3.Name = "dataWidget3";
            this.dataWidget3.ReadOnly = false;
            this.dataWidget3.Size = new System.Drawing.Size(131, 27);
            this.dataWidget3.TabIndex = 5;
            this.dataWidget3.TextTips = "";
            this.dataWidget3.Title = "名称";
            this.dataWidget3.TitleTips = "";
            // 
            // dataWidget2
            // 
            this.dataWidget2.Data = 0F;
            this.dataWidget2.DataString = "";
            this.dataWidget2.ID = 0;
            this.dataWidget2.Index = 0;
            this.dataWidget2.Location = new System.Drawing.Point(158, 12);
            this.dataWidget2.Name = "dataWidget2";
            this.dataWidget2.ReadOnly = false;
            this.dataWidget2.Size = new System.Drawing.Size(140, 27);
            this.dataWidget2.TabIndex = 1;
            this.dataWidget2.TextTips = "";
            this.dataWidget2.Title = "保留位";
            this.dataWidget2.TitleTips = "";
            // 
            // dataWidget1
            // 
            this.dataWidget1.Data = 0F;
            this.dataWidget1.DataString = "";
            this.dataWidget1.ID = 0;
            this.dataWidget1.Index = 0;
            this.dataWidget1.Location = new System.Drawing.Point(12, 12);
            this.dataWidget1.Name = "dataWidget1";
            this.dataWidget1.ReadOnly = false;
            this.dataWidget1.Size = new System.Drawing.Size(140, 27);
            this.dataWidget1.TabIndex = 0;
            this.dataWidget1.TextTips = "";
            this.dataWidget1.Title = "命令字";
            this.dataWidget1.TitleTips = "";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(304, 455);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(87, 23);
            this.button4.TabIndex = 7;
            this.button4.Text = "取消";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // EditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 490);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.dataWidget3);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataWidget2);
            this.Controls.Add(this.dataWidget1);
            this.Name = "EditForm";
            this.Text = "EditForm";
            this.ResumeLayout(false);

        }

        #endregion

        private zyc.AutoPilotTester.DataWidget dataWidget1;
        private zyc.AutoPilotTester.DataWidget dataWidget2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private zyc.AutoPilotTester.DataWidget dataWidget3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button button4;

    }
}