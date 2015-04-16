using UnityEngine;
using System.Collections;

public class WayPoint : MonoBehaviour {
	public GameObject next;
	public Vector3 start, end, segment, unitVec;
	float mag;
	
	void Start () {
		start = transform.position;
		end = next.transform.position;
		segment = end - start;
		mag = segment.magnitude;
		unitVec = segment.normalized;
	}

	void Update(){
		Debug.DrawLine(start, end, Color.green);
	}
	
	public Vector3 closestPoint(Vector3 pt){
		Vector3 startToPt = pt - start;
		float projection = Vector3.Dot (startToPt, unitVec);
		if (projection <= 0)
			return start;
		if (projection >= mag)
			return end;
		return start + unitVec * projection;
	}
}
