#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_XBOX360 || UNITY_PS3
#define SUPPORTMOVIE
#endif

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Fading))]
    public class SplashScreen : DynamicBehaviour
    {
        [System.Serializable]
        public class SplashImage
        {
            /// <summary> Optional Name </summary>
            public string Name = "Image";
            /// <summary> address of image in resources </summary>
            public string ImageResourcePath = null;
            /// <summary> Texture : it could be Texture2D or MovieTexture(Unity Pro) </summary>
            public Texture Image = null;
            /// <summary> ScaleMode of splash image</summary>
            public ScaleMode Scale = ScaleMode.ScaleToFit;

            /// <summary> Text to show </summary>
            public string Text = string.Empty;
            /// <summary> scale of font size </summary>
            public float FontScale = 0.2f;
            /// <summary> style of font </summary>
            public GUIStyle TextStyle;
            /// <summary> if true then use text as localization text key</summary>
            public bool LocalizedText = false;
            /// <summary> index of dictionary if localized is enable </summary>
            public int DictionaryIndex = 0;


            /// <summary> Size of splash relative to screen (0.0f- 1.0f)</summary>
            public float WidthPercent = 0.7f;
            /// <summary> Size of splash relative to screen (0.0f- 1.0f)</summary>
            public float HeightPercent = 0.7f;
            /// <summary> Duration of image </summary>
            public float MaxDuration = 4.0f;
            /// <summary> Allow escape slapsh after this time </summary>
            public float MinDuration = 4.0f;
            /// <summary> scale at end of show(start scale is 1.0f) </summary>
            public float EndScale = 1.0f;

            /// <summary> AudioClip to play when showing splash </summary>
            public AudioClip Clip;
            /// <summary> Volume of AudioClip </summary>
            public float Volume = 1.0f;
        }


        /// <summary> Texture : it could be Texture2D or MovieTexture(Unity Pro) </summary>
        public SplashImage[] Images;
        /// <summary> Texture to use for fading texture between each splash image (black) </summary>
        public Texture2D FadeTexture;
        /// <summary>fade tint color </summary>
        public Color FadeColor = Color.black;
        /// <summary> Level to load in background.(Unity Pro) </summary>
        public string LevelToLoad = null;
        /// <summary> Show movies in fullscreen size</summary>
        public bool FullScreenMovies = true;
        /// <summary>
        /// If true when user press escape immediately go to next splash and ignore fadeout
        /// </summary>
        public bool FastEscape = false;
        /// <summary>
        /// When AsyncOperation.progress reach this value we take it as load level process is completed
        /// </summary>
        /// <remarks>
        /// In my experience AsyncOperation.progress never reach 1.0f and i think this is a bug in unity,
        /// anyway if you think i am wrong or this bug is corrected set it to something more than 0.8f
        /// </remarks>
        public float MaxLoadProgress = 0.8f;
        public int GUIDepth = 10;

        private Frame _Frame;
        private Image _ImgSplash;
        private Label _LblSplash;
        private Image _ImgFadeTexture;
        private AsyncOperation _LoadingAsyncOperation;
        private TimeWatch _NextSplashTW;
        private TimeWatch _SplashTW;
        private Lerp _Scale;
        private int _CurrentSplashIndex;
        private Fading _Fading;
        private bool _IsCompleted;
        private DynamicFontSize _FontSize;

#if SUPPORTMOVIE
        private AudioSource _Audio;
#endif

        /// <summary> Occurs when SplashScreen is completed </summary>
        public event EventHandler Completed;
        /// <summary> Occurs when SplashScreen is completed </summary>
        protected virtual void OnCompleted()
        {
            if (_LoadingAsyncOperation != null && _LoadingAsyncOperation.progress >= MaxLoadProgress)
                _LoadingAsyncOperation.allowSceneActivation = true;
            if (Completed != null) Completed(this, EventArgs.Empty);
        }

        /// <summary>
        /// Get required references
        /// </summary>
        protected override void GetReferences()
        {
            base.GetReferences();
#if SUPPORTMOVIE
            _Audio = GetComponent<AudioSource>();
#endif
            _Fading = GetComponent<Fading>();
            _Fading.Alpha = 1.0f;
        }

        /// <summary>
        /// Start
        /// </summary>
        protected override void Start()
        {
            base.Start();
            Time.timeScale = 1;
            _FontSize = new DynamicFontSize();
            _Frame = new Frame("SplashFrame");

            for (int i = 0; i < 3; i++)
            {
                _Frame.Grid.RowDefinitions.Add(1, GridUnitType.Star);
                _Frame.Grid.ColumnDefinitions.Add(1, GridUnitType.Star);
            }

            _ImgSplash = new Image() { Row = 1, Column = 1 };
            _LblSplash = new Label() { Row = 1, Column = 1 };
            _ImgFadeTexture = new Image() { Row = 0, Column = 0, ColumnSpan = 3, RowSpan = 3, Texture = FadeTexture, TintColor = FadeColor, Scale = ScaleMode.StretchToFill, AlphaFading = _Fading };

            _Frame.Grid.Controls.Add(_ImgSplash);
            _Frame.Grid.Controls.Add(_LblSplash);
            _Frame.Grid.Controls.Add(_ImgFadeTexture);

            _CurrentSplashIndex = -1;
            ShowNext();
            LoadLevel();

            _Scale.SmoothStep = true;
        }


        private void SetFullScreen()
        {
            if (_Frame.Grid.RowDefinitions.Count != 1)
            {
                _Frame.Grid.RowDefinitions.Clear();
                _Frame.Grid.RowDefinitions.Add(1, GridUnitType.Star);
            }
            if (_Frame.Grid.ColumnDefinitions.Count != 1)
            {
                _Frame.Grid.ColumnDefinitions.Clear();
                _Frame.Grid.ColumnDefinitions.Add(1, GridUnitType.Star);
            }
            _ImgSplash.Column = _ImgSplash.Row = 0;
        }

        private void SetSize(float width, float height, ScaleMode scale)
        {
            if (_Frame.Grid.RowDefinitions.Count != 3)
            {
                _Frame.Grid.RowDefinitions.Clear();
                for (int i = 0; i < 3; i++)
                    _Frame.Grid.RowDefinitions.Add(1, GridUnitType.Star);
            }
            if (_Frame.Grid.ColumnDefinitions.Count != 3)
            {
                _Frame.Grid.ColumnDefinitions.Clear();
                for (int i = 0; i < 3; i++)
                    _Frame.Grid.ColumnDefinitions.Add(1, GridUnitType.Star);
            }

            _ImgSplash.Column = _ImgSplash.Row = 1;
            _LblSplash.Column = _LblSplash.Row = 1;

            float clampedSizeW = Mathf.Clamp01(width) * 100;
            _Frame.Grid.ColumnDefinitions[0].Width = _Frame.Grid.ColumnDefinitions[2].Width = new GridLength((100 - clampedSizeW) * 0.5f, GridUnitType.Star);
            _Frame.Grid.ColumnDefinitions[1].Width = new GridLength(clampedSizeW, GridUnitType.Star);

            float clampedSizeH = Mathf.Clamp01(height) * 100;
            _Frame.Grid.RowDefinitions[0].Height = _Frame.Grid.RowDefinitions[2].Height = new GridLength((100 - clampedSizeH) * 0.5f, GridUnitType.Star);
            _Frame.Grid.RowDefinitions[1].Height = new GridLength(clampedSizeH, GridUnitType.Star);

            _ImgSplash.Scale = scale;
        }

        private void LoadLevel()
        {
            if (!string.IsNullOrEmpty(LevelToLoad))
            {
                if (Application.CanStreamedLevelBeLoaded(LevelToLoad))
                {
                    _LoadingAsyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(LevelToLoad);
                    _LoadingAsyncOperation.allowSceneActivation = false;
                }
            }
        }


        private void ShowNext()
        {
            if (_CurrentSplashIndex >= 0 && _Fading != null)
            {
                // fadeout
                _Fading.FadeToOne();
                //prepare to change texture after fadeout                
                _NextSplashTW.Begin(_Fading.FadeOutTime + 0.2f);
            }
            else
                _NextSplashTW.Begin(0.2f);
        }


        /// <summary> Update </summary>
        protected override void Update()
        {
            if (_CurrentSplashIndex >= Images.Length) // all splash showed
            {
                if (!_IsCompleted)
                {
                    _IsCompleted = true;
                    OnCompleted();// raise Completed event
                }
            }
            else
            {
                if (_NextSplashTW.IsEnabled)
                {
                    // if user press escape button
                    // fadeout is complete and times to change splash
                    if ((FastEscape && Escape()) || _NextSplashTW.IsOver)
                    {
                        MoveNext();
                    }
                }
                else if (_SplashTW.IsEnabled)
                {
                    if (_SplashTW.IsOver) // times to fadeout and prepare to show next splash
                    {
                        _SplashTW.End();
                        ShowNext();
                    }
                    else if (_SplashTW.ElapsedTime > Images[_CurrentSplashIndex].MinDuration)
                    {
                        if (Escape())// if user press escape button
                        {
                            if (FastEscape)
                                MoveNext();
                            else
                                ShowNext();
                        }

                    }
                }

                _Frame.Update();
            }
            base.Update();
        }

        private void MoveNext()
        {
            _Scale.End();
            _NextSplashTW.End();
            // stop previous movie
#if SUPPORTMOVIE
            if (_CurrentSplashIndex >= 0)
            {
                if (_ImgSplash.Texture != null && _ImgSplash.Texture is MovieTexture)
                {
                    MovieTexture movie = (MovieTexture)_ImgSplash.Texture;
                    movie.Stop();
                }
            }
#endif

            if (_Audio != null && _Audio.clip != null)
            {
                _Audio.Stop();
                _Audio.clip = null;
            }

            _CurrentSplashIndex++;// go next splash
            if (_CurrentSplashIndex < Images.Length) // if another splash exist
            {
                if (Images[_CurrentSplashIndex].Image == null)
                {
                    if (!string.IsNullOrEmpty(Images[_CurrentSplashIndex].ImageResourcePath))
                        Images[_CurrentSplashIndex].Image = Resources.Load<Texture>(Images[_CurrentSplashIndex].ImageResourcePath);
                }

#if SUPPORTMOVIE

                if (Images[_CurrentSplashIndex].Image != null && Images[_CurrentSplashIndex].Image is MovieTexture)
                {
                    MovieTexture movie = (MovieTexture)Images[_CurrentSplashIndex].Image;
                    movie.Play();
                    if (_Audio != null && movie.audioClip != null)
                    {
                        _Audio.clip = movie.audioClip;
                        _Audio.Play();
                    }
                    if (FullScreenMovies)
                        SetSize(1.0f, 1.0f, Images[_CurrentSplashIndex].Scale);
                    else
                        SetSize(Images[_CurrentSplashIndex].WidthPercent, Images[_CurrentSplashIndex].HeightPercent, Images[_CurrentSplashIndex].Scale);

                    if (_Fading != null)
                    {
                        _Fading.FadeToZero(true);
                        _SplashTW.Begin(Mathf.Max(movie.duration - _Fading.FadeOutTime, Images[_CurrentSplashIndex].MaxDuration - _Fading.FadeOutTime, _Fading.FadeOutTime + 0.1f));
                    }
                    else
                        _SplashTW.Begin(Mathf.Max(movie.duration - 0.1f, Images[_CurrentSplashIndex].MaxDuration - 0.1f, 0.1f));
                }
                else
                {
#endif
                    SetSize(Images[_CurrentSplashIndex].WidthPercent, Images[_CurrentSplashIndex].HeightPercent, Images[_CurrentSplashIndex].Scale);
                    if (_Fading != null)
                    {
                        _Fading.FadeToZero(true);
                        _SplashTW.Begin(Mathf.Max(Images[_CurrentSplashIndex].MaxDuration - _Fading.FadeOutTime, _Fading.FadeOutTime + 0.1f));
                    }
                    else
                        _SplashTW.Begin(Mathf.Max(Images[_CurrentSplashIndex].MaxDuration - 0.1f, 0.1f));
#if SUPPORTMOVIE
                }
#endif

                _ImgSplash.Texture = Images[_CurrentSplashIndex].Image;// change texture
                if (!string.IsNullOrEmpty(Images[_CurrentSplashIndex].Text))
                {
                    _LblSplash.IsEnabled = true;
                    if (Images[_CurrentSplashIndex].LocalizedText)
                        _LblSplash.Text = Localization.Instance.GetText(Images[_CurrentSplashIndex].Text, Images[_CurrentSplashIndex].DictionaryIndex);
                    else
                        _LblSplash.Text = Images[_CurrentSplashIndex].Text;

                    _LblSplash.Style = Images[_CurrentSplashIndex].TextStyle;
                    UpdateFontScale(Images[_CurrentSplashIndex].FontScale);
                }
                else
                {
                    _LblSplash.Text = string.Empty;
                    _LblSplash.IsEnabled = false;
                }

                if (!Mathf.Approximately(Images[_CurrentSplashIndex].EndScale, 1.0f))
                    _Scale.Begin(1.0f, Images[_CurrentSplashIndex].EndScale, _SplashTW.Length + _Fading.FadeOutTime + 0.2f);

                if (Images[_CurrentSplashIndex].Clip != null)
                    _Audio.PlayOneShot(Images[_CurrentSplashIndex].Clip, Images[_CurrentSplashIndex].Volume);
            }
            else
            {
                if (_Fading != null) _Fading.Alpha = 1.0f;
                _ImgSplash.Texture = null;
                _LblSplash.Text = string.Empty;
            }
        }

        /// <summary>
        /// Allow subclass to define when user press escape 
        /// by defalut : Input.GetKeyDown(KeyCode.Escape)
        /// </summary>
        /// <returns></returns>
        protected virtual bool Escape()
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }

        /// <summary>
        /// OnGUI
        /// </summary>
        protected virtual void OnGUI()
        {
            GUI.depth = GUIDepth;

            if (_Scale.IsEnabled)
            {
                if (_Scale.IsOver)
                {
                    _Scale.End();
                    GUI.matrix = Matrix4x4.identity;
                }
                else
                {
                    SetGUIScale(_Scale.Value);
                }
            }
            else
                GUI.matrix = Matrix4x4.identity;

            _Frame.Position = new Rect(0, 0, Screen.width, Screen.height);
            _Frame.OnGUI();
        }

        private void UpdateFontScale(float scale)
        {
            if (_LblSplash.Style != null)
            {
                _FontSize.Remove(_LblSplash.Style);
                _FontSize.Add(_LblSplash.Style, scale);
                _FontSize.ForceUpdate();
            }
        }

        private void SetGUIScale(float scale)
        {
            GUIUtility.ScaleAroundPivot(new Vector2(scale, scale), Utility.ScreenRect.center);
        }
    }
}
