using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class SimplePetalCreator : MonoBehaviour
{
    public float Width = 1f;
    public float Height = 1f;
    public float Depth = 1f;
    public float CenterRatio = 0.5f;

    public Color TopColor;
    public Color SideColor;
    public Color CenterColor;
    public Color BottomColor;
    
    public bool ForceUpdateMesh = false;
        
    public Mesh MeshAsset;

    void Start()
    {
        //        ForceUpdateMesh = true;        
    }

    void Update()
    {
        if (ForceUpdateMesh)
        {
            BuildMesh();
            ForceUpdateMesh = false;
        }
    }

    /*
    private void OnValidate()
    {
        Debug.Log("Something changed. Rebuilding mesh for " + gameObject.name + ".");
        ForceUpdateMesh = true;
    }
    */

    void BuildMesh()
    {
        MeshAsset = GetMesh(); ;
        GetComponent<MeshFilter>().sharedMesh = MeshAsset;

#if UNITY_EDITOR
        AssetDatabase.CreateAsset(GetComponent<MeshFilter>().sharedMesh, "Assets/FractalFlower/SimplePetalMesh.mesh");
        AssetDatabase.SaveAssets();
#endif
    }

    // Generate mesh
    Mesh GetMesh()
    {
        float HalfWidth = Width / 2f;

        const int FaceCount = 4 * 2;
        const int VertexCount = FaceCount * 3;
        Vector3[] V = new Vector3[VertexCount];
        Vector3[] N = new Vector3[VertexCount];
        Color[] C = new Color[VertexCount];
        int[] I = new int[3 * FaceCount];

        int i = 0;

        Vector3 T = new Vector3(Height, 0f, 0f);
        Vector3 B = new Vector3(0f, 0f, 0f);
        Vector3 Ce = new Vector3(Height*CenterRatio, 0f, -Depth);
        Vector3 L = new Vector3(Height * CenterRatio, HalfWidth, 0f);
        Vector3 R = new Vector3(Height * CenterRatio, -HalfWidth, 0f);

        Vector3 Na = -Vector3.Cross(B - Ce, L - Ce); Na.Normalize();
        Vector3 Nb = -Vector3.Cross(Ce - T, L - T); Nb.Normalize();
        Vector3 Nc = -Vector3.Cross(B-R, Ce - R); Nc.Normalize();
        Vector3 Nd = -Vector3.Cross(R - T, Ce - T); Nd.Normalize();

        
        // top left
        V[i] = B;        N[i] = Na;        C[i] = BottomColor;      I[i] = i;        i++;
        V[i] = Ce;       N[i] = Na;        C[i] = CenterColor;      I[i] = i;        i++;
        V[i] = L;        N[i] = Na;        C[i] = SideColor;        I[i] = i;        i++;

        // top right
        V[i] = Ce;  N[i] = Nb; C[i] = CenterColor;  I[i] = i; i++;
        V[i] = T;   N[i] = Nb; C[i] = TopColor;     I[i] = i; i++;
        V[i] = L;   N[i] = Nb; C[i] = SideColor;    I[i] = i; i++;

        // bottom left
        V[i] = B;   N[i] = Nc;  C[i] = BottomColor;     I[i] = i; i++;
        V[i] = R;   N[i] = Nc;  C[i] = SideColor;       I[i] = i; i++;
        V[i] = Ce;  N[i] = Nc;  C[i] = CenterColor;     I[i] = i; i++;

        // bottom right
        V[i] = R;    N[i] = Nd; C[i] = SideColor;   I[i] = i; i++;
        V[i] = T;    N[i] = Nd; C[i] = TopColor;    I[i] = i; i++;
        V[i] = Ce;   N[i] = Nd; C[i] = CenterColor; I[i] = i; i++;
        

        // second side
        // top left
        V[i] = B; N[i] = -Na; C[i] = BottomColor; I[i] = i; i++;
        V[i] = L; N[i] = -Na; C[i] = SideColor; I[i] = i; i++;
        V[i] = Ce; N[i] = -Na; C[i] = CenterColor; I[i] = i; i++;

        // top right
        V[i] = Ce; N[i] = -Nb; C[i] = CenterColor; I[i] = i; i++;
        V[i] = L; N[i] = -Nb; C[i] = SideColor; I[i] = i; i++;
        V[i] = T; N[i] = -Nb; C[i] = TopColor; I[i] = i; i++;

        // bottom left
        V[i] = B; N[i] = -Nc; C[i] = BottomColor; I[i] = i; i++;
        V[i] = Ce; N[i] = -Nc; C[i] = CenterColor; I[i] = i; i++;
        V[i] = R; N[i] = -Nc; C[i] = SideColor; I[i] = i; i++;

        // bottom right
        V[i] = R; N[i] = -Nd; C[i] = SideColor; I[i] = i; i++;
        V[i] = Ce; N[i] = -Nd; C[i] = CenterColor; I[i] = i; i++;
        V[i] = T; N[i] = -Nd; C[i] = TopColor; I[i] = i; i++;
        
        Mesh M = new Mesh
        {
            vertices = V,
            normals = N,
            colors = C,
            triangles = I
        };
        return M;
    }   
}
