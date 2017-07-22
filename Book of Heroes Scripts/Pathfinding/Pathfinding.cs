using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour {

	PathRequestManager requestManager;

	void Awake() {
		requestManager = GetComponent<PathRequestManager>();
	}

	public void StartFindPath(Grid grid, Vector3 startPos, Vector3 targetPos, GameObject searchingObject) {
		StartCoroutine(FindPath(grid, startPos, targetPos, searchingObject));
	}

	IEnumerator FindPath(Grid grid, Vector3 startPos, Vector3 targetPos, GameObject searchingObject) {
		// A* algorithm to find path
		
		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;

		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		Heap<Node> openSet = new Heap<Node>(6400);
		HashSet<Node> closedSet = new HashSet<Node>();

		openSet.Add (startNode);
		while(openSet.Count > 0) {
			Node currentNode = openSet.RemoveFirst();
			closedSet.Add(currentNode);

			if(searchingObject == null || GetDistance(startNode, currentNode) > 400) {
				pathSuccess = false;
				break;
			}

			if(currentNode == targetNode) {
				pathSuccess = true;
				break;
			}

			foreach(Node neighbour in grid.GetNeighbours(currentNode)) {

				if((!neighbour.walkable && neighbour != targetNode) || closedSet.Contains(neighbour)) { 
					continue;
				}

				/*if(neighbour != targetNode && neighbour.gridX != currentNode.gridX && neighbour.gridY != currentNode.gridY
				   && (!grid.grid[currentNode.gridX, neighbour.gridY].walkable || !grid.grid[neighbour.gridX, currentNode.gridY].walkable))
					continue;*/

				int newMovementCost = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
				if(newMovementCost < neighbour.gCost || !openSet.Contains(neighbour)) {
					neighbour.gCost = newMovementCost;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = currentNode;

					if(!openSet.Contains(neighbour))
						openSet.Add(neighbour);
					else
						openSet.UpdateItem(neighbour);
				}
			}
		}
		yield return null;

		if(pathSuccess) {
			waypoints = RetracePath(startNode, targetNode);
		}
		requestManager.FinishedProcessingPath(waypoints, pathSuccess, searchingObject);
	}

	Vector3[] RetracePath(Node startNode, Node endNode) {
		// Recreate the path so a character can follow it
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while(currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}

		Vector3[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);
		return waypoints;
	}

	Vector3[] SimplifyPath(List<Node> path) {
		// Eliminate unnecessary nodes from the path
		List<Vector3> waypoints = new List<Vector3>();
		Vector2 directionOld = Vector2.zero;

		for(int i = 1; i < path.Count; ++i) {
			Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
			if(directionNew != directionOld) {
				waypoints.Add(path[i].worldPosition);
			}
			directionOld = directionNew;
		}
		return waypoints.ToArray();
	}

	int GetDistance(Node nodeA, Node nodeB) {
		int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if(distX > distY)
			return 14 * distY + 10 * (distX - distY);
		return 14 * distX + 10 * (distY - distX);
	}

	float GetWorldPosDist(Node nodeA, Node nodeB) {
		float distX = Mathf.Abs (nodeA.worldPosition.x - nodeB.worldPosition.x);
		float distY = Mathf.Abs (nodeA.worldPosition.y - nodeB.worldPosition.y);

		return Mathf.Sqrt(distX * distX + distY * distY);
	}
}
