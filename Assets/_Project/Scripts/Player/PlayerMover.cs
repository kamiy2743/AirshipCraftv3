using UnityEngine;
using Input;

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
        [Space(20)]
        [SerializeField] private bool isDebugMode;

        new private Rigidbody rigidbody;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (isDebugMode)
            {
                DebugMove();
            }
            else
            {
                Move();
            }
        }

        private void Move()
        {
            var moveVector = InputProvider.Move() * moveSpeed;
            moveVector.y = rigidbody.velocity.y;
            rigidbody.velocity = playerCamera.HorizontalRotation * moveVector;

            if (InputProvider.Jump())
            {
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
