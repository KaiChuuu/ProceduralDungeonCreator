using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BSPGenerator : MonoBehaviour
{
    BSPNode root;

    public void CreatePartitions(int width, int length, int totalRooms, int minimumSizeRoom)
    {
        //Create Partitions
        root = new BSPNode(width, length, 0, 0, 'x');
        int remainingRooms = PartitionGrid(totalRooms, minimumSizeRoom);

        if(remainingRooms > 0)
        {
            Debug.Log("Couldnt generate total requested rooms within parameters. Remaining rooms, " + remainingRooms);
        } 
    }

    /*
     * Randomly creates partitions within the space.
     * 
     * Builds a BSP (Binary Space Partitioning) through BFS traversal
     * 
     * BSP:
     *  - Creates valid partitions
     *  - Stores partitions in tree
     * BFS:
     *  - Ensures larger partitions are split
     */
    int PartitionGrid(int totalRooms, int minimumSizeRoom)
    {
        Queue<BSPNode> partitions = new Queue<BSPNode>();
        partitions.Enqueue(root);
        totalRooms--;

        while(totalRooms > 0 && partitions.Count > 0)
        { 
            BSPNode current = partitions.Dequeue();

            int width = (int)current.bounds.width;
            int length = (int)current.bounds.height;

            if(width  < minimumSizeRoom || length < minimumSizeRoom) {
                continue;
            }

            int cut;
            BSPNode leftPartition;
            BSPNode rightPartition;

            //Flip between a horizontal x or vertical z cut
            if (current.direction == 'x')
            {
                int minStartX = current.startX + minimumSizeRoom;
                int minEndX = current.startX + width - minimumSizeRoom;

                if (minStartX > minEndX)
                {
                    continue;
                }
                cut = Random.Range(minStartX, minEndX + 1) - current.startX;

                leftPartition = new BSPNode(cut, length, current.startX, current.startZ, 'z');
                rightPartition = new BSPNode(width - cut, length, cut + current.startX, current.startZ, 'z');
            }
            else
            {
                int minStartZ = current.startZ + minimumSizeRoom;
                int minEndZ = current.startZ + length - minimumSizeRoom;

                if (minStartZ > minEndZ)
                {
                    continue;
                }
                cut = Random.Range(minStartZ, minEndZ + 1) - current.startZ;

                leftPartition = new BSPNode(width, length - cut, current.startX, cut + current.startZ, 'x');
                rightPartition = new BSPNode(width, cut, current.startX, current.startZ, 'x');
            }

            current.left = leftPartition;
            current.right = rightPartition;

            partitions.Enqueue(leftPartition);
            partitions.Enqueue(rightPartition);

            totalRooms--;
        }

        return totalRooms;
    }
}

public class BSPNode
{
    public int startX;
    public int startZ;
    public Rect bounds;
    public BSPNode left;
    public BSPNode right;
    public char direction;

    public BSPNode(int width, int length, int startX, int startZ, char direction)
    {
        bounds.width = width;
        bounds.height = length;
        this.startX = startX;
        this.startZ = startZ;
        this.direction = direction;
    }
}
