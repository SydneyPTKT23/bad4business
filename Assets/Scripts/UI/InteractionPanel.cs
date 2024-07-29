using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SLC.Bad4Business.UI
{
    public class InteractionPanel : MonoBehaviour
    {
        [SerializeField] private Image heldProgressImage;
        [SerializeField] private TextMeshProUGUI tooltipLabel;



        public void SetLabel(string t_message)
        {
            tooltipLabel.SetText(t_message);
        }

        public void UpdateProgressBar(float t_percent)
        {
            heldProgressImage.fillAmount = t_percent;
        }

        public void ResetUI()
        {
            heldProgressImage.fillAmount = 0.0f;
            tooltipLabel.SetText("");
        }
    }
}