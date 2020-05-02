using System;
using System.Diagnostics;




public class Maze
{
    private static int height;
    private static int width;
    int[,] a;
    int randomNum;
    int randomPosNode;
    int randomNumCost;

    int cardinalPos;
    Random randomDir = new Random();

    bool cornerAchieved;
    bool possible = true;

    int x;
    int y;

    int oppX;
    int oppY;

    bool diagonal;
    bool adj;

    public Maze(int height, int width)
    {
        this.Height = height;
        this.Width = width;
        a = new int[height, width];
    }

    public int[,] A
    {
        get
        {
            return a;
        }
    }

    public int Height
    {
        get
        {
            return height;
        }
        set
        {
            height = value;
        }

    }
    public int Width
    {
        get
        {
            return width;
        }
        set
        {
            width = value;
        }

    }

    public string PrintMaze()
    {
        string msgString = "\n";
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {

                msgString = msgString + a[i, j];
            }
            msgString += "\n";
        }
        return msgString;


    }
    int b;
    public string GenerateMazeBT()
    {
        StartOffWithZeros();
        //a[0, 0] = 1;
        x = 0;
        y = 0;
        Random BTrand = new Random();
        bool equalToWhatINeed = false;
        a[x, y] = 1;

        //While Loop
        while (!equalToWhatINeed)
        {

            if (x == 0)
            {
                BridgeLeft();
            }
            else if (x == height - 1)
            {
                BridgeLeft();
            }
            else
            {
                b = BTrand.Next(1, 3);

                //Check();
                switch (b)
                {
                    case 1: //Bridge Up
                        BridgeUp();
                        break;
                    case 2: //Bridge Left
                        BridgeLeft();
                        break;
                }
            }

            if ((x == (height - 1)) && (y == (width - 1)))
            {
                if (b == 1)
                {
                    a[x,y] = 1;
                    a[x - 1,y] = 1;
                }
                else
                {
                    a[x,y] = 1;
                    a[x,y - 1] = 1;
                }

                equalToWhatINeed = true;
                a[x - 1,y] = 1;
                a[x,y - 1] = 1;
            }
        }


        string msgString = "\n";
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {

                msgString = msgString + a[i, j];
            }
            //Debug.Write("\n");
            msgString += "\n";
        }
        msgString += "\n Algorithm written by Derreck. This algorithm uses a binary tree like simulation to either build the maze from the right =>left or from the right => up Inspired by: https://medium.com/analytics-vidhya/maze-generations-algorithms-and-visualizations-9f5e88a3ae37 ";
        return msgString;

    }

    void BridgeUp()
    {
        //Starting at [0,0]
        //a[x,y+2]

        if ((x >= 2) && (x <= height - 1) && (y <= width))
        {

            //a[x-2][y] = 1;
            a[x, y] = 1;
            a[x - 1, y] = 1;
            MoveRightTwoUnits();


        }
        else
        {
            x += 2;
            y = 0;
        }
        //y += 2;


    }
    void BridgeLeft()
    {
        //Starting at [0,0]

        if (y <= width - 1 && y != 0) //If you can no longer go left, start two units down
        {
            if (y == width - 1 && x != 0)
            {
                
                a[x,y] = 1;
                a[x - 1,y] = 1;
            }
            else
            {
                a[x,y] = 1;
                a[x,y - 1] = 1;
            }

            MoveRightTwoUnits();
            //a[x] [y+2] = 1;
            //System.out.println("Went two units right");
            //a[x] [y+1] = 1;
            //System.out.println("Went one unit right");

        }
        else if (y == 0)
        {

            a[x,y] = 1;
            if (x != 0)
            {
                a[x - 1,y] = 1;
            }
            MoveRightTwoUnits();

            //BridgeUp();
        }
        else if (x == 0 && y == 0)
        {
            a[x,y + 1] = 1;
            MoveRightTwoUnits();
        }
        else
        {
            x += 2;
            y = 0;
        }
    }
    void MoveRightTwoUnits()
    {
        y += 2;
    }
    public void StartOffWithZeros()
    {
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                a[i, j] = 0;
            }
    }
    public bool isWall(int row, int column)
    {
        return (a[row, column] == 1) ? true : false;
    }
}


