using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TestClosePlanePoint : MonoBehaviour
{
    Camera camera;

    public GameObject Plane;
    public GameObject target;

    public GameObject result;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (camera == null)
            camera = Camera.main;
        if (Plane == null || target == null)
            return;

        /*
        var closePointForward = Math.ClosePointOnDirection(Plane.transform.position, Plane.transform.forward, target.transform.position);
        var closePointRight = Math.ClosePointOnDirection(Plane.transform.position, Plane.transform.right, target.transform.position);

        Debug.DrawLine(target.transform.position, closePointForward, Color.red, Time.deltaTime);
        Debug.DrawLine(target.transform.position, closePointRight, Color.blue, Time.deltaTime);

        if (result != null)
        {
            result.transform.position = closePointForward + closePointRight - Plane.transform.position;
        }
        */

        var closePointForward = Vector3.Project((target.transform.position - Plane.transform.position), Plane.transform.forward);


        var closePointPlane = Vector3.ProjectOnPlane((target.transform.position - Plane.transform.position), Plane.transform.up);
        if (result != null)
        {
            result.transform.position = Plane.transform.position + closePointPlane;
        }
    }
}
