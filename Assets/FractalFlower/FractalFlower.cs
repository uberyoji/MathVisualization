using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalFlower : MonoBehaviour
{
    public int PetalPerIteration = 6;
    public float ThetaRatioPerIteration = 0.5f;
    private float ThetaIteration = 0f;

    public float InterIterationDelay = 0.5f; // in seconds
    private float NextIterationTime = 0f;
    
    public GameObject PetalPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    GameObject GetNextFreePetal(float Base, float Angle)
    {        
        transform.eulerAngles = new Vector3(0, 0, Base+Angle);       // fixme optim

        return GameObject.Instantiate(PetalPrefab, transform.position, transform.rotation); // fixme optim
    }

    void SpawnPetals()
    {
        float a = 0f;
        float da = 360 / PetalPerIteration;

        GameObject Petal;

        transform.eulerAngles = new Vector3(0f, 0f, ThetaIteration);

        for (int i = 0; i < PetalPerIteration; i++)
        {
            Petal = GetNextFreePetal(ThetaIteration,a);
            a += da;
        }

        ThetaIteration += (360f * ThetaRatioPerIteration);
    }

    // Update is called once per frame
    void Update()
    {
        if (NextIterationTime <= Time.time)
        {
            SpawnPetals();
            NextIterationTime = NextIterationTime + InterIterationDelay;
        }
    }
}
