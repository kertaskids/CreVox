using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Skill.Framework.ModernUI
{
    public class MusicBrowser : MonoBehaviour
    {
        public DynamicScrollView ScrollView;
        public GameObject Loading;
        public string Filter = "*.mp3";
        public string[] Roots = new string[] { "mnt" };
        public string CacheFileName = "AudioFiles.xml";
        public System.Threading.ThreadPriority Priority = System.Threading.ThreadPriority.Normal;
        public Color EvenBackColor = Color.white;
        public Color OddBackColor = Color.white;
        public bool UseFileName = true;
        public bool Log = true;

        private bool _IsScaning;
        private bool _BreakScan;
        private System.Threading.Thread _Thread;
        private bool _ScanBegined;
        private bool _ScanEnded;
        private bool _UpdateFilter;

        private List<FileList.FileData> _FileToAdd = new List<FileList.FileData>();
        private List<FileList.FileData> _Files = new List<FileList.FileData>();
        private List<string> _Logs = new List<string>();
        private List<string> _SelectedFiles = new List<string>();


        private string _ItemFilter;
        public string ItemFilter
        {
            get
            {
                if (_ItemFilter == null)
                    _ItemFilter = string.Empty;
                return _ItemFilter;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                value = value.ToLower();
                if (_ItemFilter != value)
                {
                    _ItemFilter = value;
                    _UpdateFilter = true;
                }
            }
        }

        private bool IsFilter(FileList.FileData fileData)
        {
            if (!string.IsNullOrEmpty(_ItemFilter) && !string.IsNullOrEmpty(fileData.Title))
            {
                if (fileData.Title.Contains(_ItemFilter))
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        public string[] SelectedFiles
        {
            get
            {
                return _SelectedFiles.ToArray();
            }
            set
            {
                _SelectedFiles.Clear();
                if (value != null && value.Length > 0)
                    _SelectedFiles.AddRange(value);

                for (int i = 0; i < ScrollView.Count; i++)
                {
                    GameObject obj = ScrollView.GetObject(i);
                    if (obj != null)
                    {
                        MusicListItem item = obj.GetComponent<MusicListItem>();
                        if (item != null)
                            item.IsChecked = _SelectedFiles.Contains(item.Path);
                    }
                }
            }
        }
        protected virtual void OnDestroy()
        {
            _BreakScan = true;
        }

        private void Initialize(GameObject obj, int index, object userData)
        {
            FileList.FileData fd = (FileList.FileData)userData;
            index = _Files.IndexOf(fd);
            MusicListItem item = obj.GetComponent<MusicListItem>();
            if (item != null)
            {
                item.Browser = this;
                item.UserData = userData;
                item.Path = fd.Path;
                if (item.Title != null)
                    item.Title.text = fd.Title;
                if (item.Artist != null)
                    item.Artist.text = fd.Artist;
                if (item.Background != null)
                {
                    if (index % 2 == 0)
                        item.Background.color = EvenBackColor;
                    else
                        item.Background.color = OddBackColor;
                }

                item.IsChecked = _SelectedFiles.Contains(item.Path);
            }
        }

        public void ItemSelected(object userData, bool selected)
        {
            FileList.FileData fd = (FileList.FileData)userData;
            if (selected)
            {
                if (!_SelectedFiles.Contains(fd.Path))
                    _SelectedFiles.Add(fd.Path);
            }
            else
                _SelectedFiles.Remove(fd.Path);
        }

        protected virtual void Awake()
        {
            ScrollView.Initialize = this.Initialize;
            if (Loading != null) Loading.SetActive(false);
            LoadFiles();
        }

        protected virtual void Update()
        {
            if (_ScanBegined)
            {
                _ScanBegined = false;
                if (Loading != null) Loading.SetActive(true);
            }
            if (_ScanEnded)
            {
                _ScanEnded = false;
                if (Loading != null)
                    Loading.SetActive(false);
                SaveFiles();
                if (_Logs.Count > 0)
                {
                    lock (_Logs)
                    {
                        foreach (var item in _Logs)
                        {
                            Debug.LogWarning(item);
                        }
                        _Logs.Clear();
                    }
                }
            }

            if (_FileToAdd.Count > 0)
            {
                lock (_FileToAdd)
                {
                    foreach (var item in _FileToAdd)
                    {
                        if (!IsFilter(item))
                            ScrollView.Add(item);
                    }
                    _FileToAdd.Clear();
                }
            }

            if (_UpdateFilter)
            {
                _UpdateFilter = false;
                lock (_Files)
                {
                    foreach (var f in _Files)
                    {
                        if (IsFilter(f))
                            ScrollView.Remove(f);
                        else if (!ScrollView.Contains(f))
                            ScrollView.Add(f);
                    }
                }
            }

        }
        // events
        /// <summary> Occurs when Select button clicked </summary>
        public event EventHandler Select;
        /// <summary> Occurs when Select button clicked </summary>
        protected virtual void OnSelect()
        {
            _BreakScan = true;
            if (Select != null) Select(this, EventArgs.Empty);
        }

        /// <summary> Occurs when Cancel button clicked </summary>
        public event EventHandler Cancel;
        /// <summary> Occurs when Cancel button clicked </summary>
        protected virtual void OnCancel()
        {
            _BreakScan = true;
            if (Cancel != null) Cancel(this, EventArgs.Empty);
        }

        public void CancelClick()
        {
            OnCancel();
        }

        public void SelectClick()
        {
            OnSelect();
        }

        protected virtual bool IsFileAccepted(FileAttributes att)
        {
            return ((att & FileAttributes.Hidden) == 0) && ((att & FileAttributes.System) == 0);
        }

        public void StopScane()
        {
            _BreakScan = true;
        }

        public void StartScan()
        {
            if (_IsScaning) return;
            _BreakScan = false;
            _FileToAdd.Clear();
            _Files.Clear();
            ScrollView.Clear();
            _Thread = new System.Threading.Thread(new System.Threading.ThreadStart(Scan));
            _Thread.Priority = Priority;
            _Thread.Start();
        }

        private bool IsFileExist(string fileName)
        {
            lock (_Files)
            {
                foreach (var f in _Files)
                {
                    if (f.Path == fileName)
                        return true;
                }
            }
            return false;
        }

        private FileList.FileData AddFile(string fileName)
        {
            FileList.FileData fd = new FileList.FileData();
            fd.Index = _Files.Count;
            fd.Path = fileName.ToLower();
            string title, artist;
            GetTitleAndArtist(fileName, out title, out artist);
            fd.Title = title;
            fd.Artist = artist;
            _Files.Add(fd);
            return fd;
        }

        public void Add(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;
            fileName = fileName.ToLower();
            lock (_FileToAdd)
            {
                if (!IsFileExist(fileName))
                {
                    FileList.FileData fd = AddFile(fileName);
                    _FileToAdd.Add(fd);
                }
            }
        }

        private void Scan3()
        {
            if (string.IsNullOrEmpty(Filter))
                Filter = "*.mp3";
            string[] filterItems = Filter.Split(';');

            if (filterItems.Length == 0)
                filterItems = new string[] { "*.mp3" };

            _ScanBegined = true;
            _IsScaning = true;

            foreach (var r in Roots)
            {
                if (_BreakScan) break;
                if (System.IO.Directory.Exists(r))
                {
                    try
                    {
                        for (int i = 0; i < filterItems.Length; i++)
                        {
                            if (_BreakScan) break;
                            DirectoryInfo directory = new DirectoryInfo(r);
                            FileInfo[] fileInfoes = directory.GetFiles(filterItems[i], SearchOption.AllDirectories);
                            if (fileInfoes != null)
                            {
                                lock (_FileToAdd)
                                {
                                    foreach (var f in fileInfoes)
                                    {
                                        if (_BreakScan) break;
                                        if (IsFileAccepted(f.Attributes) && !IsFileExist(f.FullName))
                                        {
                                            string fullName = f.FullName.ToLower();
                                            FileList.FileData fd = AddFile(fullName);
                                            _FileToAdd.Add(fd);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (Log)
                        {
                            lock (_Logs)
                            {
                                _Logs.Add(ex.ToString());
                            }
                        }
                    }
                }
            }
            _IsScaning = false;
            _ScanEnded = true;
        }

        private void Scan()
        {
            if (string.IsNullOrEmpty(Filter))
                Filter = "*.mp3";
            string[] filterItems = Filter.Split(';');

            if (filterItems.Length == 0)
                filterItems = new string[] { "*.mp3" };

            _ScanBegined = true;
            _IsScaning = true;
            Queue<string> directoriesQueue = new Queue<string>();
            foreach (var r in Roots)
            {
                if (System.IO.Directory.Exists(r))
                    directoriesQueue.Enqueue(r);
            }

            try
            {
                while (directoriesQueue.Count > 0)
                {
                    if (_BreakScan) break;

                    string dir = directoriesQueue.Dequeue();
                    try
                    {
                        DirectoryInfo directory = new DirectoryInfo(dir);
                        DirectoryInfo[] dirInfoes = directory.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
                        if (dirInfoes != null)
                        {
                            foreach (var d in dirInfoes)
                                directoriesQueue.Enqueue(d.FullName);
                        }
                    }
                    catch (Exception uaex)
                    {
                        if (Log)
                        {
                            lock (_Logs)
                            {
                                _Logs.Add(uaex.ToString());
                            }
                        }
                        continue;
                    }

                    try
                    {
                        for (int i = 0; i < filterItems.Length; i++)
                        {
                            DirectoryInfo directory = new DirectoryInfo(dir);
                            FileInfo[] fileInfoes = directory.GetFiles(filterItems[i], SearchOption.TopDirectoryOnly);
                            if (fileInfoes != null)
                            {
                                lock (_FileToAdd)
                                {
                                    foreach (var f in fileInfoes)
                                    {
                                        if (IsFileAccepted(f.Attributes) && !IsFileExist(f.FullName))
                                        {
                                            string fullName = f.FullName.ToLower();
                                            FileList.FileData fd = AddFile(fullName);
                                            _FileToAdd.Add(fd);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (Log)
                        {
                            lock (_Logs)
                            {
                                _Logs.Add(ex.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Log)
                {
                    lock (_Logs)
                    {
                        _Logs.Add(ex.ToString());
                        _Logs.Add(ex.StackTrace);
                    }
                }
            }
            _IsScaning = false;
            _ScanEnded = true;
        }

        private static char[] InvalidXmlCharacters = new char[] { '&', '<', '>', '"', '\'' };
        private static string[] ValidXmlCharacters = new string[] { "&amp;", "&lt;", "&gt;", "&quot;", "&apos;" };

        private void SaveFiles()
        {
            if (string.IsNullOrEmpty(CacheFileName))
                CacheFileName = "AudioFiles.xml";

            string path = System.IO.Path.Combine(Application.persistentDataPath, CacheFileName);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            FileList fl = new FileList();
            fl.Files = _Files.ToArray();

            // safe files for xml
            for (int i = 0; i < fl.Files.Length; i++)
            {
                fl.Files[i].Path = FixXml(fl.Files[i].Path);
                fl.Files[i].Title = FixXml(fl.Files[i].Title);
                fl.Files[i].Artist = FixXml(fl.Files[i].Artist);
            }

            Skill.Framework.IO.PCSaveGame.SaveToXmlFile(fl, path);
        }

        private string FixXml(string str)
        {
            str = str.Trim();
            if (str.IndexOfAny(InvalidXmlCharacters) >= 0)
            {
                for (int j = 0; j < InvalidXmlCharacters.Length; j++)
                {
                    if (str.IndexOf(InvalidXmlCharacters[j]) >= 0)
                        str = str.Replace(InvalidXmlCharacters[j].ToString(), ValidXmlCharacters[j]);
                }
            }
            return str;
        }

        private void LoadFiles()
        {
            if (string.IsNullOrEmpty(CacheFileName))
                CacheFileName = "AudioFiles.xml";

            string path = System.IO.Path.Combine(Application.persistentDataPath, CacheFileName);

            if (System.IO.File.Exists(path))
            {
                _FileToAdd.Clear();
                _Files.Clear();
                ScrollView.Clear();

                FileList fl = new FileList();
                Skill.Framework.IO.PCSaveGame.LoadFromXmlFile(fl, path);

                int index = 0;
                if (fl.Files != null)
                {
                    foreach (var f in fl.Files)
                    {
                        if (System.IO.File.Exists(f.Path))
                        {
                            f.Index = index++;
                            _Files.Add(f);
                            _FileToAdd.Add(f);
                        }
                    }
                }
            }
        }


        public virtual void GetTitleAndArtist(string filename, out string title, out string artist)
        {
            if (filename != null && System.IO.File.Exists(filename))
            {
                title = System.IO.Path.GetFileNameWithoutExtension(filename);
                artist = "unknown artis";

                using (FileStream fs = File.OpenRead(filename))
                {
                    MusicInfo info = GetMusicInfo(fs);
                    if (UseFileName)
                    {
                        if (!IsNullOrEmpty(info.Artist))
                            artist = info.Artist;
                        else if (!IsNullOrEmpty(info.Album))
                            artist = info.Album;
                        else if (!IsNullOrEmpty(info.Title))
                            artist = info.Title;
                    }
                    {
                        if (!IsNullOrEmpty(info.Title))
                            title = info.Title;
                        if (!IsNullOrEmpty(info.Artist))
                            artist = info.Artist;
                        else if (!IsNullOrEmpty(info.Album))
                            artist = info.Album;
                    }
                }
            }
            else
            {
                title = "unknown";
                artist = "unknown artis";
            }
        }

        public static MusicInfo GetMusicInfo(FileStream stream)
        {
            MusicInfo info = new MusicInfo();

            if (stream.Length >= 128)
            {
                MusicID3Tag tag = new MusicID3Tag();

                stream.Seek(-128, SeekOrigin.End);
                stream.Read(tag.TAGID, 0, tag.TAGID.Length);
                string theTAGID = Encoding.Default.GetString(tag.TAGID);

                if (theTAGID.Equals("TAG")) // version 1
                {
                    stream.Read(tag.Title, 0, tag.Title.Length);
                    stream.Read(tag.Artist, 0, tag.Artist.Length);
                    stream.Read(tag.Album, 0, tag.Album.Length);
                    stream.Read(tag.Year, 0, tag.Year.Length);
                    stream.Read(tag.Comment, 0, tag.Comment.Length);
                    stream.Read(tag.Genre, 0, tag.Genre.Length);

                    info.Title = Encoding.Default.GetString(tag.Title);
                    info.Artist = Encoding.Default.GetString(tag.Artist);
                    info.Album = Encoding.Default.GetString(tag.Album);
                    info.Year = Encoding.Default.GetString(tag.Year);
                    info.Comment = Encoding.Default.GetString(tag.Comment);
                    info.Genre = Encoding.Default.GetString(tag.Genre);

                }
            }

            if (IsNullOrEmpty(info.Title)) info.Title = string.Empty;
            if (IsNullOrEmpty(info.Artist)) info.Artist = string.Empty;
            if (IsNullOrEmpty(info.Album)) info.Album = string.Empty;
            if (IsNullOrEmpty(info.Year)) info.Year = string.Empty;
            if (IsNullOrEmpty(info.Comment)) info.Comment = string.Empty;
            if (IsNullOrEmpty(info.Genre)) info.Genre = string.Empty;

            return info;
        }

        public static bool IsNullOrEmpty(string str)
        {
            if (string.IsNullOrEmpty(str)) return true;
            for (int i = 0; i < str.Length; i++)
            {
                if (char.IsLetterOrDigit(str[i]) && !char.IsWhiteSpace(str[i]))
                    return false;
            }
            return true;
        }

        public class MusicInfo
        {
            public string Title = string.Empty;
            public string Artist = string.Empty;
            public string Album = string.Empty;
            public string Year = string.Empty;
            public string Comment = string.Empty;
            public string Genre = string.Empty;
        }

        class MusicID3Tag
        {
            public byte[] TAGID = new byte[3];      //  3
            public byte[] Title = new byte[30];     //  30
            public byte[] Artist = new byte[30];    //  30 
            public byte[] Album = new byte[30];     //  30 
            public byte[] Year = new byte[4];       //  4 
            public byte[] Comment = new byte[30];   //  30 
            public byte[] Genre = new byte[1];      //  1
        }



        [System.Serializable]
        public class FileList : Skill.Framework.IO.ISavable
        {

            // Internal class
            [System.Serializable]
            public class FileData : Skill.Framework.IO.ISavable
            {

                // Variables
                private bool _IsDirty;
                private int _Index;
                private string _Path;
                private string _Title;
                private string _Artist;

                // Properties
                /// <summary> is any changes happened to savable object </summary>
                public bool IsDirty
                {
                    get
                    {
                        return _IsDirty;
                    }
                }
                public int Index { get { return _Index; } set { if (_Index != value) { _Index = value; _IsDirty = true; } } }
                public string Path { get { return _Path; } set { if (_Path != value) { _Path = value; _IsDirty = true; } } }
                public string Title { get { return _Title; } set { if (_Title != value) { _Title = value; _IsDirty = true; } } }
                public string Artist { get { return _Artist; } set { if (_Artist != value) { _Artist = value; _IsDirty = true; } } }

                // Methods
                public FileData()
                {
                }
                public void SetAsClean()
                {
                    _IsDirty = false;

                }
                public static FileData CreateFileData()
                {
                    return new FileData();
                }
                public void Save(Skill.Framework.IO.XmlElement e, Skill.Framework.IO.XmlSaveStream stream)
                {
                    Skill.Framework.IO.XmlElement _IndexElement = stream.Create("Index", _Index);
                    e.AppendChild(_IndexElement);
                    Skill.Framework.IO.XmlElement _PathElement = stream.Create("Path", _Path);
                    e.AppendChild(_PathElement);
                    Skill.Framework.IO.XmlElement _TitleElement = stream.Create("Title", _Title);
                    e.AppendChild(_TitleElement);
                    Skill.Framework.IO.XmlElement _ArtistElement = stream.Create("Artist", _Artist);
                    e.AppendChild(_ArtistElement);

                }
                public void Save(Skill.Framework.IO.BinarySaveStream stream)
                {
                    stream.Write(_Index);
                    stream.Write(_Path);
                    stream.Write(_Title);
                    stream.Write(_Artist);

                }
                public void Load(Skill.Framework.IO.XmlElement e, Skill.Framework.IO.XmlLoadStream stream)
                {
                    Skill.Framework.IO.XmlElement element = e.FirstChild as Skill.Framework.IO.XmlElement;
                    while (element != null)
                    {
                        switch (element.Name)
                        {
                            case "Index":
                                this._Index = stream.ReadInt(element);
                                break;
                            case "Path":
                                this._Path = stream.ReadString(element);
                                break;
                            case "Title":
                                this._Title = stream.ReadString(element);
                                break;
                            case "Artist":
                                this._Artist = stream.ReadString(element);
                                break;
                        }
                        element = e.GetNextSibling(element);
                    }
                    SetAsClean();

                }
                public void Load(Skill.Framework.IO.BinaryLoadStream stream)
                {
                    this._Index = stream.ReadInt();
                    this._Path = stream.ReadString();
                    this._Title = stream.ReadString();
                    this._Artist = stream.ReadString();
                    SetAsClean();

                }

            }

            // Variables
            private bool _IsDirty;
            private FileData[] _Files;

            // Properties
            /// <summary> is any changes happened to savable object </summary>
            public bool IsDirty
            {
                get
                {
                    if (_IsDirty) return _IsDirty;
                    if (_Files != null)
                    {
                        for (int i = 0; i < _Files.Length; i++)
                        {
                            if (_Files[i] != null && _Files[i].IsDirty) return true;
                        }
                    }
                    return _IsDirty;
                }
            }
            public FileData[] Files { get { return _Files; } set { if (_Files != value) { _Files = value; _IsDirty = true; } } }

            // Methods
            public FileList()
            {
            }
            public void SetAsClean()
            {
                _IsDirty = false;
                if (_Files != null)
                {
                    for (int i = 0; i < _Files.Length; i++)
                    {
                        if (_Files[i] != null)
                            _Files[i].SetAsClean();
                    }
                }

            }
            public static FileList CreateFileList()
            {
                return new FileList();
            }
            public void Save(Skill.Framework.IO.XmlElement e, Skill.Framework.IO.XmlSaveStream stream)
            {
                Skill.Framework.IO.XmlElement _FilesElement = stream.Create<FileData>("Files", _Files);
                e.AppendChild(_FilesElement);

            }
            public void Save(Skill.Framework.IO.BinarySaveStream stream)
            {
                stream.Write<FileData>(_Files);

            }
            public void Load(Skill.Framework.IO.XmlElement e, Skill.Framework.IO.XmlLoadStream stream)
            {
                Skill.Framework.IO.XmlElement element = e.FirstChild as Skill.Framework.IO.XmlElement;
                while (element != null)
                {
                    switch (element.Name)
                    {
                        case "Files":
                            this._Files = stream.ReadSavableArray<FileData>(element, FileData.CreateFileData);
                            break;
                    }
                    element = e.GetNextSibling(element);
                }
                SetAsClean();

            }
            public void Load(Skill.Framework.IO.BinaryLoadStream stream)
            {
                this._Files = stream.ReadSavableArray<FileData>(FileData.CreateFileData);
                SetAsClean();

            }

        }

    }
}