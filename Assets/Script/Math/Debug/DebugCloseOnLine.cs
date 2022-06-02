using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DebugCloseOnLine : MonoBehaviour
{
    public GameObject NormalObject;
    public GameObject TargetObject;

    public GameObject ClosePoint;

    public bool LimitLength = true;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //TargetObject.transform.rotation = Quaternion.LookRotation((TargetObject.transform.position - hit.point).normalized * -1);

        Vector3 normal = (NormalObject.transform.position - gameObject.transform.position).normalized;
        NormalObject.transform.rotation = Quaternion.LookRotation(normal);

        if (LimitLength)
        {
            ClosePoint.transform.position = Math.ClosePointOnLine(gameObject.transform.position, NormalObject.transform.position, TargetObject.transform.position);
        }else
        {
            ClosePoint.transform.position = Math.ClosePointOnDirection(gameObject.transform.position, normal, TargetObject.transform.position);
            //ClosePoint.transform.position = Math.ClosePointOnDirection(gameObject.transform.position, NormalObject.transform.position, TargetObject.transform.position);
        }
    }
}
