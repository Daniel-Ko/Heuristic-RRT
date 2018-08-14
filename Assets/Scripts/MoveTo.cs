using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour {
	public List<Vector3> destinations;
	// public Transform goal;
       
	void Start () {
		NavMeshAgent agent = GetComponent<NavMeshAgent>();

		
		PathWalker walker = GetComponent<PathWalker>();
		walker.getWaypoints();
		
		destinations = walker.destinations;
		print(destinations[0]);
		// agent.destination = goal.position; 
		agent.destination = destinations[0];
	}

	void fillTree() {

	}

	void calculateDestination() {

	}
}