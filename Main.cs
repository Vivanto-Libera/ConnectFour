using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Tile.State;

public partial class Main : Node
{
	[Signal]
	public delegate void GameOverEventHandler(int result);
	[Signal]
	public delegate void GameResetEventHandler();
	[Signal]
	public delegate void CallAIEventHandler();

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
		foreach(Column column in columns) 
		{
			column.pieceNumber = 0;
			foreach(Tile tile in column.tiles) 
			{
				tile.SetState(White);
			}
		}
	}
	public void AIMove() 
	{
		int whereMove = new AlphaBeta(columns).GetColumn();
		columns[whereMove].DropPiece(Red);
		Moved(false);
	}
	public void Moved(bool isPlayer) 
	{
		AlphaBeta.WhoWin who = new AlphaBeta(columns).JudgeWhoWin();
		if (who != AlphaBeta.WhoWin.NotEnd)
		{
			EmitSignal(SignalName.GameOver, (int)who);
			return;
		}
		if (isPlayer)
		{
			SetAllButtonDisable(true);
			EmitSignal(SignalName.CallAI);
		}
		else 
		{
			SetAllButtonDisable(false);
		}
	}
	public void OnFirstPressed() 
	{
		GetNode<Button>("First").Hide();
		GetNode<Button>("Second").Hide();
		SetAllButtonDisable(false);
	}
	public void OnSecondPressed() 
	{
		GetNode<Button>("First").Hide();
		GetNode<Button>("Second").Hide();
		EmitSignal(SignalName.CallAI);
	}
	public async void OnGameOver(int result) 
	{
		SetAllButtonDisable(true);
		Label message = GetNode<Label>("Message");
		if (result == (int)AlphaBeta.WhoWin.RedWin)
		{
			message.Text = "你输了";
		}
		else if (result == (int)AlphaBeta.WhoWin.BlueWin)
		{
			message.Text = "你赢了";
		}
		else
		{
			message.Text = "平局";
		}
		message.Show();
		await ToSignal(GetTree().CreateTimer(3), Timer.SignalName.Timeout);
		message.Hide();
		EmitSignal(SignalName.GameReset);
	}
	public void OnPlayerDroped() 
	{
		Moved(true);
	}

	public void OnGameReset()
	{
		GameStart();
	}
	public override void _Ready()
	{
		for (int i = 0; i < 7; i++) 
		{
			String columnName = "Column" + i.ToString();
			columns[i] = GetNode<Column>(columnName);
		}
		GameStart();
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

	int WhereDrop = -1;
	int maxValue = -100;
	public Tile.State[,] board = new Tile.State[7, 6];
	public int[] columnPiece = new int[7];
	private const int DEPTH = 8;
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
		MaxValue(-100, 100, true, 0);
		return WhereDrop;
	}
	private int MaxValue(int alpha, int beta, bool isFirst, int dep)
	{
		WhoWin who = JudgeWhoWin();
		if (who == WhoWin.BlueWin)
		{
			return -50;
		}
		if (who == WhoWin.Draw)
		{
			return 0;
		}
		if(dep == DEPTH) 
		{
			return Evaluate();
		}
		int v = -100;
		for(int i = 0; i < 7; i++) 
		{
			if (columnPiece[i] == 6) 
			{
				continue;
			}
			board[i, columnPiece[i]] = Red;
			columnPiece[i]++;
			int newV = MinValue(alpha, beta, dep + 1);
			if (isFirst) 
			{
				if(newV > maxValue) 
				{
					WhereDrop = i;
					maxValue = newV;
				}
			}
			if (newV >= beta) 
			{
				columnPiece[i]--;
				board[i, columnPiece[i]] = White;
				return newV;
			}
			v = newV > v ? newV : v;
			alpha = v > alpha ? v : alpha;
			columnPiece[i]--;
			board[i, columnPiece[i]] = White;
		}
		return v;
	}
	private int MinValue(int alpha, int beta, int dep)
	{
		WhoWin who = JudgeWhoWin();
		if (who == WhoWin.RedWin)
		{
			return 50;
		}
		if (who == WhoWin.Draw)
		{
			return 0;
		}
		if (dep == DEPTH)
		{
			return Evaluate();
		}
		int v = 100;
		for (int i = 0; i < 7; i++)
		{
			if (columnPiece[i] == 6)
			{
				continue;
			}
			board[i, columnPiece[i]] = Blue;
			columnPiece[i]++;
			int newV = MaxValue(alpha, beta, false, dep + 1);
			if (newV <= alpha)
			{
				columnPiece[i]--;
				board[i, columnPiece[i]] = White;
				return newV;
			}
			v = newV < v ? newV : v;
			beta = v < beta ? v : beta;
			columnPiece[i]--;
			board[i, columnPiece[i]] = White;
		}
		return v;
	}
	private int Evaluate() 
	{
		int v = 0;
		for (int i = 0; i < 7; i++)
		{
			int count = columnPiece[i];
			if (count < 3 || count == 6)
			{
				continue;
			}
			if (board[i, count - 1] == board[i, count - 2] && board[i, count - 1] == board[i, count - 3]) 
			{
				if (board[i, count - 1] == Red) 
				{
					v += 2;
				}
				else 
				{
					v -= 2;
				}
			}
		}
		for (int i = 0; i < 6; i++)
		{
			for(int j = 0; j < 5; j++) 
			{
				if (board[j, i] == White) 
				{
					continue;
				}
				if (board[j, i] == board[j + 1, i] && board[j, i] == board[j + 2, i]) 
				{
					if(j != 0) 
					{
						if (board[j - 1, i] == White) 
						{
							if (board[j, i] == Red)
							{
								v += 1;
							}
							else
							{
								v -= 1;
							}
						}
					}
					if(j != 4) 
					{
						if (board[j + 1, i] == White)
						{
							if (board[j, i] == Red)
							{
								v += 1;
							}
							else
							{
								v -= 1;
							}
						}
					}
				}
			}
		}
		for (int i = 0; i < 4; i++)
		{
			for (int j = 3; j < columnPiece[i]; j++)
			{
				if (board[i, j] == board[i + 1, j - 1] && board[i, j] == board[i + 2, j - 2])
				{
					if (board[i + 3, j - 3] == White) 
					{
						if (board[i, j] == Red)
						{
							v += 1;
						}
						else
						{
							v -= 1;
						}
					}
					if(i != 0 && j != 5) 
					{
						if (board[i - 1, j + 1] == White) 
						{
							if (board[i, j] == Red)
							{
								v += 1;
							}
							else
							{
								v -= 1;
							}
						}
					}
				}
			}
		}
		for (int i = 6; i > 2; i--)
		{
			for (int j = 3; j < columnPiece[i]; j++)
			{
				if (board[i, j] == board[i - 1, j - 1] && board[i, j] == board[i - 2, j - 2])
				{
					if (board[i - 3, j - 3] == White)
					{
						if (board[i, j] == Red)
						{
							v += 1;
						}
						else
						{
							v -= 1;
						}
					}
					if (i != 6 && j != 5)
					{
						if (board[i + 1, j + 1] == White)
						{
							if (board[i, j] == Red)
							{
								v += 1;
							}
							else
							{
								v -= 1;
							}
						}
					}
				}
			}
		}
		return v;
	}
	public WhoWin JudgeWhoWin() 
	{
		for(int i = 0; i < 7; i++) 
		{
			int count = columnPiece[i];
			if (count < 4) 
			{
				continue;
			}
			if (board[i ,count - 1] == board[i, count - 2] && board[i, count - 1] == board[i, count - 3]
												   && board[i, count - 1] == board[i, count - 4]) 
			{
				return StateToWhoWin(board[i, count - 1]);
			}
		}
		for(int i = 0; i < 6; i++) 
		{
			if (board[3, i] == White) 
			{
				continue;
			}
			if (board[3, i] == board[2, i] && board[3, i] == board[1, i]) 
			{
				if (board[3, i] == board[0, i] || board[3, i] == board[4, i]) 
				{
					return StateToWhoWin(board[3, i]);
				}
			}
			if (board[3, i] == board[4, i] && board[3, i] == board[5, i])
			{
				if (board[3, i] == board[2, i] || board[3, i] == board[6, i])
				{
					return StateToWhoWin(board[3, i]);
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
		else if(state == Blue)
		{
			return WhoWin.BlueWin;
		}
		else 
		{
			return WhoWin.NotEnd;
		}
	}
}
