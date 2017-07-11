using System;
using System.Collections.Generic;
using Skill.Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a text field for entering doubles.
    /// </summary>
    public class DoubleField : EditorControl
    {
        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }

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

        private double _Value;
        /// <summary>
        /// float - The value entered by the user.
        /// </summary>
        public double Value
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
        public DoubleField()
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
                Value = EditorGUI.DoubleField(RenderArea, Label, _Value, Style);
            }
            else
            {
                Value = EditorGUI.DoubleField(RenderArea, Label, _Value);
            }           
        }
    }
}
