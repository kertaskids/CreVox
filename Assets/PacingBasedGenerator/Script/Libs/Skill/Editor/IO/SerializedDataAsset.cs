using UnityEngine;
using System.Collections;

namespace Skill.Editor.IO
{
    /// <summary>
    /// Defines serializable data asset required for Implant tool
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewSaveData", menuName = "Skill/SerializedData", order = 45)]
    public class SerializedDataAsset : ScriptableObject
    {
        public string XmlData;        
        public string BuildPath = "Scripts/Designer";

        public SerializedData Load()
        {
            SerializedData saveData = null;
            if (!string.IsNullOrEmpty(XmlData))
            {
                try
                {
                    Skill.Framework.IO.XmlDocument document = new Framework.IO.XmlDocument();
                    document.LoadXml(XmlData);
                    saveData = new SerializedData();
                    saveData.Load(document.FirstChild);
                }
                catch (System.Exception ex)
                {
                    saveData = null;
                    Debug.LogException(ex, this);
                }
            }
            else
            {
                saveData = new SerializedData();                
                Save(saveData);
            }
            saveData.Name = this.name;
            return saveData;
        }

        public void Save(SerializedData saveData)
        {
            if (!this) return; // if deleted
            if (saveData == null) return;
            saveData.Name = this.name;
            Skill.Framework.IO.XmlDocument document = new Framework.IO.XmlDocument();
            document.AppendChild(saveData.ToXmlElement());
            XmlData = document.OuterXml;
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }

}
