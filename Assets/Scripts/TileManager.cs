using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileManager : MonoBehaviour {
    
    public int W;
    public int H;
    public GameObject tilePrefab;

    public bool ______________________________;

    public GameObject[,] tiles;

    void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
        tiles = new GameObject[W, H];
        for (int i = 0; i < W; i++)
        {
            for (int j = 0; j < H; j++)
            {
                tiles[i, j] = Instantiate<GameObject>(tilePrefab);
                tiles[i, j].transform.position = new Vector3(i, j, 0);
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
    }
}
