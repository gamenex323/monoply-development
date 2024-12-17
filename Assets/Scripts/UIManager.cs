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
    public TextMeshProUGUI moneyPanelText;
    public float durationOfMoneyEffect = 2f;
    void Start()
    {
        instance = this;
        UpdateMoney(0);
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

    public void UpdateMoney(int money)
    {
        int currentMoney = GlobalData.GetMoney();
        GlobalData.SetMoney(money);
        int targetMoney = GlobalData.GetMoney();
        print("Money Update Complete");
        StartCoroutine(TypewriterEffect(globalMoney, currentMoney, targetMoney, durationOfMoneyEffect));
    }

    private IEnumerator TypewriterEffect(TextMeshProUGUI textElement, float currentMoney, float targetMoney, float duration)
    {
        print("Money Update Complete");
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration); // Ensure progress stays between 0 and 1
            float updatedMoney = Mathf.Lerp(currentMoney, targetMoney, progress);

            textElement.text = Mathf.RoundToInt(updatedMoney).ToString();
            yield return null;
        }

        // Ensure the final value is set to targetMoney
        textElement.text = Mathf.RoundToInt(targetMoney).ToString();
        print("Money Update Complete");
    }

}
