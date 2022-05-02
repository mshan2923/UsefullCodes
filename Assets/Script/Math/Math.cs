using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Math
{
    /// <summary>
    /// Return To Degrees
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
    public static float Dot(Vector3 A, Vector3 B)
    {
        return Mathf.Acos(Vector3.Dot(A, B)) * Mathf.Rad2Deg;
    }
    /// <summary>
    /// Closest On infinity Length Line
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="Target"></param>
    /// <returns></returns>
    public static Vector3 ClosePointOnDirection(Vector3 origin, Vector3 direction, Vector3 Target)
    {
        return origin + Vector3.Project(Target - origin, direction);
        // line_start + Vector3.Project(point - line_start, line_end - line_start);

        /*
        Vector3 lhs = Target - origin;
        float dotP = Vector3.Dot(lhs, direction);
        return origin + direction * dotP;
        *///=====Other Way
    }

    // For finite lines:
    public static Vector3 ClosePointOnLine(Vector3 origin, Vector3 end, Vector3 target)
    {
        Vector3 direction = end - origin;
        float length = direction.magnitude;
        direction.Normalize();
        float project_length = Mathf.Clamp(Vector3.Dot(target - origin, direction), 0f, length);
        return origin + direction * project_length;
    }
    public static bool InRange(float vaule, float min, float max, bool equalPass = true)
    {
        if (equalPass)
        {
            return vaule >= min && vaule <= max;
        }else
        {
            return vaule > min && vaule < max;
        }
    }
    public static bool InRange(Vector3 vaule, Vector3 min, Vector3 max, bool equalPass = true)
    {
        return InRange(vaule.x, min.x, max.x, equalPass)
            && InRange(vaule.y, min.y, max.y, equalPass)
            && InRange(vaule.z, min.z, max.z, equalPass);
    }
}
