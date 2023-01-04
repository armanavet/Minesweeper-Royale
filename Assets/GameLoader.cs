using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameLoader : MonoBehaviour
{
    //0-Nothing, 1=Offline, 2=Online
    public static int itemChosen = 0;
    public Animator MenuAnim;
    // Start is called before the first frame update
    void Start()
    {
        itemChosen = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGame()
    {
        itemChosen = 1;
        MenuAnim.SetTrigger("load");
        Invoke("LoadGameScene", 3.5f);
    }

    public void LoadGameSceneDirectly()
    {
        itemChosen = 2;
        SceneManager.LoadScene(1);
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene(1);
    }
}
