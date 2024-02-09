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

            Debug.DrawLine(camera.transform.position, GetResize(camera.transform.position, Target.transform.position)
                , Color.black, Time.deltaTime);

            // 카메라 중심에서 x,y축 최소 최대 dot 으로 카메라 영역 계산
            {
                /*
                 
            Vector2 CameraViewAngle = new Vector2
            {
                x = Math.Dot(GetCornerNormalized(corner[1]), GetCornerNormalized(corner[2])),
                y = Math.Dot(GetCornerNormalized(corner[0]), GetCornerNormalized(corner[1]))
            };
            CameraViewAngle *= 0.5f;

            Vector2 TargetDot = new
                (
                    Math.Dot(GetCornerNormalized(new Vector3(Target.transform.position.x, camera.transform.position.y, Target.transform.position.z)), camera.transform.forward),
                    Math.Dot(GetCornerNormalized(new Vector3(camera.transform.position.x, Target.transform.position.y, Target.transform.position.z)), camera.transform.forward)
                );


            Debug.Log($"CameraView X : {CameraViewAngle.x} , Y : {CameraViewAngle.y} / Target Dir : {GetCornerNormalized(Target.transform.position)} / Camera corner Dir {GetCornerNormalized(corner[2])}" +
                $"\n Target : x : {TargetDot.x} / Target : y : {TargetDot.y}");
            //=========== XX 
            // Normalized는 항상 길이가 1이 되므로 구형태 기반

            CameraViewAngle = new Vector2
                (
                    Camera.VerticalToHorizontalFieldOfView(camera.fieldOfView, camera.aspect) * 0.5f,
                    camera.fieldOfView * 0.5f
                );
            isVisible = (TargetDot.x <= CameraViewAngle.x) && (TargetDot.y <= CameraViewAngle.y);

            Debug.Log($"vertical : {CameraViewAngle.x} / Horizon : {CameraViewAngle.y}" +
                $"\n Visible : {isVisible} , x : {TargetDot.x / CameraViewAngle.x} / y : {TargetDot.y / CameraViewAngle.y}");
            // 이걸로 해야됨*/
            }//Disable

            Debug.Log($"Result : {IsRenderable(camera, Target.transform.position)} / Rate : {IsRenderableRate(camera, Target.transform.position)}");
        }
    }

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
}
