using System.Collections;
using System.Collections.Generic;
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
        
    }

    public void Lose()
    {
        LoseScreen.SetActive(true);
    }
    public void Win()
    {
        WinScreen.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
