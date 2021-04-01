using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentObject : MonoBehaviour
{
    //Objects not instantiated yet
    private GameObject cameraTransform;
    private GridMap newMap;
    private DroneBehaviour drone;
    private TextMesh[,] currMap;

    //Size width and height for the grid
    private int sizew = 10, sizeh = 10;

    //Local starting position for a new drone
    private int[] startLocal = new int[] { 0, 0 };
    private int[] droneLastLocal;

    //Boolean to track if the current drone is dead
    public bool droneIsDead = true;

    //Counter for the amount of drones 
    private int amountOfDrones = 0;


    /*  ######!-START-!######
        Runs before the first frame in Unity
    */
    /// <summary>
    /// We initiate the camera, set a new gridmap and set the drone last location to the start location
    /// </summary>
    void Start()
    {
        cameraTransform = GameObject.FindGameObjectWithTag("simMainCamera");

        newMap = new GridMap(sizew, sizeh, sizeh);
        currMap = newMap.getMap();

        cameraTransform.transform.position = new Vector3((sizew * sizeh) / 2, (sizew * sizeh) / 2, -10);

        droneLastLocal = startLocal;
    }


    /*  ######!-SENDING OUT A NEW DRONE-!######
        The process of sending out a new drone
    */
    /// <summary>
    /// We check the last location of the drone by getting it's death location, if the last location is locked in with C or O then we find a new random location to start with.
    /// Once that is done, we just set the drone to dead once it returns the new textmesh alongside the path.
    /// </summary>
    bool SendOutDrone()
    {
        bool[] canMove;

        if (drone)
        {
            drone = null;
        }

        drone = new DroneBehaviour(startLocal, currMap);
        currMap = drone.goSideToSide(currMap, out droneLastLocal, out canMove);

        for(int i = 0; i < canMove.Length; i++){

            if(canMove[i] == true){

                Debug.Log(i);

                if (currMap[DroneUtils.returnPath(i, droneLastLocal)[0], DroneUtils.returnPath(i, droneLastLocal)[1]].text != "C"
                    && currMap[DroneUtils.returnPath(i, droneLastLocal)[0], DroneUtils.returnPath(i, droneLastLocal)[1]].text != "O")
                {

                    startLocal[0] = DroneUtils.returnPath(i, droneLastLocal)[0];
                    startLocal[1] = DroneUtils.returnPath(i, droneLastLocal)[1];
                    return true;

                }

            }

        }

        //Otherwise find new starting location
        if (currMap[startLocal[0], startLocal[1]].text == "C" || currMap[startLocal[0], startLocal[1]].text == "CM")
        {
            for (int x = 0; x < currMap.GetLength(0); x++)
            {
                for (int y = 0; y < currMap.GetLength(1); y++)
                {

                    if (currMap[x, y].text == "NC" || currMap[x, y].text == "M") //We can include M because those are unknown to the parent object
                    {

                        startLocal[0] = x;
                        startLocal[1] = y;
                        //Debug.Log(currMap[x, y].text);
                        return true;

                    }

                }
            }
        }

        return true;
    }


    /*  ######!-UPDATE UNITY-!######
        Each frame action in Unity
    */
    /// <summary>
    /// If the drone is dead, send out a new drone after 5 seconds.
    /// </summary>
    void Update()
    {

        if (droneIsDead)
        {
            droneIsDead = false;
            amountOfDrones++;
            Debug.Log(amountOfDrones);
            Invoke("initiateDroneProcedure", 2);
        }

    }

    void initiateDroneProcedure(){
        droneIsDead = SendOutDrone();
    }
}
