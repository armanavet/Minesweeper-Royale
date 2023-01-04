using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ability : MonoBehaviour
{

    Button AbilityButton;
    public Image cooldownFillImage;
    public float coolDown = 5;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        AbilityButton = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 0)
            timer = 0;
        if(timer>0)
        {
            timer -= Time.deltaTime;
            
        }
        cooldownFillImage.fillAmount = timer / coolDown;
        AbilityButton.interactable = (timer == 0);
    }

    public void StartCoolDown()
    {
        timer = coolDown;
    }


}
