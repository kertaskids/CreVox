using UnityEngine;
using System.Collections;
using System;
namespace Skill.Framework.ModernUI
{
    public class OptionBox : MonoBehaviour
    {
        // content to show
        public string[] Options;
        public UnityEngine.UI.Text OptionLabel;
        /// <summary> Whether cycle through options</summary>
        public bool Loop = false;

        /// <summary>
        /// if this option is enable, then options used as keys to get text from localization
        /// </summary>
        public bool Localized = false;
        /// <summary>
        /// index of dictionary if localized is enable
        /// </summary>
        public int DictionaryIndex = 0;

        protected virtual void OnEnable()
        {
            if (Options != null && Options.Length > 0)
                UpdateLabel();
        }


        private int _SelectedIndex;
        /// <summary> Gets or sets index of selected option </summary>
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                if (value < 0) value = 0;
                else if (value >= Options.Length) value = Options.Length - 1;
                if (_SelectedIndex != value)
                {
                    _SelectedIndex = value;
                    UpdateLabel();
                    OnSelectionChanged();
                }
            }
        }

        protected void UpdateLabel()
        {
            if (Localized && Localization.Instance != null)
                OptionLabel.text = Localization.Instance.GetText(Options[_SelectedIndex], DictionaryIndex);
            else
                OptionLabel.text = Options[_SelectedIndex];
        }

        /// <summary> Gets or sets selected option </summary>
        public string SelectedOption
        {
            get { return Options[_SelectedIndex]; }
            set
            {
                if (value == null) throw new System.ArgumentNullException("Invalid option. SelectedOption can not be null");
                int selectedIndex = -1;
                for (int i = 0; i < Options.Length; i++)
                {
                    if (Options[i] == value)
                    {
                        selectedIndex = i;
                        break;
                    }
                }
                if (selectedIndex == -1) throw new System.ArgumentNullException("Invalid option. Option does not exist");
                SelectedIndex = selectedIndex;
            }
        }

        /// <summary>
        /// Occurs when SelectedOption changed
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        /// Occurs when SelectedOption changed
        /// </summary>
        protected virtual void OnSelectionChanged()
        {
            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
        }
        protected virtual void Start()
        {
            UpdateLabel();
        }
        public void Left()
        {
            if (_SelectedIndex > 0)
            {
                SelectedIndex--;
            }
            else if (Loop)
            {
                SelectedIndex = Options.Length - 1;
            }
        }
        public void Right()
        {
            if (_SelectedIndex < Options.Length - 1)
            {
                SelectedIndex++;
            }
            else if (Loop)
            {
                SelectedIndex = 0;
            }
        }
    }
}