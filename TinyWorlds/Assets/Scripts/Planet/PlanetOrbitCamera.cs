using UnityEngine;

namespace TinyWorlds
{
    /// <summary>
    /// Bal gombos húzással a bolygó körül forog (bármerre, nincs pólus-zár), görgővel zoomol.
    /// </summary>
    public class PlanetOrbitCamera : MonoBehaviour
    {
        public HexPlanet planet;
        public float     distance    = 30f;
        public float     minDistance = 14f, maxDistance = 60f;
        public float     rotateSpeed = 0.25f;
        public float     zoomStep    = 3f;
        public float     smoothing   = 10f;

        /// <summary>Igaz, ha az aktuális lenyomás húzásnak számít (a BuildingPlacer ilyenkor nem épít).</summary>
        public bool IsDragging { get; private set; }

        private Quaternion orbit = Quaternion.identity;
        private float      targetDistance;
        private Vector2    pressPos;
        private const float DragThreshold = 8f;

        private void Start()
        {
            targetDistance = distance;
            if (planet == null) planet = FindAnyObjectByType<HexPlanet>();
            orbit = Quaternion.LookRotation(Center - transform.position);
        }

        private Vector3 Center => planet != null ? planet.transform.position : Vector3.zero;

        private void LateUpdate()
        {
            if (PointerInput.PrimaryDown) { pressPos = PointerInput.Position; IsDragging = false; }

            if (PointerInput.PrimaryHeld)
            {
                if (!IsDragging && (PointerInput.Position - pressPos).magnitude > DragThreshold) IsDragging = true;
                if (IsDragging)
                {
                    Vector2 d = PointerInput.Delta * rotateSpeed;
                    // A kamera saját tengelyei körül forgatunk, így nincs gimbal lock a pólusoknál.
                    orbit = Quaternion.AngleAxis(d.x, orbit * Vector3.up) * Quaternion.AngleAxis(-d.y, orbit * Vector3.right) * orbit;
                }
            }

            float scroll = PointerInput.Scroll;
            if (scroll != 0f) targetDistance = Mathf.Clamp(targetDistance - scroll * zoomStep, minDistance, maxDistance);
            distance = Mathf.Lerp(distance, targetDistance, 1f - Mathf.Exp(-smoothing * Time.deltaTime));

            transform.SetPositionAndRotation(Center - orbit * Vector3.forward * distance, orbit);
        }

        /// <summary>A PrimaryUp frame-jében még igaz, ha húzás volt. Utána nullázódik.</summary>
        public bool ConsumeDrag()
        {
            bool was = IsDragging;
            if (PointerInput.PrimaryUp) IsDragging = false;
            return was;
        }
    }
}
