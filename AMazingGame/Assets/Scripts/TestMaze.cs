using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestMaze : MonoBehaviour
{
    [SerializeField] private int height;
    [SerializeField] private int width;
    [SerializeField] private InputField heightI;
    [SerializeField] private InputField widthI;
    [SerializeField] private GameObject Plane;
    [SerializeField] private GameObject PlaneChild;
    [SerializeField] private GameObject Cube;
    [SerializeField] private GameObject Floor;
    [SerializeField] private GameObject Camera;
    GameObject player;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject enemy2;
    [SerializeField] private GameObject enemy3;
    [Space]
    [SerializeField] private int[,] theMaze;
    public static bool Started;
    public static bool numPressed;

    public static Maze myMaze;

    int randX;
    int randY;

    public int RandX
    {
        get
        {
            return randX;
        }
    }
    public int RandY
    {
        get
        {
            return randY;
        }
    }

    
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        enemy = GameObject.FindWithTag("Enemy");
        enemy.SetActive(false);
        enemy2 = GameObject.FindWithTag("Enemy2");
        enemy2.SetActive(false);
        enemy3 = GameObject.FindWithTag("Enemy3");
        enemy3.SetActive(false);
        GenerateMazeWithEverything();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            print("I pressed F1");
            GenerateMazeWithEverything();
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            print("Pressed 1");
            numPressed = true;
            enemy.SetActive(true);
                
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            print("Pressed 1");
            numPressed = true;
            enemy2.SetActive(true);

        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            print("Pressed 1");
            numPressed = true;
            enemy3.SetActive(true);

        }
    }
    void GenerateMaze()
    {
            myMaze = new Maze(height, width);
            try
            {
                if (height % 2 == 0 && width % 2 == 0)
                {
                    print("Even");
                    throw new UnityException("Not Odd! Cannot do!");
                }
                else
                {
                    print("Odd");
                    Debug.Log(myMaze.GenerateMazeBT());
                }
            }
            catch (UnityException ex)
            {
                Debug.Log(ex.Message);
            }

            //Plane.transform.localScale.x = (float)height;
            theMaze = myMaze.A;
            Debug.Log(theMaze[0, 0]);
            for (int i = 0; i < height; i++)
            {
                //print("inside");
                for (int j = 0; j < width; j++)
                {
                    if (theMaze[i, j] == 1)
                        Instantiate(Cube, new Vector3(i, 0, j), Quaternion.identity);
                    if (theMaze[i,j] == 0)
                        Instantiate(Floor, new Vector3(i, -1, j), Quaternion.identity);

                }
            }
            //print("outside");
            
            Camera.transform.localPosition += new Vector3(0, width);
            
    }

    public void GenerateMazeWithEverything()
    {
        Started = true;
        ResetMaze();
        GenerateMaze();
        ChooseRandPos();
        //Debug.Log("The random position to spawn the player! \n X: " + randX + "\n Y: " + randY);
        Spawn(randX, randY);
        ChooseRandPos();
        SpawnEnemy(randX, randY);
        ChooseRandPos();
        SpawnEnemy2(randX, randY);
        ChooseRandPos();
        SpawnEnemy3(randX, randY);
        //Start();
    }
    void ChooseRandPos()
    {
        randX = UnityEngine.Random.Range(0, height - 1);
        randY = UnityEngine.Random.Range(0, width - 1);
    }
    void ResetMaze()
    {
        GameObject[] allCubes = GameObject.FindGameObjectsWithTag("Cube");
        GameObject[] allFloors = GameObject.FindGameObjectsWithTag("Floor");
        for (int i = 0; i < allCubes.Length; i++)
        {
            Destroy(allCubes[i]);
            
        }
        for (int i = 0; i < allFloors.Length; i++)
        {
            Destroy(allFloors[i]);
        }
        Plane.transform.localScale = new Vector3(1, 1, 1);
        Camera.transform.localPosition = new Vector3(4.75f, 20f, 7f);
    }
    void Spawn(int x, int y)
    {

        bool spawned = false;

        Debug.Log(x + ", " + 0 + ", " + y);
        if (theMaze[x, y] == 1)
        {
            //print("There's a wall here!");
            randX = UnityEngine.Random.Range(0, height - 1);
            randY = UnityEngine.Random.Range(0, width - 1);
            //print("New Coordinates: " + randX + ", " + RandY);
            Spawn(randX, RandY);
        }
        else
        {
            player.transform.position = new Vector3(x, 0, y);
            spawned = true;
        }

    }
    void SpawnEnemy(int x, int y)
    {

        bool spawned = false;

        Debug.Log(x + ", " + 0 + ", " + y);
        if (theMaze[x, y] == 1)
        {
            //print("There's a wall here!");
            randX = UnityEngine.Random.Range(0, height - 1);
            randY = UnityEngine.Random.Range(0, width - 1);
            //print("New Coordinates: " + randX + ", " + RandY);
            SpawnEnemy(randX, RandY);
        }
        else
        {
            enemy.transform.position = new Vector3(x, 0, y);
            spawned = true;
        }

    }
    void SpawnEnemy2(int x, int y)
    {

        bool spawned = false;

        Debug.Log(x + ", " + 0 + ", " + y);
        if (theMaze[x, y] == 1)
        {
            //print("There's a wall here!");
            randX = UnityEngine.Random.Range(0, height - 1);
            randY = UnityEngine.Random.Range(0, width - 1);
            //print("New Coordinates: " + randX + ", " + RandY);
            SpawnEnemy2(randX, RandY);
        }
        else
        {
            enemy2.transform.position = new Vector3(x, 0, y);
            spawned = true;
        }

    }
    void SpawnEnemy3(int x, int y)
    {

        bool spawned = false;

        Debug.Log(x + ", " + 0 + ", " + y);
        if (theMaze[x, y] == 1)
        {
            //print("There's a wall here!");
            randX = UnityEngine.Random.Range(0, height - 1);
            randY = UnityEngine.Random.Range(0, width - 1);
            //print("New Coordinates: " + randX + ", " + RandY);
            SpawnEnemy3(randX, RandY);
        }
        else
        {
            enemy3.transform.position = new Vector3(x, 0, y);
            spawned = true;
        }

    }


}
