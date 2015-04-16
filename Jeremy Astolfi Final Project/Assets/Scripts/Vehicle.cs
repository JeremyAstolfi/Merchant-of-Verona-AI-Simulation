
using UnityEngine;
using System.Collections;
//including some .NET for dynamic arrays called List in C#
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]


abstract public class Vehicle : MonoBehaviour {

	
	//vehicle attributes
	public float maxSpeed = 10.0f;
	public float maxForce = 10.0f;
	public float mass = 1.0f;
	public float radius = 0.5f;
	public float gravity = 20.0f;

	//variables for wander
	float wanderRad = 4.0f;
	float wanderDist = 5.0f;
	float wanderRand = 3.0f;
	float wanderAng = 0.0f;

	protected CharacterController characterController;
	protected Vector3 acceleration;	//change in velocity per second
	protected Vector3 velocity;		//change in position per second
	protected Vector3 dv;           //desired velocity
	public Vector3 Velocity {
		get { return velocity; }
		set { velocity = value;}
	}

	//Classes that extend Vehicle must override CalcSteeringForce
	abstract protected void CalcSteeringForce();

	virtual public void Start(){
		acceleration = Vector3.zero;
		characterController = gameObject.GetComponent<CharacterController> ();
	}

	
	// Update is called once per frame
	protected void Update () {
		CalcSteeringForce ();
		
		//update velocity
		velocity += acceleration * Time.deltaTime;
		velocity.y = 0;	// we are staying in the x/z plane
		velocity = Vector3.ClampMagnitude (velocity, maxSpeed);
		
		//orient the transform to face where we going
		if (velocity != Vector3.zero)
			transform.forward = velocity.normalized;
		
		// keep us grounded
		velocity.y -= gravity * Time.deltaTime;
		
		// the CharacterController moves us subject to physical constraints
		characterController.Move (velocity * Time.deltaTime);
		
		//reset acceleration for next cycle
		acceleration = Vector3.zero;

	}

	protected void ApplyForce (Vector3 steeringForce){
		acceleration += steeringForce/mass;
	}

	// return the force to align with a given direction
	protected Vector3 AlignTo (Vector3 direction){
		dv = direction.normalized;
		dv *= maxSpeed;
		dv -= velocity;
		dv.y = 0;
		return dv;
	}


	protected Vector3 Seek (Vector3 targetPos)
	{
		//find dv, desired velocity
		dv = targetPos - transform.position;		
		dv = dv.normalized * maxSpeed; 	//scale by maxSpeed
		dv -= velocity;
		dv.y = 0;					// only steer in the x/z plane
		return dv;
	}
	
	protected Vector3 Flee (Vector3 targetPos)
	{
		//find dv, desired velocity
		dv = transform.position - targetPos;		
		dv = dv.normalized * maxSpeed; 	//scale by maxSpeed
		dv -= velocity;
		dv.y = 0;					// only steer in the x/z plane
		return dv;
	}

	protected Vector3 AvoidObstacle (GameObject obst, float safeDistance)
	{ 
            float obRadius = 0f;
		dv = Vector3.zero;
        if (obst.tag == "Obstacle")
        {
            obRadius = 4f;
        }
        if (obst.tag == "Townsperson")
        {
            obRadius = 0.75f;
        }
		//safeDistance += radius + obRadius;

		//vector from vehicle to center of obstacle
		Vector3 vecToCenter = obst.transform.position - transform.position;
		//eliminate y component so we have a 2D vector in the x, z plane
		vecToCenter.y = 0;

		// distance should not be allowed to be zero or negative because 
		// later we will divide by it and do not want to divide by zero
		// or cause an inadvertent sign change.
		float dist = Mathf.Max(vecToCenter.magnitude - obRadius - radius, 0.05f);
		
		// if too far to worry about, out of here
		if (dist > safeDistance)
			return Vector3.zero;
		
		//if behind us, out of here
		if (Vector3.Dot (vecToCenter, transform.forward) < 0)
			return Vector3.zero;
		
		float rightDotVTC = Vector3.Dot (vecToCenter, transform.right);
		
		//if we can pass safely, out of here
		if (Mathf.Abs (rightDotVTC) > radius + obRadius)
			return Vector3.zero;

		//if we get this far, than we need to steer
		
		//obstacle is on right so we steer to left
		if (rightDotVTC > 0)
			dv = transform.right * -maxSpeed * safeDistance / dist;
		else
		//obstacle on left so we steer to right
			dv = transform.right * maxSpeed * safeDistance / dist;

		dv -= velocity;    //calculate the steering force
		dv.y = 0;		   // only steer in the x/z plane
		return dv;
	}

	// smoothed random walk
	protected Vector3 Wander( ){
		Vector3 target = transform.position + transform.forward * wanderDist;
		Quaternion rot = Quaternion.Euler(0, wanderAng, 0);
		Vector3 offset = rot * transform.forward;
		target += offset * wanderRad;
		wanderAng += Random.Range (-wanderRand, wanderRand);
		return Seek (target);
	}

    public Vector3 Acceleration
    {
        get { return acceleration; }
        set { acceleration = value; }
    }

}
