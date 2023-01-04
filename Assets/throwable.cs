using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class throwable : MonoBehaviour
{
    public Vector3 Target;
    bool hurl = false;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if(hurl)
        {
            transform.position = Vector3.Lerp(transform.position, Target, Time.deltaTime * 5);
        }
    }

    public void Hurl()
    {
        hurl = true;
    }
}
