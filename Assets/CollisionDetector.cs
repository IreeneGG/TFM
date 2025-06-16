using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    //[SerializeField] private GameObject block;
    [SerializeField] private MyAgent agent1;
    // [SerializeField] private JumpAgent agent2;
    [SerializeField] private MyAgent agent3;
    // [SerializeField] private JumpAgent agent4;
    [SerializeField] private GameObject wall1; // Referencia al objeto wall
    [SerializeField] private GameObject goal1; // Referencia al objeto goal
    [SerializeField] private GameObject wall2; // Referencia al objeto wall
    [SerializeField] private GameObject goal2; // Referencia al objeto goal
   // [SerializeField] private GameObject jumpAgent; // Referencia al objeto jumpAgent
   


    private bool touchingGoal1 = false;
    private bool touchingWall1 = false;

    private bool touchingGoal2 = false;
    private bool touchingWall2 = false;


    private bool finishInTouch = false; 

    private bool activarEntrenamiento2 = true;
    


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == goal1)
        {
            touchingGoal1 = true;
            //Debug.Log("Tocando el GOAL1");
            CheckContact(agent1, ref touchingWall1, ref touchingGoal1, "Grupo 0");
        }

        if (collision.gameObject == wall1)
        {
            touchingWall1 = true;
            //Debug.Log("Tocando la PARED1");
            CheckContact(agent1, ref touchingWall1, ref touchingGoal1, "Grupo 0");
        }
        if (collision.gameObject == goal2)
        {
            touchingGoal2 = true;
            //Debug.Log("Tocando el GOAL2");
            CheckContact(agent3, ref touchingWall2, ref touchingGoal2, "Grupo 1");
        }

        if (collision.gameObject == wall2)
        {
            touchingWall2 = true;
            //Debug.Log("Tocando la PARED2");
            CheckContact(agent3, ref touchingWall2, ref touchingGoal2, "Grupo 1");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == wall1) touchingWall1 = false;
        if (collision.gameObject == goal1) touchingGoal1 = false;
        if (collision.gameObject == wall2) touchingWall2 = false;
        if (collision.gameObject == goal2) touchingGoal2 = false;
    }
    
    private void CheckContact(MyAgent agent, ref bool touchingWall, ref bool touchingGoal, string grupo)
    {
        if (touchingWall && touchingGoal )
        {
            Debug.Log($"Tocando WALL y GOAL al mismo tiempo - {grupo}");
            agent.AddReward(100f);
            Debug.Log($"Recompensa otorgada a {grupo}");
              

            // Reset para evitar múltiples recompensas
            touchingWall = false;
            touchingGoal = false;

            // agent1.EndEpisode();
            // agent2.EndEpisode();
              
            // agent3.EndEpisode();
            // agent4.EndEpisode();
       

        }
    }
   
    
}
