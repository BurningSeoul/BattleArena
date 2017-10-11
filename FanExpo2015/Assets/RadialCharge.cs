using UnityEngine;
using System.Collections;

public class RadialCharge : MonoBehaviour {
	private Vector3 currentWaypointPos;
	private Vector3 oppositeWaypointPos;
	private int waypoint;
	public Transform[] clockPositions;	//Positions are ordered: 0 - 12, 1 - 1, 2 - 2 etc.
	// Use this for initialization
	void Start () {
		//clockPositions = transform.GetComponentsInChildren<Transform>();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Vector3 RandomizeWaypoint(){
		waypoint = (int)Random.Range(0,clockPositions.Length);
		currentWaypointPos = clockPositions[waypoint].position;
		return currentWaypointPos;
	}

	public Vector3 GetOppositeWaypoint(){
		switch(waypoint){
		case 0:
			oppositeWaypointPos = clockPositions[6].position;
			break;
		case 1:
			oppositeWaypointPos = clockPositions[7].position;
			break;
		case 2:
			oppositeWaypointPos = clockPositions[8].position;
			break;
		case 3:
			oppositeWaypointPos = clockPositions[9].position;
			break;
		case 4:
			oppositeWaypointPos = clockPositions[10].position;
			break;
		case 5:
			oppositeWaypointPos = clockPositions[11].position;
			break;
		case 6:
			oppositeWaypointPos = clockPositions[0].position;
			break;
		case 7:
			oppositeWaypointPos = clockPositions[1].position;
			break;
		case 8:
			oppositeWaypointPos = clockPositions[2].position;
			break;
		case 9:
			oppositeWaypointPos = clockPositions[3].position;
			break;
		case 10:
			oppositeWaypointPos = clockPositions[4].position;
			break;
		case 11:
			oppositeWaypointPos = clockPositions[5].position;
			break;
		}
		return oppositeWaypointPos;
	}

	public Vector3 GetWaypoint(int i){return clockPositions[i].position;}

}
