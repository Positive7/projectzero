using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace TinyWorlds
{
    /// <summary>
    /// Vékony réteg a bemenet fölött: az új Input Systemmel és a régi Input Managerrel is működik.
    /// </summary>
    public static class PointerInput
    {
#if ENABLE_INPUT_SYSTEM
        public static Vector2 Position      => Pointer.current != null ? Pointer.current.position.ReadValue() : Vector2.zero;
        public static Vector2 Delta         => Pointer.current != null ? Pointer.current.delta.ReadValue()    : Vector2.zero;
        public static bool    PrimaryDown   => Pointer.current != null && Pointer.current.press.wasPressedThisFrame;
        public static bool    PrimaryUp     => Pointer.current != null && Pointer.current.press.wasReleasedThisFrame;
        public static bool    PrimaryHeld   => Pointer.current != null && Pointer.current.press.isPressed;
        public static bool    SecondaryDown => Mouse.current   != null && Mouse.current.rightButton.wasPressedThisFrame;
        public static float   Scroll        => Mouse.current   != null ? Mathf.Sign(Mouse.current.scroll.ReadValue().y) * (Mouse.current.scroll.ReadValue().y != 0 ? 1 : 0) : 0f;
#else
        public static Vector2 Position      => Input.mousePosition;
        public static Vector2 Delta         => new(Input.GetAxis("Mouse X") * 10f, Input.GetAxis("Mouse Y") * 10f);
        public static bool    PrimaryDown   => Input.GetMouseButtonDown(0);
        public static bool    PrimaryUp     => Input.GetMouseButtonUp(0);
        public static bool    PrimaryHeld   => Input.GetMouseButton(0);
        public static bool    SecondaryDown => Input.GetMouseButtonDown(1);
        public static float   Scroll        => Mathf.Sign(Input.mouseScrollDelta.y) * (Input.mouseScrollDelta.y != 0 ? 1 : 0);
#endif
    }
}
