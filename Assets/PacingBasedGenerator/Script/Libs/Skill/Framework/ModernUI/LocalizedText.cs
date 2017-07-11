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
    public class LocalizedText : MonoBehaviour
    {
        public string Key;
        public int DictionaryIndex = 0;
        public int FontIndex = 0;
        public bool CheckRTL = false;
        public bool InverseRTL = false;

        private UnityEngine.UI.Text _Text;
        private string _Language;
        private bool _Initialized;

        void Start()
        {
            _Text = GetComponent<UnityEngine.UI.Text>();
            _Initialized = true;
            UpdateText();
        }

        void OnEnable()
        {
            if (_Initialized)
                UpdateText();
        }

        public void UpdateText()
        {
            if (Localization.Instance != null && _Text != null)
            {
                if (string.IsNullOrEmpty(_Language) || _Language != Localization.Instance.SelectedLanguage.Name)
                {
                    if (!string.IsNullOrEmpty(this.Key))
                        _Text.text = Localization.Instance.GetText(this.Key, DictionaryIndex);

                    Font f = Localization.Instance.GetFont(FontIndex);
                    if (f != null)
                        _Text.font = f;
                    if (CheckRTL)
                    {
                        int alignment = (int)_Text.alignment / 3 * 3;
                        if (Localization.Instance.IsRightToLeft)
                        {
                            if (InverseRTL)
                                _Text.alignment = (TextAnchor)(alignment + 0);
                            else
                                _Text.alignment = (TextAnchor)(alignment + 2);
                        }
                        else
                        {
                            if (InverseRTL)
                                _Text.alignment = (TextAnchor)(alignment + 2);
                            else
                                _Text.alignment = (TextAnchor)(alignment + 0);
                        }
                    }

                    _Language = Localization.Instance.SelectedLanguage.Name;
                }
            }
        }

        /// <summary> Update all instance if LocalizedText in scene </summary>
        public static void UpdateAll()
        {
            LocalizedText[] allTexts = FindObjectsOfType<LocalizedText>();
            if (allTexts != null)
            {
                for (int i = 0; i < allTexts.Length; i++)
                    allTexts[i].UpdateText();
            }
        }
    }
}