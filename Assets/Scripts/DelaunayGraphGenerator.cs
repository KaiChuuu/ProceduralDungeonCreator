using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelaunayGraphGenerator : MonoBehaviour
{
    public void GenerateDelaunayTriangulationGraph()
    {
        List<BSPNode> partitions = new List<BSPNode>();
        //retrievePartitions(ref partitions, root);
    }

    void retrievePartitions(ref List<BSPNode> positions, BSPNode node)
    {
        if (node.left == null && node.right == null)
        {
            positions.Add(node);
            return;
        }

        if (node.left != null)
        {
            retrievePartitions(ref positions, node.left);
        }
        if (node.right != null)
        {
            retrievePartitions(ref positions, node.right);
        }
    }
}
