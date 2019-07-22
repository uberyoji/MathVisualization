using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOfLifeCube : MonoBehaviour
{
    public int GridDimension = 64;

    public int RuleA, RuleB, RuleC, RuleD = 0;

    public float Frequency = 60f;
    public float CubeAnimationScale = .25f;

    public float InitialSeedChance = .5f;

    public int GotoGeneration = 0;

    public Gradient Color;
    
    // naive implementation
    private bool[,,,] Cells;

    private int Src = 0;
    private int Dst = 1;
    private int GenerationCounter = 0;

    private Dictionary<int,Material> CubeMats = new Dictionary<int, Material>();

    public bool RandomizeSeed = false;

    // Start is called before the first frame update
    void Start()
    {
        GridDimension = URLParameters.GetSearchParameters().GetInt("size", GridDimension);
        RuleA = URLParameters.GetSearchParameters().GetInt("a", RuleA);
        RuleB = URLParameters.GetSearchParameters().GetInt("b", RuleB);
        RuleC = URLParameters.GetSearchParameters().GetInt("c", RuleC);
        RuleD = URLParameters.GetSearchParameters().GetInt("d", RuleD);

        int Randomize = 0;
        Randomize = URLParameters.GetSearchParameters().GetInt("randomize", Randomize);
        RandomizeSeed = Randomize != 0;

        InitialSeedChance = (float)URLParameters.GetSearchParameters().GetDouble("seed", InitialSeedChance);

        Frequency = (float)URLParameters.GetSearchParameters().GetDouble("freq", Frequency);

        Cells = new bool[2,GridDimension, GridDimension,GridDimension];

        SetInitialSeed();

        InitCells();
        UpdateCellMaterial();
    }

    private void SetInitialSeed()
    {
        if (RandomizeSeed)
        {
            for (int z = 1; z < GridDimension - 1; z++)
                for (int y = 1; y < GridDimension - 1; y++)
                    for (int x = 1; x < GridDimension - 1; x++)
                        Cells[0, x, y, z] = Cells[1, x, y, z] = Random.Range(0f, 1f) <= InitialSeedChance;
        }
        else
        {
            if (GridDimension % 2 == 0)
            {
                int HGD = (GridDimension / 2) - 1;

                for (int z = HGD; z < HGD + 2; z++)
                    for (int y = HGD; y < HGD + 2; y++)
                        for (int x = HGD; x < HGD + 2; x++)
                            Cells[0, x, y, z] = Cells[1, x, y, z] = true;
            }
            else
            {
                int HGD = (GridDimension / 2);

                for (int z = HGD - 1; z < HGD + 2; z++)
                    for (int y = HGD - 1; y < HGD + 2; y++)
                        for (int x = HGD - 1; x < HGD + 2; x++)
                            Cells[0, x, y, z] = Cells[1, x, y, z] = true;
            }
        }        
    }

    int Evaluate( int x, int y, int z )
    {
        int Count =0;

        // depth z-1
        if (Cells[Src, x - 1, y - 1, z-1]) ++Count;
        if (Cells[Src, x    , y - 1, z-1]) ++Count;
        if (Cells[Src, x + 1, y - 1, z-1]) ++Count;
        if (Cells[Src, x - 1, y    , z-1]) ++Count;
        if (Cells[Src, x    , y    , z-1]) ++Count;
        if (Cells[Src, x + 1, y    , z-1]) ++Count;
        if (Cells[Src, x - 1, y + 1, z-1]) ++Count;
        if (Cells[Src, x    , y + 1, z-1]) ++Count;
        if (Cells[Src, x + 1, y + 1, z-1]) ++Count;
        
        // depth z
        if (Cells[Src, x - 1, y - 1,z]) ++Count; // top row
        if (Cells[Src, x    , y - 1,z]) ++Count;
        if (Cells[Src, x + 1, y - 1,z]) ++Count;
        if (Cells[Src, x - 1, y    ,z]) ++Count; // sides
        if (Cells[Src, x + 1, y    ,z]) ++Count;
        if (Cells[Src, x - 1, y + 1,z]) ++Count; // bottom row
        if (Cells[Src, x    , y + 1,z]) ++Count;
        if (Cells[Src, x + 1, y + 1,z]) ++Count;
        
        // depth z+1
        if (Cells[Src, x - 1, y - 1, z+1]) ++Count;
        if (Cells[Src, x    , y - 1, z+1]) ++Count;
        if (Cells[Src, x + 1, y - 1, z+1]) ++Count;
        if (Cells[Src, x - 1, y    , z+1]) ++Count;
        if (Cells[Src, x    , y    , z+1]) ++Count;
        if (Cells[Src, x + 1, y    , z+1]) ++Count;
        if (Cells[Src, x - 1, y + 1, z+1]) ++Count;
        if (Cells[Src, x    , y + 1, z+1]) ++Count;
        if (Cells[Src, x + 1, y + 1, z+1]) ++Count;

        return Count;
    }

    float NextUpdate = 0f;

    void ApplyRule()
    {
        // apply rule
        /*
        In the Game of Life, a population of cells evolves over time. In each generation, whether a cells lives or dies depends on its neighbors.

        More specifically, choose four numbers a, b, c, and d. Here's the breakdown:

        If a living cube has between a and b neighbors, it stays alive to the next generation. Otherwise it dies(by over - or underpopulation.)
        If an empty cell has between c and d neighbors, it becomes alive in the next generation(by reproduction).
        Each cube can have between 0 and 26 neighbors.
        */

        int NeighborCount = 0;

        if (GenerationCounter < GotoGeneration)
        {
            for (int z = 1; z < GridDimension - 1; z++)
                for (int y = 1; y < GridDimension - 1; y++)
                    for (int x = 1; x < GridDimension - 1; x++)
                    {
                        NeighborCount = Evaluate(x, y, z);

                        if (Cells[Src, x, y, z])    // cell is alive, check for survival
                        {
                            Cells[Dst, x, y, z] = RuleA <= NeighborCount && NeighborCount <= RuleB;
                        }
                        else // check for birth
                        {
                            Cells[Dst, x, y, z] = RuleC <= NeighborCount && NeighborCount <= RuleD;
                        }
                    }

            UpdateCellMaterial();

            ++GenerationCounter;
            Src = GenerationCounter % 2;
            Dst = (Src + 1) % 2;

            TextGeneration.text = "Generation\r\n" + GenerationCounter.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (AutoPlay == false)
            return;

        if (NextUpdate > Time.time)
            return;

        NextUpdate = Time.time + (1f / Frequency);

        ApplyRule();
    }

    public class CellRef
    {
        public GameObject GO;
        public Material Mat;
        public Renderer R;
        public CellCube CC;
    }

    public GameObject Prefab;
    private CellRef[,,] CellCubes;
    public Material CubeMat;

    void InitCells()
    {                        
        // Create cubes
        CellCubes = new CellRef[GridDimension, GridDimension,GridDimension];
        
        int GridDimensionMinusOne = GridDimension - 1;

        int GridDimensionMinusOneOverTwoMinusOne = (GridDimensionMinusOne / 2) - 1;
     
        float MaxDist = (3f * GridDimensionMinusOneOverTwoMinusOne * GridDimensionMinusOneOverTwoMinusOne);

        int _x, _y, _z;

        int Dist;

        Material Mat;

        for (var z = 0; z < GridDimension; z++)
            for (var y = 0; y < GridDimension; y++)
                for (var x = 0; x < GridDimension; x++)
                {                 
                    CellCubes[x, y, z] = new CellRef();
                    CellCubes[x, y, z].GO = GameObject.Instantiate(Prefab, new Vector3(x-(GridDimensionMinusOne / 2f), y - (GridDimensionMinusOne / 2f), z - (GridDimensionMinusOne / 2f)), Quaternion.identity);
                    CellCubes[x, y, z].GO.transform.parent = transform;
                    CellCubes[x, y, z].R = CellCubes[x, y,z].GO.GetComponent<Renderer>();
                    CellCubes[x, y, z].CC = CellCubes[x, y, z].GO.GetComponent<CellCube>();
                    CellCubes[x, y, z].CC.AnimationDuration = CubeAnimationScale / Frequency;
                    CellCubes[x, y, z].CC.SetState(Cells[Src, x, y, z]);

                    _x = (int)CellCubes[x, y, z].GO.transform.position.x;
                    _y = (int)CellCubes[x, y, z].GO.transform.position.y;
                    _z = (int)CellCubes[x, y, z].GO.transform.position.z;

                    Dist = (_x * _x + _y * _y + _z * _z);

                    if( CubeMats.TryGetValue( Dist, out Mat ) )
                    {
                        CellCubes[x, y, z].R.material = Mat;
                    }
                    else
                    {
                        Mat = new Material(CubeMat);
                        Mat.color = Color.Evaluate( Dist / MaxDist);
                        CubeMats[Dist] = Mat;
                        CellCubes[x, y, z].R.material = Mat;
                    }
                }
    }

    void UpdateCellMaterial()
    {
        for (var z = 0; z < GridDimension; z++)
            for (var y = 0; y < GridDimension; y++)
                for (var x = 0; x < GridDimension; x++)
                    CellCubes[x, y, z].CC.Toggle(Cells[Dst, x, y, z]);
    }


    /// UI
    /// 

    private bool AutoPlay = false;

    public Button BtnPlayStop;
    public Sprite Play;
    public Sprite Stop;
    public TextMeshProUGUI TextGeneration;
    public TextMeshProUGUI TextSpeed;

    class Preset
    {
        public Preset(float S, string T) { TimeScale = S; Text = T; }
        public float TimeScale;
        public string Text;
    }

    private Preset[] Presets = new Preset[] { new Preset(1f, "1x"), new Preset(2f, "2x"), new Preset(4f, "4x"), new Preset(8f, "8x"), new Preset(16f, "16x"), new Preset(32f, "32X") };
    private int PresetIndex = 0;
    public void PS()
    {
        AutoPlay = !AutoPlay;

        BtnPlayStop.image.sprite = AutoPlay ? Stop : Play;

    }

    public void SU()
    {
        PresetIndex = ++PresetIndex % Presets.Length;

        Time.timeScale = Presets[PresetIndex].TimeScale;

        TextSpeed.text = Presets[PresetIndex].Text;
    }

    public void SD()
    {
        PresetIndex = (PresetIndex + Presets.Length -1) % Presets.Length;

        Time.timeScale = Presets[PresetIndex].TimeScale;

        TextSpeed.text = Presets[PresetIndex].Text;
    }

    public void NG()
    {
        ApplyRule();
    }
}
