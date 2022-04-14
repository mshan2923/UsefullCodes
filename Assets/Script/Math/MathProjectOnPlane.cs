using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MathProjectOnPlane : MonoBehaviour
{
    public GameObject TargetObject;
    public GameObject NormalObject;
    public GameObject OutObject;

    public bool OnPlane = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3.ProjectOnPlane();
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, 10))
        {
            TargetObject.transform.rotation = Quaternion.LookRotation((TargetObject.transform.position - hit.point).normalized * -1);

            NormalObject.transform.position = hit.point;
            NormalObject.transform.rotation = Quaternion.LookRotation(hit.normal);

            if (OnPlane)
            {
                Vector3 Result = Vector3.ProjectOnPlane(TargetObject.transform.position, hit.normal);

                OutObject.transform.position = Result;//hit.point + 
                OutObject.transform.rotation = Quaternion.LookRotation(Result);
            }
            else
            {
                Vector3 Result = Vector3.Project(TargetObject.transform.position, hit.normal);

                OutObject.transform.position = hit.point + Result;
                OutObject.transform.rotation = Quaternion.LookRotation(Result);
            }

            //Debug.Log(TargetObject.transform.position);
        }
    }
}
