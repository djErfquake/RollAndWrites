﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CCGameOverPanel : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI yourScoreText, puzzleScoreText, highScoreText;
    public GameObject newButton, sameButton, quitButton;
    private CanvasGroup gameOverGroup;


    private bool gameOverTransitioning = false;

    [MinMax(-100, 100)]
    public MinMaxRange lineHeightRegions = new MinMaxRange(-100, 100);
    private Tween lineHeightLoop;



    public void Setup(System.Action retryAction, System.Action retrySameAction)
    {
        gameOverGroup = gameOverPanel.GetComponent<CanvasGroup>();

        newButton.GetComponent<ChunkyButton>().onClick.AddListener(() => { retryAction(); });
        sameButton.GetComponent<ChunkyButton>().onClick.AddListener(() => { retrySameAction(); });
        quitButton.GetComponent<ChunkyButton>().onClick.AddListener(() => { Application.Quit(); });
        ExhibitUtilities.AddEventTrigger(gameOverPanel, UnityEngine.EventSystems.EventTriggerType.PointerDown, () => { gameOverGroup.alpha = 0; });
        ExhibitUtilities.AddEventTrigger(gameOverPanel, UnityEngine.EventSystems.EventTriggerType.PointerUp, () => { gameOverGroup.alpha = 1; });
        gameOverPanel.SetActive(false);
    }




    public void Show(int score, int puzzleScore, int highScore)
    {
        yourScoreText.text = "Your Score: " + score.ToString();
        puzzleScoreText.text = "Best Puzzle's Score: " + puzzleScore.ToString();
        highScoreText.text = "Best High Score: " + highScore.ToString();

        gameOverTransitioning = true;
        gameOverPanel.SetActive(true);
        gameOverPanel.transform.localScale = Vector2.zero;
        gameOverPanel.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).OnComplete(() => { gameOverTransitioning = false; });

        if (lineHeightLoop != null) { lineHeightLoop.Kill(); lineHeightLoop = null; }
        gameOverText.lineSpacing = lineHeightRegions.max;
        lineHeightLoop = DOTween.To(() => gameOverText.lineSpacing, x => gameOverText.lineSpacing = x, lineHeightRegions.min, 7f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }


    public void Hide()
    {
        if (gameOverTransitioning) { return; }

        gameOverTransitioning = true;
        gameOverPanel.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            if (lineHeightLoop != null) { lineHeightLoop.Kill(); lineHeightLoop = null; }

            gameOverTransitioning = false;
            gameOverPanel.SetActive(false);
        });
    }

}
