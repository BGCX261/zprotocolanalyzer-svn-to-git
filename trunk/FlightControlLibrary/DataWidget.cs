using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace zyc.AutoPilotTester
{
    public partial class DataWidget : UserControl, IDataControl
    {
        public DataWidget()
        {
            InitializeComponent();
        }

        void ChangeLayout()
        {
            lbName.Top = (this.Height - lbName.Height) / 2;
            tbData.Top = (this.Height - tbData.Height) / 2;

            lbName.Left = 6;
            tbData.Left = lbName.Left + lbName.Width + 6;
            int w = this.Width - tbData.Left - 6;
            if (w >= 0)
            {
                tbData.Width = w;
            }
            else
            {
                tbData.Width = 0;
            }
        }

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
            get
            {
                float r = 0;
                if (float.TryParse(tbData.Text, out r))
                {
                    _Data = r;
                }
                else
                {
                    //tbData.BackColor = Color.Red;
                }
                return _Data;
            }
            set
            {
                if ((_IsLooked == false) && (_IsPaused == false))
                {
                    _Data = value;
                    tbData.Text = value.ToString();
                }
            }
        }

        #endregion

        bool _IsLooked, _IsPaused;
        public bool IsPaused
        {
            get { return _IsPaused; }
        }

        public bool IsLocked
        {
            get { return _IsLooked; }
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

        public string DataString
        {
            get
            {
                return tbData.Text;
            }
            set
            {
                if ((_IsLooked == false) && (_IsPaused == false))
                {
                    tbData.Text = value;
                }
            }
        }

        public bool ReadOnly
        {
            get
            {
                return tbData.ReadOnly;
            }
            set
            {
                tbData.ReadOnly = value;
            }
        }

        private void DataWidget_SizeChanged(object sender, EventArgs e)
        {
            ChangeLayout();
        }

        private void tbData_Enter(object sender, EventArgs e)
        {
            _IsPaused = true;
        }

        private void tbData_Leave(object sender, EventArgs e)
        {
            _IsPaused = false;
        }

        private void tbData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                Unlock();
            }
            if ((e.KeyCode == Keys.Back) || (e.KeyCode == Keys.Delete) || ((e.KeyCode >= Keys.D0) && (e.KeyCode <= Keys.D9)) || ((e.KeyCode >= Keys.A) && (e.KeyCode <= Keys.Z)))
            {
                Lock();
            }
        }

        public void Unlock()
        {
            _IsLooked = false;
            tbData.BackColor = Color.White;
        }

        private void Lock()
        {
            _IsLooked = true;
            tbData.BackColor = Color.Blue;
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

        public string TextTips
        {
            get
            {
                return toolTip1.GetToolTip(tbData);
            }
            set
            {
                toolTip1.SetToolTip(tbData, value);
            }
        }
    }
}
