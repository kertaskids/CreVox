using UnityEngine;
using System.Collections;

namespace Skill.Framework.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class ButtonSoundManager : MonoBehaviour
    {
        public SoundCategory Category = SoundCategory.FX;
        public float VolumeScale= 1.0f;
        public AudioClip[] Clips;

        private static ButtonSoundManager _Instance;
        private AudioSource _Audio;

        void Awake()
        {
            _Instance = this;
            _Audio = GetComponent<AudioSource>();
        }
        void OnDestroy()
        {
            if (_Instance == this)
                _Instance = null;
        }


        public static void Play(int index)
        {
            if (_Instance != null && _Instance._Audio != null)
            {
                if (index >= 0 && index < _Instance.Clips.Length)
                {
                    if (Global.Instance != null)
                        Global.Instance.PlayOneShot(_Instance._Audio, _Instance.Clips[index], Skill.Framework.Audio.SoundCategory.FX, _Instance.VolumeScale);
                    else
                        _Instance._Audio.PlayOneShot(_Instance.Clips[index], _Instance.VolumeScale);
                }
            }
        }
    }
}