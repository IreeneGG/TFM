using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetectorJumping : MonoBehaviour
{
    [SerializeField] private GameObject agente;  // El objetivo con el que se mide la distancia
    [SerializeField] private JumpAgent agent;  // El agente que se mueve

    [SerializeField] private float distanceThreshold = 5f;  // Distancia umbral para detectar el goal

    void Update()
    {
        // Calcular la distancia entre el agente y el goal
        float distance = Vector3.Distance(agente.transform.position, transform.position);


        Debug.Log($"Distancia entre el agente y el goal: {distance}");
        // Verificar si la distancia es menor que el umbral
        if (distance < distanceThreshold)
        {
            Debug.Log("GOAL alcanzado!");
            agent.AddReward(100f);  // Asumiendo que "agent" es el agente que maneja recompensas
            agent.EndEpisode();
        }
    }
}
