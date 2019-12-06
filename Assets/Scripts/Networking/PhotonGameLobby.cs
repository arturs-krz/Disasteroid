using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PhotonGameLobby : MonoBehaviourPunCallbacks
{
    private static PhotonGameLobby _instance;
    public static PhotonGameLobby Instance { get { return _instance; } }

    private RoomInfo gameRoom = null;

    public bool connected { get; private set; } = false;

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            //PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = "0.2";
            // Set in the settings panel (Window -> Photon Unity Networking -> Highlight Server Settings)
            PhotonNetwork.ConnectUsingSettings();
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Failed to connect. StatusCode: " + cause.ToString() + " ServerAddress: " + PhotonNetwork.ServerAddress);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to the master server! Version: " + PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion);

        // Join the room right away only if not on mobile
        if (Application.platform != RuntimePlatform.Android)
        {
            CreateGame();
        }
        else 
        {
            PhotonNetwork.JoinLobby(TypedLobby.Default);
        }
    }

    private void CreateGame()
    {
        Debug.Log("Creating room...");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 20;
        PhotonNetwork.CreateRoom("DisasteroidMain", roomOptions, TypedLobby.Default);
    }
    public void JoinGame()
    {
        if (!connected && gameRoom != null)
        {
            PhotonNetwork.JoinRoom(gameRoom.Name);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (roomList.Count > 0)
        {
            gameRoom = roomList[0];
        }
        else {
            gameRoom = null;
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created new room!");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room!");
        connected = true;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("NetworkDebugger", new Vector3(0,0,0), Quaternion.identity);
        }
    }

    public override void OnLeftRoom()
    {
        connected = false;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join room -- " + returnCode + ": " + message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player " + newPlayer.ActorNumber + " joined the game!");
    }
}
