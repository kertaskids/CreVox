using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skill.Framework;

public class PacingCurveTest : MonoBehaviour {
    [CurveEditor(1, 0, 0, "Threat")]
    public AnimationCurve Curve1;
    [CurveEditor(0, 1, 0, "Impetus")]
    public AnimationCurve Curve2;
    [CurveEditor(0, 0, 1, "Tempo")]
    public AnimationCurve Curve3;

}
