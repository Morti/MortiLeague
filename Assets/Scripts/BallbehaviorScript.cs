using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class BallbehaviorScript : NetworkBehaviour {
    
    private Rigidbody rb;
    
    private GameObject gameManager;
    private GameManager gm;

    /*[SyncVar]
    private Vector3 realposition;

    [SyncVar]
    private Quaternion realrotation;

    private Vector3 velocity = Vector3.zero;

    //Test
    [SyncVar] private Vector3 syncPos;
    [SyncVar] private Quaternion syncRot;

    private Vector3 lastPos;
    private Quaternion lastRot;
    private Transform myTransform;
    private float lerpRate = 10;
    private float posThreshold = 0.5f;
    private float rotThreshold = 5;*/

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        
        gameManager = GameObject.Find("Game Manager");
        gm = gameManager.GetComponent<GameManager>();

        //myTransform = transform;
    }

    /*void Update()
    {
        TransmitMotion();
        LerpMotion();
    }*/

    /*void TransmitMotion()
    {
        if (!isServer)
            return;

        if(Vector3.Distance(myTransform.position, lastPos) > posThreshold || Quaternion.Angle(myTransform.rotation, lastRot) > rotThreshold)
        {
            lastPos = myTransform.position;
            lastRot = myTransform.rotation;

            syncPos = myTransform.position;
            syncRot = myTransform.rotation;
        }
    }*/

    /*void LerpMotion()
    {
        if (isServer)
            return;

        myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);
        //myTransform.position = Vector3.Lerp(myTransform.position, syncPos, 0.3f);
        myTransform.rotation = Quaternion.Lerp(myTransform.rotation, syncRot, Time.deltaTime * lerpRate);
        //myTransform.rotation = Quaternion.Lerp(myTransform.rotation, syncRot, 0.3f);
    }*/

    void FixedUpdate()
    {
        gm.updateBallTrans(rb.transform);
    }

    void OnCollisionEnter(Collision col)
    {
        if (!isServer)
            return;

        if (col.gameObject.tag == "Player")
        {
            rb.AddExplosionForce(40, col.rigidbody.position, 30, 1, ForceMode.VelocityChange);
        }
        
        if (col.gameObject.tag == "GreenGoal")
        {
            gm.ResetBall(gameObject);

            Score(2);
        }

        if (col.gameObject.tag == "RedGoal")
        {
            gm.ResetBall(gameObject);

            Score(1);
        }
    }

    public void Score(int team)
    {
        if(isServer)
        {
            gm.RpcScore(team);            
        }
    }

    /*[Command]
    private void CmdUpdateRealPosition()
    {
        realposition = transform.position;
        realrotation = transform.rotation;
    }*/
}
