using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour {
	public List<Vector3> destinations;
	int nextPointIndex = 0;
       
	void Start () {
		
		
		PathWalker walker = GetComponent<PathWalker>();
		// walker.getWaypoints();
		
		// this.destinations = walker.destinations;
		
		// agent.destination = goal.position; 
		// agent.destination = destinations[0];

	}

	public void GoToNextPoint(NavMeshAgent agent, Vector3 destination) {
		// if(this.destinations.Count == 0) 
		// 	return;
		
		// agent.destination = this.destinations[nextPointIndex];
		agent.destination = destination;
		// nextPointIndex = (nextPointIndex + 1) % destinations.Count;

		// print(agent.destination);
	}
}