using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PetalConfig
{
    public float RemoveAfter = 1.0f;
    public float Speed = 1f;

    public float BaseLineWidth = 0.2f;
    public AnimationCurve WidthOverTime;
    public Gradient ColorOverTime;

    public float Angle = 60f;
}
