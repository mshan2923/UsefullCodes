using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom;

[ExecuteInEditMode]
public class DebugVisibleSize : MonoBehaviour
{
    public GameObject Target;

    public MeshRenderer TargetRenderer;
    public GameObject View;

    public Vector3 LookDirtion = new();
    public float LookDistance = 1;
    public Vector2 Result = new Vector2();

    private void OnEnable()
    {
        if (TargetRenderer == null)
        {
            TargetRenderer = Target.GetComponent<MeshRenderer>();
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (TargetRenderer == null)
        {
            SetTargetRenderer();
        }
        else if (TargetRenderer.gameObject != Target)
        {
            SetTargetRenderer();
        }

        if (Target != null && View != null)
        {
            //View.transform.rotation = Quaternion.LookRotation(Target.transform.position - View.transform.position);
            View.transform.rotation = Quaternion.Euler(LookDirtion);
            View.transform.position = Target.transform.position + View.transform.rotation * Vector3.back * LookDistance;

            Result = Math.Bound2Rect(TargetRenderer.bounds, View.transform.rotation).size;
        }
    }

    void SetTargetRenderer()
    {
        TargetRenderer = Target.GetComponent<MeshRenderer>();
    }
}
