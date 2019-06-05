using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingRay : MonoBehaviour
{
    public Vector2 NoiseScale;
    public Vector2 NoiseOffset;

    // Start is called before the first frame update
    void Start()
    {
       GenerateNoiseBuffer(NoiseTextureResolution, NoiseScale, NoiseOffset);
        GetComponent<Renderer>().material.mainTexture = GenerateTexture();
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public int NoiseTextureResolution = 512;

    private float[,] NoiseBuffer;

    private void GenerateNoiseBuffer(int Resolution, Vector2 Scale, Vector2 Offset)
    {
        NoiseBuffer = new float[Resolution, Resolution];

        float xCoord, yCoord;

        for (int x = 0; x < Resolution; x++)
        {
            for (int y = 0; y < Resolution; y++)
            {
                xCoord = (x + Offset.x) / (Resolution / Scale.x);
                yCoord = (y + Offset.y) / (Resolution / Scale.y);

                NoiseBuffer[x, y] = Mathf.PerlinNoise(xCoord, yCoord);
            }
        }
    }

    private Texture2D GenerateTexture()
    {
        Texture2D T = new Texture2D(NoiseTextureResolution, NoiseTextureResolution );

        for (int x = 0; x < NoiseTextureResolution; x++)
        {
            for (int y = 0; y < NoiseTextureResolution; y++)
            {
                T.SetPixel(x, y, new Color( NoiseBuffer[x, y], NoiseBuffer[x, y], NoiseBuffer[x, y]) );
            }
        }

        T.Apply();

        return T;
    }
}