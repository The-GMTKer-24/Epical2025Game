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
        icon.sprite = targetItem.Icon;

        if (targetItem is not null)
        {
        bool success = GameInfo.Instance.SubmittedItems.TryGetValue(targetItem, out var value);
        if (success)
        {
            countText.text = value.ToString();
        }
        else
        {
            countText.text = "0";
        }}
    }
}
