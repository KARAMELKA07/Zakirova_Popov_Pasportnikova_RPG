using UnityEngine;
using UnityEngine.UI;

public class PlayerUIView : MonoBehaviour
{
    [Header("Health")]
    public Slider hpBar;
    public Text hpText;

    [Header("Magic Cooldown")]
    public Image magicCooldownImage;
}