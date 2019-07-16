using System.Collections.Generic;

public class ExhibitScreenManager
{
    private List<ExhibitScreen> screens = new List<ExhibitScreen>();
    private ExhibitScreen currentScreen;


    public ExhibitScreenManager() { }
    public ExhibitScreenManager(ExhibitScreen[] screens)
    {
        for (int i = 0; i < screens.Length; i++) { this.screens.Add(screens[i]); }
        currentScreen = this.screens[0];
    }


    public void AddScreen(ExhibitScreen screen)
    {
        screens.Add(screen);
    }


    public void RemoveScreen(ExhibitScreen screen)
    {
        screens.Remove(screen);
    }


    /// <summary>
    /// Hides all other screens and shows the passed in screen
    /// </summary>
    /// <param name="gotoScreen"></param>
    /// <param name="onComplete"></param>
    public void GoToScreen(ExhibitScreen gotoScreen, System.Action onComplete = null)
    {
        currentScreen = gotoScreen;
        for (int i = 0; i < screens.Count; i++)
        {
            if (screens[i] != currentScreen)
            {
                screens[i].Hide();
            }
        }

        currentScreen.Show(onComplete);
    }


    /// <summary>
    /// Hides all other screens and shows the first screen
    /// </summary>
    /// <param name="onComplete"></param>
    public void GoToFirstScreen(System.Action onComplete = null)
    {
        GoToScreen(screens[0], onComplete);
    }


    /// <summary>
    /// Hides the current active screen and shows the passed in screen, then sets it to the current screen
    /// </summary>
    /// <param name="swapScreen"></param>
    /// <param name="onComplete"></param>
    public void SwapScreen(ExhibitScreen swapScreen, System.Action onComplete = null)
    {
        currentScreen.RemoveAllEventListeners();
        swapScreen.RemoveAllEventListeners();

        swapScreen.Load(() =>
        {
            currentScreen.Hide(() =>
            {
                swapScreen.Setup();
                swapScreen.Show(() =>
                {
                    currentScreen.Unload();
                    swapScreen.AddAllEventListeners();

                    currentScreen = swapScreen;

                    if (onComplete != null) { onComplete(); }
                });
            });
        });

    }


    /// <summary>
    /// Hides the current active screen and shows the current active screen's <b>nextScreen</b>, and sets it to the current screen
    /// </summary>
    /// <param name="onComplete"></param>
    public void AdvanceScreen(System.Action onComplete = null)
    {
        ExhibitScreen nextScreen = currentScreen.nextScreen;
        SwapScreen(nextScreen, onComplete);
    }


    /// <summary>
    /// Calls <b>Reset</b> on all screens
    /// </summary>
    public void ResetScreens()
    {
        for (int i = 0; i < screens.Count; i++) { screens[i].Reset(); }
    }

}
