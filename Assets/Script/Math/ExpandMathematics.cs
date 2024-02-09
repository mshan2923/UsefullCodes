using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Mathematics
{
    
    /// <summary>
    /// Can't use operator
    /// </summary>
    public class ExpandMathematics
    {
        #region float3
        public static float3 Multi(float3 lhs, Vector3 rhs)
        {
            return new float3(lhs.x * rhs.x, lhs.y * rhs.y, rhs.z * lhs.z);
        }
        public static float3 Multi(Vector3 lhs, Vector3 rhs)
        {
            return new float3(lhs.x * rhs.x, lhs.y * rhs.y, rhs.z * lhs.z);
        }

        public static float3 Add(float3 lhs, Vector3 rhs)
        {
            return new float3(lhs.x + rhs.x, lhs.y + rhs.y, rhs.z + lhs.z);
        }
        public static float3 Add(Vector3 lhs, Vector3 rhs)
        {
            return new float3(lhs.x + rhs.x, lhs.y + rhs.y, rhs.z + lhs.z);
        }

        public static float3 Subtraction(float3 lhs, Vector3 rhs)
        {
            return new float3(lhs.x - rhs.x, lhs.y - rhs.y, rhs.z - lhs.z);
        }
        public static float3 Subtraction(Vector3 lhs, float3 rhs)
        {
            return new float3(lhs.x - rhs.x, lhs.y - rhs.y, rhs.z - lhs.z);
        }
        public static float3 Subtraction(Vector3 lhs, Vector3 rhs)
        {
            return new float3(lhs.x - rhs.x, lhs.y - rhs.y, rhs.z - lhs.z);
        }

        public static float3 Division(float3 lhs, Vector3 rhs)
        {
            return new float3(lhs.x / rhs.x, lhs.y / rhs.y, rhs.z / lhs.z);
        }
        public static float3 Division(Vector3 lhs, float3 rhs)
        {
            return new float3(lhs.x / rhs.x, lhs.y / rhs.y, rhs.z / lhs.z);
        }
        public static float3 Division(Vector3 lhs, Vector3 rhs)
        {
            return new float3(lhs.x / rhs.x, lhs.y / rhs.y, rhs.z / lhs.z);
        }

        public static float3 Convert(Vector3 value)
        {
            return new float3(value.x, value.y, value.z);
        }
        public static Vector3 Convert(float3 value)
        {
            return new Vector3(value.x, value.y, value.z);
        }
        #endregion

        public static Quaternion ToQuaternion(float4 value)
        {
            return new Quaternion(value.x, value.y, value.z, value.w);
        }
        public static float4 FormQuaternion(Quaternion value)
        {
            return new float4(value.x, value.y, value.z, value.w);
        }

        public static float4 Multi(float4 lhs, float4 rhs)
        {
            return FormQuaternion(ToQuaternion(lhs) * ToQuaternion(rhs));
        }
        public static float4 Multi(Quaternion lhs, float4 rhs)
        {
            return FormQuaternion(lhs * ToQuaternion(rhs));
        }
        public static float4 Multi(float4 lhs, Quaternion rhs)
        {
            return FormQuaternion(ToQuaternion(lhs) * rhs);
        }

        //public static float3 operator *(quaternion rotation, float3 point)
        public static float3 Multi(quaternion rotation, float3 point)
        {
            float num = rotation.value.x * 2f;
            float num2 = rotation.value.y * 2f;
            float num3 = rotation.value.z * 2f;
            float num4 = rotation.value.x * num;
            float num5 = rotation.value.y * num2;
            float num6 = rotation.value.z * num3;
            float num7 = rotation.value.x * num2;
            float num8 = rotation.value.x * num3;
            float num9 = rotation.value.y * num3;
            float num10 = rotation.value.w * num;
            float num11 = rotation.value.w * num2;
            float num12 = rotation.value.w * num3;
            float3 result = default(float3);
            result.x = (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
            result.y = (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
            result.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1f - (num4 + num5)) * point.z;
            return result;
        }
        public static quaternion Multi(quaternion lhs, quaternion rhs)
        {
            return new quaternion(lhs.value.w * rhs.value.x + lhs.value.x * rhs.value.w + lhs.value.y * rhs.value.z - lhs.value.z * rhs.value.y, lhs.value.w * rhs.value.y + lhs.value.y * rhs.value.w + lhs.value.z * rhs.value.x - lhs.value.x * rhs.value.z, lhs.value.w * rhs.value.z + lhs.value.z * rhs.value.w + lhs.value.x * rhs.value.y - lhs.value.y * rhs.value.x, lhs.value.w * rhs.value.w - lhs.value.x * rhs.value.x - lhs.value.y * rhs.value.y - lhs.value.z * rhs.value.z);
        }
    }
    

}
