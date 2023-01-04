using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action<bool> onGameOver;
    public static GameMode gameMode = GameMode.Offline;
    private static GameStatus currentGameStatus;
    [SerializeField] private GameObject botManager;
    [SerializeField] private GameObject offlinePlayer;
    public Material[] allMats;
    public GameObject[] allInitialPos;
    public GameObject throwButton;
    private void Awake()
    {
        currentGameStatus = GameStatus.homeScreen;
    }

    public static void ChangeGameStatus(GameStatus _status, bool _isWin = false)
    {
        currentGameStatus = _status;
        
        if(_status == GameStatus.gameOver)
        onGameOver?.Invoke(_isWin);
    }

    public static GameStatus GetGameStatus() => currentGameStatus;

    public void OnOfflineClicked()
    {
        botManager.SetActive(true);
        offlinePlayer.SetActive(true);
    }

}

public enum GameStatus
{
    homeScreen,
    running,
    gameOver
}

public enum GameMode
{
    Offline,
    Online
}
