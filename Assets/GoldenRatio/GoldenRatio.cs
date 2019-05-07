using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Seed
{
    public GameObject GO;
    public SpriteRenderer SR;
}

public class GoldenRatio : MonoBehaviour
{
    
    public int Amount = 256;
    public float Distance = 0.1f;

    [Range(0f,2f)]
    public float Ratio = 0.123456789f;
    public bool UseTime = false;
    public float TimeScale = 0.01f;

    public Gradient Color;

    public GameObject SeedPrefab;

    private float CurrRatio;
    private float CurrDistance;
    private Seed[] SeedPool;

	// Use this for initialization
	void Start ()
    {
        SeedPool = new Seed[Amount];

        for(int Index=0;Index<Amount;Index++)
        {
            SeedPool[Index] = new Seed();
            SeedPool[Index].GO = GameObject.Instantiate(SeedPrefab);
            SeedPool[Index].SR = SeedPool[Index].GO.GetComponent<SpriteRenderer>();
        }        
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (UseTime)
            Ratio += Time.deltaTime * TimeScale;
                
        CurrRatio = 0;
        CurrDistance = Distance;

        float MaxDistance = Amount * Distance;

        float DistanceRatio = 1f;

        float RandomRatio = Mathf.Repeat(Time.time/2f,1f);

        Debug.Log(RandomRatio);

        foreach (Seed S in SeedPool)
        {
            S.GO.transform.position = new Vector3(Mathf.Cos(CurrRatio * Mathf.PI * 2) * CurrDistance, Mathf.Sin(CurrRatio * Mathf.PI * 2) * CurrDistance, 0f);
            DistanceRatio = CurrDistance / MaxDistance;

            S.SR.color = Color.Evaluate(DistanceRatio);

            CurrRatio += Ratio;
            CurrDistance += Distance;
        }		
	}
}
