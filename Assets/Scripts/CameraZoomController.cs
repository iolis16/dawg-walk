using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

namespace DawgWalk
{
    [RequireComponent(typeof(CinemachineOrbitalFollow))]
    public class CameraZoomController : MonoBehaviour
    {
        [SerializeField] float zoomSensitivity = 0.05f;
        [SerializeField] float minValue = 2f;
        [SerializeField] float maxValue = 150f;

        CinemachineOrbitalFollow orbitalFollow;

        void Awake()
        {
            orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
        }

        void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            float scroll = mouse.scroll.ReadValue().y;
            if (Mathf.Approximately(scroll, 0f)) return;

            var axis = orbitalFollow.RadialAxis;
            axis.Value = Mathf.Clamp(axis.Value - scroll * zoomSensitivity, minValue, maxValue);
            orbitalFollow.RadialAxis = axis;
        }
    }
}