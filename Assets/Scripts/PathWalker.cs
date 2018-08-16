using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathWalker : MonoBehaviour {
	NavMeshAgent agent;
	Transform agentLoc;
	MoveTo impulser;

	public List<Vector3> destinations = new List<Vector3>();
	public float radiusToSample = 300f;
	public float deltaToTryMove = 5f;
	public float agentSpeed = 40f;

	PathNode root;
	int id = 1;

	void Start() {
		this.agent = GetComponent<NavMeshAgent>();
		this.agentLoc = agent.transform;
		agent.autoBraking = false;
		agent.speed = this.agentSpeed;

		this.impulser = GetComponent<MoveTo>();

		root = new PathNode(null, agentLoc.position);
		root.setID("root");
	}

	public void getWaypoints() {
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		agentLoc = agent.transform;

		root = new PathNode(null, agentLoc.position);
		root.setID("root");
		
		FillTree();
		// destinations.Add(RandomNavmeshLocation(200f));
	}

	public Vector3 RandomNavmeshLocation(float radius) {
         Vector3 randomDirection = Random.insideUnitSphere * radius;
         randomDirection += agentLoc.position;
         NavMeshHit hit;
         Vector3 finalPosition = Vector3.zero;
         if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
             finalPosition = hit.position;            
         }
         return finalPosition;
     }

	 
	void FillTree() {
		for(int i = 0; i < 100; i++) {
			// Pick random point in the map
			Vector3 randPt = RandomNavmeshLocation(300f);

			// Find the closest node in the tree to that point
			PathNode closestNode = root.ClosestNode(randPt);
			print("Parent: " + closestNode.id);
			// Go to neighbour and walk 5 m towards that point
			destinations.Add(closestNode.position);
			Vector3 newPos = Vector3.MoveTowards(closestNode.position, randPt, this.deltaToTryMove);
			destinations.Add(newPos);
			// If we aren't at the random point, add this new point to the tree as a node (via closestNode)
			PathNode newSample = new PathNode(closestNode, newPos);

			newSample.setID(this.id.ToString());
			this.id++;

			closestNode.children.Add(newSample);
			print("\tNew sample: " + newSample.id);
		}
	}

	void TravelToNextPoint() {
		Vector3 randPt = RandomNavmeshLocation(300f);

		while(agent.transform.position != randPt) {
			// Find the closest node in the tree to that point
			PathNode closestNode = root.ClosestNode(randPt);
			print("Parent: " + closestNode.id);
			// Go to neighbour and walk 5 m towards that point
			
			Vector3 newPos = Vector3.MoveTowards(closestNode.position, randPt, this.deltaToTryMove);
			
			// If we aren't at the random point, add this new point to the tree as a node (via closestNode)
			PathNode newSample = new PathNode(closestNode, newPos);

			// "Name" the new child for debugging
			newSample.setID(this.id.ToString());
			this.id++;

			// Add it to the family as a child of closest node found
			closestNode.children.Add(newSample);
			print("\tNew sample: " + newSample.id);

			// Move towards it
			this.impulser.GoToNextPoint(this.agent, newSample.position);
		}
	}

	void Update() {
		if (!agent.pathPending) //&& agent.remainingDistance < 0.5f
            TravelToNextPoint();
	}

}
