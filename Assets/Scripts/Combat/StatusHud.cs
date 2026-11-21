using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UltimaGuardia
{
    public sealed class StatusHud
    {
        private readonly GameObject[] rows = new GameObject[3];
        private readonly Text[] labels = new Text[3];
        public StatusHud(Text owner)
        {
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = new GameObject("Status_" + (StatusKind)i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                rows[i].GetComponent<Image>().color = new Color(0.035f, 0.055f, 0.085f, 1);
                rows[i].GetComponent<Image>().raycastTarget = false;
                var rect = rows[i].GetComponent<RectTransform>(); rect.SetParent(owner.transform, false);
                rect.anchoredPosition = new Vector2(0, -80 - i * 22); rect.sizeDelta = new Vector2(300, 22);
                var icon = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(StatusIcon)).GetComponent<StatusIcon>();
                icon.rectTransform.SetParent(rect, false); icon.rectTransform.anchoredPosition = new Vector2(-138, 0);
                icon.rectTransform.sizeDelta = new Vector2(20, 20); icon.Kind = (StatusKind)i;
                icon.color = i == 0 ? new Color(0.4f, 1, 0.5f) : i == 1 ? new Color(0.45f, 0.75f, 1) : new Color(1, 0.65f, 0.3f);
                icon.raycastTarget = false;
                labels[i] = new GameObject("NameAndCounter", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<Text>();
                labels[i].rectTransform.SetParent(rect, false); labels[i].rectTransform.anchoredPosition = new Vector2(12, 0);
                labels[i].rectTransform.sizeDelta = new Vector2(272, 22);
                labels[i].font = owner.font; labels[i].fontSize = 17; labels[i].color = Color.white;
                labels[i].alignment = TextAnchor.MiddleLeft; labels[i].raycastTarget = false;
                rows[i].SetActive(false);
            }
        }
        public void Present(CombatantState fighter)
        {
            for (int i = 0; i < rows.Length; i++) rows[i].SetActive(false);
            int row = 0;
            foreach (var effect in fighter.Effects.OrderBy(e => e.Definition.Kind))
            {
                int i = (int)effect.Definition.Kind;
                rows[i].SetActive(true);
                ((RectTransform)rows[i].transform).anchoredPosition = new Vector2(0, -80 - row++ * 22);
                labels[i].text = effect.Definition.Name + " · " + effect.Remaining +
                    (effect.Definition.Trigger == EffectTrigger.TurnStart ? effect.Remaining == 1 ? " activación" : " activaciones" : effect.Remaining == 1 ? " acción" : " acciones");
            }
        }
    }
}
