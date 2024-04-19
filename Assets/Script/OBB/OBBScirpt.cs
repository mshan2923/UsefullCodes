using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Networking.UnityWebRequest;
using static UnityEngine.UI.Image;

//Reference : https://www.slideshare.net/KooKyeongWon/ss-6716176
//  https://velog.io/@sjhbelieve/Unity%EC%B6%A9%EB%8F%8C%EC%B2%98%EB%A6%AC
//https://sanghoon23.tistory.com/82
[ExecuteAlways]
public class OBBScirpt : MonoBehaviour
{
    public GameObject Obb0;
    public GameObject Obb1;

    public BoxCollider Collider0;
    public BoxCollider Collider1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Obb0 == null || Obb1 == null)
            return;

        if (Collider0 == null)
        {
            Collider0 = Obb0.GetComponent<BoxCollider>();
        }
        if (Collider1 == null)
        {
            Collider1 = Obb1.GetComponent<BoxCollider>();
        }

        /*
        Debug.Log($"{Collider0.bounds.center} / {Collider0.bounds.extents} \n " +
            $"x : {Vector3.Project(Collider0.bounds.center, Obb0.transform.right)}, " +
            $"y : {Vector3.Project(Collider0.bounds.center, Obb0.transform.up)}," +
            $"z : {Vector3.Project(Collider0.bounds.center, Obb0.transform.forward)}");//로컬 좌표계 전환
        */



        {
            
            var Ax = Obb0.transform.rotation * Vector3.right;//origin right
            var Ay = Obb0.transform.rotation * Vector3.up;//origin up
            var Az = Obb0.transform.rotation * Vector3.forward;//origin forward

            var Bx = Obb1.transform.rotation * Vector3.right;//Target right
            var By = Obb1.transform.rotation * Vector3.up;//Target up
            var Bz = Obb1.transform.rotation * Vector3.forward;//Target forward

            var resultLog = new Dictionary<string, bool>();
            bool result = true;

            /*


            var a = Obb0.transform.rotation * Collider0.bounds.extents;//Collider0.bounds.extents;
            var b = Obb1.transform.rotation * Collider1.bounds.extents;
            var D = (Obb0.transform.position - Obb1.transform.position).magnitude;





            var AxisList = new List<Vector3>() { Ax, Ay, Az, Bx, By, Bz};
            for (int i = 0; i < 2; i++)
            {
                var origin = Vector3.Project(Obb0.transform.position, AxisList[i]);
                var target = Vector3.Project(Obb1.transform.position, AxisList[i]);

                var originBound = Vector3.Project(Obb0.transform.rotation * Collider0.bounds.max, AxisList[i]) - origin;
                var targetBound = Vector3.Project(Obb0.transform.rotation * Collider1.bounds.max, AxisList[i]) - target;
                var distace = target - origin;

                result &= distace.magnitude < (originBound.magnitude + targetBound.magnitude);

                var text = "";
                switch (i)
                {
                    case 0:
                        text = "Ax";
                        break;
                    case 1:
                        text = "Ay";
                        break;
                    case 2:
                        text = "Az";
                        break;
                    case 3:
                        text = "Bx";
                        break;
                    case 4:
                        text = "By";
                        break;
                    case 5:
                        text = "Bz";
                        break;
                }
                resultLog.Add(text, distace.magnitude < (originBound.magnitude + targetBound.magnitude));
            }

            for (int i = 2; i < AxisList.Count; i++)
            {
                var origin = Vector3.Project(Obb0.transform.position, AxisList[i]);
                var target = Vector3.Project(Obb1.transform.position, AxisList[i]);

                var originBound = Vector3.Project(Obb1.transform.rotation * Collider0.bounds.max, AxisList[i]) - origin;
                var targetBound = Vector3.Project(Obb1.transform.rotation * Collider1.bounds.max, AxisList[i]) - target;
                var distace = target - origin;

                result &= distace.magnitude < (originBound.magnitude + targetBound.magnitude);

                var text = "";
                switch (i)
                {
                    case 0:
                        text = "Ax";
                        break;
                    case 1:
                        text = "Ay";
                        break;
                    case 2:
                        text = "Az";
                        break;
                    case 3:
                        text = "Bx";
                        break;
                    case 4:
                        text = "By";
                        break;
                    case 5:
                        text = "Bz";
                        break;
                }
                resultLog.Add(text, distace.magnitude < (originBound.magnitude + targetBound.magnitude));
            }
            */

            {
                var originSize = Obb0.transform.right * Obb0.transform.localScale.x
                    + Obb0.transform.up * Obb0.transform.localScale.y
                    + Obb0.transform.forward * Obb0.transform.localScale.z;
                originSize *= 0.5f;
                var targetSize = Obb1.transform.right * Obb1.transform.localScale.x
                    + Obb1.transform.up * Obb1.transform.localScale.y
                    + Obb1.transform.forward * Obb1.transform.localScale.z;
                targetSize *= 0.5f;

            }

            {
                var AxisList = new List<Vector3>() { Ax, Ay, Az, Bx, By, Bz };//------ 외적축 을 포함해야한다곤 하는데
                for (int i = 0; i < AxisList.Count; i++)
                {
                    var origin = Vector3.Project(Obb0.transform.position, AxisList[i]);
                    var target = Vector3.Project(Obb1.transform.position, AxisList[i]);


                    var originBoundLocal = GetRotatedBound(Obb0.transform, AxisList[i]);//originBound - origin;
                    var targetBoundLocal = GetRotatedBound(Obb1.transform, AxisList[i]);//targetBound - target;

                    var originBound = originBoundLocal + origin;
                    var targetBound = targetBoundLocal + target;


                    var distace = target - origin;

                    result &= distace.magnitude < (originBoundLocal.magnitude + targetBoundLocal.magnitude);

                    var text = "";
                    switch (i)
                    {
                        case 0:
                            text = "Ax";
                            break;
                        case 1:
                            text = "Ay";
                            break;
                        case 2:
                            text = "Az";
                            break;
                        case 3:
                            text = "Bx";
                            break;
                        case 4:
                            text = "By";
                            break;
                        case 5:
                            text = "Bz";
                            break;
                    }
                    resultLog.Add(text, distace.magnitude < (originBoundLocal.magnitude + targetBoundLocal.magnitude));
                }
            }

            {
                /*
                var testAxis = Az;

                var origin = Vector3.Project(Obb0.transform.position, testAxis);
                var target = Vector3.Project(Obb1.transform.position, testAxis);


                var originBoundLocal = GetRotatedBound(Obb0.transform, testAxis);//originBound - origin;
                var targetBoundLocal = GetRotatedBound(Obb1.transform, testAxis);//targetBound - target;

                var originBound = originBoundLocal + origin;
                var targetBound = targetBoundLocal + target;

                var distace = target - origin;

                {
                    Debug.DrawLine(Obb1.transform.position, Obb1.transform.position + Obb1.transform.rotation * (Obb1.transform.localScale * 0.5f), Color.black, Time.deltaTime);

                    Debug.DrawLine(target - GetRotatedBound(Obb1.transform, testAxis) + Vector3.up * -0.1f, target + GetRotatedBound(Obb1.transform, testAxis) + Vector3.up * -0.1f, Color.cyan, Time.deltaTime);

                    Debug.Log($"{distace} < {originBoundLocal} + {targetBoundLocal}  / {distace.magnitude} < {originBoundLocal.magnitude} + {targetBoundLocal.magnitude}\n" +
                            $"Is Collision : {distace.magnitude < (originBoundLocal.magnitude + targetBoundLocal.magnitude)}");
                    Debug.DrawLine(origin, Obb0.transform.position, Color.green, Time.deltaTime);
                    Debug.DrawLine(origin, originBound, Color.green, Time.deltaTime);
                    Debug.DrawLine(originBound, Obb0.transform.position + originBoundLocal, Color.green, Time.deltaTime);

                    Debug.DrawLine(target , Obb1.transform.position , Color.red, Time.deltaTime);
                    Debug.DrawLine(target , target - targetBoundLocal, Color.red, Time.deltaTime);
                    Debug.DrawLine(target - targetBoundLocal, Obb1.transform.position - targetBoundLocal, Color.red, Time.deltaTime);
                }// - log
                */
            }//DebugLine - Disable

            
            {
                var sb_Not = new StringBuilder();
                var sb_Collision = new StringBuilder();

                foreach(var v in resultLog)
                {
                    if (v.Value)
                    {
                        sb_Collision.Append(v.ToString());
                        sb_Collision.Append(" /");
                    }
                    else
                    {
                        sb_Not.Append(v.ToString());
                        sb_Not.Append(" /");
                    }
                }
                //Debug.Log($"Not Collision : {sb_Not.ToString()} \n Collision : {sb_Collision}");
            }
        }

    }

    public Vector3 GetRotatedBound(Transform transform, Vector3 Axis)
    {
        float x = transform.localScale.x;
        float y = transform.localScale.y;
        float z = transform.localScale.z;

        Vector3[] pointList = new Vector3[8];

        pointList[0] = Vector3.Project(transform.rotation * transform.localScale * 0.5f, Axis);//+++
        pointList[1] = Vector3.Project(transform.rotation * new Vector3(x, y, -z) * 0.5f, Axis);//++-
        pointList[2] = Vector3.Project(transform.rotation * new Vector3(x, -y, z) * 0.5f, Axis);//+-+
        pointList[3] = Vector3.Project(transform.rotation * new Vector3(x, -y, -z) * 0.5f, Axis);//+--

        pointList[4] = Vector3.Project(transform.rotation * transform.localScale * -0.5f, Axis);//---
        pointList[5] = Vector3.Project(transform.rotation * new Vector3(x, y, -z) * -0.5f, Axis);//--+
        pointList[6] = Vector3.Project(transform.rotation * new Vector3(x, -y, z) * -0.5f, Axis);//--+-
        pointList[7] = Vector3.Project(transform.rotation * new Vector3(x, -y, -z) * -0.5f, Axis);//-++

        var size = pointList.Max(x => x.sqrMagnitude);

        return Axis * Mathf.Sqrt(size); ;
    }

}
