using Player;
using UnityEngine;

namespace InteractionSystem
{
    public class Interact : MonoBehaviour
    {
        private InteractEvent interact = new InteractEvent();
        private PlayerInteractor player;

        public InteractEvent GetInteractEvent
        {
            get
            {
                if (interact == null) interact = new InteractEvent();
                return interact;
            }
        }

        public PlayerInteractor GetPlayer
        {
            get
            {
                return player;
            }
        }

        public void CallInteract(PlayerInteractor interactedPlayer)
        {
            player = interactedPlayer;
            interact.CallInteractEvent();
        }
        
        
    }
    

    public class InteractEvent
    {
        public delegate void InteractHandler();

        public event InteractHandler HasInteracted;

        public void CallInteractEvent() => HasInteracted?.Invoke();
    }
}