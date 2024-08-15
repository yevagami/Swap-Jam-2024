using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public PlayerController player;
    public BossBehaviour boss;

    public Image healthComponent;
    public Image manaComponent;
    public Image bossHealthComponent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bossHealthComponent.fillAmount = boss.currentHealth / boss.baseHealth;
        healthComponent.fillAmount = (float)player.health / (float)player.baseHealth;
        manaComponent.fillAmount = player.MP / player.maxMP;
    }
}
