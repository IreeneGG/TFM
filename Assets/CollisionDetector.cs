using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private GameObject block;
    [SerializeField] private MyAgent agent;

    private bool activarEntrenamiento2 = false; 

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == block)
        {
            Debug.Log("GOAL!");
            agent.AddReward(100f);

            if (activarEntrenamiento2 == true)
            {
                Debug.Log("Entrenamiento 2 activado. No acaba el episodio ");
                agent.DisableAgentTemporarily(35); 
            }
            else
            {
                // Si no se activa el entrenamiento, puedes agregar otra lógica aquí si es necesario.
                Debug.Log("Entrenamiento 2 no activado.");
                agent.EndEpisode();
            }
            //agent.EndEpisode();
        }
    }
}
