using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector2 : MonoBehaviour
{
    [SerializeField] private GameObject agente; //es el bloque
    [SerializeField] private MyAgent myagenteRECOMPENSA1;
    [SerializeField] private JumpAgent jumpagentRECOMPENSA2;
   
    [SerializeField] private MyAgent myagenteNORECOMPENSA1;
    [SerializeField] private JumpAgent jumpagentNORECOMPENSA2;
 

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == agente)
        {
            Debug.Log("GOAL FINAL!");
            LogToFile.Write("GOAL FINAL!"); // <== NUEVO

            myagenteRECOMPENSA1.AddReward(100f);
            jumpagentRECOMPENSA2.AddReward(100f);
            

            //myagenteNORECOMPENSA1.AddReward(-100f);
            //myagenteNORECOMPENSA2.AddReward(-100f);
            //myagenteNORECOMPENSA3.AddReward(-100f);

            //myagenteNORECOMPENSA1.EndEpisode();
            //myagenteNORECOMPENSA2.EndEpisode();
            //myagenteNORECOMPENSA3.EndEpisode();

            myagenteRECOMPENSA1.EndEpisode();
            jumpagentRECOMPENSA2.EndEpisode();
           

            myagenteNORECOMPENSA1.EndEpisode();
            jumpagentNORECOMPENSA2.EndEpisode();
           
            //jumpagenteRECOMPENSA.AddReward(50f);

            //jumpagenteNORECOMPENSA.EndEpisode();
            //jumpagenteRECOMPENSA.EndEpisode();
            //myagenteNORECOMPENSA.EndEpisode();
            //myagenteRECOMPENSA.EndEpisode();
        }
    }
}
