using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectScreenSize : MonoBehaviour
{
    public enum E_CalculateAxis
    {
        x , y, Both
    }
    //휴머노이드 인경우 다르게 계산(계산 최소화) / 인식감소 시작구간만 정해주면 크기(지름 * 높이) * 거리에 비례해서 
    public E_CalculateAxis CalculateAxis = E_CalculateAxis.x;

    [SerializeField , Tooltip("if GetCamera is Null , Set to Camera.main")]
    Camera getCamera;
    public Camera GetCamera
    {
        get
        {
            if (getCamera)
            {
                return getCamera;
            }else
            {
                getCamera = Camera.main;
                return getCamera;
            }
        }
    }

    public GameObject Spector = null;
    public GameObject Target = null;

    Bounds Bound;
    public Vector3 RealSize;
    public float Distance;
    public float XDirection;
    public Vector2 LookSize;

    public float VH = 0;
    public float XLookForwardAngle = 0;
    public float NormalHeight2D = 0;
    public float NormalHeight = 0;

    public bool XLookForward = false;
    public bool YLookForward = false;

    public float ScreenSize = 0;

    float[] CalculAngle = new float[4];
    Vector3 View_NP;

    /// <summary>
    /// Local Get LookSize
    /// </summary>
    public void SetLookSize()
    {
        if (Target.GetComponent<Renderer>())
        {
            Bound = Target.GetComponent<Renderer>().bounds;
            RealSize = Bound.size;
            Vector3 Lpos = Spector.transform.position - Target.transform.position;

            Distance = new Vector3(Lpos.x, 0, Lpos.z).magnitude;//Vector3.Distance(Spector.transform.position, Target.transform.position);

            NormalHeight2D = Mutiply((Bound.ClosestPoint(Spector.transform.position) - Spector.transform.position), new Vector3(1, 0, 1)).magnitude;//Y값무시
            NormalHeight = (Bound.ClosestPoint(Spector.transform.position) - Spector.transform.position).magnitude;

            XDirection = Vector3.Angle(Vector3.forward, (new Vector3(Lpos.x, 0, Lpos.z)));
            //YDirection = Vector3.Angle(Vector3.forward, (Vector3.forward * Distance + new Vector3(0, Lpos.y, 0)));

            VH = Distance * Mathf.Cos(GetLookAngle(XDirection) * Mathf.Deg2Rad);// VH는 LookForwardAngle이 0 일때 Distance과 같음 , 정면일때 바라보고있는 면과 수직


            {
                if (XDirection / 45 == 1 || XDirection / 45 == 2)
                {
                    XLookForwardAngle = Mathf.Atan(RealSize.x / VH) * Mathf.Rad2Deg;
                }
                else
                {
                    XLookForwardAngle = Mathf.Atan(RealSize.z / VH) * Mathf.Rad2Deg;
                }
            }//Set XLookForwardAngle

            {
                if (XDirection < 45)
                {
                    XLookForward = InRange(XDirection, 0, XLookForwardAngle * 0.5f, true);
                }
                else if (XDirection >= 45 && XDirection < 135)
                {
                    XLookForward = InRange(XDirection, 90 - XLookForwardAngle * 0.5f, 90 + XLookForwardAngle * 0.5f, true);
                }
                else
                {
                    XLookForward = InRange(XDirection, 180 - XLookForwardAngle * 0.5f, 180 + XLookForwardAngle * 0.5f, true);
                }
            }//Set XLookForward

            YLookForward = InRange(Lpos.y, Bound.min.y, Bound.max.y, true);

            if (XLookForward)
            {
                if (Mathf.FloorToInt(XDirection / 45) is 1 or 2)
                {
                    LookSize.x = RealSize.z;
                }
                else
                {
                    LookSize.x = RealSize.x;
                }
            }
            else
            {
                View_NP = Spector.transform.position - Bound.ClosestPoint(Spector.transform.position);
                CalculAngle[0] = Mathf.Atan2(Mathf.Abs(View_NP.x), Mathf.Abs(View_NP.z)) * Mathf.Rad2Deg;
                CalculAngle[1] = Mathf.Atan2(Mathf.Abs(View_NP.x) + +RealSize.x, Mathf.Abs(View_NP.z)) * Mathf.Rad2Deg;

                //Mathf.Tan(CalculAngle[1] - CalculAngle[0]) * Mathf.Rad2Deg * NormalHeight2D// Line PA

                CalculAngle[2] = Mathf.Atan2(Mathf.Abs(View_NP.z), Mathf.Abs(View_NP.x)) * Mathf.Rad2Deg;
                CalculAngle[3] = Mathf.Atan2(Mathf.Abs(View_NP.z) + RealSize.z, Mathf.Abs(View_NP.x)) * Mathf.Rad2Deg;

                //Mathf.Tan((CalculAngle[3] - CalculAngle[2]) * Mathf.Deg2Rad) * NormalHeight2D// Lien PB

                LookSize.x = Mathf.Abs(Mathf.Tan((CalculAngle[1] - CalculAngle[0]) * Mathf.Deg2Rad) * NormalHeight2D)
                    + Mathf.Abs(Mathf.Tan((CalculAngle[3] - CalculAngle[2]) * Mathf.Deg2Rad) * NormalHeight2D);

            }

            if (YLookForward)
            {
                LookSize.y = RealSize.y;

            }
            else
            {
                float PF = 0;
                if (Mathf.FloorToInt(XDirection / 45) is 1 or 2)
                {
                    PF = RealSize.x / Mathf.Cos(GetLookAngle(XDirection) * Mathf.Deg2Rad);
                }
                else
                {
                    PF = RealSize.z / Mathf.Cos(GetLookAngle(XDirection) * Mathf.Deg2Rad);
                }

                float T2B = Mathf.Max((Bound.max.y - Spector.transform.position.y), (Spector.transform.position.y - Bound.min.y));
                CalculAngle[0] = Mathf.Atan2(T2B, NormalHeight2D) * Mathf.Rad2Deg;
                CalculAngle[1] = Mathf.Atan2(Mathf.Abs(Lpos.y), NormalHeight2D) * Mathf.Rad2Deg;

                //(NormalHeight2D * Mathf.Tan((CalculAngle[1] - CalculAngle[0]) * Mathf.Rad2Deg));//Line LP

                CalculAngle[2] = Mathf.Atan2(NormalHeight2D + PF, Lpos.y) * Mathf.Rad2Deg;
                CalculAngle[3] = Mathf.Atan2(NormalHeight2D, Lpos.y) * Mathf.Rad2Deg;

                //(NormalHeight * Mathf.Tan((CalculAngle[2] - CalculAngle[3]) * Mathf.Rad2Deg));// Line PR

                LookSize.y = (NormalHeight2D * Mathf.Tan((CalculAngle[0] - CalculAngle[1]) * Mathf.Deg2Rad))
                    + (NormalHeight * Mathf.Tan(Mathf.Abs(CalculAngle[2] - CalculAngle[3]) * Mathf.Deg2Rad));
            }

        }
    }

    public void Calculation()
    {
        if (Target.GetComponent<Renderer>())
        {
            {
                Bound = Target.GetComponent<Renderer>().bounds;
                RealSize = Bound.size;
                Vector3 Lpos = Spector.transform.position - Target.transform.position;

                Distance = new Vector3(Lpos.x, 0, Lpos.z).magnitude;

                NormalHeight2D = Mutiply((Bound.ClosestPoint(Spector.transform.position) - Spector.transform.position), new Vector3(1, 0, 1)).magnitude;//Y값무시
                NormalHeight = (Bound.ClosestPoint(Spector.transform.position) - Spector.transform.position).magnitude;

                XDirection = Vector3.Angle(Vector3.forward, (new Vector3(Lpos.x, 0, Lpos.z)));
                //YDirection = Vector3.Angle(Vector3.forward, (Vector3.forward * Distance + new Vector3(0, Lpos.y, 0)));

                VH = Distance * Mathf.Cos(GetLookAngle(XDirection) * Mathf.Deg2Rad);// VH는 LookForwardAngle이 0 일때 Distance과 같음 , 정면일때 바라보고있는 면과 수직
            }//SetUp

            switch (CalculateAxis)
            {
                case E_CalculateAxis.x:
                    {
                        XCalculate();
                        ScreenSize = GetScreenSize_Horizon();
                        break;
                    }
                case E_CalculateAxis.y:
                    {
                        YCalculate();
                        ScreenSize = GetScreenSize_Vertical();
                        break;
                    }
                case E_CalculateAxis.Both:
                    {
                        XCalculate();
                        YCalculate();

                        ScreenSize = Mathf.Clamp01(GetScreenSize_Horizon()) * Mathf.Clamp01(GetScreenSize_Vertical());
                        break;
                    }
            }
        }
    }

    public void XCalculate()
    {
        {
            if (XDirection / 45 == 1 || XDirection / 45 == 2)
            {
                XLookForwardAngle = Mathf.Atan(RealSize.x / VH) * Mathf.Rad2Deg;
            }
            else
            {
                XLookForwardAngle = Mathf.Atan(RealSize.z / VH) * Mathf.Rad2Deg;
            }
        }//Set XLookForwardAngle

        {
            if (XDirection < 45)
            {
                XLookForward = InRange(XDirection, 0, XLookForwardAngle * 0.5f, true);
            }
            else if (XDirection >= 45 && XDirection < 135)
            {
                XLookForward = InRange(XDirection, 90 - XLookForwardAngle * 0.5f, 90 + XLookForwardAngle * 0.5f, true);
            }
            else
            {
                XLookForward = InRange(XDirection, 180 - XLookForwardAngle * 0.5f, 180 + XLookForwardAngle * 0.5f, true);
            }
        }//Set XLookForward

        if (XLookForward)
        {
            if (Mathf.FloorToInt(XDirection / 45) is 1 or 2)
            {
                LookSize.x = RealSize.z;
            }
            else
            {
                LookSize.x = RealSize.x;
            }
        }
        else
        {
            View_NP = Spector.transform.position - Bound.ClosestPoint(Spector.transform.position);
            CalculAngle[0] = Mathf.Atan2(Mathf.Abs(View_NP.x), Mathf.Abs(View_NP.z)) * Mathf.Rad2Deg;
            CalculAngle[1] = Mathf.Atan2(Mathf.Abs(View_NP.x) + +RealSize.x, Mathf.Abs(View_NP.z)) * Mathf.Rad2Deg;

            //Mathf.Tan(CalculAngle[1] - CalculAngle[0]) * Mathf.Rad2Deg * NormalHeight2D// Line PA

            CalculAngle[2] = Mathf.Atan2(Mathf.Abs(View_NP.z), Mathf.Abs(View_NP.x)) * Mathf.Rad2Deg;
            CalculAngle[3] = Mathf.Atan2(Mathf.Abs(View_NP.z) + RealSize.z, Mathf.Abs(View_NP.x)) * Mathf.Rad2Deg;

            //Mathf.Tan((CalculAngle[3] - CalculAngle[2]) * Mathf.Deg2Rad) * NormalHeight2D// Lien PB

            LookSize.x = Mathf.Abs(Mathf.Tan((CalculAngle[1] - CalculAngle[0]) * Mathf.Deg2Rad) * NormalHeight2D)
                + Mathf.Abs(Mathf.Tan((CalculAngle[3] - CalculAngle[2]) * Mathf.Deg2Rad) * NormalHeight2D);

        }
    }

    public void YCalculate()
    {
        Vector3 Lpos = Spector.transform.position - Target.transform.position;
        YLookForward = InRange(Lpos.y, Bound.min.y, Bound.max.y, true);

        if (YLookForward)
        {
            LookSize.y = RealSize.y;

        }
        else
        {
            float PF = 0;
            if (Mathf.FloorToInt(XDirection / 45) is 1 or 2)
            {
                PF = RealSize.x / Mathf.Cos(GetLookAngle(XDirection) * Mathf.Deg2Rad);
            }
            else
            {
                PF = RealSize.z / Mathf.Cos(GetLookAngle(XDirection) * Mathf.Deg2Rad);
            }

            float T2B = Mathf.Max((Bound.max.y - Spector.transform.position.y), (Spector.transform.position.y - Bound.min.y));
            CalculAngle[0] = Mathf.Atan2(T2B, NormalHeight2D) * Mathf.Rad2Deg;
            CalculAngle[1] = Mathf.Atan2(Mathf.Abs(Lpos.y), NormalHeight2D) * Mathf.Rad2Deg;

            //(NormalHeight2D * Mathf.Tan((CalculAngle[1] - CalculAngle[0]) * Mathf.Rad2Deg));//Line LP

            CalculAngle[2] = Mathf.Atan2(NormalHeight2D + PF, Lpos.y) * Mathf.Rad2Deg;
            CalculAngle[3] = Mathf.Atan2(NormalHeight2D, Lpos.y) * Mathf.Rad2Deg;

            //(NormalHeight * Mathf.Tan((CalculAngle[2] - CalculAngle[3]) * Mathf.Rad2Deg));// Line PR

            LookSize.y = (NormalHeight2D * Mathf.Tan((CalculAngle[0] - CalculAngle[1]) * Mathf.Deg2Rad))
                + (NormalHeight * Mathf.Tan(Mathf.Abs(CalculAngle[2] - CalculAngle[3]) * Mathf.Deg2Rad));
        }
    }

    public float GetNormalHeight_Horizon(float ScreenSizeRate)
    {
        //Mathf.Tan(세로 FOV * 화면 WH 비율 * 라디안 전환 * 0.5f)

        float CameraAngle = (Camera.VerticalToHorizontalFieldOfView(getCamera.fieldOfView, getCamera.aspect) * 0.5f) + 15;
        return LookSize.x / (2 * Mathf.Tan(CameraAngle * Mathf.Deg2Rad) * ScreenSizeRate);
    }
    public float GetScreenSize_Horizon()
    {
        float CameraAngle = (Camera.VerticalToHorizontalFieldOfView(getCamera.fieldOfView, getCamera.aspect) * 0.5f) + 15;
        return LookSize.x / (2 * Mathf.Tan(CameraAngle * Mathf.Deg2Rad) * NormalHeight);
    }
    public float GetScreenSize_Vertical()
    {
        return LookSize.y / (2 * Mathf.Tan(getCamera.fieldOfView * Mathf.Deg2Rad * 0.5f) * NormalHeight);
    }

    /// <summary>
    ///  Angle : 0 ~ 180 return -45 ~ 45
    /// </summary>
    /// <param name="Angle"></param>
    /// <returns></returns>
    public static float GetLookAngle(float Angle)
    {
        if (Angle < 45)
        {
            return Angle;
        }
        else if (Angle >= 45 && Angle < 135)
        {
            return Mathf.Abs(90 - Angle);
        }
        else
        {
            return Mathf.Abs(180 - Angle);
        }
    }
    public static bool InRange(float vaule, float min, float max, bool InclueBorder = false)
    {
        if (vaule > min && vaule < max)
            return true;
        else if (Mathf.Approximately(vaule, min) || Mathf.Approximately(vaule, max))
        {
            return InclueBorder;
        }

        return false;
    }

    public static Vector3 Mutiply(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ObjectScreenSize))]
public class ObjectScreenSizeEditor : Editor
{
    ObjectScreenSize onwer;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        onwer = target as ObjectScreenSize;

        //onwer.Calculation();
    }
}
#endif
