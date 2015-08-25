using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using zyc.DataLib;

namespace zyc.AutoPilotTester
{
    public partial class DataMap : UserControl
    {
        public CycleList<PointF> DataList;

        float MinX, MinY, MaxX, MaxY;

        public DataMap()
        {
            InitializeComponent();
            DataList = new CycleList<PointF>(2000);
        }

        void CalaMax()
        {
            if (DataList != null && DataList.Amount > 1)
            {
                MinX = DataList[0].X; MinY = DataList[0].Y;
                MaxX = DataList[0].X; MaxY = DataList[0].Y;
                for (int i = 0; i < DataList.Amount; i++)
                {
                    if (DataList[i].X < MinX)
                        MinX = DataList[i].X;
                    if (DataList[i].Y < MinY)
                        MinY = DataList[i].Y;
                    if (DataList[i].X > MaxX)
                        MaxX = DataList[i].X;
                    if (DataList[i].Y > MaxY)
                        MaxY = DataList[i].Y;
                }
            }
            else
            {
                MinX = MinY = MaxX = MaxY = 0;
            }
        }

        public void AddPoint(PointF point)
        {
            lock (DataList)
            {
                DataList.InsertObject(point);
            }
            this.Invalidate();
        }

        private void DataMap_Paint(object sender, PaintEventArgs e)
        {
            
            if (DataList.Amount > 1)
            {
                lock (DataList)
                {
                    CalaMax();
                    try
                    {
                        PointF[] pfs = new PointF[DataList.Amount];
                        for (int i = 0; i < DataList.Amount; i++)
                        {
                            pfs[i].X = CalcX(DataList[i].X);
                            pfs[i].Y = CalcY(DataList[i].Y);
                        }
                        e.Graphics.DrawLines(new Pen(Color.Blue,2), pfs);
                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine(ee.Message);
                    }
                }
            }
        }

        public void Clear()
        {
            lock (DataList)
            {
                DataList.Clear();
            }
            this.Invalidate();
        }

        private float CalcX(float p)
        {
            float f = (p - MinX) * this.Width / (MaxX - MinX);
            if (float.IsInfinity(f))
            {
                f = this.Width / 2;
            }
            return f;
        }

        private float CalcY(float p)
        {
            float f = (p - MinY) * this.Height / (MaxY - MinY);
            if (float.IsInfinity(f))
            {
                f = this.Height / 2;
            }
            return f;
        }
    }
}
