using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransitionLogic : MonoBehaviour
{
    /// <summary>
    /// Just the variables used to send the player to another area.
    /// </summary>
    public GoTo GoToRoom;
    public Direction FaceDirection;
    public Vector3 SpawnPosition;
    public Area CurrentArea;

    //Tells the player which room to be sent to.
    public enum GoTo
    {
        Default,
        Room1,
        Room2
    }
    //Tells the player which direction they should be facing when on the other side.
    public enum Direction
    {
        North,
        South,
        West,
        East,
        NorthWest,
        NorthEast,
        SouthWest,
        SouthEast
    }
    //Tells the player script which "area" it currently is in.
    public enum Area
    {
        WindWispWoods,
        ManaCrystalMines
    }
}
