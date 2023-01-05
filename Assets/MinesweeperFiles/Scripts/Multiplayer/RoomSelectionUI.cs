using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class RoomSelectionUI : MonoBehaviourPunCallbacks
{
    public enum Status {Offline, Connecting, Joining, Creating, Rooming};

    public string room    = "The Room";
    public string avatar  = "Avatar";
    public bool   verbose = true;

    [Header("Informative")]
    public Status status = Status.Offline;

    [SerializeField] private TextMeshProUGUI statusText, startsInText;

    [SerializeField] internal GameObject restUI, playBlocker;
    [SerializeField] private GameObject RoomJoinUI;
    [SerializeField] private TMP_InputField roomNameInputField;
    public PhotonView photonView;
    // --------------------------------------------------------------------------

    private static int times=1;
    private void Start()
    {
            photonView = GetComponent<PhotonView>();

            if (GameLoader.itemChosen == 2)
            {
                RoomJoinUI.SetActive(true);
            }
            else
            {
                OnOfflineClicked();
            }
    }

    public void OnCreateRoomClicked()
    {
        if (!string.IsNullOrEmpty(roomNameInputField.text))
            room = roomNameInputField.text;
        
        RoomJoinUI.SetActive(false);
        OnOnlineClicked();
    }

    public void OnJoinRoomClicked()
    {
        if (!string.IsNullOrEmpty(roomNameInputField.text))
            room = roomNameInputField.text;
        
        RoomJoinUI.SetActive(false);
        OnOnlineClicked();
    }

    private void OnDestroy()
    {
        PhotonNetwork.Disconnect();
    }

    public void OnOfflineClicked()
    {
        GameManager.gameMode = GameMode.Offline;
        GameObject.FindObjectOfType<GameManager>().OnOfflineClicked();
        this.gameObject.SetActive(false);
    }

    public void OnOnlineClicked () {
        GameManager.gameMode = GameMode.Online;
        status = Status.Connecting;
        PhotonNetwork.ConnectUsingSettings();
        restUI.SetActive(false);
        Debug.Log("We sent COnnectUsing Settings");
        statusText.text = "Connecting to server...";
        
    }

    public override void OnConnectedToMaster()
    {
        if (GameManager.GetGameStatus() == GameStatus.gameOver)
            return;
        
        status = Status.Joining;
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("On Connected to master");
        Log("On Connected to master");
    }

    public override void OnJoinRandomFailed(short returnCode, string message){
        status = Status.Creating;
        PhotonNetwork.CreateRoom(room);
        Debug.LogError("^^^^^^^^^^^^^ On Join Random Failed");
        Log("Creating New Room: "+ room);
    }

    public override void OnCreatedRoom(){
        // Don't care because we still receive OnJoinedRoom
        Debug.Log("On Created room");
        Log("On Created room");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        status = Status.Creating;
        room += "_"+Random.Range(0, 100);
        PhotonNetwork.CreateRoom(room);
        Debug.LogError("^^^^^^^^^^^^^ On Join Random Failed");
        Log("Creating New Room: "+ room);
    }

    public override void OnJoinedRoom(){
       
        restUI.SetActive(false);
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Hey I am the master");
            PhotonNetwork.CurrentRoom.MaxPlayers = 4;
            
        }

        status = Status.Rooming;// Log("Joined room");
        if(avatar.Length>0){
            PhotonNetwork.Instantiate(avatar, Vector3.zero, Quaternion.identity, 0);
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            statusText.text = "Waiting for others to Join...";
        else
        {
            statusText.text = "Total Players: " + PhotonNetwork.CurrentRoom.PlayerCount;
        }
        Debug.Log("On Joined room: "+ PhotonNetwork.CurrentRoom.Name + " : "+ PhotonNetwork.CurrentRoom.PlayerCount);
      //  statusText.text = "Total Players: " + PhotonNetwork.CurrentRoom.PlayerCount;
        
    }

    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
       // statusText.text = "Total Players: " + PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            statusText.text = "Waiting for others to Join...";
        else
        {
            statusText.text = "Total Players: " + PhotonNetwork.CurrentRoom.PlayerCount;
        }

        if (RoomCoroutine == null)
        {
            RoomCoroutine = RoomWaiting();
            StartCoroutine(RoomCoroutine);
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            StopCoroutine(RoomCoroutine);
            StartGame();
        }

    }
    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        Debug.LogError("Player Left Room: ^^^^^^^: "+ PhotonNetwork.CurrentRoom.PlayerCount + " : "+ GameManager.GetGameStatus() );
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
           // if (photonView.IsMine)
            {
                if (GameManager.GetGameStatus() == GameStatus.running)
                {
                    FindObjectOfType<UiManager>().Win();
                    statusText.gameObject.SetActive(false);
                   // statusText.text = "You are the only left, You Should Win here once we implement Game Win screen...";
                }
                else if(GameManager.GetGameStatus() == GameStatus.homeScreen)
                    statusText.text = "Waiting for others to Join...";
            }

            startsInText.text = "";
            StopCoroutine(RoomCoroutine);
            RoomCoroutine = null;
        }
        else
        {
            statusText.text = "Total Players: " + PhotonNetwork.CurrentRoom.PlayerCount;
        }

       // statusText.text = newPlayer.NickName + " Left room: " + PhotonNetwork.CurrentRoom.Name;
    }

    public void DisableTexts()
    {
        startsInText.gameObject.SetActive(false);
        statusText.gameObject.SetActive(false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {

        if (GameManager.GetGameStatus() == GameStatus.gameOver)
            return;
        if(cause!=DisconnectCause.DisconnectByClientLogic)
            Log("Photon error: " + cause);
        Debug.Log("On Disconnected");
        
       statusText.gameObject.SetActive(false);
       startsInText.gameObject.SetActive(false);
       
       FindObjectOfType<UiManager>().Lose();
    }

    void Log(string x)
    {
        statusText.text = x;
        if(verbose) Debug.Log(x);
    }

    [PunRPC]
    public void GetTimeTillRoomOpen(int  _time)
    {
        startsInText.text = "Game Starts in "+_time;
        
        if(_time == 0)
            StartGame();
    }

    [PunRPC]
    public void OnSomeoneElseWon()
    {
        if (GameManager.GetGameStatus() == GameStatus.gameOver)
            return;
        GameManager.ChangeGameStatus(GameStatus.gameOver);
       // playBlocker.SetActive(true);
        statusText.text = "You LOSTTTTT...";
        statusText.gameObject.SetActive(false);
        FindObjectOfType<UiManager>().Lose();
      //  Invoke("Restart", 4);
    }

    private IEnumerator RoomCoroutine;
    IEnumerator RoomWaiting()
    {
        Debug.LogError("Room Waiting started: "+ Time.realtimeSinceStartup);
        int time = 10;

        while (time >= 0)
        {
            photonView.RPC("GetTimeTillRoomOpen", RpcTarget.All, time);
            yield return new WaitForSeconds(1);
            time -= 1;
        }
    }

    void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        restUI.SetActive(false);
        playBlocker.SetActive(false);
        statusText.text = "Game Running: "+ PhotonNetwork.CurrentRoom.Name;
       // statusText.gameObject.SetActive(false);
        startsInText.gameObject.SetActive(false);
        GameManager.ChangeGameStatus(GameStatus.running);
    }
    void Restart()
    {
        if(photonView.IsMine)
            SceneManager.LoadScene(0);
    }

    public void OnCloseButtonClicked()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }
}
