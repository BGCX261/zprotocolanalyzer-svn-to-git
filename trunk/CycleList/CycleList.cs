using System;
using System.Collections.Generic;
using System.Text;

namespace zyc.DataLib
{
    /// <summary>
    /// 循环列队容器
    /// 提供一个指定大小的泛型容器，当容器填满之后添加的对象会挤出最老的对象。
    /// </summary>
    /// <remarks>
    /// 先入低址
    /// </remarks>
    /// <typeparam name="T">待储存泛型对象</typeparam>
    public class CycleList<T>
    {
        T[] _Datas;
        int _Pointer;
        int _Amount;

        /// <summary>
        /// OnThrowObeject事件的函数模板
        /// </summary>
        /// <param name="ThrowedObject">被丢弃的对象</param>
        public delegate void ThrowObeject(T ThrowedObject);
        /// <summary>
        /// 当需要丢弃对象时引发
        /// </summary>
        public event ThrowObeject OnThrowObeject;

        public CycleList(int Capacity)
        {
            _Datas = new T[Capacity];
            _Pointer = 0;
            _Amount = 0;
        }
        public int GetCapacity()
        {
            if (_Datas != null)
            {
                return _Datas.Length;
            }
            else
            {
                return 0;
            }
        }
        public bool SetCapacity(int Capacity)
        {
            _Pointer = 0;
            _Amount = 0;

            if ((_Datas == null) || (Capacity != _Datas.Length))
            {
                if (Capacity == 0)
                {
                    _Datas = null;
                }
                else
                {
                    _Datas = new T[Capacity];
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public int Amount
        {
            get { return _Amount; }
        }
        public void Clear()
        {
            if (_Datas != null)
                Array.Clear(_Datas, 0, _Datas.Length);
            _Pointer = 0;
            _Amount = 0;
        }
        public void InsertObject(T Object)
        {
            if (this.GetCapacity() == 0)
            {
                throw new Exception("There is no space to InsertObject,maybe you didn`t SetCapacity>0");
            }
            else
            {
                if (_Amount == this.GetCapacity())
                {
                    if (OnThrowObeject != null)
                    {
                        OnThrowObeject(_Datas[_Pointer]);
                    }
                    _Datas[_Pointer] = Object;
                    _Pointer++;
                    if (_Pointer >= _Datas.Length)
                        _Pointer -= _Datas.Length;
                }
                else
                {
                    int n = _Pointer + _Amount;
                    if (n >= _Datas.Length)
                        n -= _Datas.Length;
                    _Datas[n] = Object;
                    _Amount++;
                }

            }
        }

        public T FetchObject()
        {
            if (_Amount > 0)
            {
                T t = _Datas[_Pointer];
                _Pointer++;
                if (_Pointer >= _Datas.Length)
                    _Pointer -= _Datas.Length;
                _Amount--;
                return t;
            }
            else
            {
                throw new Exception("There is no Object to Fetch");
            }
        }

        public T this[int index]
        {
            get
            {
                if ((index < _Amount) && (index >= 0))
                {
                    int n = _Pointer + index;
                    if (n >= _Datas.Length)
                        n -= _Datas.Length;
                    return _Datas[n];
                }
                else
                {
                    throw new IndexOutOfRangeException("Out of range of store data");
                }
            }
        }
    }
}
