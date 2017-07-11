using UnityEngine;
using System.Collections;
namespace Skill.Framework
{
    public class AndroidLogger : MonoBehaviour
    {
        public bool LogApplicationMessage = true;
        public bool OverrideFile = true;
        private static AndroidLogger _Instance;

        /// <summary>
        /// Awake
        /// </summary>
        void Awake()
        {
            if (_Instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _Instance = this;
                DontDestroyOnLoad(this.gameObject);
                if (LogApplicationMessage)
                    Application.logMessageReceived += Application_logMessageReceived;
            }
        }

        void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            Log(type.ToString(), "     condition : " + condition, "       stackTrace : " + stackTrace);
        }

        void OnDestroy()
        {
            if (_Instance == this)
                _Instance = null;

            if (LogApplicationMessage)
                Application.logMessageReceived -= Application_logMessageReceived;
        }


        private static string _FileName;
        private static string GetFileName()
        {
            if (string.IsNullOrEmpty(_FileName))
            {

                if (_Instance == null || _Instance.OverrideFile)
                    _FileName = Application.persistentDataPath + "/Log.txt";
                else
                {
                    string path = Application.persistentDataPath + "/";
                    string name = "Log";
                    int index = 1;
                    while (true)
                    {
                        _FileName = string.Format("{0}{1}{2}.txt", path, name, index);
                        if (!System.IO.File.Exists(_FileName))
                            break;
                        else
                            index++;
                    }
                }
            }
            return _FileName;
        }

        static int _Counter = 0;
        public static void Log(params string[] logs)
        {

            string fileName = GetFileName();

            if (_Counter == 0)
            {
                if (System.IO.File.Exists(fileName))
                    System.IO.File.Delete(fileName);
            }
            _Counter++;
            var stream = System.IO.File.AppendText(fileName);
            foreach (var s in logs)
            {
                if (s != null)
                    stream.WriteLine(s);
            }
            stream.Close();
        }
    }
}