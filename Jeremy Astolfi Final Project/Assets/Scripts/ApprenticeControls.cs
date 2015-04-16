using UnityEngine;
using System.Collections;

public class ApprenticeControls : Vehicle {
    public GameObject merchant;
    private float timer = 0f;

	// Use this for initialization
	void Start () {
        base.Start();
        timer = 4f;
	}
	
    protected override void CalcSteeringForce()
    {
        Vector3 force = Vector3.zero;
        Seek(merchant.transform.position);
        Arrival(merchant);
    }

    private void Arrival(GameObject target)
    {
        Vector3 desired = target.transform.position - transform.position;
        float d = desired.magnitude;
        desired.Normalize();
        if (d < 7)
        {
            desired = (d / 7) * maxSpeed * desired;
            if (d / 7 < 0.15f)
            {
                Sale();
            }
        }
        else
        {
            desired = maxSpeed * desired;
        }
        desired -= velocity;
        desired = Vector3.ClampMagnitude(desired, maxForce);
        ApplyForce(desired);
    }

    private void Sale()
    {
        while (timer > 0f)
        {
            velocity = Vector3.zero;
            transform.LookAt(merchant.transform.position);
            timer -= Time.deltaTime;
        }
        timer = Time.deltaTime + 4f;
    }
}
