using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[ExecuteInEditMode]
public class CameraRenderPlace : MonoBehaviour
{
    public Camera camera;
    public GameObject Target;

    public bool isVisible;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (camera == null || Target == null)
            return;
        {
            //Camera.farClipPlane
            //Camera.nearClipPlane

            Vector3[] corner = new Vector3[4];
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, corner);

            Debug.DrawLine(camera.transform.position, GetConerResize(corner[0]), Color.red, Time.deltaTime);
            Debug.DrawLine(camera.transform.position, GetConerResize(corner[1]), Color.green, Time.deltaTime);
            Debug.DrawLine(camera.transform.position, GetConerResize(corner[2]), Color.blue, Time.deltaTime);
            Debug.DrawLine(camera.transform.position, GetConerResize(corner[3]), Color.white, Time.deltaTime);

            //Debug.DrawLine(camera.transform.position, GetResize(camera.transform.position, Target.transform.position) , Color.black, Time.deltaTime);

            //Regacy
            //Debug.Log($"Result : {IsRenderable(camera, Target.transform.position)} / Rate : {IsRenderableRate(camera, Target.transform.position)}");
            

            {
                
                float coneAngle = Math.Dot((corner[0] - camera.transform.position).normalized, camera.transform.forward);
                Debug.Log($"Cone Angle : {coneAngle} | {GetCameraConeAngle(camera.transform.rotation, camera.fieldOfView, camera.aspect)} || {math.tan(math.radians(30))}");



                /*
                var DrawDir = Quaternion.Euler(0, coneAngle, 0) * Vector3.forward;
                Quaternion DrawRot = new();

                for (int i = 0; i < 360; i += 10)
                {
                    DrawRot.eulerAngles = new Vector3(0, 0, i);

                    Debug.DrawLine(camera.transform.position, camera.transform.position + (DrawRot * DrawDir), Color.black, Time.deltaTime);
                }*/
            }//Draw Cone

            {
                var YawDir = Vector3.ProjectOnPlane(Target.transform.position - camera.transform.position, camera.transform.up);
                var pitchDir = Vector3.ProjectOnPlane(Target.transform.position - camera.transform.position, camera.transform.right);

                Debug.DrawLine(camera.transform.position + YawDir, camera.transform.position + pitchDir, Color.black, Time.deltaTime);

                var TargetPlaceYaw = Math.Dot(camera.transform.forward, YawDir.normalized);
                var TargetPlacePitch = Math.Dot(camera.transform.forward, pitchDir.normalized);

                Debug.Log($"Yaw : {TargetPlaceYaw <= Camera.VerticalToHorizontalFieldOfView(camera.fieldOfView, camera.aspect) * 0.5f}" +
                    $" , Pitch : {TargetPlacePitch <= camera.fieldOfView * 0.5f}" +
                    $"\n Yaw : {TargetPlaceYaw}  | {TargetPlaceYaw / (Camera.VerticalToHorizontalFieldOfView(camera.fieldOfView, camera.aspect) * 0.5f)}%" +
                    $"  / Pitch : {TargetPlaceYaw} | {TargetPlacePitch / (camera.fieldOfView * 0.5f)}%");
                
            }
            
        }
    }

    #region regacy
    Vector3 GetResize(Vector3 origin, Vector3 vaule)
    {
        return origin + (vaule - origin).normalized;
    }
    Vector3 GetConerResize(Vector3 vaule)
    {
        return GetResize(camera.transform.position, camera.transform.rotation * vaule);
    }
    Vector3 GetCornerNormalized(Vector3 origin, Vector3 vaule)
    {
        return (vaule - origin).normalized;
    }
    Vector3 GetCornerNormalized(Vector3 vaule)
    {
        return GetCornerNormalized(camera.transform.position, vaule);
    }



    /// <summary>
    /// Not Support Camera Roll
    /// </summary>
    public static bool IsRenderable(Vector3 cameraPos, Vector3 cameraForward, float fov, float aspect, Vector3 targetPos)
    {
        /*
        Unity.Mathematics.float2 targetDot = new()
        {
            x = Math.Dot(new Vector3(targetPos.x - cameraPos.x, 0 , targetPos.z - targetPos.z), cameraForward),
            y = Math.Dot(new Vector3(0, targetPos.y - cameraPos.y, targetPos.z - targetPos.z), cameraForward),
        };

        float2 cameraView = new float2
        {
            x = Camera.VerticalToHorizontalFieldOfView(fov, aspect) * 0.5f,
            y = fov * 0.5f
        };
        */

        return (Math.Dot(new Vector3(targetPos.x - cameraPos.x, 0, targetPos.z - cameraPos.z).normalized, cameraForward)
            <= (Camera.VerticalToHorizontalFieldOfView(fov, aspect) * 0.5f)) 
            &&
            (Math.Dot(new Vector3(0, targetPos.y - cameraPos.y, targetPos.z - cameraPos.z).normalized, cameraForward) <= (fov * 0.5f));
    }
    /// <summary>
    /// Not Support Camera Roll
    /// </summary>
    public static bool IsRenderable(Camera camera, Vector3 targetPos)
    {
        return IsRenderable(camera.transform.position, camera.transform.forward, camera.fieldOfView, camera.aspect, targetPos);
    }
    /// <summary>
    /// Not Support Camera Roll
    /// </summary>
    public static float2 IsRenderableRate(Vector3 cameraPos, Vector3 cameraForward, float fov, float aspect, Vector3 targetPos)
    {
        return new float2
        {
            x = (Math.Dot(new Vector3(targetPos.x - cameraPos.x, 0, targetPos.z - cameraPos.z).normalized, cameraForward)
                / (Camera.VerticalToHorizontalFieldOfView(fov, aspect) * 0.5f)),
            y = Math.Dot(new Vector3(0, targetPos.y - cameraPos.y, targetPos.z - cameraPos.z).normalized, cameraForward) / (fov * 0.5f)
        };
    }
    /// <summary>
    /// Not Support Camera Roll
    /// </summary>
    public static float2 IsRenderableRate(Camera camera, Vector3 targetPos)
    {
        return IsRenderableRate(camera.transform.position, camera.transform.forward, camera.fieldOfView, camera.aspect, targetPos);
    }
    #endregion

    public float GetCameraConeAngle(Quaternion cameraRot, float fov, float horizonFov)
    {
        return Math.Dot(cameraRot * Vector3.forward,
            (
                cameraRot * Vector3.forward
                + cameraRot * Vector3.up * math.tan(math.radians(fov * 0.5f))
                + cameraRot * Vector3.right * math.tan(math.radians(horizonFov * 0.5f))
            ).normalized);
    }
    public float GetCameraConeAngle(Camera camera)
    {
        return GetCameraConeAngle(camera.transform.rotation, camera.fieldOfView, Camera.VerticalToHorizontalFieldOfView(camera.fieldOfView, camera.aspect));
    }

    public Vector2 GetRenderAreaAngle(Vector3 cameraPos, Quaternion cameraRot, Vector3 target)
    {
        return new Vector2
        {
            x = Math.Dot((cameraRot * Vector3.forward), Vector3.ProjectOnPlane(target - cameraPos, cameraRot * Vector3.up)),
            y = Math.Dot((cameraRot * Vector3.forward), Vector3.ProjectOnPlane(target - cameraPos, cameraRot * Vector3.right))
        };
    }
    public Vector2 GetRenderAreaAngle(Camera camera, Vector3 target)
    {
        return GetRenderAreaAngle(camera.transform.position, camera.transform.rotation, target);
    }

    public bool IsRenderArea(Vector3 cameraPos, Quaternion cameraRot, float fov, float horizonFov, Vector3 target)
    {
        var angles = GetRenderAreaAngle(cameraPos, cameraRot, target);
        return (angles.x <= horizonFov * 0.5f) && (angles.y <= fov * 0.5f);
    }
    public bool IsRenderArea(Camera camera, Vector3 target)
    {
        return IsRenderArea(camera.transform.position, camera.transform.rotation, camera.fieldOfView,
            Camera.VerticalToHorizontalFieldOfView(camera.fieldOfView, camera.aspect), target);
    }

    public bool IsRenderAreaLite(Vector3 cameraPos, Quaternion cameraRot, float CameraConeAngle, float horizonFov, Vector3 target)
    {
        if (Math.Dot(cameraRot * Vector3.forward, (target - cameraPos).normalized) > CameraConeAngle)
        {
            return false;
        }else
        {
            return (horizonFov * 0.5 <= (Math.Dot((cameraRot * Vector3.forward), Vector3.ProjectOnPlane(target - cameraPos, cameraRot * Vector3.right))));
        }
    }
    public bool IsRenderAreaLite(Camera camera, Vector3 target)
    {
        return IsRenderAreaLite(camera.transform.position, camera.transform.rotation, camera.fieldOfView,
            Camera.VerticalToHorizontalFieldOfView(camera.fieldOfView, camera.aspect), target);
    }

    public Vector2 IsRenderAreaRate(Vector3 cameraPos, Quaternion cameraRot, float fov, float horizonFov, Vector3 target)
    {
        var angles = GetRenderAreaAngle(cameraPos, cameraRot, target);
        return new Vector2
        {
            x = angles.x / horizonFov,
            y = angles.y / fov
        };
    }
    public Vector2 IsRenderAreaRate(Camera camera, Vector3 target)
    {
        return IsRenderAreaRate(camera.transform.position, camera.transform.rotation, camera.fieldOfView,
            Camera.VerticalToHorizontalFieldOfView(camera.fieldOfView, camera.aspect), target);
    }
}
