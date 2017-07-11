using UnityEngine;
using System.Collections;


namespace Skill.Framework.Modules
{
    public class PlatformObject : MonoBehaviour
    {
        public GameObject Editor;
        public PlatformData[] Platforms;

        [System.Serializable]
        public class PlatformData
        {
            [HideInInspector]
            public string Name;
            public RuntimePlatform Platform;
            public GameObject Object;
        }

        void OnValidate()
        {
            foreach (var p in Platforms)
                p.Name = p.Platform.ToString();
        }

        void Awake()
        {
            bool found = false;
            var platform = Application.platform;
            foreach (var p in Platforms)
            {
                if (p.Platform == platform)
                {
                    Active(p.Object);
                    found = true;
                }
                else
                    Deactive(p.Object);
            }
            Active(Editor, !found);
        }


        private void Active(GameObject go) { Active(go, true); }
        private void Deactive(GameObject go) { Active(go, false); }
        private void Active(GameObject go, bool value) { if (go != null) go.SetActive(value); }
    }
}
