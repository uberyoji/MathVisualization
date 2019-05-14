using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SignalType
{
    Sine,
    Square,
    Triangle,
    Sawtooth
}

[System.Serializable]
public class SignalGenerator
{
    
    [SerializeField]
    private SignalType signalType = SignalType.Sine;
    
    public SignalType SignalType
    {
        get { return signalType; }
        set { signalType = value; }
    }
    [SerializeField]
    private float frequency = 1f;    
    public float Frequency
    {
        get { return frequency; }
        set { frequency = value; }
    }
    [SerializeField]
    private float phase = 0f;
    
    public float Phase
    {
        get { return phase; }
        set { phase = value; }
    }
    [SerializeField]
    private float amplitude = 1f;
    
    public float Amplitude
    {
        get { return amplitude; }
        set { amplitude = value; }

    }
    [SerializeField]
    private float offset = 0f;
    
    public float Offset
    {
        get { return offset; }
        set { offset = value; }
    }
    [SerializeField]
    private float invert = 1; // Yes=-1, No=1
    
    public bool Invert
    {
        get { return invert == -1; }
        set { invert = value ? -1 : 1; }
    }
        

    public SignalGenerator(SignalType initialSignalType)
    {
        signalType = initialSignalType;
    }

    public SignalGenerator() { }

    public float GetValue(float time)
    {
        float value = 0f;
        float t = frequency * time + phase;
        switch (signalType)
        { // http://en.wikipedia.org/wiki/Waveform
            case SignalType.Sine: // sin( 2 * pi * t )
                value = (float)Mathf.Sin(2f * Mathf.PI * t);
                break;
            case SignalType.Square: // sign( sin( 2 * pi * t ) )
                value = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * t));
                break;
            case SignalType.Triangle:
                // 2 * abs( t - 2 * floor( t / 2 ) - 1 ) - 1
                value = 1f - 4f * (float)Mathf.Abs
                    (Mathf.Round(t - 0.25f) - (t - 0.25f));
                break;
            case SignalType.Sawtooth:
                // 2 * ( t/a - floor( t/a + 1/2 ) )
                value = 2f * (t - (float)Mathf.Floor(t + 0.5f));
                break;
        }

        return (invert * amplitude * value + offset);
    }
}

public class Fourier : MonoBehaviour
{
    public float TimeScale = 0.5f;

    public GameObject SeedInstance;
    public ParticleSystem PS;

    private float Position = 0f;
    public float Speed = 0.1f;

    public SignalGenerator[] Signals;

    public GameObject SeedInstanceB;

    [Range(0,10)]
    public float WindingPeriod = 1f; // 1 is full circle
    
    public GameObject SeedInstanceC;

    public LineRenderer GraphWinding;
    public LineRenderer GraphCenterOfMass;
    public int SampleCount = 100;

    Vector3 GetCenterOfMass(float WindingFrequency, int SampleCount)
    {
        Vector3 CenterOfMass = new Vector3(0f, 0f, 0f);

        float Time = 0f;
        float TimeDt = 2f * Mathf.PI * WindingFrequency / SampleCount;

        for (int s = 0; s < SampleCount; s++)
        {
            float SignalHeight = 0f;
            foreach (SignalGenerator S in Signals)
                SignalHeight += S.GetValue(Time);

            CenterOfMass += new Vector3(Mathf.Cos(Time) * SignalHeight, Mathf.Sin(Time) * SignalHeight, 0f);

            Time += TimeDt;
        }

        CenterOfMass /= SampleCount;

        return CenterOfMass;
    }

    void UpdateGraphCenterOfMass()
    {        
        float Time = 0f;
        float TimeDt = 15f / SampleCount;

        GraphCenterOfMass.positionCount = SampleCount;

        for ( int s = 0; s < SampleCount; s++ )
        {
            Vector3 CM = GetCenterOfMass(Time, SampleCount);

            GraphCenterOfMass.SetPosition(s, new Vector3(Time, CM.y, 0f));

            Time += TimeDt;
        }
    }

    void UpdateGraphWinding()
    {
        float Time = 0f;
        float TimeDt = WindingPeriod / SampleCount;

        GraphWinding.positionCount = SampleCount;

        for (int s = 0; s < SampleCount; s++)
        {
            float SignalHeight = 0f;
            foreach (SignalGenerator S in Signals)
                SignalHeight += S.GetValue(Time);

            GraphWinding.SetPosition(s, new Vector3(Mathf.Cos(Time) * SignalHeight, Mathf.Sin(Time) * SignalHeight, 0f) );

            Time += TimeDt;
        }
    }

    // Use this for initialization
    void Start ()
    {
        UpdateGraphCenterOfMass();

    }

    float EulerFunction( float t )
    {
        return Mathf.Exp(2 * Mathf.PI * t);
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Time.timeScale = TimeScale;

        float SimTime = Time.time * TimeScale;

        var main = PS.main;

        if (Mathf.RoundToInt(SimTime) % 2 == 0)
            main.startColor = Color.white;
        else
            main.startColor = Color.yellow;

        Position += Time.deltaTime * Speed * SimTime;

        float SignalHeight = 0f;
        foreach (SignalGenerator S in Signals)
            SignalHeight += S.GetValue(SimTime);

        SeedInstance.transform.position = new Vector3(0f, SignalHeight, 0f);

        UpdateGraphWinding();

        // Winding real time
        /*
        float Winding = 1f / WindingPeriod;
                
        float CurrentWinding = -2f * Mathf.PI * Mathf.Repeat(SimTime, Winding) / Winding;

        // Debug.Log(CurrentWinding);
        
        float OX = Mathf.Cos(CurrentWinding) * SignalHeight;
        float OY = Mathf.Sin(CurrentWinding) * SignalHeight;
        SeedInstanceB.transform.position = new Vector3( OX, OY, 0f);
        */
    }

    
}
