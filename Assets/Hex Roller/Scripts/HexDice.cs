using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexDice : MonoBehaviour
{
    public List<Die> dice;
    public List<RectTransform> diceTrays;
    public GameObject rollButton;

    [ButtonMethod]
    public void Roll()
    {
        for (int i = 0; i < dice.Count; i++)
        {
            dice[i].Roll();
            int diceTrayIndex = int.Parse(dice[i].GetFace().name) - 3;
            dice[i].GetComponent<RectTransform>().SetParent(diceTrays[diceTrayIndex]);
        }
    }

    private void Start()
    {
        ExhibitUtilities.AddEventTrigger(rollButton, UnityEngine.EventSystems.EventTriggerType.PointerDown, () => {
            Roll();
        });
    }
}
