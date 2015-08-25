using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace zyc.AutoPilotTester
{
    public partial class DataEdit : UserControl
    {
        public DataEdit()
        {
            InitializeComponent();
        }

        public string DataType
        {
            get
            {
                return cbType.Text;
            }
            set
            {
                cbType.Text = value;
            }
        }

        public string DataName
        {
            get
            {
                return tbName.Text;
            }
            set
            {
                tbName.Text = value;
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
                tbData.Text = value;
            }
        }

    }
}
