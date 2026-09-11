using UnityEngine;
using UnityEngine.InputSystem;

namespace DawgWalk
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float sprintMultiplier = 1.6f;
        [SerializeField] float turnSmoothTime = 0.1f;
        [SerializeField] float gravity = -15f;
        [SerializeField] float jumpHeight = 1.2f;

        CharacterController controller;
        Camera mainCamera;
        float turnSmoothVelocity;
        float verticalVelocity;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            mainCamera = Camera.main;
        }

        void Update()
        {
            Vector2 moveInput = ReadMoveInput();
            Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            bool grounded = controller.isGrounded;
            if (grounded && verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (inputDir.magnitude >= 0.1f)
            {
                float cameraYaw = mainCamera != null ? mainCamera.transform.eulerAngles.y : 0f;
                float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraYaw;
                float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                float speed = moveSpeed * (IsSprinting() ? sprintMultiplier : 1f);
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
            }

            if (grounded && IsJumpPressed())
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            verticalVelocity += gravity * Time.deltaTime;
            controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
        }

        Vector2 ReadMoveInput()
        {
            Vector2 input = Vector2.zero;
            var kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.wKey.isPressed) input.y += 1f;
                if (kb.sKey.isPressed) input.y -= 1f;
                if (kb.dKey.isPressed) input.x += 1f;
                if (kb.aKey.isPressed) input.x -= 1f;
            }

            var gp = Gamepad.current;
            if (gp != null)
                input += gp.leftStick.ReadValue();

            return Vector2.ClampMagnitude(input, 1f);
        }

        bool IsSprinting()
        {
            if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed) return true;
            if (Gamepad.current != null && Gamepad.current.leftStickButton.isPressed) return true;
            return false;
        }

        bool IsJumpPressed()
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) return true;
            if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) return true;
            return false;
        }
    }
}