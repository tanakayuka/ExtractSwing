using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class Play_log : MonoBehaviour
{

    public GameObject[] Sensors;

    private StreamReader txtfile;
    private string[] file;
    private float i;
    private int CounterSlow;
    private bool stop = false;
    // Start is called before the first frame update
    void Start()
    {
        txtfile = new StreamReader(Application.dataPath +"/MotionLog913.txt", Encoding.UTF8);
        string str = txtfile.ReadToEnd();
        file = str.Split('\n');
        i = 0;
        Debug.Log("file length:" + file.Length);

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space)) stop = !stop;
        //if (stop) return;
        //string recdata = txtfile.ReadLine();
        //Debug.Log(recdata);
        //if(recdata == null)
        //{
        //　終了ないしループをさせたいよね！！　これは終了のパターン
        //Quit();


        //やっぱりループしたい
        //    return;
        //}
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            i = (i - 4 + file.Length) % file.Length;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            i = (i + 4) % file.Length;
        }
        else if(!stop)
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                i = i + 0.25f;
                if (i >= file.Length) i = i - (float)file.Length;
            }
            else
            {
                i = (i + 1) % file.Length;
            }
        }

        string recdata = file[(int)i];
        string[] datas = recdata.Split(':');
        if(datas.Length != Sensors.Length)
        {
            Debug.Log("txtファイルをちゃんと読めてない");
            i = (i + 1) % file.Length;
            Debug.Log(recdata);
            return;
        }


        for (int j = 0; j < Sensors.Length; j++)
        {
            if (SetPosRot(Sensors[j], datas[j].Split(' ')) < 0)
            {
                Debug.Log("読み込みえらーだよ");
                break;
            }
        }
        //if(!stop)i = (i + 1) % file.Length;
        //Debug.Log(i);

    }

    int SetPosRot(GameObject obj, string[] datas)
    {
        if(datas.Length != 2)
        {
            Debug.Log("error (読めてないか読み込みデータがおかしい)");
            return -1;
        }

        string[] posdatas = datas[0].Trim('(', ')').Split(',');
        string[] rotdatas = datas[1].Replace("\r","").Trim('(', ')').Split(',');

        if(posdatas.Length != 3 || rotdatas.Length != 4)
        {
            Debug.Log("error (読めてないか読み込みデータがおかしい)");
            return -1;
        }

        Vector3 pos = new Vector3(
                                float.Parse(posdatas[0]),
                                float.Parse(posdatas[1]),
                                float.Parse(posdatas[2]));
        Quaternion rot = new Quaternion(
                                float.Parse(rotdatas[0]),
                                float.Parse(rotdatas[1]),
                                float.Parse(rotdatas[2]),
                                float.Parse(rotdatas[3]));
        obj.transform.position = pos;
        obj.transform.rotation = rot;
        return 1;                

    }

    void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
            UnityEngine.Application.Quit();
        #endif
    }
}
