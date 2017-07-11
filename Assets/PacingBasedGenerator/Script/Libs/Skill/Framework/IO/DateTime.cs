using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Skill.Framework.IO;


namespace Skill.Framework.IO
{
    public partial struct DateTime : Skill.Framework.IO.ISavable
    {

        public System.DateTime ToSystem()
        {
            return new System.DateTime(Year, Month, Day, Hour, Minute, Second, Millisecond);
        }


        public void FromSystem(System.DateTime dt)
        {
            _Year = dt.Year;
            _Month = dt.Month;
            _Day = dt.Day;
            _Hour = dt.Hour;
            _Minute = dt.Minute;
            _Second = dt.Second;
            _Millisecond = dt.Millisecond;
            _IsDirty = true;
        }


        public DateTime(System.DateTime dt)
        {            
            _Year = dt.Year;
            _Month = dt.Month;
            _Day = dt.Day;
            _Hour = dt.Hour;
            _Minute = dt.Minute;
            _Second = dt.Second;
            _Millisecond = dt.Millisecond;
            _IsDirty = true;
        }
    }
}
