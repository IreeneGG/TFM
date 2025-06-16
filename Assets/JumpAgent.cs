using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(Rigidbody))]
public class JumpAgent : Agent
{
    public float moveSpeed = 5f;
    public float turnSpeed = 200f;
    public float jumpForce = 120f;
    public float jumpCooldown = 0.5f;

    private float bestDistanceToGoal;

    private Rigidbody rb;
    private bool isGrounded;
    private float lastJumpTime = -999f;
    public int teamId;
    [SerializeField] private GameObject goal;
    [SerializeField] private GameObject groundObject;
    [SerializeField] private GameObject groundObjectBlock;
    [SerializeField] private Transform initialAgentPosition;


    [SerializeField] private MyAgent amigo;



    private bool canAct = false;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public override void OnEpisodeBegin()
    {
        canAct = true;
        transform.position = initialAgentPosition.position;
        transform.rotation = initialAgentPosition.rotation;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        lastJumpTime = -999f;

        bestDistanceToGoal = Vector3.Distance(transform.position, goal.transform.position);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //if (!canAct) return;
        sensor.AddObservation(transform.position);            // Agente
        sensor.AddObservation(amigo.transform.position);//conocer observaciones del amigo
        sensor.AddObservation(groundObjectBlock.transform.position);            // BLOCK 
        sensor.AddObservation(goal.transform.position);       // Objetivo
        sensor.AddObservation(teamId);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject == groundObject || collision.gameObject == groundObjectBlock)
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
        if (collision.gameObject == groundObject || collision.gameObject == groundObjectBlock)
        {
            isGrounded = false;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!canAct) return;

        int move = actions.DiscreteActions[0];   // 0 = no move, 1 = forward
        int rotate = actions.DiscreteActions[1]; // 0 = no turn, 1 = left, 2 = right
        int jump = actions.DiscreteActions[2];   // 0 = no jump, 1 = jump

        Vector3 currentVelocity = rb.velocity;

        // Movimiento hacia adelante
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

        // Rotación
        if (rotate == 1)
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.fixedDeltaTime);
        }
        else if (rotate == 2)
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.fixedDeltaTime);
        }

        // Salto con cooldown
        if (jump == 1 && isGrounded && Time.time - lastJumpTime > jumpCooldown)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            lastJumpTime = Time.time;
        }

        float currentDistance = Vector3.Distance(transform.position, goal.transform.position);

        if (currentDistance < bestDistanceToGoal)
        {
            float improvement = bestDistanceToGoal - currentDistance;
            AddReward(improvement * 0.01f); 
            bestDistanceToGoal = currentDistance;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
        discreteActions[1] = Input.GetKey(KeyCode.LeftArrow) ? 1 :
                             Input.GetKey(KeyCode.RightArrow) ? 2 : 0;
        discreteActions[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
}
