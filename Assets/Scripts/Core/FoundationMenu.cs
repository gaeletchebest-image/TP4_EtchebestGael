using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace UltimaGuardia
{
    // H00 scene/input scaffold. Combat and final menus belong to later milestones.
    public sealed class FoundationMenu : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputTemplate;
        [SerializeField] private InputSystemUIInputModule inputModule;
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject controlsPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject firstButton;
        [SerializeField] private GameObject controlsBackButton;
        [SerializeField] private GameObject pauseContinueButton;
        [SerializeField] private bool combatScene;

        private InputActionAsset runtimeInput;
        private InputActionReference moveReference;
        private InputActionReference submitReference;
        private GameObject previousSelection;
        private bool transitioning;

        public bool IsPaused => pausePanel != null && pausePanel.activeSelf;
        public bool ControlsOpen => controlsPanel != null && controlsPanel.activeSelf;

        public void Configure(InputActionAsset input, InputSystemUIInputModule module,
            GameObject main, GameObject controls, GameObject pause, GameObject first,
            GameObject controlsBack, GameObject pauseContinue, bool isCombat)
        {
            inputTemplate = input;
            inputModule = module;
            mainPanel = main;
            controlsPanel = controls;
            pausePanel = pause;
            firstButton = first;
            controlsBackButton = controlsBack;
            pauseContinueButton = pauseContinue;
            combatScene = isCombat;
        }

        private void Awake()
        {
            runtimeInput = Instantiate(inputTemplate);
            inputModule.actionsAsset = runtimeInput;
            moveReference = InputActionReference.Create(runtimeInput.FindAction("UI/Navigate", true));
            submitReference = InputActionReference.Create(runtimeInput.FindAction("UI/Confirm", true));
            inputModule.move = moveReference;
            inputModule.submit = submitReference;
            // Cancel and pause share Escape and are resolved once, by context, below.
            inputModule.cancel = null;
            inputModule.moveRepeatDelay = 0.4f;
            inputModule.moveRepeatRate = 0.12f;
        }

        private void OnEnable() => runtimeInput.Enable();
        private void OnDisable() => runtimeInput.Disable();

        private IEnumerator Start()
        {
            yield return null;
            Select(firstButton);
        }

        private void Update()
        {
            if (transitioning) return;
            bool cancel = runtimeInput.FindAction("UI/Cancel").WasPressedThisFrame();
            bool pause = runtimeInput.FindAction("UI/Pause").WasPressedThisFrame();
            if (ControlsOpen && cancel) CloseControls();
            else if (combatScene && (pause || cancel)) TogglePause();
        }

        public void OpenControls()
        {
            previousSelection = EventSystem.current.currentSelectedGameObject;
            mainPanel.SetActive(false);
            controlsPanel.SetActive(true);
            Select(controlsBackButton);
        }

        public void CloseControls()
        {
            controlsPanel.SetActive(false);
            mainPanel.SetActive(true);
            Select(previousSelection != null ? previousSelection : firstButton);
        }

        public void TogglePause()
        {
            bool opening = !IsPaused;
            mainPanel.SetActive(!opening);
            pausePanel.SetActive(opening);
            Select(opening ? pauseContinueButton : firstButton);
        }

        public void Play() => Load("Combat");
        public void ReturnToMenu() => Load("MainMenu");

        private void Load(string scene)
        {
            if (transitioning) return;
            transitioning = true;
            SceneManager.LoadSceneAsync(scene);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private static void Select(GameObject button)
        {
            if (EventSystem.current == null) return;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(button);
        }

        private void OnApplicationFocus(bool focused)
        {
            if (focused && EventSystem.current != null &&
                EventSystem.current.currentSelectedGameObject == null)
                Select(ControlsOpen ? controlsBackButton : IsPaused ? pauseContinueButton : firstButton);
        }

        private void OnDestroy()
        {
            Destroy(moveReference);
            Destroy(submitReference);
            Destroy(runtimeInput);
        }
    }
}
