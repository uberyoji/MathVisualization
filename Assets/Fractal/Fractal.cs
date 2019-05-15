using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Fractal : MonoBehaviour
{
    public LineRenderer LR;

    [Range(3,8)]
    public int SeedObjectCount = 3;
    private int LastSeedObjectCount = 0;
    public GameObject[] SeedObjects;
    private List<Vector3> SeedPositions = new List<Vector3>();
    private List<Vector3> SeedVectors = new List<Vector3>();

    [Range(0, 5)]
    public int IterationCount = 0;
    private int LastIterationCount = -1;

    private List<Vector3> Positions;

    public void ResetSeed()
    {
        SeedPositions.Clear();
        for ( int i=0; i<SeedObjectCount; i++)
            SeedPositions.Add(SeedObjects[i].transform.position);

        float Length = (SeedPositions[SeedPositions.Count - 1] - SeedPositions[0]).magnitude;

        SeedVectors.Clear();
        for (int Index = 0; Index < SeedPositions.Count - 1; Index++)
        {
            SeedVectors.Add((SeedPositions[Index + 1] - SeedPositions[0]) / Length);
        }

        Positions = new List<Vector3>(SeedPositions);
    }

    Vector3 Transform(Vector3 From, Vector3 To)
    {
        return new Vector3((From.x * To.x) + (From.y * -To.y),
                            (From.x * To.y) + (From.y * To.x),
                            0f);
    }

    List<Vector3> Spread(List<Vector3> Positions)
    {
        List<Vector3> Output = new List<Vector3>();

        for (int PosIndex = 0; PosIndex < Positions.Count - 1; PosIndex++)
        {
            Vector3 PosVec = Positions[PosIndex + 1] - Positions[PosIndex];

            Output.Add(Positions[PosIndex]);

            for (int SeedIndex = 0; SeedIndex < SeedPositions.Count - 2; SeedIndex++)
            {
                Output.Add(Positions[PosIndex] + Transform(PosVec, SeedVectors[SeedIndex]));
            }
        }

        Output.Add(Positions[Positions.Count - 1]);

        return Output;
    }

    public void UpdateLineRenderer()
    {
        LR.positionCount = Positions.Count;
        LR.SetPositions(Positions.ToArray());
    }

    public void ApplyIterations()
    {
        ResetSeed();

        for (int Count = 0; Count < IterationCount; Count++)
            Positions = Spread(Positions);
    }

    // Use this for initialization
    void Start()
    {
        UpdateControlPoints();
        SetInitialControlPointPosition();

        ResetSeed();
        UpdateLineRenderer();
    }

    private void SetInitialControlPointPosition()
    {
        float da = Mathf.PI * 2f / SeedObjectCount;
        float a = 0f;

        for (int s = 0; s < SeedObjectCount; s++)
        {
            SeedObjects[s].transform.position = new Vector3( -0.5f + s / 2.0f * SeedObjectCount, Mathf.Sin(a), 0f);
            a += da;
        }
    }

    private void UpdateControlPoints()
    {
        for (int i = 0; i < SeedObjects.Length; i++)
            SeedObjects[i].SetActive(i < SeedObjectCount);
    }

    // Update is called once per frame
    void Update()
    {
        //if (LastIterationCount != IterationCount)
        {
            LastIterationCount = IterationCount;

            ApplyIterations();

            // UpdateLineRenderer();
        }

        if( LastSeedObjectCount != SeedObjectCount)
        {
            LastSeedObjectCount = SeedObjectCount;

            UpdateControlPoints();
            SetInitialControlPointPosition();
        }

        UpdateLineRenderer();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Fractal))]
class FractalEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Fractal F = (Fractal)target;

        if (GUILayout.Button("Apply Iterations"))
        {
            F.ApplyIterations();
            F.UpdateLineRenderer();
        }

        if (GUILayout.Button("Update Line Renderer"))
        {
            F.ResetSeed();
            F.UpdateLineRenderer();
        }
    }
}
#endif
