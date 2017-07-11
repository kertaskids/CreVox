using UnityEngine;
using System.Collections;
using Skill.Framework;

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
#endif

namespace Skill.Framework.ModernUI
{
    [RequireComponent(typeof(UnityEngine.UI.Text))]
    public class LocalizedTextAlignment : MonoBehaviour
    {
        public TextAlignmentData[] Alignmens;

        [System.Serializable]
        public class TextAlignmentData
        {
            public string Language;
            public TextAnchor Anchor;
        }


        private UnityEngine.UI.Text _Text;
        private string _Language;
        private bool _Initialized;
        private TextAnchor _DefaultAlignmen;

        void Start()
        {
            _Text = GetComponent<UnityEngine.UI.Text>();
            if (_Text != null)
                _DefaultAlignmen = _Text.alignment;
            _Initialized = true;
            UpdateAlignment();
        }

        void OnEnable()
        {
            if (_Initialized)
                UpdateAlignment();
        }

        public void UpdateAlignment()
        {
            if (Localization.Instance != null && _Text != null)
            {
                if (string.IsNullOrEmpty(_Language) || _Language != Localization.Instance.SelectedLanguage.Name)
                {
                    _Language = Localization.Instance.SelectedLanguage.Name;

                    TextAlignmentData data = null;
                    for (int i = 0; i < Alignmens.Length; i++)
                    {
                        if (Alignmens[i].Language == _Language)
                        {
                            data = Alignmens[i];
                            break;
                        }
                    }

                    if (data != null)
                        _Text.alignment = data.Anchor;
                    else
                        _Text.alignment = _DefaultAlignmen;
                }
            }
        }

        /// <summary> Update all instance if LocalizedText in scene </summary>
        public static void UpdateAll()
        {
            LocalizedTextAlignment[] allTexts = FindObjectsOfType<LocalizedTextAlignment>();
            if (allTexts != null)
            {
                for (int i = 0; i < allTexts.Length; i++)
                    allTexts[i].UpdateAlignment();
            }
        }


#if UNITY_EDITOR

        [ContextMenu("Find Languages")]
        public void FindLanguages()
        {
            string[] languages = Localization.GetLanguages();

            if (languages != null && languages.Length > 0)
            {
                List<TextAlignmentData> list = new List<TextAlignmentData>();
                if (Alignmens != null)
                {
                    for (int i = 0; i < Alignmens.Length; i++)
                        list.Add(Alignmens[i]);
                }

                _Text = GetComponent<UnityEngine.UI.Text>();
                for (int i = 0; i < languages.Length; i++)
                {
                    if (list.Count(t => t.Language == languages[i]) == 0)
                    {
                        list.Add(new TextAlignmentData() { Language = languages[i], Anchor = _Text.alignment });
                    }
                }

                Alignmens = list.ToArray();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}