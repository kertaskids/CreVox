using UnityEngine;
using System.Collections;

namespace Skill.Framework.Audio
{
    public class ButtonSound : MonoBehaviour
    {
        public int SoundIndex;

        // Use this for initialization
        void Start()
        {
            UnityEngine.UI.Button button = GetComponent<UnityEngine.UI.Button>();
            if (button != null)
                button.onClick.AddListener(Click);
        }


        private void Click()
        {
            ButtonSoundManager.Play(SoundIndex);
        }
    }
}
