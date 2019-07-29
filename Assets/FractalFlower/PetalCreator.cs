using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class PetalCreator : MonoBehaviour
{
    public float Width = 1f;
    public float Height = 1f;
    public int WidthSegmentCount = 16;
    public int HeightSegmentCount = 16;

    public AnimationCurve HalfShape;
    public Gradient ColorCenter;
    public Gradient ColorSide;

    public bool ForceUpdateMesh = false;

    public BoneController[] Bones;

    public Mesh SkinnedMesh;

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
        int i = 0;
        CombineInstance[] combine = new CombineInstance[2];

        combine[0].mesh = GetMesh(true);
        combine[0].transform = Matrix4x4.identity;

        combine[1].mesh = GetMesh(false);
        combine[1].transform = Matrix4x4.identity;

        SkinnedMeshRenderer SMR = GetComponent<SkinnedMeshRenderer>();
        if (SMR)
        {
            // build bone and binpose arrays
            Transform[] bones = new Transform[Bones.Length];
            Matrix4x4[] bindPoses = new Matrix4x4[Bones.Length];
            for (i = 0; i < Bones.Length; i++)
            {
                bones[i] = Bones[i].transform;
                bindPoses[i] = bones[i].worldToLocalMatrix * transform.localToWorldMatrix;
            }

            // combine mesh first
            SkinnedMesh = new Mesh();
            SkinnedMesh.CombineMeshes(combine);

            // then weights
            SkinnedMesh.boneWeights = GetBoneWeightFromInfluenceSpheres(SkinnedMesh.vertices);
            SkinnedMesh.bindposes = bindPoses;

            // assign to renderer
            SMR.bones = bones;
            SMR.sharedMesh = SkinnedMesh;

            GetComponent<MeshFilter>().sharedMesh = SkinnedMesh;
        }
        else
        {
            GetComponent<MeshFilter>().sharedMesh = new Mesh();
            GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
        }
#if UNITY_EDITOR
        AssetDatabase.CreateAsset(SkinnedMesh, "Assets/FractalFlower/PetalMesh.mesh");
        AssetDatabase.SaveAssets();
#endif
    }

    Vector3 GetVector(float r, float a, float w)
    {
        return new Vector3(r * Mathf.Cos(a), w, r * Mathf.Sin(a));
    }

    BoneWeight[] GetBoneWeightFromInfluenceSpheres( Vector3[] V )
    {
        if( Bones.Length == 0 )
            return null;

        BoneWeight[] B = new BoneWeight[V.Length];

        float Distance = 0f;
        float Influence = 0f;

        for (int i=0;i<V.Length;i++)
        {
            Distance = (V[i] - Bones[0].transform.position).magnitude;
            B[i].boneIndex0 = 0;
            B[i].weight0 = Mathf.Max((Bones[0].InfluenceRadius - Distance) / Bones[0].InfluenceRadius, 0f);

            Distance = (V[i] - Bones[1].transform.position).magnitude;
            B[i].boneIndex1 = 1;
            B[i].weight1 = Mathf.Max((Bones[1].InfluenceRadius - Distance) / Bones[1].InfluenceRadius, 0f);

            Distance = (V[i] - Bones[2].transform.position).magnitude;
            B[i].boneIndex2 = 2;
            B[i].weight2 = Mathf.Max((Bones[2].InfluenceRadius - Distance) / Bones[2].InfluenceRadius, 0f);

            Distance = (V[i] - Bones[3].transform.position).magnitude;
            B[i].boneIndex3 = 3;
            B[i].weight3 = Mathf.Max((Bones[3].InfluenceRadius - Distance) / Bones[3].InfluenceRadius, 0f);

            Influence = B[i].weight0 + B[i].weight1 + B[i].weight2 + B[i].weight3;
            B[i].weight0 /= Influence;
            B[i].weight1 /= Influence;
            B[i].weight2 /= Influence;
            B[i].weight3 /= Influence;
        }

        // todo collect closest bones

        return B;
    }

    // Generate mesh
    Mesh GetMesh(bool LeftSide)
    {
        float HalfWidth = Width / 2f;

        int VertexCount = WidthSegmentCount * HeightSegmentCount;
        Vector3[] V = new Vector3[VertexCount];
        Vector3[] N = new Vector3[VertexCount];
        Color[] C = new Color[VertexCount];

        int n = 0;
        float wr = 0f;
        float hr = 0f;
        float hw = 0f;
        for (int w = 0; w < WidthSegmentCount; w++)
        {
            wr = (float)w / (WidthSegmentCount-1);

            for (int h = 0; h < HeightSegmentCount; h++)
            {
                hr = (float)h / (HeightSegmentCount - 1);

                hw = HalfWidth * HalfShape.Evaluate(hr) * (LeftSide ? 1f : -1f);

                V[n] = new Vector3(Height * hr, wr * hw, 0f);
                N[n] = new Vector3(0f, -1f, 0f);
                C[n] = (ColorCenter.Evaluate(hr) * wr + ColorSide.Evaluate(hr) * (1f - wr));

                n++;
            }
        }

        int FaceCount = 2 * (WidthSegmentCount-1) * (HeightSegmentCount-1);
        int[] I = new int[3 * FaceCount];

        int bv, k=0;
        for (int h = 0; h < WidthSegmentCount - 1; h++)
        {
            for (int s = 0; s < HeightSegmentCount - 1; s++)
            {
                bv = h * HeightSegmentCount + s;
                    
                if(LeftSide)
                {
                    // first face
                    I[k++] = bv;
                    I[k++] = bv + HeightSegmentCount;
                    I[k++] = bv + 1;
                    // second face
                    I[k++] = bv + 1;
                    I[k++] = bv + HeightSegmentCount;
                    I[k++] = bv + HeightSegmentCount + 1;
                }
                else
                {
                    // first face
                    I[k++] = bv;
                    I[k++] = bv + 1;
                    I[k++] = bv + HeightSegmentCount;
                    // second face
                    I[k++] = bv + 1;
                    I[k++] = bv + HeightSegmentCount + 1;
                    I[k++] = bv + HeightSegmentCount;                    
                }
            }
        }

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
