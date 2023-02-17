using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MathCollision : MonoBehaviour
{
    public GameObject Adirection;
    public GameObject Bdirection;
    public GameObject Hit2Ddirection;
    public GameObject Hitdirection;
    public GameObject ReflectA;
    public GameObject ReflectB;

    [Expand.AttributeLabel("")]
    public bool IsCollision = false;

    public float A_Mass = 1;
    public float B_Mass = 1;
    public struct collisionData
    {
        //public Vector3 Vector;
        public Quaternion Rot;
        public float Mass;
    }

    collisionData Adata;
    collisionData Bdata;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Adata = Trans(Adirection, A_Mass);
        Bdata = Trans(Bdirection, B_Mass);

        IsCollision = GetIntersct2D(Adirection.transform, Bdirection.transform, out Vector2 result);

        if (IsCollision)
            Hit2Ddirection.transform.localPosition = new Vector3(result.x, result.y, (Adirection.transform.position.z + Bdirection.transform.position.z) * 0.5f);
        else
        {
            //Intersection(out var resultTop, Adirection.transform.position, RemoveY(Adirection.transform.rotation * Vector3.forward),
            //    Bdirection.transform.position, RemoveY(Bdirection.transform.rotation * Vector3.forward));

            var posA = Adirection.transform.position;
            var posB = Bdirection.transform.position;
            var dirA = Adirection.transform.rotation * Vector3.forward;
            var dirB = Bdirection.transform.rotation * Vector3.forward;

            GetIntersctPoint2D(Convert3dToTop(posA), Convert3dToTop(posA + dirA), Convert3dToTop(posB), Convert3dToTop(posB + dirB), out var resultTop);
            Hit2Ddirection.transform.position = new Vector3(resultTop.x, 0, resultTop.y);

            Debug.LogWarning("Not Visible 2D");
        }

        {
            Debug.DrawLine(Adirection.transform.position + Convert3dTo2d(Adirection.transform.rotation * Vector3.back)
                , Adirection.transform.position + Convert3dTo2d(Adirection.transform.rotation * Vector3.forward), Color.yellow, Time.deltaTime);

            Debug.DrawLine(Bdirection.transform.position + Convert3dTo2d(Bdirection.transform.rotation * Vector3.back)
                , Bdirection.transform.position + Convert3dTo2d(Bdirection.transform.rotation * Vector3.forward), Color.yellow, Time.deltaTime);
        }//Debug 2D


        {
            Debug.DrawLine(Adirection.transform.position
                , Adirection.transform.position + Adirection.transform.rotation * Vector3.forward, Color.blue, Time.deltaTime);

            Debug.DrawLine(Bdirection.transform.position
                , Bdirection.transform.position + Bdirection.transform.rotation * Vector3.forward, Color.blue, Time.deltaTime);
        }//Debug 3D

        {
            var AddVecForward = (Adata.Rot * Vector3.forward * Adata.Mass + Bdata.Rot * Vector3.forward * Bdata.Mass).normalized;
            var AddVecUp = (Adata.Rot * Vector3.up * Adata.Mass + Bdata.Rot * Vector3.up * Bdata.Mass).normalized;

            var averPos = (Adirection.transform.position + Bdirection.transform.position) * 0.5f;
            var averRot = Quaternion.LookRotation(AddVecForward, AddVecUp);


            Debug.DrawLine(averPos, averPos + averRot * Vector3.forward * 0.1f, Color.blue, Time.deltaTime);
            Debug.DrawLine(averPos, averPos + averRot * Vector3.up * 0.1f, Color.green, Time.deltaTime);
            Debug.DrawLine(averPos, averPos + averRot * Vector3.right * 0.1f, Color.red, Time.deltaTime);
        }// A + B 방향 좌표계


        var cloests = GetCloestLines(Adirection.transform.position, Adirection.transform.rotation * Vector3.forward,
            Bdirection.transform.position, Bdirection.transform.rotation * Vector3.forward);

        //접점 OR 가까운지점 평균지점에 , 방향백터 평균을 기준으로 서로 반사 


        ReflectA.transform.SetPositionAndRotation(cloests[0], GetCollisionReflect(Adata, Bdata));
        ReflectB.transform.SetPositionAndRotation(cloests.Length >= 2 ? cloests[1] : cloests[0], GetCollisionReflect(Bdata, Adata));


        //교점 OR 중심점 , Adata, Bdata  ==> 반사 방향

    }

    collisionData Trans(GameObject obj , float mass)
    {
        return new collisionData
        {
            Rot = obj.transform.rotation,
            Mass = mass
        };
    }
    Vector3 Convert3dTo2d(Vector3 dir)
    {
        return Vector3.Normalize(dir * Vector2.one);
    }// (x,y,z) >> (x,y)
    Vector3 Convert3dToTop(Vector3 dir)
    {
        return (new Vector3(dir.x, dir.z));
    }// (x,y,z) >> (x,z)

    bool GetIntersctPoint2D(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 result)
    {
        float incA = 0, conA = 0, sameA = 0;
        float incB = 0, conB = 0, sameB = 0;

        if (Mathf.Approximately(a1.x, a2.x) && Mathf.Approximately(a1.y, a2.y))
        {
            result = Vector2.zero;
            return false;
        }
        if (Mathf.Approximately(b1.x, b2.x) && Mathf.Approximately(b1.y, b2.y))
        {
            result = Vector2.zero;
            return false;
        }//입력된 값이 선이 아닐때

        if (Mathf.Approximately(a1.x, a2.x))//Y축과 평행
            sameA = a1.x;
        else
        {
            incA = (a2.y - a1.y) / (a2.x - a1.x);   //기울기 구하기
            conA = a1.y - incA * a1.x;              // 상수값
        }
        if (b1.x == b2.x)//Y축과 평행
            sameB = b1.x;
        else
        {
            incB = (b2.y - b1.y) / (b2.x - b1.x);   //기울기 구하기
            conB = b1.y - incB * b1.x;              // 상수값
        }

        if (Mathf.Approximately(a1.x, a2.x) && Mathf.Approximately(b1.x, b2.x))
        {
            result = new Vector2(1, 0);
            return false;
        }//X 평행
        if (Mathf.Approximately(a1.x, a2.x))
        {
            result = new Vector2(sameA, incB * sameA + conB);//A가 y축 평행시
            return true;
        }
        else if (Mathf.Approximately(b1.x, b2.x))
        {
            result = new Vector2(sameB, incA * sameB + conA);//B가 y축 평행시
            return true;
        }
        else if (Mathf.Approximately((incA - incB), 0))
        {
            result = new Vector2(0 , conA);
            return false;
        }// X축 평행
        else
        {
            float t_inc = -(conA - conB) / (incA - incB);
            result = new Vector2(t_inc, incA * t_inc + conA);
            return true;
        }

    }
    bool GetIntersct2D(Transform Apoint , Transform Bpoint, out Vector2 result)
    {
        return GetIntersctPoint2D(Apoint.localPosition, Apoint.localPosition + Apoint.rotation * Vector3.forward,
            Bpoint.localPosition, Bpoint.localPosition + Bpoint.rotation * Vector3.forward, out result);
    }
    bool GetIntersctTop(Transform Apoint, Transform Bpoint, out Vector2 result)
    {
        return GetIntersctPoint2D(Apoint.localPosition, Convert3dToTop(Apoint.localPosition + Apoint.rotation * Vector3.forward),
            Bpoint.localPosition, Convert3dToTop(Bpoint.localPosition + Bpoint.rotation * Vector3.forward), out result);
    }//NotWork

    public static bool Intersection(out Vector3 intersection, Vector3 linePoint1,
        Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parallel
        if (Mathf.Abs(planarFactor * Mathf.Rad2Deg) < 0.1f && crossVec1and2.sqrMagnitude > 0.001f)
        //if (Mathf.Abs(planarFactor * Mathf.Rad2Deg) < 0.1f + "오차 각도" && crossVec1and2.sqrMagnitude > 0.001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2)
                    / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);

            return true;
        }
        else
        {

            intersection = Vector3.zero;
            return false;
        }
    }
    public Quaternion GetCollisionReflect(collisionData Target, collisionData Other)
    {
        var AddVecForward = (Target.Rot * Vector3.forward * Target.Mass + Other.Rot * Vector3.forward * Other.Mass).normalized;
        var AddVecUp = (Target.Rot * Vector3.up * Target.Mass + Other.Rot * Vector3.up * Other.Mass).normalized;

        var AddVecRot = Quaternion.LookRotation(AddVecForward, AddVecUp);

        return Quaternion.LookRotation(Vector3.Reflect(Target.Rot * Vector3.forward, AddVecRot * Vector3.right));
    }// 충돌을 했을때 기준 방향


    public Vector3[] GetCloestLines(Vector3 Point1, Vector3 Vec1, Vector3 Point2, Vector3 Vec2, float intersectLength = 0.01f)
    {
        Vector3 Cloest1 = Vector3.zero;
        Vector3 Cloest2 = Vector3.zero;

        {
            Vector3 Vec3 = Point2 - Point1;
            Vector3 crossVec1and2 = Vector3.Cross(Vec1, Vec2);
            Vector3 crossVec3and2 = Vector3.Cross(Vec3, Vec2);

            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            Cloest1 = Point1 + (Vec1 * s);
        }
        {
            Vector3 vec3 = Point1 - Point2;
            Vector3 cross1N2 = Vector3.Cross(Vec2, Vec1);
            Vector3 cross3N1 = Vector3.Cross(vec3, Vec1);

            float t = Vector3.Dot(cross3N1, cross1N2) / cross1N2.sqrMagnitude;
            Cloest2 = Point2 + Vec2 * t;
        }

        if ((Cloest1 - Cloest2).sqrMagnitude < (intersectLength * intersectLength))
        {
            return new Vector3[] { Cloest1 };
        }else
        {
            return new Vector3[] { Cloest1 , Cloest2 };
        }
    }
    public float GetCloestLinesDistance(Vector3 Point1, Vector3 Vec1, Vector3 Point2, Vector3 Vec2, float intersectLength = 0.01f, bool sqr = true)
    {
        Vector3 Cloest1 = Vector3.zero;
        Vector3 Cloest2 = Vector3.zero;

        {
            Vector3 Vec3 = Point2 - Point1;
            Vector3 crossVec1and2 = Vector3.Cross(Vec1, Vec2);
            Vector3 crossVec3and2 = Vector3.Cross(Vec3, Vec2);

            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            Cloest1 = Point1 + (Vec1 * s);
        }
        {
            Vector3 vec3 = Point1 - Point2;
            Vector3 cross1N2 = Vector3.Cross(Vec2, Vec1);
            Vector3 cross3N1 = Vector3.Cross(vec3, Vec1);

            float t = Vector3.Dot(cross3N1, cross1N2) / cross1N2.sqrMagnitude;
            Cloest2 = Point2 + Vec2 * t;
        }

        if ((Cloest1 - Cloest2).sqrMagnitude < (intersectLength * intersectLength))
        {
            return 0;
        }
        else
        {
            return sqr ? (Cloest1 - Cloest2).sqrMagnitude : (Cloest1 - Cloest2).magnitude;
        }
    }
    public Vector3 GetCollisionReflect(Quaternion Target, Quaternion Other, float TargetMass, float OtherMass)
    {
        var AddVecForward = (Target * Vector3.forward * TargetMass + Other * Vector3.forward * OtherMass).normalized;
        var AddVecUp = (Target * Vector3.up * TargetMass + Other * Vector3.up * OtherMass).normalized;

        var AddVecRot = Quaternion.LookRotation(AddVecForward, AddVecUp);

        return Quaternion.LookRotation(Vector3.Reflect(Target * Vector3.forward, AddVecRot * Vector3.right)) * Vector3.forward * (TargetMass / (TargetMass + OtherMass));
    }// 충돌을 했을때 기준 방향
}
