using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSeed : MonoBehaviour
{
    [Range(0,90)]
    public float SeparationAngle;

    [HideInInspector]
    public int Iteration = 0;
    public int MaxIterationAmount = 8;

    public float GrowthDelay = 1f;

    public float Distance;
    public float DistanceRatio = 0.8f;
    
    private float StartGrowthTime = 0f;
    private float EndGrowthTime = 0f;
    
    public float LineWidth = 0.5f;
    public float LineWidthRatio = 0.5f;

    public LineRenderer LR;

    private GameObject BranchA;
    private GameObject BranchB;

    private Quaternion RotA = new Quaternion();
    private Quaternion RotB = new Quaternion();

    public GameObject Prefab;

    public bool Dynamic = false;

    public Gradient Color;

    private GradientColorKey[] ColorKeys = new GradientColorKey[2];
    private GradientAlphaKey[] AlphaKeys = new GradientAlphaKey[2];
    private Gradient BranchGradient = new Gradient(); 

    private Transform ParentTransform;

    [HideInInspector]
    public TreeSeed BranchTSA;

    [HideInInspector]
    public TreeSeed BranchTSB;

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
        
        RotA.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + SeparationAngle );
        RotB.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - SeparationAngle);

        UpdateColorGradient();
    }

    void UpdateBranchesFromParent()
    { 
        UpdateBranchFromParentA();
        UpdateBranchFromParentB();
    }

    void UpdateBranchFromParentA()
    {
        RotA.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + SeparationAngle);

        BranchTSA.transform.rotation = RotA;

        BranchTSA.DistanceRatio = DistanceRatio;
        BranchTSA.SeparationAngle = SeparationAngle;
        BranchTSA.MaxIterationAmount = MaxIterationAmount;
        BranchTSA.Iteration = Iteration + 1;
        BranchTSA.Distance = Distance * DistanceRatio;
        BranchTSA.LineWidth = LineWidth * LineWidthRatio;
        BranchTSA.Color = Color;

        if (Iteration < MaxIterationAmount && BranchTSA.gameObject.activeSelf == false)
            BranchTSA.gameObject.SetActive(true);

        if (Iteration >= MaxIterationAmount && BranchTSA.gameObject.activeSelf == true)
            BranchTSA.gameObject.SetActive(false);
    }
    void UpdateBranchFromParentB()
    {
        RotB.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - SeparationAngle);

        BranchTSB.transform.rotation = RotB;

        BranchTSB.DistanceRatio = DistanceRatio;
        BranchTSB.SeparationAngle = SeparationAngle;
        BranchTSB.MaxIterationAmount = MaxIterationAmount;
        BranchTSB.Iteration = Iteration + 1;
        BranchTSB.Distance = Distance * DistanceRatio;
        BranchTSB.LineWidth = LineWidth * LineWidthRatio;
        BranchTSB.Color = Color;

        if (Iteration < MaxIterationAmount && BranchTSB.gameObject.activeSelf == false)
            BranchTSB.gameObject.SetActive(true);
        if (Iteration >= MaxIterationAmount && BranchTSB.gameObject.activeSelf == true)
            BranchTSB.gameObject.SetActive(false);
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
            if(BranchTSA && Iteration < MaxIterationAmount-1)
            {
                BranchTSA.Propagate = true;
                UpdateBranchFromParentA();
            }                
            if(BranchTSB && Iteration < MaxIterationAmount-1)
            {
                BranchTSB.Propagate = true;
                UpdateBranchFromParentB();
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

            if ( BranchA == null && Iteration < MaxIterationAmount-1)
            {
                BranchA = GameObject.Instantiate(Prefab, transform.position, RotA);
                BranchTSA = BranchA.GetComponent<TreeSeed>();
                BranchTSA.ParentTransform = transform;
                UpdateBranchFromParentA();
            }
            if (BranchB == null && Iteration < MaxIterationAmount-1)
            {
                BranchB = GameObject.Instantiate(Prefab, transform.position, RotB);                
                BranchTSB = BranchB.GetComponent<TreeSeed>();
                BranchTSB.ParentTransform = transform;
                UpdateBranchFromParentB();
            }            
        }        
    }
}
