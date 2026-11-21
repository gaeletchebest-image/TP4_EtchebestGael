using UnityEngine;

namespace UltimaGuardia
{
    // Presentation binding shared by the H05 characters and earlier archived prototypes.
    public sealed class CombatantView : MonoBehaviour
    {
        public string combatantId;
        public Transform visual;
        public Transform projectileOrigin;
        public Transform effectAnchor;
        public GameObject actorMarker;
        public GameObject targetMarker;
        public TextMesh nameLabel;
        public GameObject projectilePrefab;
        public Material projectileMaterial;
        private Quaternion initialRotation;
        private Vector3 initialScale;

        public void RememberPose()
        {
            initialRotation = visual.localRotation;
            initialScale = visual.localScale;
        }

        public void Present(CombatantState state, bool active, bool targeted, float pulse)
        {
            actorMarker.SetActive(active && state.IsAlive);
            targetMarker.SetActive(targeted && state.IsAlive);
            if (visual.GetComponent<CharacterAnimationPlayer>() == null)
            {
                visual.localRotation = initialRotation * (state.IsAlive ? Quaternion.identity : Quaternion.Euler(0, 0, 80));
                visual.localScale = initialScale * (1 + pulse * 0.08f);
            }
            nameLabel.text = state.Definition.Name + (!state.IsAlive ? " · Derrotado" : active ? " · ACTÚA" : "");
            if (Camera.main != null) nameLabel.transform.rotation = Camera.main.transform.rotation;
        }
    }
}
