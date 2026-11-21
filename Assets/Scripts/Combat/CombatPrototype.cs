using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UltimaGuardia
{
    public enum PrototypePhase { Commands, Targets, Executing, Result, Advancing }

    public sealed class CombatPrototype : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputTemplate;
        [SerializeField] private EncounterData encounter;
        [SerializeField] private Image[] atbBars;
        [SerializeField] private CombatantView[] views;
        [SerializeField] private Text[] resourceLabels;
        [SerializeField] private Text turnLabel, eventLabel, commandLabel, descriptionLabel, helpLabel;
        [SerializeField] private GameObject decisionPanel, overlayPanel;
        [SerializeField] private Text overlayTitle, overlayOptions;
        [SerializeField] private Camera combatCamera;
        private InputActionAsset input;
        private InputAction navigate, confirm, cancel, pause;
        private Vector3 cameraPosition;
        private Quaternion cameraRotation;
        private Vector2 previousDirection;
        private float repeatAt, actionTime;
        private bool confirmArmed, transitioning;
        private int commandIndex, targetIndex, overlayIndex;
        private CombatAction selectedAction;
        private string notice;
        public CombatSession Session { get; private set; }
        public PrototypePhase Phase { get; private set; }
        private bool isPaused;
        public bool IsPaused
        {
            get => isPaused;
            private set { isPaused = value; if (Session != null) Session.IsPaused = value; }
        }
        public EncounterData Encounter => encounter;
        public void ConfigureEncounter(EncounterData data, Image[] bars) { encounter = data; atbBars = bars; }
        public int SelectedCommand => commandIndex;
        public int OverlaySelection => overlayIndex;
        public float ActionTime => actionTime;
        public CombatantState SelectedTarget => Phase == PrototypePhase.Targets ? CurrentTargets().ElementAtOrDefault(targetIndex) : null;

        public void Configure(InputActionAsset template, CombatantView[] combatants, Text[] resources,
            Text turn, Text events, Text commands, Text description, Text help, GameObject decision,
            GameObject overlay, Text title, Text options, Camera camera)
        {
            inputTemplate = template; views = combatants; resourceLabels = resources;
            turnLabel = turn; eventLabel = events; commandLabel = commands; descriptionLabel = description;
            helpLabel = help; decisionPanel = decision; overlayPanel = overlay;
            overlayTitle = title; overlayOptions = options; combatCamera = camera;
        }

        private void Awake()
        {
            input = Instantiate(inputTemplate);
            navigate = input.FindAction("UI/Navigate", true);
            confirm = input.FindAction("UI/Confirm", true);
            cancel = input.FindAction("UI/Cancel", true);
            pause = input.FindAction("UI/Pause", true);
            cameraPosition = combatCamera.transform.position;
            cameraRotation = combatCamera.transform.rotation;
            foreach (var view in views) view.RememberPose();
            RestartCombat();
        }

        private void OnEnable() => input.Enable();
        private void OnDisable() => input.Disable();
        private void OnDestroy() { if (input != null) Destroy(input); }

        public void RestartCombat()
        {
            Session = new CombatSession(encounter);
            foreach (var slot in encounter.participants)
            {
                var view = views.First(v => v.combatantId == slot.combatant.id);
                view.transform.parent.SetPositionAndRotation(slot.position, Quaternion.Euler(slot.rotation));
            }
            Phase = PrototypePhase.Advancing;
            IsPaused = transitioning = false;
            commandIndex = targetIndex = overlayIndex = 0;
            actionTime = 0; notice = null; confirmArmed = false;
            previousDirection = Vector2.zero;
            combatCamera.transform.SetPositionAndRotation(cameraPosition, cameraRotation);
            RefreshDisplay();
        }

        private void Update()
        {
            if (transitioning) return;
            if (!confirm.IsPressed()) confirmArmed = true;
            bool submit = confirmArmed && confirm.WasPressedThisFrame();
            if (submit) confirmArmed = false;
            bool back = cancel.WasPressedThisFrame();
            bool menuButton = pause.WasPressedThisFrame() && pause.activeControl?.device is Gamepad;

            if (IsPaused)
            {
                if (menuButton || back) { IsPaused = false; confirmArmed = false; return; }
                int step = NavigationStep(false);
                overlayIndex = Wrap(overlayIndex + step, 3);
                if (submit)
                {
                    if (overlayIndex == 0) { IsPaused = false; confirmArmed = false; }
                    else if (overlayIndex == 1) RestartCombat();
                    else ReturnToMenu();
                }
                return;
            }
            if (Phase == PrototypePhase.Result)
            {
                overlayIndex = Wrap(overlayIndex + NavigationStep(false), 2);
                if (submit) { if (overlayIndex == 0) RestartCombat(); else ReturnToMenu(); }
                return;
            }
            if (menuButton || (back && Phase != PrototypePhase.Targets && cancel.activeControl?.device is Keyboard))
            { IsPaused = true; overlayIndex = 0; return; }
            if (back && Phase == PrototypePhase.Targets)
            { Phase = PrototypePhase.Commands; notice = null; return; }

            if (Phase == PrototypePhase.Executing)
            {
                actionTime += Time.deltaTime;
                if (actionTime >= 0.35f) Session.ApplyImpact();
                if (actionTime >= 0.85f)
                {
                    Session.FinishAction();
                    Phase = Session.Outcome != CombatOutcome.Ongoing ? PrototypePhase.Result :
                        Session.Actor == null ? PrototypePhase.Advancing : PrototypePhase.Commands;
                    commandIndex = targetIndex = overlayIndex = 0;
                    confirmArmed = !confirm.IsPressed();
                }
                return;
            }
            if (Phase == PrototypePhase.Advancing)
            {
                Session.AdvanceTime(Time.deltaTime);
                if (Session.Outcome != CombatOutcome.Ongoing) Phase = PrototypePhase.Result;
                else if (Session.Actor != null) { Phase = PrototypePhase.Commands; confirmArmed = !confirm.IsPressed(); }
                return;
            }
            if (Session.Actor == null) { Phase = PrototypePhase.Advancing; return; }
            if (Session.Actor.Definition.Team == CombatTeam.Enemies)
            {
                if (Session.CommitEnemyAction()) BeginPresentation();
                return;
            }
            if (Phase == PrototypePhase.Commands)
            {
                var actions = Commands();
                commandIndex = Wrap(commandIndex + NavigationStep(false), actions.Length);
                if (submit)
                {
                    selectedAction = actions[commandIndex];
                    notice = Session.UnavailableReason(selectedAction);
                    if (notice == null) { Phase = PrototypePhase.Targets; targetIndex = 0; }
                }
            }
            else if (Phase == PrototypePhase.Targets)
            {
                var targets = CurrentTargets();
                if (targets.Count == 0) { Phase = PrototypePhase.Commands; notice = "No hay objetivos válidos."; return; }
                targetIndex = Wrap(targetIndex + NavigationStep(true), targets.Count);
                if (submit)
                {
                    if (Session.TryCommit(selectedAction, targets[targetIndex], out notice)) BeginPresentation();
                    else Phase = PrototypePhase.Commands;
                }
            }
        }

        private void BeginPresentation() { actionTime = 0; notice = null; Phase = PrototypePhase.Executing; }
        private void ReturnToMenu() { transitioning = true; SceneManager.LoadScene("MainMenu"); }
        private CombatAction[] Commands() => Session.AvailableCommands();

        private List<CombatantState> CurrentTargets() => Session.TargetsFor(selectedAction)
            .OrderBy(f => combatCamera.WorldToViewportPoint(views.First(v => v.combatantId == f.Definition.Id).transform.position).x).ToList();

        private int NavigationStep(bool horizontal)
        {
            var raw = navigate.ReadValue<Vector2>();
            var direction = new Vector2(Mathf.Abs(raw.x) > 0.5f ? Mathf.Sign(raw.x) : 0,
                Mathf.Abs(raw.y) > 0.5f ? Mathf.Sign(raw.y) : 0);
            bool moved = direction != Vector2.zero && (direction != previousDirection || Time.unscaledTime >= repeatAt);
            if (direction != previousDirection) repeatAt = Time.unscaledTime + 0.4f;
            else if (moved) repeatAt = Time.unscaledTime + 0.12f;
            previousDirection = direction;
            return moved ? (horizontal ? (int)direction.x : -(int)direction.y) : 0;
        }

        private static int Wrap(int value, int count) => count == 0 ? 0 : (value % count + count) % count;
        private void LateUpdate() => RefreshDisplay();

        private void RefreshDisplay()
        {
            if (Session == null) return;
            var pending = Session.Pending;
            for (int i = 0; i < views.Length; i++)
            {
                var state = Session.Fighters.First(f => f.Definition.Id == views[i].combatantId);
                bool targeted = pending != null ? pending.Target == state : SelectedTarget == state;
                float pulse = pending != null && pending.Target == state && pending.ImpactApplied
                    ? Mathf.Sin(Mathf.Clamp01((actionTime - 0.35f) / 0.5f) * Mathf.PI) : 0;
                views[i].Present(state, Session.Outcome == CombatOutcome.Ongoing && Session.Actor == state, targeted, pulse);
                resourceLabels[i].text = state.Definition.Name + " · " + (state.Definition.Team == CombatTeam.Allies ? "Aliado" : "Enemigo") +
                    (state.IsAlive ? "" : " · DERROTADO") + "\nVida " + state.Health + "/" + state.Definition.MaxHealth +
                    "    Maná " + state.Mana + "/" + state.Definition.MaxMana + "\n" +
                    (!state.IsAlive ? "Fuera de combate" : state.Atb >= 100 ? "Listo" : state.RecoveryRemaining > 0 ?
                    "Recuperando · " + state.RecoveryRemaining.ToString("0.0") + " s" : "Llenando · " + state.Atb.ToString("0") + " %");
                if (atbBars != null && i < atbBars.Length && atbBars[i] != null)
                {
                    atbBars[i].fillAmount = (float)(state.Atb / 100);
                    atbBars[i].color = state.Atb >= 100 ? new Color(1, 0.8f, 0.2f) : new Color(0.2f, 0.7f, 0.9f);
                }
            }
            turnLabel.text = "ÚLTIMA GUARDIA  /  " + (Phase == PrototypePhase.Result ? "Combate terminado" :
                Session.Actor == null ? "Avance ATB" : "Turno: " + Session.Actor.Definition.Name);
            eventLabel.text = Session.LastEvent;
            decisionPanel.SetActive(Phase != PrototypePhase.Result);
            if (Phase == PrototypePhase.Executing)
            {
                commandLabel.text = "Acción en ejecución";
                descriptionLabel.text = pending == null ? "" : pending.Actor.Definition.Name + " → " + pending.Target.Definition.Name +
                    "\n" + Session.NameFor(pending.Kind);
            }
            else if (Phase == PrototypePhase.Targets)
            {
                var target = SelectedTarget;
                commandLabel.text = "Elegí un objetivo\n" + Session.NameFor(selectedAction);
                descriptionLabel.text = target == null ? "Sin objetivos válidos" : "→ " + target.Definition.Name + " · " +
                    (target.Definition.Team == CombatTeam.Allies ? "Aliado" : "Enemigo") + "\nIzquierda / derecha: cambiar · Confirmar: ejecutar";
            }
            else if (Phase == PrototypePhase.Advancing || Phase == PrototypePhase.Result || Session.Actor == null)
            {
                commandLabel.text = "Avanzan las barras ATB";
                descriptionLabel.text = "Cada participante carga según su velocidad.\nLa recuperación se consume antes de volver a cargar.\nPodés pausar con Escape o el botón de menú.";
            }
            else
            {
                var actions = Commands();
                commandIndex = Wrap(commandIndex, actions.Length);
                commandLabel.text = string.Join("\n", actions.Select((a, i) => (i == commandIndex ? "→ " : "    ") +
                    Session.NameFor(a) + (a == CombatAction.Potion ? " ×" + Session.Potions : "") +
                    (Session.UnavailableReason(a) == null ? "" : " [No disponible]")));
                var action = actions[commandIndex];
                var data = Session.ActionFor(action);
                descriptionLabel.text = (action == CombatAction.Potion ? Session.Potion.Description + " · +" + Session.Potion.Amount + " vida\nInventario compartido · Sin costo de maná" :
                    data.Description + "\nPotencia " + data.Potency.ToString("0.##") + " · Maná: " + data.ManaCost) +
                    " · Recuperación: " + Session.RecoveryFor(action).ToString("0.0") + " s\n" +
                    (notice ?? Session.UnavailableReason(action) ?? "Elegí una acción. Reloj detenido.");
            }
            helpLabel.text = "↑↓ / cruceta / stick: acción     ←→: objetivo     Enter / Espacio / botón inferior: confirmar\nEscape / botón derecho: cancelar     Escape en raíz / botón de menú: pausa";
            overlayPanel.SetActive(IsPaused || Phase == PrototypePhase.Result);
            if (IsPaused)
            {
                overlayTitle.text = "Pausa";
                overlayOptions.text = OverlayLines(new[] { "Continuar", "Reiniciar", "Volver al menú" });
            }
            else if (Phase == PrototypePhase.Result)
            {
                overlayTitle.text = Session.Outcome == CombatOutcome.Victory ? "VICTORIA" : "DERROTA";
                overlayOptions.text = OverlayLines(new[] { "Reiniciar", "Volver al menú" });
            }
        }

        private string OverlayLines(string[] options) => string.Join("\n\n", options.Select((option, i) => (overlayIndex == i ? "→ " : "    ") + option));
    }
}
