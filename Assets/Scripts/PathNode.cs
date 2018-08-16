using System.Collections;
using System.Collections.Generic;
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
		foreach(PathNode c in children) {
			if(Vector3.Distance(c.ClosestNode(randPt).position, lowest.position) < 0) {
				lowest = c;
			}
		}
		return lowest;
	}
}
