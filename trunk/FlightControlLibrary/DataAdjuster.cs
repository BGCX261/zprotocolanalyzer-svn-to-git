using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace zyc.AutoPilotTester
{
    public partial class DataAdjuster : UserControl, IDataControl
    {
        public DataAdjuster()
        {
            InitializeComponent();
        }

        public delegate void CallBackHandle();
        public event CallBackHandle FillPercentChangedByCode;
        public event CallBackHandle FillPercentChangedByUser;

        int _ID;
        int _Index;
        float _Data;

        #region IDataControl 成员

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public int Index
        {
            get { return _Index; }
            set { _Index = value; }
        }

        public float Data
        {
            get { return _Data; }
            set { _Data = value; }
        }

        #endregion

        double _FillPercent;
        public double FillPercent
        {
            get { return _FillPercent; }
            set
            {
                if ((value >= 0) && (value <= 1))
                {
                    _FillPercent = value;
                    if (FillPercentChangedByCode != null)
                        FillPercentChangedByCode();
                    this.Invalidate();
                }
                else
                {
                    throw new Exception("错误的输入数据,百分比的值必须在0和1之间.");
                }
            }
        }

        public enum FillModeEnum
        {
            Left, Right, Top, Bottom
        }

        FillModeEnum _FillMode;
        public FillModeEnum FillMode
        {
            get { return _FillMode; }
            set
            {
                _FillMode = value;
                this.Invalidate();
            }
        }

        string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                this.Invalidate();
            }
        }

        private void DataAdjuster_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                switch (_FillMode)
                {
                    case FillModeEnum.Left:
                        FillPercent = e.X / (double)this.Width;
                        break;
                    case FillModeEnum.Right:
                        FillPercent = 1 - e.X / (double)this.Width;
                        break;
                    case FillModeEnum.Top:
                        FillPercent = e.Y / (double)this.Height;
                        break;
                    case FillModeEnum.Bottom:
                        FillPercent = 1 - e.Y / (double)this.Height;
                        break;
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                FillPercent = 0.5;
            }
            if (FillPercentChangedByUser != null)
                FillPercentChangedByUser();
        }

        private void DataAdjuster_Paint(object sender, PaintEventArgs e)
        {
            switch (_FillMode)
            {
                case FillModeEnum.Left:
                    e.Graphics.FillRectangle(Brushes.YellowGreen, 0, 0, (float)(this.Width * _FillPercent), this.Height);
                    break;
                case FillModeEnum.Right:
                    e.Graphics.FillRectangle(Brushes.YellowGreen, (float)(this.Width * (1 - _FillPercent)), 0, this.Width, this.Height);
                    break;
                case FillModeEnum.Top:
                    e.Graphics.FillRectangle(Brushes.YellowGreen, 0, 0, this.Width, (float)(this.Height * _FillPercent));
                    break;
                case FillModeEnum.Bottom:
                    e.Graphics.FillRectangle(Brushes.YellowGreen, 0, (float)(this.Height * (1 - _FillPercent)), this.Width, this.Height);
                    break;
            }
            e.Graphics.DrawRectangle(Pens.Black, 0, 0, this.Width - 1, this.Height - 1);
        }
    }
}
