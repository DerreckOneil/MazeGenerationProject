using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    int x;
    int y;
    float f, g, h;
    Node parent;
    
    public Node(int x, int y, Node parent)
    {
        this.x = x;
        this.y = y;
        this.parent = parent;
    }
    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
        //this.parent = parent;
    }
    public int X
    {
        get
        {
            return x;
        }
        set
        {
            x = value;
        }
    }
    public int Y
    {
        get
        {
            return y;
        }
        set
        {
            y = value;
        }
    }
    #region Getters and setters for F,G,H
    public float F
    {
        get
        {
            return f;
        }
        set
        {
            this.f = value;
        }
    }
    public float G
    {
        get
        {
            return g;
        }
        set
        {
            this.g = value;
        }
    }
    public float H
    {
        get
        {
            return h;
        }
        set
        {
            this.h = value;
        }
    }
    #endregion
    public Node Parent
    {
        get
        {
            return this.parent;
        }
        set
        {
            this.parent = value;
        }
    }
        
    public string Position()
    {
        return "Position " + x + ", " + y ;
    }
    public string Cost()
    {
        return "For the position: " + Position() + "\nF: " + f + "\nG: " + g + "\nH: " + h;
    }
    public float GValue(Node startNode, Node currentPos)
    {
        //Normal distance formula dx+dy
        
        return Mathf.Abs((currentPos.X - startNode.X) + (currentPos.Y - startNode.Y));
    }
    public float HValue(Node currentPos, Node finishNode)
    {
        //Use The Cartesian Plane Distance Formula
        //((x2-x1)^2 + (y2-y1)^2)^1/2
        return Mathf.Sqrt(Mathf.Pow(finishNode.X - currentPos.X, 2) + Mathf.Pow(finishNode.Y - currentPos.Y, 2));
    }
}
