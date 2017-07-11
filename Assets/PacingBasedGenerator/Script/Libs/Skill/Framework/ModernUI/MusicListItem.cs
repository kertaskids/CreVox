using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
namespace Skill.Framework.ModernUI
{
    public class MusicListItem : MonoBehaviour
    {
        public UnityEngine.UI.Toggle Checkmark;
        public UnityEngine.UI.Text Title;
        public UnityEngine.UI.Text Artist;
        public UnityEngine.UI.Image Background;        


        void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            Checkmark.onValueChanged.AddListener(Check_Changed);
        }

        public RectTransform RectTransform { get; private set; }
        public bool IsChecked { get { return Checkmark.isOn; } set { Checkmark.isOn = value; } }                
        public string Path { get; set; }
        public object UserData { get; set; }
        public MusicBrowser Browser { get; set; }

        private void Check_Changed(bool isChecked)
        {
            Browser.ItemSelected(UserData, IsChecked);
        }
    }
}