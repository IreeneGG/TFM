using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(Rigidbody))]


public class Antagonist : Agent
{
    public float moveSpeed = 5f;
    public float turnSpeed = 300f;
    public float jumpForce = 8f;

    private Rigidbody rb;
    private bool isGrounded;

    [SerializeField] private GameObject block; // Referencia al objeto block
    [SerializeField] private GameObject goal; // Referencia al objeto block

    [SerializeField] private Transform initialAgentPosition; // Para almacenar la posición inicial del agente
   



    private bool canAct = false;
    private bool disableScheduled = false;
    private int stepsUntilDisable = 0;

        public override void CollectObservations(VectorSensor sensor)
    {
        //Debug.Log("Collecting observations...");

        // Agregar la posición (x, y, z) del agente
        sensor.AddObservation(transform.position);  // [x, y, z] del agente
        //Debug.Log($"Agent position: {transform.position}");

        // Agregar la posición (x, y, z) del bloque
        sensor.AddObservation(block.transform.position);  // [x, y, z] del bloque
        //Debug.Log($"Block position: {block.transform.position}");

        sensor.AddObservation(Vector3.Distance(goal.transform.position, block.transform.position));
    }

    
    
    public override void OnEpisodeBegin()
    {
        transform.position = initialAgentPosition.position;
        transform.rotation = initialAgentPosition.rotation;

        // Aquí se reinicia el agente y lo habilitamos para actuar
        canAct = true;
        disableScheduled = false;
        stepsUntilDisable = 0;

        Rigidbody agentRb = GetComponent<Rigidbody>();
        if (agentRb != null)
        {
            agentRb.velocity = Vector3.zero;
            agentRb.angularVelocity = Vector3.zero;
        }


        float distanceToBlock = Vector3.Distance(goal.transform.position, block.transform.position) - 5f; // Ajusta el valor 5f según sea necesario; 

        float distancePenalty = distanceToBlock * 0.01f;
        AddReward(distancePenalty);
    }


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Raycast desde el centro hacia abajo para detectar si está tocando el suelo
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.1f);
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

        int move = actions.DiscreteActions[0];   // 0 = no move, 1 = forward
        int rotate = actions.DiscreteActions[1]; // 0 = no turn, 1 = left, 2 = right
        int jump = actions.DiscreteActions[2];   // 0 = no jump, 1 = jump

        // Movimiento hacia adelante
        if (move == 1)
        {
            Vector3 forwardMove = transform.forward * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardMove);
        }

        // Rotación
        if (rotate == 1)
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.fixedDeltaTime);
        }
        else if (rotate == 2)
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.fixedDeltaTime);
        }

        // Salto
        if (jump == 1 && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        //RECOMPENSAS dinámicas por distancia al bloque
        float distanceToBlock = Vector3.Distance(goal.transform.position, block.transform.position) - 5f; // Ajusta el valor 5f según sea necesario; 

        //Debug.Log($"Distance to block: {distanceToBlock * -0.01f}");
        // Recompensar por acercarse al bloque (cerca = más recompensa)

        // Penalizar con base en la distancia (lejos = más penalización)
        float distancePenalty = distanceToBlock * 0.01f;
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
        discreteActions[0] = Input.GetKey(KeyCode.W) ? 1 : 0; // Forward
        discreteActions[1] = Input.GetKey(KeyCode.A) ? 1 : Input.GetKey(KeyCode.D) ? 2 : 0; // Turn
        discreteActions[2] = Input.GetKey(KeyCode.Space) ? 1 : 0; // Jump
    }
}
