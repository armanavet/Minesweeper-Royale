using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Bot : MonoBehaviour
{
    [SerializeField] internal float smartness;
    public int blood = 5;

    private NavMeshAgent mMeshAgent;

    private Brick mPreviousBrick;

    [SerializeField] Brick mCurrentBrick;

    public int currentTile;
    public TMP_Text currentText;
    public Animator anim;
    public GameObject Man, Ragdoll;
    public bool lost;
    public float explForce;
    public GameObject ExplosionEffect;
    
    private float  clickTime = 2;
    private float currentClick = 0;
    
    const int row = 20;
    const int column = 20;
    [SerializeField] private Brick finalBlock;

    private Brick target;
    private bool stopMoving = false;
    private List<Brick> VisitedBricks = new List<Brick>();
    // Start is called before the first frame update
    void Start()
    {
        mMeshAgent = GetComponent<NavMeshAgent>();
        currentTile = 0;
        lost = false;
        clickTime = Random.Range(1.9f, 3.2f);
        GameManager.onGameOver += GameManagerOnonGameOver;
    }

    private void OnDestroy()
    {
        GameManager.onGameOver -= GameManagerOnonGameOver;
        
    }

    private void GameManagerOnonGameOver(bool _isWin)
    {
        stopMoving = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (stopMoving)
        {
            anim.SetBool("run", false);
            return;
        }

        if(!lost)
        {
            currentText.text = currentTile.ToString();
            currentText.gameObject.SetActive(!lost);
            currentClick += Time.deltaTime;
            
            if (currentClick > clickTime)
            {
                 target = GetClosestBlock();
                 VisitedBricks.Add(target);
                 currentClick = 0;
 
                 GameObject hitObject = target.transform.gameObject;
                 Brick brick = hitObject.GetComponent<Brick>();
                 if (brick != null)
                 {
                     mMeshAgent.SetDestination(target.transform.position);
                 }

                DetectMine();
                if (target == finalBlock)
                {
                   GameManager.ChangeGameStatus(GameStatus.gameOver);
                    stopMoving = true;
                }
            }
           
            anim.SetBool("run", mMeshAgent.velocity != Vector3.zero);
        }

    }
    Brick GetClosestBlock()
    {
        Brick closeBlock = null;
        bool fromStart = Vector3.Distance(finalBlock.transform.position, mCurrentBrick.mNeighbors[0].transform.position) <  
                         Vector3.Distance(finalBlock.transform.position, mCurrentBrick.mNeighbors[mCurrentBrick.mNeighbors.Count-1].transform.position);

        float shortDist = Mathf.Infinity;

        if (smartness < 20)
            fromStart = !fromStart;

        float bombChance = Random.Range(0, 100);
            for (int i = fromStart ? 0 : mCurrentBrick.mNeighbors.Count-1; 
                 fromStart ? i < mCurrentBrick.mNeighbors.Count : i >=0;)
            {
                if (smartness < 20)
                {
                    if (bombChance < 10 && mCurrentBrick.mNeighbors[i].mine)
                        return mCurrentBrick.mNeighbors[i];
                }

                if (!mCurrentBrick.mNeighbors[i].mine && !VisitedBricks.Contains(mCurrentBrick.mNeighbors[i]))
                {
                    if (closeBlock == null)
                    {
                        closeBlock = mCurrentBrick.mNeighbors[i];
                        shortDist = Vector3.Distance(finalBlock.transform.position,
                            mCurrentBrick.mNeighbors[i].transform.position);
                    }
                    else
                    {
                        float dist = Vector3.Distance(finalBlock.transform.position,
                            mCurrentBrick.mNeighbors[i].transform.position);

                        if (dist < shortDist)
                        {
                            closeBlock = mCurrentBrick.mNeighbors[i];
                            shortDist = dist;
                            
                        }
                    }

                    if (Random.Range(0, 100) > smartness)
                        break;
                    //return mCurrentBrick.mNeighbors[i];
                }

                if (fromStart) i++;
                else i--;
            }
     

        return mCurrentBrick.mNeighbors[0];
    }

    private void DetectMine()
    {
       GameObject hitObject = target.gameObject; //hit.transform.gameObject;
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
                    }
                }

                if (brick != mCurrentBrick) {
                    mPreviousBrick = mCurrentBrick;
                    mCurrentBrick = brick;
                }
            }
    }


    void Restart()
    {
        SceneManager.LoadScene(0);
    }


}
