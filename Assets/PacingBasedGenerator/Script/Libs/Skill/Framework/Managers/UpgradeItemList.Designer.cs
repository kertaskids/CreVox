using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Skill.Framework.IO;


namespace Skill.Framework.Managers
{
[System.Serializable]
public class UpgradeItemList : Skill.Framework.IO.ISavable
{

// Variables
private  bool _IsDirty;
private  int _Version;
private  Skill.Framework.Managers.UpgradeItem[] _Items;

// Properties
/// <summary> is any changes happened to savable object </summary>
public   bool IsDirty { get { if(_IsDirty)  return _IsDirty;
if(_Items != null) {
for (int i = 0; i < _Items.Length; i++) {
if(_Items[i] != null && _Items[i].IsDirty) return true;
}
}
return _IsDirty;
 } }
public   int Version { get { return _Version; } set { if(_Version != value) {  _Version = value; _IsDirty = true; } } }
public   Skill.Framework.Managers.UpgradeItem[] Items { get { return _Items; } set { if(_Items != value) {  _Items = value; _IsDirty = true; } } }

// Methods
public    UpgradeItemList()
{
}
public   void SetAsClean()
{
_IsDirty = false;
if(_Items != null) {
for (int i = 0; i < _Items.Length; i++) {
if(_Items[i] != null)
_Items[i].SetAsClean();
}
}

}
public static  UpgradeItemList CreateUpgradeItemList()
{
return new UpgradeItemList();
}
public   void Save(Skill.Framework.IO.XmlElement e, Skill.Framework.IO.XmlSaveStream stream)
{
Skill.Framework.IO.XmlElement _VersionElement = stream.Create("Version",_Version);
e.AppendChild(_VersionElement);
Skill.Framework.IO.XmlElement _ItemsElement = stream.Create<Skill.Framework.Managers.UpgradeItem>("Items",_Items);
e.AppendChild(_ItemsElement);

}
public   void Save(Skill.Framework.IO.BinarySaveStream stream)
{
stream.Write(_Version);
stream.Write<Skill.Framework.Managers.UpgradeItem>(_Items);

}
public   void Load(Skill.Framework.IO.XmlElement e, Skill.Framework.IO.XmlLoadStream stream)
{
Skill.Framework.IO.XmlElement element = e.FirstChild as Skill.Framework.IO.XmlElement;
while (element != null)
{
switch (element.Name)
{
case "Version":
this._Version = stream.ReadInt( element );
break;
case "Items":
this._Items = stream.ReadSavableArray<Skill.Framework.Managers.UpgradeItem>( element , Skill.Framework.Managers.UpgradeItem.CreateUpgradeItem );
break;
}
element = e.GetNextSibling(element);
}
SetAsClean();

}
public   void Load(Skill.Framework.IO.BinaryLoadStream stream)
{
this._Version = stream.ReadInt();
this._Items = stream.ReadSavableArray<Skill.Framework.Managers.UpgradeItem>( Skill.Framework.Managers.UpgradeItem.CreateUpgradeItem );
SetAsClean();

}

}
}
