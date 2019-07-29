using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Petal : MonoBehaviour
{    
    private float DeathTime;
    
    private Vector3 Position = new Vector3();
    private Vector3 Velocity = new Vector3();
        
    public LineRenderer LR;
    
    public PetalConfig Config;

    // Start is called before the first frame update
    void Start()
    {
        DeathTime = Time.time + Config.LifeTime;

        transform.position = Position;

        Velocity.Set(Mathf.Cos( Mathf.Deg2Rad * transform.eulerAngles.z) * Config.Speed, Mathf.Sin( Mathf.Deg2Rad * transform.eulerAngles.z) * Config.Speed, 0f);

        float Scale = Config.WidthOverTime.Evaluate(0f);
        transform.localScale = new Vector3(Scale, Scale, 1f);

        if( LR )
        {
            LR.startColor = Config.ColorOverTime.Evaluate(0f);
            LR.endColor = Config.ColorOverTime.Evaluate(0f);
            LR.widthMultiplier = Scale * Config.BaseLineWidth;

            float C = Mathf.Cos(Mathf.Deg2Rad * Config.Angle/2f) * Config.BaseLineLength;
            float S = Mathf.Sin(Mathf.Deg2Rad * Config.Angle/2f) * Config.BaseLineLength;

            LR.SetPosition(0, new Vector3(-C, S, 0f));
            LR.SetPosition(1, new Vector3(0f, 0f, 0f));
            LR.SetPosition(2, new Vector3(-C, -S, 0f));
        }        
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
            float Eval = (DeathTime - Time.time) / Config.LifeTime;
            Eval = Mathf.Clamp(1f - Eval, 0f, 1f);

            float Scale = Config.WidthOverTime.Evaluate(Eval);

            if( LR )
            {
                LR.startColor = Config.ColorOverTime.Evaluate(Eval);
                LR.endColor = Config.ColorOverTime.Evaluate(Eval);
                LR.widthMultiplier = Scale * Config.BaseLineWidth;
            }            
            
            transform.localScale = new Vector3(Scale, Scale, 1f);

            Position += Velocity * Time.deltaTime;
            transform.position = Position;
        }
    }
}
