using UnityEngine;
using System.Collections;


namespace Skill.Framework.ModernUI
{
    [RequireComponent(typeof(UnityEngine.UI.Slider))]
    [ExecuteInEditMode]
    public class DynamicHandleSize : MonoBehaviour
    {
        public float ScreenFactor = 0.2f;
        public float ScreenWidthFactor = 1.0f;
        public float ScreenHeightFactor = 1.0f;

        private ScreenSizeChange _ScreenSizeChange;
        private float _PreScreenWidthFactor;
        private float _PreScreenHeightFactor;
        private float _PreScreenFactor;
        private UnityEngine.UI.Slider _Slider;
        void Awake()
        {
            _Slider = GetComponent<UnityEngine.UI.Slider>();
            if (_Slider == null)
                throw new MissingComponentException("DynamicHandleSize needs a UnityEngine.UI.Slider component");
        }

        // Update is called once per frame
        void Update()
        {
            if (_ScreenSizeChange.IsChanged ||
                _PreScreenFactor != ScreenFactor ||
                _PreScreenWidthFactor != ScreenWidthFactor ||
                _PreScreenHeightFactor != ScreenHeightFactor)
            {
                _PreScreenFactor = ScreenFactor;
                _PreScreenWidthFactor = ScreenWidthFactor;
                _PreScreenHeightFactor = ScreenHeightFactor;
                if (_Slider == null)
                    _Slider = GetComponent<UnityEngine.UI.Slider>();
                if (_Slider != null && _Slider.handleRect != null)
                {
                    float factor = (((Screen.width * ScreenWidthFactor) + (Screen.height * ScreenHeightFactor)) * 0.1f) * ScreenFactor;
                    Vector2 sizeDelta = _Slider.handleRect.sizeDelta;
                    if (_Slider.direction == UnityEngine.UI.Slider.Direction.LeftToRight || _Slider.direction == UnityEngine.UI.Slider.Direction.RightToLeft)
                        sizeDelta.x = factor;
                    else
                        sizeDelta.y = factor;
                    _Slider.handleRect.sizeDelta = sizeDelta;
                }
            }
        }
    }
}
