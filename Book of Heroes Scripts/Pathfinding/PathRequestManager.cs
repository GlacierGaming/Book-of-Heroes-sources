using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour {
	// Controlling the requested paths 

	Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
	PathRequest currentPathRequest;

	static PathRequestManager instance;
	Pathfinding pathfinding;

	bool isProcessingPath;

	void Awake() {
		instance = this;
		pathfinding = GetComponent<Pathfinding>();
	}

	public static void RequestPath(Grid grid, Vector3 pathStart, Vector3 pathEnd, GameObject searchingObject, Action<Vector3[], bool> callback) {
		// Putting the requests in a queue to process them in order
		PathRequest newRequest = new PathRequest(grid, pathStart, pathEnd, callback, searchingObject);
		instance.pathRequestQueue.Enqueue(newRequest);
		instance.TryProcessNext();
	}

	void TryProcessNext() {
		// Process the next path is possible
		if(!isProcessingPath && pathRequestQueue.Count > 0) {
			currentPathRequest = pathRequestQueue.Dequeue();
			isProcessingPath = true; 
			pathfinding.StartFindPath(currentPathRequest.grid, currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.searchingObject);
		}
	}

	public void FinishedProcessingPath(Vector3[] path, bool success, GameObject searchingObject) {
		// Return the result of the processed path and process the next one
		if(searchingObject == null) {
			success = false;
		}
		currentPathRequest.callback(path, success);
		isProcessingPath = false;
		TryProcessNext();
	}

	public int GetQueueSize() {
		return pathRequestQueue.Count;
	}

	struct PathRequest {
		// Struct for the paths that need to be processed
		public Vector3 pathStart, pathEnd;
		public Action<Vector3[], bool> callback;
		public GameObject searchingObject;
		public Grid grid;

		public PathRequest(Grid _grid, Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback, GameObject _searchingObject) {
			grid = _grid;
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
			searchingObject = _searchingObject;
		}
	}
}
