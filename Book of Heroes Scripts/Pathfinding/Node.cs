using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node> {

	public bool walkable;
	public Vector3 worldPosition;
	public int gridX, gridY, gCost, hCost, movementPenalty;
	public Node parent;
	int heapIndex;

	public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY, int _movementPenalty) {
		walkable = _walkable;
		worldPosition = _worldPosition;
		gridX = _gridX;
		gridY = _gridY;
		movementPenalty = _movementPenalty;
	}

	public int fCost {
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(Node otherNode) {
		int compare = fCost.CompareTo(otherNode.fCost);
		if(compare == 0)
			compare = hCost.CompareTo(otherNode.hCost);

		return -compare;
	}
}
