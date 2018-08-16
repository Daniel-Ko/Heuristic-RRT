using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour {
	public void GoToNextPoint(NavMeshAgent agent, Vector3 destination) {
		agent.SetDestination(destination);
	}
}