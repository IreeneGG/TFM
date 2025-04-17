using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector2 : MonoBehaviour
{
    [SerializeField] private GameObject block;
    [SerializeField] private MyAgent agent1;
    [SerializeField] private JumpAgent agent2;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == block)
        {
            Debug.Log("GOAL!");
            agent2.AddReward(100f);
            agent1.EndEpisode();
            agent2.EndEpisode();
        }
    }
}
