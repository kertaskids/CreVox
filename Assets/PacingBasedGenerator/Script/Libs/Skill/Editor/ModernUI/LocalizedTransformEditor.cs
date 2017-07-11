using UnityEngine;
using System.Collections;
using Skill.Framework.ModernUI;

namespace Skill.Editor.ModernUI
{
    [UnityEditor.CustomEditor(typeof(LocalizedTransform))]
    public class LocalizedTransformEditor : UnityEditor.Editor
    {

        private LocalizedTransform _Data;

        void OnEnable()
        {
            _Data = target as LocalizedTransform;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_Data.Positions != null && _Data.Positions.Length > 0)
            {
                UnityEditor.EditorGUILayout.BeginVertical();
                for (int i = 0; i < _Data.Positions.Length; i++)
                {
                    UnityEditor.EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Save for " + _Data.Positions[i].Language))
                        SaveFor(i);
                    if (GUILayout.Button("Restore for " + _Data.Positions[i].Language))
                        RestoreFor(i);
                    UnityEditor.EditorGUILayout.EndHorizontal();
                }
                UnityEditor.EditorGUILayout.EndVertical();
            }
        }

        private void SaveFor(int index)
        {
            RectTransform transform = _Data.GetComponent<RectTransform>();
            if (transform != null && _Data.Positions != null && _Data.Positions.Length > 0)
            {
                if (index >= 0 && index < _Data.Positions.Length)
                {
                    _Data.Positions[index].Pivot = transform.pivot;
                    _Data.Positions[index].AnchorMin = transform.anchorMin;
                    _Data.Positions[index].AnchorMax = transform.anchorMax;
                    _Data.Positions[index].OffsetMin = transform.offsetMin;
                    _Data.Positions[index].OffsetMax = transform.offsetMax;
                    UnityEditor.EditorUtility.SetDirty(_Data);
                }
            }
        }

        private void RestoreFor(int index)
        {
            RectTransform transform = _Data.GetComponent<RectTransform>();
            if (transform != null && _Data.Positions != null && _Data.Positions.Length > 0)
            {
                if (index >= 0 && index < _Data.Positions.Length)
                {
                    transform.pivot = _Data.Positions[index].Pivot;
                    transform.anchorMin = _Data.Positions[index].AnchorMin;
                    transform.anchorMax = _Data.Positions[index].AnchorMax;
                    transform.offsetMin = _Data.Positions[index].OffsetMin;
                    transform.offsetMax = _Data.Positions[index].OffsetMax;
                    UnityEditor.EditorUtility.SetDirty(transform);
                }
            }
        }
    }
}