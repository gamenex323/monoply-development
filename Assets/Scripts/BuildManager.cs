using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Building
{
    public string buildingName;            // Name of the building
    public int currentLevel;               // Current level of the building
    public int maxLevel;                   // Max upgrade level allowed
    public int[] upgradeCosts;             // Cost for upgrading each level (index corresponds to level)
    public GameObject[] buildingLevels;    // Array of GameObjects for each level of the building (index corresponds to level)
    public Button upgradeButton;           // Reference to the upgrade button
    public TextMeshProUGUI upgradeCostText;  // Reference to the TextMeshPro UI text to show the cost
    public Image buildingImage;            // Reference to the Image component to show the current building level image
    public Sprite[] buildingSprites;       // Array of sprites for each building level
}


public class BuildManager : MonoBehaviour
{
    public Building[] buildings;  // Array of buildings in the scene
    private TextMeshProUGUI moneyText;  // UI Text to show player's money

    void Start()
    {
        Invoke(nameof(Init), 1f);
    }

    void Init()
    {
        moneyText = UIManager.instance.globalMoney;
        LoadBuildingData();
        UpdateMoneyText();
    }

    void Update()
    {
        // Additional update logic if necessary
    }

    // Call this method when the player clicks an upgrade button
    public void OnUpgradeClick(int buildingIndex)
    {
        Building building = buildings[buildingIndex];
        int playerMoney = GetMoney();  // Get the player's current money from PlayerPrefs

        if (building.currentLevel < building.maxLevel)
        {
            int upgradeCost = building.upgradeCosts[building.currentLevel];
            if (playerMoney >= upgradeCost)
            {
                // Deduct money and upgrade the building
                playerMoney -= upgradeCost;
                SetMoney(playerMoney);  // Save the updated money to PlayerPrefs

                // Change the building object for the new level
                ChangeBuildingAppearance(building);

                // Upgrade the level
                building.currentLevel++;
                SaveBuildingData();
                UpdateMoneyText();

                // Update the button cost text and image after the upgrade
                UpdateUpgradeButtonText(building);
                UpdateBuildingImage(building);
            }
            else
            {
                Debug.Log("Not enough money to upgrade!");
            }
        }
        else
        {
            Debug.Log("Building is already at max level!");
        }
    }

    // Update the player's money text
    void UpdateMoneyText()
    {
        int playerMoney = GetMoney();  // Get the player's money
        if (moneyText != null)
        {
            moneyText.text = "Money: " + playerMoney.ToString();
        }
    }

    // Save the building data (current levels)
    void SaveBuildingData()
    {
        for (int i = 0; i < buildings.Length; i++)
        {
            PlayerPrefs.SetInt("Building_" + buildings[i].buildingName + "_Level", buildings[i].currentLevel);
        }
        PlayerPrefs.Save();
    }

    // Load the building data (current levels) from PlayerPrefs
    void LoadBuildingData()
    {
        for (int i = 0; i < buildings.Length; i++)
        {
            int savedLevel = PlayerPrefs.GetInt("Building_" + buildings[i].buildingName + "_Level", 0);
            buildings[i].currentLevel = savedLevel;

            // Change the building appearance to match the saved level
            ChangeBuildingAppearance(buildings[i]);

            // Update the button with the correct upgrade cost when loading
            UpdateUpgradeButtonText(buildings[i]);

            // Update the building image when loading
            UpdateBuildingImage(buildings[i]);
        }
    }

    // Method to get the player's money from PlayerPrefs
    public int GetMoney()
    {
        return PlayerPrefs.GetInt("PlayerMoney", 1000);  // Default value of 1000 if not set
    }

    // Method to set the player's money in PlayerPrefs
    public void SetMoney(int amount)
    {
        PlayerPrefs.SetInt("PlayerMoney", amount);
        PlayerPrefs.Save();
    }

    // Method to change the appearance of the building based on its level
    void ChangeBuildingAppearance(Building building)
    {
        // Deactivate all building levels
        foreach (GameObject level in building.buildingLevels)
        {
            level.SetActive(false);
        }

        // Activate the building's current level
        if (building.currentLevel < building.buildingLevels.Length)
        {
            building.buildingLevels[building.currentLevel].SetActive(true);
        }
    }

    // Update the upgrade button text to show the current upgrade cost
    void UpdateUpgradeButtonText(Building building)
    {
        if (building.currentLevel < building.maxLevel)
        {
            int nextCost = building.upgradeCosts[building.currentLevel];
            if (building.upgradeCostText != null)
            {
                building.upgradeCostText.text = nextCost.ToString();
            }
        }
        else
        {
            if (building.upgradeCostText != null)
            {
                //building.upgradeCostText.text = "Max Level Reached";
                building.upgradeButton.interactable = false;
                building.upgradeCostText.gameObject.SetActive(false);
            }
        }
    }

    // Update the building image based on the current level
    void UpdateBuildingImage(Building building)
    {
        if (building.buildingImage != null && building.buildingSprites.Length > building.currentLevel)
        {
            // Update the image to reflect the current level
            building.buildingImage.sprite = building.buildingSprites[building.currentLevel];
        }
    }
}
