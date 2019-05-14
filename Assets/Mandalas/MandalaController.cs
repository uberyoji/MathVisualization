using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandalaController : MonoBehaviour
{
    public GeoMandala Mandala;

    private Vector2 CurrPos = new Vector2();
    private Vector2 LastPos = new Vector2();
    private Vector2 DeltaPos = new Vector2();

    private float PC = 1;
    public float YFactor = 100f;

    void Start()
    {
        if (Camera.main.pixelHeight > Camera.main.pixelWidth)
            Camera.main.orthographicSize = 2;

        PC = (float)Mandala.PointCount;
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
            DeltaPos.Set(LastPos.x - CurrPos.x, LastPos.y - CurrPos.y);
            {
                Mandala.Ratio += DeltaPos.x;
                Mandala.Ratio = Mathf.Repeat(Mandala.Ratio, 1f);
            }
            {
                PC += (DeltaPos.y * YFactor);
                PC = Mathf.Clamp(PC, 3, 16);
                Mandala.PointCount = (int)PC;
            }
            LastPos.Set(CurrPos.x, CurrPos.y);
        }
    }
}
