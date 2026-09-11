using UnityEngine;
using UnityEngine.InputSystem;

namespace DawgWalk
{
    public class CursorLock : MonoBehaviour
    {
        void Start()
        {
            Lock();
        }

        void Update()
        {
            var kb = Keyboard.current;
            var mouse = Mouse.current;

            if (kb != null && kb.escapeKey.wasPressedThisFrame)
                Unlock();
            else if (Cursor.lockState != CursorLockMode.Locked && mouse != null && mouse.leftButton.wasPressedThisFrame)
                Lock();
        }

        void Lock()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Unlock()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}