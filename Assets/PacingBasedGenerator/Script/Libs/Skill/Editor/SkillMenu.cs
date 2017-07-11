using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor
{
    class SkillMenu : ScriptableObject
    {
        // ********************************* Editor Windows ********************************* 
        [UnityEditor.MenuItem("Pacing Generator/Skill/Curve Editor", false, 5)]
        static void ShowCurveEditorWindow()
        {
            Skill.Editor.Curve.CurveEditorWindow.Instance.Show();
        }
    }
}
