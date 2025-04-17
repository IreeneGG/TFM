using UnityEngine;

public class Controlador : MonoBehaviour
{
    [SerializeField] private GameObject block; // Referencia al objeto block
    [SerializeField] private GameObject goal;  // Referencia al objeto goal

    public MyAgent agent;

    void Update()
    {
        // Verificamos si el "block" y el "goal" están tocando
        if (block != null && goal != null)
        {
             float distancia = Vector3.Distance(block.transform.position, goal.transform.position);
            //Debug.Log("Distancia entre block y goal: " + distancia);
             
            // Usamos OnCollisionEnter si los objetos tienen colliders normales
            Collider blockCollider = block.GetComponent<Collider>();
            Collider goalCollider = goal.GetComponent<Collider>();

            if (blockCollider != null && goalCollider != null)
            {
                // Comprobamos si los dos colliders están tocando
                if (blockCollider.bounds.Intersects(goalCollider.bounds))
                {
                    Debug.Log("GOAL!");
                    agent.AddReward(100f); // recompensa al agente
                    agent.EndEpisode();  // Termina el episodio
                }
            }
        }
    }
}
