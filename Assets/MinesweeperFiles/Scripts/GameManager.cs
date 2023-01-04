using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action<bool> onGameOver;

    private static GameStatus currentGameStatus;
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

}

public enum GameStatus
{
    homeScreen,
    running,
    gameOver
}
