using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class OrbitCamera : MonoBehaviour
{

    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
    public float zSpeed = 10f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    float x = 0.0f;
    float y = 0.0f;
    float z = 0f;
    float lastz = 0f;
    float dz = 0f;

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void UpdateRotation()
    {
        // touch input
        
        if (Input.touchCount == 1)
        {
            x+= Input.GetTouch(0).deltaPosition.x * xSpeed * 0.001f;
            y -= Input.GetTouch(0).deltaPosition.y * ySpeed * 0.001f;            
        }

        // mouse input
        if ( Application.isEditor && Input.GetMouseButton(0) )
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
        }

        y = ClampAngle(y, yMinLimit, yMaxLimit);
    }

    void UpdateZoom()
    {
        if (Input.touchCount > 1)
        {
            z = (Input.GetTouch(0).position - Input.GetTouch(1).position).magnitude / (Screen.height / 2 + Screen.width / 2);

            if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began)
                lastz = z;  // just started touching to dz is now ref.

            dz = lastz - z;

            distance = Mathf.Clamp(distance - dz * zSpeed * -2f, distanceMin, distanceMax);

            lastz = z;
        }
        else
            lastz = 0f;

        if (Application.isEditor)
        {
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * zSpeed, distanceMin, distanceMax);
        }
    }

    void LateUpdate()
    {
        if (target )
        {
            UpdateRotation();

            UpdateZoom();

            Quaternion rotation = Quaternion.Euler(y, x, 0);
                        
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
   
}
