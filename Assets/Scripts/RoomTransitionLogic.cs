using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransitionLogic : MonoBehaviour
{
    public enum GoTo
    {
        Default,
        Room1,
        Room2
    }

    public GoTo GoToRoom;
    public Direction FaceDirection;
    public Vector3 SpawnPosition;


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
}
