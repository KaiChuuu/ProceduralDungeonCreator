using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPGenerator : MonoBehaviour
{
    BSPNode root;
    List<Vertex> points;

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

    public List<Vertex> CreatePoints()
    {
        points = new List<Vertex>();

        if (root != null)
        {
            GetLeafPartitions(root);
        }

        return points;
    }

    //Display vertex points
    private void OnDrawGizmos()
    {
        if (points != null)
        {
            foreach(var point in points)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(new Vector3(point.x, 3, point.y), 3);
            }
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

    public void GetLeafPartitions(BSPNode node)
    {
        if (node.left == null && node.right == null)
        {
            int centerX = node.startX + (int) node.bounds.width / 2;
            int centerY = node.startZ + (int) node.bounds.height / 2;
            points.Add(new Vertex(centerX, centerY));
            return;
        }

       if(node.left != null)
       {
            GetLeafPartitions(node.left);
       }
       if(node.right != null)
       {
            GetLeafPartitions(node.right);
       }
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
