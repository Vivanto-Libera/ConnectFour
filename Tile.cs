using Godot;
using System;

public partial class Tile : Node2D
{
	public enum State
	{
		White,
		Red,
		Blue,
	}
	private State state = State.White;
	public State GetState()
	{
		return state;
	}
	public void SetState(State newState) 
	{
		state = newState;
		string Picpath = "res://Image/" + state.ToString() + "Round.png";
		GetNode<TextureRect>("TextureRect").Texture = GD.Load<Texture2D>(Picpath);
	}
}
