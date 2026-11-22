using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace UltimaGuardia.Tests
{
    public class CombatFlowTests
    {
#if UNITY_EDITOR
        private InputSettings.EditorInputBehaviorInPlayMode previousEditorInput;
#endif
        private Keyboard keyboard;
        private Gamepad gamepad;
        private bool wasBackground;
        private float previousTimeScale;
        private InputSettings.BackgroundBehavior previousBehavior;
        private CombatPrototype Battle => Object.FindFirstObjectByType<CombatPrototype>();

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            wasBackground = Application.runInBackground;
            previousTimeScale = Time.timeScale;
            previousBehavior = InputSystem.settings.backgroundBehavior;
            Application.runInBackground = true;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
#if UNITY_EDITOR
            previousEditorInput = InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
#endif
            keyboard = InputSystem.AddDevice<Keyboard>();
            gamepad = InputSystem.AddDevice<Gamepad>();
            yield return SceneManager.LoadSceneAsync("Combat");
            yield return null; yield return null;
            yield return WaitForCommands();
            Assert.AreEqual(PrototypePhase.Commands, Battle.Phase);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (keyboard != null && keyboard.added) InputSystem.RemoveDevice(keyboard);
            if (gamepad != null && gamepad.added) InputSystem.RemoveDevice(gamepad);
#if UNITY_EDITOR
            InputSystem.settings.editorInputBehaviorInPlayMode = previousEditorInput;
#endif
            InputSystem.settings.backgroundBehavior = previousBehavior;
            Application.runInBackground = wasBackground;
            Time.timeScale = previousTimeScale;
            yield return null;
        }

        [UnityTest]
        public IEnumerator CancellationPauseRepeatedConfirmAndRestartPreserveRules()
        {
            // ATB starts with the mage; resolve her basic to reach the warrior's heavy strike.
            yield return Press(Key.Enter); yield return Press(Key.Enter);
            yield return WaitForCommands();
            Assert.AreEqual("warrior", Battle.Session.Actor.Definition.Id);
            yield return Capture("arena-1280x720", 1280, 720);
            yield return Capture("arena-1920x1080", 1920, 1080);
            yield return Press(Key.DownArrow);
            yield return Press(Key.Enter);
            Assert.AreEqual(CombatMenu.Skills, Battle.Menu);
            yield return Press(Key.Enter);
            Assert.AreEqual(PrototypePhase.Targets, Battle.Phase);
            Assert.AreEqual(30, Battle.Session.Actor.Mana);
            yield return Press(Key.RightArrow);
            yield return Capture("target", 1280, 720);
            yield return Press(Key.Escape);
            Assert.AreEqual(PrototypePhase.Commands, Battle.Phase);
            Assert.IsFalse(Battle.IsPaused);
            Assert.AreEqual(30, Battle.Session.Actor.Mana);
            Assert.AreEqual(3, Battle.Session.Potions);
            yield return Press(Key.Enter); // reopen targets
            var target = Battle.SelectedTarget;
            float normalTimeScale = Time.timeScale;
            Time.timeScale = .1f;
            yield return Press(Key.Enter); // commit heavy
            Assert.AreEqual(PrototypePhase.Executing, Battle.Phase);
            Assert.AreEqual(22, Battle.Session.Actor.Mana);
            yield return Press(Key.Enter); // ignored during execution
            yield return Press(Key.Escape); // before impact
            Assert.IsTrue(Battle.IsPaused);
            float elapsed = Battle.ActionTime;
            Assert.Less(elapsed, .35f, "The test pauses before the configured impact time.");
            int health = target.Health;
            yield return new WaitForSecondsRealtime(0.45f);
            Assert.AreEqual(elapsed, Battle.ActionTime);
            Assert.AreEqual(health, target.Health);
            yield return Capture("pause", 1280, 720);
            yield return Press(Key.Escape);
            Time.timeScale = normalTimeScale;
            while (Battle.ActionTime < 0.4f) yield return null;
            Assert.AreEqual(health - CombatSession.CalculateDamage(28, 1.8, target.Definition.Defense), target.Health);
            yield return Press(Key.Escape); // after impact
            int damaged = target.Health;
            yield return new WaitForSecondsRealtime(0.3f);
            Assert.AreEqual(damaged, target.Health);
            yield return RestartFromPause();
            AssertFreshBattle();
            yield return WaitForCommands();
            yield return Press(Key.Enter); // target selection
            yield return Press(Key.Escape);
            yield return Press(Key.Escape);
            yield return ReturnFromPause();
            Assert.AreEqual("MainMenu", SceneManager.GetActiveScene().name);
        }

        [UnityTest]
        public IEnumerator DecisionAndActivePauseFreezeBarsAndRecovery()
        {
            double clock = Battle.Session.ActiveTime;
            var bars = Battle.Session.Fighters.Select(f => f.Atb).ToArray();
            yield return new WaitForSecondsRealtime(0.3f);
            Assert.AreEqual(clock, Battle.Session.ActiveTime);
            CollectionAssert.AreEqual(bars, Battle.Session.Fighters.Select(f => f.Atb));
            yield return Press(Key.Enter); yield return Press(Key.Enter);
            yield return WaitForCommands();
            Assert.AreEqual("warrior", Battle.Session.Actor.Definition.Id);
            yield return Press(Key.DownArrow);
            yield return Press(Key.Enter); yield return Press(Key.Enter);
            yield return Press(Key.Enter);
            while (Battle.Session.CompletedActions < 2 || Battle.Session.Actor == null) yield return null;
            while (Battle.Phase != PrototypePhase.Commands) yield return null;
            Assert.IsFalse(Battle.Session.ClockRunning, "The next decision must stop ATB and recovery before measuring them.");
            var warrior = Battle.Session.Fighters[0];
            Assert.Greater(warrior.RecoveryRemaining, 0);
            double recovery = warrior.RecoveryRemaining;
            yield return Capture("recovery-1280x720", 1280, 720);
            yield return Capture("recovery-1920x1080", 1920, 1080);
            yield return new WaitForSecondsRealtime(0.3f);
            Assert.AreEqual(recovery, warrior.RecoveryRemaining);
            Battle.RestartCombat();
            yield return Press(Key.Escape);
            Assert.IsTrue(Battle.IsPaused); Assert.AreEqual(PrototypePhase.Advancing, Battle.Phase);
            bars = Battle.Session.Fighters.Select(f => f.Atb).ToArray();
            yield return new WaitForSecondsRealtime(0.3f);
            CollectionAssert.AreEqual(bars, Battle.Session.Fighters.Select(f => f.Atb));
            yield return Press(Key.Escape);
            yield return new WaitForSecondsRealtime(0.2f);
            Assert.Greater(Battle.Session.Fighters[1].Atb, bars[1]);
            yield return Capture("advancing-1280x720", 1280, 720);
        }

        [UnityTest]
        public IEnumerator NestedMenusGroupSelectionAndItemsPreserveFocusAndResources()
        {
            double clock = Battle.Session.ActiveTime;
            yield return Press(Key.DownArrow); yield return Press(Key.Enter);
            Assert.AreEqual(CombatMenu.Skills, Battle.Menu);
            yield return Press(Key.Enter); // Full health: unavailable healing stays in the list.
            Assert.AreEqual(PrototypePhase.Commands, Battle.Phase); Assert.IsNull(Battle.Session.Pending);
            yield return Press(Key.DownArrow); yield return Press(Key.Enter);
            Assert.AreEqual(PrototypePhase.Targets, Battle.Phase);
            yield return Capture("storm-group-1280x720", 1280, 720);
            yield return Capture("storm-group-1920x1080", 1920, 1080);
            foreach (var view in Object.FindObjectsByType<CombatantView>(FindObjectsSortMode.None))
                Assert.AreEqual(view.combatantId == "ogre" || view.combatantId == "shaman", view.targetMarker.activeSelf);
            yield return Press(Key.Escape); Assert.AreEqual(1, Battle.SelectedCommand);
            yield return Press(Key.Escape); Assert.AreEqual(CombatMenu.Root, Battle.Menu); Assert.AreEqual(1, Battle.SelectedCommand);
            yield return Press(Key.DownArrow); yield return Press(Key.Enter);
            Assert.AreEqual(CombatMenu.Items, Battle.Menu);
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(i, Battle.SelectedCommand);
                yield return Press(Key.Enter);
                Assert.AreEqual(i == 2 || i == 3 ? PrototypePhase.Targets : PrototypePhase.Commands, Battle.Phase);
                Assert.IsNull(Battle.Session.Pending);
                if (i == 2 || i == 3) yield return Press(Key.Escape);
                if (i < 4) yield return Press(Key.DownArrow);
            }
            yield return Capture("items-1280x720", 1280, 720);
            yield return Capture("items-1920x1080", 1920, 1080);
            Assert.AreEqual(clock, Battle.Session.ActiveTime);
            CollectionAssert.AreEqual(new[] { 3, 2, 1, 1, 2 }, Battle.Session.Inventory.Values);
            yield return Pad(GamepadButton.Start); Assert.IsTrue(Battle.IsPaused);
            yield return Pad(GamepadButton.East); Assert.IsFalse(Battle.IsPaused);
            Assert.AreEqual(CombatMenu.Items, Battle.Menu); Assert.AreEqual(4, Battle.SelectedCommand);
            yield return Press(Key.Escape); Assert.AreEqual(CombatMenu.Root, Battle.Menu); Assert.AreEqual(2, Battle.SelectedCommand);
        }

        [UnityTest]
        public IEnumerator HealingEtherAndStormWorkThroughMenusAndRestartDuringAreaPauseIsClean()
        {
            var mage = Battle.Session.Actor; mage.Damage(50);
            yield return Press(Key.DownArrow); yield return Press(Key.Enter); // Skills
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Enter));
            yield return null; yield return null;
            yield return new WaitForSecondsRealtime(0.2f);
            Assert.AreEqual(PrototypePhase.Targets, Battle.Phase); Assert.IsNull(Battle.Session.Pending);
            InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
            Assert.AreSame(mage, Battle.SelectedTarget);
            yield return Capture("healing-self", 1280, 720);
            yield return Press(Key.Enter);
            yield return WaitForCommands();
            Assert.AreEqual(96, mage.Health); Assert.AreEqual(35, mage.Mana);
            Assert.AreEqual("warrior", Battle.Session.Actor.Definition.Id);
            yield return Press(Key.DownArrow); yield return Press(Key.DownArrow); yield return Press(Key.Enter);
            yield return Press(Key.DownArrow); yield return Press(Key.Enter); // Ether
            Assert.AreSame(mage, Battle.SelectedTarget);
            yield return Capture("ether-target", 1280, 720);
            yield return Press(Key.Escape); Assert.AreEqual(2, Battle.Session.QuantityFor(CombatAction.Ether));
            yield return Press(Key.Enter); yield return Press(Key.Enter);
            yield return WaitForCommands();
            Assert.AreEqual(45, mage.Mana); Assert.AreEqual(1, Battle.Session.QuantityFor(CombatAction.Ether));
            // Wait through enemy turns to the next player decision, then reach the mage using basics.
            while (Battle.Session.Actor == null || Battle.Session.Actor.Definition.Id != "mage")
            {
                if (Battle.Phase == PrototypePhase.Commands && Battle.Session.Actor?.Definition.Team == CombatTeam.Allies)
                { yield return Press(Key.Enter); yield return Press(Key.Enter); }
                yield return null;
            }
            while (Battle.Phase != PrototypePhase.Commands) yield return null;
            yield return Press(Key.DownArrow); yield return Press(Key.Enter); yield return Press(Key.DownArrow); yield return Press(Key.Enter);
            yield return Press(Key.Enter); // Storm commit
            Assert.AreEqual(27, Battle.Session.Actor.Mana);
            yield return Press(Key.Escape); Assert.IsTrue(Battle.IsPaused);
            yield return Capture("storm-paused", 1280, 720);
            yield return RestartFromPause();
            AssertFreshBattle();
            CollectionAssert.AreEqual(new[] { 3, 2, 1, 1, 2 }, Battle.Session.Inventory.Values);
            foreach (var view in Object.FindObjectsByType<CombatantView>(FindObjectsSortMode.None)) Assert.IsFalse(view.targetMarker.activeSelf);
        }

        [UnityTest]
        public IEnumerator KeyboardWinsNormalEncounterAndRestartsFromResult()
        {
            yield return PlayToResult(false);
            Assert.AreEqual(CombatOutcome.Victory, Battle.Session.Outcome);
            yield return Capture("victory", 1280, 720);
            TestContext.WriteLine("Normal keyboard victory: " + Battle.Session.CompletedActions + " actions, potions " + Battle.Session.Potions);
            yield return Press(Key.Enter);
            AssertFreshBattle();
        }

        [UnityTest]
        public IEnumerator VirtualGamepadWinsThenLosesLowHealthScenarioAndRestarts()
        {
            // Stick opens Skills; cancel returns to the root without spending.
            // Stick navigation and cancellation before the full dpad/button route.
            InputSystem.QueueStateEvent(gamepad, new GamepadState { leftStick = Vector2.down });
            yield return null; yield return null;
            InputSystem.QueueStateEvent(gamepad, new GamepadState());
            yield return null;
            Assert.AreEqual(1, Battle.SelectedCommand);
            yield return Pad(GamepadButton.South);
            yield return Pad(GamepadButton.East);
            Assert.AreEqual(PrototypePhase.Commands, Battle.Phase);
            yield return Pad(GamepadButton.DpadUp);
            yield return PlayToResult(true);
            Assert.AreEqual(CombatOutcome.Victory, Battle.Session.Outcome);
            TestContext.WriteLine("Normal virtual gamepad victory: " + Battle.Session.CompletedActions + " actions, potions " + Battle.Session.Potions);
            yield return Pad(GamepadButton.South);
            AssertFreshBattle();
            // Explicit boundary fixture, not a claim about natural balance or physical hardware.
            foreach (var ally in Battle.Session.Fighters.Where(f => f.Definition.Team == CombatTeam.Allies))
                ally.Damage(ally.Health - 1);
            yield return PlayToResult(true, false);
            Assert.AreEqual(CombatOutcome.Defeat, Battle.Session.Outcome);
            yield return Capture("defeat-forced-low-health", 1280, 720);
            yield return Pad(GamepadButton.South);
            AssertFreshBattle();
            yield return Pad(GamepadButton.Start);
            Assert.IsTrue(Battle.IsPaused);
            yield return Pad(GamepadButton.East);
            Assert.IsFalse(Battle.IsPaused);
        }

        [UnityTest]
        public IEnumerator StatusObjectsWorkWithPadAndKeyboardAndShowCoexistingCounters()
        {
            var mage = Battle.Session.Actor;
            yield return Pad(GamepadButton.DpadDown); yield return Pad(GamepadButton.DpadDown); yield return Pad(GamepadButton.South);
            yield return Pad(GamepadButton.DpadDown); yield return Pad(GamepadButton.DpadDown); yield return Pad(GamepadButton.South);
            for (int i = 0; Battle.SelectedTarget != mage && i < 3; i++) yield return Pad(GamepadButton.DpadRight);
            Assert.AreSame(mage, Battle.SelectedTarget);
            yield return Pad(GamepadButton.East); Assert.AreEqual(1, Battle.Session.QuantityFor(CombatAction.HasteTonic));
            yield return Pad(GamepadButton.South);
            for (int i = 0; Battle.SelectedTarget != mage && i < 3; i++) yield return Pad(GamepadButton.DpadRight);
            yield return Pad(GamepadButton.South); yield return WaitForPlayerCommands();
            Assert.AreEqual(2, mage.Effects.Single(e => e.Definition.Kind == StatusKind.Haste).Remaining);
            Assert.AreEqual(42, mage.EffectiveSpeed); Assert.AreEqual(0, Battle.Session.QuantityFor(CombatAction.HasteTonic));
            // Explicit coexistence fixture for the three final sprites and counters.
            mage.ApplyStatus(Battle.Session.ItemFor(CombatAction.Antidote).AppliedEffect);
            mage.ApplyStatus(Battle.Session.ItemFor(CombatAction.CorrosiveFlask).AppliedEffect);
            yield return Capture("statuses-coexist-1280x720", 1280, 720);
            yield return Capture("statuses-coexist-1920x1080", 1920, 1080);
            var labels = Object.FindObjectsByType<UnityEngine.UI.Text>(FindObjectsSortMode.None);
            foreach (var effect in mage.Effects)
                Assert.IsTrue(labels.Any(t => t.text == effect.Definition.Name + " " + effect.Remaining));
            var card = GameObject.Find("Card_mage");
            foreach (var icon in new[] { "Icon_poison", "Icon_haste", "Icon_corrosion" })
            {
                var image = card.transform.Find(icon).GetComponent<UnityEngine.UI.Image>();
                Assert.IsTrue(image.gameObject.activeInHierarchy); Assert.IsNotNull(image.sprite);
            }
            yield return Press(Key.DownArrow); yield return Press(Key.DownArrow); yield return Press(Key.Enter);
            for (int i = 0; i < 3; i++) yield return Press(Key.DownArrow);
            yield return Press(Key.Enter);
            for (int i = 0; Battle.SelectedTarget.Definition.Id != "ogre" && i < 3; i++) yield return Press(Key.RightArrow);
            yield return Press(Key.Enter); yield return Press(Key.Escape);
            Assert.IsTrue(Battle.IsPaused); var ogre = Battle.Session.Fighters.Single(f => f.Definition.Id == "ogre");
            Assert.IsFalse(ogre.HasStatus(StatusKind.Corrosion));
            yield return new WaitForSecondsRealtime(0.2f); Assert.IsFalse(ogre.HasStatus(StatusKind.Corrosion));
            yield return Press(Key.Escape);
            while (Battle.ActionTime < 0.4f) yield return null;
            Assert.AreEqual(3, ogre.EffectiveDefense); Assert.AreEqual(0, Battle.Session.QuantityFor(CombatAction.CorrosiveFlask));
            yield return Press(Key.Escape); yield return Capture("corrosion-impact-paused", 1280, 720);
            yield return Press(Key.Escape); yield return WaitForPlayerCommands();
            // Either survivor can use the shared antidote on the affected mage.
            Assert.IsTrue(mage.IsAlive); Assert.IsTrue(mage.HasStatus(StatusKind.Poison));
            yield return Press(Key.DownArrow); yield return Press(Key.DownArrow); yield return Press(Key.Enter);
            for (int i = 0; i < 4; i++) yield return Press(Key.DownArrow);
            yield return Press(Key.Enter);
            for (int i = 0; Battle.SelectedTarget != mage && i < 3; i++) yield return Press(Key.RightArrow);
            int health = mage.Health;
            yield return Press(Key.Enter);
            Assert.IsFalse(mage.HasStatus(StatusKind.Poison)); Assert.AreEqual(health, mage.Health);
            Assert.AreEqual(1, Battle.Session.QuantityFor(CombatAction.Antidote));
            yield return Press(Key.Escape); yield return Capture("antidote-confirmed-paused", 1280, 720);
            yield return RestartFromPause(); AssertFreshBattle();
            Assert.IsTrue(Battle.Session.Fighters.All(f => f.Effects.Count == 0));
        }

        [UnityTest]
        public IEnumerator PoisonDeathBeforeTurnShowsResultWithoutMenuAndRestartClearsMarkers()
        {
            var warrior = Battle.Session.Fighters.Single(f => f.Definition.Id == "warrior");
            warrior.Damage(warrior.Health - 8);
            warrior.ApplyStatus(Battle.Session.ItemFor(CombatAction.Antidote).AppliedEffect);
            Battle.Session.Actor.Damage(999); // Last survivor is about to receive his first turn.
            float deadline = Time.realtimeSinceStartup + 10;
            while (Battle.Phase != PrototypePhase.Result && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.AreEqual(PrototypePhase.Result, Battle.Phase); Assert.AreEqual(CombatOutcome.Defeat, Battle.Session.Outcome);
            Assert.AreEqual(0, Battle.Session.CompletedActions); Assert.IsNull(Battle.Session.Actor); Assert.IsNull(Battle.Session.Pending);
            Assert.IsTrue(Object.FindObjectsByType<CombatantView>(FindObjectsSortMode.None).All(v => !v.actorMarker.activeSelf && !v.targetMarker.activeSelf));
            yield return Capture("poison-lethal-defeat", 1280, 720);
            yield return Press(Key.Enter); AssertFreshBattle();
        }

        private IEnumerator WaitForPlayerCommands()
        {
            float deadline = Time.realtimeSinceStartup + 15;
            while ((Battle.Phase != PrototypePhase.Commands || Battle.Session.Actor?.Definition.Team != CombatTeam.Allies) && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.AreEqual(PrototypePhase.Commands, Battle.Phase); Assert.AreEqual(CombatTeam.Allies, Battle.Session.Actor.Definition.Team);
        }

        private IEnumerator PlayToResult(bool pad, bool useResources = true)
        {
            Time.timeScale = 4;
            float deadline = Time.realtimeSinceStartup + 180;
            while (Battle.Phase != PrototypePhase.Result && Time.realtimeSinceStartup < deadline)
            {
                if (Battle.Phase == PrototypePhase.Commands && Battle.Session.Actor.Definition.Team == CombatTeam.Allies)
                {
                    var s = Battle.Session;
                    bool potion = useResources && s.Potions > 0 && s.Fighters.Any(f => f.Definition.Team == CombatTeam.Allies && f.IsAlive && f.Health <= f.Definition.MaxHealth - 50);
                    bool heal = useResources && !potion && s.Actor.Definition.Id == "mage" && s.Actor.Mana >= 10 &&
                        s.Fighters.Any(f => f.IsAlive && f.Definition.Team == CombatTeam.Allies && f.Health <= f.Definition.MaxHealth - 36);
                    bool heavy = useResources && !potion && s.Actor.Definition.Id == "warrior" && s.Actor.Mana >= 8;
                    bool storm = useResources && !potion && !heal && s.Actor.Definition.Id == "mage" && s.Actor.Mana >= 18 &&
                        s.Fighters.Count(f => f.IsAlive && f.Definition.Team == CombatTeam.Enemies) == 2;
                    CombatAction chosen = potion ? CombatAction.Potion : heal ? CombatAction.Heal : heavy ? CombatAction.HeavyStrike : storm ? CombatAction.ArcaneStorm : CombatAction.Basic;
                    int desired = potion ? 2 : chosen != CombatAction.Basic ? 1 : 0;
                    while (Battle.SelectedCommand != desired) yield return Direction(pad, false);
                    yield return Confirm(pad);
                    if (chosen != CombatAction.Basic)
                    {
                        var commands = s.AvailableCommands().Where(a => potion ? CombatSession.IsItem(a) : !CombatSession.IsItem(a) && a != CombatAction.Basic).ToArray();
                        int selected = System.Array.IndexOf(commands, chosen);
                        while (Battle.SelectedCommand != selected) yield return Direction(pad, false);
                        yield return Confirm(pad);
                    }
                    Assert.AreEqual(PrototypePhase.Targets, Battle.Phase);
                    var wanted = potion || heal ? s.Fighters.Where(f => f.Definition.Team == CombatTeam.Allies && f.IsAlive && f.Health < f.Definition.MaxHealth)
                        .OrderBy(f => (float)f.Health / f.Definition.MaxHealth).First()
                        : storm ? Battle.SelectedTarget : s.Fighters.Where(f => f.Definition.Team == CombatTeam.Enemies && f.IsAlive).OrderBy(f => f.Definition.Id == "shaman" ? 0 : 1).First();
                    int steps = 0;
                    while (Battle.SelectedTarget != wanted && steps++ < 4) yield return Direction(pad, true);
                    Assert.AreSame(wanted, Battle.SelectedTarget);
                    int completedBefore = s.CompletedActions;
                    yield return Confirm(pad);
                    Assert.IsTrue(Battle.Phase == PrototypePhase.Executing || s.CompletedActions > completedBefore ||
                        Battle.Phase == PrototypePhase.Result,
                        "Confirmation must begin or finish exactly one action before the next ATB advance.");
                }
                yield return null;
            }
            Assert.AreEqual(PrototypePhase.Result, Battle.Phase, "Combat must finish before deadline.");
            Assert.IsNull(Battle.Session.Pending);
            Time.timeScale = previousTimeScale;
        }

        private void AssertFreshBattle()
        {
            Assert.AreEqual(PrototypePhase.Advancing, Battle.Phase);
            Assert.IsFalse(Battle.IsPaused);
            Assert.IsNull(Battle.Session.Pending);
            Assert.AreEqual(3, Battle.Session.Potions);
            Assert.AreEqual(0, Battle.Session.CompletedActions);
            Assert.IsNull(Battle.Session.Actor);
            foreach (var f in Battle.Session.Fighters)
            { Assert.AreEqual(f.Definition.MaxHealth, f.Health); Assert.AreEqual(f.Definition.MaxMana, f.Mana); Assert.AreEqual(0, f.RecoveryRemaining); Assert.Less(f.Atb, 25); }
            foreach (var view in Object.FindObjectsByType<CombatantView>(FindObjectsSortMode.None))
            {
                var prefab = Battle.Encounter.participants.First(s => s.combatant.id == view.combatantId).combatant.prefab.GetComponent<CombatantView>();
                Assert.Less(Quaternion.Angle(prefab.visual.localRotation, view.visual.localRotation), 0.01f);
                Assert.Less(Vector3.Distance(prefab.visual.localScale, view.visual.localScale), 0.001f);
            }
        }

        [UnityTest]
        public IEnumerator DisconnectDuringStormPausesAndResumeDoesNotRepeatImpact()
        {
            Assert.AreEqual("mage", Battle.Session.Actor.Definition.Id);
            yield return Pad(GamepadButton.DpadDown);
            yield return Pad(GamepadButton.South);
            Assert.AreEqual(CombatMenu.Skills, Battle.Menu);
            int stormIndex = System.Array.IndexOf(Battle.DisplayedCommands, CombatAction.ArcaneStorm);
            Assert.GreaterOrEqual(stormIndex, 0);
            for (int i = 0; i < stormIndex; i++) yield return Pad(GamepadButton.DpadDown);
            yield return Pad(GamepadButton.South);
            Assert.AreEqual(PrototypePhase.Targets, Battle.Phase);

            var enemies = Battle.Session.Fighters.Where(f => f.Definition.Team == CombatTeam.Enemies && f.IsAlive).ToArray();
            var expectedDamage = enemies.ToDictionary(f => f, f => CombatSession.CalculateDamage(
                Battle.Session.Actor.Definition.Attack, Battle.Session.ActionFor(CombatAction.ArcaneStorm).Potency, f.EffectiveDefense));
            yield return Pad(GamepadButton.South);
            Assert.AreEqual(PrototypePhase.Executing, Battle.Phase);
            Assert.AreEqual(27, Battle.Session.Actor.Mana);

            InputSystem.RemoveDevice(gamepad); gamepad = null;
            yield return null; yield return null;
            Assert.IsTrue(Battle.IsPaused);
            Assert.AreEqual(4, Battle.PausePage);
            yield return Capture("h11-controller-disconnected", 1280, 720);
            float frozen = Battle.ActionTime;
            foreach (var enemy in enemies) Assert.AreEqual(enemy.Definition.MaxHealth, enemy.Health);
            yield return new WaitForSecondsRealtime(.3f);
            Assert.AreEqual(frozen, Battle.ActionTime, .001f, "The special sequence remains paused after disconnect.");

            yield return Press(Key.Enter);
            Assert.IsFalse(Battle.IsPaused);
            float deadline = Time.realtimeSinceStartup + 5;
            while (Battle.Phase == PrototypePhase.Executing && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.AreNotEqual(PrototypePhase.Executing, Battle.Phase);
            foreach (var enemy in enemies) Assert.AreEqual(enemy.Definition.MaxHealth - expectedDamage[enemy], enemy.Health,
                "Each special target receives exactly one impact across disconnect and resume.");
        }

        [UnityTest]
        public IEnumerator RemovedTargetReturnsFocusToFirstRemainingValidTarget()
        {
            Assert.AreEqual(PrototypePhase.Commands, Battle.Phase);
            yield return Press(Key.Enter);
            Assert.AreEqual(PrototypePhase.Targets, Battle.Phase);
            var first = Battle.SelectedTarget;
            Assert.IsNotNull(first);
            string removedId = first.Definition.Id;
            first.Damage(first.Health);
            yield return null;

            Assert.IsNotNull(Battle.SelectedTarget);
            Assert.AreNotEqual(removedId, Battle.SelectedTarget.Definition.Id);
            var firstRemaining = Battle.Session.TargetsFor(CombatAction.Basic).OrderBy(f => Camera.main.WorldToViewportPoint(
                Object.FindObjectsByType<CombatantView>(FindObjectsSortMode.None).First(v => v.combatantId == f.Definition.Id).transform.position).x).First();
            Assert.AreSame(firstRemaining, Battle.SelectedTarget,
                "If the selected target disappears, focus moves to the first valid target.");
        }

        [UnityTest]
        public IEnumerator CancelingLeaveAndRestartConfirmationsKeepsPauseAndCombat()
        {
            var originalSession = Battle.Session;
            yield return Press(Key.Escape);
            Assert.IsTrue(Battle.IsPaused);
            for (int i = 0; i < 4; i++) yield return Press(Key.DownArrow);
            yield return Press(Key.Enter);
            Assert.AreEqual(3, Battle.PausePage);
            Assert.AreEqual(0, Battle.ConfirmationSelection, "Cancelar is selected by default.");
            yield return Press(Key.Enter);
            Assert.AreEqual("Combat", SceneManager.GetActiveScene().name);
            Assert.IsTrue(Battle.IsPaused);
            Assert.AreSame(originalSession, Battle.Session, "Canceling return preserves the same attempt and pause.");

            yield return Press(Key.UpArrow);
            yield return Press(Key.Enter);
            Assert.AreEqual(3, Battle.PausePage);
            yield return Press(Key.Enter);
            Assert.IsTrue(Battle.IsPaused);
            Assert.AreSame(originalSession, Battle.Session, "Canceling restart preserves the same attempt and pause.");
        }

        private IEnumerator RestartFromPause()
        {
            for (int i = 0; i < 3; i++) yield return Press(Key.DownArrow);
            yield return Press(Key.Enter);
            Assert.AreEqual(3, Battle.PausePage);
            yield return Press(Key.DownArrow); yield return Press(Key.Enter);
        }
        private IEnumerator ReturnFromPause()
        {
            for (int i = 0; i < 4; i++) yield return Press(Key.DownArrow);
            yield return Press(Key.Enter);
            Assert.AreEqual(3, Battle.PausePage);
            yield return Press(Key.DownArrow); yield return Press(Key.Enter);
        }
        private IEnumerator WaitForCommands()
        {
            float deadline = Time.realtimeSinceStartup + 15;
            while (Battle.Phase != PrototypePhase.Commands && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.AreEqual(PrototypePhase.Commands, Battle.Phase);
        }

        private IEnumerator Direction(bool pad, bool horizontal) => pad ? Pad(horizontal ? GamepadButton.DpadRight : GamepadButton.DpadDown) : Press(horizontal ? Key.RightArrow : Key.DownArrow);
        private IEnumerator Confirm(bool pad) => pad ? Pad(GamepadButton.South) : Press(Key.Enter);
        private IEnumerator Press(Key key)
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(key));
            yield return null; yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return null; yield return null;
        }
        private IEnumerator Pad(GamepadButton button)
        {
            InputSystem.QueueStateEvent(gamepad, new GamepadState().WithButton(button));
            yield return null; yield return null;
            InputSystem.QueueStateEvent(gamepad, new GamepadState());
            yield return null; yield return null;
        }

        private IEnumerator Capture(string name, int width, int height)
        {
            yield return null;
            var camera = Camera.main;
            var canvas = Object.FindFirstObjectByType<Canvas>();
            var oldMode = canvas.renderMode; var oldCamera = canvas.worldCamera;
            float oldDistance = canvas.planeDistance;
            var oldTarget = camera.targetTexture; var oldActive = RenderTexture.active;
            var render = RenderTexture.GetTemporary(width, height, 24);
            Texture2D image = null;
            try
            {
                camera.targetTexture = render;
                canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 1;
                yield return null;
                Canvas.ForceUpdateCanvases();
                camera.Render(); RenderTexture.active = render;
                image = new Texture2D(width, height, TextureFormat.RGB24, false);
                image.ReadPixels(new Rect(0, 0, width, height), 0, 0); image.Apply();
                var directory = InterfaceEvidence.Root;
                Directory.CreateDirectory(directory);
                File.WriteAllBytes(Path.Combine(directory, name + ".png"), image.EncodeToPNG());
            }
            finally
            {
                canvas.renderMode = oldMode; canvas.worldCamera = oldCamera; canvas.planeDistance = oldDistance;
                camera.targetTexture = oldTarget; RenderTexture.active = oldActive;
                if (image != null) Object.Destroy(image);
                RenderTexture.ReleaseTemporary(render);
            }
        }
    }
}
