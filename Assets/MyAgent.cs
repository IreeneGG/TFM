using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(Rigidbody))]
public class MyAgent : Agent
{
    public float moveSpeed = 5f;
    public float turnSpeed = 300f;
    public float jumpForce = 8f;

    private Rigidbody rb;
    private bool isGrounded;
    public int teamId;
    [SerializeField] private GameObject block;
    [SerializeField] private GameObject goal;
    [SerializeField] private GameObject spawnPositionsContainer;
    private Transform[] spawnPositions;

    [SerializeField] private Transform initialAgentPosition;
    [SerializeField] private Transform initialBlockPosition;
    [SerializeField] private JumpAgent amigo;

    private bool canAct = false;
    private bool disableScheduled = false;
    private int stepsUntilDisable = 0;

    public bool printVar = false;

    private float previousDistanceToGoal;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public override void OnEpisodeBegin()
    {
        spawnPositions = spawnPositionsContainer.GetComponentsInChildren<Transform>();
        canAct = true;
        disableScheduled = false;
        stepsUntilDisable = 0;

        if (spawnPositions.Length > 0)
        {
            transform.position = initialAgentPosition.position;
            transform.rotation = initialAgentPosition.rotation;

            Transform randomSpawnPoint = GetRandomSpawnPoint();
            block.transform.position = randomSpawnPoint.position;
            block.transform.rotation = randomSpawnPoint.rotation;
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Rigidbody blockRb = block.GetComponent<Rigidbody>();
        if (blockRb != null)
        {
            blockRb.velocity = Vector3.zero;
            blockRb.angularVelocity = Vector3.zero;
        }

        //float distanceToBlock = Vector3.Distance(goal.transform.position, block.transform.position) - 5f;
        //AddReward(distanceToBlock * -0.01f);�

        previousDistanceToGoal = Vector3.Distance(goal.transform.position, block.transform.position);
    }

    private Transform GetRandomSpawnPoint()
    {
        int randomIndex = Random.Range(1, spawnPositions.Length);
        return spawnPositions[randomIndex];
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    isGrounded = true;
                    return;
                }
            }
        }
        isGrounded = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = false;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(block.transform.position);
        sensor.AddObservation(amigo.transform.position); //conocer observaciones del amigo
        sensor.AddObservation(Vector3.Distance(goal.transform.position, block.transform.position));
        sensor.AddObservation(Vector3.Distance(transform.position, block.transform.position));
        sensor.AddObservation(teamId);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!canAct) return;

        if (disableScheduled)
        {
            stepsUntilDisable--;
            if (stepsUntilDisable <= 0)
            {
                canAct = false;
                disableScheduled = false;
            }
        }

        int move = actions.DiscreteActions[0];
        int rotate = actions.DiscreteActions[1];
        int jump = actions.DiscreteActions[2];

        Vector3 currentVelocity = rb.velocity;
        //sss
        if (move == 1)
        {
            Vector3 forwardVelocity = transform.forward * moveSpeed;
            currentVelocity.x = forwardVelocity.x;
            currentVelocity.z = forwardVelocity.z;
        }
        else
        {
            currentVelocity.x = 0f;
            currentVelocity.z = 0f;
        }

        rb.velocity = new Vector3(currentVelocity.x, rb.velocity.y, currentVelocity.z);

        if (rotate == 1)
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.fixedDeltaTime);
        }
        else if (rotate == 2)
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.fixedDeltaTime);
        }

        if (jump == 1 && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        float currentDistance = Vector3.Distance(goal.transform.position, block.transform.position);

        if (currentDistance < previousDistanceToGoal)
        {
            float improvement = previousDistanceToGoal - currentDistance;
            AddReward(improvement * 10); // Solo recompensa si mejora la mejor marca
            if (printVar) { Debug.Log(improvement); }

            previousDistanceToGoal = currentDistance; // Actualiza solo si mejora
        }

    }

    public void DisableAgentTemporarily(int delaySteps = 10)
    {
        disableScheduled = true;
        stepsUntilDisable = delaySteps;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.W) ? 1 : 0;
        discreteActions[1] = Input.GetKey(KeyCode.A) ? 1 :
                             Input.GetKey(KeyCode.D) ? 2 : 0;
        discreteActions[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
}
