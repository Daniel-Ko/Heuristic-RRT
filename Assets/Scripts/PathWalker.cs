using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathWalker : MonoBehaviour {
	NavMeshAgent agent;
	Transform agentLoc;
	MoveTo impulser;
	TrailRenderer trail;

	public List<Vector3> destinations = new List<Vector3>();
	public float radiusToSample = 20f;
	public float deltaToTryMove = 5f;
	public float agentSpeed = 80f;

	// Used to force points to spread out
	public float usefulDist = 10f;

	public float timeToPathFade = 3.0f;

	PathNode root;
	int id = 1;

	void Start() {
		this.agent = GetComponent<NavMeshAgent>();
		this.agentLoc = agent.transform;
		agent.autoBraking = false;
		agent.updateUpAxis = true;
		agent.speed = this.agentSpeed;
		agent.angularSpeed = 120f;

		this.impulser = GetComponent<MoveTo>();

		root = new PathNode(null, agentLoc.position);
		// Add for logging
		root.setID("root");
		destinations.Add(root.position);
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

	void ExploreToNextPoint() {
		Vector3 randPt = RandomNavmeshLocation(this.radiusToSample);

		Vector3 currentPt = agent.transform.position;
		int tries = 0;
		// while(currentPt != randPt || tries < 3) {
			print("Current: "+currentPt + " | Dest: " + randPt);
			// Find the closest node in the tree to that point
			PathNode closestNode = root.ClosestNode(randPt);
			print("Parent: " + closestNode.id);

			// Check that the new point is a useful distance away (IMPROVES PATHFINDING)
			float distToRandPt = Vector3.Distance(closestNode.position, randPt);
			if(distToRandPt < usefulDist)
				return;

			Vector3 newPos = Vector3.MoveTowards(closestNode.position, randPt, this.deltaToTryMove);
			
			// Move towards it
			this.impulser.GoToNextPoint(this.agent, newPos);

			// If we aren't at the random point, add this new point to the tree as a node (via closestNode)
			PathNode newSample = new PathNode(closestNode, newPos);

			// "Name" the new child for debugging
			newSample.setID(this.id.ToString());
			this.id++;

			// Add it to the family as a child of closest node found
			closestNode.children.Add(newSample);
			destinations.Add(newPos);

			

			currentPt = newPos;
			tries++;
			
		// }
		tries = 0;
	}

	void Update() {
		if (!agent.pathPending) { //&& agent.remainingDistance < 0.5f
            ExploreToNextPoint();
			TrailDraw();
		}
	}

	/*  Draws trail behind agent.
		Based off code found in the Unity forums to draw (predicted) path of agent
	 */
	void OnDrawGizmosSelected() {
		if( this.agent == null || this.agent.path == null )
			return;

		this.trail = this.GetComponent<TrailRenderer>();
		if(trail == null) {
			this.trail = this.gameObject.AddComponent<TrailRenderer>();
		}
		// float alpha = 1.0f;
		// Gradient gradient = new Gradient();
		// gradient.SetKeys(
		// 	new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.red, 1.0f) },
		// 	new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
		// );
		// trail.colorGradient = gradient;
	}

	void TrailDraw() {
		if( this.agent == null || this.agent.path == null )
			return;

		this.trail = this.GetComponent<TrailRenderer>();
		if(trail == null) {
			this.trail = this.gameObject.AddComponent<TrailRenderer>();
		}

		trail.time = timeToPathFade;
	}

	void OnGUI() {
        GUI.Label(new Rect(10, 20, 200, 30), "FadeTime");
        timeToPathFade = GUI.HorizontalSlider(new Rect(15, 40, 200, 30), timeToPathFade, 0.1f, 100f);

		GUI.Label(new Rect(275, 20, 200, 30), "DeltaToSample");
		deltaToTryMove = GUI.HorizontalSlider(new Rect(280, 40, 200, 30), deltaToTryMove, 1f, 100f);

		GUI.Label(new Rect(10, 200, 200, 30), "Radius");
		radiusToSample = GUI.HorizontalSlider(new Rect(15, 220, 200, 30), radiusToSample, 10f, 300f);
    }

}
