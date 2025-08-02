using System;
using Game_Info;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Money : MonoBehaviour
{
    [SerializeField] private int money = 0;
    [SerializeField] private TextMeshProUGUI moneytext;
    [SerializeField] private TextMeshProUGUI unlockedItems;
    private GameInfo gameInfo;

    private void Start()
    {
        gameInfo = GameInfo.Instance;
    }

    // Update is called once per frame
    private void Update()
    {
        moneytext.text = $"Money: {gameInfo.Money}";
        String unlockedThings = "Unlocked Items:\n";
        // Debug.Log(gameInfo.UnlockedFactoryElements.Count);
        foreach (var unlockedElement in gameInfo.UnlockedFactoryElements)
        {
            // Debug.Log(unlockedElement);
            unlockedThings += $"{unlockedElement}\n";
        }
        
        // Debug.Log(unlockedThings);
        
        unlockedItems.text = unlockedThings;
    }
}
