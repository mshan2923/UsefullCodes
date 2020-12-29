using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRactCal : MonoBehaviour
{
    public GameObject A;
    public GameObject B;
    public Vector3 AOffset;
    public Vector3 BOffset;

    public GameObject Select;
    public GameObject Border;
    public GameObject BorderPerant;

    LineRenderer LR;
    Vector2[] LinePos = new Vector2[2];
    GameObject[] obs = new GameObject[4];
    

    void Start()
    {
        LR = GetComponent<LineRenderer>();

        Quaternion temp = Quaternion.Euler(90, 0, 0);
        for(int i = 0; i < obs.Length; i++)
        {
            obs[i] = Instantiate(Border, Vector3.zero, Quaternion.identity);
            obs[i].transform.rotation = temp;
            obs[i].SetActive(true);
            obs[i].transform.SetParent(BorderPerant.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        LinePos[0] = B.transform.position + BOffset;
        LinePos[1] = A.transform.position + AOffset;

        LR.SetPosition(0, LinePos[1]);
        LR.SetPosition(1, LinePos[0]);


        Select.transform.position = SquareCalculate.SquareBorder(LinePos, A.transform.position, new Vector2(0.5f, 0.5f), A.transform.localScale);

        var borders = SquareCalculate.SquareBorders(LinePos, A.transform.position, new Vector2(0.5f, 0.5f), A.transform.localScale, false);
        for (int i = 0; i < borders.Count; i++)
        {
            obs[i].transform.position = borders[i];
        }
    }
}
