using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject moneyPanel, bankHiestPanel, attackPanel, shieldPanel;
    public GameObject[] AllRewardPanels;
    public TextMeshProUGUI globalMoney;
    void Start()
    {
        instance = this;
    }

    public void DisableAllRewardedPanels()
    {
        foreach (GameObject panel in AllRewardPanels)
        {
            panel.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMoney(float money)
    {

        StartCoroutine(TypewriterEffect(globalMoney, GlobalData.GetMoney().ToString()));
    }

    private IEnumerator TypewriterEffect(TextMeshProUGUI textElement, string targetText, float delay = 0.05f)
    {
        textElement.text = ""; // Clear current text
        foreach (char c in targetText)
        {
            textElement.text += c; // Append each character
            yield return new WaitForSeconds(delay); // Wait before adding the next character
        }
    }
}
