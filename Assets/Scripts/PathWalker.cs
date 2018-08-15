using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathWalker : MonoBehaviour {
	Transform agentLoc;
	public List<Vector3> destinations = new List<Vector3>();
	// Use this for initialization
	void Start () {
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		agentLoc = agent.transform;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void getWaypoints() {
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		agentLoc = agent.transform;
		destinations.Add(RandomNavmeshLocation(200f));
	}

	public Vector3 RandomNavmeshLocation(float radius) {
         Vector3 randomDirection = Random.insideUnitSphere * radius;
		 print(agentLoc);
         randomDirection += agentLoc.position;
         NavMeshHit hit;
         Vector3 finalPosition = Vector3.zero;
         if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
             finalPosition = hit.position;            
         }
         return finalPosition;
     }

}
