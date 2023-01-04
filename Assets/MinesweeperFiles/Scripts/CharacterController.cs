using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    public bool throwMode;
    public GameObject ThrowButton;
    List<Brick> PossibleTargets;
    public float ThrowDist;
    public throwable rock;
    // Start is called before the first frame update
    void Start()
    {
        List<Brick> PossibleTargets = new List<Brick>();
        mMeshAgent = GetComponent<NavMeshAgent>();
        currentTile = 0;
        lost = false;
    }

    // Update is called once per frame
    void Update()
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, throwMode ? 90 : 60, Time.deltaTime * 10);
        ThrowButton.SetActive(!throwMode);
        if(!lost&&!throwMode)
        {
            currentText.text = currentTile.ToString();
            currentText.gameObject.SetActive(!lost);
            if (Input.GetMouseButtonDown(0)&& !IsPointerOverUIObject())
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
        if(throwMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject hitObject = hit.transform.gameObject;
                    Brick brick = hitObject.GetComponent<Brick>();
                    if (brick != null&&brick.IsTarget)
                    {
                        Throw(brick);
                    }
                }
            }
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
                        //Instantiate(ExplosionEffect, transform.position,new Quaternion());
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

    public void EnterThrowMode()
    {
        throwMode = true;
        var bricks = FindObjectsOfType<Brick>();
        PossibleTargets = new List<Brick>();
        foreach (var brick in bricks)
        {
            if(!brick.mShowed&&Vector3.Distance(brick.transform.position,transform.position)<ThrowDist)
            {
                PossibleTargets.Add(brick);
                brick.IsTarget = true;
            }
        }
    }

    public void Throw(Brick brick)
    {
        var projectile = Instantiate(rock, transform.position, Quaternion.identity);
        projectile.Target = brick.transform.position+Vector3.up*2;
        projectile.Hurl();
        brick.ShowSecret(true);
        if(brick.mine)
        {
            Instantiate(ExplosionEffect, brick.transform.position + Vector3.up *2, Quaternion.identity);
            brick.RevealNeighbors();
        }
        foreach (var target in PossibleTargets)
        {
            target.IsTarget = false;
        }
        throwMode = false;
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

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
