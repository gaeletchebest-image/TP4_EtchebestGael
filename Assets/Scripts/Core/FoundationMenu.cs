using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UltimaGuardia
{
    public sealed class FoundationMenu : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputTemplate;
        [SerializeField] private InputSystemUIInputModule inputModule;
        [SerializeField] private GameObject mainPanel, controlsPanel, pausePanel, firstButton, controlsBackButton, pauseContinueButton;
        [SerializeField] private bool combatScene;
        [SerializeField] private Button[] mainButtons;
        [SerializeField] private Text pageTitle, pageBody, pageFooter, deviceHelp;
        [SerializeField] private Image deviceIndicator;
        private InputActionAsset runtimeInput;
        private InputAction navigate, confirm, cancel, pause;
        private int index, page;
        private bool transitioning, armed;
        private Vector2 previousDirection;
        private float repeatAt;
        private bool volumeOpen;
        public bool IsPaused => pausePanel != null && pausePanel.activeSelf;
        public bool ControlsOpen => controlsPanel != null && controlsPanel.activeSelf && !volumeOpen;
        public bool VolumeOpen => controlsPanel != null && controlsPanel.activeSelf && volumeOpen;
        public int HelpPage => page;

        public void Configure(InputActionAsset input, InputSystemUIInputModule module, GameObject main, GameObject controls,
            GameObject pauseObject, GameObject first, GameObject controlsBack, GameObject pauseContinue, bool isCombat)
        {
            inputTemplate = input; inputModule = module; mainPanel = main; controlsPanel = controls; pausePanel = pauseObject;
            firstButton = first; controlsBackButton = controlsBack; pauseContinueButton = pauseContinue; combatScene = isCombat;
        }
        public void ConfigureFinal(Button[] buttons, Text title, Text body, Text footer, Text help, Image indicator)
        { mainButtons = buttons; pageTitle = title; pageBody = body; pageFooter = footer; deviceHelp = help; deviceIndicator = indicator; }
        private void Awake()
        {
            runtimeInput = Instantiate(inputTemplate);
            navigate = runtimeInput.FindAction("UI/Navigate", true); confirm = runtimeInput.FindAction("UI/Confirm", true);
            cancel = runtimeInput.FindAction("UI/Cancel", true); pause = runtimeInput.FindAction("UI/Pause", true);
            // One input owner prevents inherited submit and duplicate EventSystem navigation.
            inputModule.enabled = false;
        }
        private void OnEnable() => runtimeInput.Enable();
        private void OnDisable() { if (runtimeInput != null) runtimeInput.Disable(); }
        private IEnumerator Start() { yield return null; Select(firstButton); }
        private void Update()
        {
            if (transitioning) return;
            SessionOptions.Observe(navigate, confirm, cancel, pause);
            if (!confirm.IsPressed()) armed = true;
            bool submit = armed && confirm.WasPressedThisFrame(); if (submit) armed = false;
            var direction = navigate.ReadValue<Vector2>();
            direction = new Vector2(Mathf.Abs(direction.x) > .5f ? Mathf.Sign(direction.x) : 0, Mathf.Abs(direction.y) > .5f ? Mathf.Sign(direction.y) : 0);
            bool move = direction != Vector2.zero && (direction != previousDirection || Time.unscaledTime >= repeatAt);
            if (direction != previousDirection) repeatAt = Time.unscaledTime + .4f;
            else if (move) repeatAt = Time.unscaledTime + .12f;
            previousDirection = direction;
            if (controlsPanel.activeSelf)
            {
                if (move && direction.x != 0)
                {
                    GameAudio.Current.Ui("ui_move");
                    if (volumeOpen) SessionOptions.SetVolume(SessionOptions.Volume + (int)direction.x * 10);
                    else page = (page + (int)direction.x + 4) % 4;
                }
                if (cancel.WasPressedThisFrame() || submit) { GameAudio.Current.Ui(cancel.WasPressedThisFrame() ? "ui_cancel" : "ui_confirm"); CloseControls(); }
            }
            else
            {
                if (move && direction.y != 0) { index = (index - (int)direction.y + mainButtons.Length) % mainButtons.Length; Select(mainButtons[index].gameObject); GameAudio.Current.Ui("ui_move"); }
                if (submit) { GameAudio.Current.Ui("ui_confirm"); mainButtons[index].onClick.Invoke(); }
            }
            Present();
        }
        private void Present()
        {
            deviceHelp.text = SessionOptions.Help;
            deviceIndicator.sprite = Resources.Load<Sprite>("Interface/" + (SessionOptions.UsingGamepad ? "gamepad" : "keyboard"));
            if (!controlsPanel.activeSelf) return;
            pageTitle.text = volumeOpen ? "Volumen general" : SessionOptions.HelpTitles[page];
            pageBody.text = volumeOpen ? SessionOptions.Volume + " %\n\n←  Disminuir     Aumentar  →\n\nEl volumen se conserva durante esta sesión.\nAfecta a música, combate e interfaz." : SessionOptions.HelpBody(page);
            pageFooter.text = volumeOpen ? "Pasos de 10 · 0 = silencio" : "←  Página " + (page + 1) + " / 4  →";
        }
        public void OpenControls() { volumeOpen = false; page = 0; OpenPage(); }
        public void OpenVolume() { volumeOpen = true; OpenPage(); }
        private void OpenPage() { mainPanel.SetActive(false); controlsPanel.SetActive(true); armed = false; Select(controlsBackButton); Present(); }
        public void CloseControls() { controlsPanel.SetActive(false); mainPanel.SetActive(true); armed = false; Select(mainButtons[index].gameObject); }
        public void TogglePause() { if (pausePanel != null) pausePanel.SetActive(!pausePanel.activeSelf); }
        public void Play() => Load("Combat");
        public void ReturnToMenu() => Load("MainMenu");
        private void Load(string scene) { if (transitioning) return; transitioning = true; SceneManager.LoadSceneAsync(scene); }
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        private static void Select(GameObject button)
        { if (EventSystem.current == null) return; EventSystem.current.SetSelectedGameObject(null); EventSystem.current.SetSelectedGameObject(button); }
        private void OnApplicationFocus(bool focused)
        { if (focused && EventSystem.current != null) Select(controlsPanel.activeSelf ? controlsBackButton : mainButtons[index].gameObject); }
        private void OnDestroy() { if (runtimeInput != null) Destroy(runtimeInput); }
    }
}
