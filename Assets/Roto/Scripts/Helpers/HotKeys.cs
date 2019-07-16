using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotKeys : MonoBehaviour
{

    #region singleton
    // singleton
    public static HotKeys instance;
    private void Awake()
    {
        if (instance && instance != this) { Destroy(gameObject); return; }
        instance = this;
    }
    #endregion


    private List<HotKeyAction> hotkeyActions = new List<HotKeyAction>();


    public void AddHotKey(HotKeyAction hotKeyAction)
    {
        hotkeyActions.Add(hotKeyAction);
    }



    private void Update()
    {
        for (int i = 0; i < hotkeyActions.Count; i++)
        {
            HotKeyAction hka = hotkeyActions[i];
            if (Input.GetKeyDown(hka.key)) { hka.Toggle(); }
        }
    }





}


[System.Serializable]
public class HotKeyAction
{
    public bool active = false;
    public KeyCode key = KeyCode.F1;
    private System.Action toggleAction;


    public HotKeyAction(KeyCode key, System.Action toggleAction, bool startActive = false)
    {
        this.key = key;
        this.toggleAction = toggleAction;
        this.active = startActive;
    }


    public void Toggle()
    {
        active = !active;
        DoAction();
    }


    public void DoAction()
    {
        if (toggleAction != null) { toggleAction(); }
    }
}
