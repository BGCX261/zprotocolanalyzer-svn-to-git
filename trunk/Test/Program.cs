using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using zyc.ZProtocolAnalyzer;
using zyc.DataLib;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestSerialIOSwitch();
            //TestFile();
            //TestZProtocolProcess();
            //TestZDescribeProcess();
            ZDescribeProcess ZDescribeProcessObj = new ZDescribeProcess();
            ZDescribeProcessObj.LoadDescribesFile("ProtocolDescribe.txt");
            File fio = new File("Frame1.bin", "out1.bin");
            fio.Open();
            //DataCenter DC = new DataCenter(fio, ZDescribeProcessObj);
            //DC.OnNewData += new DataCenter.NewData(DC_OnNewData);
            //DC.Start();
            fio.ReadStream(6);
            fio.ReadStream(15);

            //DC.WriteFrame(1, 2, 0);
            //DataStruct ds = DC.GetData(1);
            //ds.Data[0] = "0";
            //ds.Data[1] = "1";
            //ds.Data[2] = "ff";
            //ds.Data[3] = "Z";
            //ds.Data[4] = "2.3";
            //DC.AddData(ds);
            //DC.Send();

            Console.ReadLine();
        }

        static void DC_OnNewData(byte Word)
        {
            //int k = 0;
            //throw new NotImplementedException();
        }

        private static void TestSerialIOSwitch()
        {
            File[] io = new File[2];
            io[0] = new File("in1.bin", "1.bin");
            io[1] = new File("in2.bin", "2.bin");
            SerialIOSwitch sios = new SerialIOSwitch();
            sios.Open();
            //sios.WriteToList[0] = true;
            //sios.WriteToList[1] = true;
            //sios.ReadFromNo = 0;
            sios.OnRecevice += new ReceviceByte(sios_OnRecevice);
            byte b = sios.ReadByte();
            byte[] bs = new byte[12];
            for (int i = 0; i < 12; i++)
            {
                bs[i] = (byte)i;
            }
            sios.WriteBytes(bs, 0, 12);
            io[0].ReadStream(10);
        }

        static void sios_OnRecevice()
        {
            //int k = 0;
        }

        private static void TestFile()
        {
            File file = new File("in.bin", "1.bin");
            file.Open();
            byte[] bs = new byte[12];
            for (int i = 0; i < 12; i++)
            {
                bs[i] = (byte)i;
            }
            file.WriteBytes(bs, 0, 12);

            file.OnRecevice += new ReceviceByte(file_OnReceviceByte);
            byte b = file.ReadByte();
            file.ReadStream(2);
            file.ReadBytes(bs, 0, 12);
            b = file.ReadByte();

            file.Close();
        }

        static void file_OnReceviceByte()
        {
            //int k = 0;
        }

        private static void TestZDescribeProcess()
        {
            ZDescribeProcess zDescribeProcess = new ZDescribeProcess();
            zDescribeProcess.LoadDescribesFile("ProtocolDescribe.txt");
            ZBlockDescribe zpd = zDescribeProcess.Lookup(0xc0);
        }

        private static void TestZProtocolProcess()
        {
            RS232 rs232 = new RS232("COM4", 115200);
            rs232.Open();
            ZProtocolProcess zProtocolProcess = new ZProtocolProcess(rs232);
            zProtocolProcess.Start();
            Console.WriteLine("Ready...");
            //while (true)
            //{
            //    Console.ReadLine();
            //    zProtocolProcess.WriteFrame(1, 2, 0);
            //    zProtocolProcess.AddPackage(0xc0, 0xff, new byte[12]);
            //    zProtocolProcess.Send();
            //    Console.WriteLine("Send");
            //}
            byte[] bs = new byte[12];
            for (int i = 0; i < 12; i++)
            {
                bs[i] = (byte)i;
            }
            Console.ReadLine();
            for (int i = 0; i < 100; i++)
            {
                zProtocolProcess.WriteFrame((byte)i, (byte)(i + 1), 0);
                zProtocolProcess.AddBlock((byte)(i + 100), 0xff, bs);
                zProtocolProcess.Send();
                Console.WriteLine("Send");
                Thread.Sleep(15);
            }
        }
    }
}
