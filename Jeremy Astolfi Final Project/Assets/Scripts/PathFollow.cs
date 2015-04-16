using UnityEngine;
using System.Collections;

public class PathFollow : Vehicle {
	public float inFront;
	public float future;
	public float seekWt;
	public float width;
	GameObject[] path;
	Vector3 target;
	Vector3 futurePosition;
	Vector3 closestPoint;
	float closestDistance;
	public GameObject wp;
    public int Hwp;
    private int reputation;
    private float timer;
    private bool sold = false;
    private GameObject[] obstacles;
    private GameObject[] townspeople;

	// Use this for initialization
	void Start () {
		base.Start (); 
		path = GameObject.FindGameObjectsWithTag ("WP");
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        townspeople = GameObject.FindGameObjectsWithTag("Townsperson");
        Hwp = 0;
        reputation = 0;
        timer = 2f;
		getClosestPoint ();
	}
	
	protected override void CalcSteeringForce(){
        Vector3 force = Vector3.zero;
        nextClosestPoint();
        if (wp.GetComponent<WayPoint>().next.name == "Hwp")
        {
            Hwp = 1;
        }
        else
        {
            Hwp = 0;
        }
		target = closestPoint + wp.GetComponent<WayPoint> ().unitVec * inFront;
		if (closestDistance > width/2) {
			force += seekWt * Seek (target);
		}
		//limit force to maxForce and apply
		force = Vector3.ClampMagnitude (force, maxForce);
        ApplyForce(force);

        if (Hwp == 1)
        {
            Arrival(wp.GetComponent<WayPoint>().next);
        }
        foreach(GameObject o in obstacles){
            ApplyForce(2f*AvoidObstacle(o, 5f));
        }
		
		//show force as a blue line pushing the guy like a jet stream
		Debug.DrawLine(transform.position, transform.position - force,Color.blue);
		//red line to the target which may be out of sight
		Debug.DrawLine (transform.position, target,Color.red);
		
	}

    private void Arrival(GameObject target)
    {
        Acceleration = Vector3.zero;
        Vector3 desired = target.transform.position - transform.position;
        float d = desired.magnitude;
        desired.Normalize();
        if (d < 20)
        {
            desired = (d / 20 + 0.1f) * maxSpeed * desired;
            S();
            //if (d / 10 < 0.2f && !Sold)
            //{
            //   StartCoroutine(Sale());
            //}
        }
        else
        {
            desired = maxSpeed * desired;
        }
        desired -= velocity;
        desired = Vector3.ClampMagnitude(desired, maxForce);
        ApplyForce(desired);
    }
    private IEnumerator Sale()
    {
        while (timer > 0)
        {
            yield return new WaitForSeconds(1f);
            maxSpeed = 0;
            timer -= 1;
        }
        maxSpeed = 6;
        timer = 2f;
        Sold = true;
        reputation++;
    }

    private void S()
    {
        bool s = false;
        foreach (GameObject t in townspeople)
        {
            if (Vector3.Distance(t.transform.position, transform.position) < 3)
            {
                s = true;
            }
        }
        if (s)
        {
            maxSpeed = 0;
        }
        else
        {
            maxSpeed = 6;
        }
    }
    

	private void nextClosestPoint(){
		GameObject[] nextPoints = new GameObject[2];
		nextPoints[0] = wp.gameObject;
		nextPoints [1] = nextPoints[0].GetComponent<WayPoint>().next;
		closestDistance = 1000;
		float curClosestDistance;
		Vector3 curClosestPoint;
		futurePosition = transform.position + velocity * future;
		for (int i=0; i < 2; i++) {
			GameObject curWP = nextPoints[i];
			curClosestPoint = curWP.GetComponent<WayPoint>().closestPoint(futurePosition);
			curClosestDistance = Vector3.Distance(curClosestPoint, futurePosition);
			if(curClosestDistance < closestDistance){
                Sold = false;
				closestDistance = curClosestDistance;
				closestPoint = curClosestPoint;
				wp = curWP;
			}
		}
	}

	private void getClosestPoint(){
		closestDistance = 1000;
		float curClosestDistance;
		Vector3 curClosestPoint;
		futurePosition = transform.position + velocity * future;
		for (int i=0; i < path.Length; i++) {
			GameObject curWP = path[i];
			curClosestPoint = curWP.GetComponent<WayPoint>().closestPoint(futurePosition);
			curClosestDistance = Vector3.Distance(curClosestPoint, futurePosition);
			if(curClosestDistance < closestDistance){
				closestDistance = curClosestDistance;
				closestPoint = curClosestPoint;
				wp = curWP;
			}
		}
	}
    public int Reputation
    {
        get { return reputation; }
        set { reputation = value; }
    }

    public bool Sold
    {
        get { return sold; }
        set { sold = value; }
    }
}
