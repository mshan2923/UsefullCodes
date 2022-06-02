using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IK
{
    /// <summary>
    /// ElbowOffset - ��� �������� �󸶳� ��������
    /// </summary>
    /// <param name="Shoulder"></param>
    /// <param name="Elbow"></param>
    /// <param name="Hand"></param>
    /// <param name="ElbowOffset"></param>
    /// <param name="UpperArm"></param>
    /// <param name="LowerArm"></param>
    /// <param name="IkedHand"></param>
    /// <returns></returns>
    public static Vector3 Arm_IK(Vector3 Shoulder, Vector3 Elbow, Vector3 Hand , Vector3 ElbowOffset, float UpperArm, float LowerArm, out Vector3 IkedHand)
    {
        //line_start + Vector3.Project(point - line_start, line_end - line_start); / ClosePointOnDirection


        Vector3 ClosePoint = Shoulder + Vector3.Project(Elbow + ElbowOffset - Shoulder, Hand - Shoulder);
        //Vector3 Center = Hand - (Hand - Shoulder).normalized * (LowerArm / (LowerArm + UpperArm)) * (Hand - Shoulder).magnitude;

        //UpperArm , LowerArm �� ���̰� �´� ClosePoint ~ Elbow ������ �Ÿ�
        // Acos (HC / LowerArm) = Angle  ==>  LowerArm * Sin (Angle) = EC
        // HC = Hand ~ ClosePoint / EC = Elbow ~ ClosePoint

        float Height = LowerArm * Mathf.Sin(Mathf.Acos((Hand - ClosePoint).magnitude / LowerArm));//Elbow�� ������ UpperArm + �ణ�� Z�࿡ ����
        //float Height = UpperArm * Mathf.Sin(Mathf.Acos((Shoulder - ClosePoint).magnitude / UpperArm));//IkedElbow�� �չ��������� �Ÿ��� �ȱ�������

        //UpperArm �� LowerArm ������ �´ºκ� ���� Heghit ��ŭ

        Vector3 CorrectionElbow = Height > 0 ? (ClosePoint + (Elbow + ElbowOffset - ClosePoint).normalized * Height) : ClosePoint;

        Vector3 IKedElbow = Shoulder + (CorrectionElbow - Shoulder).normalized * UpperArm;
        IkedHand = IKedElbow + (Hand - IKedElbow).normalized * LowerArm;

        return IKedElbow;

        //Out ���� ����ġ �������� , �׻� UpperArm �� LowerArm ���� ����
        

        //�������� ������ ������ ����?
    }
}

[ExecuteInEditMode]
public class Debug_IK : MonoBehaviour
{
    [Header("Not Correct Elbow Position")]
    public LineRenderer line;

    public GameObject Shoulder;
    public GameObject Elbow;
    public GameObject Hand;
    public GameObject DebugPoint;

    public float UpperArmLength = 1.2f;
    public float LowerArmLength = 1.5f;

    public Vector3 IKedElbow = Vector3.zero;
    public Vector3 IKedHand = Vector3.zero;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        IKedElbow = IK.Arm_IK(GetPos(Shoulder), GetPos(Elbow), GetPos(Hand), new Vector3(0,0,0.2f), UpperArmLength, LowerArmLength, out IKedHand);

        line.SetPosition(0, GetPos(Shoulder));
        line.SetPosition(1, IKedElbow);
        line.SetPosition(2, IKedHand);

        DebugPoint.transform.position = line.GetPosition(1);
    }

    public Vector3 GetPos(GameObject obj)
    {
        return obj.transform.position;
    }
}
