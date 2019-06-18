using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RootMenu : MonoBehaviour
{
    public string SceneToLoad;

    public int SomeInt = 0;
    public float SomeFloat = 0f;

    // Start is called before the first frame update
    void Start()
    {
        /*
        SomeInt = URLParameters.GetSearchParameters().GetInt("someint", SomeInt);
        SomeFloat = (float)URLParameters.GetSearchParameters().GetDouble("somefloat", SomeFloat);        
        */

        if (SceneToLoad != "" || URLParameters.GetSearchParameters().TryGetValue("scene", out SceneToLoad))
        {
            // TODO check if scene exists before loading
            SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Additive);
        }
        else
        {
            // todo activate menu
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
