using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
public class ExtractSwing_fast : ExtractSwing
{

    private Vector3 TargetPos;
    private Quaternion TargetRot;
    private Vector3 BallPos;
    private Quaternion BallRot;


    public Transform Dummy;

    private GameObject player_swing;

    private int index;
    private string[] file;
    private bool cont = true;
    private int show = -1;

    private TrajectryDrawer ModelDrawer;
    // Start is called before the first frame update
    void Start()
    {
        ModelDrawer = new TrajectryDrawer(lineLength, lineWidth, dt);
        StreamReader txtfile = new StreamReader(Application.dataPath + "/MotionLog913.txt", Encoding.UTF8);
        index = 0;
        string str = txtfile.ReadToEnd();
        file = str.Split('\n');
        while (cont)
        {
            GameObject traj_p = Instantiate(SwingHolderPrefab);
            GameObject former = new GameObject("former");
            GameObject latter = new GameObject("latter");

            former.transform.parent = traj_p.transform;
            latter.transform.parent = traj_p.transform;

            while (SetPosition()) if(UpdatePos()<0)break;
            while (SwingStart() < 0.15f) if (UpdatePos() < 0) break;
            //Unityは左手系→ボールのzを前にしたとき、ユーザにとって右側はｘ軸の正の方向
            if (SwingStart() <= 0f)
            {
                if (UpdatePos() < 0) break;
                Destroy(traj_p);
                continue;
            }
            foreach (Transform child in this.transform)
            {
                child.gameObject.SetActive(false);
            }
            Debug.Log("swing may start");
            GameObject last;
            ModelDrawer.SetColorAndTag(new Color(1.0f, 0f, 0f, 0.1f), "TakeBack");
            ModelDrawer.setLength(this.lineLength);

            Dummy.position = TargetPos;
            Dummy.rotation = TargetRot;
            ModelDrawer.init(Dummy, former);
            while (true)
            {
                Dummy.position = TargetPos;
                Dummy.rotation = TargetRot;
                last = ModelDrawer.drawLine(Dummy);
                if (last != null) break;
                if (UpdatePos() < 0) break; ;
            }
            if (int.Parse(last.name) <= 0)
            {
                Destroy(traj_p);
                continue;
            }
            ModelDrawer.Turn(lineLength * 2f, latter);
            Color c2 = new Color(0f, 1.0f, 0f, 0.1f);
            ModelDrawer.SetColorAndTag(c2, "DownSwing");

            while (true)
            {
                Dummy.position = TargetPos;
                Dummy.rotation = TargetRot;
                last = ModelDrawer.drawLine(Dummy);
                if (last != null) break;
                if (UpdatePos() < 0) break;
            }
            if (int.Parse(last.name) <= 0)
            {
                Destroy(traj_p);
                continue;

            }

            Destroy(last);
            Debug.Log("swing ends");
            traj_p.transform.parent = this.transform;
            if (!cont) Destroy(traj_p);
            
        }


    }
    private bool SetPosition()
    {
        //ボールから見たクラブヘッドの位置
        Quaternion conj = BallRot;
        conj.w = -conj.w;
        Vector3 ball2head = conj * ((TargetPos + TargetRot * (0.8f * Vector3.forward)) - BallPos);


        //クラブヘッドがボールに十分近づいた
        if (ball2head.magnitude < 0.15f) return false;
        else return true;

    }

    private float SwingStart()
    {
        //ボールから見たクラブヘッドの位置
        Quaternion conj = BallRot;
        conj.w = -conj.w;
        Vector3 ball2head = conj * ((TargetPos + TargetRot * (0.8f * Vector3.forward)) - BallPos);
        return ball2head.x;

    }

    Vector3 GetPosition(string[] datas)
    {
        string[] posdatas = datas[0].Trim('(', ')').Split(',');
        Vector3 pos = new Vector3(
            float.Parse(posdatas[0]),
            float.Parse(posdatas[1]),
            float.Parse(posdatas[2]));
        return pos;
    }

    Quaternion GetRotation(string[] datas)
    {
        string[] rotdatas = datas[1].Replace("\r", "").Trim('(', ')').Split(',');
        Quaternion rot = new Quaternion(
                        float.Parse(rotdatas[0]),
                        float.Parse(rotdatas[1]),
                        float.Parse(rotdatas[2]),
                        float.Parse(rotdatas[3]));
        return rot;
    }

    private int UpdatePos()
    {
        if (index >= file.Length)
        {
            cont = false;
            return -1;
        }
        string recdata = file[index];
        string[] datas = recdata.Split(':');
        if(datas.Length < 5)
        {
            Debug.Log(recdata);
            cont = false;
            return -1;
        }
        
        this.BallRot = GetRotation(datas[2].Split(' '));
        this.BallPos = GetPosition(datas[2].Split(' '));
        this.TargetRot = GetRotation(datas[1].Split(' '));
        this.TargetPos = GetPosition(datas[1].Split(' ')) +
            TargetRot * (-0.3f * Vector3.forward);
        index += 1;
        return 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            show += 1;
            if (show >= this.transform.childCount) show = 0;
            foreach(Transform child in this.transform)
            {
                child.gameObject.SetActive(false);
            }
            this.transform.GetChild(show).gameObject.SetActive(true);
            
        }
    }
}
