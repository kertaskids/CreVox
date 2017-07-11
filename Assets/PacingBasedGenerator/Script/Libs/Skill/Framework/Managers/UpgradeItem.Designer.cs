using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Skill.Framework.IO;


namespace Skill.Framework.Managers
{
[System.Serializable]
public partial class UpgradeItem : Skill.Framework.IO.ISavable
{

// Variables
private  bool _IsDirty;
private  int _Id;
private  float _Duration;
private  int _RepeatCount;
private  int _UpgradeCount;
private  string _Tag;
private  Skill.Framework.IO.DateTime _StartTime;

// Properties
/// <summary> is any changes happened to savable object </summary>
public   bool IsDirty { get { if(_IsDirty)  return _IsDirty;
if(_StartTime.IsDirty) return true;
return _IsDirty;
 } }
public   int Id { get { return _Id; } set { if(_Id != value) {  _Id = value; _IsDirty = true; } } }
public   float Duration { get { return _Duration; } set { if(_Duration != value) {  _Duration = value; _IsDirty = true; } } }
public   int RepeatCount { get { return _RepeatCount; } set { if(_RepeatCount != value) {  _RepeatCount = value; _IsDirty = true; } } }
public   int UpgradeCount { get { return _UpgradeCount; } set { if(_UpgradeCount != value) {  _UpgradeCount = value; _IsDirty = true; } } }
public   string Tag { get { return _Tag; } set { if(_Tag != value) {  _Tag = value; _IsDirty = true; } } }
public   Skill.Framework.IO.DateTime StartTime { get { return _StartTime; } set { _StartTime = value; _IsDirty = true; } }

// Methods
public    UpgradeItem()
{
}
public   void SetAsClean()
{
_IsDirty = false;
_StartTime.SetAsClean();

}
public static  UpgradeItem CreateUpgradeItem()
{
return new UpgradeItem();
}
public   void Save(Skill.Framework.IO.XmlElement e, Skill.Framework.IO.XmlSaveStream stream)
{
Skill.Framework.IO.XmlElement _IdElement = stream.Create("Id",_Id);
e.AppendChild(_IdElement);
Skill.Framework.IO.XmlElement _DurationElement = stream.Create("Duration",_Duration);
e.AppendChild(_DurationElement);
Skill.Framework.IO.XmlElement _RepeatCountElement = stream.Create("RepeatCount",_RepeatCount);
e.AppendChild(_RepeatCountElement);
Skill.Framework.IO.XmlElement _UpgradeCountElement = stream.Create("UpgradeCount",_UpgradeCount);
e.AppendChild(_UpgradeCountElement);
Skill.Framework.IO.XmlElement _TagElement = stream.Create("Tag",_Tag);
e.AppendChild(_TagElement);
Skill.Framework.IO.XmlElement _StartTimeElement = stream.Create<Skill.Framework.IO.DateTime>("StartTime",_StartTime);
e.AppendChild(_StartTimeElement);

}
public   void Save(Skill.Framework.IO.BinarySaveStream stream)
{
stream.Write(_Id);
stream.Write(_Duration);
stream.Write(_RepeatCount);
stream.Write(_UpgradeCount);
stream.Write(_Tag);
stream.Write<Skill.Framework.IO.DateTime>(_StartTime);

}
public   void Load(Skill.Framework.IO.XmlElement e, Skill.Framework.IO.XmlLoadStream stream)
{
Skill.Framework.IO.XmlElement element = e.FirstChild as Skill.Framework.IO.XmlElement;
while (element != null)
{
switch (element.Name)
{
case "Id":
this._Id = stream.ReadInt( element );
break;
case "Duration":
this._Duration = stream.ReadFloat( element );
break;
case "RepeatCount":
this._RepeatCount = stream.ReadInt( element );
break;
case "UpgradeCount":
this._UpgradeCount = stream.ReadInt( element );
break;
case "Tag":
this._Tag = stream.ReadString( element );
break;
case "StartTime":
this._StartTime = stream.ReadSavable<Skill.Framework.IO.DateTime>( element , Skill.Framework.IO.DateTime.CreateDateTime );
break;
}
element = e.GetNextSibling(element);
}
SetAsClean();

}
public   void Load(Skill.Framework.IO.BinaryLoadStream stream)
{
this._Id = stream.ReadInt();
this._Duration = stream.ReadFloat();
this._RepeatCount = stream.ReadInt();
this._UpgradeCount = stream.ReadInt();
this._Tag = stream.ReadString();
this._StartTime = stream.ReadSavable<Skill.Framework.IO.DateTime>( Skill.Framework.IO.DateTime.CreateDateTime );
SetAsClean();

}

}
}
