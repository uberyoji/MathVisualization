using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalFlower : MonoBehaviour
{
    public int PetalCountPerIteration = 6;
    public float ThetaRatioPerIteration = 0.5f;
    private float ThetaIteration = 0f;

    public float InterIterationDelay = 0.5f; // in seconds
    private float NextIterationTime = 0f;
    
    public GameObject PetalPrefab;

    public PetalConfig Config;

    // Start is called before the first frame update
    void Start()
    {
        PetalCountPerIteration = URLParameters.GetSearchParameters().GetInt("count", PetalCountPerIteration);
        ThetaRatioPerIteration = (float)URLParameters.GetSearchParameters().GetDouble("theta", ThetaRatioPerIteration);
        InterIterationDelay = (float)URLParameters.GetSearchParameters().GetDouble("delay", InterIterationDelay);

        Config.Angle = (float)URLParameters.GetSearchParameters().GetDouble("angle", Config.Angle);
        Config.Speed = (float)URLParameters.GetSearchParameters().GetDouble("speed", Config.Speed);
        Config.BaseLineWidth = (float)URLParameters.GetSearchParameters().GetDouble("width", Config.BaseLineWidth);
        Config.BaseLineLength = (float)URLParameters.GetSearchParameters().GetDouble("length", Config.BaseLineLength);
        Config.LifeTime = (float)URLParameters.GetSearchParameters().GetDouble("lifetime", Config.LifeTime);
    }

    GameObject GetNextFreePetal(float Base, float Angle)
    {        
        transform.eulerAngles = new Vector3(0, 0, Base+Angle);       // fixme optim

        GameObject P = GameObject.Instantiate(PetalPrefab, transform.position, transform.rotation); // fixme optim

        P.GetComponent<Petal>().Config = Config;

        return P;
    }

    void SpawnPetals()
    {
        float a = 0f;
        float da = 360 / PetalCountPerIteration;

        transform.eulerAngles = new Vector3(0f, 0f, ThetaIteration);

        for (int i = 0; i < PetalCountPerIteration; i++)
        {
            GetNextFreePetal(ThetaIteration,a);
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
