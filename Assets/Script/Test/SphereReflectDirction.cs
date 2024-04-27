using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Physics;
using UnityEngine;

[ExecuteAlways]
public class SphereReflectDirction : MonoBehaviour
{
    public bool runRegacy = true;

    public GameObject Target0;
    public GameObject Target1;
    public GameObject Target0Arrow;
    public GameObject Target1Arrow;
    [Space(10)]
    public float Target0SpeedMultiply = 1f;
    public float Target1SpeedMultiply = 1f;

    [Space(10)]
    public Vector3 Target0Vec;
    public Vector3 Target1Vec;
    public float Target0Speed;
    public float Target1Speed;
    public Vector3 normal;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Target0Vec = Target0.transform.forward;
        Target1Vec = Target1.transform.forward;

        normal = Vector3.Normalize(Target0Vec + Target1Vec);


        if (runRegacy)
        {
            var reflected0 = -Vector3.Reflect(Target0Vec * Target0SpeedMultiply, normal);
            var reflected1 = -Vector3.Reflect(Target1Vec * Target1SpeedMultiply, normal);

            Target0Arrow.transform.rotation = Quaternion.FromToRotation(Target0.transform.forward, reflected0);
            Target1Arrow.transform.rotation = Quaternion.FromToRotation(Target1.transform.forward, reflected1);
        }else
        {
            /*
            var PostVec0 = (Target0Vec + Target1Vec) / 2 - Target1Vec;//질량이 같아서
            var PostVec1 = (Target1Vec + Target0Vec) / 2 - Target0Vec;
            var ReflectVec0 = PostVec0 - Target0Vec;//v0r
            var ReflectVec1 = PostVec1 - Target1Vec;//v1r

            var dot0 = Vector3.Dot(ReflectVec0, normal);
            var dot1 = Vector3.Dot(ReflectVec1, normal);

            ReflectVec0 -= 2 * dot0 * normal;
            ReflectVec1 -= 2 * dot1 * normal;

            Target0Arrow.transform.rotation = Quaternion.FromToRotation(Target0.transform.forward, ReflectVec0);
            Target1Arrow.transform.rotation = Quaternion.FromToRotation(Target1.transform.forward, ReflectVec1);
            */

            Custom.Math.CollisionSphereReflect(Target0Vec * Target0SpeedMultiply, Target1Vec * Target1SpeedMultiply, out var vec0, out var vec1);
            Target0Arrow.transform.rotation = Quaternion.FromToRotation(Target0.transform.forward, Vector3.Normalize(vec0));
            Target1Arrow.transform.rotation = Quaternion.FromToRotation(Target1.transform.forward, Vector3.Normalize(vec1));

            Target0Speed = vec0.magnitude;
            Target1Speed = vec1.magnitude;
        }

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="v0">Direction * Speed</param>
    /// <param name="v1">Direction * Speed</param>
    public static void CollisionReflect(Vector3 v0, Vector3 v1, float mass0, float mass1, out Vector3 vec0, out Vector3 vec1)
    {
        var normal = Vector3.Normalize(v0 + v1);

        var postV0 = (mass1 * v1 + mass0 * v0) / (mass0 + mass1) - v1;
        var postV1 = (mass1 * v1 + mass0 * v0) / (mass0 + mass1) - v0;
        var v0r = postV0 - v0;
        var v1r = postV1 - v1;

        var dot0 = Vector3.Dot(v0r, normal);
        var dot1 = Vector3.Dot(v1r, normal);

        v0r -= 2 * dot0 * normal;
        v1r -= 2 * dot1 * normal;

        vec0 = v0r.normalized * postV0.magnitude;
        vec1 = v1r.normalized * postV1.magnitude;
    }
    /// <summary>
    /// Equal Mass
    /// </summary>
    /// <param name="v0">Direction * Speed</param>
    /// <param name="v1">Direction * Speed</param>
    public static void CollisionReflect(Vector3 v0, Vector3 v1, out Vector3 vec0, out Vector3 vec1)
    {
        var normal = Vector3.Normalize(v0 + v1);

        var postV0 = (v0 + v1) * 0.5f - v1;
        var postV1 = (v0 + v1) * 0.5f - v0;
        var v0r = postV0 - v0;
        var v1r = postV1 - v1;

        var dot0 = Vector3.Dot(v0r, normal);
        var dot1 = Vector3.Dot(v1r, normal);

        v0r -= 2 * dot0 * normal;
        v1r -= 2 * dot1 * normal;

        vec0 = v0r.normalized * postV0.magnitude;
        vec1 = v1r.normalized * postV1.magnitude;
    }
}
