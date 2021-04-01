using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GridMap
{
    //Integer type for height and width
    private int height, width;

    //Float type for the cellsize in the grid
    private float cellSize;
    
    //Grid map integer and a textmesh following it in the same size
    private int[,] gridmap;
    private TextMesh[,] allText;


    /*  ######!-CONSTRUCTOR WHICH CREATES THE GRID-!######
        Takes in height, width, and cellsize; Creates the grid using these parameters
    */
    /// <summary>
    /// Set the gridmap and textmesh size to the height and width, then fill it in with C.
    /// Out of some random number between 0-->25 if it's 15 or 10 set it as an object (O);
    /// If between 0 and 50 you get 25 set it as a mine (M); 
    /// Otherwise just set it to not checked.
    /// </summary>
    public GridMap(int heights, int widths, int cellSize)
    {
        this.height = heights;
        this.width = widths;
        this.cellSize = cellSize;

        this.gridmap = new int[this.height, this.width];
        this.allText = new TextMesh[this.height, this.width];

        for (int x = 0; x < gridmap.GetLength(0); x++)
        {
            //Go by Column
            for (int y = 0; y < gridmap.GetLength(1); y++)
            {
                //Build upwards 
                gridmap[x, y] = 0;

                //Debug.Log(x + "," + y);
                if (Random.Range(0, 25) == 15 || Random.Range(0, 25) == 10)
                {
                    allText[x, y] = DrawUtils.DrawTextMesh(null, (new Vector3(x, y) * cellSize) + new Vector3(cellSize, cellSize) * 0.5f, "O", 20, Color.black, 0);
                }
                else if (Random.Range(0, 50) == 25)
                {
                    allText[x, y] = DrawUtils.DrawTextMesh(null, (new Vector3(x, y) * cellSize) + new Vector3(cellSize, cellSize) * 0.5f, "M", 20, Color.red, 0);
                }
                else
                {
                    allText[x, y] = DrawUtils.DrawTextMesh(null, (new Vector3(x, y) * cellSize) + new Vector3(cellSize, cellSize) * 0.5f, "NC", 20, Color.white, 0);
                }

                Debug.DrawLine(new Vector3(x, y) * cellSize, new Vector3(x, y + 1) * cellSize, Color.green, 99999f);
                Debug.DrawLine(new Vector3(x + 1, y) * cellSize, new Vector3(x, y) * cellSize, Color.green, 99999f);
            }

        }

        Debug.DrawLine(new Vector3(0, height) * cellSize, new Vector3(width, height) * cellSize, Color.green, 99999f);
        Debug.DrawLine(new Vector3(width, 0) * cellSize, new Vector3(width, height) * cellSize, Color.green, 99999f);

    }


    /*  ######!-SET A TEXTMESH IN UNITY-!######
        Set some text mesh in unity with all the properties set
    */
    /// <summary>
    /// Take in the parent transform, local position vector (X, Y, Z), text to be used, fontsize for such text, color, and the sorting order.
    /// Once this is done we just set all the parameters to the components of the created textmesh.
    /// </summary>
    public TextMesh[,] getMap()
    {
        return allText;
    }

}
