using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    int row, column;

    int Dir;

    int[,] theMaze;
    bool up, down, left, right;
    bool arrived;
    bool canMove = true;
    [SerializeField] private float speed;
    float x, z;
    public Vector3 CurrentPosition()
    {
        return transform.position;
    }
    public Vector3 GridPos
    {
        get
        {
            return CurrentPosition();
        }
    }

    void Start()
    {
        up = true;
        down = true;
        left = true;
        right = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TestMaze.Started)
        {
            theMaze = TestMaze.myMaze.A;
            //print("The maze isn't null: " + theMaze[0, 0]);
        }
        if (!canMove) { return;}
        
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
       
        if (z > 0)
        {
            print("Moving up");
            if (up == true)
            {
                canMove = false;
                MoveCardinal(1);
                canMove = true;
            }
            
        }
        else if (z < 0)
        {
            print("Moving down");
            if (down == true)
            {
                canMove = false;
                MoveCardinal(2);
                canMove = true;
            }
        }
        if (x > 0)
        {
            print("Moving Right");
            if (right == true)
            {
                canMove = false;
                MoveCardinal(4);
                canMove = true;
            }
        }
        else if( x < 0)
        {
            print("Moving Left");
            if (left == true)
            {
                canMove = false;
                MoveCardinal(3);
                canMove = true;
            }
        }

        
        //transform.Translate(x*Time.deltaTime*speed, 0.0f ,z*Time.deltaTime*speed);
        
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

            case 1: //Up
                print(CurrentPosition());
                print("Up seems to be: " + theMaze[Mathf.RoundToInt(posx) - 1, Mathf.RoundToInt(posz)]);

                if(theMaze[Mathf.RoundToInt(posx), Mathf.RoundToInt(posz) + 1] == 1)
                {
                    print("There's a wall there!"); 
                }
                else
                {
                    print("The spot's open");
                    print("Current Pos:" + CurrentPosition());
                    print("Going to: " + Mathf.RoundToInt(posx) + " , 0 , " + Mathf.RoundToInt(posz) +1 );
                   
                      
                        MoveUpWhile(new Vector3(Mathf.FloorToInt(posx), 0, Mathf.FloorToInt(posz) + 1));
                 
                }
                break;
            case 2: //Down
                if (theMaze[Mathf.RoundToInt(posx), Mathf.RoundToInt(posz) - 1] == 1)
                {
                    print("There's a wall there!");
                }
                else
                {
                    print("The spot's open");
                    //MoveDown(new Vector3(Mathf.FloorToInt(posx), 0, Mathf.FloorToInt(posz) - 1));
                    
                        MoveDownWhile(new Vector3(Mathf.RoundToInt(posx), 0, Mathf.RoundToInt(posz) - 1));
                    
                }
                break;
            case 3: //Left
                if (theMaze[Mathf.RoundToInt(posx) - 1, Mathf.RoundToInt(posz)] == 1)
                {
                    print("There's a wall there!");
                }
                else
                {
                    print("Going Left");
                    print("The spot's open");
                    
                        MoveLeftWhile(new Vector3(Mathf.RoundToInt(posx) - 1, 0, Mathf.RoundToInt(posz)));
                    
                }
                break;
            case 4: //Right
                if (theMaze[Mathf.RoundToInt(posx) + 1, Mathf.RoundToInt(posz)] == 1)
                {
                    print("There's a wall there!");
                }
                else
                {
                    print("going right");
                    print("The spot's open");
                   
                        MoveRightWhile(new Vector3(Mathf.RoundToInt(posx) + 1, 0, Mathf.RoundToInt(posz)));
                    
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
        
        /*
        float distToTarget = 0; 
        arrived = false;
        while (!arrived)
        {
            distToTarget = Vector3.Distance(gameObject.transform.position, upV);
            if (distToTarget == 0)
            {
                arrived = true;
                print("Stop moving Up! Return!");
                return;
            }
            else
                arrived = false;

            print("distance: " + distToTarget);
            transform.position = Vector3.MoveTowards(gameObject.transform.position, upV, speed * Time.deltaTime);

        }
        */

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
