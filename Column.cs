using Godot;
using System;
using System.Collections.Generic;

public partial class Column : Node2D
{
	List<Tile> tiles = new List<Tile>();
	public int pieceNumber = 0;

	public void DropPiece(Tile.State state) 
	{
		tiles[pieceNumber].SetState(state);
		pieceNumber++;
		if(pieceNumber == 6) 
		{
			SetButtonDisable(true);
		}

	}
	public void SetButtonDisable(bool isDisable) 
	{
		GetNode<Button>("Button").SetDeferred(Button.PropertyName.Disabled, isDisable);
	}
	public override void _Ready()
	{
		for(int i = 0; i< 6; i++) 
		{
			String tileName = "Tile" + i.ToString();
			tiles.Add(GetNode<Tile>(tileName));
		}
	}
}
