using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Petal : MonoBehaviour
{
    public float RemoveAfter = 1.0f;
    private float DeathTime;

    public float Speed = 1f;

    private Vector3 Position = new Vector3();
    private Vector3 Velocity = new Vector3();

    public AnimationCurve WidthOverTime;
    public LineRenderer LR;
    public float BaseLineWidth = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        DeathTime = Time.time + RemoveAfter;

        transform.position = Position;

        Velocity.Set(Mathf.Cos( Mathf.Deg2Rad * transform.eulerAngles.z) * Speed, Mathf.Sin( Mathf.Deg2Rad * transform.eulerAngles.z) * Speed, 0f);

        float Scale = WidthOverTime.Evaluate(0f);
        transform.localScale = new Vector3(Scale, Scale, 1f);

        LR.widthMultiplier = Scale * BaseLineWidth;

        float C = Mathf.Cos( Mathf.Deg2Rad * 60f );
        float S = Mathf.Sin(Mathf.Deg2Rad * 60f);

        LR.SetPosition(0, new Vector3(-C, S,0f));
        LR.SetPosition(2, new Vector3(-C, -S, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        if(DeathTime <= Time.time)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            float Eval = (DeathTime - Time.time) / RemoveAfter;
            Eval = Mathf.Clamp(1f - Eval, 0f, 1f);

            float Scale = WidthOverTime.Evaluate(Eval);

            LR.widthMultiplier = Scale * BaseLineWidth;
            
            transform.localScale = new Vector3(Scale, Scale, 1f);

            Position += Velocity * Time.deltaTime;
            transform.position = Position;
        }
    }
}
