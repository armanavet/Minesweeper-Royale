using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialLogic : MonoBehaviour
{
    public Sprite[] sprites;
    public Image tutorial;
    public GameObject tutwindow;
    int tutNum = 0;
    bool inTutorial = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tutwindow.SetActive(inTutorial);
        tutorial.sprite = sprites[tutNum];
        if(inTutorial&&Input.GetMouseButtonDown(0))
        {
            NextTut();
        }
    }

    public void OpenTut()
    {
        inTutorial = true;
    }

    public void CloseTut()
    {
        inTutorial = false;
    }

    public void NextTut()
    {
        if (tutNum < sprites.Length - 1)
            tutNum++;
        else
            tutNum = 0;
    }
}
