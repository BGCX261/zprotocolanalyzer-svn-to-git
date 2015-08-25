using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace zyc.AutoPilotTester
{
    public partial class DataBlock : UserControl
    {
        public List<DataWidget> Parts = new List<DataWidget>(6);
        int _Margin = 0;

        public DataBlock()
        {
            InitializeComponent();
        }

        public void AddDataWidget(DataWidget dw)
        {
            dw.Top = 0;
            Parts.Add(dw);
            this.Controls.Add(dw);
            ChangeLayout();
        }

        void ChangeLayout()
        {
            int p = lbName.Left + lbName.Width + _Margin;
            for (int i = 0; i < Parts.Count; i++)
            {
                Parts[i].Left = p;
                p += Parts[i].Width + _Margin;
            }
            this.Width = p;
        }

        public bool IsLock()
        {
            foreach (var dw in Parts)
            {
                if (dw.IsLocked)
                    return true;
            }
            return false;
        }

        public void SetPartsWidth(int Width)
        {
            foreach (var dw in Parts)
            {
                dw.Width = Width;
            }
            ChangeLayout();
        }

        public string Title
        {
            get
            {
                return lbName.Text;
            }
            set
            {
                lbName.Text = value;
                ChangeLayout();
            }
        }

        public string TitleTips
        {
            get
            {
                return toolTip1.GetToolTip(lbName);
            }
            set
            {
                toolTip1.SetToolTip(lbName, value);
            }
        }

        //public int Margin
        //{
        //    get
        //    {
        //        return _Margin;
        //    }
        //    set
        //    {
        //        _Margin = value;
        //        ChangeLayout();
        //    }
        //}
    }
}
