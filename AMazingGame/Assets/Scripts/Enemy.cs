using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    int[,] theMaze;
    int x, z;

    bool up, down, left, right;


    [SerializeField] private float speed;
    bool stop = false;
    bool started = false;
    GameObject player;

    enum Direction { Up ,Down, Left, Right };
    Vector3 CurrentPosition()
    {
        return transform.position;
    }
    void Start()
    {
        x = Random.Range(0, 4);
        player = GameObject.FindWithTag("Player");
        up = true;
        down = true;
        left = true;
        right = true;
        //gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
        if (TestMaze.Started )
        {
            print("Both are true");
            theMaze = TestMaze.myMaze.A;
            //print("The maze isn't null: " + theMaze[0, 0]);
            started = true;
            

        }
        if (started && stop)
        {
            Debug.Log("Picking another spot!");
            x = Random.Range(0, 4);
            print("x: " + x);
            stop = false;
        }
        else
        {
            if(started)
                MoveCardinal(x);
        }
        
        if (transform.position == player.transform.position)
        {
            Destroy(player);
        }




    }

    

    
    private void OnCollisionEnter(Collision collision)
    {
        print("I hit: " + collision.gameObject.name);
    }
    void MoveCardinal(int Direction)
    {
        float posx = CurrentPosition().x;
        float posz = CurrentPosition().z;
        switch (Direction)
        {

            case 0: //Up
                //print("Up seems to be: " + theMaze[Mathf.RoundToInt(posx) - 1, Mathf.RoundToInt(posz)]);
                
                if (theMaze[Mathf.RoundToInt(posx), Mathf.RoundToInt(posz) + 1] == 1)
                {
                    print("There's a wall there!");
                    stop = true;
                }
                else
                {
                    Debug.DrawRay(transform.position, new Vector3(0,0,1), Color.red);
                    print("The spot's open");
                    print("Current Pos:" + CurrentPosition());
                    print("Going to: " + Mathf.RoundToInt(posx) + " , 0 , " + Mathf.RoundToInt(posz));
                    if (up)
                    {
                        up = false;
                        MoveUpWhile(new Vector3(Mathf.FloorToInt(posx), 0, Mathf.FloorToInt(posz) + 1));
                        stop = true;
                    }
                }
                break;
            case 1: //Down
                if (theMaze[Mathf.RoundToInt(posx), Mathf.RoundToInt(posz) - 1] == 1)
                {
                    print("There's a wall there!");
                    stop = true;
                }
                else
                {
                    Debug.DrawRay(transform.position,new Vector3(0,0,-1), Color.red);
                    print("The spot's open");
                    //MoveDown(new Vector3(Mathf.FloorToInt(posx), 0, Mathf.FloorToInt(posz) - 1));
                    if (down)
                    {
                        down = false;
                        MoveDownWhile(new Vector3(Mathf.RoundToInt(posx), 0, Mathf.RoundToInt(posz) - 1));
                        stop = true;
                    }
                }
                break;
            case 2: //Left
                if (theMaze[Mathf.RoundToInt(posx) - 1, Mathf.RoundToInt(posz)] == 1)
                {
                    print("There's a wall there!");
                    stop = true;
                }
                else
                {
                    Debug.DrawRay(transform.position, new Vector3(-1, 0, 0), Color.red);
                    print("Going Left");
                    print("The spot's open");
                    if (left)
                    {
                        left = false;
                        MoveLeftWhile(new Vector3(Mathf.RoundToInt(posx) - 1, 0, Mathf.RoundToInt(posz)));
                        stop = true;
                    }
                }
                break;
            case 3: //Right
                if (theMaze[Mathf.RoundToInt(posx) + 1, Mathf.RoundToInt(posz)] == 1)
                {
                    print("There's a wall there!");
                    stop = true;
                }
                else
                {
                    Debug.DrawRay(transform.position, new Vector3(1, 0, 0), Color.red);
                    print("going right");
                    print("The spot's open");
                    if (right)
                    {
                        right = false;
                        MoveRightWhile(new Vector3(Mathf.RoundToInt(posx) + 1, 0, Mathf.RoundToInt(posz)));
                        stop = true;
                    }
                }
                break;
        }
    }
    void RoundOffPos()
    {
        transform.localPosition = new Vector3(Mathf.RoundToInt(CurrentPosition().x), 0, Mathf.RoundToInt(CurrentPosition().z));
    }
    void MoveUpWhile(Vector3 upV)
    {

        int tenthUnit = 0;
        while (tenthUnit != 10)
        {
            up = false;
            transform.Translate(0, 0, .1f);
            tenthUnit++;
        }
        RoundOffPos();
        up = true;

    }
    void MoveDownWhile(Vector3 downV)
    {

        int tenthUnit = 0;
        while (tenthUnit != 10)
        {
            down = false;
            transform.Translate(0, 0, -.1f);
            tenthUnit++;
        }
        RoundOffPos();
        down = true;

    }
    void MoveLeftWhile(Vector3 leftV)
    {

        int tenthUnit = 0;
        while (tenthUnit != 10)
        {
            left = false;
            transform.Translate(-.1f, 0, 0);
            print("Should've moved left");
            tenthUnit++;
        }
        RoundOffPos();
        left = true;

    }

    void MoveRightWhile(Vector3 rightV)
    {
        int tenthUnit = 0;
        while (tenthUnit != 10)
        {
            right = false;
            transform.Translate(.1f, 0, 0);
            tenthUnit++;
        }
        RoundOffPos();
        right = true;

    }
}
