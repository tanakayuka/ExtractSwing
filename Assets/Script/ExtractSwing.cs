using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
public class ExtractSwing : MonoBehaviour
{
    public bool fast = false;

    public float lineLength = 0.2f;
    public float lineWidth = 0.1f;
    public GameObject linePrefab;


    public GameObject TargetObject;
    public GameObject Clubhead;

    public ExtractSwing dt;
    public GameObject SwingHolderPrefab;


    private Transform TargetPos;
    private Transform ballpos;

    private GameObject player_swing;

    private TrajectryDrawer ModelDrawer;


    private int active_sound = 0;
    //スイングの記録
    private List<GameObject> swing;
    private Vector3 predir;
    protected void Start()
    {

        TargetPos = TargetObject.transform;
        ballpos = GameObject.Find("ball").transform;

        predir = new Vector3(0f, 0f, 0f);
        swing = new List<GameObject>();

        player_swing = new GameObject("player_swing");
        player_swing.transform.parent = this.transform;

        ModelDrawer = new TrajectryDrawer(lineLength, lineWidth, dt);

        if(!fast)
        StartCoroutine(SwingRecord(false));
        //sound_controller = controller_list[0];

    }

    private bool SetPosition()
    {
        //ボールから見たクラブヘッドの位置
        Vector3 ball2head = ballpos.rotation * (Clubhead.transform.position - ballpos.position);


        //クラブヘッドがボールに十分近づいた
        if (Mathf.Abs(ball2head.x) < 0.15f) return false;
        else return true;

    }

    private float SwingStart()
    {
        //ボールから見たクラブヘッドの位置
        Vector3 ball2head = ballpos.rotation * (Clubhead.transform.position - ballpos.position);
        return ball2head.x;

    }

    private IEnumerator SwingRecord(bool active)
    {
        //スイングホルダー　親オブジェクトの作成



        int num_of_swing = 0;
        while (true)
        {



            GameObject traj_p = Instantiate(SwingHolderPrefab);
            GameObject former = new GameObject("former");
            GameObject latter = new GameObject("latter");

            former.transform.parent = traj_p.transform;
            latter.transform.parent = traj_p.transform;
            
            Debug.Log(num_of_swing++);
            while (SetPosition()) yield return null;
            while (SwingStart() < 0.15f) yield return null;
            //Unityは左手系→ボールのzを前にしたとき、ユーザにとって右側はｘ軸の正の方向
            if (SwingStart() <= 0f)
            {
                yield return null;
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
            ModelDrawer.init(TargetPos, former);
            while (true)
            {
                last = ModelDrawer.drawLine(TargetPos);
                if (last != null) break;
                yield return null;
            }
            if (int.Parse(last.name) <= 10) continue;

            ModelDrawer.Turn(lineLength * 2f, latter);
            Color c2 = new Color(0f, 1.0f, 0f, 0.1f);
            ModelDrawer.SetColorAndTag(c2, "DownSwing");
            
            while (true)
            {
                last = ModelDrawer.drawLine(TargetPos);
                if (last != null) break;
                yield return null;
            }
            if (int.Parse(last.name) <= 10) continue;
            Destroy(last);
            Debug.Log("swing ends");
            traj_p.transform.parent = this.transform;
        }
        /*
        while (true)
        {
            Vector3 ball2head = ballpos.rotation * (Clubhead.transform.position - ballpos.position);
            yield return null;
            //if (ball2head.magnitude < 0.2f) break;
            if (Mathf.Abs(ball2head.x) < 0.15f) break;
        }
        Debug.Log("position has been set");
        ModelDrawer.SetColorAndTag(new Color(1.0f, 0f, 0f, 0.1f), "TakeBack");
        ModelDrawer.setLength(this.lineLength);
        ModelDrawer.init(TargetPos, former);


        
        while (true)
        {
            
            Vector3 ball2head = ballpos.rotation * (Clubhead.transform.position - ballpos.position);
            Debug.Log(ball2head);
            if (ball2head.x <= 0.15f)
            {
                //touchPos = TargetPos.position;
                ModelDrawer.init(TargetPos.position, former);
                ModelDrawer.SetPrefab(linePrefab);
                PlayerDrawer.init(TargetPos.position, player_swing);
                PlayerDrawer.SetPrefab(PlayerlinePrefab);
                yield return null;
                break;
            }
            yield return null;
        }
        
        
        //predir = new Vector3(0f, 0f, 0f);
        Debug.Log("Swing start");

        //GameObject last;
        while (true)
        {

            last = ModelDrawer.drawLine(TargetPos);
            if (last != null) break;
            yield return null;
        }

        //Color c2 = new Color(0f, 1.0f, 0f, 0.1f);
        //last.GetComponent<MeshRenderer>().material.color = c2;
        //last.tag = "TakeBack";
        //last.name = (ModelDrawer.Turn()).ToString();
        Destroy(last);
        ModelDrawer.Turn(lineLength * 2f, latter);

        yield return null;
        Debug.Log("down swing starts");
        ModelDrawer.SetColorAndTag(c2, "DownSwing");

        while (true)
        {

            last = ModelDrawer.drawLine(TargetPos);
 
            if (last != null) break;
            yield return null;
        }
        Destroy(last);
        Debug.Log("swing ends");
        //yield return null;
        swing.Add(traj_p);
        */


    }


    public GameObject MakeLine()
    {
        GameObject obj = Instantiate(linePrefab, this.transform.position, this.transform.rotation);
        return obj;
    }


    protected void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Update ballpos");
            ballpos = GameObject.Find("ball").transform;
        }



        //drawLine();
    }


}


public class TrajectryDrawer
{
    private float lineLength;
    private float lineWidth;

    private Vector3 predir;
    private Vector3 touchPos;
    private Quaternion touchRot;


    private Color c;
    private string tag_name;
    private GameObject p;

    private GameObject prefab;
    private int id;

    private ExtractSwing drawtrajectory;
    //private GameObject linePrefab;

    public TrajectryDrawer(float length, float width, ExtractSwing d)
    {
        this.lineLength = length;
        this.lineWidth = width;
        this.drawtrajectory = d;


    }


    public int Turn(float length, GameObject later_p)
    {
        this.lineLength = length;
        id -= 1;
        this.p = later_p;
        return id;
    }

    public void setLength(float length)
    {
        this.lineLength = length;
    }

    public void init(Transform tpos, GameObject parent)
    {
        this.touchPos = tpos.position;
        this.touchRot = tpos.rotation;
        predir = new Vector3(0f, 0f, 0f);
        this.p = parent;
        //this.linePrefab = prefab;
        this.id = 0;

    }
    public void SetColorAndTag(Color c, string tag)
    {

        this.c = c;
        tag_name = tag;

    }

    public GameObject drawLine(Transform TargetPos)
    {

        Vector3 startPos = touchPos;
        Vector3 endPos = TargetPos.position;
        //mPos.z = 10.0f;
        //Vector3 endPos = Camera.main.ScreenToWorldPoint(mPos);
        //endPos.z = 0.0f;

        //Debug.Log("endpos:" + endPos);
        if ((endPos - startPos).magnitude > lineLength)
        {
            GameObject obj;
            obj = this.drawtrajectory.MakeLine();
            //GameObject obj = Instantiate(linePrefab, transform.position, transform.rotation) as GameObject;
            obj.transform.position = (startPos + endPos) / 2f;

            obj.transform.up = (endPos - startPos).normalized;
            obj.transform.localScale = new Vector3(lineWidth, (endPos - startPos).magnitude / 2f, lineWidth);

            obj.transform.parent = p.transform;
            //obj.tag = tag_name;
            obj.name = (id++).ToString();
            obj.SetActive(true);
            obj.GetComponent<MeshRenderer>().material.color = c;
            float ret = Vector3.Dot(predir, (endPos - startPos).normalized);


            predir = (endPos - startPos).normalized;


            if (ret < 0f) return obj;
            touchPos = endPos;
            touchRot = TargetPos.rotation;

        }
        return null;
    }

}