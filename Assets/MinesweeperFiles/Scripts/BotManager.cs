using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    private const string PREFS_PLAYER_ELO = "PREFS_PLAYER_ELO";
    [SerializeField] private Bot[] allBots;

    private void Awake()
    {
        GameManager.onGameOver += GameManagerOnonGameOver;
    }
    private void OnDestroy()
    {
        GameManager.onGameOver -= GameManagerOnonGameOver;
    }

    public void GameManagerOnonGameOver(bool isWin)
    {
        int mult = isWin ? 1 : -1;
        float current = GetPlayerCurrentElo();
        float finalElo = current +( (50 - (Mathf.Abs(50 - current)) * 0.3f) * mult);
        finalElo = Mathf.Clamp(finalElo, 0, 100);
        PlayerPrefs.SetFloat(PREFS_PLAYER_ELO, finalElo);
    }

    float GetPlayerCurrentElo() => PlayerPrefs.GetFloat(PREFS_PLAYER_ELO, 20);
}
