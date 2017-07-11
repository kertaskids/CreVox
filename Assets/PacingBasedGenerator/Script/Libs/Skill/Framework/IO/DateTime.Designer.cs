using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Skill.Framework.IO;


namespace Skill.Framework.IO
{
public partial struct DateTime : Skill.Framework.IO.ISavable
{

// Variables
private  bool _IsDirty;
private  int _Year;
private  int _Month;
private  int _Day;
private  int _Hour;
private  int _Minute;
private  int _Second;
private  int _Millisecond;

// Properties
/// <summary> is any changes happened to savable object </summary>
public   bool IsDirty { get { return _IsDirty;
 } }
public   int Year { get { return _Year; } set { if(_Year != value) {  _Year = value; _IsDirty = true; } } }
public   int Month { get { return _Month; } set { if(_Month != value) {  _Month = value; _IsDirty = true; } } }
public   int Day { get { return _Day; } set { if(_Day != value) {  _Day = value; _IsDirty = true; } } }
public   int Hour { get { return _Hour; } set { if(_Hour != value) {  _Hour = value; _IsDirty = true; } } }
public   int Minute { get { return _Minute; } set { if(_Minute != value) {  _Minute = value; _IsDirty = true; } } }
public   int Second { get { return _Second; } set { if(_Second != value) {  _Second = value; _IsDirty = true; } } }
public   int Millisecond { get { return _Millisecond; } set { if(_Millisecond != value) {  _Millisecond = value; _IsDirty = true; } } }

// Methods
public   void SetAsClean()
{
_IsDirty = false;

}
public static  DateTime CreateDateTime()
{
return new DateTime();
}
public   void Save(Skill.Framework.IO.XmlElement e, Skill.Framework.IO.XmlSaveStream stream)
{
Skill.Framework.IO.XmlElement _YearElement = stream.Create("Year",_Year);
e.AppendChild(_YearElement);
Skill.Framework.IO.XmlElement _MonthElement = stream.Create("Month",_Month);
e.AppendChild(_MonthElement);
Skill.Framework.IO.XmlElement _DayElement = stream.Create("Day",_Day);
e.AppendChild(_DayElement);
Skill.Framework.IO.XmlElement _HourElement = stream.Create("Hour",_Hour);
e.AppendChild(_HourElement);
Skill.Framework.IO.XmlElement _MinuteElement = stream.Create("Minute",_Minute);
e.AppendChild(_MinuteElement);
Skill.Framework.IO.XmlElement _SecondElement = stream.Create("Second",_Second);
e.AppendChild(_SecondElement);
Skill.Framework.IO.XmlElement _MillisecondElement = stream.Create("Millisecond",_Millisecond);
e.AppendChild(_MillisecondElement);

}
public   void Save(Skill.Framework.IO.BinarySaveStream stream)
{
stream.Write(_Year);
stream.Write(_Month);
stream.Write(_Day);
stream.Write(_Hour);
stream.Write(_Minute);
stream.Write(_Second);
stream.Write(_Millisecond);

}
public   void Load(Skill.Framework.IO.XmlElement e, Skill.Framework.IO.XmlLoadStream stream)
{
Skill.Framework.IO.XmlElement element = e.FirstChild as Skill.Framework.IO.XmlElement;
while (element != null)
{
switch (element.Name)
{
case "Year":
this._Year = stream.ReadInt( element );
break;
case "Month":
this._Month = stream.ReadInt( element );
break;
case "Day":
this._Day = stream.ReadInt( element );
break;
case "Hour":
this._Hour = stream.ReadInt( element );
break;
case "Minute":
this._Minute = stream.ReadInt( element );
break;
case "Second":
this._Second = stream.ReadInt( element );
break;
case "Millisecond":
this._Millisecond = stream.ReadInt( element );
break;
}
element = e.GetNextSibling(element);
}
SetAsClean();

}
public   void Load(Skill.Framework.IO.BinaryLoadStream stream)
{
this._Year = stream.ReadInt();
this._Month = stream.ReadInt();
this._Day = stream.ReadInt();
this._Hour = stream.ReadInt();
this._Minute = stream.ReadInt();
this._Second = stream.ReadInt();
this._Millisecond = stream.ReadInt();
SetAsClean();

}

}
}
