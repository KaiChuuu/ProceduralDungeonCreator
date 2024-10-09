using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 Referenced bowyer-watson algorithm steps from blog,
 https://www.gorillasun.de/blog/bowyer-watson-algorithm-for-delaunay-triangulation/
 */
public class DelaunayGraphGenerator : MonoBehaviour
{
    List<Triangle> triangles;
    Triangle superTriangle;

    public List<Triangle> GenerateDelaunayTriangulationGraph(List<Vertex> points)
    {
        //Create triangle encapsulating all points
        superTriangle = CreateSuperTriangle(points);

        triangles = new List<Triangle>() { superTriangle };
        
        //Add each points to the Triangle list, connecting edges to the super triangle and other points
        
        foreach(Vertex point in points)
        {
            AddPoint(point);
        }
        
        //Remove the outer super triangle
        RemoveSuperTriangle(superTriangle);

        return triangles;
    }

    /*
     Display Delaunay Graph
     */
    /*
    private void OnDrawGizmos()
    {
        if(superTriangle != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(superTriangle.v1.x, 3, superTriangle.v1.y), new Vector3(superTriangle.v2.x, 3, superTriangle.v2.y));
            Gizmos.DrawLine(new Vector3(superTriangle.v2.x, 3, superTriangle.v2.y), new Vector3(superTriangle.v3.x, 3, superTriangle.v3.y));
            Gizmos.DrawLine(new Vector3(superTriangle.v3.x, 3, superTriangle.v3.y), new Vector3(superTriangle.v1.x, 3, superTriangle.v1.y));
        }

        Gizmos.color = Color.green;

        if (triangles != null)
        {
          
            foreach (Triangle triangle in triangles)
            {
                Gizmos.DrawLine(new Vector3(triangle.v1.x, 2, triangle.v1.y), new Vector3(triangle.v2.x, 2, triangle.v2.y));
                Gizmos.DrawLine(new Vector3(triangle.v2.x, 2, triangle.v2.y), new Vector3(triangle.v3.x, 2, triangle.v3.y));
                Gizmos.DrawLine(new Vector3(triangle.v3.x, 2, triangle.v3.y), new Vector3(triangle.v1.x, 2, triangle.v1.y));
            }
        }
    }
    */

    void AddPoint(Vertex point)
    {
        List<Edge> edges =  new List<Edge>();

        triangles = triangles.Where(triangle =>
        {
            if (triangle.InCircumCircle(point))
            {
                edges.Add(new Edge(triangle.v1, triangle.v2));
                edges.Add(new Edge(triangle.v2, triangle.v3));
                edges.Add(new Edge(triangle.v3, triangle.v1));
                return false;
            }
            return true;
        }).ToList();

        edges = UniqueEdges(edges);

        foreach(Edge edge in edges)
        {
            triangles.Add(new Triangle(edge.v1, edge.v2, point));
        }
    }

    Triangle CreateSuperTriangle(List<Vertex> vertices)
    {
        //Find the bounding box
        float minX = vertices.Min(v => v.x);
        float minY = vertices.Min(v => v.y);
        float maxX = vertices.Max(v => v.x);
        float maxY = vertices.Max(v => v.y);

        //Define a large enough margin
        float dx = maxX - minX;
        float dy = maxY - minY;
        float deltaMax = Mathf.Max(dx, dy) * 10;

        //Create super triangle vertices
        Vertex v1 = new Vertex(minX - deltaMax, minY - deltaMax);
        Vertex v2 = new Vertex(minX + 2 * deltaMax, minY + deltaMax);
        Vertex v3 = new Vertex(minX - deltaMax, minY + 2 * deltaMax);

        return new Triangle(v1, v2, v3);
    }

    List<Edge> UniqueEdges(List<Edge> edges)
    {
        List<Edge> uniqueEdges = new List<Edge>();
        for(int i=0; i<edges.Count; i++)
        {
            bool isUnique = true;

            for(int j=0; j<edges.Count; j++)
            {
                if(i != j && edges[i].Equals(edges[j]))
                {
                    isUnique = false;
                    break;
                }
            }

            if (isUnique)
            {
                uniqueEdges.Add(edges[i]);
            }
        }

        return uniqueEdges;
    }

    void RemoveSuperTriangle(Triangle superTriangle)
    {
        triangles.RemoveAll(triangle => triangle.ContainsVertex(superTriangle.v1) ||
                                        triangle.ContainsVertex(superTriangle.v2) || 
                                        triangle.ContainsVertex(superTriangle.v3));
    }
}

public class Vertex
{
    public float x;
    public float y;

    public Vertex(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Vertex))
        {
            return false;
        }

        Vertex v = (Vertex)obj;
        return this.x == v.x && this.y == v.y;
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode();
    }
}

public class Edge
{
    public Vertex v1;
    public Vertex v2;
    public float weight = 0;

    public Edge(Vertex v1, Vertex v2)
    {
        this.v1 = v1;
        this.v2 = v2;

        //Generate Edge weights based on Euclidean Distance, distance between two points.
        weight = Mathf.Sqrt(Mathf.Pow(v2.x - v1.x, 2) + Mathf.Pow(v2.y - v1.y, 2));
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Edge))
        {
            return false;
        }
           
        Edge other = (Edge) obj;
        return (this.v1.Equals(other.v1) && this.v2.Equals(other.v2)) ||
            (this.v1.Equals(other.v2) && this.v2.Equals(other.v1));
    }

    public override int GetHashCode()
    {
        int hash1 = v1.GetHashCode();
        int hash2 = v2.GetHashCode();

        return hash1 ^ hash2;
    }
}

public class Triangle
{
    public Vertex v1;
    public Vertex v2;
    public Vertex v3;
    public float circumcenterX;
    public float circumcenterY;
    public float circumRadius;

    public Triangle(Vertex v1, Vertex v2, Vertex v3)
    {
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;

        CalulateCircumcenter();
        CalculateCircumCircleRadius();
    }

    public void CalulateCircumcenter()
    {
        //Midpoint of the edges (for 2 sides)
        float midX1 = (v1.x + v2.x) / 2;
        float midY1 = (v1.y + v2.y) / 2;

        float midX2 = (v2.x + v3.x) / 2;
        float midY2 = (v2.y + v3.y) / 2;

        //Slope of the lines 
        float slope1 = (v2.y - v1.y) / (v2.x - v1.x);
        float slope2 = (v3.y - v2.y) / (v3.x - v2.x);

        //Slope for perpendicular bisectors
        float perpSlope1 = -1 / slope1;
        float perpSlope2 = -1 / slope2;

        //Intersection of two lines
        circumcenterX = ((perpSlope1 * midX1 - midY1) - (perpSlope2 * midX2 - midY2)) / (perpSlope1 - perpSlope2);
        circumcenterY = perpSlope1 * (circumcenterX - midX1) + midY1;
        
    }

    public void CalculateCircumCircleRadius()
    {
        //Calculate distance from circumcenter to any given point
        float dx = circumcenterX - v1.x;
        float dy = circumcenterY - v1.y;
        circumRadius = Mathf.Sqrt(dx * dx + dy * dy);
    }

    public bool InCircumCircle(Vertex other)
    {
        float dx = circumcenterX - other.x;
        float dy = circumcenterY - other.y;
        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        return distance <= circumRadius;
    }

    public List<Edge> GetEdges()
    {
        List<Edge> edges = new List<Edge>
        {
            new Edge(v1, v2),
            new Edge(v2, v3),
            new Edge(v3, v1)
        };

        return edges;
    }

    public bool ContainsVertex(Vertex other)
    {
        return v1.Equals(other) || v2.Equals(other) || v3.Equals(other);
    }
}
