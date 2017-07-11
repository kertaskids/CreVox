using UnityEngine;
using System.Collections;
using Skill.Framework;

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
#endif

namespace Skill.Framework
{
    [RequireComponent(typeof(Renderer))]
    public class LocalizedMaterial : MonoBehaviour
    {
        public MaterialData[] Materials;

        [System.Serializable]
        public class MaterialData
        {
            public string Language;
            public Material Material;
        }

        private Renderer _Renderer;
        private string _Language;
        private Material _DefaultMaterial;
        private bool _Initialized;

        void Start()
        {
            _Renderer = GetComponent<Renderer>();
            if (_Renderer != null)
                _DefaultMaterial = _Renderer.material;
            _Initialized = true;
            UpdateMaterial();
        }

        void OnEnable()
        {
            if (_Initialized)
                UpdateMaterial();
        }

        public void UpdateMaterial()
        {
            if (Localization.Instance != null && _Renderer != null)
            {
                if (string.IsNullOrEmpty(_Language) || _Language != Localization.Instance.SelectedLanguage.Name)
                {
                    _Language = Localization.Instance.SelectedLanguage.Name;

                    MaterialData data = null;
                    for (int i = 0; i < Materials.Length; i++)
                    {
                        if (Materials[i].Language == _Language)
                        {
                            data = Materials[i];
                            break;
                        }
                    }

                    Material mat = null;

                    if (data != null)
                        mat = data.Material;
                    else
                        mat = _DefaultMaterial;

                    if (_Renderer.material != mat)
                        _Renderer.material = mat;
                }
            }
        }

        /// <summary> Update all instance if LocalizedImage in scene </summary>
        public static void UpdateAll()
        {
            LocalizedMaterial[] allMaterials = FindObjectsOfType<LocalizedMaterial>();
            if (allMaterials != null)
            {
                for (int i = 0; i < allMaterials.Length; i++)
                    allMaterials[i].UpdateMaterial();
            }
        }




#if UNITY_EDITOR

        [ContextMenu("Find Languages")]
        public void FindLanguages()
        {
            string[] languages = Localization.GetLanguages();

            if (languages != null && languages.Length > 0)
            {
                List<MaterialData> list = new List<MaterialData>();
                if (Materials != null)
                {
                    for (int i = 0; i < Materials.Length; i++)
                        list.Add(Materials[i]);
                }

                for (int i = 0; i < languages.Length; i++)
                {
                    if (list.Count(t => t.Language == languages[i]) == 0)
                    {
                        list.Add(new MaterialData() { Language = languages[i] });
                    }
                }

                Materials = list.ToArray();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }


}