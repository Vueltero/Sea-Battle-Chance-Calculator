using System.Collections.Generic;
using com.SeaBattle.singleton;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] private List<CellController> _setUpCellsList;
    [SerializeField] private List<CellController> _chanceCellsList;

    private SeaBattle _seaBattle;

    public void ConfirmSetUpClick()
    {
        _seaBattle = new SeaBattle();

        for (int i = 0; i < _setUpCellsList.Count; i++)
            _chanceCellsList[i].Init(i, _setUpCellsList[i].CellState == CellState.SHIP);

        CalculateAllCellChances();
    }

    private void CalculateAllCellChances()
    {
        double[,] hitProbabilities = _seaBattle.CalculateHitProbabilities();
        Color[,] colorGrid = GetColorGrid(hitProbabilities);

        hitProbabilities = Transpose(hitProbabilities);
        colorGrid = Transpose(colorGrid);

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                int index = i * 10 + j;

                CellController cell = _chanceCellsList[index];

                cell.SetText(hitProbabilities[i, j]);
                cell.SetTextColor(colorGrid[i, j]);
            }
        }
    }

    public void SetCellState(int x, int y, CellState cellState)
    {
        _seaBattle.SetCellState(x, y, cellState);
        CalculateAllCellChances();
    }

    public Color[,] GetColorGrid(double[,] hitProbabilities)
    {
        int rows = hitProbabilities.GetLength(0);
        int cols = hitProbabilities.GetLength(1);
        Color[,] colorGrid = new Color[rows, cols];

        //find min and max values in hitProbabilities
        double min = double.MaxValue;
        double max = double.MinValue;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (hitProbabilities[i, j] < min && hitProbabilities[i, j] != 0f)
                    min = hitProbabilities[i, j];

                if (hitProbabilities[i, j] > max && hitProbabilities[i, j] != 1f)
                    max = hitProbabilities[i, j];
            }
        }

        //map probabilities to colors
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                double normalizedValue = (hitProbabilities[i, j] - min) / (max - min);
                colorGrid[i, j] = hitProbabilities[i, j] == 0 || hitProbabilities[i, j] == 1 ? Color.white : GetColorFromGradient(normalizedValue);
            }
        }

        return colorGrid;
    }

    public Color GetColorFromGradient(double value)
    {
        int r = (int)(255 * (1 - value)); //red decreases as value increases
        int g = (int)(255 * value);       //green increases as value increases
        //int b = 0;                      //blue is always 0

        return new Color(r / 255f, g / 255f, 0f);
    }

    public T[,] Transpose<T>(T[,] array)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        T[,] transposedArray = new T[cols, rows];

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                transposedArray[j, i] = array[i, j];

        return transposedArray;
    }
}
