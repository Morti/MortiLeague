using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BallScript : NetworkBehaviour
{
    public GameObject ballPrefab;

    public override void OnStartServer()
    {
        var spawnPosition = new Vector3(0.0f, 5.0f, 0.0f);
        var spawnRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        var ball = (GameObject)Instantiate(ballPrefab, spawnPosition, spawnRotation);
        NetworkServer.Spawn(ball);
    }
}
