using Game_Info;
using Scriptable_Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCountUpToDate : MonoBehaviour
{
    [SerializeField] public ItemType targetItem;
    [SerializeField] public int targetCount;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private Image icon;


    // Update is called once per frame
    void Update()
    {
        if (targetItem is not null)
        {
            icon.sprite = targetItem.Icon;
        
        
            bool success = GameInfo.Instance.SubmittedItems.TryGetValue(targetItem, out var value);
            targetText.text = targetCount.ToString();
            if (success)
            {
                countText.text = value.ToString();
            }
            else
            {
                countText.text = "0";
            }}
    }

    public bool HasCompleted()
    {
        GameInfo.Instance.SubmittedItems.TryGetValue(targetItem, out var value);
        return (value >= targetCount);
    }
}
