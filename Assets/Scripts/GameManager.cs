using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    public GameObject ballPrefab;
    
    [SyncVar]
    private int greenScore;

    [SyncVar]
    private int redScore;

    [SyncVar]
    private Transform balltrans;

    public Text scoreText;

    List<NetworkInstanceId> teamGreen = new List<NetworkInstanceId>();
    List<NetworkInstanceId> teamRed = new List<NetworkInstanceId>();
    
    public override void OnStartServer()
    {
        greenScore = 0;
        redScore = 0;
    }

    public void FixedUpdate()
    {
        if(scoreText != null)
            scoreText.text = redScore + " - Red Team | Green Team - " + greenScore;
    }

    [ClientRpc]
    public void RpcScore(int team)
    {
        if (team == 1)
            greenScore++;
        else
            redScore++;
    }
    
    public int assignTeam(NetworkInstanceId playerid)
    {
        if (teamGreen.Count <= teamRed.Count)
        {
            teamGreen.Add(playerid);
            return 1;
        }
        else
        {
            teamRed.Add(playerid);
            return 0;
        }
    }

    public void unAssignTeam(NetworkInstanceId playerid)
    {
        foreach (NetworkInstanceId ID in teamGreen)
        {
            if(ID == playerid)
            {
                teamGreen.Remove(playerid);
            }
        }

        foreach (NetworkInstanceId ID in teamRed)
        {
            if (ID == playerid)
            {
                teamRed.Remove(playerid);
            }
        }

    }

    public int getRedScore()
    { 
        return redScore;
    }

    public int getGreenScore()
    {
        return greenScore;
    }

    public void updateBallTrans(Transform trans)
    {
        balltrans = trans;
    }

    public Transform getBallTrans()
    {
        return balltrans;
    }

    public void ResetBall(GameObject ballObject)
    {
        if (!isServer)
            return;

        DestroyObject(ballObject);

        var spawnPosition = new Vector3(0.0f, 5.0f, 0.0f);
        var spawnRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        var ball = (GameObject)Instantiate(ballPrefab, spawnPosition, spawnRotation);
        NetworkServer.Spawn(ball);
    }
}
