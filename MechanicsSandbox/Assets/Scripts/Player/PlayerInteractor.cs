using InteractionSystem;
using UnityEngine;

namespace Player
{
    public class PlayerInteractor : MonoBehaviour
    {
        public void PlayerInteract()
        {
            int layermask0 = 1 << 0;
            int layermask3 = 1 << 3;
            int finalmask = layermask0 | layermask3;

            RaycastHit hit;
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            
            if (Physics.Raycast(ray, out hit, 15, finalmask))
            {
                Debug.Log("Interaction");
                Interact interactScript = hit.transform.GetComponent<Interact>();
                // Debug.Log(interactScript);
                if (interactScript) interactScript.CallInteract(this);
            }
            else
            {
                Debug.Log("No Interaction");
            }
        }
    }
}
