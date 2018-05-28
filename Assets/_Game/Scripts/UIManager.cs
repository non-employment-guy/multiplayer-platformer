using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public Text HealthText;

    public Image ShieldBuff;

    public Image FreezeBuff;

    public Image DamageBuff;

    public Text EnemiesCountText;

    public Text EnemiesKilledText;

    public Player LocalPlayer;

    public Health PlayerHealth;

    void Start()
    {
       
    }   

    void Update()
    {
        if (LocalPlayer == null) return;
        HealthText.text = PlayerHealth.HealthValue + " HP";

        //   EnemiesCountText.text = "Enemies Count: " + GameManager.Instance.Enemies.Count;

        EnemiesKilledText.text = "Enemies killed: " + LocalPlayer.Kills;

        ShieldBuff.enabled = GameManager.Instance.ShieldBuffActive;
        FreezeBuff.enabled = GameManager.Instance.FreezeBuffActive;
        DamageBuff.enabled = GameManager.Instance.DamageBuffActive;
    }
}