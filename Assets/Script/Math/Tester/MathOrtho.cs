using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MathOrtho : MonoBehaviour
{
    Vector3 normal;
    Vector3 tangent;
    public Vector3 binormal;

    public GameObject NormalObject;
    public GameObject RayObject;
    public GameObject TangentObject;
    public GameObject BinormalObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, 10))
        {
            normal = hit.normal;
            tangent = gameObject.transform.forward;

            Vector3.OrthoNormalize(ref normal, ref tangent, ref binormal);

            NormalObject.transform.position = hit.point;
            NormalObject.transform.rotation = Quaternion.LookRotation(normal);

            TangentObject.transform.position = hit.point;
            TangentObject.transform.rotation = Quaternion.LookRotation(tangent);

            BinormalObject.transform.position = hit.point;
            BinormalObject.transform.rotation = Quaternion.LookRotation(binormal);

            RayObject.transform.localPosition = Vector3.zero;
            RayObject.transform.localRotation = Quaternion.identity;

        }
        //
    }
}
