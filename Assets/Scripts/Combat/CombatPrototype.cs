using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UltimaGuardia
{
    public enum PrototypePhase { Commands, Targets, Executing, Result, Advancing }
    public enum CombatMenu { Root, Skills, Items }

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
        private int rootIndex, skillIndex, itemIndex;
        private CombatAction selectedAction;
        private string notice;
        private StatusHud[] statusHud;
        private CombatPresentation presentation;
        [SerializeField] private FinalCombatHud finalHud;
        private int pausePage, helpPage, confirmationIndex;
        public int PausePage => pausePage;
        public int HelpPage => helpPage;
        public int ConfirmationSelection => confirmationIndex;
        public CombatAction[] DisplayedCommands => Commands();
        public void ConfigureFinal(FinalCombatHud hud) => finalHud = hud;
        public CombatPresentation Presentation => presentation;
        public CombatSession Session { get; private set; }
        public PrototypePhase Phase { get; private set; }
        public CombatMenu Menu { get; private set; }
        private bool isPaused;
        public bool IsPaused
        {
            get => isPaused;
            private set { isPaused = value; if (Session != null) Session.IsPaused = value; GameAudio.Current.SetPaused(value); }
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
            presentation = new CombatPresentation(views);
            if (finalHud == null) statusHud = resourceLabels.Select(label => new StatusHud(label)).ToArray();
            RestartCombat();
        }

        private void OnEnable() => input.Enable();
        private void OnDisable() => input.Disable();
        private void OnDestroy() { presentation?.Dispose(); if (input != null) Destroy(input); }

        public void RestartCombat()
        {
            presentation?.Cancel();
            GameAudio.Current.StopCombat();
            Session = new CombatSession(encounter);
            Session.PoisonTick += fighter => { GameAudio.Current.Combat("poison_tick"); if (!fighter.IsAlive) GameAudio.Current.Combat("death"); };
            GameAudio.Current.StartCombat();
            foreach (var slot in encounter.participants)
            {
                var view = views.First(v => v.combatantId == slot.combatant.id);
                view.transform.parent.SetPositionAndRotation(slot.position, Quaternion.Euler(slot.rotation));
                var animation = view.GetComponentInChildren<CharacterAnimationPlayer>();
                if (animation != null)
                {
                    var fighter = Session.Fighters.First(f => f.Definition.Id == view.combatantId);
                    if (animation.CurrentState == null) animation.Initialize(fighter);
                    else animation.ResetPresentation(fighter);
                }
            }
            Phase = PrototypePhase.Advancing;
            IsPaused = transitioning = false;
            commandIndex = targetIndex = overlayIndex = 0;
            rootIndex = skillIndex = itemIndex = 0; Menu = CombatMenu.Root;
            actionTime = 0; notice = null; confirmArmed = false;
            pausePage = helpPage = confirmationIndex = 0;
            finalHud?.Bind(Session);
            previousDirection = Vector2.zero;
            combatCamera.transform.SetPositionAndRotation(cameraPosition, cameraRotation);
            RefreshDisplay();
        }

        private void Update()
        {
            if (transitioning) return;
            SessionOptions.Observe(navigate, confirm, cancel, pause);
            if (!confirm.IsPressed()) confirmArmed = true;
            bool submit = confirmArmed && confirm.WasPressedThisFrame();
            if (submit) confirmArmed = false;
            bool back = cancel.WasPressedThisFrame();
            bool menuButton = pause.WasPressedThisFrame() && pause.activeControl?.device is Gamepad;

            if (IsPaused)
            {
                if (menuButton) { GameAudio.Current.Ui("ui_cancel"); IsPaused = false; pausePage = 0; confirmArmed = false; return; }
                if (back)
                {
                    GameAudio.Current.Ui("ui_cancel");
                    if (pausePage != 0) pausePage = 0; else IsPaused = false;
                    confirmArmed = false; return;
                }
                if (pausePage == 1) { int step = NavigationStep(true); if (step != 0) GameAudio.Current.Ui("ui_move"); helpPage = Wrap(helpPage + step, 4); if (submit) { GameAudio.Current.Ui("ui_confirm"); pausePage = 0; } }
                else if (pausePage == 2) { int step = NavigationStep(true); if (step != 0) GameAudio.Current.Ui("ui_move"); SessionOptions.SetVolume(SessionOptions.Volume + step * 10); if (submit) { GameAudio.Current.Ui("ui_confirm"); pausePage = 0; } }
                else if (pausePage == 3)
                {
                    int step = NavigationStep(false); if (step != 0) GameAudio.Current.Ui("ui_move");
                    confirmationIndex = Wrap(confirmationIndex + step, 2);
                    if (submit) { GameAudio.Current.Ui("ui_confirm"); if (confirmationIndex == 0) pausePage = 0; else if (overlayIndex == 3) RestartCombat(); else ReturnToMenu(); }
                }
                else
                {
                    int step = NavigationStep(false); if (step != 0) GameAudio.Current.Ui("ui_move");
                    overlayIndex = Wrap(overlayIndex + step, 5);
                    if (submit)
                    {
                        GameAudio.Current.Ui("ui_confirm");
                        if (overlayIndex == 0) { IsPaused = false; confirmArmed = false; }
                        else if (overlayIndex == 1) { pausePage = 1; helpPage = 0; }
                        else if (overlayIndex == 2) pausePage = 2;
                        else { pausePage = 3; confirmationIndex = 0; }
                    }
                }
                return;
            }
            if (Phase == PrototypePhase.Result)
            {
                int step = NavigationStep(false); if (step != 0) GameAudio.Current.Ui("ui_move");
                overlayIndex = Wrap(overlayIndex + step, 2);
                if (submit) { GameAudio.Current.Ui("ui_confirm"); if (overlayIndex == 0) RestartCombat(); else ReturnToMenu(); }
                return;
            }
            bool inSelection = Phase == PrototypePhase.Targets || (Phase == PrototypePhase.Commands && Menu != CombatMenu.Root);
            if (menuButton || (back && !inSelection && cancel.activeControl?.device is Keyboard))
            { IsPaused = true; GameAudio.Current.Ui("ui_confirm"); overlayIndex = 0; pausePage = 0; return; }
            if (back && Phase == PrototypePhase.Targets)
            { GameAudio.Current.Ui("ui_cancel"); Phase = PrototypePhase.Commands; notice = null; return; }
            if (back && Phase == PrototypePhase.Commands && Menu != CombatMenu.Root)
            { GameAudio.Current.Ui("ui_cancel"); RememberSelection(); Menu = CombatMenu.Root; commandIndex = rootIndex; notice = null; return; }

            if (Phase == PrototypePhase.Executing)
            {
                bool finished = presentation.Tick(Time.deltaTime);
                actionTime = presentation.Time;
                if (finished)
                {
                    Session.FinishAction();
                    if (Session.Outcome != CombatOutcome.Ongoing) GameAudio.Current.Result(Session.Outcome);
                    Phase = Session.Outcome != CombatOutcome.Ongoing ? PrototypePhase.Result :
                        Session.Actor == null ? PrototypePhase.Advancing : PrototypePhase.Commands;
                    commandIndex = targetIndex = overlayIndex = 0;
                    rootIndex = skillIndex = itemIndex = 0; Menu = CombatMenu.Root;
                    confirmArmed = !confirm.IsPressed();
                }
                return;
            }
            if (Phase == PrototypePhase.Advancing)
            {
                Session.AdvanceTime(Time.deltaTime);
                if (Session.Outcome != CombatOutcome.Ongoing) { GameAudio.Current.Result(Session.Outcome); Phase = PrototypePhase.Result; }
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
                int oldIndex = commandIndex;
                commandIndex = Wrap(commandIndex + NavigationStep(false), Menu == CombatMenu.Root ? 3 : actions.Length);
                if (commandIndex != oldIndex) GameAudio.Current.Ui("ui_move");
                if (commandIndex != oldIndex) notice = null;
                if (submit)
                {
                    GameAudio.Current.Ui("ui_confirm");
                    if (Menu == CombatMenu.Root && commandIndex != 0)
                    {
                        rootIndex = commandIndex; Menu = commandIndex == 1 ? CombatMenu.Skills : CombatMenu.Items;
                        commandIndex = Menu == CombatMenu.Skills ? skillIndex : itemIndex; notice = null;
                    }
                    else if (Menu == CombatMenu.Root || actions.Length > 0)
                    {
                        selectedAction = Menu == CombatMenu.Root ? CombatAction.Basic : actions[commandIndex];
                        notice = Session.UnavailableReason(selectedAction);
                        if (notice == null) { RememberSelection(); Phase = PrototypePhase.Targets; targetIndex = 0; }
                        else GameAudio.Current.Ui("ui_invalid");
                    }
                }
            }
            else if (Phase == PrototypePhase.Targets)
            {
                var targets = CurrentTargets();
                if (targets.Count == 0) { Phase = PrototypePhase.Commands; notice = "No hay objetivos válidos."; return; }
                int oldTarget = targetIndex;
                targetIndex = Session.IsArea(selectedAction) ? 0 : Wrap(targetIndex + NavigationStep(true), targets.Count);
                if (targetIndex != oldTarget) GameAudio.Current.Ui("ui_move");
                if (submit)
                {
                    GameAudio.Current.Ui("ui_confirm");
                    if (Session.TryCommit(selectedAction, targets[targetIndex], out notice)) BeginPresentation();
                    else { GameAudio.Current.Ui("ui_invalid"); Phase = PrototypePhase.Commands; }
                }
            }
        }

        private void BeginPresentation() { actionTime = 0; notice = null; presentation.Begin(Session); Phase = PrototypePhase.Executing; }
        private void ReturnToMenu() { transitioning = true; presentation.Cancel(); GameAudio.Current.StopCombat(); SceneManager.LoadScene("MainMenu"); }
        private CombatAction[] Commands() => Menu == CombatMenu.Root ? new[] { CombatAction.Basic } :
            Session.AvailableCommands().Where(a => Menu == CombatMenu.Items ? CombatSession.IsItem(a) : !CombatSession.IsItem(a) && a != CombatAction.Basic).ToArray();
        private void RememberSelection()
        {
            if (Menu == CombatMenu.Root) rootIndex = commandIndex;
            else if (Menu == CombatMenu.Skills) skillIndex = commandIndex;
            else itemIndex = commandIndex;
        }

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
        private void LateUpdate()
        {
            if (!transitioning && Session != null)
                foreach (var view in views)
                    view.GetComponentInChildren<CharacterAnimationPlayer>()?.Tick(IsPaused ? 0 : Time.deltaTime,
                        Session.Fighters.First(f => f.Definition.Id == view.combatantId));
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            if (Session == null) return;
            var pending = Session.Pending;
            for (int i = 0; i < views.Length; i++)
            {
                var state = Session.Fighters.First(f => f.Definition.Id == views[i].combatantId);
                bool targeted = pending != null ? pending.Targets.Contains(state) :
                    Phase == PrototypePhase.Targets && Session.IsArea(selectedAction) ? CurrentTargets().Contains(state) : SelectedTarget == state;
                float pulse = pending != null && pending.Targets.Contains(state) && pending.ImpactApplied
                    ? Mathf.Sin(Mathf.Clamp01((actionTime - 0.35f) / 0.5f) * Mathf.PI) : 0;
                views[i].Present(state, Session.Outcome == CombatOutcome.Ongoing && Session.Actor == state, targeted, pulse);
                if (finalHud != null) continue;
                statusHud[i].Present(state);
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
                descriptionLabel.text = pending == null ? "" : pending.Actor.Definition.Name + " → " + string.Join(", ", pending.Targets.Select(t => t.Definition.Name)) +
                    "\n" + Session.NameFor(pending.Kind);
            }
            else if (Phase == PrototypePhase.Targets)
            {
                var target = SelectedTarget;
                bool area = Session.IsArea(selectedAction);
                commandLabel.text = (area ? "Confirmá el grupo enemigo" : "Elegí un objetivo") + "\n" + Session.NameFor(selectedAction);
                descriptionLabel.text = Session.Describe(selectedAction) + "\n→ " + (area ? string.Join(", ", CurrentTargets().Select(t => t.Definition.Name)) :
                    target == null ? "Sin objetivos válidos" : target.Definition.Name + " · " + (target.Definition.Team == CombatTeam.Allies ? "Aliado" : "Enemigo")) +
                    "\n" + (area ? "Confirmar: ejecutar · Cancelar: volver" : "←→: cambiar · Confirmar: ejecutar");
            }
            else if (Phase == PrototypePhase.Advancing || Phase == PrototypePhase.Result || Session.Actor == null)
            {
                commandLabel.text = "Avanzan las barras ATB";
                descriptionLabel.text = "Cada participante carga según su velocidad.\nLa recuperación se consume antes de volver a cargar.\nPodés pausar con Escape o el botón de menú.";
            }
            else
            {
                var actions = Commands();
                commandIndex = Wrap(commandIndex, Menu == CombatMenu.Root ? 3 : actions.Length);
                if (Menu == CombatMenu.Root)
                {
                    commandLabel.text = string.Join("\n", new[] { "Atacar", "Habilidades", "Objetos" }.Select((s, i) => (i == commandIndex ? "→ " : "    ") + s));
                    descriptionLabel.text = commandIndex == 0 ? Session.NameFor(CombatAction.Basic) + " · " + Session.Describe(CombatAction.Basic) :
                        commandIndex == 1 ? "Habilidades de " + Session.Actor.Definition.Name + ".\nConsultá efecto, costo y recuperación antes de confirmar." :
                        "Inventario compartido por ambos aliados.\nCada objeto consume una acción y una unidad al confirmar.";
                    descriptionLabel.text += "\n" + (notice ?? "Elegí una acción. Reloj detenido.");
                }
                else
                {
                    commandLabel.text = actions.Length == 0 ? "Sin habilidades disponibles" : string.Join("\n", actions.Select((a, i) => (i == commandIndex ? "→ " : "    ") +
                        Session.NameFor(a) + (CombatSession.IsItem(a) ? " ×" + Session.QuantityFor(a) : "") +
                        (Session.UnavailableReason(a) == null ? "" : " [—]")));
                    descriptionLabel.text = actions.Length == 0 ? "Cancelar: volver" : Session.Describe(actions[commandIndex]) + "\n" +
                        (notice ?? Session.UnavailableReason(actions[commandIndex]) ?? "Elegí una acción. Reloj detenido.");
                }
            }
            helpLabel.text = SessionOptions.Help;
            overlayPanel.SetActive(IsPaused || Phase == PrototypePhase.Result);
            if (IsPaused)
            {
                overlayTitle.text = "Pausa";
                overlayTitle.text = pausePage == 1 ? SessionOptions.HelpTitles[helpPage] : pausePage == 2 ? "Volumen general" : pausePage == 3 ? (overlayIndex == 3 ? "¿Reiniciar el combate?" : "¿Volver al menú?") : "Pausa";
                overlayOptions.text = pausePage == 1 ? SessionOptions.HelpBody(helpPage) + "\n\n← Página " + (helpPage + 1) + " / 4 →\nConfirmar / volver: regresar a pausa" :
                    pausePage == 2 ? SessionOptions.Volume + " %\n\n← Disminuir     Aumentar →\n\nPasos de 10 · Confirmar / volver: regresar a pausa" :
                    pausePage == 3 ? "Se perderá el intento actual.\n\n" + string.Join("\n\n", new[] { "Cancelar", "Confirmar" }.Select((s, i) => (confirmationIndex == i ? "→ " : "    ") + s)) :
                    OverlayLines(new[] { "Continuar", "Cómo jugar", "Volumen", "Reiniciar", "Volver al menú" });
            }
            else if (Phase == PrototypePhase.Result)
            {
                overlayTitle.text = Session.Outcome == CombatOutcome.Victory ? "VICTORIA" : "DERROTA";
                overlayOptions.text = OverlayLines(new[] { "Reiniciar", "Volver al menú" });
            }
            finalHud?.Present(this);
        }

        private string OverlayLines(string[] options) => string.Join("\n\n", options.Select((option, i) => (overlayIndex == i ? "→ " : "    ") + option));
    }
}
