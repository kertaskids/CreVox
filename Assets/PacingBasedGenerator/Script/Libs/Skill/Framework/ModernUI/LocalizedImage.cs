using UnityEngine;
using System.Collections;
using Skill.Framework;

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
#endif

namespace Skill.Framework.ModernUI
{
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class LocalizedImage : MonoBehaviour
    {
        public Sprite Default;
        public ImageData[] Images;

        [System.Serializable]
        public class ImageData
        {
            public string Language;
            public Sprite Image;
        }

        private UnityEngine.UI.Image _Image;
        private string _Language;
        private bool _Initialized;

        void Start()
        {
            _Image = GetComponent<UnityEngine.UI.Image>();
            _Initialized = true;
            UpdateImage();
        }

        void OnEnable()
        {
            if (_Initialized)
                UpdateImage();
        }

        public void UpdateImage()
        {
            if (Localization.Instance != null && _Image != null)
            {
                if (string.IsNullOrEmpty(_Language) || _Language != Localization.Instance.SelectedLanguage.Name)
                {
                    _Language = Localization.Instance.SelectedLanguage.Name;

                    ImageData data = null;
                    for (int i = 0; i < Images.Length; i++)
                    {
                        if (Images[i].Language == _Language)
                        {
                            data = Images[i];
                            break;
                        }
                    }

                    if (data != null)
                        _Image.sprite = data.Image;
                    else
                        _Image.sprite = Default;
                }
            }
        }

        /// <summary> Update all instance if LocalizedImage in scene </summary>
        public static void UpdateAll()
        {
            LocalizedImage[] allImages = FindObjectsOfType<LocalizedImage>();
            if (allImages != null)
            {
                for (int i = 0; i < allImages.Length; i++)
                    allImages[i].UpdateImage();
            }
        }




#if UNITY_EDITOR

        [ContextMenu("Find Languages")]
        public void FindLanguages()
        {
            string[] languages = Localization.GetLanguages();

            if (languages != null && languages.Length > 0)
            {
                List<ImageData> list = new List<ImageData>();
                if (Images != null)
                {
                    for (int i = 0; i < Images.Length; i++)
                        list.Add(Images[i]);
                }

                for (int i = 0; i < languages.Length; i++)
                {
                    if (list.Count(t => t.Language == languages[i]) == 0)
                    {
                        list.Add(new ImageData() { Language = languages[i], Image = Default });
                    }
                }

                Images = list.ToArray();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }


}