using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandalaController : MonoBehaviour
{
    public Mandala M;

    private Vector2 CurrPos = new Vector2();
    private Vector2 LastPos = new Vector2();
    private Vector2 DeltaPos = new Vector2();
        
    void Start()
    {
        if (Camera.main.pixelHeight > Camera.main.pixelWidth)
            Camera.main.orthographicSize = 2;
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
                M.Ratio += DeltaPos.x;
                M.Ratio = Mathf.Repeat(M.Ratio, 1f);
            }
                        
            LastPos.Set(CurrPos.x, CurrPos.y);
        }

        if( Input.GetMouseButtonUp(0) )
        {
            CurrPos.Set(Input.mousePosition.x / Camera.main.pixelWidth, (Camera.main.pixelHeight - Input.mousePosition.y) / Camera.main.pixelHeight);

            if (CurrPos.y > 0.8f)
                --M.PointCount;
            if (CurrPos.y < 0.2f)
                ++M.PointCount;

            M.PointCount = Mathf.Clamp(M.PointCount, 3, 16);
        }
    }
}
