using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SphereCollision : MonoBehaviour
{
    public GameObject TargetSphere;
    public GameObject Projecter;
    public GameObject NormalDirection;

    public float TargetRadius = 1f;
    //public float ProjectRadius = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 레이케스트 없이 계산으로만!
        if (TargetSphere == null || Projecter == null || NormalDirection == null)
        {
            return;
        }


        Quaternion CollisionNormal;
        CalculateSphereNormal(TargetSphere.transform.position, Projecter.transform.position
             , Projecter.transform.rotation * Vector3.forward, TargetRadius, out CollisionNormal);

        NormalDirection.transform.rotation = CollisionNormal;
        NormalDirection.transform.position = TargetSphere.transform.position + CollisionNormal * Vector3.forward * TargetRadius;
    }

    bool CalculateSphereNormal(Vector3 TargetPos , Vector3 ProjectPos, Vector3 ProjectDir, float targetRadius, out Quaternion Normal)
    {
        var dis = (TargetPos - ProjectPos).magnitude;
        var ProjectDot = Vector3.Dot(ProjectDir, (ProjectPos - TargetPos).normalized);
        var ProjectRad = Mathf.Acos(ProjectDot);

        var OthoLength = dis * Mathf.Sin(ProjectRad);
        if (OthoLength > targetRadius)
        {
            Normal = Quaternion.identity;
            return false;
        }

        var ProjectLength = dis * Mathf.Cos(ProjectRad);
        var ProjectForwardPos = TargetPos + ProjectDir * ProjectLength;
        var ProjectUp = (ProjectForwardPos + (ProjectPos - ProjectForwardPos).normalized * OthoLength) - ProjectForwardPos;

        var RotationAxis = Quaternion.LookRotation(ProjectDir * -1, ProjectUp);
        var CollisionRad = Mathf.Asin(OthoLength / targetRadius);

        Normal = RotationAxis * Quaternion.AngleAxis(CollisionRad * Mathf.Rad2Deg * -1, Vector3.right);
        return true;
    }
}
