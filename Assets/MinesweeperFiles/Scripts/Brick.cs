using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Brick : MonoBehaviour
{    
    private static Dictionary<string, Sprite> mTileImages;

    public bool mine = false;

    public float radius = 1.42f;

    public SpriteRenderer tile = null;

    public List<Brick> mNeighbors;

    public bool mShowed = false;
    public GameObject TargetGraphic;
    public bool IsTarget = false;
    public GameObject ExplosionEffect;
    public static void BuildSpritesMap()
    {
        if (mTileImages == null) {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/MinesweeperSpritesheet");
            mTileImages = new Dictionary<string, Sprite>();
            for (int i = 0; i < sprites.Length; i++) {
                mTileImages.Add(sprites[i].name, (Sprite) sprites[i]);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
        BuildSpritesMap();
    }

    // Update is called once per frame
    void Update()
    {
        TargetGraphic.SetActive(IsTarget);
    }

    void OnValidate()
    {
        FindNeighbors();
    }

    private void FindNeighbors()
    {
        var allBricks = GameObject.FindGameObjectsWithTag("Brick");

        mNeighbors = new List<Brick>();

        for (int i = 0; i < allBricks.Length; i++) {
            var brick = allBricks[i];
            var distance = Vector3.Distance(transform.position, brick.transform.position);
            if (0 < distance && distance <= radius) {
                mNeighbors.Add(brick.GetComponent<Brick>());
            }
        }

        Debug.Log($"{mNeighbors.Count} neighbors");
    }

    public void ShowSecret(bool reveal)
    {
        //if (mShowed) return;
        IsTarget = false;
        mShowed = true;

        string name;

        if (mine) {
            name = "TileMine";
            Instantiate(ExplosionEffect, transform.position + Vector3.up * 2, Quaternion.identity);
        } 
        else {
            int num = 0;
            mNeighbors.ForEach(brick => {
                if (brick.mine) num += 1;
            });
            name = $"Tile{num}";
            FindObjectOfType<CharacterController>().currentTile = num;
            if(num==0&& reveal)
            {
                RevealNeighbors();
            }
 
        }

        Sprite sprite;
        if (mTileImages.TryGetValue(name, out sprite))
            tile.sprite = sprite;
    }

    public void RevealNeighbors()
    {
        foreach (var n in mNeighbors)
        {
            n.ShowSecret(n.mine);
        }
    }
}
