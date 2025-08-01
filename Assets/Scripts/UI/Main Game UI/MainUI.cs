using TMPro;
using UnityEngine;

public class Money : MonoBehaviour
{
    [SerializeField] private int money = 0;
    [SerializeField] private TextMeshProUGUI moneytext;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        moneytext.text = $"Money: {money}";
    }
}
