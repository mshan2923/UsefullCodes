using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
//using Unity.Entities;

[ExecuteAlways]
public class CalculateBoxNormal : MonoBehaviour
{
    public GameObject Pointer;
    public GameObject TargetBox;

    public float PointerRadius = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Pointer == null || TargetBox == null)
            return;

        {
            /*        bool isCollision = true;

        var boxTransform = TargetBox.transform;
        var pointerTrans = Pointer.transform;
        var BoxMaxPos = boxTransform.position
            + (boxTransform.right * boxTransform.localScale.x + boxTransform.up * boxTransform.localScale.y + boxTransform.forward * boxTransform.localScale.z) * 0.5f;

        Debug.Log($" P : {pointerTrans.position} / B : {boxTransform.position} / M : {BoxMaxPos}");

        var ProjectPointerX = Vector3.Project(pointerTrans.position, boxTransform.right);
        var ProjectBoxX = Vector3.Project(boxTransform.position, boxTransform.right);
        var ProjectBoxMaxX = Vector3.Project(BoxMaxPos, boxTransform.right);
        var disX = Vector3.Magnitude(ProjectPointerX - ProjectBoxX) - Vector3.Magnitude(ProjectBoxMaxX - ProjectBoxX);
        if (disX > PointerRadius) { isCollision = false; return; }

        var ProjectPointerY = Vector3.Project(pointerTrans.position, boxTransform.up);
        var ProjectBoxY = Vector3.Project(boxTransform.position, boxTransform.up);
        var ProjectBoxMaxY = Vector3.Project(BoxMaxPos, boxTransform.up);
        var disY = Vector3.Magnitude(ProjectPointerY - ProjectBoxY) - Vector3.Magnitude(ProjectBoxMaxY - ProjectBoxY);
        if (disY > PointerRadius) { isCollision = false; return; }

        var ProjectPointerZ = Vector3.Project(pointerTrans.position, boxTransform.forward);
        var ProjectBoxZ = Vector3.Project(boxTransform.position, boxTransform.forward);
        var ProjectBoxMaxZ = Vector3.Project(BoxMaxPos, boxTransform.forward);
        var disZ = Vector3.Magnitude(ProjectPointerZ - ProjectBoxZ) - Vector3.Magnitude(ProjectBoxMaxZ - ProjectBoxZ);
        if (disZ > PointerRadius) { isCollision = false; return; }

        Debug.DrawLine(pointerTrans.position, boxTransform.position, Color.gray, Time.deltaTime);
        Debug.Log($"{(isCollision ? "Collision" : "Not")} {disX} , {disY} , {disZ}");

        {
            var ClosePoint = ((disX > 0) ? ProjectBoxMaxX : ProjectPointerX)
                + ((disY > 0) ? ProjectBoxMaxY : ProjectPointerY)
                + ((disZ > 0) ? ProjectBoxMaxZ : ProjectPointerZ);
            if (disX < 0 && disY < 0 && disZ < 0)
            {
                //중점이 box 안으로 들어 갔을때
                Debug.DrawLine(pointerTrans.position, 
                    pointerTrans.position - (boxTransform.position - pointerTrans.position).normalized
                    , Color.red, Time.deltaTime);
            }
            else
            {
                Debug.DrawLine(ClosePoint, pointerTrans.position, Color.red, Time.deltaTime);
            }
                //ProjectPointerX + ProjectPointerY + ProjectPointerZ;
        }*/
        }

        var isCollision = CalculationBoxOffsetOnhitSphere(new LocalTransform { Position = TargetBox.transform.position, Rotation = TargetBox.transform.rotation, Scale = 1},
            TargetBox.transform.localScale * 0.5f, Pointer.transform.position, PointerRadius, out var dir);

        var depth = "";
        switch(isCollision)
        {
            case -1:
                depth = "Sphere into Box";
                break;
            case 0:
                depth = (PointerRadius - dir.magnitude).ToString();
                break;
            case 1:
                depth = "Not Collision Distace : " + (dir.magnitude - PointerRadius).ToString();
                break;
        }
        Debug.Log($"isCollision : {(isCollision <= 0 ? "true" : "false")} / Direction : {dir} / depth : {depth}");
        Debug.DrawLine(Pointer.transform.position, (Pointer.transform.position - dir), Color.red, Time.deltaTime);
    }
    public static Vector3 GetRotatedBoxSize(LocalTransform transform, Vector3 extents)
    {
        Vector3[] points = new Vector3[]
        {
                transform.Right() * extents.x + transform.Up() * extents.y + transform.Forward() * extents.z,//ppp
                transform.Right() * extents.x + transform.Up() * extents.y - transform.Forward() * extents.z,//ppm
                transform.Right() * extents.x - transform.Up() * extents.y + transform.Forward() * extents.z,//pmp
                transform.Right() * extents.x - transform.Up() * extents.y - transform.Forward() * extents.z,//pmm
                Vector3.zero, Vector3.zero ,Vector3.zero, Vector3.zero
        };
        points[4] = -points[0];
        points[5] = -points[1];
        points[6] = -points[2];
        points[7] = -points[3];

        return new Vector3
        {
            x = points.Max(t => t.x) - points.Min(t => t.x),
            y = points.Max(t => t.y) - points.Min(t => t.y),
            z = points.Max(t => t.z) - points.Min(t => t.z)
        };
    }

    /// <summary>
    /// return  -1 -> Sphere orign into Box , 0 -> Collision , 1 -> Not Collision
    /// </summary>
    /// <param name="boxTransform"></param>
    /// <param name="boxExtent"></param>
    /// <param name="pointer"></param>
    /// <param name="pointerRadius"></param>
    /// <param name="Normal"></param>
    /// <returns></returns>
    public static int CalculationBoxOffsetOnhitSphere(LocalTransform boxTransform, float3 boxExtent, float3 pointer, float pointerRadius, out Vector3 Normal)
    {
        bool isCollision = true;

        var RotatedExtent = (boxTransform.Right() * boxExtent.x + boxTransform.Up() * boxExtent.y + boxTransform.Forward() * boxExtent.z);
        var BoxMaxPos = boxTransform.Position + RotatedExtent;

        var ProjectPointerX = math.project(pointer, boxTransform.Right());
        var ProjectBoxX = math.project(boxTransform.Position, boxTransform.Right());
        var ProjectBoxMaxX = math.project(BoxMaxPos, boxTransform.Right());
        var disX = Vector3.Magnitude(ProjectPointerX - ProjectBoxX) - Vector3.Magnitude(ProjectBoxMaxX - ProjectBoxX);
        if (disX > pointerRadius) { isCollision = false; }

        var ProjectPointerY = math.project(pointer, boxTransform.Up());
        var ProjectBoxY = math.project(boxTransform.Position, boxTransform.Up());
        var ProjectBoxMaxY = math.project(BoxMaxPos, boxTransform.Up());
        var disY = Vector3.Magnitude(ProjectPointerY - ProjectBoxY) - Vector3.Magnitude(ProjectBoxMaxY - ProjectBoxY);
        if (disY > pointerRadius) { isCollision = false; }

        var ProjectPointerZ = math.project(pointer, boxTransform.Forward());
        var ProjectBoxZ = math.project(boxTransform.Position, boxTransform.Forward());
        var ProjectBoxMaxZ = math.project(BoxMaxPos, boxTransform.Forward());
        var disZ = Vector3.Magnitude(ProjectPointerZ - ProjectBoxZ) - Vector3.Magnitude(ProjectBoxMaxZ - ProjectBoxZ);
        if (disZ > pointerRadius) { isCollision = false; }


        float3 ClosePoint = float3.zero;
        {
            var BoxMinPos = boxTransform.Position - RotatedExtent;
            var ProjectBoxMinX = math.project(BoxMinPos, boxTransform.Right());
            var ProjectBoxMinY = math.project(BoxMinPos, boxTransform.Up());
            var ProjectBoxMinZ = math.project(BoxMinPos, boxTransform.Forward());

            if (disX > 0)
                ClosePoint += (math.distance(ProjectPointerX, ProjectBoxMinX) < math.distance(ProjectPointerX, ProjectBoxMaxX))
                    ? ProjectBoxMinX
                    : ProjectBoxMaxX;
            else
                ClosePoint += ProjectPointerX;

            if (disY > 0)
                ClosePoint += (math.distance(ProjectPointerY, ProjectBoxMinY) < math.distance(ProjectPointerY, ProjectBoxMaxY))
                    ? ProjectBoxMinY
                    : ProjectBoxMaxY;
            else
                ClosePoint += ProjectPointerY;

            if (disZ > 0)
                ClosePoint += (math.distance(ProjectPointerZ, ProjectBoxMinZ) < math.distance(ProjectPointerZ, ProjectBoxMaxZ))
                    ? ProjectBoxMinZ
                    : ProjectBoxMaxZ;
            else
                ClosePoint += ProjectPointerZ;
        }


        if (disX < 0 && disY < 0 && disZ < 0)
        {
            //중점이 box 안으로 들어 갔을때
            Normal = math.normalize(-(boxTransform.Position - pointer));
            return -1;
        }
        else
        {
            Normal = pointer - ClosePoint;
            //ClosePointer = pointer - Normal
            return isCollision ? 0 : 1;
        }
    }
    public static bool CalculationBoxNormalOnhitSphere(LocalTransform boxTransform, float3 boxExtent, float3 pointer, float pointerRadius, out Vector3 Normal)
    {
        var RotatedExtent = (boxTransform.Right() * boxExtent.x + boxTransform.Up() * boxExtent.y + boxTransform.Forward() * boxExtent.z);
        var BoxMaxPos = boxTransform.Position + RotatedExtent;

        var ProjectPointerX = math.project(pointer, boxTransform.Right());
        var ProjectBoxX = math.project(boxTransform.Position, boxTransform.Right());
        var ProjectBoxMaxX = math.project(BoxMaxPos, boxTransform.Right());
        var disX = Vector3.Magnitude(ProjectPointerX - ProjectBoxX) - Vector3.Magnitude(ProjectBoxMaxX - ProjectBoxX);
        if (disX > pointerRadius) { Normal = Vector3.zero; return false; }

        var ProjectPointerY = math.project(pointer, boxTransform.Up());
        var ProjectBoxY = math.project(boxTransform.Position, boxTransform.Up());
        var ProjectBoxMaxY = math.project(BoxMaxPos, boxTransform.Up());
        var disY = Vector3.Magnitude(ProjectPointerY - ProjectBoxY) - Vector3.Magnitude(ProjectBoxMaxY - ProjectBoxY);
        if (disY > pointerRadius) { Normal = Vector3.zero; return false; }

        var ProjectPointerZ = math.project(pointer, boxTransform.Forward());
        var ProjectBoxZ = math.project(boxTransform.Position, boxTransform.Forward());
        var ProjectBoxMaxZ = math.project(BoxMaxPos, boxTransform.Forward());
        var disZ = Vector3.Magnitude(ProjectPointerZ - ProjectBoxZ) - Vector3.Magnitude(ProjectBoxMaxZ - ProjectBoxZ);
        if (disZ > pointerRadius) { Normal = Vector3.zero; return false; }

        float3 ClosePoint = float3.zero;
        {
            var BoxMinPos = boxTransform.Position - RotatedExtent;
            var ProjectBoxMinX = math.project(BoxMinPos, boxTransform.Right());
            var ProjectBoxMinY = math.project(BoxMinPos, boxTransform.Up());
            var ProjectBoxMinZ = math.project(BoxMinPos, boxTransform.Forward());

            if (disX > 0)
                ClosePoint += (math.distance(ProjectPointerX, ProjectBoxMinX) < math.distance(ProjectPointerX, ProjectBoxMaxX))
                    ? ProjectBoxMinX
                    : ProjectBoxMaxX;
            else
                ClosePoint += ProjectPointerX;

            if (disY > 0)
                ClosePoint += (math.distance(ProjectPointerY, ProjectBoxMinY) < math.distance(ProjectPointerY, ProjectBoxMaxY))
                    ? ProjectBoxMinY
                    : ProjectBoxMaxY;
            else
                ClosePoint += ProjectPointerY;

            if (disZ > 0)
                ClosePoint += (math.distance(ProjectPointerZ, ProjectBoxMinZ) < math.distance(ProjectPointerZ, ProjectBoxMaxZ))
                    ? ProjectBoxMinZ
                    : ProjectBoxMaxZ;
            else
                ClosePoint += ProjectPointerZ;
        }//Set ClosePoint

        if (disX < 0 && disY < 0 && disZ < 0)
        {
            //중점이 box 안으로 들어 갔을때
            Normal = math.normalize(-(boxTransform.Position - pointer));
        }
        else
        {
            Normal = pointer - ClosePoint;
            //ClosePointer = pointer - Normal
        }

        return true;
    }
}
