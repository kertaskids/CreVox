using UnityEngine;
using System.Collections;
using Skill.Framework;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
#endif

namespace Skill.Framework.ModernUI
{
    [RequireComponent(typeof(RectTransform))]
    public class LocalizedTransform : MonoBehaviour
    {
        public TransformData[] Positions;

        [System.Serializable]
        public class TransformData
        {
            public string Language;
            public Vector2 Pivot;
            public Vector2 AnchorMin;
            public Vector2 AnchorMax;
            public Vector2 OffsetMin;
            public Vector2 OffsetMax;

        }

        private RectTransform _Transform;
        private TransformData _DefaultData;
        private string _Language;
        private bool _Initialized;

        void Start()
        {
            _Transform = GetComponent<RectTransform>();

            if (_Transform != null)
            {
                _DefaultData = new TransformData();
                Copy(_Transform, _DefaultData);
            }
            _Initialized = true;
            UpdateTransform();
        }

        void OnEnable()
        {
            if (_Initialized)
                UpdateTransform();
        }

        public void UpdateTransform()
        {
            if (Localization.Instance != null && _Transform != null)
            {
                if (string.IsNullOrEmpty(_Language) || _Language != Localization.Instance.SelectedLanguage.Name)
                {
                    _Language = Localization.Instance.SelectedLanguage.Name;

                    TransformData data = null;
                    for (int i = 0; i < Positions.Length; i++)
                    {
                        if (Positions[i].Language == _Language)
                        {
                            data = Positions[i];
                            break;
                        }
                    }

                    if (data != null)
                        Apply(data);
                    else if (_DefaultData != null)
                        Apply(_DefaultData);
                }
            }
        }

        private void Apply(TransformData data)
        {
            _Transform.pivot = data.Pivot;
            _Transform.offsetMin = data.OffsetMin;
            _Transform.offsetMax = data.OffsetMax;
            _Transform.anchorMin = data.AnchorMin;
            _Transform.anchorMax = data.AnchorMax;
        }

        /// <summary> Update all instance if LocalizedTransform in scene </summary>
        public static void UpdateAll()
        {
            LocalizedTransform[] allTransforms = FindObjectsOfType<LocalizedTransform>();
            if (allTransforms != null)
            {
                for (int i = 0; i < allTransforms.Length; i++)
                    allTransforms[i].UpdateTransform();
            }
        }


        private static void Copy(RectTransform transform, TransformData data)
        {
            if (transform != null)
            {
                data = new TransformData();
                data.AnchorMin = transform.anchorMin;
                data.AnchorMax = transform.anchorMax;
                data.Pivot = transform.pivot;
                data.OffsetMin = transform.offsetMin;
                data.OffsetMax = transform.offsetMax;
            }
        }

#if UNITY_EDITOR

        [ContextMenu("Find Languages")]
        public void FindLanguages()
        {
            string[] languages = Localization.GetLanguages();

            if (languages != null && languages.Length > 0)
            {
                List<TransformData> list = new List<TransformData>();
                if (Positions != null)
                {
                    for (int i = 0; i < Positions.Length; i++)
                        list.Add(Positions[i]);
                }

                for (int i = 0; i < languages.Length; i++)
                {
                    if (list.Count(t => t.Language == languages[i]) == 0)
                    {
                        TransformData data = new TransformData() { Language = languages[i] };
                        Copy(GetComponent<RectTransform>(), data);
                        list.Add(data);
                    }
                }

                Positions = list.ToArray();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }

#endif
    }
}