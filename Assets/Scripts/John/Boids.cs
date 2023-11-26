using UnityEngine;

public class Boids : MonoBehaviour
{
    [SerializeField] private Transform flock;
    [SerializeField] private float cohesionFactor = 0.2f;
    [SerializeField] private float separationFactor = 6.0f;
    [SerializeField] private float allignFactor = 1.0f;
    [SerializeField] private float constrainFactor = 2.0f;
    [SerializeField] private float avoidFactor = 20.0f;
    [SerializeField] private float collisionDistance = 6.0f;
    [SerializeField] private float speed = 6.0f;
    [SerializeField] private Vector3 constrainPoint;

    [SerializeField] private Vector3 avoidObst;
    [SerializeField] private float integrationRate = 3.0f;

    //final velocity
    public Vector3 velocity;

    //states
    public bool isFlocking = true;
    public Transform target;
    public GameObject batGeom;

    private float avoidCount;


    // Start is called before the first frame update
    private void Start()
    {
        flock = transform.parent;


        var pos = new Vector3(Random.Range(0f, 80), Random.Range(0f, 20f), Random.Range(0f, 80));
        var look = new Vector3(Random.Range(-1000f, 1000f), Random.Range(-1000f, 1000f), Random.Range(-1000f, 1000f));
        var speed = Random.Range(0f, 3f);


        transform.localPosition = pos;
        transform.LookAt(look);
        velocity = (look - pos) * speed;
    }

    private void Update()
    {
        //bat is dead
        if (batGeom && batGeom.activeSelf == false) Destroy(transform.gameObject);

        if (isFlocking)
        {
            constrainPoint = flock.position; //flock follows player

            var newVelocity = new Vector3(0, 0, 0);
            // rule 1 all boids steer towards center of mass - cohesion
            newVelocity += cohesion() * cohesionFactor;

            // rule 2 all boids steer away from each other - avoidance        
            newVelocity += separation() * separationFactor;

            // rule 3 all boids match velocity - alignment
            newVelocity += align() * allignFactor;

            newVelocity += constrain() * constrainFactor;

            newVelocity += avoid() * avoidFactor;

            var slerpVelo = Vector3.Slerp(velocity, newVelocity, Time.deltaTime * integrationRate);

            velocity = slerpVelo.normalized;

            transform.position += velocity * (Time.deltaTime * speed);
            transform.LookAt(transform.position + velocity);
        }
        else
        if (target)
        {
            Debug.Log("Attacking");

            //if not flocking, its going for a target, usually attacking
            var newVelocity = target.position - transform.position;

            var slerpVelo = Vector3.Slerp(newVelocity, velocity, Time.deltaTime * integrationRate);

            velocity = slerpVelo.normalized;

            transform.position += velocity * (Time.deltaTime * speed);
            transform.LookAt(transform.position + velocity);

            if (Vector3.Distance(transform.position, target.position) < 0.3f)
            {
                //Attack successfull, do damage, fly away
                Debug.Log("Hit Target");
                isFlocking = true;
            }
        }
    }

    private Vector3 avoid()
    {
        if (avoidCount > 0) return (avoidObst / avoidCount).normalized;

        return Vector3.zero;
    }

    private Vector3 constrain()
    {
        var steer = new Vector3(0, 0, 0);

        steer += constrainPoint - transform.position;

        steer.Normalize();

        return steer;
    }

    private Vector3 cohesion()
    {
        var steer = new Vector3(0, 0, 0);

        var sibs = 0; //count the boids, it might change

        foreach (Transform boid in flock)
            if (boid != transform)
            {
                steer += boid.transform.position;
                sibs++;
            }

        steer /= sibs; //center of mass is the average position of all        

        steer -= transform.position;

        steer.Normalize();


        return steer;
    }

    private Vector3 separation()
    {
        var steer = new Vector3(0, 0, 0);

        var sibs = 0;


        foreach (Transform boid in flock)
            // if boid is not itself
            if (boid != transform)
                // if this boids position is within the collision distance of a neighbouring boid
                if ((transform.position - boid.transform.position).magnitude < collisionDistance)
                {
                    // our vector becomes this boids pos - neighbouring boids pos
                    steer += transform.position - boid.transform.position;
                    sibs++;
                }

        steer /= sibs;
        steer.Normalize(); //unit, just direction
        return steer;
    }

    private Vector3 align()
    {
        var steer = new Vector3(0, 0, 0);
        var sibs = 0;
        foreach (Transform boid in flock)
        {
            if (boid == transform) continue;
            steer += boid.GetComponent<Boids>().velocity;
            sibs++;
        }

        steer /= sibs;

        steer.Normalize();

        return steer;
    }

    public void accumAvoid(Vector3 avoid)
    {
        avoidObst += transform.position - avoid;
        avoidCount++;
    }

    public void resetAvoid()
    {
        avoidCount = 0;
        avoidObst *= 0;
    }
}