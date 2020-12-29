using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSaveLoad : MonoBehaviour
{
    public struct data
    {
        public List<string> text;
    }

    SaveLoad<data> saveLoad = new SaveLoad<data>();
    public List<string> Texts = new List<string>();

    string path = "";
    string FileName = "SaveFile";

    void Start()
    {
        path = Application.dataPath + "/Script/SaveLoad";
        //Application.persistentDataPath 일경우 로컬 저장소 , dataPath 는 실행파일 위치
        saveLoad.Save(new data { text = Texts}, FileName, path);

        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1);

        data Ldata = new data();
        saveLoad.Load(FileName, path, out Ldata);

        for(int i = 0; i < Ldata.text.Count; i++)
        {
            Debug.Log(Ldata.text[i]);
        }
    }
}
