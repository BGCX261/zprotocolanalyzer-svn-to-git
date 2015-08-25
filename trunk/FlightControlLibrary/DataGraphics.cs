using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using zyc.DataLib;
using System.Drawing.Drawing2D;

namespace zyc.AutoPilotTester
{
    public partial class DataGraphics : UserControl, IDataControl
    {
        #region Data

        //数据点循环列队
        CycleList<float> _Datas;

        //数据点间隔
        int _Interval = 3;

        //绘图模式
        FillModeEnum _FillMode = FillModeEnum.Limit;

        //数据统计信息
        float _High, _Low, high, low;

        //表头
        string _Title;

        //绘图区位置
        int DrawX, DrawY, DrawWidth, DrawHeight;

        //栅格位置，栅格半径
        int GridPosi, _GridR = 15;

        Pen _GridPen = Pens.LightGray;
        Brush _BackgroundBrush = Brushes.Snow;

        #endregion

        #region Init

        public DataGraphics()
        {
            InitializeComponent();
            //默认值
            //_Interval = 3;
            //_GridR = 15;
            //_FillMode = FillModeEnum.All;
            //初始化数据
            InitData();
            //初始化界面
            InitFace();
        }

        void InitData()
        {
            int n = this.Width / _Interval + 1;//计算需储存数据个数
            if (_Datas == null)
            {
                _Datas = new CycleList<float>(n);
            }
            else
            {
                _Datas.SetCapacity(n);
            }

            //设置极值
            low = float.PositiveInfinity;
            high = float.NegativeInfinity;
        }

        private void InitFace()
        {
            //计算绘图区位置与尺寸
            DrawX = 0;
            DrawY = label1.Height + label1.Top + 1;
            DrawWidth = this.Width;
            DrawHeight = this.Height - DrawY;
            //初始化背景格位置
            GridPosi = 0;
        }

        #endregion

        #region Property

        /// <summary>
        /// 数据点间隔
        /// </summary>
        public int Interval
        {
            get { return _Interval; }
            set
            {
                if (_Interval <= 0)
                {
                    throw new ArgumentException("Interval cann`t less than zero");
                }
                if (_Interval != value)
                {
                    _Interval = value;
                    GridPosi = 0;
                    InitData();
                }
            }
        }

        /// <summary>
        /// 绘图模式
        /// </summary>
        public FillModeEnum FillMode
        {
            get { return _FillMode; }
            set
            {
                if (value != _FillMode)
                {
                    _FillMode = value;
                    ClearData();
                }
            }
        }

        /// <summary>
        /// 背景方格边长
        /// </summary>
        public int GridR
        {
            get { return _GridR; }
            set
            {
                if (_GridR != value)
                {
                    _GridR = value;
                    InitFace();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        public void ClearData()
        {
            if (_Datas != null)
                _Datas.Clear();
            InitData();
            this.Invalidate();
        }

        /// <summary>
        /// 以储存数据数量
        /// </summary>
        public int Amount
        {
            get
            {
                if (_Datas != null)
                    return _Datas.Amount;
                else
                    return 0;
            }
        }

        /// <summary>
        /// 数据最小值（部分模式下可靠）
        /// </summary>
        public float Low
        {
            get { return _Low; }
            set { _Low = value; }
        }

        /// <summary>
        /// 数据最大值（部分模式下可靠）
        /// </summary>
        public float High
        {
            get { return _High; }
            set { _High = value; }
        }

        /// <summary>
        /// 设置标题
        /// </summary>
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                label1.Text = _Title;
            }
        }

        /// <summary>
        /// 标题字体颜色
        /// </summary>
        public Color FontColor
        {
            get { return label1.ForeColor; }
            set { label1.ForeColor = value; }
        }

        /// <summary>
        /// 标题背景颜色
        /// </summary>
        public Color TitleColor
        {
            get { return this.BackColor; }
            set { this.BackColor = value; }
        }


        /// <summary>
        /// 背景框样式
        /// </summary>
        public Pen GridPen
        {
            get { return _GridPen; }
            set { _GridPen = value; this.Invalidate(); ;}
        }

        Pen _LinePen = new Pen(Color.Black,2);
        Pen _ZeroPen = new Pen(Color.Blue, 1);


        /// <summary>
        /// 绘图线样式
        /// </summary>
        public Pen LinePen
        {
            get { return _LinePen; }
            set { _LinePen = value; this.Invalidate(); }
        }

        /// <summary>
        /// 背景样式
        /// </summary>
        public Brush BackgroundBrush
        {
            get { return _BackgroundBrush; }
            set { _BackgroundBrush = value; this.Invalidate(); }
        }

        #endregion

        #region Enum

        /// <summary>
        /// 绘图模式
        /// All:全体实数,以最大和最小值为边界自动缩放到合适比例.
        /// AboveZero:非负数,以零和最大值为边界自动缩放到合适比例.
        /// Limit:指定范围,以指定范围为边界自动缩放到合适比例.
        /// </summary>
        public enum FillModeEnum
        {
            All, AboveZero, Limit
        }

        #endregion

        #region IDataControl 成员

        int _ID;
        int _Index;
        float _Data;

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

        #region Method

        private void DataGraphics_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            //绘制示波器背景
            g.FillRectangle(_BackgroundBrush, DrawX, DrawY, DrawWidth, DrawHeight);
            //绘制竖条纹
            for (int i = DrawWidth - GridPosi; i > 0; i -= _GridR)
            {
                g.DrawLine(_GridPen, i, DrawY, i, DrawY + DrawHeight);
            }
            //绘制横条纹
            for (int i = DrawY; i < this.Height; i += _GridR)
            {
                g.DrawLine(_GridPen, DrawX, i, DrawWidth, i);
            }
            //算个点位置
            PointF[] points = new PointF[_Datas.Amount];
            for (int i = 0; i < _Datas.Amount; i++)
            {
                points[i].X = DrawX + DrawWidth - _Interval * i;//计算X轴坐标//_Datas.Amount - i - 1
                switch (_FillMode)
                {
                    case FillModeEnum.All:
                        DrawZeroLine(g, MapData(high, low, 0));
                        points[_Datas.Amount - i - 1].Y = DrawY + DrawHeight - MapData(high, low, _Datas[i]);
                        break;
                    case FillModeEnum.AboveZero:
                        points[_Datas.Amount - i - 1].Y = DrawY + DrawHeight - MapData(high, 0, _Datas[i]);
                        break;
                    case FillModeEnum.Limit:
                        DrawZeroLine(g, MapData(_High, _Low, 0));
                        points[_Datas.Amount - i - 1].Y = DrawY + DrawHeight - MapData(_High, _Low, _Datas[i]);
                        break;
                }
            }
            //绘制折线图
            if ((points != null) && (points.Length > 1))
            {
                g.DrawLines(_LinePen, points);
            }
        }

        //计算数据对应坐标位置
        private float MapData(float high, float low, double d)
        {
            float band = high - low;
            if (band == 0)
                return (float)(0.5 * DrawHeight);
            return (float)(d - low) / (band) * DrawHeight;
        }

        //绘制零位线
        private void DrawZeroLine(Graphics g, float y)
        {
            if (high * low < 0)
            {
                _ZeroPen.DashStyle = DashStyle.Dash;
                g.DrawLine(_ZeroPen, DrawX, DrawY + DrawHeight - y, DrawX + DrawWidth, DrawY + DrawHeight - y);
            }
        }

        //响应尺寸改变事件
        private void DataGraphics_Resize(object sender, EventArgs e)
        {
            InitData();
            InitFace();
            this.Invalidate();
        }

        //计算最小值
        private float ComputLow(CycleList<float> Datas)
        {
            if (Datas.Amount > 0)
            {
                float r = Datas[0];
                for (int i = 0; i < Datas.Amount; i++)
                {
                    if (Datas[i] < r)
                        r = Datas[i];
                }
                return r;
            }
            return 0;
        }

        //计算最大值
        private float ComputHigh(CycleList<float> Datas)
        {
            if (Datas.Amount > 0)
            {
                float r = Datas[0];
                for (int i = 0; i < Datas.Amount; i++)
                {
                    if (Datas[i] > r)
                        r = Datas[i];
                }
                return r;
            }
            return 0;
        }

        /// <summary>
        /// 向示波器控件添加数据
        /// </summary>
        /// <param name="data">待添加数据</param>
        public void AddData(float data)
        {
            //插入数据
            _Datas.InsertObject(data);
            //检测模式，校验数据，计算极值
            switch (_FillMode)
            {
                case FillModeEnum.All:
                    high = ComputHigh(_Datas);
                    low = ComputLow(_Datas);
                    break;
                case FillModeEnum.AboveZero:
                    if (data < 0)
                        throw new ArgumentException("Data must above zero in AboveZero mode");
                    high = ComputHigh(_Datas);
                    break;
                case FillModeEnum.Limit:
                    if ((data < _Low) || (data > _High))
                        throw new ArgumentException("Data must in the limit zone in Limit mode");
                    break;
            }
            //递增网格绘制位置
            GridPosi += _Interval;
            //当达到临界值则返回0
            if (GridPosi >= _GridR)
            {
                GridPosi -= _GridR;
            }
            //重绘
            this.Invalidate();
        }



        /// <summary>
        /// 返回制定位置的数据
        /// </summary>
        /// <param name="index">序号(以从早到晚的添加顺序摆列)</param>
        /// <returns>所存数据</returns>
        public float GetData(int index)
        {
            return _Datas[index];
        }

        #endregion

        private void DataGraphics_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if ((e.X > DrawX) && (e.X < DrawX + DrawWidth) && (e.Y > DrawY) && (e.Y < DrawY + DrawHeight))
                {
                    int n = (DrawX + DrawWidth - e.X) / _Interval;
                    n = _Datas.Amount - 2 - n;
                    if ((n >= 0) && (n < _Datas.Amount))
                    {
                        float f = _Datas[n];
                        this.label1.ForeColor = System.Drawing.Color.Blue;
                        this.label1.Text = _Title + " Data=" + f;
                    }
                    else
                    {
                        this.label1.Text = _Title;
                    }
                }
            }
        }
    }
}
