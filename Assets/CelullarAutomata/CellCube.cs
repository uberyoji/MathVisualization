using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellCube : MonoBehaviour
{
    private bool Animate = false;
    public float AnimationDuration = 1f;
    private Vector3 Scale;

    private bool State = true;
    private float EndAnim = 0f;

    private float Ratio;
    
    // Update is called once per frame
    void Update()
    {
        if (Animate)
        {
            if (EndAnim > Time.time)
            {
                Ratio = (EndAnim - Time.time) / AnimationDuration;

                Ratio = State ? 1f - Ratio : Ratio;

                Ratio = Mathf.SmoothStep(0f, 1f, Ratio);

                Ratio = Mathf.Clamp(Ratio, 0.01f, 1f);

                Scale.Set(Ratio, Ratio, Ratio);

                transform.localScale = Scale;
            }
            else
            {
                Animate = false;                
                SetState(State);
            }
        }
    }

    public void SetState( bool NewState )
    {        
        State = NewState;
        Ratio = State ? 1f : 0.01f;
        Scale.Set(Ratio, Ratio, Ratio);
        transform.localScale = Scale;
        gameObject.SetActive(State);
    }

    public void Toggle( bool NewState )
    {
        if( State != NewState )
        {
            State = NewState;
            EndAnim = Time.time + AnimationDuration;
            Animate = true;

            if (State)
                gameObject.SetActive(true);
        }
    }
}
