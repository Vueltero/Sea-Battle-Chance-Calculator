public class SeaBattle
{
    private const int GRID_SIZE = 10;
    private CellState[,] board = new CellState[GRID_SIZE, GRID_SIZE];

    //ship sizes (lengths)
    private int[] _shipSizes = { 5, 5, 5, 4, 3, 2, 2 };
    //there is one ship that is 5x2, but I calculate it as 2 5x1 ships

    public SeaBattle()
    {
        for (int i = 0; i < GRID_SIZE; i++)
            for (int j = 0; j < GRID_SIZE; j++)
                board[i, j] = CellState.CAN_FIRE;
    }

    public void SetCellState(int x, int y, CellState state)
    {
        board[x, y] = state;
    }

    public double[,] CalculateHitProbabilities()
    {
        double[,] probabilities = new double[GRID_SIZE, GRID_SIZE];
        int[,] configurationCount = new int[GRID_SIZE, GRID_SIZE];

        //generate all possible ship positions and check valid configurations
        foreach (var shipSize in _shipSizes)
        {
            for (int i = 0; i < GRID_SIZE; i++)
            {
                for (int j = 0; j < GRID_SIZE; j++)
                {
                    //horizontal ship placement
                    if (j + shipSize <= GRID_SIZE)
                    {
                        bool valid = true;
                        for (int k = 0; k < shipSize; k++)
                        {
                            if (board[i, j + k] == CellState.MISS)
                            {
                                valid = false;
                                break;
                            }
                        }

                        if (valid)
                            for (int k = 0; k < shipSize; k++)
                                configurationCount[i, j + k]++;
                    }

                    //vertical ship placement
                    if (i + shipSize <= GRID_SIZE)
                    {
                        bool valid = true;
                        for (int k = 0; k < shipSize; k++)
                        {
                            if (board[i + k, j] == CellState.MISS)
                            {
                                valid = false;
                                break;
                            }
                        }

                        if (valid)
                            for (int k = 0; k < shipSize; k++)
                                configurationCount[i + k, j]++;
                    }
                }
            }
        }

        //calculate the total number of valid configurations
        int totalConfigurations = 0;
        for (int i = 0; i < GRID_SIZE; i++)
            for (int j = 0; j < GRID_SIZE; j++)
                totalConfigurations += configurationCount[i, j];

        //calculate the probability for each cell
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                if (board[i, j] == CellState.MISS)
                {
                    probabilities[i, j] = 0.0;
                }
                else if (board[i, j] == CellState.HIT)
                {
                    probabilities[i, j] = 1.0;
                }
                else if (totalConfigurations > 0)
                {
                    probabilities[i, j] = (double)configurationCount[i, j] / totalConfigurations;
                }
                else
                {
                    probabilities[i, j] = 0.0;
                }
            }
        }

        return probabilities;
    }
}
