using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class NetworkManager_custom : NetworkManager {
    
    private Vector3 spawnPoint1 = new Vector3(-50, -25, 40);
    private Vector3 spawnPoint2 = new Vector3(50, -25, 40);
    private Vector3 spawnPoint3 = new Vector3(-50, -25, -40);
    private Vector3 spawnPoint4 = new Vector3(50, -25, -40);

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player;
        if (numPlayers == 0)
        { 
            player = Instantiate(Resources.Load("PlayerCarGreen"), spawnPoint1, Quaternion.identity) as GameObject;
        }
        else if (numPlayers == 1)
        { 
            player = Instantiate(Resources.Load("PlayerCarRed"), spawnPoint2, Quaternion.identity) as GameObject;
        }
        else if (numPlayers == 2)
        {
            player = Instantiate(Resources.Load("PlayerCarGreen"), spawnPoint3, Quaternion.identity) as GameObject;
        }
        else 
        {
            player = Instantiate(Resources.Load("PlayerCarRed"), spawnPoint4, Quaternion.identity) as GameObject;
        }

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
}
