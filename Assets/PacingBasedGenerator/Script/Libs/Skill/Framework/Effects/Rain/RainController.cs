using UnityEngine;
using System.Collections;
using Skill.Framework;
namespace Skill.Framework.Effects
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Fading))]
    public class RainController : DynamicBehaviour
    {
        public static RainController Instance { get; private set; }

        public Light SceneLight;
        public RainLight RainLight;
        public ParticleSystem Particle;
        public SpecularController Specular;
        public float MaxEmission = 300;

        public float RainVolume = 0.5f;
        public Color RainSpecular;


        private TimeWatch _FadeTW;
        private bool _Starting;
        private bool _Stopping;
        private AudioSource _RainAudio;
        private Fading _Fading;

        protected override void GetReferences()
        {
            base.GetReferences();
            _RainAudio = GetComponent<AudioSource>();
            _Fading = GetComponent<Fading>();
        }

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
            if (RainLight.SceneLight == null)
                RainLight.SceneLight = this.SceneLight;
            RainLight.enabled = false;
            Particle.gameObject.SetActive(false);
            _Fading.Alpha = 1.0f;
            _RainAudio.volume = 0;
            _RainAudio.Stop();
        }

        private bool _IsRaining;
        public bool IsRaining { get { return _IsRaining; } }

        public void StartRain()
        {
            if (_IsRaining) return;
            _IsRaining = true;
            _RainAudio.Play();
            _Fading.FadeToZero();
            _Starting = true;
            _Stopping = false;
            RainLight.enabled = true;

            ParticleSystem.EmissionModule emission = Particle.emission;
            // emission.rate = new ParticleSystem.MinMaxCurve(0);
            emission.rateOverDistance = new ParticleSystem.MinMaxCurve(0);
            emission.enabled = true;

            Particle.gameObject.SetActive(true);
            Specular.SpecularColor = Specular.DefaultColor;
            _FadeTW.Begin(_Fading.FadeInTime + 0.1f);
        }

        public void StopRain()
        {
            if (!_IsRaining) return;
            _IsRaining = false;

            RainLight.enabled = false;
            _Fading.FadeToOne();
            _Starting = false;
            _Stopping = true;
            _FadeTW.Begin(_Fading.FadeOutTime + 0.1f);
            Specular.SpecularColor = RainSpecular;
        }


        protected override void Update()
        {
            base.Update();
            if (Global.Instance != null)
                _RainAudio.volume = (1.0f - _Fading.Alpha) * (Global.Instance.Settings.Audio.FxVolume * RainVolume);
            else
                _RainAudio.volume = (1.0f - _Fading.Alpha) * RainVolume;

            if (_FadeTW.IsEnabled)
            {
                ParticleSystem.EmissionModule emission = Particle.emission;
                if (_FadeTW.IsOver)
                {
                    _FadeTW.End();
                    if (_Starting)
                    {
                        emission.rateOverDistance = new ParticleSystem.MinMaxCurve(MaxEmission);
                        Specular.SpecularColor = RainSpecular;
                    }
                    if (_Stopping)
                    {
                        emission.rateOverDistance = new ParticleSystem.MinMaxCurve(0);
                        Particle.gameObject.SetActive(false);
                        Specular.SpecularColor = Specular.DefaultColor;
                        _RainAudio.Stop();
                    }

                    _Starting = false;
                    _Stopping = false;
                }
                else
                {
                    float percent = _FadeTW.Percent;
                    if (_Stopping)
                        percent = 1.0f - percent;

                    emission.rateOverDistance = new ParticleSystem.MinMaxCurve(Mathf.Lerp(0, MaxEmission, percent));
                    Specular.SpecularColor = Color.Lerp(Specular.DefaultColor, RainSpecular, percent);
                }
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Toggle")]
        public void ToggleRain()
        {
            if (Application.isPlaying)
            {
                if (_IsRaining)
                    StopRain();
                else
                    StartRain();
            }
            else
            {
                Debug.LogError("Toggle rain works only in play mode");
            }
        }

#endif
    }
}