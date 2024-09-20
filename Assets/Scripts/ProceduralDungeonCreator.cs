using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProceduralDungeonCreator : MonoBehaviour
{
    [Tooltip("Length along the x axis")]
    [SerializeField] int length; //x

    [Tooltip("Length along the z axis")]
    [SerializeField] int width;  //z

    [Space]

    [SerializeField] int totalRooms;
    [SerializeField] int minimumSizeRoom;

    [Space]
    [Space]

    [SerializeField] GameObject startingRoom; //can place out of grid

    [Tooltip("Ensure the 'general' rooms are within the minimumSizeRoom bounds.")]
    [SerializeField] List<GameObject> generalRooms;

    [Tooltip("Rooms found at end paths within the MST. This could also include the end room.")]
    [SerializeField] List<GameObject> specialRooms;

    [Space]

    [SerializeField] List<GameObject> currentRooms;

    void Start()
    {
        GetComponent<BSPGenerator>().CreatePartitions(width, length, totalRooms, minimumSizeRoom);

        //GetComponent<DelaunayGraphGenerator>().GenerateDelaunayTriangulationGraph();  
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(new Vector3(width / 2, -2, length / 2), new Vector3(width, 1, length));

        /*
        for (int i = 0; i < roomPositions.Count; i++) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(new Vector3(roomPositions[i].x, 0, roomPositions[i].y), 2);
        }
        */
    }
}

public class SpecialRoom
{
    public GameObject room;
    public float probability;
}
