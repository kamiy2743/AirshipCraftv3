using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input;

namespace Player
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;

        void Start()
        {
            var playerInput = new PlayerInputActions();
            playerInput.Enable();
        }

        void Update()
        {
            transform.position += InputProvider.DebugMove() * moveSpeed * Time.deltaTime;
        }
    }
}
