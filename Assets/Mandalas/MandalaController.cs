using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandalaController : MonoBehaviour
{
    public GeoMandala Mandala;

    private Vector2 CurrPos = new Vector2();
    private Vector2 LastPos = new Vector2();
    private Vector2 DeltaPos = new Vector2();

    private float Iterations = 1;
    public float IterationFactor = 1000f;

    void Start()
    {
        if (Camera.main.pixelHeight > Camera.main.pixelWidth)
            Camera.main.orthographicSize = 2;

        Iterations = Mandala.Iterations;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LastPos.Set( Input.mousePosition.x / Camera.main.pixelWidth, (Camera.main.pixelHeight - Input.mousePosition.y) / Camera.main.pixelHeight );
            CurrPos.Set(Input.mousePosition.x / Camera.main.pixelWidth, (Camera.main.pixelHeight - Input.mousePosition.y) / Camera.main.pixelHeight);
        }

        if (Input.GetMouseButton(0))
        {
            CurrPos.Set(Input.mousePosition.x / Camera.main.pixelWidth, (Camera.main.pixelHeight - Input.mousePosition.y) / Camera.main.pixelHeight);

            //            Debug.Log("Mouse Position CX=" + CurrPos + " LX="+LastPos);

            DeltaPos.Set(LastPos.x - CurrPos.x, LastPos.y - CurrPos.y);

//            if ( Mathf.Abs(LastPos.x) > Mathf.Abs(LastPos.y) )
            {
                Mandala.Ratio += DeltaPos.x;
                Mandala.Ratio = Mathf.Repeat(Mandala.Ratio, 1f);
            }
//            else
            {
                Iterations += DeltaPos.y * IterationFactor;
                Iterations = Mathf.Clamp(Iterations, 0, 50);
                Mandala.Iterations = (int)Iterations;
            }

            LastPos.Set(CurrPos.x, CurrPos.y);
        }
    }
}
