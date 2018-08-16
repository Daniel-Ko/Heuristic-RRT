using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour {
	NavMeshAgent agent;

	public List<Vector3> destinations;

	public float agentSpeed = 40f;
	int nextPointIndex = 0;
	
       
	void Start () {
		this.agent = GetComponent<NavMeshAgent>();
		PathWalker walker = GetComponent<PathWalker>();
		walker.getWaypoints(this.agent);
		
		this.destinations = walker.destinations;
	}

	public void GoToNextPoint() { //NavMeshAgent agent, Vector3 destination
		if(this.destinations.Count == 0) 
			return;
		
		agent.destination = this.destinations[nextPointIndex]; // agent.destination = destination;
		
		nextPointIndex = (nextPointIndex + 1) % destinations.Count;

		// print(agent.destination);
	}

	void Update() {
		if (!agent.pathPending) //&& agent.remainingDistance < 0.5f
            GoToNextPoint();
	}
}