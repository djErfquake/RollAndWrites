using System;
using UnityEngine;

public abstract class ExhibitScreen : MonoBehaviour
{
    public ExhibitScreen nextScreen;
    

    /// <summary>
    /// Use to load media before transitioning to screen
    /// </summary>
    /// <param name="onCompletedCallback"></param>
    public virtual void Load(Action onCompletedCallback = null) { if (onCompletedCallback != null) { onCompletedCallback(); } else { onCompletedCallback = () => { }; onCompletedCallback(); } }

    /// <summary>
    /// Use to unload media after transitioning
    /// </summary>
    /// <param name="onCompletedCallback"></param>
    public virtual void Unload(Action onCompletedCallback = null) { if (onCompletedCallback != null) { onCompletedCallback(); } else { onCompletedCallback = () => { }; onCompletedCallback(); } }


    /// <summary>
    /// Use to remove all touch event listeners on buttons while transitioning
    /// </summary>
    public virtual void RemoveAllEventListeners() { }

    /// <summary>
    /// Use to add touch events back after transitioning
    /// </summary>
    public virtual void AddAllEventListeners() { this.RemoveAllEventListeners(); } // prevent double binds


    /// <summary>
    /// Use to do any setup directly before showing the screen
    /// </summary>
    public virtual void Setup() { }


    /// <summary>
    /// Use for any animations for transitioning the screen into view
    /// </summary>
    /// <param name="onCompletedCallback"></param>
    public virtual void Show(Action onCompletedCallback = null) { if (onCompletedCallback != null) { onCompletedCallback(); } else { onCompletedCallback = () => { }; onCompletedCallback(); } }

    /// <summary>
    /// Use for animations for transitioning the screen out of view
    /// </summary>
    /// <param name="onCompletedCallback"></param>
    public virtual void Hide(Action onCompletedCallback = null) { if (onCompletedCallback != null) { onCompletedCallback(); } else { onCompletedCallback = () => { }; onCompletedCallback(); } }


    /// <summary>
    /// Use to set the screen visible without any animations
    /// </summary>
    /// <param name="visible"></param>
    public virtual void SetVisible(bool visible) { }


    /// <summary>
    /// Use for resetting all screen values back to base values
    /// </summary>
    public virtual void Reset() { }

}
