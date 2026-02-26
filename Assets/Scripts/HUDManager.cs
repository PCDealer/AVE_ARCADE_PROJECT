using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("Health UI")]
    public Image healthFill;
    public PlayerHealth playerHealth;

    [Header("Dash UI")]
    public Image dashFill;
    public Player playerScript;
    public Text dashCountText; 

    void Update()
    {
        if (playerHealth != null && healthFill != null)
        {
            float targetFill = playerHealth.playerHealth / playerHealth.maxHealth;
            healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, targetFill, Time.deltaTime * 10f);
        }

        if (playerScript != null && dashFill != null)
        {
            float currentDashes = playerScript.GetCurrentDashes();
            float currentCharge = playerScript.GetDashChargeProgress(); 
            
            float totalDashPower = (currentDashes + currentCharge) / playerScript.maxDashes;
            
            dashFill.fillAmount = totalDashPower;

            if(dashCountText != null)
                dashCountText.text = "DASH x" + currentDashes;
        }
    }
}