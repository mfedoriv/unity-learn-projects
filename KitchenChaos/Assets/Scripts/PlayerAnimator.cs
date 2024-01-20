using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Player player;
    
    private Animator _animator;
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.SetBool(IsWalking, player.IsWalking());
    }

    private void Update()
    {
        _animator.SetBool(IsWalking, player.IsWalking());
    }
}
