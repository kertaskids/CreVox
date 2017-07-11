using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using Skill.Framework.UI;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a text field.
    /// </summary>
    public class TextField : EditorControl
    {
        /// <summary>
        /// Optional label to display in front of the text field.
        /// </summary>
        public GUIContent Label { get; private set; }
        
        private string _Text;
        /// <summary>
        /// The text to edit.
        /// </summary>
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                if (_Text != value)
                {
                    _Text = value;
                    OnTextChanged();
                }
            }
        }

        /// <summary>
        /// will not return the new value until the user has pressed enter or focus is moved away from the text field.
        /// </summary>
        public bool Delayed { get; set; }

        /// <summary>
        /// Occurs when Text of TextField changed
        /// </summary>
        public event EventHandler TextChanged;
        /// <summary>
        /// when Text of TextField changed
        /// </summary>
        protected virtual void OnTextChanged()
        {
            if (TextChanged != null) TextChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Create a TextField
        /// </summary>
        public TextField()
        {
            this.Label = new GUIContent();
            this._Text = string.Empty;
            this.Height = 16;            
        }

        /// <summary>
        /// Render TextField
        /// </summary>
        protected override void Render()
        {            
            if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);

            if (Style != null)
            {
                if (Delayed)
                    Text = EditorGUI.DelayedTextField(RenderArea, Label, _Text, Style);
                else
                    Text = EditorGUI.TextField(RenderArea, Label, _Text, Style);
            }
            else
            {
                if (Delayed)
                    Text = EditorGUI.DelayedTextField(RenderArea, Label, _Text);
                else
                    Text = EditorGUI.TextField(RenderArea, Label, _Text);
            }

            //if (ChangeOnReturn)
            //{
            //    if (_Text != _TextBeforChange)
            //    {
            //        Event e = Event.current;
            //        if (e != null && e.isKey)
            //        {
            //            if (e.keyCode == KeyCode.Return)
            //            {
            //                Text = _TextBeforChange;
            //            }
            //            else if (e.keyCode == KeyCode.Escape)
            //            {
            //                _TextBeforChange = _Text;
            //            }
            //        }
            //    }
            //}
        }

    }
}