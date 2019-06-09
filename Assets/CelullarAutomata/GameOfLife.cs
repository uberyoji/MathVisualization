using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    public int GridWidth = 64;

    public int RuleA, RuleB, RuleC, RuleD = 0;

    public float Frequency = 60f;

    public float InitialSeedChance = .5f;
    
    public int GotoGeneration = 1;

    // naive implementation
    private bool[,,] Cells;


    private int Src = 0;
    private int Dst = 1;
    private int GenerationCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cells = new bool[2,GridWidth, GridWidth];

        // set initial seed
        for (int y = 1; y < GridWidth - 1; y++)
            for (int x = 1; x < GridWidth - 1; x++)
                Cells[0,x, y] = Cells[1, x, y] = Random.Range(0f, 1f) < InitialSeedChance;

        InitCells();
        UpdateCellMaterial();
    }
        
    int Evaluate( int x, int y )
    {
        int Count =0;
        // top row
        if (Cells[Src, x - 1, y - 1]) ++Count;
        if (Cells[Src, x    , y - 1]) ++Count;
        if (Cells[Src, x + 1, y - 1]) ++Count;

        // sides
        if (Cells[Src, x - 1, y    ]) ++Count;
        if (Cells[Src, x + 1, y    ]) ++Count;

        // bottom row
        if (Cells[Src, x - 1, y + 1]) ++Count;
        if (Cells[Src, x    , y + 1]) ++Count;
        if (Cells[Src, x + 1, y + 1]) ++Count;

        return Count;
    }

    float NextUpdate = 0f;

    // Update is called once per frame
    void Update()
    {
        if (NextUpdate > Time.time)
            return;

        NextUpdate = Time.time + (1f / Frequency);

        // apply rule
        /*
        In the Game of Life, a population of cells evolves over time. In each generation, whether a cells lives or dies depends on its neighbors.

        More specifically, choose four numbers a, b, c, and d. Here's the breakdown:

        If a living cube has between a and b neighbors, it stays alive to the next generation. Otherwise it dies(by over - or underpopulation.)
        If an empty cell has between c and d neighbors, it becomes alive in the next generation(by reproduction).
        Each cube can have between 0 and 26 neighbors.
        */

        int NeighborCount = 0;

        if( GenerationCounter < GotoGeneration )
        {
            for (int y = 1; y < GridWidth-1; y++)
                for (int x=1; x<GridWidth-1; x++)
                {
                    NeighborCount = Evaluate(x,y);

                    if ( Cells[Src, x,y] )    // cell is alive, check for survival
                    {
                        Cells[Dst, x, y] = RuleA <= NeighborCount && NeighborCount <= RuleB;
                    }
                    else // check for birth
                    {
                        Cells[Dst, x, y] = RuleC <= NeighborCount && NeighborCount <= RuleD;
                    }
                }        

            UpdateCellMaterial();

            ++GenerationCounter;
            Src = GenerationCounter % 2;
            Dst = (Src + 1) % 2;
        }            
    }

    public class CellRef
    {
        public GameObject GO;
        public Material Mat;
        public Renderer R;
    }

    public GameObject Prefab;
    private CellRef[,] CellQuads;
    public Material CellOff;
    public Material CellOn;

    void InitCells()
    {
        CellQuads = new CellRef[GridWidth, GridWidth];

        for (var x = 0; x < GridWidth; x++)
        {
            for (var y = 0; y < GridWidth; y++)
            {
                CellQuads[x, y] = new CellRef();
                CellQuads[x, y].GO = GameObject.Instantiate(Prefab, new Vector3(x, y, 0), Quaternion.identity);
                CellQuads[x, y].GO.transform.parent = transform;
                CellQuads[x, y].R = CellQuads[x, y].GO.GetComponent<Renderer>();
                CellQuads[x, y].R.material = CellOff;
            }
        }
    }

    void UpdateCellMaterial()
    {
        for (var x = 0; x < GridWidth; x++)
        {
            for (var y = 0; y < GridWidth; y++)
            {
                CellQuads[x, y].R.material = Cells[Dst,x, y] ? CellOn : CellOff;
            }
        }
    }
}
