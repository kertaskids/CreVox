using UnityEditor;
using UnityEngine;
using System;
using Skill.Editor.UI;
using Skill.Framework.UI;
using Skill.Editor.Curve;

namespace PBGWindow {
    public class CurveEditorWindow : EditorWindow {
        public static bool IsOpen { get { return Instance != null; } }
        
        private static Vector2 Size = new Vector2(800, 300);
        private static CurveEditorWindow _Instance;
        public static CurveEditorWindow Instance {
            get {
                if (_Instance == null)
                    _Instance = ScriptableObject.CreateInstance<CurveEditorWindow>();
                return _Instance;
            }
        }

        private EditorFrame _Frame;
        private CurveEditor _CurveEditor;
        private Skill.Framework.UI.Grid _PnlLeft;
        private TreeView _CurveTreeView;
        private ObjectField<GameObject> _ObjectField;
        private Skill.Editor.UI.GridSplitter _GridSplitter;
        //private CurvePresetLibrary _PresetPanel;

        [SerializeField]
        private GameObject _Object;
        public GameObject Object {
            get { return _Object; }
            set {
                if (_Object != value) {
                    _Object = value;
                    Rebuild();
                }
            }
        }
        public CurveEditorWindow() {
            // hideFlags = HideFlags.DontSave;
            // titleContent = new GUIContent("Curve Editor");
            base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = new Vector2(Size.x, Size.y);

            CreateUI();
        }
        void OnAwake() {
            hideFlags = HideFlags.DontSave;
        }
        void OnGUI() {
            UpdateStyles();
            _Frame.OnGUI();
        }
        void OnDestroy() {
            _Instance = null;
        }
        void OnFocus() {
            if (_Frame != null) {
                _ObjectField.Object = _Object;
                Rebuild();
            }
        }
        private void Rebuild() {
            _CurveTreeView.Controls.Clear();
            _CurveEditor.Selection.Clear();
            _CurveEditor.RemoveAllCurves();
            if (_Object != null) {
                Component[] components = _Object.GetComponents<Component>();
                foreach (var c in components) {
                    AddCurves(c);
                }
            }
        }
        private void AddCurves(Component component) {
            CurveEditor.EditCurveInfo[] curves = CurveEditor.GetCurves(component);
            if (curves != null && curves.Length > 0) {
                FolderView folder = new FolderView();
                folder.Foldout.Content.text = component.GetType().Name;
                folder.Foldout.IsOpen = true;

                foreach (var c in curves) {
                    CurveTrack track = _CurveEditor.AddCurve(c.GetCurve(), c.Attribute.Color);
                    CurveTrackTreeViewItem item = new CurveTrackTreeViewItem(track, c);
                    folder.Controls.Add(item);
                }
                _CurveTreeView.Controls.Add(folder);
            }
        }

        private void CreateUI() {
            _Frame = new EditorFrame("Frame", this);

            _Frame.Grid.ColumnDefinitions.Add(224, Skill.Framework.UI.GridUnitType.Pixel);// _PnlLeft
            _Frame.Grid.ColumnDefinitions[0].MinWidth = 224;
            _Frame.Grid.ColumnDefinitions.Add(2, Skill.Framework.UI.GridUnitType.Pixel); // _GridSplitter
            _Frame.Grid.ColumnDefinitions.Add(5, Skill.Framework.UI.GridUnitType.Star);  // _CurveEditor        

            _PnlLeft = new Skill.Framework.UI.Grid() { Row = 0, Column = 0 };
            _PnlLeft.RowDefinitions.Add(24, Skill.Framework.UI.GridUnitType.Pixel); // _ObjectField
            _PnlLeft.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); // _CurveTreeView        
            //_PnlLeft.RowDefinitions.Add(26, Skill.Framework.UI.GridUnitType.Pixel); // _PresetPanel

            _ObjectField = new ObjectField<GameObject>() { Row = 0, Column = 0, VerticalAlignment = Skill.Framework.UI.VerticalAlignment.Center };
            _CurveTreeView = new TreeView() { Row = 1, Column = 0 };
            _CurveTreeView.DisableFocusable();
           // _PresetPanel = new CurvePresetLibrary() { Row = 2, Column = 0 };

            _PnlLeft.Controls.Add(_ObjectField);
            _PnlLeft.Controls.Add(_CurveTreeView);
           // _PnlLeft.Controls.Add(_PresetPanel);


            _GridSplitter = new Skill.Editor.UI.GridSplitter() { Row = 0, Column = 1, Orientation = Skill.Framework.UI.Orientation.Vertical };
            _CurveEditor = new CurveEditor() { Row = 0, Column = 2 };

            _Frame.Controls.Add(_PnlLeft);
            _Frame.Controls.Add(_GridSplitter);
            _Frame.Controls.Add(_CurveEditor);

            _ObjectField.ObjectChanged += _ObjectField_ObjectChanged;
           // _PresetPanel.PresetSelected += _PresetPanel_PresetSelected;
        }

        void _ObjectField_ObjectChanged(object sender, System.EventArgs e) {
            Object = _ObjectField.Object;
        }
        /*void _PresetPanel_PresetSelected(object sender, EventArgs e) {
            if (_PresetPanel.Preset != null && _CurveTreeView.SelectedItem != null && _CurveTreeView.SelectedItem is CurveTrackTreeViewItem) {
                CurveTrackTreeViewItem item = (CurveTrackTreeViewItem)_CurveTreeView.SelectedItem;
                while (item.Track.Curve.length > 0)
                    item.Track.Curve.RemoveKey(item.Track.Curve.length - 1);
                for (int i = 0; i < _PresetPanel.Preset.Keys.Length; i++)
                    item.Track.Curve.AddKey(_PresetPanel.Preset.Keys[i]);
                item.Track.RebuildKeys();
            }
        }*/
        private void UpdateStyles() {
            if (_GridSplitter.Style == null) {
                _GridSplitter.Style = Skill.Editor.Resources.Styles.VerticalSplitter;
            }
        }
        
        class CurveTrackTreeViewItem : Grid {
            private Skill.Editor.UI.ToggleButton _tbVisible;
            private Skill.Framework.UI.Label _labelName;
            private Skill.Editor.UI.ColorField _cvColor;

            public CurveTrack Track { get; private set; }
            public CurveEditor.EditCurveInfo Info { get; private set; }

            public CurveTrackTreeViewItem(CurveTrack curveTrack, CurveEditor.EditCurveInfo info) {
                this.Track = curveTrack;
                this.Info = info;
                this.ColumnDefinitions.Add(20, GridUnitType.Pixel);
                this.ColumnDefinitions.Add(1, GridUnitType.Star);
                this.ColumnDefinitions.Add(36, GridUnitType.Pixel);

                _tbVisible = new Skill.Editor.UI.ToggleButton() { Column = 0, IsChecked = curveTrack.Visibility == Skill.Framework.UI.Visibility.Visible };
                _labelName = new Label() { Column = 1, Text = info.Attribute.Name };
                _cvColor = new Skill.Editor.UI.ColorField() { Column = 2, Color = curveTrack.Color };

                this.Controls.Add(_tbVisible);
                this.Controls.Add(_labelName);
                this.Controls.Add(_cvColor);

                _tbVisible.Changed += TbVisibleChanged;
                _cvColor.ColorChanged += CvColorChanged;
            }

            void CvColorChanged(object sender, EventArgs e) {
                Track.Color = _cvColor.Color;
            }
            void TbVisibleChanged(object sender, EventArgs e) {
                Track.Visibility = _tbVisible.IsChecked ? Skill.Framework.UI.Visibility.Visible : Skill.Framework.UI.Visibility.Hidden;
            }
        }
    }   
}