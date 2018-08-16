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
		foreach(PathNode c in children) {
			PathNode closestChild = c.ClosestNode(randPt);
			if(Vector3.Distance(closestChild.position, randPt) < distToPoint) {
				lowest = closestChild;
			}
		}
		return lowest;
	}
}
