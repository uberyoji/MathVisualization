using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif


public class TreeGenerator : MonoBehaviour
{
    [Range(0, 90)]
    public float SeparationAngle;

    public int IterationAmount = 4;

    public float GrowthDelay = 1f;

    public float Distance;
    public float DistanceRatio = 0.8f;

    public TreeSeed ST;

    private Vector2 CurrPos = new Vector2();
    private Vector2 LastPos = new Vector2();
    //    private Vector2 DeltaPos = new Vector2();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LastPos.Set(Input.mousePosition.x / Camera.main.pixelWidth, (Camera.main.pixelHeight - Input.mousePosition.y) / Camera.main.pixelHeight);
            CurrPos.Set(Input.mousePosition.x / Camera.main.pixelWidth, (Camera.main.pixelHeight - Input.mousePosition.y) / Camera.main.pixelHeight);
        }

        if (Input.GetMouseButton(0))
        {
            CurrPos.Set(Input.mousePosition.x / Camera.main.pixelWidth, (Camera.main.pixelHeight - Input.mousePosition.y) / Camera.main.pixelHeight);

            ST.DistanceRatio = Mathf.Clamp01(1f-CurrPos.y);
            ST.SeparationAngle = Mathf.Abs(CurrPos.x - .5f) * 2f * 90;

//            Debug.LogFormat("DistanceRatio={0}, SeparationAngle={1}", ST.DistanceRatio, ST.SeparationAngle);

            ST.Propagate = true;
           
            LastPos.Set(CurrPos.x, CurrPos.y);
        }
    }
}




#if UNITY_EDITOR
[CustomEditor(typeof(TreeSeed))]
class TreeSeedEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TreeSeed F = (TreeSeed)target;

        if (GUILayout.Button("Update"))
        {
            F.Propagate = true;
        }
    }
}
#endif

