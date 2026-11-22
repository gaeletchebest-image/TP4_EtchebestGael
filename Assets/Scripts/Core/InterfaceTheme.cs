using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UltimaGuardia
{
    // Shared, code-native design system. No external artwork or font dependency.
    public static class InterfaceTheme
    {
        public static readonly Color Ink = new Color32(14, 23, 34, 250);
        public static readonly Color Panel = new Color32(23, 36, 49, 250);
        public static readonly Color Gold = new Color32(232, 194, 119, 255);
        public static readonly Color Muted = new Color32(178, 194, 205, 255);
        public static readonly Color Life = new Color32(115, 216, 164, 255);
        public static readonly Color Mana = new Color32(116, 184, 244, 255);
        public static readonly Color Danger = new Color32(255, 151, 133, 255);
        public static Font Font => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        public static RectTransform Rect(Transform parent, string name, float x, float y, float w, float h)
        {
            var r = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            r.SetParent(parent, false); r.sizeDelta = new Vector2(w, h); r.anchoredPosition = new Vector2(x, y);
            return r;
        }
        public static Image Box(Transform parent, string name, float x, float y, float w, float h, Color color)
        {
            var image = Rect(parent, name, x, y, w, h).gameObject.AddComponent<Image>();
            image.color = color; image.raycastTarget = false; return image;
        }
        public static Text Text(Transform parent, string name, string value, int size, float x, float y, float w, float h,
            TextAnchor align = TextAnchor.MiddleLeft)
            => CreateText(parent, name, value, size, x, y, w, h, align);
        public static Text MenuText(Transform parent, string name, string value, int size, float x, float y, float w, float h,
            TextAnchor align = TextAnchor.MiddleLeft)
        {
            // Native Text generates twice as many glyph pixels; the transform restores
            // the authored visual size. Keep native font atlas rebuilds and mesh handling.
            var text = CreateText(parent, name, value, size * 2, x, y, w * 2, h * 2, align);
            text.rectTransform.localScale = Vector3.one * .5f;
            return text;
        }
        private static Text CreateText(Transform parent, string name, string value, int size, float x, float y, float w, float h,
            TextAnchor align)
        {
            var go = Rect(parent, name, x, y, w, h).gameObject;
            var t = go.AddComponent<Text>();
            t.font = Font; t.fontSize = size; t.text = value; t.color = Color.white; t.alignment = align;
            t.raycastTarget = false; t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Truncate;
            return t;
        }
        public static Image Icon(Transform parent, string id, float x, float y, float size, Color color)
        {
            var image = Box(parent, "Icon_" + id, x, y, size, size, color);
            image.sprite = Resources.Load<Sprite>("Interface/" + id); image.preserveAspect = true;
            return image;
        }
        public static Image Bar(Transform parent, string id, float x, float y, float width, Color color)
        {
            Box(parent, id + "Track", x, y, width, 5, new Color32(53, 69, 82, 255));
            var bar = Box(parent, id, x, y, width, 5, color);
            bar.sprite = Resources.Load<Sprite>("Interface/bar"); bar.type = Image.Type.Filled;
            bar.fillMethod = Image.FillMethod.Horizontal; return bar;
        }
    }

    public static class SessionOptions
    {
        public static int Volume { get; private set; } = 100;
        public static bool UsingGamepad { get; private set; }
        public static InputDevice LastInputDevice { get; private set; }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() { Volume = 100; UsingGamepad = false; LastInputDevice = null; AudioListener.volume = 1; }
        public static void SetVolume(int value) { Volume = Mathf.Clamp(value, 0, 100) / 10 * 10; AudioListener.volume = Volume / 100f; }
        public static void Observe(InputAction navigation, InputAction confirm, InputAction cancel, InputAction pause)
        {
            foreach (var action in new[] { navigation, confirm, cancel, pause })
                if (action.WasPerformedThisFrame() && action.activeControl != null)
                {
                    LastInputDevice = action.activeControl.device;
                    UsingGamepad = LastInputDevice is Gamepad;
                }
        }
        public static void ForgetDevice(InputDevice device)
        {
            if (ReferenceEquals(LastInputDevice, device)) { LastInputDevice = null; UsingGamepad = false; }
        }
        public static string Help => UsingGamepad
            ? "Cruceta / stick: navegar     Botón inferior: confirmar     Botón derecho: volver     Menú: pausa"
            : "Flechas: navegar     Enter / Espacio: confirmar     Escape: volver / pausa";
        public static string[] HelpTitles = { "Tu última defensa", "El tiempo y los turnos", "Recursos y estados", "Controles" };
        public static string HelpBody(int page) => page == 0
            ? "Derrotá al ogro y al chamán antes de perder a tus dos aliados.\n\nEl guerrero combina resistencia y golpes fuertes. La maga puede atacar, curar o dañar al grupo enemigo.\n\nConcentrar ataques en el chamán reduce curaciones y veneno. Eliminar al ogro reduce el daño recibido."
            : page == 1
            ? "La barra ATB se llena según la velocidad de cada personaje. Al quedar alguien listo, el reloj se detiene.\n\nElegí sin apuro: decidir y cancelar no consumen tiempo. Se ejecuta una acción por vez.\n\nDespués de actuar, se cumple la recuperación de la acción antes de volver a llenar ATB."
            : page == 2
            ? "Los básicos son gratuitos. Las habilidades gastan maná; no hay regeneración pasiva. Ambos aliados comparten cinco tipos de objetos limitados.\n\nVeneno: 8 de daño al inicio de 3 turnos. Antídoto lo elimina. Rapidez: velocidad ×1,5 por 2 acciones. Corrosión: defensa ×0,5 por 2 acciones.\n\nLos caídos no actúan ni pueden revivir. Reiniciar restaura el intento."
            : UsingGamepad
            ? "Cruceta o stick izquierdo: arriba / abajo para listas; izquierda / derecha para objetivos, páginas y volumen.\n\nBotón inferior (A / cruz): confirmar.\nBotón derecho (B / círculo): volver.\nBotón de menú (Start / Options): abrir o cerrar pausa.\n\nPodés cambiar a teclado durante la partida."
            : "Flechas arriba / abajo: listas.\nFlechas izquierda / derecha: objetivos, páginas y volumen.\nEnter o Espacio: confirmar.\nEscape: volver un nivel; desde la raíz o durante una acción, abrir pausa.\n\nPodés cambiar a control durante la partida.";
    }
}
