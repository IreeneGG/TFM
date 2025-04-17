using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    //[SerializeField] private GameObject block;
    [SerializeField] private MyAgent agent;
    [SerializeField] private GameObject wall; // Referencia al objeto wall
    [SerializeField] private GameObject goal; // Referencia al objeto goal

    private bool touchingGoal = false;
    private bool touchingWall = false;

    private bool activarEntrenamiento2 = false;
 private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == goal)
        {
            touchingGoal = true;
            //Debug.Log("Tocando el GOAL");
            CheckBothTouched();
        }

        if (collision.gameObject == wall)
        {
            touchingWall = true;
            //Debug.Log("Tocando la PARED");
            CheckBothTouched();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == goal)
        {
            touchingGoal = false;
            //Debug.Log("Ya no toca el GOAL");
        }

        if (collision.gameObject == wall)
        {
            touchingWall = false;
            //Debug.Log("Ya no toca la PARED");
        }
    }
    
    private void CheckBothTouched()
    {
        if (touchingGoal && touchingWall)
        {
            Debug.Log("Tocando GOAL y PARED al mismo tiempo");

            agent.AddReward(100f);
            Debug.Log("Recompensa de 100 otorgada.");

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
            // Opcional: resetear para que no se dispare más de una vez
            touchingGoal = false;
            touchingWall = false;
        }
    }
    
    
}
