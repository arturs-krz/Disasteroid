﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PhotonGameLobby : MonoBehaviourPunCallbacks
{
    private static PhotonGameLobby _instance;
    public static PhotonGameLobby Instance { get { return _instance; } }

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
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = "0.1";
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
        Debug.Log("Connected to the master server!");
        //PhotonNetwork.JoinLobby(TypedLobby.Default);

        // Join the room right away only if not on mobile
        if (Application.platform != RuntimePlatform.Android)
        {
            JoinGame();
        }
    }

    public void JoinGame()
    {
        Debug.Log("Trying to join existing room or creating...");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 20;
        PhotonNetwork.JoinOrCreateRoom("DisasteroidMain", roomOptions, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created new room!");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room!");
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
