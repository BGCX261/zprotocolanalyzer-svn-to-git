using System;
using System.Collections.Generic;
using System.Text;

namespace zyc.AutoPilotTester
{
    public interface IDataControl
    {
        int ID
        {
            get;
            set;
        }
        int Index
        {
            get;
            set;
        }
        float Data
        {
            get;
            set;
        }
    }
}
