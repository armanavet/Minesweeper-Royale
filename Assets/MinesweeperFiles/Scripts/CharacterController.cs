using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CharacterController : MonoBehaviour, IPunObservable
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
    public GameObject Fireworks;
            internal PhotonView photonView;
            private CharacterController[] allChar;
            private List<int> IDs;
            private Vector3 navmeshDest_Network;
        
            private int myViewId;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        List<Brick> PossibleTargets = new List<Brick>();
        mMeshAgent = GetComponent<NavMeshAgent>();
        currentTile = 0;
        lost = false;

        photonView = GetComponent<PhotonView>();
        
        if (ThrowButton == null)
            ThrowButton = GameObject.FindObjectOfType<GameManager>().throwButton;
        
         if (GameManager.gameMode == GameMode.Online) 
         {
             yield return new WaitForSeconds(0.1f);
             myViewId = photonView.ViewID;
             if (myViewId >= 5000)
             {
                 allChar = GameObject.FindObjectsOfType<CharacterController>();
                 IDs = new List<int>() {1001, 2001, 3001, 4001 };
                 for (int i = 0; i < allChar.Length; i++)
                 {
                     IDs.Remove(allChar[i].photonView.ViewID);
                 }
                
                 myViewId = IDs[0];
             }
            
             Debug.LogError("My View ID: " + myViewId);
             Material[] allMats = Man.GetComponentInChildren<SkinnedMeshRenderer>().materials;
             allMats[1] =
                 GameObject.FindObjectOfType<GameManager>().allMats[(myViewId / 1000)-1];
             Man.GetComponentInChildren<SkinnedMeshRenderer>().materials = allMats;
             Vector3 pos = GameObject.FindObjectOfType<GameManager>().allInitialPos[(myViewId / 1000)-1].transform
                                                       .position;
             this.gameObject.transform.position = new Vector3(pos.x, this.transform.position.y, pos.z) ;
             this.gameObject.transform.eulerAngles =  GameObject.FindObjectOfType<GameManager>().allInitialPos[(myViewId / 1000)-1].transform.eulerAngles;
             if (photonView.IsMine)
             {
                 this.gameObject.name += "_Me";
                Debug.Log("making me: "+ this.gameObject.name + " : "+ this.gameObject.transform.position);
                 GameObject.FindObjectOfType<CameraFollower>().ReCalculateForOnlineChar(this.gameObject, (myViewId / 1000)-1);
             }
         }
        
         yield return null;
         if (photonView.IsMine)
         {
             yield return new WaitForSeconds(2);
             ThrowButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
             ThrowButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(this.EnterThrowMode);
         }
    }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
               // Debug.Log("OnPhotonSerializedView: "+ Time.realtimeSinceStartup);
                    if (stream.IsWriting)
                    {
                       // Debug.LogError("Writing something: "+ this.gameObject.name);
                            stream.SendNext(info.photonView.gameObject.transform.position);
                    }
                if (stream.IsReading)
                    {
                      //  Debug.LogError("Reading something: " +  this.gameObject.name);
                            mMeshAgent.SetDestination((Vector3)stream.ReceiveNext());
                    }
        
                }
    // Update is called once per frame
    void Update()
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, throwMode ? 90 : 60, Time.deltaTime * 10);
        ThrowButton.SetActive(!throwMode&&!lost);
        if(!lost&&!throwMode)
        {
            //currentText.text = currentTile.ToString();
            //currentText.gameObject.SetActive(!lost);
            if (GameManager.gameMode == GameMode.Online && PhotonNetwork.IsConnected && !photonView.IsMine)
            {
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
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

                        StartCoroutine(Throw(brick));
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
           //Debug.Log(hit.transform.name);
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
                        if(photonView.IsMine)
                        Invoke("Lose", 2);
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
        if (PossibleTargets.Count > 0)
        {
            anim.SetTrigger("ready");
            throwMode = true;
        }
    }

    IEnumerator Throw(Brick brick)
    {
        
        anim.SetTrigger("throw");
        var projectile = Instantiate(rock, transform.position, Quaternion.identity);
        projectile.Target = brick.transform.position+Vector3.up*2;
        projectile.Hurl();
        throwMode = false;
        ThrowButton.GetComponent<Ability>().StartCoolDown();
        foreach (var target in PossibleTargets)
        {
            target.IsTarget = false;
        }
        yield return new WaitForSeconds(0.2f);
        brick.ShowSecret(true);
        if(brick.mine)
        {
            Instantiate(ExplosionEffect, brick.transform.position + Vector3.up *2, Quaternion.identity);
            brick.RevealNeighbors();
        }
        
        
    }

    void Win()
    {
        FindObjectOfType<UiManager>().Win();
    }
    void Lose()
    {
        FindObjectOfType<UiManager>().Lose();
    }

    void Restart()
    {
        if(photonView.IsMine)
        SceneManager.LoadScene(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Finish"))
        {
            lost = true;
            anim.SetTrigger("cheer");
            Destroy(other.gameObject);
            Instantiate(Fireworks, other.transform.position, Quaternion.identity);
            GameManager.ChangeGameStatus(GameStatus.gameOver);
            Invoke("Win", 2);
            GameObject.FindObjectOfType<RoomSelectionUI>().photonView.RPC("OnSomeoneElseWon", RpcTarget.Others);
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
