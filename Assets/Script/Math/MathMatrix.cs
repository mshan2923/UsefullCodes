using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MathMatrix : MonoBehaviour
{
    public Transform LastObject;
    public Transform LastWorldObject;


    Stack<Transform> parents = new();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTest()
    {
        Transform Ltf = LastObject;

        Matrix4x4 calcu_M = new Matrix4x4();
        calcu_M.SetTRS(Ltf.localPosition, Ltf.localRotation, Ltf.localScale);

        while (Ltf != LastObject.root)
        {
            parents.Push(Ltf);

            Ltf = Ltf.parent;
        }

        int count = 0;
        while (parents.Count > 0)
        {
            var p = parents.Pop();
            Matrix4x4 Lm = new Matrix4x4();
            Lm.SetTRS(p.localPosition, p.localRotation, p.localScale);

            if (count == 0)
                calcu_M = Lm;
            else
                calcu_M *= Lm;

            count++;
        }

        Debug.Log($"Root : {LastObject.root} , {parents.Count}");
        Debug.Log($"Matrix : {calcu_M.GetPosition()} , {calcu_M.rotation.eulerAngles}, {calcu_M.lossyScale}" +
            $"\n Dot : {Quaternion.Angle(calcu_M.rotation, LastWorldObject.rotation)}");

    }
}

[CustomEditor(typeof(MathMatrix))]
public class MathMatrixEditor : Editor
{
    MathMatrix target;
    protected override void OnHeaderGUI()
    {
        base.OnHeaderGUI();

    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        target = serializedObject.targetObject as MathMatrix;

       if (GUILayout.Button("Å×½ºÆ®"))
        {
            target.OnTest();
        }
    }
}