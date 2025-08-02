using System;
using Game_Info;
using Scriptable_Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Money : MonoBehaviour
{
    [SerializeField] private int money;
    [SerializeField] private TextMeshProUGUI moneytext;
    [SerializeField] private TextMeshProUGUI unlockedItems;
    [SerializeField] private GameObject gridSystem;
    [SerializeField] GameObject selectedItem;


    private GridSystem gridSystemClass;
    private GameInfo gameInfo;
    private Image uiImage;
    private FactoryElementType previousFrameSelectedElement;

    private void Start()
    {
        uiImage = selectedItem.GetComponent<Image>();
        gridSystemClass = gridSystem.GetComponent<GridSystem>();
        gameInfo = GameInfo.Instance;
    }

    // Update is called once per frame
    private void Update()
    {
        moneytext.text = $"Money: {gameInfo.Money}";
        String unlockedThings = "Unlocked Items:\n";
        foreach (var unlockedElement in gameInfo.UnlockedFactoryElements)
        {
            unlockedThings += $"{unlockedElement.name}\n";
        }
        
        unlockedItems.text = unlockedThings;

        if (gridSystemClass.selectedElement != previousFrameSelectedElement)
        {
            uiImage.sprite = gridSystemClass.selectedElement.Prefab.GetComponent<SpriteRenderer>().sprite;
        }

        previousFrameSelectedElement = gridSystemClass.selectedElement;
    }
}
