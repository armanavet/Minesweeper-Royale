using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{

    public GameObject LoseScreen, WinScreen;
    // Start is called before the first frame update
    void Start()
    {
        LoseScreen.SetActive(false);
        WinScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            RevealBoard();
        }
    }

    public void Lose()
    {
        GameManager.ChangeGameStatus(GameStatus.gameOver);
        PhotonNetwork.Disconnect();
        Debug.Log("Lose call came");
        LoseScreen.SetActive(true);
        
        RoomSelectionUI roomUI = GameObject.FindObjectOfType<RoomSelectionUI>();
        if(roomUI != null)
            roomUI.DisableTexts();
        
    }
    public void Win()
    {
        GameManager.ChangeGameStatus(GameStatus.gameOver);
        PhotonNetwork.Disconnect();
        Debug.Log("Win call came");
        WinScreen.SetActive(true);
        
        RoomSelectionUI roomUI = GameObject.FindObjectOfType<RoomSelectionUI>();
        if(roomUI != null)
            roomUI.DisableTexts();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void RevealBoard()
    {
        var bricks = FindObjectsOfType<Brick>();
        foreach (var brick in bricks)
        {
            if (!brick.mShowed&&!brick.mine)
                brick.ShowSecret(false);
        }
    }
}
