using UnityEngine;
using System.Collections;

public class SpawnerScript : MonoBehaviour {
	public float spawnTime = 3f;		// The amount of time between each spawn.
	public float spawnDelay = 0f;		// The amount of time before spawning starts.
	
	public float spawnCrowTime = 8f;
	public float spawnCrowDelay = 6f;
	
	private GameObject [] object_prefabs;		// Array of prefabs.
	private int number;
	
	public static GameObject[]	boids ;
	float[] boid;

	
	// Use this for initialization
	void Start () {
		boids = new GameObject[200];
		boid = new float[200*3];
		object_prefabs = new GameObject[1];
		object_prefabs [0] = Resources.Load<GameObject> ("Prefabs/particle_prefab");
		number = 0;
		
		InvokeRepeating("Spawn", spawnDelay, spawnTime);
		for(int i=0;i<200;i++){
			if (boids[i] != null) {
					boid [i*3] =UnityEngine.Random.Range (-1, 1);
					boid [i*3+1] = UnityEngine.Random.Range (-1, 1);
					boid [i*3+2] =UnityEngine.Random.Range (-1, 1);
			}
		}	
	}
	
	void Spawn ()
	{
		if(number>=100)return;
		
		transform.position = new Vector3(0.0f, 0.0f, 0.0f);
		
		for(int i=0; i<50; i++){
			float r = 200.0f;
			float x_perturb = Random.Range (-r, r);
			float y_perturb = Random.Range (-r, r);
			float z_perturb = Random.Range (-r, r);
			Vector3 pos = new Vector3(transform.position.x+x_perturb, transform.position.y+y_perturb, transform.position.z+z_perturb);
			boids[number] = Instantiate(object_prefabs[0], pos, transform.rotation)as GameObject;
			
			number++;
			
		}
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		for (int i=0; i<100; i++) {
			if (boids[i] != null) {

				Vector3 velocity = new Vector3 (boid [i*3], boid [1+i*3], boid [2+i*3]);
				velocity += avoidWalls(boids [i]);
				if(Random.Range(0.0f,1.0f)>0.5f){
					velocity += flock(boid, boids,i);
				}
				if (velocity.magnitude > 4) {
					velocity *= (4 / velocity.magnitude);
				}


				float ry = Mathf.Atan2(velocity.x, velocity.z)*Mathf.Rad2Deg;
				float rx = -Mathf.Asin(velocity.y/velocity.magnitude)*Mathf.Rad2Deg;
				Vector3 rotation = new Vector3(rx,ry,0);
				boids [i].transform.eulerAngles = rotation;
				boids [i].transform.position += velocity;
				boid [i*3] = velocity [0];
				boid [i*3+1] = velocity [1];
				boid [i*3+2] = velocity [2];
			}
		}
	}

	
	Vector3 avoidWalls(GameObject boids)
	{
		Vector3 vector = Vector3.zero;
		vector += avoid(new Vector3(-500.0f, boids.transform.position.y, boids.transform.position.z),boids);
		vector += avoid(new Vector3(500.0f,boids.transform.position.y, boids.transform.position.z),boids);
		vector += avoid(new Vector3(boids.transform.position.x, -500.0f, boids.transform.position.z),boids);
		vector += avoid(new Vector3(boids.transform.position.x, 500.0f, boids.transform.position.z),boids);
		vector += avoid(new Vector3(boids.transform.position.x,boids.transform.position.y, -400.0f),boids);
		vector += avoid(new Vector3(boids.transform.position.x, boids.transform.position.y, 400.0f),boids);
		return vector;
		
		
		
	}

	Vector3 avoid(Vector3 target, GameObject boids)
	{
		Vector3  steer;
		steer = boids.transform.position-target;
		steer /= (steer.sqrMagnitude);
		return steer*5;
	}

	//Flock
	Vector3 flock(float[] boid, GameObject[] boids, int id)
	{
		Vector3 acceleration = Vector3.zero;
		acceleration += alignment(boid, boids,id);
		acceleration += cohesion(boid, boids,id);
		acceleration += separation(boid, boids,id);
		return acceleration;
	}


	// Dan lien ket
	Vector3 alignment(float[] boid, GameObject[] boids, int id) {
		Vector3 velSum = Vector3.zero;
		int count = 0;
		for (int i = 0; i <100;i++) {
			if (boids[i] != null){
			if(UnityEngine.Random.Range (0, 1)<0.6f)
			{
				float distance = (boids[i].transform.position - boids[id].transform.position).magnitude;
				if (distance > 0.0f && distance <= 80.0f)
				{
					velSum += new Vector3(boid[i*3], boid[i*3+1], boid[i*3+2]);
					count++;
				}
				}
			}
			
		}
		if (count > 0.0) 
		{
			velSum /= count;
			float l = velSum.magnitude;
			if (l > 0.1f)
			{
				velSum *=(0.1f/l);
			}
		}
		return velSum;
	}
// Dan gan ket

	Vector3 cohesion(float[] boid, GameObject[] boids, int id) {
		Vector3 posSum = Vector3.zero;
		int count = 0;
		for (int i = 0; i <100;i++) {
			if (boids[i] != null){
			if(UnityEngine.Random.Range (0, 1)<0.6f)
			{
				float distance = (boids[i].transform.position - boids[id].transform.position).magnitude;
				if (distance > 0.0f && distance <= 80.0f)
				{
					posSum +=  boids[i].transform.position;
					count++;
				}
			}
			}
		}
		if (count > 0.0) {
			posSum /= count;
		}
			posSum -= boids[id].transform.position;
			float l = posSum.magnitude;
			if (l > 0.1f)
			{
				posSum *=(0.1f/l);
			}
		return posSum;
	} 
	// Khoang cach
	Vector3 separation(float[] boid, GameObject[] boids, int id) {
		Vector3 posSum = Vector3.zero;
		Vector3 repulse;
		for (int i = 0; i <100;i++) {
			if (boids[i] != null){
			if(UnityEngine.Random.Range (0, 1)<0.6f)
			{
				float distance = (boids[i].transform.position - boids[id].transform.position).magnitude;
				if (distance > 0.0f && distance <= 80.0f)
				{
					repulse = boids[id].transform.position-boids[i].transform.position;
					repulse = repulse.normalized;
					repulse /= distance;
					posSum += repulse;
				}
			}
			}
		}
		return posSum;
	} 
}