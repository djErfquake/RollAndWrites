using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CrissCross : MonoBehaviour
{
    private const int ROWS = 5;
    private const int COLUMNS = 5;

    [Header("Dice")]
    public Die[] dice;

    [Header("Game Grid Squares")]
    public DieValueViewer[] gameGrid;

    [Header("Scoring Areas")]
    public DieValueViewer[] rowScores;
    public DieValueViewer[] columnScores;
    public TextMeshProUGUI scoreText;


    [Header("Game Over")]
    public CCGameOverPanel gameOverPanel;


    // camera
    [Header("Screen Shake")]
    public Transform canvasTransform;
    public float cameraShakeStrength = 10f;
    private float originalCameraShakeStrength = 10f;
    public float cameraShakeIncrement = 2f;


    // variables
    private int totalScore = 0;
    private int puzzleScore = -999;
    private Dictionary<int, int> scoringValues;

    private int diceSet = 0;
    private int highlightedSquares = 0;

    private Die selectedDie = null;
    private DieValueViewer selectedSquare = null;

    [Header("Retry Same")]
    public bool retrySame = true;
    private CCRecorder recorder;
    public int recordDieIndex = 0;



    private void Start()
    {
        recorder = new CCRecorder();

        scoringValues = new Dictionary<int, int>();
        scoringValues.Add(0, 0);
        scoringValues.Add(1, 0);
        scoringValues.Add(2, 2);
        scoringValues.Add(3, 3);
        scoringValues.Add(4, 8);
        scoringValues.Add(5, 10);

        for (int i = 0; i < gameGrid.Length; i++)
        {
            DieValueViewer square = gameGrid[i];
            square.SetToUnused();
            ExhibitUtilities.AddEventTrigger(square.gameObject, UnityEngine.EventSystems.EventTriggerType.PointerUp, () => { SquareClicked(square); });
        }

        for (int i = 0; i < dice.Length; i++)
        {
            Die die = dice[i];
            ExhibitUtilities.AddEventTrigger(die.gameObject, UnityEngine.EventSystems.EventTriggerType.PointerUp, () => { DieClicked(die); });
        }



        // game over panel
        gameOverPanel.Setup(RetryButtonPressed, RetrySameButtonPressed);


        // start game
        SetupForGame();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
    }



    private void RetryButtonPressed()
    {
        retrySame = false;
        puzzleScore = 0;
        SetupForGame();
    }

    private void RetrySameButtonPressed()
    {
        retrySame = true;
        SetupForGame();
    }



    private void SetupForGame()
    {
        originalCameraShakeStrength = cameraShakeStrength;

        gameOverPanel.Hide();

        for (int i = 0; i < gameGrid.Length; i++) { gameGrid[i].SetToUnused(); }

        DieFace startingFace = dice[0].RandomFace();
        if (recorder.dice[0] == null || !retrySame) { recorder.dice[0] = startingFace; }
        else { startingFace = recorder.dice[0]; }
        GetSquare(0, 0).SetViewAnimated(startingFace);
        recordDieIndex = 1;

        selectedDie = null;
        selectedSquare = null;
        cameraShakeStrength = 10f;

        CalculateScore();
        StartRound();
    }




    private void StartRound()
    {
        diceSet = 0;
        RollDice();
    }


    public void SquareClicked(DieValueViewer square)
    {
        if (square.Outlined())
        {
            selectedSquare = square;
            ApplyDieValueToSquare();
        }
    }

    public void DieClicked(Die die)
    {
        selectedDie = die;
        for (int i = 0; i < dice.Length; i++) { if (dice[i] != selectedDie) { dice[i].UnHighlight(); } }
        selectedDie.Highlight();
        HighlightApplicableSquares();
    }


    private void HighlightApplicableSquares()
    {
        List<DieValueViewer> squaresToHighlight = new List<DieValueViewer>();

        if (diceSet == 0)
        {
            for (int i = 0; i < gameGrid.Length; i++) { if (gameGrid[i].Unused()) { squaresToHighlight.Add(gameGrid[i]); } }
        }
        else if (diceSet == 1)
        {

            for (int i = 0; i < gameGrid.Length; i++)
            {
                if (gameGrid[i] == selectedSquare)
                {
                    Vector2Int coordinates = GetSquareCoordinates(i);

                    if (coordinates.x >= 1)
                    {
                        DieValueViewer square = GetSquare(coordinates.x - 1, coordinates.y);
                        if (square.Unused()) { squaresToHighlight.Add(square); }
                    }

                    if (coordinates.x < COLUMNS - 1)
                    {
                        DieValueViewer square = GetSquare(coordinates.x + 1, coordinates.y);
                        if (square.Unused()) { squaresToHighlight.Add(square); }
                    }

                    if (coordinates.y >= 1)
                    {
                        DieValueViewer square = GetSquare(coordinates.x, coordinates.y - 1);
                        if (square.Unused()) { squaresToHighlight.Add(square); }
                    }

                    if (coordinates.y < ROWS - 1)
                    {
                        DieValueViewer square = GetSquare(coordinates.x, coordinates.y + 1);
                        if (square.Unused()) { squaresToHighlight.Add(square); }
                    }

                    break;
                }
            }
        }

        highlightedSquares = squaresToHighlight.Count;
        Color dieColor = selectedDie.GetFace().color;
        for (int i = 0; i < squaresToHighlight.Count; i++)
        {
            squaresToHighlight[i].SetOutline(dieColor);
        }
    }

    private void HideHighlights()
    {
        highlightedSquares = 0;
        for (int i = 0; i < gameGrid.Length; i++) { gameGrid[i].HideOutline(); }
    }


    private void ApplyDieValueToSquare()
    {
        CameraShake();

        selectedDie.Hide();
        selectedSquare.SetViewAnimated(selectedDie.GetFace());

        HideHighlights();

        diceSet++;

        CalculateScore();

        if (DoneWithRound())
        {
            if (NoRoomLeft()) { GameEnded(); }
            else { StartRound(); }
        }
        else
        {
            for (int j = 0; j < dice.Length; j++) { if (dice[j].Available()) { selectedDie = dice[j]; selectedDie.Highlight(); break; } }
            HighlightApplicableSquares();
        }

        if (AllDiceNotPlaced()) { GameEnded(); }

        selectedSquare = null;
    }


    private void CalculateScore()
    {
        totalScore = 0;

        // rows
        totalScore += GetRowsScore();

        // columns
        totalScore += GetColumnsScore();

        // diagonal
        totalScore += GetDiagonalScore();

        // set final score
        scoreText.text = totalScore.ToString();
    }


    private int GetRowsScore()
    {
        int rowsScore = 0;

        for (int y = 1; y < rowScores.Length; y++)
        {
            int score = 0;
            int inARow = 1;
            Color lastColor = GetSquare(0, y - 1).GetCurrentFace().color;
            for (int x = 1; x < COLUMNS; x++)
            {
                DieValueViewer currentSquare = GetSquare(x, y - 1);

                Color thisColor = currentSquare.GetCurrentFace().color;
                if (thisColor == lastColor && !currentSquare.Unused()) { inARow++; }
                else
                {
                    score += scoringValues[inARow];
                    inARow = 1;
                }

                lastColor = thisColor;
            }
            score += scoringValues[inARow];

            if (score == 0) { score = -5; }

            rowScores[y].SetText(score.ToString());

            rowsScore += score;
        }

        return rowsScore;
    }


    private int GetColumnsScore()
    {
        int columnsScore = 0;

        for (int x = 0; x < columnScores.Length; x++)
        {
            int score = 0;
            int inARow = 1;
            Color lastColor = GetSquare(x, 0).GetCurrentFace().color;
            for (int y = 1; y < ROWS; y++)
            {
                DieValueViewer currentSquare = GetSquare(x, y);

                Color thisColor = currentSquare.GetCurrentFace().color;
                if (thisColor == lastColor && !currentSquare.Unused()) { inARow++; }
                else
                {
                    score += scoringValues[inARow];
                    inARow = 1;
                }

                lastColor = thisColor;
            }
            score += scoringValues[inARow];

            if (score == 0) { score = -5; }

            columnScores[x].SetText(score.ToString());

            columnsScore += score;
        }

        return columnsScore;
    }


    private int GetDiagonalScore()
    {
        int diagonalScore = 0;
        int x = 0;
        int inARow = 1;

        Color lastColor = GetSquare(x, ROWS - 1).GetCurrentFace().color;
        for (int y = ROWS - 2; y >= 0; y--)
        {
            x++;
            DieValueViewer currentSquare = GetSquare(x, y);

            Color thisColor = currentSquare.GetCurrentFace().color;
            if (thisColor == lastColor && !currentSquare.Unused()) { inARow++; }
            else
            {
                diagonalScore += scoringValues[inARow];
                inARow = 1;
            }

            lastColor = thisColor;
        }
        diagonalScore += scoringValues[inARow];

        if (diagonalScore == 0) { diagonalScore = -5; }
        rowScores[0].SetText(diagonalScore.ToString());

        return diagonalScore;
    }






    private bool DoneWithRound()
    {
        return diceSet == 2;
    }

    private bool AllDiceNotPlaced()
    {
        return highlightedSquares == 0 && diceSet == 1;
    }


    private bool NoRoomLeft()
    {
        for (int x = 0; x < COLUMNS; x++)
        {
            for (int y = 0; y < ROWS; y++)
            {
                bool leftUnused = (x > 0) ? GetSquare(x - 1, y).Unused() : false;
                bool rightUnused = (x < COLUMNS - 1) ? GetSquare(x + 1, y).Unused() : false;
                bool topUnused = (y > 0) ? GetSquare(x, y - 1).Unused() : false;
                bool bottomUnused = (y < ROWS - 1) ? GetSquare(x, y + 1).Unused() : false;

                if (GetSquare(x, y).Unused() && (leftUnused || rightUnused || topUnused || bottomUnused))
                {
                    return false;
                }
            }
        }

        return true;
    }


    private void GameEnded()
    {
        int highScore = -999;
        if (PlayerPrefs.HasKey("CrissCrossHighScore")) { highScore = PlayerPrefs.GetInt("CrissCrossHighScore"); }

        if (totalScore > puzzleScore) { puzzleScore = totalScore; }
        if (totalScore > highScore) { highScore = totalScore; PlayerPrefs.SetInt("CrissCrossHighScore", highScore); }

        gameOverPanel.Show(totalScore, puzzleScore, highScore);
    }



    private void RollDice()
    {
        for (int i = 0; i < dice.Length; i++)
        { 
            dice[i].Show();
            if (recorder.dice[recordDieIndex] == null || !retrySame) { dice[i].Roll(); recorder.dice[recordDieIndex] = dice[i].GetFace(); }
            else { dice[i].SetFace(recorder.dice[recordDieIndex]); }
            recordDieIndex++;
        }
    }


    private void CameraShake()
    {
        canvasTransform.DOShakePosition(0.8f, cameraShakeStrength);
        cameraShakeStrength += cameraShakeIncrement;
    }





    public DieValueViewer GetSquare(int x, int y)
    {
        return gameGrid[x + (COLUMNS * y)];
    }

    public Vector2Int GetSquareCoordinates(int index)
    {
        return new Vector2Int(index % COLUMNS, index / COLUMNS);
    }
}


/*
 * TODO:
 * 
 * Particle effects on placement
 * scores show a gradient based on score
 * tutorial
 * 
 * 
 * 
 */