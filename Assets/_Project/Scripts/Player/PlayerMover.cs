using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input;
using Cinemachine;

namespace Player
{
    internal class PlayerMover : MonoBehaviour
    {
        [SerializeField] private PlayerCamera playerCamera;
        [Space(20)]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float jumpForce;
        [SerializeField] private float gravity;
        [Space(20)]
        [SerializeField] private float debugMoveSpeed;

        private Rigidbody rigidbody;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            var moveVector = InputProvider.Move() * moveSpeed;
            moveVector.y = rigidbody.velocity.y;
            rigidbody.velocity = playerCamera.HorizontalRotation * moveVector;

            if (InputProvider.Jump())
            {
                Debug.Log("jump");
                rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }

            rigidbody.AddForce(Vector3.down * gravity);
        }

        private void DebugMove()
        {
            transform.position += playerCamera.HorizontalRotation * (InputProvider.DebugMove() * debugMoveSpeed * Time.deltaTime);
        }
    }
}
