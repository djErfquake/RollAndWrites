using System.Collections.Generic;
using UnityEngine;

[HelpURL("https://code.roto.com/standard-applications/unity-roto-base#ui-helpers")]
[AddComponentMenu("Roto/Button Group")]
public class RotoButtonGroup : MonoBehaviour
{
    public enum Mode { OneShot, RadioButton };

    [Header("Settings")]
    public Mode mode = Mode.OneShot;

    [HideInInspector]
    public int radioButtonMaxCount = 1;
    private List<RotoButton> selectedButtons = new List<RotoButton>();

    [Header("Buttons")] [SerializeField]
    private List<RotoButton> buttons = new List<RotoButton>();


    private void Start()
    {
        for (int i = 0; i < buttons.Count; i++) { buttons[i].SetGroup(this); }
    }


    // functions
    public void Reset()
    {
        for (int i = 0; i < buttons.Count; i++) { buttons[i].Select(false); buttons[i].Enable(true); }
    }

    public void DisableAll()
    {
        for (int i = 0; i < buttons.Count; i++) { buttons[i].Enable(false); }
    }

    public void UnSelectAll()
    {
        for (int i = 0; i < buttons.Count; i++) { buttons[i].Select(false); }
    }

    public void AddButtonToSelection(RotoButton button)
    {
        if (radioButtonMaxCount == 1)
        {
            if (selectedButtons.Count == radioButtonMaxCount)
            {
                selectedButtons[0].Select(false);
                selectedButtons.RemoveAt(0);
            }
        }

        if (selectedButtons.Count < radioButtonMaxCount)
        {
            button.Select(true);
            selectedButtons.Add(button);

            if (selectedButtons.Count == radioButtonMaxCount)
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    if (!buttons[i].IsSelected())
                    {
                        buttons[i].Enable(false);
                    }
                }
            }
        }
    }

    public void EnableAll()
    {
        for (int i = 0; i < buttons.Count; i++) { buttons[i].Enable(true); }
    }


    // getters
    public int GetSelectedCount()
    {
        return selectedButtons.Count;
    }

    public List<RotoButton> GetSelectedButtons()
    {
        return selectedButtons;
    }




    // buttons call this
    public void ButtonPressed(RotoButton button)
    {
        switch (mode)
        {
            case Mode.OneShot:
                DisableAll();
                break;
            case Mode.RadioButton:
                if (!button.IsSelected())
                {
                    AddButtonToSelection(button);
                }
                else
                {
                    button.Select(false);
                    selectedButtons.Remove(button);
                    EnableAll();
                }
                break;
        }
    }
}


//[CustomEditor(typeof(RotoButtonGroup))]
//public class RotoButtonGroupEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        RotoButtonGroup buttonGroup = target as RotoButtonGroup;

//        buttonGroup.mode = GUILayout.(buttonGroup.mode, "Flag");

//        if (buttonGroup.flag)
//            buttonGroup.i = EditorGUILayout.IntSlider("I field:", buttonGroup.i, 1, 100);

//    }
//}
