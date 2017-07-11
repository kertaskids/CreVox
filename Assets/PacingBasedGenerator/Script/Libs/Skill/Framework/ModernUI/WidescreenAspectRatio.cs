using UnityEngine;
using System.Collections;

namespace Skill.Framework.ModernUI
{
    [RequireComponent(typeof(RectTransform))]
    public class WidescreenAspectRatio : MonoBehaviour
    {
        public float Aspect = 16.0f / 9.0f;
        public bool UseScale = true;

        private const float _WIDE1 = 16.0f / 9.0f;
        private const float _WIDE2 = 16.0f / 10.0f;

        private RectTransform _Transform;
        private Vector3 _InitialScale;
        Vector2 _InitialOffsetMin;
        Vector2 _InitialOffsetMax;
        private bool _Initialized;

        // Use this for initialization
        void OnEnable()
        {
            float aspect = (float)Screen.width / Screen.height;
            if (Mathf.Abs(aspect - _WIDE1) > 0.01f && Mathf.Abs(aspect - _WIDE2) > 0.01f)
            {
                if (_Transform == null)
                    _Transform = GetComponent<RectTransform>();

                if (_Transform != null)
                {
                    if (!_Initialized)
                    {
                        _Initialized = true;
                        _InitialScale = _Transform.localScale;
                        _InitialOffsetMin = _Transform.offsetMin;
                        _InitialOffsetMax = _Transform.offsetMax;
                    }

                    if (UseScale)
                    {
                        Vector3 scale = _InitialScale;
                        scale.y *= (aspect / Aspect);
                        _Transform.localScale = scale;
                    }
                    else
                    {
                        float offset = (Screen.height - (Screen.height * (aspect / Aspect))) * 0.5f;

                        Vector2 offsetMin = _InitialOffsetMin;
                        Vector2 offsetMax = _InitialOffsetMax;

                        offsetMin.y += offset;
                        offsetMax.y -= offset;

                        _Transform.offsetMin = offsetMin;
                        _Transform.offsetMax = offsetMax;
                    }
                }
            }
        }


    }
}