using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneUtils : MonoBehaviour
{

    
    /*  ######!-SET A TEXTMESH IN UNITY-!######
        Set some text mesh in unity with all the properties set
    */
    /// <summary>
    /// Take in the parent transform, local position vector (X, Y, Z), text to be used, fontsize for such text, color, and the sorting order.
    /// Once this is done we just set all the parameters to the components of the created textmesh.
    /// </summary>
    public static int[] returnPath(int moveInto, int[] currPosition)
    {
        int[] moveIntoPath = new int[] { 0, 0 };

        switch (moveInto)
        {

            case 0: //Right
                moveIntoPath[0] = currPosition[0] + 1;
                moveIntoPath[1] = currPosition[1];
                break;
            case 1: //Up 
                moveIntoPath[0] = currPosition[0];
                moveIntoPath[1] = currPosition[1] + 1;
                break;

            case 2: //Bottom
                moveIntoPath[0] = currPosition[0];
                moveIntoPath[1] = currPosition[1] - 1;
                break;

            case 3: //Left 
                moveIntoPath[0] = currPosition[0] - 1;
                moveIntoPath[1] = currPosition[1];
                break;

            default:
                break;
        }

        return moveIntoPath;
    }


    /*  ######!-SET A TEXTMESH IN UNITY-!######
        Set some text mesh in unity with all the properties set
    */
    /// <summary>
    /// Take in the parent transform, local position vector (X, Y, Z), text to be used, fontsize for such text, color, and the sorting order.
    /// Once this is done we just set all the parameters to the components of the created textmesh.
    /// </summary>
    public static bool[] cleanBooleanSingle(int index, bool[] squareCheck)
    {

        for (int i = 0; i < squareCheck.Length; i++)
        {

            squareCheck[i] = false;

            if (i == index)
            {
                squareCheck[index] = true;
            }

        }

        return squareCheck;
    }
}
