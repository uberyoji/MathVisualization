using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GeoMandala : MonoBehaviour
{
    public LineRenderer LR;

    private bool ForceUpdate = false;

    public int SideCount = 4;

    [Range(-2f, 2f)]
    public float Ratio = 0.1f;

    public int Iterations = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ForceUpdate)
        {
            Build();
            ForceUpdate = false;
        }
    }

    private void OnValidate()
    {
        Debug.Log("Something changed. Rebuilding for " + gameObject.name + ".");
        ForceUpdate = true;
    }

    private void SetInitialShape()
    {
        LR.positionCount = (SideCount + 1) * (Iterations+1);

        float da = Mathf.PI * 2f / SideCount;
        float a = 0f;

        for( int s = 0; s<SideCount+1; s++ )
        {
            LR.SetPosition(s, new Vector3( Mathf.Cos(a), Mathf.Sin(a), 0f));
            a += da;
        }        
    }

    private void PerformIteration( int IterationIndex )
    {
        int BaseIndex = 0;
        int TargetIndex = 0;

        for (int s = 0; s < SideCount; s++)
        {
            BaseIndex = IterationIndex * (SideCount + 1) + s;
            TargetIndex = BaseIndex + (SideCount + 1);
            // initial bit
            Vector3 R = LR.GetPosition(BaseIndex) + (LR.GetPosition(BaseIndex + 1) - LR.GetPosition(BaseIndex)) * Ratio;

            LR.SetPosition(TargetIndex, R);
        }
        LR.SetPosition(TargetIndex+1, LR.GetPosition(BaseIndex+2) );
    }

    private void Build()
    {
        SetInitialShape();

        for (int i = 0; i < Iterations; i++)
        {
            PerformIteration(i);
        }
    }
}
