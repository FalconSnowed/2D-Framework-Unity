using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class UIManager : MonoBehaviour
{
    public Slider healthSlider;
    public UnityEngine.UI.Text gameOverText;
    public UnityEngine.UI.Text winText;

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthSlider.value = currentHealth / maxHealth;
    }

    public void ShowGameOver()
    {
        gameOverText.gameObject.SetActive(true);
    }

    public void ShowWinScreen()
    {
        winText.gameObject.SetActive(true);
    }
}
