COMP313
Daniel Ko
Assignment 1

# **Optimised Randomly-Exploring Random Tree**

## **Demo**:

https://www.youtube.com/watch?v=w4I2DR7xQPE
https://www.youtube.com/watch?v=S1Nvf0foQG4

## **Description**

---

Terrain is rarely mapped by hand in video games. As the complexity of games increases, the requirements on their maps do too. With many game actions taking place on game maps which are getting increasingly larger and more detailed, meshing and pathfinding is most quickly and quite accurately done by tools.
These technological advances, however, do not take away from the need to have navigation be a core mechanic and finely-tuned in order to appeal to the player. Boundaries need to be solid and pathfinding needs to be logical. It is important, then, to find approaches to making sure large, detailed maps are well-defined.
One way of approaching exploring automatically generated mesh is with a state-search algorithm. I used a well-known and easy algorithm for this purpose, called Randomly-Exploring Random Tree.

It randomly finds a point around it in and treads a set distance toward it via the closest node in this tree. If it cannot get there, it'll make a new node in the tree at that spot of failure to mark its progress.
I also made a definite choice by making an agent tread the path of the tree. Instead of store all the node positions in a collection, I decided to make the agent 'explore' the map and generate where to go next dynamically.

This data structure got the agent moving, but it wasn't efficient. The agent would walk back and forth, or get caught on a cliff-top. If it went into a corner, the agent would stop dead.

Loosely following several articles on similar processes, I optimised the agent to 'scan' around it obstacles or boundaries, and mark them with red lines. I also made it find an estimated escape-route AKA any route that is not blocked.

Then I made the agent not only pick a random point but instead use that random point to scan around itself for the furthest distance away from any nodes in the tree. This resulted in the agent exploring the map quite well.

The agent was still getting stuck so I tried making the agent's scan for obstacles recursive -- if there was no escape route found, recurse with half the radius and try again, and again. This improvement lets the agent wiggle in tight spaces and find places to go. I still don't know if it can find its way out of a maze, however.

These optimisations were a little crude, but they present a good example of how agents can 'semi-intelligently' explore the map on their own, and how an algorithm can try to explore the map to detect boundaries. This part works very well.

## **Search Terms**

---

'RRT', 'Randomly-Exploring Random Tree', 'Any-Angle Pathfinding'

## **Sources**

---

http://the-witness.net/news/2012/12/mapping-the-islands-walkable-surfaces/
http://aigamedev.com/open/highlights/rapidly-exploring-random-trees/
http://msl.cs.uiuc.edu/rrt/about.html
http://aigamedev.com/open/tutorials/theta-star-any-angle-paths/ (used the Line Of Sight concept)
