using System;
using System.Collections.Generic;
using Skill.Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a text field for entering ints.
    /// </summary>
    public class IntField : EditorControl
    {
        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }

        /// <summary>
        /// will not return the new value until the user has pressed enter or focus is moved away from the int field.
        /// </summary>
        public bool Delayed { get; set; }

        /// <summary>
        /// Occurs when Value of IntField changed
        /// </summary>
        public event EventHandler ValueChanged;
        /// <summary>
        /// when Value of IntField changed
        /// </summary>
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
        }

        private int _Value;
        /// <summary>
        /// int - The value entered by the user.
        /// </summary>
        public int Value
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
        /// Create an instance of IntField
        /// </summary>
        public IntField()
        {
            Label = new GUIContent();
            this.Height = 16;            
        }

        /// <summary>
        /// Render IntField
        /// </summary>
        protected override void Render()
        {
            if (Style != null)
            {
                if (Delayed)
                    Value = EditorGUI.DelayedIntField(RenderArea, Label, _Value, Style);
                else
                    Value = EditorGUI.IntField(RenderArea, Label, _Value, Style);
            }
            else
            {
                if (Delayed)
                    Value = EditorGUI.DelayedIntField(RenderArea, Label, _Value);
                else
                    Value = EditorGUI.IntField(RenderArea, Label, _Value);
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
