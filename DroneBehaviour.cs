using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBehaviour : MonoBehaviour
{
    //Textmesh type to follow this map 
    private TextMesh[,] mapToFollow;

    //Starting position and current position
    private int[] startingLocal, currPosition;

    //Booleans to store different movement parameters
    //Equivalent to [Right, Up, Bottom, Left]
    bool[] recMove = new bool[] { false, false, false, false };
    bool[] canMove = new bool[] { false, false, false, false };

    //Resistance to be giving to some cell
    private int resistanceToGive = 1;

    public DroneBehaviour(int[] startingLocal, TextMesh[,] mapToFollow)
    {
        this.startingLocal = startingLocal;
        this.mapToFollow = mapToFollow;
        this.currPosition = this.startingLocal;
    }


    /*  ######!-ALTERNATE GRASSFIRE ALGORITHM-!######
        This method is a "main" method for the actual alternate grassfire algorithm. 
    */
    /// <summary>
    /// It will call on methods: cellCurrState(), counterClockChoose(), getResistanceOnCell, chooseNextPos(), and moveDrone().
    /// All these methods happen in a loop on the area of the actual map; We avoid the drone idling because of the finite loop.
    /// </summary>
    public TextMesh[,] goSideToSide(TextMesh[,] mapToFollow, out int[] droneLastLocal, out bool[] canMoveO)
    {

        int[,] resistanceMap = getResistanceMap();
        string nextPos;

        //Loop till the area is overstepped of the whole graph (This is a deterrent to the drone going idle)
        for (int i = 0; i < this.mapToFollow.GetLength(0) * this.mapToFollow.GetLength(1); i++)
        {
            //If the current cell state is a mine, this method should return true, we can just break
            if (cellCurrState()) break;

            //Check states
            setCanMove();

            //Set adjacent resistance cells 
            counterClockChoose(currPosition);

            //Set resistance 
            getResistanceOnCell(resistanceMap);

            //Choose a cell of lowest resistance to go to & go to it 
            nextPos = chooseNextPos(resistanceMap);
            moveDrone(currPosition, nextPos);

        }

        foreach(bool a in this.canMove){
            Debug.Log(a);
        }

        setCanMove();

        //Return the edited map to the parent object
        canMoveO = this.canMove;
        droneLastLocal = this.currPosition;
        return this.mapToFollow;
    }


    /*  ######!-WHICH CELLS ARE FREE-!######
        Check which cells I can move into this turn 
    */
    /// <summary>
    /// Checks through if statements right, top, bottom, left cells if they're avaliable; 
    /// If they are, set a correlated cell in a boolean to true.
    /// </summary>
    private void whichICanMoveInto()
    {

        //Check right square 
        if (this.currPosition[0] + 1 < this.mapToFollow.GetLength(0))
        {
            //Debug.Log("Curr position" + this.currPosition[0] + 1 + "Map length" + this.mapToFollow.GetLength(0));
            this.canMove[0] = true;
        }

        //Check top square
        if (this.currPosition[1] + 1 < this.mapToFollow.GetLength(1))
        {
            this.canMove[1] = true;
        }

        //Check bottom square
        if (currPosition[1] - 1 > -1)
        {
            this.canMove[2] = true;
        }

        //Check left square
        if (currPosition[0] - 1 > -1)
        {
            this.canMove[3] = true;
        }

    }


    /*  ######!-CHANGE CELL STATUS-!######
        This method changes the cell state; Updates the TextMesh multidimensional array.
    */
    /// <summary>
    /// Through if statements, a mine would be set as CM (Checked Mine), while anything else just turns into C.
    /// Returns a true of false for aborting the main loop.
    /// </summary>
    private bool cellCurrState()
    {

        //Current should be checked
        if (this.mapToFollow[currPosition[0], currPosition[1]].text == "M")
        {
            this.mapToFollow[currPosition[0], currPosition[1]].text = "CM";
            this.mapToFollow[currPosition[0], currPosition[1]].color = Color.blue;
            return true;
        }
        else if (this.mapToFollow[currPosition[0], currPosition[1]].text != "M" && this.mapToFollow[currPosition[0], currPosition[1]].text != "CM"
                && this.mapToFollow[currPosition[0], currPosition[1]].text != "O")
        {
            this.mapToFollow[currPosition[0], currPosition[1]].text = "C";
            this.mapToFollow[currPosition[0], currPosition[1]].color = Color.yellow;
        }

        return false;

    }


    /*  ######!-SET PREFERRED DIRECTION-!######
        In a counter clockwise movement plan, choose the best preferred path. 
    */
    /// <summary>
    /// We have the distances from all sides to my current position. 
    /// Depending on which side you're closer to, using the distances, we can determine the prefferred path to take.
    /// </summary>
    private void counterClockChoose(int[] position)
    {

        //Used to then make all the other items false 
        int boolFix = 0;

        //Get distances from each side
        int distL = currPosition[0], distR = mapToFollow.GetLength(0) - currPosition[0], distU = mapToFollow.GetLength(1) - currPosition[1], distD = currPosition[1];

        //Bottom left corner
        if (distL < distR && distD < distU)
        {
            this.recMove[0] = true; //Move Right
            boolFix = 0;
        }//Bottom right corner
        else if (distL > distR && distD < distU)
        {
            this.recMove[1] = true; //Move Up
            boolFix = 1;
        }//Top left corner
        else if (distL < distR && distD > distU)
        {
            this.recMove[2] = true; //Move Down
            boolFix = 2;
        }//Top right corner
        else if (distL > distR && distD > distU)
        {
            this.recMove[3] = true; //Move Left
            boolFix = 3;
        }

        //Make sure array is clean
        this.recMove = DroneUtils.cleanBooleanSingle(boolFix, recMove);

    }


    /*  ######!-SET THE RESISTANCE OF A CELL-!######
        This method updates the resistance map for the drone. 
    */
    /// <summary>
    /// Going through a for loop, if canMove is true, take the correlated index and lets check the resistance.
    /// If it's a cell we can move into, we pass i into the rule check to indicate which path we're taking.
    /// </summary>
    private void getResistanceOnCell(int[,] resistanceMap)
    {

        int i = 0;
        foreach (bool a in canMove)
        {

            resistanceToGive = 1;

            if (a.Equals(true))
            {
                resistanceMap[DroneUtils.returnPath(i, currPosition)[0], DroneUtils.returnPath(i, currPosition)[1]] = resistanceRuleCheck(i);
            }
            i++;

        }
    }


    /*  ######!-RESISTANCE RULE CHECK-!######
        This does a rule check to see what the resistance should be of a cell. 
    */
    /// <summary>
    /// If the cell is C then add 10, if the cell is CM or O then set it to much higher, otherwise if it's in the wrong preferred location just add 1.
    /// </summary>
    private int resistanceRuleCheck(int cellCheckSpecific)
    {

        //If in another row
        if (!this.recMove[cellCheckSpecific])
        {
            resistanceToGive += 1;
        }

        if (this.mapToFollow[DroneUtils.returnPath(cellCheckSpecific, currPosition)[0], DroneUtils.returnPath(cellCheckSpecific, currPosition)[1]].text == "C")
        {
            resistanceToGive += 10;
        }
        else if (this.mapToFollow[DroneUtils.returnPath(cellCheckSpecific, currPosition)[0], DroneUtils.returnPath(cellCheckSpecific, currPosition)[1]].text == "O"
                || this.mapToFollow[DroneUtils.returnPath(cellCheckSpecific, currPosition)[0], DroneUtils.returnPath(cellCheckSpecific, currPosition)[1]].text == "CM")
        {
            resistanceToGive += 99999;
        }

        return resistanceToGive;
    }


    /*  ######!-MOVE THE DRONE-!######
        Depending on the position, just move the drone. 
    */
    /// <summary>
    /// Accepts the currentposition and then a string position, then just moves the drone.
    /// </summary>
    private void moveDrone(int[] currPos, string position)
    {

        switch (position)
        {

            case "up":
                this.currPosition[1] += 1;
                break;

            case "bottom":
                this.currPosition[1] -= 1;
                break;

            case "left":
                this.currPosition[0] -= 1;
                break;

            case "right":
                this.currPosition[0] += 1;
                break;

            default:
                //Debug.Log("Not working position");
                break;

        }
    }


    /*  ######!-CHOOSE THE BEST MOVEMENT-!######
        This method will return which position is preffered to move into 
    */
    /// <summary>
    /// By just looking through the cells you can move into and comparing the resistanceMap, we can just take the min and return the position for that.
    /// </summary>
    private string chooseNextPos(int[,] resistanceMap)
    {
        int min = 2147483647;
        string[] position = new string[] { "right", "up", "bottom", "left" };
        string rString = "";

        //Can move
        for (int i = 0; i < canMove.Length; i++)
        {

            if (canMove[i] == true)
            {

                if (min > resistanceMap[DroneUtils.returnPath(i, currPosition)[0], DroneUtils.returnPath(i, currPosition)[1]])
                {
                    min = resistanceMap[DroneUtils.returnPath(i, currPosition)[0], DroneUtils.returnPath(i, currPosition)[1]];
                    rString = position[i];
                }

            }

        }

        //Debug.Log(rString);

        return rString;
    }


    /*  ######!-CREATE THE RESISTANCE MAP-!######
        This creates the resistance map for the user
    */
    /// <summary>
    /// Create the resistance map by building it according to the text mesh.
    /// </summary>
    private int[,] getResistanceMap()
    {
        int[,] mapConv = new int[mapToFollow.GetLength(0), mapToFollow.GetLength(1)];

        for (int x = 0; x < mapToFollow.GetLength(0); x++)
        {
            for (int y = 0; y < mapToFollow.GetLength(1); y++)
            {

                if (mapToFollow[x, y].text == "NC")
                {

                    mapConv[x, y] = 0;

                }
                else if (mapToFollow[x, y].text == "O")
                {

                    mapConv[x, y] = -1;

                }
                else if (mapToFollow[x, y].text == "MC")
                {

                    mapConv[x, y] = -1;

                }

            }
        }

        return mapConv;
    }


    /*  ######!-CREATE THE RESISTANCE MAP-!######
        This just returns the current position if needed.         
    */
    /// <summary>
    /// Returns the current position
    /// </summary>
    public int[] getLocation()
    {
        return this.currPosition;
    }

    public void setCanMove(){
        //Reset canMove
        for (int c = 0; c < this.canMove.Length; c++)
        {
            this.canMove[c] = false;
        }

        whichICanMoveInto();
    }
}
