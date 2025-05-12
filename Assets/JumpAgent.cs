using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(Rigidbody))]


public class JumpAgent : Agent
{
    public float moveSpeed = 5f;
    public float turnSpeed = 300f;
    public float jumpForce = 8f;

    private Rigidbody rb;
    private bool isGrounded;

    [SerializeField] private GameObject goal; // Referencia al objeto block
    private Transform[] spawnPositions; // Array de las posiciones de spawn
    


    [SerializeField] private Transform initialAgentPosition; // Para almacenar la posición inicial del agente

    private bool canAct = false;

    // public void EnableAgent()
    // {
    //     canAct = true;
    // }

    public override void CollectObservations(VectorSensor sensor)
    {
        
        if (!canAct) return;
        // Agregar la posición (x, y, z) del agente
        sensor.AddObservation(transform.position);  // [x, y, z] del agente
        //Debug.Log($"Agent position: {transform.position}");

        // Agregar la posición (x, y, z) del bloque
        sensor.AddObservation(goal.transform.position);  // [x, y, z] del bloque
        //Debug.Log($"Block position: {goal.transform.position}");
    } 
    
    
    public override void OnEpisodeBegin()
    {
        canAct = true; // Habilitar el agente al inicio del episodio
        //canAct = false;
        // Restablecer la posición del agente a su posición inicial
        transform.position = initialAgentPosition.position;
        transform.rotation = initialAgentPosition.rotation;


        // restablecer las velocidades del Rigidbody 
        Rigidbody agentRb = GetComponent<Rigidbody>();
        if (agentRb != null)
        {
            agentRb.velocity = Vector3.zero;
            agentRb.angularVelocity = Vector3.zero;
        }

 
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





    //  public override void OnActionReceived(ActionBuffers actions)
    // {
    //     int move = actions.DiscreteActions[0];   // 0 = no move, 1 = forward
    //     int rotate = actions.DiscreteActions[1]; // 0 = no turn, 1 = left, 2 = right
    //     int jump = actions.DiscreteActions[2];   // 0 = no jump, 1 = jump

    //     // Movimiento hacia adelante
    //     if (move == 1)
    //     {
    //         Vector3 forwardMove = transform.forward * moveSpeed * Time.fixedDeltaTime;
    //         rb.MovePosition(rb.position + forwardMove);
    //     }

    //     // Rotación
    //     if (rotate == 1)
    //     {
    //         transform.Rotate(Vector3.up, -turnSpeed * Time.fixedDeltaTime);
    //     }
    //     else if (rotate == 2)
    //     {
    //         transform.Rotate(Vector3.up, turnSpeed * Time.fixedDeltaTime);
    //     }

    //     // Salto
    //     if (jump == 1 && isGrounded)
    //     {
    //         rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    //     }

    
    //     AddReward(0);
    // }
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!canAct) return;

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

        // Calcular la distancia entre el agente y el goal
        float distanceToGoal = Vector3.Distance(transform.position, goal.transform.position);

        // Recompensa negativa (más lejos del goal, más negativa la recompensa)
        // Restamos la distancia más una penalización constante de -0.01
        float reward = -distanceToGoal * 0.01f;

        //Debug.Log($"Distancia al objetivo: {distanceToGoal}, Recompensa calculada: {reward}");

        // Aplicar la recompensa calculada
        AddReward(reward);
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;

        // Flecha arriba = avanzar
        discreteActions[0] = Input.GetKey(KeyCode.UpArrow) ? 1 : 0;

        // Giro con flechas izquierda/derecha
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActions[1] = 1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActions[1] = 2;
        }
        else
        {
            discreteActions[1] = 0;
        }

        // Espacio para saltar
        discreteActions[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

}
