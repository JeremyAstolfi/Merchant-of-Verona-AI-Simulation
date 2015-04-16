using UnityEngine;
using System.Collections;
//including some .NET for dynamic arrays called List in C#
using System.Collections.Generic;


public class FlockManager : MonoBehaviour
{
	// weight parameters are set in editor and used by all flockers 
	// if they are initialized here, the editor will override settings	 
	// weights used to arbitrate btweeen concurrent steering forces 
	public float alignmentWt;
	public float separationWt;
	public float cohesionWt;
	public float avoidWt;
	public float inBoundsWt;

	// these distances modify the respective steering behaviors
	public float avoidDist;
	public float separationDist;
	

	// set in editor to promote reusability.
	public int numberOfFlockers;
	public Object flockerPrefab;
	public Object obstaclePrefab;
	
	//values used by all flockers that are recalculated on update
	private Vector3 direction;
	private Vector3 centroid;
	
	//accessors
	private static FlockManager instance;
	public static FlockManager Instance { get { return instance; } }

	public Vector3 FlockDirection {
		get { return direction; }
	}
	
	public Vector3 Centroid { get { return centroid; } }
	public GameObject centroidContainer;
	
	public float howMany;
		
	// list of flockers with accessor
	private List<GameObject> flockers = new List<GameObject>();
	public List<GameObject> Flockers {get{return flockers;}}

	// array of obstacles with accessor
	private  GameObject[] obstacles;
	public GameObject[] Obstacles {get{return obstacles;}}

	// this is a 2-dimensional array for distances between flockers
	// it is recalculated each frame on update
	private float[,] distances;
		
		//construct our 2d array based on the value set in
	public void Start ()
	{
		instance = this;
		//construct our 2d array based on the value set in the editor
		distances = new float[numberOfFlockers, numberOfFlockers];
		//reference to Vehicle script component for each flocker
		Flocker flocker; // reference to flocker scripts
	
		for (int i=0; i< howMany; i++) 
		{
			Vector3 pos =  new Vector3(Random.Range(-40, 40), 4f, Random.Range(-40, 40));
			Quaternion rot = Quaternion.Euler(0, Random.Range(0, 90), 0);
			GameObject.Instantiate(obstaclePrefab, pos, rot);
		}

		obstacles = GameObject.FindGameObjectsWithTag ("Obstacle");

		for (int i = 0; i < numberOfFlockers; i++) {
			//Instantiate a flocker prefab, catch the reference, cast it to a GameObject
			//and add it to our list all in one line.
			Vector3 pos = new Vector3(Random.Range(-40, 40), 1f, Random.Range( -40, 40));

			flockers.Add ((GameObject)Instantiate (flockerPrefab, pos, Quaternion.identity));
			//grab a component reference
			flocker = flockers [i].GetComponent<Flocker> ();
			//set values in the Vehicle script
			flocker.Index = i;
		}

		centroidContainer.transform.position = new Vector3 (320, 30, 100);
		
	}
	public void Update( )
	{
		calcCentroid( );//find average position of each flocker 
		calcFlockDirection( );//find average "forward" for each flocker

		//position & orient the centoid container for the SmoothFollow camera script
		centroidContainer.transform.position = centroid;
		centroidContainer.transform.forward = direction;

		// This function which populates the 2-dimensional distance array, lets us 
		// calculate distance between each pair of flockers exactly once per frame
		calcDistances( );
	}
	
	
	void calcDistances( )
	{
		float dist;
		for(int i = 0 ; i < numberOfFlockers; i++)
		{
			for( int j = i+1; j < numberOfFlockers; j++)
			{
				dist = Vector3.Distance(flockers[i].transform.position, flockers[j].transform.position);
				distances[i, j] = dist;
				distances[j, i] = dist;
			}
		}
	}
	
	public float getDistance(int i, int j)
	{
		return distances[i, j];
	}
	
	
		
	private void calcCentroid ()
	{
		//*******************************************************
		// calculate the current centroid of the flock
		// use transform.position - you need to write this!
		//*******************************************************
		foreach(GameObject f in flockers){
		centroid += f.transform.position;
		}
		centroid /= flockers.Count;
	}
	
	private void calcFlockDirection ()
	{
		//*******************************************************
		// calculate the average heading of the flock
		// use transform.forward - you need to write this!
		//*******************************************************
		
		foreach(GameObject f in flockers){
			direction += f.transform.forward;
		}
		direction.Normalize ();
		direction *= 10.0f;
		direction -= centroid;
	}
	
}