using UnityEngine;
using System.Collections;

public class TownspersonControls : Vehicle {
    GameObject merchant;
    public GameObject wp;
    public GameObject house;
    public GameObject next;
    private GameObject[] townspeople;
    private bool queued = false;
    private float timer = 0f;

	// Use this for initialization
	void Start () {
        base.Start();
        merchant = GameObject.FindGameObjectWithTag("Merchant");
        townspeople = GameObject.FindGameObjectsWithTag("Townsperson");
        timer = 10f;
	}

    protected override void CalcSteeringForce()
    {
        Vector3 force = Vector3.zero;
        if (Vector3.Distance(wp.transform.position, merchant.transform.position) < 15f)
        {
            if (velocity == Vector3.zero)
                velocity += 3*Vector3.forward;
            queued = true;
        }
        foreach (GameObject t in townspeople)
        {
            if (t.transform.position != transform.position)
            {
                ApplyForce(AvoidObstacle(t, 0.05f));
            }
        }
        if (queued)
        {
            Arrival(Next.transform.position - Next.transform.forward);
            Queue();
            if (velocity == Vector3.zero)
            {
                queued = false;
            }
        }
    }

	public void Queue()
	{
        if (timer <= 0)
        {
            if (Next == WP)
            {
                Next = House;
            }
            else
            {
                foreach (GameObject t in townspeople)
                {
                    if (t == Next)
                    {
                        Next = t.GetComponent<TownspersonControls>().Next;
                    }
                }
            }
            timer = Time.deltaTime + 10f;
        }
        timer -= Time.deltaTime;
	}

	public GameObject Next
	{
		get{ return next;}
		set{ next = value;}
	}

    public GameObject WP
    {
        get { return wp; }
        set { wp = value; }
    }

    public GameObject House
    {
        get { return house; }
        set { house = value; }
    }
	
	private void Arrival(Vector3 target)
	{
		Vector3 desired = target - transform.position;
		float d = desired.magnitude;
		desired.Normalize();
		if (d < 30 && d > 1)
		{
			desired = (d / 30) * maxSpeed * desired;
		}
        else if (d <= 1)
        {
            desired = Vector3.zero;
        }
        else
        {
            desired = maxSpeed * desired;
        }
		desired -= velocity;
		desired = Vector3.ClampMagnitude(desired, maxForce);
		ApplyForce(desired);
	}
}
