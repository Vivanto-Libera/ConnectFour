using Godot;
using System;
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
	public Tile.State[,] board = new Tile.State[7, 6];
	
	public AlphaBeta(Column[] columns) 
	{
		for(int i = 0; i < 7; i++) 
		{
			Tile.State[] states = columns[i].GetStates();
			for(int j = 0; j < 6; j++) 
			{
				board[i, j] = states[j];
			}
		}
	}
}
