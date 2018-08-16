using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class PathNode {
	public PathNode parent;
	public HashSet<PathNode> children = new HashSet<PathNode>();
	public Vector3 position;
	public string id = "unset";

	public PathNode(PathNode par, Vector3 pos) {
		this.parent = par;
		this.position = pos;
	}

	public void setID(string ID) {
		this.id = ID;
	}

	public PathNode ClosestNode(Vector3 randPt) {
		if(children.Count == 0) {
			return this;
		}
		
		PathNode lowest = this;

		float distToPoint = Vector3.Distance(this.position, randPt);
		// Debug.Log("\t\t" +this.id + " has dist: " + distToPoint);
		foreach(PathNode c in children) {
			PathNode closestChild = c.ClosestNode(randPt);
			// Debug.Log("\t\t" +closestChild.id + " has dist: " + Vector3.Distance(closestChild.position, randPt));
			if(Vector3.Distance(closestChild.position, randPt) < distToPoint) {
				lowest = closestChild;
			}
		}
		Debug.Log("\tFinally chose: "+lowest.id);
		return lowest;
	}
}
