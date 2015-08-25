using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using zyc.AutoPilotTester;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace zyc.ZProtocolAnalyzer
{
    public partial class MainForm : Form
    {
        string[] InitInfo = { "", "", "", "" };
        bool IsRuning;
        StringBuilder SB = new StringBuilder(1000);

        RS232 RS232Obj;
        File FileObj;
        SerialIOSwitch SerialIOSwitchObj;
        ZDescribeProcess ZDescribeProcessObj;
        ZProtocolProcess ZProtocolProcessObj;

        Dictionary<byte, DataBlock> Blocks;
        Dictionary<string, ZPackage> Packages;

        //wcl_start
        ZDescribeProcess ZDescribeProcessObjMin;
        Dictionary<byte, DataBlock> BlocksMin;
        //wcl_end

        #region 初始化

        public MainForm()
        {
            InitializeComponent();
            FlowPlan.MouseWheel += new MouseEventHandler(flowLayoutPanel1_MouseWheel);

            ZDescribeProcessObj = new ZDescribeProcess();
            ZDescribeProcessObj.LoadDescribesFile("ProtocolDescribe.txt");
            WriteLine("Loaded " + ZDescribeProcessObj.ZPackageDescribeNum + " Protocol Describe Info");

            Blocks = new Dictionary<byte, DataBlock>(ZDescribeProcessObj.ZPackageDescribeNum);
            foreach (KeyValuePair<byte, ZBlockDescribe> z in ZDescribeProcessObj)
            {
                DataBlock dataBlock = Converter.CreateDataBlock(z.Value);
                Blocks.Add(z.Key, dataBlock);
                FlowPlan.Controls.Add(dataBlock);
                dataBlock.MouseClick += new MouseEventHandler(dataBlock_MouseClick);
                dataBlock.MouseDoubleClick += new MouseEventHandler(dataBlock_MouseDoubleClick);
            }

            //wcl_start
            FlowPlanMin.MouseWheel += new MouseEventHandler(flowLayoutPanel1_MouseWheelMin);
            ZDescribeProcessObjMin = new ZDescribeProcess();
            ZDescribeProcessObjMin.LoadDescribesFile("ProtocolDescribeMin.txt");
            WriteLine("Loaded " + ZDescribeProcessObjMin.ZPackageDescribeNum + " Protocol Describe Info");

            BlocksMin = new Dictionary<byte, DataBlock>(ZDescribeProcessObjMin.ZPackageDescribeNum);
            foreach (KeyValuePair<byte, ZBlockDescribe> zMin in ZDescribeProcessObjMin)
            {
                DataBlock dataBlockMin = Converter.CreateDataBlock(zMin.Value);
                BlocksMin.Add(zMin.Key, dataBlockMin);
                FlowPlanMin.Controls.Add(dataBlockMin);
            }
            //wcl_end

            InitPackages();
            WriteLine("Loaded " + Packages.Count + " Protocol Data");
            UpdatePackagelist();

            ToStop();
        }

        void dataBlock_MouseClick(object sender, MouseEventArgs e)
        {
            DataBlock dataBlock = (DataBlock)sender;
            //dataBlock.Parts[0].ID
            ZBlock block = new ZBlock();
            block.Word = 0xF0;
            block.Data[0] = (byte)dataBlock.Parts[0].ID;
            ZProtocolProcessObj.AddBlock(block);
            if (ZProtocolProcessObj.PackageNumWaitToSend == ZProtocolProcess.BlockNumMax)
            {
                ZProtocolProcessObj.Send();
            }
        }

        void dataBlock_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DataBlock dataBlock = (DataBlock)sender;
            //dataBlock.Parts[0].ID
            ZBlock block = new ZBlock();
            block.Word = 0xF0;
            block.Data[0] = (byte)dataBlock.Parts[0].ID;
            ZProtocolProcessObj.AddBlock(block);
            if (ZProtocolProcessObj.PackageNumWaitToSend == ZProtocolProcess.BlockNumMax)
            {
                ZProtocolProcessObj.Send();
            }
        }

        private void InitPackages()
        {
            Stream sr = null;
            try
            {
                sr = System.IO.File.Open("Package.bin", FileMode.OpenOrCreate);
                BinaryFormatter bf = new BinaryFormatter();
                Packages = (Dictionary<string, ZPackage>)bf.Deserialize(sr);
                sr.Close();
            }
            catch
            {
                Packages = new Dictionary<string, ZPackage>(20);
                if (sr != null)
                {
                    sr.Close();
                }
            }
        }

        void ReInit()
        {
            if (SerialIOSwitchObj != null)
            {
                SerialIOSwitchObj.Close();
            }

            FileObj = new File(InitInfo[2], InitInfo[3]);

            if (InitInfo[0] != "None")
            {
                RS232Obj = new RS232(InitInfo[0], int.Parse(InitInfo[1]));
            }
            else
            {
                RS232Obj = null;
            }

            SerialIOSwitchObj = new SerialIOSwitch(RS232Obj, FileObj);

            SerialIOSwitchObj.Open();

            ZProtocolProcessObj = new ZProtocolProcess(SerialIOSwitchObj);
            ZProtocolProcessObj.OnReceviceFrame += new ZProtocolProcess.ReceviceData(ZProtocolProcessObj_OnReceviceFrame);
            ZProtocolProcessObj.OnReceviceBlock += new ZProtocolProcess.ReceviceData(ZProtocolProcessObj_OnReceviceBlock);
        }

        private void btInit_Click(object sender, EventArgs e)
        {
            if (IsRuning == false)
            {
                if (!(cbCOM.Text == InitInfo[0] && cbBPS.Text == InitInfo[1] && tbIn.Text == InitInfo[2] && tbOut.Text == InitInfo[3]))
                {
                    InitInfo[0] = cbCOM.Text;
                    InitInfo[1] = cbBPS.Text;
                    InitInfo[2] = tbIn.Text;
                    InitInfo[3] = tbOut.Text;
                    try
                    {
                        ReInit();
                    }
                    catch (IOException ee)
                    {
                        MessageBox.Show("指定端口不存在");
                        return;
                    }
                }
                ZProtocolProcessObj.Start();
                ToRun();
            }
            else
            {
                ZProtocolProcessObj.Stop();
                if (SerialIOSwitchObj != null)
                {
                    SerialIOSwitchObj.Close();
                }
                ToStop();
            }
        }

        void ToRun()
        {
            cbRecord.Enabled = true;
            btStartPlay.Enabled = true;
            btStopPlay.Enabled = true;
            btReSend.Enabled = true;
            btReSendMin.Enabled = true;
            btSend.Enabled = true;
            btInit.Text = "停止";
            IsRuning = true;
            cbCOM.Enabled = false;
            cbBPS.Enabled = false;
            tbIn.Enabled = false;
            tbOut.Enabled = false;
        }

        void ToStop()
        {
            cbRecord.Enabled = false;
            btStartPlay.Enabled = false;
            btStopPlay.Enabled = false;
            btReSend.Enabled = false;
            btReSendMin.Enabled = false;
            btSend.Enabled = false;
            btInit.Text = "启动";
            IsRuning = false;
            cbCOM.Enabled = true;
            cbBPS.Enabled = true;
            tbIn.Enabled = true;
            tbOut.Enabled = true;
        }

        #endregion

        #region 数据接收处理

        void ZProtocolProcessObj_OnReceviceBlock()
        {
            while (ZProtocolProcessObj.BufferBlockNum > 0)
            {
                ZBlock zBlock = ZProtocolProcessObj.FetchBlock();//取出数据
                ZBlockDescribe zBlockDescribe = new ZBlockDescribe();
                try
                {
                    zBlockDescribe = ZDescribeProcessObj.Lookup(zBlock.Word);//查询描述
                    if (zBlockDescribe.BlockWord != 0 && zBlockDescribe.BlockWord == zBlock.Word)
                    {
                        DataString dataString = Converter.GetDataString(zBlock, zBlockDescribe);//格式化数据
                        Converter.UpdateDataBlock(Blocks[zBlock.Word], dataString);//更新显示
                        //附加处理
                        //记录飞行状态数据
                        if (cbFlyRec.Checked == true)
                        {
                            FlyRec(dataString);
                        }
                        //TODO:特定命令字处理
                        //显示消息
                        ShowMessage(ref zBlock);
                    }
                }
                catch
                {
                 //   WriteLine("Undefine Word:0x" + zBlock.Word.ToString("X"));
                }

                try
                {
                    zBlockDescribe = ZDescribeProcessObjMin.Lookup(zBlock.Word);//查询描述
                    if (zBlockDescribe.BlockWord != 0 && zBlockDescribe.BlockWord == zBlock.Word)
                    {
                        DataString dataString = Converter.GetDataString(zBlock, zBlockDescribe);//格式化数据
                        Converter.UpdateDataBlock(BlocksMin[zBlock.Word], dataString);//更新显示

                        switch (dataString.BlockWord)
                        {
                            case 0x5B:
                                dataGraphics1.AddData(dataString.Value[0]);//x位置
                                dataGraphics2.AddData(dataString.Value[1]);//y位置
                                dataGraphics3.AddData(dataString.Value[2]);//高度
                                break;
                            case 0x5C:
                                dataGraphics4.AddData(dataString.Value[0]);//x速度
                                dataGraphics6.AddData(dataString.Value[1]);//y速度
                                dataGraphics10.AddData(dataString.Value[2]);//z速度
                                break;
                            case 0x59:
                                dataGraphics5.AddData(dataString.Value[0]);//俯仰角
                                dataGraphics7.AddData(dataString.Value[1]);//滚转角
                                dataGraphics9.AddData(dataString.Value[2]);//航向角
                                break;
                            case 0x5A:
                                dataGraphics8.AddData(dataString.Value[2]);//航向角速度
                                break;
                            case 0x52:
                                dataGraphicsTwoLine1.AddData(dataString.Value[0], 0);//算法舵量A
                                dataGraphicsTwoLine2.AddData(dataString.Value[1], 0);//算法舵量B
                                dataGraphicsTwoLine3.AddData(dataString.Value[2], 0);//算法舵量C
                                break;
                            case 0x53:
                                dataGraphicsTwoLine4.AddData(dataString.Value[0], 0);//算法舵量D
                                dataGraphicsTwoLine5.AddData(dataString.Value[1], 0);//算法舵量E
                                break;
                            case 0x54:
                                dataGraphicsTwoLine1.AddData(dataString.Value[0], 1);//遥控舵量A
                                dataGraphicsTwoLine2.AddData(dataString.Value[1], 1);//遥控舵量B
                                dataGraphicsTwoLine3.AddData(dataString.Value[2], 1);//遥控舵量C
                                break;
                            case 0x55:
                                dataGraphicsTwoLine4.AddData(dataString.Value[0], 1);//遥控舵量D
                                dataGraphicsTwoLine5.AddData(dataString.Value[1], 1);//遥控舵量E
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch
                {
                   // WriteLine("Undefine Word:0x" + zBlock.Word.ToString("X"));
                }


            }
        }

        private void ShowMessage(ref ZBlock zBlock)
        {
            if (zBlock.Word == 0x10)
            {
                char[] cs = new char[12];
                int i = 0;
                for (i = 0; i < 12; i++)
                {
                    char c = (char)zBlock.Data[i];
                    if (c == 0)
                    {
                        break;
                    }
                    cs[i] = c;
                }
                string s = new string(cs, 0, i);
                WriteLine(s);
            }
        }

        void ZProtocolProcessObj_OnReceviceFrame()
        {
            while (ZProtocolProcessObj.BufferFrameNum > 0)
            {
                ZFrame z = ZProtocolProcessObj.FetchFrame();//取出数据
                //更新显示
                tbSourceShow.Text = z.Source.ToString();
                tbTargetShow.Text = z.Target.ToString();
                tbReserveShow.Text = z.Reserve.ToString();
                //TODO:0Block事件处理
                if (z.PackageNum == 0)
                {
                    string s;
                    switch (z.Reserve)
                    {
                        case 0x05:
                            s = "OKFPGA";
                            break;
                        case 0xE5:
                            s = "ErrFPGA";
                            break;
                        case 0x10:
                            s = "MODEManual";
                            break;
                        case 0x11:
                            s = "MODEMix";
                            break;
                        case 0x12:
                            s = "MODETarget0";
                            break;
                        case 0x13:
                            s = "MODETargetP";
                            break;
                        case 0x14:
                            s = "MODETargetV";
                            break;
                        case 0x15:
                            s = "MODETargetX";
                            break;
                        case 0x16:
                            s = "MODEFactory";
                            break;
                        case 0x20:
                            s = "中位数据采集完成";
                            break;
                        default:
                            s = "UnknowEventID:0x" + z.Reserve.ToString("X");
                            break;
                    }
                    WriteLine(DateTime.Now.ToLongTimeString() + "-" + s);
                }
            }
        }

        #endregion

        #region 回放部分

        private void cbRecord_CheckedChanged(object sender, EventArgs e)
        {
            SerialIOSwitchObj.IsLog = cbRecord.Checked;
        }

        private void btStopPlay_Click(object sender, EventArgs e)
        {
            FileObj.Seek(0);
        }

        private void btStartPlay_Click(object sender, EventArgs e)
        {
            timerPlay.Enabled = !timerPlay.Enabled;
            UpdatePlayState();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int n = FileObj.ReadStream(40);
            lbPlayState.Text = "接收:" + FileObj.ReceiveByteNum + "字节";
            if (n == 0)
            {
                timerPlay.Enabled = false;
                UpdatePlayState();
                FileObj.Seek(0);
            }
        }

        private void UpdatePlayState()
        {
            if (timerPlay.Enabled == true)
            {
                SerialIOSwitchObj.ReadSwitch = SerialIOSwitch.ReadWay.File;
                btStartPlay.Text = "停止回放";
            }
            else
            {
                if (RS232Obj != null)
                {
                    SerialIOSwitchObj.ReadSwitch = SerialIOSwitch.ReadWay.RS232;
                }
                btStartPlay.Text = "开始回放";
            }
        }

        #endregion

        #region 数据显示修改部分

        private void btUnlock_Click(object sender, EventArgs e)
        {
            foreach (var b in Blocks)
            {
                foreach (var p in b.Value.Parts)
                {
                    p.Unlock();
                }
            }
        }

        private void btUnlockMin_Click(object sender, EventArgs e)
        {
            foreach (var b in BlocksMin)
            {
                foreach (var p in b.Value.Parts)
                {
                    p.Unlock();
                }
            }
        }

        private void btReSend_Click(object sender, EventArgs e)
        {
            ZProtocolProcessObj.WriteFrame(byte.Parse(tbSourceShow.Text), byte.Parse(tbTargetShow.Text), byte.Parse(tbReserveShow.Text));//TODO:Try
            foreach (var b in Blocks)
            {
                bool IsLock = false;
                foreach (var p in b.Value.Parts)
                {
                    if (p.IsLocked)
                    {
                        IsLock = true;
                        break;
                    }
                }
                if (IsLock == true)
                {
                    ZBlockDescribe zBlockDescribe = ZDescribeProcessObj.Lookup(b.Key);
                    ZBlock zBlock = Converter.GetZBlock(b.Value, zBlockDescribe);
                    ZProtocolProcessObj.AddBlock(zBlock);
                    foreach (var p in b.Value.Parts)
                    {
                        p.Unlock();
                    }
                    if (ZProtocolProcessObj.PackageNumWaitToSend == ZProtocolProcess.BlockNumMax)
                    {
                        ZProtocolProcessObj.Send();
                    }
                }
            }
            if (ZProtocolProcessObj.PackageNumWaitToSend > 0)
            {
                ZProtocolProcessObj.Send();
            }
        }

        private void btReSendMin_Click(object sender, EventArgs e)
        {
            ZProtocolProcessObj.WriteFrame(byte.Parse(tbSourceShow.Text), byte.Parse(tbTargetShow.Text), byte.Parse(tbReserveShow.Text));//TODO:Try
            foreach (var b in BlocksMin)
            {
                bool IsLock = false;
                foreach (var p in b.Value.Parts)
                {
                    if (p.IsLocked)
                    {
                        IsLock = true;
                        break;
                    }
                }
                if (IsLock == true)
                {
                    ZBlockDescribe zBlockDescribeMin = ZDescribeProcessObjMin.Lookup(b.Key);
                    ZBlock zBlockMin = Converter.GetZBlock(b.Value, zBlockDescribeMin);
                    ZProtocolProcessObj.AddBlock(zBlockMin);
                    foreach (var p in b.Value.Parts)
                    {
                        p.Unlock();
                    }
                    if (ZProtocolProcessObj.PackageNumWaitToSend == ZProtocolProcess.BlockNumMax)
                    {
                        ZProtocolProcessObj.Send();
                    }
                }
            }
            if (ZProtocolProcessObj.PackageNumWaitToSend > 0)
            {
                ZProtocolProcessObj.Send();
            }
        }

        //TODO:对发送的数据进行保存

        #endregion

        #region 数据编辑部分

        List<ZBlockDescribe> ZBlockDescribes = new List<ZBlockDescribe>(10);

        private void tbAdd_Click(object sender, EventArgs e)
        {
            EditForm eidtForm = new EditForm();
            eidtForm.OnConvertFinish += new EventHandler(eidtForm_OnConvertFinish);
            eidtForm.ShowDialog();
        }

        void eidtForm_OnConvertFinish(object sender, EventArgs e)
        {
            EditForm eidtForm = (EditForm)sender;
            DataBlock dataBlock = Converter.CreateDataBlock(eidtForm.zBlockDescribe);
            Converter.UpdateDataBlock(dataBlock, eidtForm.dataString);
            flowLayoutPanel2.Controls.Add(dataBlock);
            ZBlockDescribes.Add(eidtForm.zBlockDescribe);
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            flowLayoutPanel2.Controls.Clear();
            ZBlockDescribes.Clear();
        }

        private void btSend_Click(object sender, EventArgs e)
        {
            try
            {
                ZPackage zPackage = MakeZPackage();
                SendPackage(zPackage);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void SendPackage(ZPackage zPackage)
        {
            ZProtocolProcessObj.WriteFrame(zPackage.Frame);
            for (int i = 0; i < zPackage.Blocks.Length; i++)
            {
                ZProtocolProcessObj.AddBlock(zPackage.Blocks[i]);
            }
            ZProtocolProcessObj.Send();
        }

        private ZPackage MakeZPackage()
        {
            ZPackage zPackage = new ZPackage();

            zPackage.Frame.PackageNum = (byte)ZBlockDescribes.Count;
            zPackage.Frame.Reserve = byte.Parse(tbReserve.Text);
            zPackage.Frame.Source = byte.Parse(tbSource.Text);
            zPackage.Frame.Target = byte.Parse(tbTarget.Text);
            zPackage.Frame.S1 = ZProtocolProcess.S1;
            zPackage.Frame.S2 = ZProtocolProcess.S2;

            zPackage.Blocks = new ZBlock[zPackage.Frame.PackageNum];
            for (int i = 0; i < zPackage.Frame.PackageNum; i++)
            {
                zPackage.Blocks[i] = Converter.GetZBlock((DataBlock)flowLayoutPanel2.Controls[i], ZBlockDescribes[i]);
            }

            return zPackage;
        }

        void UpdatePackagelist()
        {
            lbCommand.Items.Clear();
            foreach (var z in Packages)
            {
                lbCommand.Items.Add(z.Key);
            }
        }

        #endregion

        #region 数据包持久化及处理

        //保存
        private void btSave_Click(object sender, EventArgs e)
        {
            if (tbName.Text != "")
            {
                if (Packages.ContainsKey(tbName.Text) == false)
                {
                    ZPackage zPackage = MakeZPackage();
                    Packages.Add(tbName.Text, zPackage);
                    UpdatePackagelist();
                }
                else
                {
                    MessageBox.Show("名称重复");
                }
            }
            else
            {
                MessageBox.Show("名称不能为空");
            }
        }

        //持久化
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Stream sr = System.IO.File.Open("Package.bin", FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(sr, Packages);
                sr.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, "无法保存Packages");
            }
            try
            {
                if (SerialIOSwitchObj != null)
                {
                    SerialIOSwitchObj.Close();
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, "端口关闭错误");
            }

        }

        //发送
        private void lbCommand_DoubleClick(object sender, EventArgs e)
        {
            if (lbCommand.SelectedIndex >= 0 && lbCommand.SelectedIndex < lbCommand.Items.Count)
            {
                if (IsRuning == true)
                {
                    SendPackage(Packages[(string)lbCommand.Items[lbCommand.SelectedIndex]]);
                }
            }
        }

        //删除
        private void lbCommand_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                if (lbCommand.SelectedIndex >= 0 && lbCommand.SelectedIndex < lbCommand.Items.Count)
                {
                    Packages.Remove((string)lbCommand.Items[lbCommand.SelectedIndex]);
                    UpdatePackagelist();
                }
            }
        }

        #endregion

        void WriteLine(string s)
        {
            //SB.AppendLine(s);
            //tbInfo.Text = SB.ToString();
            tbInfo.AppendText(s + "\n");
        }

        //数据区获得焦点
        private void flowLayoutPanel1_Click(object sender, EventArgs e)
        {
            FlowPlan.Focus();
        }
        private void flowLayoutPanel1Min_Click(object sender, EventArgs e)
        {
            FlowPlanMin.Focus();
        }

        //数据区滚动
        void flowLayoutPanel1_MouseWheel(object sender, MouseEventArgs e)
        {
            //int max = flowLayoutPanel1.VerticalScroll.Maximum;
            //int min = flowLayoutPanel1.VerticalScroll.Minimum;
            //int t = flowLayoutPanel1.VerticalScroll.Value + (e.Delta / 40);
            //if (t > max)
            //    t = max;
            //if (t < min)
            //    t = min;
            FlowPlan.VerticalScroll.Value += e.Delta / 40;
        }
        void flowLayoutPanel1_MouseWheelMin(object sender, MouseEventArgs e)
        {
            //int max = flowLayoutPanel1.VerticalScroll.Maximum;
            //int min = flowLayoutPanel1.VerticalScroll.Minimum;
            //int t = flowLayoutPanel1.VerticalScroll.Value + (e.Delta / 40);
            //if (t > max)
            //    t = max;
            //if (t < min)
            //    t = min;
            FlowPlanMin.VerticalScroll.Value += e.Delta / 40;
        }

        //状态更新
        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            if (ZProtocolProcessObj != null)
            {
                if (ZProtocolProcessObj.ReceiveByteNum > 10000)
                {
                    lbInfo.Text = string.Format("{0},{1},{2}", ZProtocolProcessObj.ReceiveByteNum, ZProtocolProcessObj.ReceiveFrameNum, ZProtocolProcessObj.ReceiveBlockNum);
                }
                else
                {
                    lbInfo.Text = string.Format("Byte:{0},Frame:{1},Block:{2}", ZProtocolProcessObj.ReceiveByteNum, ZProtocolProcessObj.ReceiveFrameNum, ZProtocolProcessObj.ReceiveBlockNum);
                }
            }
            else
            {
                lbInfo.Text = "Byte:0,Frame:0,Block:0";
            }
        }


        #region 附加的飞行数据记录

        StreamWriter FlyRecStream = null;

        private void FlyRec(DataString dataString)
        {
            if (FlyRecStream != null)
            {
                StringBuilder sb = new StringBuilder(100);
                sb.Append("0x");
                sb.Append(dataString.BlockWord.ToString("X"));
                for (int i = 0; i < dataString.Data.Length; i++)
                {
                    sb.Append(" ");
                    sb.Append(dataString.Data[i]);
                }
                FlyRecStream.WriteLine(sb.ToString());
            }
        }

        private void cbFlyRec_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFlyRec.Checked)
            {
                if (FlyRecStream == null)
                {
                    FlyRecStream = new StreamWriter("FlyRecData.log", true);
                }
            }
            else
            {
                if (FlyRecStream != null)
                {
                    FlyRecStream.Close();
                    FlyRecStream.Dispose();
                    FlyRecStream = null;
                }
            }
        }

        #endregion

    }
}