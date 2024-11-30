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
}
