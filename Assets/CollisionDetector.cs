using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private MyAgent agent;       // El agente ML
    [SerializeField] private GameObject goal;     // Objeto objetivo
    [SerializeField] private GameObject cube;     // El bloque que debe tocar el goal

    private void OnCollisionEnter(Collision collision)
    {
        // Verificamos que quien colisiona sea el cubo con el goal
        if ((gameObject == cube && collision.gameObject == goal) ||
            (gameObject == goal && collision.gameObject == cube))
        {
            Debug.Log("✅ ¡El cubo colisionó con el goal!");

            if (agent != null)
            {
                agent.AddReward(1.0f); // Recompensa positiva
                agent.EndEpisode();    // Terminar episodio
            }
        }
    }
}
