using UnityEditor;
using System.Collections;
namespace Skill.Editor
{
    public class SkillAssetModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {

            if (PaintColorEditor.Instance != null)
                PaintColorEditor.Instance.SaveIfChanged();


            //UnityEngine.Debug.Log("OnWillSaveAssets");
            //foreach (string path in paths)
            //    UnityEngine.Debug.Log(path);

            return paths;
        }
    }
}