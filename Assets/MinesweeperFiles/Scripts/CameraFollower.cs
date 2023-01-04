using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public GameObject character;

    public float smoothSpeed = 5f;

    private Vector3 mDistance;

    // Start is called before the first frame update
    void Start()
    {
        mDistance = character.transform.position - transform.position;
        Debug.LogError("mDistance called: "+ mDistance);
    }

    void LateUpdate()
    {
        if (character != null)
        {
            Vector3 nextPos = character.transform.position - mDistance;
            transform.position = Vector3.Lerp(transform.position, nextPos, smoothSpeed * Time.deltaTime);
        }
    }

    public void ReCalculateForOnlineChar(GameObject _obj, int index)
    {
        character = _obj;
        transform.position = character.transform.position - mDistance;
        Debug.LogError("ReCalculateForOnlineChar call came: "+ _obj.name + " : "+ _obj.transform.position);

        if (index >= 2)
        {
            transform.RotateAround(character.transform.position, Vector3.down, 180);
            mDistance = character.transform.position - transform.position;
        }

    }
}
