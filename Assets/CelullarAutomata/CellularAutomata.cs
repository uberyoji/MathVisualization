using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata : MonoBehaviour
{
    public int GridWidth = 64;

    public int RuleNumber = 0;

    public bool[] Rule = new bool[8];

    public float Frequency = 60f;

    // naive implementation
    private bool[,] Cells;
            
    // Start is called before the first frame update
    void Start()
    {
        Cells = new bool[GridWidth, GridWidth];

        Cells[GridWidth / 2,0] = true; // initial seed

        InitCells();

        if( RuleNumber != 0 )
        {
            for(int i=0;i<8;i++)
            {
                Rule[i] = ((RuleNumber & (1 << i)) != 0);
            }
        }
    }
       
    // 111, 110, 101, 100, 011, 010, 001, 000 
    bool Evaluate( bool A, bool B, bool C )
    {
        return Rule[(A ? 1 : 0) + (B ? 2 : 0) + (C ? 4 : 0)];
    }

    float NextUpdate = 0f;

    // Update is called once per frame
    void Update()
    {
        if (NextUpdate > Time.time)
            return;

        NextUpdate = Time.time + (1f / Frequency);

        // propagate down
        for (int y = GridWidth-1; y > 0; y--)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                Cells[x, y] = Cells[x, y-1];
            }
        }

        // apply rule
        bool Result = false;

        int Src = 1;
        int Dst = 0;

        for(int x=0; x<GridWidth; x++)
        {
            if (x == 0)
                Result = Evaluate(false, Cells[x,Src], Cells[x + 1,Src]);
            else if (x == GridWidth - 1)
                Result = Evaluate(Cells[x - 1,Src], Cells[x,Src], false);
            else
                Result = Evaluate(Cells[x - 1,Src], Cells[x,Src], Cells[x + 1,Src]);

            Cells[x,Dst] = Result;
        }        

        UpdateCellMaterial();
    }

    public class CellRef
    {
        public GameObject GO;
        public Material Mat;
        public Renderer R;
    }

    public GameObject Prefab;
    private CellRef[,] Cubes;
    public Material CellOff;
    public Material CellOn;

    void InitCells()
    {
        Cubes = new CellRef[GridWidth, GridWidth];

        for (var x = 0; x < GridWidth; x++)
        {
            for (var y = 0; y < GridWidth; y++)
            {
                Cubes[x, y] = new CellRef();
                Cubes[x, y].GO = GameObject.Instantiate(Prefab, new Vector3(x, y, 0), Quaternion.identity);
                Cubes[x, y].R = Cubes[x, y].GO.GetComponent<Renderer>();
                Cubes[x, y].R.material = CellOff;
            }
        }
    }

    void UpdateCellMaterial()
    {
        for (var x = 0; x < GridWidth; x++)
        {
            for (var y = 0; y < GridWidth; y++)
            {
                Cubes[x, y].R.material = Cells[x, y] ? CellOn : CellOff;
            }
        }
    }
}
