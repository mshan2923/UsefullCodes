using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class DebugContectTwocircle : MonoBehaviour
{
    public GameObject Origin;
    public GameObject Target;

    public GameObject Contect_A;
    public GameObject Contect_B;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Calculate();
    }
    public void Calculate()
    {
        var data = Math.ContactTwoCircle(RemoveY(Origin), Origin.transform.localScale.x * 0.5f, RemoveY(Target), Target.transform.localScale.x * 0.5f);

        if (data == null)
        {
            Contect_A.transform.position = gameObject.transform.position;
            Contect_B.transform.position = gameObject.transform.position;
        }
        else
        {
            //¿ùµå ÁÂÇ¥·Î ÇÏ´Ï±ñ Á¶±Ý ¿ÀÂ÷°¡ »ý±è
            Contect_A.transform.position = new Vector3(data[0].x, 0, data[0].y) + gameObject.transform.position;
            Contect_B.transform.position = new Vector3(data[1].x, 0, data[1].y) + gameObject.transform.position;
        }
    }
    public Vector2 RemoveY (GameObject pos)
    {
        return new Vector2(pos.transform.localPosition.x, pos.transform.localPosition.z);
    }
}

[CustomEditor(typeof(DebugContectTwocircle))]
public class DebugContectTwocircleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Test"))
        {
            (target as DebugContectTwocircle).Calculate();
        }
    }
}
