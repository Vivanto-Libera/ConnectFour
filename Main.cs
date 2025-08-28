using Godot;
using System;
using System.Collections.Generic;
using static Tile.State;

public partial class Main : Node
{
	public Column[] columns = new Column[7];

	public void SetAllButtonDisable(bool disable) 
	{
		foreach(Column column in columns) 
		{
			column.SetButtonDisable(disable);
		}
	}
	public void SetAllTileWhite() 
	{
		foreach(Column column in columns) 
		{
			foreach(Tile tile in column.tiles) 
			{
				tile.SetState(White);
			}
		}
	}
	public void GameStart() 
	{
        SetAllButtonDisable(true);
        SetAllTileWhite();
		GetNode<Button>("First").Show();
		GetNode<Button>("Second").Show();
    }
	public override void _Ready()
	{
		for (int i = 0; i < 7; i++) 
		{
			String columnName = "Column" + i.ToString();
			columns[i] = GetNode<Column>(columnName);
		}
	}
}

public class AlphaBeta 
{
    public enum WhoWin
    {
        RedWin,
        BlueWin,
        Draw,
        NotEnd,
    }

    List<int> action1 = new List<int>();
    List<int> action0 = new List<int>();
    List<int> actionm1 = new List<int>();
    public Tile.State[,] board = new Tile.State[7, 6];
    public int[] columnPiece = new int[7];
	
	public AlphaBeta(Column[] columns) 
	{
		for(int i = 0; i < 7; i++) 
		{
			Tile.State[] states = columns[i].GetStates();
            columnPiece[i] = columns[i].pieceNumber;
			for(int j = 0; j < 6; j++) 
			{
				board[i, j] = states[j];
			}
		}
	}
    public int GetColumn()
    {
        int alpha = -2;
        int beta = 2;
        MaxValue(ref alpha, ref beta, true);
        if (action1.Count != 0)
        {
            return action1[GD.RandRange(0, action1.Count - 1)];
        }
        if (action0.Count != 0)
        {
            return action0[GD.RandRange(0, action0.Count - 1)];
        }
        return actionm1[GD.RandRange(0, actionm1.Count - 1)];
    }
    private int MaxValue(ref int alpha, ref int beta, bool isFirst)
    {
        WhoWin who = JudgeWhoWin();
        if (who == WhoWin.BlueWin)
        {
            return -1;
        }
        if (who == WhoWin.Draw)
        {
            return 0;
        }
        int v = -2;
        for(int i = 0; i < 7; i++) 
        {
            if (columnPiece[i] == 6) 
            {
                continue;
            }
            board[i, columnPiece[i]] = Red;
            columnPiece[i]++;
            int newV = MinValue(ref alpha, ref beta);
            if (isFirst) 
            {
                switch (newV) 
                {
                    case -1:
                        actionm1.Add(i);
                        break;
                    case 0:
                        action0.Add(i);
                        break;
                    case 1:
                        action1.Add(i);
                        break;
                }
            }
            if (newV >= beta) 
            {
                columnPiece[i]--;
                board[i, columnPiece[i]] = White;
                return newV;
            }
            v = newV > v ? newV : v;
            columnPiece[i]--;
            board[i, columnPiece[i]] = White;
        }
        alpha = v > alpha ? v : alpha;
        return v;
    }
    private int MinValue(ref int alpha, ref int beta)
    {
        WhoWin who = JudgeWhoWin();
        if (who == WhoWin.RedWin)
        {
            return 1;
        }
        if (who == WhoWin.Draw)
        {
            return 0;
        }
        int v = 2;
        for (int i = 0; i < 7; i++)
        {
            if (columnPiece[i] == 6)
            {
                continue;
            }
            board[i, columnPiece[i]] = Blue;
            columnPiece[i]++;
            int newV = MinValue(ref alpha, ref beta);
            if (newV >= beta)
            {
                columnPiece[i]--;
                board[i, columnPiece[i]] = White;
                return newV;
            }
            v = newV > v ? newV : v;
            columnPiece[i]--;
            board[i, columnPiece[i]] = White;
        }
        beta = v < beta ? v : beta;
        return v;
    }
    public WhoWin JudgeWhoWin() 
    {
        for(int i = 0; i < 7; i++) 
        {
            if (columnPiece[i] < 4) 
            {
                continue;
            }
            for(int j = 0; j <= columnPiece[i] - 4; j++) 
            {
                if (board[i ,j] == board[i, j + 1] && board[i, j] == board[i, j + 2]
                                                   && board[i, j] == board[i, j + 3]) 
                {
                    return StateToWhoWin(board[i, j]);
                }
            }
        }
        for(int i = 0;i < 6; i++) 
        {
            if (board[i, 3] == White) 
            {
                continue;
            }
            if (board[i ,3] == board[i, 2] && board[i, 3] == board[i, 1]) 
            {
                if (board[i, 3] == board[i, 0] || board[i, 3] == board[i, 4]) 
                {
                    return StateToWhoWin(board[i, 3]);
                }
            }
            if (board[i, 3] == board[i, 4] && board[i, 3] == board[i, 5])
            {
                if (board[i, 3] == board[i, 2] || board[i, 3] == board[i, 6])
                {
                    return StateToWhoWin(board[i, 3]);
                }
            }
        }
        for(int i = 0; i < 4; i++) 
        {
            for(int j = 3; j < columnPiece[i]; j++) 
            {
                if (board[i, j] == board[i + 1, j - 1] && board[i, j] == board[i + 2, j - 2]
                                                       && board[i, j] == board[i + 3, j - 3]) 
                {
                    return StateToWhoWin(board[i, j]);
                }
            }
        }
        for (int i = 6; i > 2; i--)
        {
            for (int j = 3; j < columnPiece[i]; j++)
            {
                if (board[i, j] == board[i - 1, j - 1] && board[i, j] == board[i - 2, j - 2]
                                                       && board[i, j] == board[i - 3, j - 3])
                {
                    return StateToWhoWin(board[i, j]);
                }
            }
        }
        foreach(int aColumn in columnPiece) 
        {
            if(aColumn != 6) 
            {
                return WhoWin.NotEnd;
            }
        }
        return WhoWin.Draw;
    }
    public WhoWin StateToWhoWin(Tile.State state) 
    {
        if(state == Red) 
        {
            return WhoWin.RedWin;
        }
        else 
        {
            return WhoWin.BlueWin;
        }
    }
}
