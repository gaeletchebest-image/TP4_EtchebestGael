using UnityEngine;

namespace UltimaGuardia
{
    // The combat presentation clock explicitly samples saved clips. Animator never runs
    // a second clock or gameplay events, so pause/restart cannot replay an impact.
    [RequireComponent(typeof(Animator), typeof(CharacterRig))]
    public sealed class CharacterAnimationPlayer : MonoBehaviour
    {
        public CharacterAnimationSet animations;
        public GameObject itemProp;
        private Animator animator;
        private CharacterRig rig;
        private AnimationClip clip;
        private string state;
        private float elapsed;
        private int previousHealth;
        private bool dead, playingAction;
        private Vector3 initialPosition;
        private Quaternion initialRotation;
        public string CurrentState => state;
        public float Elapsed => elapsed;
        public bool IsDead => dead;

        public void Initialize(CombatantState fighter)
        {
            animator = GetComponent<Animator>(); rig = GetComponent<CharacterRig>();
            initialPosition = transform.localPosition; initialRotation = transform.localRotation;
            ResetPresentation(fighter);
        }

        public void ResetPresentation(CombatantState fighter)
        {
            if (animator == null) { Initialize(fighter); return; }
            dead = false; previousHealth = fighter.Health;
            if (itemProp != null) itemProp.SetActive(false);
            transform.SetLocalPositionAndRotation(initialPosition, initialRotation);
            rig.RestoreRestPose();
            animator.applyRootMotion = false; animator.fireEvents = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate; animator.speed = 0;
            animator.Rebind(); animator.SetBool("Dead", false);
            Play("Idle", animations.idle);
        }

        public void BeginAction(PendingCombatAction pending, Vector3 target)
        {
            if (dead) return;
            var direction = target - transform.position; direction.y = 0;
            if (direction.sqrMagnitude > .01f) transform.rotation = Quaternion.LookRotation(direction);
            var binding = animations.Find(pending.Kind);
            if (itemProp != null) itemProp.SetActive(pending.Item != null);
            Play(pending.Item != null ? "Item" : pending.Kind.ToString(), pending.Item != null ? animations.item : binding.clip);
        }

        public void EndAction()
        {
            if (itemProp != null) itemProp.SetActive(false);
            transform.localRotation = initialRotation;
            if (!dead) Play("Idle", animations.idle);
        }

        internal void SynchronizeAction(float time)
        {
            if (!playingAction || dead) return;
            elapsed = time; Sample();
        }

        public void Tick(float seconds, CombatantState fighter)
        {
            if (seconds <= 0) return;
            if (!dead && !fighter.IsAlive)
            {
                dead = true; animator.SetBool("Dead", true);
                transform.localRotation = initialRotation;
                Play("Death", animations.death);
            }
            else if (!dead && fighter.Health < previousHealth) Play("Reaction", animations.reaction);
            previousHealth = fighter.Health;
            if (!playingAction) elapsed += seconds;
            if (!dead && !playingAction && state != "Idle" && elapsed >= clip.length)
            {
                transform.localRotation = initialRotation;
                Play("Idle", animations.idle);
            }
            Sample();
        }

        private void Play(string next, AnimationClip motion)
        {
            if (next != "Item" && itemProp != null) itemProp.SetActive(false);
            state = next; clip = motion; elapsed = 0;
            playingAction = next != "Idle" && next != "Reaction" && next != "Death";
            animator.SetBool("Executing", next != "Idle");
            Sample();
        }

        private void Sample()
        {
            float normalized = state == "Idle" ? Mathf.Repeat(elapsed / clip.length, 1) : Mathf.Clamp01(elapsed / clip.length);
            animator.Play(state, 0, normalized); animator.Update(0);
        }
    }
}
