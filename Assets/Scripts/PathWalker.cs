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
	public List<Vector3> boundaries = new List<Vector3>();
	public float radiusToSample = 20f;
	public float deltaToTryMove = 5f;
	public float agentSpeed = 80f;

	// Used to force points to spread out
	public float usefulDist = 10f;

	public float timeToPathFade = 3.0f;

	// Optimised Randomly-Exploring Random Tree 
	PathNode root;

	// Logging stats
	int id = 1;
	int boundariesMarked = 0;

	void Start() {
		InvokeRepeating("Log", 15.0f, 15.0f);

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

	 public Vector3 AimAwayFromNeighbourhood() {
		Vector3 randPt = RandomNavmeshLocation(this.radiusToSample);
		Vector3 newAngle = randPt;

		float largestDistFromTree = 0;
		Vector3 freshestAngle = randPt;

		// Check 18 different angles around the clock for boundaries/free space
		for(int i = 0; i < 18; i++) {
			// Find closest node in tree to the random pt we have chosen
			PathNode closestNode = root.ClosestNode(newAngle);

			// Agent is closer to unexplored space than the tree is
			float distFromTree = Vector3.Distance(closestNode.position, newAngle);
			if(Vector3.Distance(this.agentLoc.position, newAngle) < 
				distFromTree) {
				return newAngle;
			}
			if(largestDistFromTree < distFromTree)
				freshestAngle = newAngle;
			// rotate clockwise by 40degrees
			newAngle = Quaternion.Euler(20, 0, 0) * (newAngle - this.agentLoc.position) + 
						this.agentLoc.position;
		}
		return freshestAngle;
		

	 }

	void ExploreToNextPoint() {
		this.agentLoc = this.agent.transform;
		Vector3 currentPt = this.agentLoc.position;
		
		Vector3 target = AimAwayFromNeighbourhood();

		int tries = 0;
		
		// Find the closest node in the tree to that point
		PathNode closestNode = root.ClosestNode(target);
		// print("Parent: " + closestNode.id);

		// Check that the new point is a useful distance away (IMPROVES PATHFINDING)
		float distToTarget = Vector3.Distance(closestNode.position, target);
		if(distToTarget < usefulDist)
			return;
		
		// Test for boundaries by raycasting in that direction.
		target = ScanForBoundaries(target);

		// Now that we know the point is not out of bounds, we get the coords
		Vector3 newPos = Vector3.MoveTowards(closestNode.position, target, this.deltaToTryMove);
		
		// And move the agent towards the goal
		this.impulser.GoToNextPoint(this.agent, newPos);

		// If our move towards the point timed out, 
		// add where we ended up to the tree as a node (via closestNode)
		PathNode newSample = new PathNode(closestNode, newPos);

		// "Name" the new child for debugging
		newSample.setID(this.id.ToString());
		this.id++;

		// And introduce it to the family as a child of closest node found
		closestNode.children.Add(newSample);
		destinations.Add(newPos);

			

		currentPt = newPos;
		tries++;
	
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
	}

	//360 scan and mark boundaries while also recording escape routes just in case we are in a corner
	Vector3 ScanForBoundaries(Vector3 randPt) {

		// Base Case to make sure the tightening of the radius is not too small (see end of function)
		if (Vector3.Distance(this.agentLoc.position, randPt) <= 2f)
			return randPt;
		
		NavMeshHit possibleBoundaryEdge;
		Vector3 newAngle = randPt;
		Vector3? escapeVector = null;

		// Check 18 different angles around the clock for boundaries/free space
		for(int i = 0; i < 18; i++) {
			if(this.agent.Raycast(newAngle, out possibleBoundaryEdge)) {
				Vector3 bot = possibleBoundaryEdge.position;
				bot.y -= 5;
				Vector3 top = possibleBoundaryEdge.position;
				top.y += 5;
				Debug.DrawLine(bot, top, Color.red, Mathf.Infinity);
				boundariesMarked++;
			} else {
				// free zone found
				escapeVector = newAngle;
			}
			// rotate clockwise by 40degrees
			newAngle = Quaternion.Euler(20, 0, 0) * (newAngle - this.agentLoc.position) + 
						this.agentLoc.position;
		}

		// if no free zone found, tighten the search radius and try again
		if(!escapeVector.HasValue) {
			return ScanForBoundaries(Vector3.Lerp(this.agentLoc.position, randPt, 0.5f));
		}
		return (Vector3) escapeVector;
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

		// GUI.Label(new Rect(10, 200, 200, 30), "Radius");
		// radiusToSample = GUI.HorizontalSlider(new Rect(15, 220, 200, 30), radiusToSample, 10f, 300f);
    }

	void Log() {
		print((Time.time) + " sec elapsed.\nBoundaries Marked: " + this.boundariesMarked + "\nNodes created: " + this.id);
	}

}
