using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MathReflect : MonoBehaviour
{
    public GameObject DirectionObject;
    public GameObject OutDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward, Color.black, Time.smoothDeltaTime);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, 10))
        {
            DirectionObject.transform.rotation = Quaternion.LookRotation((DirectionObject.transform.position - hit.point).normalized * -1);

            Vector3 reflect = Vector3.Reflect((DirectionObject.transform.position - hit.point).normalized, hit.normal);
            OutDirection.transform.position = hit.point;
            OutDirection.transform.rotation = Quaternion.LookRotation((reflect.normalized) * -1);
        }




        //
    }
}
