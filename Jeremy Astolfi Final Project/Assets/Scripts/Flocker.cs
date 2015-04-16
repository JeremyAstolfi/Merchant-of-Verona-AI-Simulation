using UnityEngine;
using System.Collections;
//including some .NET for dynamic arrays called List in C#
using System.Collections.Generic;


public class Flocker : Vehicle
{
	private FlockManager flockManager;
	public GameObject plane;

	// a unique identification number assigned by the flock manager
	// for the purpose of using the distances array
	private int index = -1;
	public int Index {
		get { return index; }
		set { index = value; }
	}
	
	override public void Start ()
	{
		base.Start ();
		//get component references
		flockManager = FlockManager.Instance;
		plane = GameObject.FindGameObjectWithTag ("Plane");
	}


	//This function depends on the flockManager updating the direction
	//of the flock every frame. It makes use of the AlignTo function
	//that has been added to the Vehicle class.
	private Vector3 Alignment ()
	{
		return AlignTo (flockManager.FlockDirection);
	}

	
	// This function depends on the flockManager updating the centroid
	// of the flock every frame.
	private Vector3 Cohesion ()
	{
		return Seek (flockManager.Centroid);
	}



	// We clearly need lignment, Cohesion and Separation
	// for our Flockers, but you will nee to handle obstacle
	// avoidance and implement a strategy for keeping your
	// flock on stage - avoid the lemming effect
	 protected override void CalcSteeringForce ()
	{
		Vector3 force = Vector3.zero;
		//force += flockManager.alignmentWt * Alignment ();
		//force += flockManager.cohesionWt * Cohesion ();
		//force += flockManager.separationWt * Separation (flockManager.Flockers, flockManager.separationDist);

		for (int i=0; i<flockManager.Obstacles.Length; i++) {	
				force += flockManager.avoidWt * AvoidObstacle (flockManager.Obstacles [i], flockManager.avoidDist);
		}
		if (Vector3.Distance (transform.position, plane.transform.position) >= plane.transform.position.x * plane.transform.localScale.x) {
			force += (plane.transform.position - transform.position);
				}

		//limit force to maxForce and apply
		force = Vector3.ClampMagnitude (force, maxForce);
		ApplyForce(force);
	}
	
}