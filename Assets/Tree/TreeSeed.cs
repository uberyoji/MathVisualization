using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Inspiration: https://generativelandscapes.wordpress.com/2014/10/07/fractal-trees-basic-l-system-example-9-4/

// TODO: Add variant system
// TODO: Variant: 3 Branches (trident)
// TODO: Variant: 1 angled + 1straight followed by 2 angled
// TODO: Variant: 1 straight + 1 alternating angled
// TODO: Add noise on separation angle
// TODO: Add noise on distance ratio

public class BranchData
{
    public GameObject GO;
    public TreeSeed TS;
    public Quaternion Rot = new Quaternion();
}

public class TreeSeed : MonoBehaviour
{
    [Range(0,90)]
    public float SeparationAngle;

    [HideInInspector]
    public int Iteration = 0;
    public int MaxIterationAmount = 8;

    public int SplitIndex = 0;

    public float GrowthDelay = 1f;

    public float Distance;
    public float DistanceRatio = 0.8f;
    
    private float StartGrowthTime = 0f;
    private float EndGrowthTime = 0f;
    
    public float LineWidth = 0.5f;
    public float LineWidthRatio = 0.5f;

    public int Variant = 0;

    public LineRenderer LR;

    [HideInInspector]
    public BranchData[] Branches = new[] { new BranchData(), new BranchData() };

    /*
    private GameObject BranchA;
    private GameObject BranchB;

    private Quaternion RotA = new Quaternion();
    private Quaternion RotB = new Quaternion();
    */

    public GameObject Prefab;

    public bool Dynamic = false;

    public Gradient Color;

    private GradientColorKey[] ColorKeys = new GradientColorKey[2];
    private GradientAlphaKey[] AlphaKeys = new GradientAlphaKey[2];
    private Gradient BranchGradient = new Gradient(); 

    private Transform ParentTransform;

    /*
    [HideInInspector]
    public TreeSeed BranchTSA;

    [HideInInspector]
    public TreeSeed BranchTSB;
    */

    [HideInInspector]
    public bool Propagate = false;

    // Start is called before the first frame update
    void Start()
    {
        StartGrowthTime = Time.time;
        EndGrowthTime = Time.time + GrowthDelay;

        if (ParentTransform==null)
            ParentTransform = transform.parent;

        LR.SetPosition(0, ParentTransform.position);

        LR.startWidth = LineWidth;
        LR.endWidth = LineWidth * LineWidthRatio;
        
        UpdateColorGradient();
    }

    /*
    void UpdateBranchesFromParent()
    { 
        foreach( var B in Branches )
        {
            UpdateBranch(B);
        }
    }
    */

    float GetDeltaAngle( int Type, TreeSeed Branch )
    {
        switch( Type )
        {
            case 0: //  1 left, 1 right
                return -Branch.SeparationAngle + 2 * Branch.SeparationAngle * Branch.SplitIndex;
            case 1: //  1 straight + 1 alternating angled
                return Branch.SeparationAngle * ( -1 + 2 * (Branch.Iteration % 2) ) * Branch.SplitIndex;
            case 2: //  1 straight + 1 alternating angled
                if ( Branch.Iteration % 2 == 0 )
                {
                    return -Branch.SeparationAngle + 2 * Branch.SeparationAngle * Branch.SplitIndex;
                }
                else
                {
                    return Branch.SeparationAngle * (-1 + 2 * (Branch.Iteration % 3)) * Branch.SplitIndex;
                }
            case 3: // v split with alternating separation
                return (-Branch.SeparationAngle + 2 * Branch.SeparationAngle * Branch.SplitIndex) * ( 0.5f + (Branch.Iteration % 2 ) * 0.5f );
            case 4: // spread separation across branches
                float Angle = Branch.SeparationAngle * Branch.Iteration / Branch.MaxIterationAmount;
                return (-Angle + 2 * Angle * Branch.SplitIndex);

        }
        return 0f;
    }

    void UpdateBranch( BranchData B )
    {
        UpdateBranchFromParent(B.TS, B.Rot, GetDeltaAngle( Variant, B.TS ) );
    }

    void UpdateBranchFromParent( TreeSeed Branch, Quaternion Rot, float DeltaAngle )
    {
        // propagate data
        Branch.DistanceRatio = DistanceRatio;
        Branch.SeparationAngle = SeparationAngle;
        Branch.MaxIterationAmount = MaxIterationAmount;
        Branch.Iteration = Iteration + 1;
        Branch.Distance = Distance * DistanceRatio;
        Branch.LineWidth = LineWidth * LineWidthRatio;
        Branch.Color = Color;
        Branch.Variant = Variant;

        Rot.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + DeltaAngle);
        Branch.transform.rotation = Rot;

        // deactivate gameobjects beyond max iteration
        if (Iteration < MaxIterationAmount && Branch.gameObject.activeSelf == false)
            Branch.gameObject.SetActive(true);

        if (Iteration >= MaxIterationAmount && Branch.gameObject.activeSelf == true)
            Branch.gameObject.SetActive(false);
    }

    void UpdateColorGradient()
    {
        float KeyS = Iteration / (float)(MaxIterationAmount);        
        float KeyE = (Iteration + 1) / (float)(MaxIterationAmount);

        AlphaKeys[0].time = 0f; AlphaKeys[0].alpha = Color.Evaluate(KeyS).a;
        AlphaKeys[1].time = 1f; AlphaKeys[1].alpha = Color.Evaluate(KeyE).a;

        ColorKeys[0].time = 0f; ColorKeys[0].color = Color.Evaluate(KeyS);
        ColorKeys[1].time = 1f; ColorKeys[1].color = Color.Evaluate(KeyE);

        BranchGradient.SetKeys(ColorKeys, AlphaKeys);
        LR.colorGradient = BranchGradient;
    }

    // Update is called once per frame
    void Update()
    {
        if( Propagate )
        {
            if( Iteration < MaxIterationAmount-1)
            {
                foreach (var B in Branches)
                {
                    if (B.TS)
                    {
                        B.TS.Propagate = true;
                        UpdateBranch(B);
                    }
                }                
            }
            
            LR.SetPosition(0, ParentTransform.position);
            LR.SetPosition(1, transform.position);

            UpdateColorGradient();

            Propagate = false;
        }

        if ( Time.time < EndGrowthTime )
        {
            transform.position = ParentTransform.position + transform.right * Distance * (Time.time-StartGrowthTime) / GrowthDelay;
            LR.SetPosition(1, transform.position);
        }
        else
        {
            transform.position = ParentTransform.position + transform.right * Distance;
            LR.SetPosition(1, transform.position);

            if ( Iteration < MaxIterationAmount - 1)
            {
                for( int i = 0; i < Branches.Length; i++ )
                {
                    if(Branches[i].GO == null )
                    {
                        Branches[i].GO = GameObject.Instantiate(Prefab, transform.position, Branches[i].Rot);
                        Branches[i].TS = Branches[i].GO.GetComponent<TreeSeed>();
                        Branches[i].TS.ParentTransform = transform;
                        Branches[i].TS.SplitIndex = i;
                        UpdateBranch(Branches[i]);
                    }                   
                }
            }    
        }        
    }
}
