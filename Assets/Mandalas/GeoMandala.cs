using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GeoMandala : MonoBehaviour
{
    public LineRenderer LR;
        
    [Range(3, 8)]
    public int PointCount = 4;
    private int LastPointCount = 0;

    [Range(-2f, 2f)]
    public float Ratio = 0.1f;

    public int Iterations = 2;
    private int LastIterations = 2;

    public GameObject[] CPS;

    public SignalGenerator RatioAnimation;

    // Start is called before the first frame update
    void Start()
    {
        SetInitialControlPointPosition();
    }

    void Check()
    {
        if (LastPointCount != PointCount || LastIterations != Iterations )
        {
            LR.positionCount = (PointCount + 1) * (Iterations + 1);

            UpdateControlPoints();
            SetInitialControlPointPosition();

            LastIterations = Iterations;
            LastPointCount = PointCount;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Check();

        /*
        if (Application.isPlaying )
            Ratio = RatioAnimation.GetValue(Time.time);
        */
        
        SetInitialShapeFromControlPoint();
        for (int i = 0; i < Iterations; i++)
            PerformIteration(i, Ratio );
    }

    private void UpdateControlPoints()
    {
        for (int i = 0; i < CPS.Length; i++)
            CPS[i].SetActive( i < PointCount);
    }

    private void SetInitialShapeFromControlPoint()
    {
        for (int s = 0; s < PointCount; s++)
            LR.SetPosition(s, CPS[s].transform.position );

        // close shape
        LR.SetPosition(PointCount, CPS[0].transform.position); 
    }

    private void SetInitialControlPointPosition()
    {
        float da = Mathf.PI * 2f / PointCount;
        float a = 0f;

        for( int s = 0; s<PointCount; s++ )
        {
            CPS[s].transform.position = new Vector3( Mathf.Cos(a), Mathf.Sin(a), 0f);
            a += da;
        }
    }

    private void PerformIteration( int IterationIndex, float IterationRatio )
    {
        int BaseIndex = 0;
        int TargetIndex = 0;

        for (int s = 0; s < PointCount; s++)
        {
            BaseIndex = IterationIndex * (PointCount + 1) + s;
            TargetIndex = BaseIndex + (PointCount + 1);
            // initial bit
            Vector3 R = LR.GetPosition(BaseIndex) + (LR.GetPosition(BaseIndex + 1) - LR.GetPosition(BaseIndex)) * IterationRatio;

            LR.SetPosition(TargetIndex, R);
        }
        LR.SetPosition(TargetIndex+1, LR.GetPosition(BaseIndex+2) );
    }
}
