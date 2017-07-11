using System;
using System.Collections.Generic;
using Skill.Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a text field for entering floats.
    /// </summary>
    public class FloatField : EditorControl
    {
        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }

        /// <summary>
        /// will not return the new value until the user has pressed enter or focus is moved away from the float field.
        /// </summary>
        public bool Delayed { get; set; }

        /// <summary>
        /// Occurs when value of FloatField changed
        /// </summary>
        public event EventHandler ValueChanged;
        /// <summary>
        /// when value of FloatField changed
        /// </summary>
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
        }
                
        private float _Value;
        /// <summary>
        /// float - The value entered by the user.
        /// </summary>
        public float Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    OnValueChanged();
                }
            }
        }

        /// <summary>
        /// Create an instance of FloatField
        /// </summary>
        public FloatField()
        {
            Label = new GUIContent();
            this.Height = 16;            
        }

        /// <summary>
        /// Render FloatField
        /// </summary>
        protected override void Render()
        {            
            if (Style != null)
            {
                if (Delayed)
                    Value = EditorGUI.DelayedFloatField(RenderArea, Label, _Value, Style);
                else
                    Value = EditorGUI.FloatField(RenderArea, Label, _Value, Style);
            }
            else
            {
                if (Delayed)
                    Value = EditorGUI.DelayedFloatField(RenderArea, Label, _Value, Style);
                else
                    Value = EditorGUI.FloatField(RenderArea, Label, _Value);
            }
            //if (ChangeOnReturn)
            //{
            //    if (_Value != _ValueBeforChange)
            //    {
            //        Event e = Event.current;
            //        if (e != null && e.isKey)
            //        {
            //            if (e.keyCode == KeyCode.Return)
            //            {
            //                Value = _ValueBeforChange;
            //            }
            //            else if (e.keyCode == KeyCode.Escape)
            //            {
            //                _ValueBeforChange = _Value;
            //            }
            //        }
            //    }
            //}
        }
    }
}
