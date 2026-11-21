using UnityEngine;

namespace UltimaGuardia
{
    // Deliberately provisional geometry. Stable sockets survive replacement in H05/H06.
    public sealed class CombatantView : MonoBehaviour
    {
        public string combatantId;
        public Transform visual;
        public Transform projectileOrigin;
        public Transform effectAnchor;
        public GameObject actorMarker;
        public GameObject targetMarker;
        public TextMesh nameLabel;
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
            visual.localRotation = initialRotation * (state.IsAlive ? Quaternion.identity : Quaternion.Euler(0, 0, 80));
            visual.localScale = initialScale * (1 + pulse * 0.08f);
            nameLabel.text = state.Definition.Name + (state.IsAlive ? "" : " · Derrotado");
            if (Camera.main != null) nameLabel.transform.rotation = Camera.main.transform.rotation;
        }
    }
}
