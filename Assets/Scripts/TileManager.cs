using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileManager : MonoBehaviour {
    
    public GameObject tilePrefab;
    public GameObject playerPrefab;
    public GameObject enemySpawnerPrefab;
    public TextAsset levelAsset; // this is the file containing the level that will be loaded

    public bool ______________________________;

    // colors used for level design
    Color32 playerSpawnColor = new Color32(0, 255, 0, 255); // green
    Color32 tileColor = new Color32(0, 0, 0, 255); // black
    Color32 enemySpawnColor = new Color32(255, 0, 0, 255); // red
    Color32 airColor = new Color32(255, 255, 255, 255); // white
    int spawnerCount = 0;

    void Awake()
    {
        LoadLevel();
    }
    
    // Update is called once per frame
    void Update()
    {
    }

    // Loads an image level into the scene
    void LoadLevel() {
        Texture2D tex = new Texture2D(1, 1); // create an arbitrary texture object
        tex.LoadImage(levelAsset.bytes); // load the image into the texture object
        Color32[] pixels = tex.GetPixels32(); // store all the pixels into a usable format

        for (int i = 0; i < pixels.Length; i++) { // for each pixel in the map
            int x, y;
            Convert1D(i, tex.width, tex.height, out x, out y); // determine the pixel's x,y coordinates
            GameObject obj = ColorToObject(pixels[i]); // determine which object is represented by the pixel's color
            PlaceObject(x, y, obj); // place the object into the scene
        }

    }

    // takes X and Y coordinates and converts them to their position in a 1-dimensional array
    int ConvertXY(int x, int y, int width, int height) {
        return y * width + x;
    }

    // takes the index inside a 1-dimensional array and returns the corresponding x and y coordinates
    void Convert1D(int index, int width, int height, out int x, out int y) {
        y = index / width;
        x = index - y*width;
    }

    // determines the game object corresponding to the given color
    GameObject ColorToObject(Color32 color) {
        if (color.Equals(playerSpawnColor)) {
            return playerPrefab;
        }
        else if (color.Equals(tileColor)) {
            return tilePrefab;
        }
        else if (color.Equals(enemySpawnColor)) {
            return enemySpawnerPrefab;
        }
        else if (color.Equals(airColor)) {
            return null;
        }
        else {
            print("An unrecognized color was found in the map! Something is wrong!");
            return null;
        }
    }

    // places an object onto the level given its position
    void PlaceObject(int x, int y, GameObject obj) {
        if (obj == null) {
            return; // don't instantiate air
        }
        GameObject GO = Instantiate(obj); // instantiate the object
        GO.name = obj.name; // remove (Clone) from the name       
        GO.transform.position = new Vector3(x, y, 0); // place it in the correct spot

        // give spawners unique names for the EnemyManager
        if(GO.name == "Spawner") {
            GO.name = "Spawner" + (++this.spawnerCount).ToString();
        }
    }
}
