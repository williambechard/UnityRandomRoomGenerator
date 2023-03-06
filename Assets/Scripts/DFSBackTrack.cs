using System.Collections.Generic;
using UnityEngine;

public class DFSBackTrack : MonoBehaviour
{
    List<List<Node>> allPaths = new List<List<Node>>();
    List<Node> currentPath = new List<Node>();

    // I did not implement and initial List of forbidden paths
    // Since we are working 4-Way linked lists of nodes.
    // Forbidden nodes by their very nature won't ever be created.
    // Since any link from a node to another node is by default a valid node
    // Forbidden nodes would be implemented if you used a static 2D array of Nodes
    public List<List<Node>> FindPaths(Node startNode, Node endNode, int length)
    {
        allPaths.Clear();
        currentPath.Clear();
        // We still need a list of visited nodes however, no infinte looping in circles
        List<Node> visited = new List<Node>();
        FindEachPath(startNode, endNode, startNode, length, visited);
        return allPaths;
    }

    private void FindEachPath(Node startNode, Node endNode, Node currentNode, int depth, List<Node> visited)
    {
        // add our currentNode to the path
        currentPath.Add(currentNode);
        visited.Add(currentNode);

        // if we made it to the path length, see if we are at the end.
        if (depth == 0)
        {
            // if we are at the end, save this path
            if (currentNode == endNode)
            {
                // We need to copy the current path to a new path
                // because lists are references and we need a new entire object to ref
                List<Node> newPath = new List<Node>();
                foreach (Node oneNode in currentPath)
                    newPath.Add(oneNode);
                allPaths.Add(newPath);
            }
            return;
        }

        // if not at the end, find our neighbors
        List<Node> neighbors = GetNeighbors(currentNode, visited);
        foreach (Node oneNode in neighbors)
        {
            FindEachPath(startNode, endNode, oneNode, depth - 1, visited);
            //Do some clean up since right here we are in the "backtracking" part
            //Pop the last node we added to path and visited off the lists
            visited.RemoveAt(visited.Count - 1);
            currentPath.RemoveAt(currentPath.Count - 1);
        }

    }

    private List<Node> GetNeighbors(Node current, List<Node> visited)
    {
        List<Node> neighborNodes = new List<Node>();
        if (current.NodeNorth != null)
        {
            neighborNodes.Add(current.NodeNorth.GetComponent<Node>());
        }
        if (current.NodeEast != null)
        {
            neighborNodes.Add(current.NodeEast.GetComponent<Node>());
        }
        if (current.NodeSouth != null)
        {
            neighborNodes.Add(current.NodeSouth.GetComponent<Node>());
        }
        if (current.NodeSouth != null)
        {
            neighborNodes.Add(current.NodeSouth.GetComponent<Node>());
        }
        //Check if we visited them already
        //Removing Nodes int the middle of a foreach loop is dicey at best
        //So best to start at the end and check to the beginning to remove
        for (int i = neighborNodes.Count - 1; i >= 0; i--)
            if (visited.Contains(neighborNodes[i]))
                neighborNodes.RemoveAt(i);
        return neighborNodes;

    }
}

public class Node
{
    public GameObject NodeNorth;
    public GameObject NodeSouth;
    public GameObject NodeEast;
    public GameObject NodeWest;

}
