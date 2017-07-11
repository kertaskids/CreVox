using UnityEngine;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Skill.Editor
{
    class ManifestTools
    {

        private static string _Namespace = "";
        private static XmlDocument _Document = null;
        private static XmlNode _ManifestNode = null;
        private static XmlNode _ApplicationNode = null;

        public static string ManifestPath { get { return Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml"); } }

        public static void OpenManifest()
        {
            string path = ManifestPath;

            // only copy over a fresh copy of the AndroidManifest if one does not exist
            if (!File.Exists(path))
            {
                var inputFile = Path.Combine(UnityEditor.EditorApplication.applicationContentsPath, "PlaybackEngines/androidplayer/AndroidManifest.xml");
                File.Copy(inputFile, path);
            }

            _Document = new XmlDocument();
            _Document.Load(path);

            if (_Document == null)
            {
                Debug.LogError("Couldn't load " + path);
                return;
            }

            _ManifestNode = FindChildNode(_Document, "manifest");
            _Namespace = _ManifestNode.GetNamespaceOfPrefix("android");
            _ApplicationNode = FindChildNode(_ManifestNode, "application");

            if (_ApplicationNode == null)
            {
                Debug.LogError("Error parsing " + path);
                return;
            }

            SetPermission("android.permission.INTERNET");
        }

        public static void SaveManifest()
        {
            _Document.Save(ManifestPath);
        }

        public static void AddActivity(string activityName, Dictionary<string, string> attributes)
        {
            AppendApplicationElement("activity", activityName, attributes);
        }

        public static void RemoveActivity(string activityName)
        {
            RemoveApplicationElement("activity", activityName);
        }

        public static void SetPermission(string permissionName)
        {
            PrependManifestElement("uses-permission", permissionName);
        }

        public static void RemovePermission(string permissionName)
        {
            RemoveManifestElement("uses-permission", permissionName);
        }

        public static void RemoveMetaDataTag(string mdName)
        {
            RemoveElement("meta-data", mdName, _ApplicationNode);
        }

        public static void RemoveMetaDataTag(string mdName, XmlNode parent)
        {
            RemoveElement("meta-data", mdName, parent);
        }

        public static XmlElement AppendApplicationElement(string tagName, string name, Dictionary<string, string> attributes)
        {
            return AppendElementIfMissing(tagName, name, attributes, _ApplicationNode);
        }

        public static void RemoveApplicationElement(string tagName, string name)
        {
            RemoveElement(tagName, name, _ApplicationNode);
        }

        public static XmlElement PrependManifestElement(string tagName, string name)
        {
            return PrependElementIfMissing(tagName, name, null, _ManifestNode);
        }

        public static void RemoveManifestElement(string tagName, string name)
        {
            RemoveElement(tagName, name, _ManifestNode);
        }

        public static XmlElement AddMetaDataTag(string mdName, string mdValue)
        {
            return AppendApplicationElement("meta-data", mdName, new Dictionary<string, string>() {
                                                                        { "value", mdValue }
                                                                    });
        }

        public static XmlElement AddMetaDataTag(string mdName, string mdValue, XmlNode parent)
        {
            return AppendElementIfMissing("meta-data", mdName, new Dictionary<string, string>() { { "value", mdValue } }, parent);
        }

        public static XmlElement AppendElementIfMissing(string tagName, string name, Dictionary<string, string> otherAttributes, XmlNode parent)
        {
            XmlElement e = null;
            if (!string.IsNullOrEmpty(name))
            {
                e = FindElementWithTagAndName(tagName, name, parent);
            }

            if (e == null)
            {
                e = _Document.CreateElement(tagName);
                if (!string.IsNullOrEmpty(name))
                {
                    e.SetAttribute("name", _Namespace, name);
                }

                parent.AppendChild(e);
            }

            if (otherAttributes != null)
            {
                foreach (string key in otherAttributes.Keys)
                {
                    e.SetAttribute(key, _Namespace, otherAttributes[key]);
                }
            }

            return e;
        }

        public static XmlElement PrependElementIfMissing(string tagName, string name, Dictionary<string, string> otherAttributes, XmlNode parent)
        {
            XmlElement e = null;
            if (!string.IsNullOrEmpty(name))
            {
                e = FindElementWithTagAndName(tagName, name, parent);
            }

            if (e == null)
            {
                e = _Document.CreateElement(tagName);
                if (!string.IsNullOrEmpty(name))
                {
                    e.SetAttribute("name", _Namespace, name);
                }

                parent.PrependChild(e);
            }

            if (otherAttributes != null)
            {
                foreach (string key in otherAttributes.Keys)
                {
                    e.SetAttribute(key, _Namespace, otherAttributes[key]);
                }
            }

            return e;
        }

        public static void RemoveElement(string tagName, string name, XmlNode parent)
        {
            XmlElement e = FindElementWithTagAndName(tagName, name, parent);
            if (e != null)
            {
                parent.RemoveChild(e);
            }
        }

        public static XmlElement FindActivity(string name)
        {
            return FindElementWithTagAndName("activity", name, _ApplicationNode);
        }

        public static XmlNode FindChildNode(XmlNode parent, string tagName)
        {
            XmlNode curr = parent.FirstChild;
            while (curr != null)
            {
                if (curr.Name.Equals(tagName))
                {
                    return curr;
                }
                curr = curr.NextSibling;
            }
            return null;
        }

        public static XmlElement FindChildElement(XmlNode parent, string tagName)
        {
            XmlNode curr = parent.FirstChild;
            while (curr != null)
            {
                if (curr.Name.Equals(tagName))
                {
                    return curr as XmlElement;
                }
                curr = curr.NextSibling;
            }
            return null;
        }

        public static XmlElement FindElementWithTagAndName(string tagName, string name, XmlNode parent)
        {
            var curr = parent.FirstChild;
            while (curr != null)
            {
                if (curr.Name.Equals(tagName) && curr is XmlElement && ((XmlElement)curr).GetAttribute("name", _Namespace) == name)
                {
                    return curr as XmlElement;
                }
                curr = curr.NextSibling;
            }
            return null;
        }
    }
}