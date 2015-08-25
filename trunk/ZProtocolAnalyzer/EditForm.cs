using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using zyc.AutoPilotTester;

namespace zyc.ZProtocolAnalyzer
{
    public partial class EditForm : Form
    {
        public event EventHandler OnConvertFinish;

        List<DataEdit> list = new List<DataEdit>(12);

        public DataString dataString;
        public ZBlockDescribe zBlockDescribe;

        public EditForm()
        {
            InitializeComponent();
            AddOne();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddOne();
        }

        private void AddOne()
        {
            DataEdit de = new DataEdit();
            list.Add(de);
            flowLayoutPanel1.Controls.Add(de);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (list.Count > 0)
            {
                list.RemoveAt(list.Count - 1);
                flowLayoutPanel1.Controls.RemoveAt(flowLayoutPanel1.Controls.Count - 1);
            }
        }

        private void Convert()
        {
            dataString = new DataString();
            dataString.BlockWord = byte.Parse(dataWidget1.DataString);

            zBlockDescribe = new ZBlockDescribe();
            zBlockDescribe.BlockName = dataWidget3.DataString;
            zBlockDescribe.BlockWord = byte.Parse(dataWidget1.DataString);

            if (dataWidget2.DataString == "")
            {
                dataString.Data = new string[list.Count];
                dataString.Value = new float[list.Count];


                zBlockDescribe.ZParts = new ZPartDescribe[list.Count];
                //zBlockDescribe.ZPartNum = list.Count;

                for (int i = 0; i < list.Count; i++)
                {
                    zBlockDescribe.ZParts[i].Name = list[i].DataName;
                    zBlockDescribe.ZParts[i].DataType = ConvertType(list[i].DataType);
                    zBlockDescribe.ZParts[i].Index = (byte)i;
                    dataString.Data[i] = list[i].DataString;
                }
            }
            else
            {
                dataString.Data = new string[list.Count + 1];
                dataString.Value = new float[list.Count + 1];

                zBlockDescribe.ZParts = new ZPartDescribe[list.Count + 1];
                //zBlockDescribe.ZPartNum = list.Count + 1;

                dataString.Data[0] = dataWidget2.DataString;

                zBlockDescribe.ZParts[0].DataType = DataTypeEnum.Reserve;
                zBlockDescribe.ZParts[0].Index = 0;
                zBlockDescribe.ZParts[0].Name = "保留位";
                zBlockDescribe.ZParts[0].BlockWord = zBlockDescribe.BlockWord;

                for (int i = 1; i < list.Count + 1; i++)
                {
                    zBlockDescribe.ZParts[i].BlockWord = zBlockDescribe.BlockWord;
                    zBlockDescribe.ZParts[i].Name = list[i-1].DataName;
                    zBlockDescribe.ZParts[i].DataType = ConvertType(list[i-1].DataType);
                    zBlockDescribe.ZParts[i].Index = (byte)i;
                    dataString.Data[i] = list[i-1].DataString;
                }
            }
        }

        private DataTypeEnum ConvertType(string p)
        {
            switch (p)
            {
                case "U8":
                    return DataTypeEnum.U8;
                case "U16":
                    return DataTypeEnum.U16;
                case "U32":
                    return DataTypeEnum.U32;
                case "S16":
                    return DataTypeEnum.S16;
                case "S32":
                    return DataTypeEnum.S32;
                case "F32":
                    return DataTypeEnum.F32;
                default:
                    return DataTypeEnum.U8;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Convert();
                this.Close();
                if (OnConvertFinish != null)
                {
                    OnConvertFinish(this, null);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
