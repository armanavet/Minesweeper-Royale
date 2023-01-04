using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameLoader : MonoBehaviour
{
    public Animator MenuAnim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGame()
    {
        MenuAnim.SetTrigger("load");
        Invoke("LoadGameScene", 3.5f);
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene(1);
    }
}
