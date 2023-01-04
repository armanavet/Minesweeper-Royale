using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterController : MonoBehaviour
{
    public int blood = 5;

    private NavMeshAgent mMeshAgent;

    private Brick mPreviousBrick;

    private Brick mCurrentBrick;

    public int currentTile;
    public TMP_Text currentText;
    public Animator anim;
    public GameObject Man, Ragdoll;
    public bool lost;
    public float explForce;
    public GameObject ExplosionEffect;
    // Start is called before the first frame update
    void Start()
    {
        mMeshAgent = GetComponent<NavMeshAgent>();
        currentTile = 0;
        lost = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!lost)
        {
            currentText.text = currentTile.ToString();
            currentText.gameObject.SetActive(!lost);
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject hitObject = hit.transform.gameObject;
                    Brick brick = hitObject.GetComponent<Brick>();
                    if (brick != null)
                    {
                        mMeshAgent.SetDestination(hit.transform.position);
                    }
                }
            }
            anim.SetBool("run", mMeshAgent.velocity != Vector3.zero);
            DetectMine();
        }

    }

    private void DetectMine()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.SphereCast(ray, 0.2f, out hit)) {
            Debug.Log(hit.transform.name);
            GameObject hitObject = hit.transform.gameObject;
            Brick brick = hitObject.GetComponent<Brick>();
            if (brick != null) {
                brick.ShowSecret(true);

                if (brick.mine && mPreviousBrick != null) {
                    //mMeshAgent.SetDestination(mPreviousBrick.transform.position);
                    blood -= 1;
                    Man.SetActive(false);
                    Ragdoll.SetActive(true);
                    Ragdoll.transform.parent = null;
                    var rbs = Ragdoll.GetComponentsInChildren<Rigidbody>();
                    foreach (var rb in rbs)
                    {
                        lost = true;
                        rb.AddExplosionForce(explForce, rb.position, 2);
                        Instantiate(ExplosionEffect, transform.position,new Quaternion());
                        Invoke("Restart", 2);
                    }
                }

                if (brick != mCurrentBrick) {
                    mPreviousBrick = mCurrentBrick;
                    mCurrentBrick = brick;
                }
            }
        }
    }


    void Restart()
    {
        SceneManager.LoadScene(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Finish"))
        {
            lost = true;
            anim.SetTrigger("cheer");
            Destroy(other.gameObject);
        }    
    }
}
