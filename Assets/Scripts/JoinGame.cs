using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoinGame : MonoBehaviour {

    List<GameObject> roomList = new List<GameObject>();

    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private Transform roomListParent;

    private NetworkManager networkManager;

    void Start()
    {
        networkManager = NetworkManager.singleton;
        if(networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        RefreshRoomList();
    }

    public void RefreshRoomList()
    {
        ClearRoomList();

        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
        status.text = "Loading...";
    }

    public void OnMatchList (bool success, string extentedInfo, List<MatchInfoSnapshot> matchList)
    {
        status.text = "";
        if(!success || matchList == null)
        {
            status.text = "Can't find matchList, check your connection";
            return;
        }

        foreach(MatchInfoSnapshot match in matchList)
        {
            GameObject roomListItemGameObject = Instantiate(roomListItemPrefab);
            roomListItemGameObject.transform.SetParent(roomListParent);

            RoomListItem roomListItem = roomListItemGameObject.GetComponent<RoomListItem>();
            if(roomListItem != null)
            {
                roomListItem.Setup(match, JoinRoom);
            }

            
            //as well as setting up a callback function that will join the game

            roomList.Add(roomListItemGameObject);
        }

        if (roomList.Count == 0)
        {
            status.text = "No rooms open at the moment";
        }
    }

    void ClearRoomList()
    {
        for(int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        roomList.Clear();
    }

    public void JoinRoom(MatchInfoSnapshot match)
    {
        networkManager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        ClearRoomList();

        status.text = "Joining Game...";
    }
}
