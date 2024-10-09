using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MSTGenerator : MonoBehaviour
{
    HashSet<Vertex> vertices = new HashSet<Vertex>();
    HashSet<Edge> edges = new HashSet<Edge>();
    
    List<Edge> mstEdges = new List<Edge>();

    public void GenerateMinimumSpanningTree(List<Triangle> triangles)
    {
        //Retrieve edges and vertices from Delaunay Triangulation Graph
        PopulateGraph(triangles);

        PrimsAlgorithm();
    }

    void PopulateGraph(List<Triangle> triangles)
    {
        if (triangles != null)
        {
            foreach (Triangle triangle in triangles)
            {
                vertices.Add(new Vertex(triangle.v1.x, triangle.v1.y));
                vertices.Add(new Vertex(triangle.v2.x, triangle.v3.y));
                vertices.Add(new Vertex(triangle.v3.x, triangle.v1.y));

                edges.Add(new Edge(triangle.v1, triangle.v2));        
                edges.Add(new Edge(triangle.v2, triangle.v3));    
                edges.Add(new Edge(triangle.v3, triangle.v1));
            }
        }
    }

    /*
     View MST tree over Delaunay Graph
     */
    private void OnDrawGizmos()
    {
        if (edges != null)
        {
            Gizmos.color = Color.green;
            foreach (Edge edge in edges)
            {
                Gizmos.DrawLine(new Vector3(edge.v1.x, 2, edge.v1.y), new Vector3(edge.v2.x, 2, edge.v2.y));
            }

            Gizmos.color = Color.red;
            foreach (Edge edge in mstEdges)
            {
                Gizmos.DrawLine(new Vector3(edge.v1.x, 5, edge.v1.y), new Vector3(edge.v2.x, 5, edge.v2.y));
            }
        }
    }

    void PrimsAlgorithm()
    {
        HashSet<Vertex> visited = new HashSet<Vertex>();
        SortedSet<Edge> destinations = new SortedSet<Edge>(new EdgeComparer());

        Vertex start = vertices.First();
        visited.Add(start);
        AddDestinations(ref visited, ref destinations, start);

        /*
        Debug.Log(start.x + " " + start.y + " : Start");
        Debug.Log(visited.Count + " : Visited");
        Debug.Log(destinations.Count + " : Destinations");
        Debug.Log(edges.Count + " : Edges");
        */

        while (destinations.Count > 0)
        {    
            var destination = destinations.Min;
            destinations.Remove(destination);

            if (visited.Contains(destination.v1) && visited.Contains(destination.v2))
            {
                continue;
            }

            mstEdges.Add(destination);

            Vertex destinationVertex;
            destinationVertex = (visited.Contains(destination.v1)) ? destination.v2 : destination.v1;
            visited.Add(destinationVertex);
            

            AddDestinations(ref visited, ref destinations, destinationVertex);
        }
    }

    void AddDestinations(ref HashSet<Vertex> visited, ref SortedSet<Edge> destinations, Vertex current)
    {
        foreach(Edge edge in edges)
        {
            if(edge.v1.Equals(current) && !visited.Contains(edge.v2))
            {
                destinations.Add(edge);
            }
            else if(edge.v2.Equals(current) && !visited.Contains(edge.v1))
            {
                destinations.Add(edge);
            }
        }
    }
}

class EdgeComparer : IComparer<Edge>
{
    public int Compare(Edge dest1, Edge dest2)
    {
        if (dest1 == null || dest2 == null) return 0;
        
        if (dest1.weight < dest2.weight) return -1;
        if (dest1.weight == dest2.weight) return 0;

        return 1;
    }
}
