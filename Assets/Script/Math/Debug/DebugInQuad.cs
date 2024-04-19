using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom;

[ExecuteInEditMode]
public class DebugInQuad : MonoBehaviour
{
    public GameObject Origin;
    public GameObject Target;

    public GameObject vaule;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Origin != null && Target != null && vaule != null)
            vaule.transform.position =
                Math.ClosestQuad(Origin.transform.localScale, Origin.transform.position, Origin.transform.rotation.eulerAngles.y, Target.transform.position);
    }
}
