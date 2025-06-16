using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(Rigidbody))]
public class MyAgent2 : Agent
{
    public float moveSpeed = 5f;
    public float turnSpeed = 300f;
    public float jumpForce = 8f;

    private Rigidbody rb;
    private bool isGrounded;

    [SerializeField] private GameObject block;
    [SerializeField] private GameObject goal;
    [SerializeField] private GameObject spawnPositionsContainer;
    private Transform[] spawnPositions;

    [SerializeField] private Transform initialAgentPosition;
    [SerializeField] private Transform initialBlockPosition;

    private bool canAct = false;
    private bool disableScheduled = false;
    private int stepsUntilDisable = 0;

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(block.transform.position);
        sensor.AddObservation(Vector3.Distance(goal.transform.position, block.transform.position));
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

        Rigidbody agentRb = GetComponent<Rigidbody>();
        if (agentRb != null)
        {
            agentRb.velocity = Vector3.zero;
            agentRb.angularVelocity = Vector3.zero;
        }

        Rigidbody blockRb = block.GetComponent<Rigidbody>();
        if (blockRb != null)
        {
            blockRb.velocity = Vector3.zero;
            blockRb.angularVelocity = Vector3.zero;
        }

        float distanceToBlock = Vector3.Distance(goal.transform.position, block.transform.position) - 5f;
        float distancePenalty = distanceToBlock * -0.01f;
        AddReward(distancePenalty);
    }

    private Transform GetRandomSpawnPoint()
    {
        int randomIndex = Random.Range(1, spawnPositions.Length);
        return spawnPositions[randomIndex];
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.1f);

        // Check if block has reached the goal
        float distanceToGoal = Vector3.Distance(block.transform.position, goal.transform.position);
        if (distanceToGoal < 1.0f) // Puedes ajustar el umbral
        {
            AddReward(1.0f); // Recompensa por éxito
            EndEpisode();
        }
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

        if (move == 1)
        {
            Vector3 forwardMove = transform.forward * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardMove);
        }

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

        float distanceToBlock = Vector3.Distance(goal.transform.position, block.transform.position) - 5f;
        float distancePenalty = distanceToBlock * -0.01f;
        AddReward(distancePenalty);
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
        discreteActions[1] = Input.GetKey(KeyCode.A) ? 1 : Input.GetKey(KeyCode.D) ? 2 : 0;
        discreteActions[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
}
