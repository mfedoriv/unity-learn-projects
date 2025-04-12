using System;
using UnityEngine;

namespace InteractionSystem
{
    public class TestInteractable : MonoBehaviour
    {
        private Interact _triggerFromInteraction;
        
        private void Awake()
        {
            _triggerFromInteraction = GetComponent<Interact>();
        }


        private void OnEnable()
        {
            if (_triggerFromInteraction)
            {
                _triggerFromInteraction.GetInteractEvent.HasInteracted += Boink;
            }
        }

        private void OnDisable()
        {
            if (_triggerFromInteraction)
            {
                _triggerFromInteraction.GetInteractEvent.HasInteracted -= Boink;
            }
        }

        private void Boink()
        {
            Debug.Log("Boink!");
        }
    }
}