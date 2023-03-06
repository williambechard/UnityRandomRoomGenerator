using System.Collections.Generic;
using UnityEngine;
public enum NodeState
{
    Available,
    Current,
    Completed,
    Start,
    Final
}

public class Room : MonoBehaviour
{
    public List<GameObject> openDoors = new();

    public GameObject topDoor;
    public GameObject bottomDoor;
    public GameObject leftDoor;
    public GameObject rightDoor;

    public Vector2 pos;

    public int x;
    public int y;

    public SpriteRenderer floor;
    public NodeState state;

    public void SetState(NodeState state)
    {
        this.state = state;
        switch (state)
        {
            case NodeState.Available:
                floor.color = Color.white;
                break;
            case NodeState.Current:
                floor.color = Color.yellow;
                break;
            case NodeState.Completed:
                floor.color = Color.blue;
                break;
            case NodeState.Start:
                floor.color = Color.green;
                break;
            case NodeState.Final:
                floor.color = Color.red;
                break;
        }
    }

}
