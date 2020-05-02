using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAStar : MonoBehaviour
{
    int fCost, //Total cost of the node
        gCost, //Distance between the current node and start node
        hCost; // distance from the current node to the end node
    //ScriptableObject
    float fCostN,
        fCostS,
        fCostE,
        fCostW;

    Node firstNode;
    Node endNode;
    Node comingFrom;
    Node lastTraversedNode;
    Node rootNode;

    int iterations = 1;
    int nodesAvailablePaths;
    int numberOfRoots;

    bool newRoot;

    GameObject sphere;

    [SerializeField] private List<Node> Open = new List<Node>();
    [SerializeField] private List<Node> Closed = new List<Node>();
    [SerializeField] private List<Node> solution = new List<Node>();
    [SerializeField] private List<Node> solution2 = new List<Node>();
    [SerializeField] private List<Node> parentNodes = new List<Node>();

    [SerializeField] private Stack nodesAtIntersections = new Stack();
    [SerializeField] private Queue messUpNodes = new Queue();
    [SerializeField] private Stack messedUpNodes = new Stack();

    bool grabbedArray;
    [SerializeField] private float speed;
    int[,] theMaze;
    [SerializeField] CharacterController playerPos;
    [SerializeField] private GameObject player;
    [SerializeField] Vector3 startPos;
    int x, y;
    bool pathGenerated;
    private void Start()
    {


    }
    // Update is called once per frame
    void Update()
    {

        if (TestMaze.Started && !grabbedArray)
        {
            print("Grabbing the grid");
            theMaze = TestMaze.myMaze.A;
            grabbedArray = true;
            startPos = transform.position;
            Debug.Log(transform.position);
            Debug.Log("Making first node at the spot: " + (int)transform.position.x + ", " + (int)transform.position.z);
            firstNode = new Node((int)transform.position.x, (int)transform.position.z, null);
            Debug.Log("grabbing data? " + firstNode.Position());
            endNode = new Node((int)playerPos.GridPos.x, (int)playerPos.GridPos.z, null);
            Open.Add(firstNode);
            solution.Add(firstNode);
            parentNodes.Add(firstNode);
            StartAStar(firstNode, playerPos.GridPos);
        }
        //print("Player Position: " + playerPos.GridPos);
        //if(!pathGenerated && Time.fixedTime % 2  == 0)
        //StartAStar(transform.position, playerPos.GridPos);
        if (transform.position == playerPos.GridPos)
        {
            print("In the same position as the player, Killing player ");
            Destroy(player);
        }

        DrawMyLine();
        DrawLinePointToPoint();
        if(pathGenerated)
        {
            print("Path generated, time to move!");
            Node[] a = null;
            a = solution.ToArray();

            for (int i = 0; i < a.Length;)
            {
                bool arrived = false;
                int distance = (int)Vector3.Distance(transform.position, new Vector3(a[i].X, 0, a[i].Y));
                transform.LookAt(new Vector3(a[i].X, 0, a[i].Y));
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
                if (distance == 0)
                {
                    print("Arrived!");
                    arrived = true;
                    i++;
                }

            }
            Destroy(player);
        }
    }
    
    void StartAStar(Node myPos, Vector3 Destination)
    {
        Debug.Log("PositionX: " + myPos.X + "\nPositionY: " + myPos.Y);
        //Remove the first node from the open list and add it to the closed list
        if (Open.Contains(firstNode))
        {

            if (iterations != 1)
            {
                Closed.Add(firstNode);
                print("First Node's still in...removing...");
            }
            Open.Remove(firstNode);
            comingFrom = firstNode;
            Debug.Log("For the second time! PositionX: " + myPos.X + "\nPositionY: " + myPos.Y);
        }
        else
        {
            //FirstNode doesn't exist so where am I coming from? 
            Debug.Log("Clearing the open list!" + iterations);
            Open.Clear();
            //comingFrom = solution.Find(x => (x.X.Equals(myPos.x) && x.Y.Equals(myPos.y)));
            //Debug.Log("I'm coming from: " + comingFrom.Position());
        }


        PlaceNodesInNeighbors(myPos);
    }
    void PlaceNodesInNeighbors(Node position)
    {
        Node CurrentNode = null;

        nodesAvailablePaths = 0;
        if (position.X == endNode.X && position.Y == endNode.Y)
        {
            print("I Made It!");
            Debug.Log(PrintOutSolutionSet());
           
            //***********************endgame
            DeleteAllUnneededNodes();
            Debug.Log(PrintOutSolutionSet());
           
           



            parentNodes.Add(position);
            pathGenerated = true;
            return;
        }
        //Can Go someplace from this position
        if (CanGoUp(position))
        {
            nodesAvailablePaths++;
            CurrentNode = new Node(position.X, position.Y + 1, position);
            PlaceNode(new Vector3(position.X, 0, position.Y + 1), CurrentNode);
            Debug.Log("Node Placed Up: " + CurrentNode.Position());
            //StartAStar(CurrentNode, playerPos.GridPos);
        }
        if (CanGoDown(position))
        {
            nodesAvailablePaths++;
            CurrentNode = new Node(position.X, position.Y - 1, position);
            PlaceNode(new Vector3(position.X, 0, position.Y - 1), CurrentNode);
            Debug.Log("Node Placed Down: " + CurrentNode.Position());
            //StartAStar(CurrentNode, playerPos.GridPos);
        }
        if (CanGoLeft(position))
        {
            nodesAvailablePaths++;
            CurrentNode = new Node(position.X - 1, position.Y, position);
            PlaceNode(new Vector3(position.X - 1, 0, position.Y), CurrentNode);
            Debug.Log("Node Placed Left: " + CurrentNode.Position());
            //StartAStar(CurrentNode, playerPos.GridPos);
        }
        if (CanGoRight(position))
        {
            nodesAvailablePaths++;
            CurrentNode = new Node(position.X + 1, position.Y, position);
            PlaceNode(new Vector3(position.X + 1, 0, position.Y), CurrentNode);
            Debug.Log("Node Placed Right: " + CurrentNode.Position());
            //StartAStar(CurrentNode, playerPos.GridPos);
        }
        print("My available paths are: " + nodesAvailablePaths);
        //CHECK TO SEE IF IM IN AN ADJ CELL
        if (nodesAvailablePaths >= 2)
        {
            print("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!This node's at a intersection!  pushing it onto stack \nX: " + position.X + " Y: " + position.Y);

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = new Vector3(position.X, 0, position.Y);
            numberOfRoots++;
            /*
            if (nodesAtIntersections.Count == 0 && !(position.X == firstNode.X && position.Y != firstNode.Y))
            {
                
                print("Stack Empty");
                rootNode = new Node(position.X, position.Y, null);
                print("Root node created! \n" + rootNode.Position());
                sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = new Vector3(rootNode.X, 0, rootNode.Y);
                
                newRoot = true;
            }
            */

            nodesAtIntersections.Push(new Node(position.X, position.Y, null));
        }
        
        Debug.Log("**********************************AllNodesPlaced**********************************");
        //Time to pick! Traverse through the list and find the node with the least F value!!!
        if(nodesAvailablePaths == 0)
        {
            print("rrrrrrrrrrrrrrrrrrrrrrrrrThis node is at the end of the root! \nPos: " +position.Position() + " \nit's parent: "+ position.Parent);
        }
        if (Open.Count >= 1)
            TraverseThroughAllNodes(position);
        else
        {
            Debug.Log("Cannot go anywhere else! ");
            Debug.Log(PrintOutSolutionSet());
            lastTraversedNode = position;
            print("LAST VISITED POSITION: " + lastTraversedNode.X + ", " + lastTraversedNode.Y);
            messedUpNodes.Push(lastTraversedNode);
            if (lastTraversedNode.X == endNode.X && lastTraversedNode.Y == endNode.Y)
            {
                pathGenerated = true;
            }
            else
            {

                pathGenerated = false;
                print("DEAD END REACHED! regenerate path! ");
                iterations++;
                Node newPath = null;
                newPath = (Node)nodesAtIntersections.Pop();
                print("Going to the new path: " + newPath.Position());
                newPath.Parent = null;
                print("My parent should be Null: ");
                LookForSurroundingNodes(lastTraversedNode, newPath);  
                DealWithAllMessedUpNodes(lastTraversedNode, newPath);
                
                //This node shouldn't exist in the set.

                //solution.Remove(position);
                GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //box.gameObject.GetComponent<Renderer>().material.color = Color.red;
                //box.transform.position = new Vector3(position.X, 0, position.Y);
                //RemoveAllNodesUntilNull(position);

                print("I shouldn't exist in the solution set, my position is " + position.Position());
                print("I shouldn't exist in the set!\n is it in the set? " + (solution.Contains(position)));



                //print("Next node is: " + (Node)nodesAtIntersections.Peek());  turned out null!
                /*
                if (nodesAtIntersections.Count == 0)
                {

                    print("Stack Empty");
                    rootNode = new Node(position.X, position.Y, null);
                    print("Root node created! \n" + rootNode.Position());
                    sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.position = new Vector3(rootNode.X, 0, rootNode.Y);

                    //sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //sphere.transform.position = new Vector3(rootNode.X, 0, rootNode.Y);

                    newRoot = true;
                }
                */
                print("Place a new node at this intersection:\nX: " + newPath.X + " Y: " + newPath.Y);
                parentNodes.Add(newPath);
                //if(iterations == 2)
                //Too many commands, it'll shut down unity! try waiting a second?
                StartCoroutine(WaitOneSecond(newPath));
                //StartAStar(firstNode, playerPos.GridPos);
            }
            return;
        }

    }
    void DeleteAllUnneededNodes()
    {
        while(messedUpNodes.Count > 0 )
        {
            Node currentNode = null;
            currentNode = (Node)messedUpNodes.Pop();
            print(currentNode.Position());
            solution.Remove(currentNode);
            Node thisNode = solution.Find(strangeNode => (strangeNode.X == currentNode.X) && (strangeNode.Y == currentNode.Y)); //The Lambda expression works!!!!! holy crap
            if (solution.Contains(thisNode))
            {
                print("Even though I was removed, I still exist in solution list");
                solution.Remove(thisNode);
            }
        }
        
    }
    void DealWithAllMessedUpNodes(Node lastNode, Node rootNode)
    {
        print("Last node (node where a Deadend has occured)" + lastNode.Position() + " \nRootNode : " + rootNode.Position());
        //print("I came from: " + lastNode.Parent.Position());
        print("Root node " + rootNode.Position() + " \nRootNode's Parent " + rootNode.Parent);
        //find all messed up nodes in the solution and remove them
        //find the nodes first
        print("comparing: " + lastNode.X + " and " + rootNode.X);
        print("comparing: " + lastNode.Y + " and " + rootNode.Y);

        if ((lastNode.X == rootNode.X) && (lastNode.Y == rootNode.Y)) //The coodinates are equal
        {
            return;
            
        }
        else
        {
            print("This Node has a parent, placing a red cube on it");
            //print("I came from: " + lastNode.Parent.Position());
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.gameObject.GetComponent<Renderer>().material.color = Color.red;
            box.transform.position = new Vector3(lastNode.X, 0, lastNode.Y);
            //box.transform.localScale = new Vector3(.5f, .5f,.5f);
            messedUpNodes.Push(lastNode);
            if (lastNode.Parent != null)
            {
                DealWithAllMessedUpNodes(lastNode.Parent, rootNode);
            }
            else
            {
                print("nnnnnnnnnnnnnnnnnnnnnnnnnMy parent is null!");
                print("backtracking has doubled the node that should be removed is: " + rootNode.Position());
                //messedUpNodes.Push(lastNode);
                messedUpNodes.Push(rootNode);
                //solution.Remove(rootNode);
                //Find all nodes surrounding this node for rootNode
                LookForSurroundingNodes(lastNode, rootNode);
                return;
            }
        }
        /*
        Node[] a;
        a = solution.ToArray();
        for(int i = 0; i<a.Length; i++)
        {
            //if()
        }
        */

    }
    void DealWithAllMessedUpNodesBacktrack(Node lastNode, Node theRoot)
    {
        print("Last node is now: " + lastNode.Position());
        print("theRoot: " + theRoot.Position());
        //find all messed up nodes in the solution and remove them
        //find the nodes first
        //print("comparing: " + lastNode.X + " and " + rootNode.X);
        //print("comparing: " + lastNode.Y + " and " + rootNode.Y);
        if (lastNode == null) //The coodinates are equal
        {
            return;

        }
        else
        {
            print("This Node has a parent, placing a red cube on it");
            //print("I came from: " + lastNode.Parent.Position());
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.gameObject.GetComponent<Renderer>().material.color = Color.red;
            box.transform.position = new Vector3(lastNode.X, 0, lastNode.Y);
            box.transform.localScale = new Vector3(.5f, .5f, .5f);
            messedUpNodes.Push(lastNode);
            //Recursively
            if (lastNode.Parent != null)
            {
                if ((lastNode.Parent.X != theRoot.X) && (lastNode.Parent.Y != theRoot.Y))
                    DealWithAllMessedUpNodesBacktrack(lastNode.Parent, theRoot);
                else
                {
                    print("nnnnnnnnnnnnnnnnnnnnnnnnnMy parent is null!");

                    //Find all nodes surrounding this node for rootNode
                    //LookForSurroundingNodes(lastNode, theRoot); // very promising with the parents commented out
                    return;
                }
            }
            
        }
        /*
        Node[] a;
        a = solution.ToArray();
        for(int i = 0; i<a.Length; i++)
        {
            //if()
        }
        */

    }

    Node LookForSurroundingNodes(Node nodeAtIntersection, Node theRoot)
    {
        Node[] a = null;
        Node lastSpot = null;
        a = solution.ToArray();
        //check all possibilities and make sure their parent is not equal to the current spot.
        x = nodeAtIntersection.X;
        y = nodeAtIntersection.Y;
        //Node upNodeParent = null;
        //upNodeParent = 
        //Up
        print("X: " + x + "\nY: " + y);
        if (theMaze[x, y + 1] == 1)
        {
            print("The maze won't allow it" + " X: " + x + " Y: " + y);
        }
        else
        {
            print("I can go up ");
            for(int i=0;i< a.Length; i++)
            {
                //Is the node in this spot in the solution?
                
                
                if((nodeAtIntersection.X == a[i].X && (nodeAtIntersection.Y + 1) == a[i].Y ))
                {
                    print("The up node is in the solution");
                    lastSpot = a[i];
                    print(a[i].Parent.Position());
                    solution.Remove(nodeAtIntersection);
                    if ((a[i].Parent.X == nodeAtIntersection.X) && (a[i].Parent.Y == nodeAtIntersection.Y))
                    {
                        print("My parent is the root");
                    }
                    else
                    {
                        print("this node and it's parents must be destroyed!");
                        //solution.Remove(nodeAtIntersection);
                        DealWithAllMessedUpNodes(lastSpot, theRoot);
                    }

                    //return lastSpot;
                }
            }
        }
        //Down
        print("X: " + x + "\nY: " + y);
        if (theMaze[x, y - 1] == 1)
        {
            print("The maze won't allow it" + " X: " + x + " Y: " + y);
        }
        else
        {
            print("I can go Down ");
            for (int i = 0; i < a.Length; i++)
            {
                //Is the node in this spot in the solution?


                if ((nodeAtIntersection.X == a[i].X && (nodeAtIntersection.Y - 1) == a[i].Y))
                {
                    print("The down node is in the solution");
                    lastSpot = a[i];
                    print(a[i].Parent.Position());
                    solution.Remove(nodeAtIntersection);
                    if ((a[i].Parent.X == nodeAtIntersection.X) && (a[i].Parent.Y == nodeAtIntersection.Y))
                    {
                        print("My parent is the root");
                    }
                    else
                    {
                        print("this node and it's parents must be destroyed!");
                        //solution.Remove(nodeAtIntersection);
                        DealWithAllMessedUpNodes(lastSpot, theRoot);
                    }

                    //return lastSpot;
                }
            }
        }
        //Left
        print("X: " + x + "\nY: " + y);
        if (theMaze[x - 1, y] == 1)
        {
            print("The maze won't allow it" + " X: " + x + " Y: " + y);
        }
        else
        {
            print("I can go Left ");
            for (int i = 0; i < a.Length; i++)
            {
                //Is the node in this spot in the solution?


                if (((nodeAtIntersection.X - 1) == a[i].X && (nodeAtIntersection.Y) == a[i].Y))
                {
                    print("The left node is in the solution");
                    lastSpot = a[i];
                    print(a[i].Parent.Position());
                    solution.Remove(nodeAtIntersection);
                    if ((a[i].Parent.X == nodeAtIntersection.X) && (a[i].Parent.Y == nodeAtIntersection.Y))
                    {
                        print("My parent is the root");
                    }
                    else
                    {
                        print("this node and it's parents must be destroyed!");
                        //solution.Remove(nodeAtIntersection);
                        DealWithAllMessedUpNodes(lastSpot, theRoot);
                    }

                    //return lastSpot;
                }
            }
        }
        //Right
        print("X: " + x + "\nY: " + y);
        if (theMaze[x + 1, y] == 1)
        {
            print("The maze won't allow it" + " X: " + x + " Y: " + y);
        }
        else
        {
            print("I can go Right ");
            for (int i = 0; i < a.Length; i++)
            {
                //Is the node in this spot in the solution?


                if (((nodeAtIntersection.X + 1) == a[i].X && (nodeAtIntersection.Y) == a[i].Y))
                {
                    print("The right node is in the solution");
                    lastSpot = a[i];
                    //print(a[i].Parent.Position());
                    solution.Remove(nodeAtIntersection);
                    if ((a[i].Parent.X == nodeAtIntersection.X) && (a[i].Parent.Y == nodeAtIntersection.Y))
                    {
                        print("My parent is the root");
                    }
                    else
                    {
                        print("this node and it's parents must be destroyed!");
                        //solution.Remove(nodeAtIntersection);
                        DealWithAllMessedUpNodes(lastSpot, theRoot);
                    }

                    //return lastSpot;
                }
            }
        }
        return lastSpot;
    }
    void RemoveAllNodesUntilNull(Node thisNode)
    {
        // print("thisNode has a parent of: " + thisNode.Parent +" \nthisNode's parent's parent is: " + thisNode.Parent.Parent);
        Node [] a;
        a = solution.ToArray();
        for(int i = 0; i<a.Length; i++)
        {
            if (a[i].Parent != null)
            {
                print("I'm not removing the root but this node has to go! \nPosition: " + a[i].Position());
                GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                box.gameObject.GetComponent<Renderer>().material.color = Color.blue;
                box.transform.position = new Vector3(a[i].X, 0, a[i].Y);
                box.transform.localScale = new Vector3(.5f, .5f, .5f);
                solution.Remove(a[i]);
                messUpNodes.Enqueue(a[i]);
                Closed.Add(a[i]);
                //if(a[i].Parent.Parent != null)

            }
            else
            {
                print("I don't have a parent (like batman...) Pos: " + a[i].Position());

                //solution.Remove(node);
            }
        }
        Node leaf = null;
        leaf = (Node)messUpNodes.Dequeue();
        //solution.Add(leaf);
        //PlaceNode(new Vector3(leaf.X, 0, leaf.Y), leaf);
        print("qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqFirst thing in the Queue was: " + leaf.Position());
        /* Bug bug bug bug etc.
        foreach(Node node in solution)
        {
            if(node.Parent != null)
            {
                print("I'm not removing the root but this node has to go! \nPosition: "  + node.Position());
                solution.Remove(node);
            }
            else
            {
                print("I don't have a parent (like batman...) Pos: " + node.Position());
                
                //solution.Remove(node);
            }
        }
        */
    }
    IEnumerator WaitOneSecond(Node newPathNode)
    {
        yield return new WaitForSeconds(1);
        
        StartAStar(newPathNode, playerPos.GridPos);
    }
    void TraverseThroughAllNodes(Node currentNode)
    {
        //What's the node i'm coming from?
        Node[] a;
        Node least = null;
        a = Open.ToArray();

        if (a.Length > 1)
        {
            for (int i = 0; i < a.Length - 1; i++)
            {
                //Debug.Log("Current Node's value: " + a[i].F);

                //print("Comparing! " + a[i].F + " and the next: " + a[i + 1].F);
                if ((a[i].F < a[i + 1].F))
                {
                    if (least != null)
                    {
                        if (a[i].F < least.F)
                            least = a[i];
                    }
                    else
                    {
                        least = a[i];
                    }
                }
                else
                {
                    least = a[i + 1];
                }
                if (a[i].F == a[i + 1].F)
                {
                    print("They're equal so I'm just picking the first");
                    least = a[i]; //doesn't really matter, pick one
                }

            }
        }
        else
        {
            print("I'm in a corner, setting the value to whatever is open");
            least = a[0];
        }
        Debug.Log("The least is: " + least.F);
        //The node that we're coming from is the node with the least value. Also it will be the node that we need to put into the solution list so...
        solution.Add(least);

        Node newSpot;
        newSpot = least;
        //newSpot.Parent = currentNode;
        print("My New SpotX: " + newSpot.X + "My New SpotY: " + newSpot.Y);
        print("My New Spot's Parent, X: " + newSpot.Parent.X + "My New SpotY: " + newSpot.Parent.Y);
        //iterations++;
        StartAStar(newSpot, playerPos.GridPos);
        #region Do it twice
        /*
        if (iterations == 2)
        {
            return;
        }
        else
        {
            Node newSpot;
            newSpot = least;
            print("My New SpotX: " + newSpot.X + "My New SpotY: " + newSpot.Y);
            iterations++;
            StartAStar(newSpot, playerPos.GridPos);
            
        }
        */
        #endregion

        //Open.Add(least);
    }
    void DrawLinePointToPoint()
    {
        Node[] a;
        a = solution.ToArray();
        if (a.Length >= 1)
        {
            for (int i = 0; i < a.Length - 1; i++)
            {
                Debug.DrawLine((new Vector3(a[i].X, 0, a[i].Y)), (new Vector3(a[i + 1].X, 0, a[i + 1].Y)), Color.blue);
            }
        }
    }

    void DrawMyLine()
    {
        Node[] a;
        a = parentNodes.ToArray();
        if (a.Length >= 1)
        {
            for (int i = 0; i < a.Length-1; i++)
            {
                
                Debug.DrawLine((new Vector3(a[i].X, 0, a[i].Y)), (new Vector3(a[i+1].X, 0, a[i+1].Y)), Color.red);
                
            }
        }
    }
    void PlaceNode(Vector3 Position, Node CurrentNode)
    {
        //Put all neighbors in the open list then pick the least for F

        //Make the parent null for now. Resolve it later
        Open.Add(CurrentNode);
        //Next Step:
        //Calculate all cooresponding values for this node!
        CalculateAllValuesForNode(CurrentNode);
        print("Heres the data");
        Debug.Log(CurrentNode.Cost());

    }

    void CalculateAllValuesForNode(Node thisNode)
    {
        thisNode.G = Mathf.Abs(Mathf.Abs(endNode.X - thisNode.X) + (endNode.Y - thisNode.Y));
        //print("First node: " + firstNode.Position() + " currentNode: " + thisNode.Position());
        print("Subtracting: " + endNode.X + " - " + thisNode.X + " + " + endNode.Y + " - " + thisNode.Y);
        //print("should have: " + ());
        thisNode.H = Mathf.Sqrt(Mathf.Pow(endNode.X - thisNode.X, 2) + Mathf.Pow(endNode.Y - thisNode.Y, 2));

        thisNode.F = thisNode.G + thisNode.H;
    }

    public float GValue(Node startNode, Node currentPos)
    {
        //Use The Cartesian Plane Distance Formula
        //((x2-x1)^2 + (y2-y1)^2)^1/2
        return Mathf.Sqrt(Mathf.Pow(currentPos.X - startNode.X, 2) + Mathf.Pow(currentPos.Y - startNode.Y, 2));
    }
    public float HValue(Node currentPos, Node finishNode)
    {
        //Use The Cartesian Plane Distance Formula
        //((x2-x1)^2 + (y2-y1)^2)^1/2
        return Mathf.Sqrt(Mathf.Pow(finishNode.X - currentPos.X, 2) + Mathf.Pow(finishNode.Y - currentPos.Y, 2));
    }

    #region Check Grid To See All The Directions I Can Go
    bool CanGoUp(Node pos)
    {
        x = pos.X;
        y = pos.Y;
        print("X: " + x + "\nY: " + y);
        if (theMaze[x, y + 1] == 1)
        {
            print("The maze won't allow it" + " X: " + x + " Y: " + y);
            return false;
        }
        else if ((BeenToThisCell(new Node(x, y + 1, null)) || (BeenToThisCellOnClosedList(new Node(x, y + 1, null)))))
        {
            print("I've been there before!");
            return false;
        }
        else
        {
            print("I haven't traversed here before as proof,: " + PrintOutSolutionSet());
            return true;
        }
    }
    bool CanGoDown(Node pos)
    {
        x = pos.X;
        y = pos.Y;
        print("X: " + x + "\nY: " + y);
        if (theMaze[x, y - 1] == 1)
        {
            print("The maze won't allow it" + " X: "+ x + " Y: " + y );
            return false;
        }
        else if ((BeenToThisCell(new Node(x, y - 1, null)) || (BeenToThisCellOnClosedList(new Node(x, y - 1, null)))))
        {
            print("I've been there before!");
            return false;
        }
        else
        {
            print("I haven't traversed here before as proof,: " + PrintOutSolutionSet());
            return true;
        }
    }
    bool CanGoLeft(Node pos)
    {
        x = pos.X;
        y = pos.Y;
        print("X: " + x + "\nY: " + y);
        if (theMaze[x - 1, y] == 1)
        {
            print("The maze won't allow it" + " X: " + x + " Y: " + y);
            return false;
        }
        else if ((BeenToThisCell(new Node(x-1, y, null)) || (BeenToThisCellOnClosedList(new Node(x - 1, y, null)))))
        {
            print("I've been there before!");
            return false;
        }
        else
        {
            print("I haven't traversed here before as proof,: " + PrintOutSolutionSet());
            return true;
        }
    }
    bool CanGoRight(Node pos)
    {
        x = pos.X;
        y = pos.Y;
        print("X: " + x + "\nY: " + y);
        if (theMaze[x + 1, y] == 1)
        {
            print("The maze won't allow it" + " X: " + x + " Y: " + y);
            return false;
        }
        else if ((BeenToThisCell(new Node(x+1, y, null)) || (BeenToThisCellOnClosedList(new Node(x + 1, y, null)))))
        {
            print("I've been there before!");
            return false;
        }
        else
        {
            print("I haven't traversed here before as proof,: " + PrintOutSolutionSet());

            return true;
        }
    }
    bool BeenToThisCell(Node currentCell)
    {
        Node[] a;
        Node[] b;
        a = solution.ToArray();
        b = Closed.ToArray();


        if (a.Length >= 1)
        {
            for (int i = 0; i < a.Length; i++)
            {
                //print("Current cell X: " + currentCell.X + " solution X: " + a[i].X + "\nCurrent cell Y: " + currentCell.Y + " solution Y: " + a[i].Y);
                if (currentCell.X == a[i].X && currentCell.Y == a[i].Y)
                {
                    print("On solution List!");
                    return true;
                }
            }
            print("this is not in the solution list");
        }
        return false;
    }
    bool BeenToThisCellOnClosedList(Node currentCell)
    {
        Node[] b;
        b = Closed.ToArray();
        print("Do I exist in closed? " + Closed.Contains(currentCell) + "this cell's position: " + currentCell.Position());


        if (b.Length >= 1)
        {
            for (int i = 0; i < b.Length; i++)
            {
                //print("Current cell X: " + currentCell.X + " solution X: " + b[i].X + "\nCurrent cell Y: " + currentCell.Y + " solution Y: " + b[i].Y);
                if (currentCell.X == b[i].X && currentCell.Y == b[i].Y)
                {
                    print("On Closed List! ");
                    return true;
                }
            }
            print("Not in the closed list");
        }
        return false;
    }
    string PrintOutSolutionSet()
    {
        Node[] a;
        a = solution.ToArray();
        string allValues = null;
        if (a.Length >= 1)
        {
            for (int i = 0; i < a.Length; i++)
            {
                allValues += a[i].Position() + " | ";
            }
        }
        return allValues;
    }
    #endregion
    #region Created by Derreck
    //Created by Derreck
    #endregion
    void MoveToCell(Node someNode)
    {
        //transform.position = Vector3.MoveTowards(transform.position, new Vector3(x, 0, y), speed * Time.deltaTime);
        print("going to: " + someNode.Position());
        //int num = 0;
        //bool arrived = false;
        
            
        
        
        //print("arrived");
        //transform.position = newSpot;
    }
}
