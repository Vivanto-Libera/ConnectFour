# Connect Four

四子棋

<img width="709" height="737" alt="屏幕截图 2025-08-28 152202" src="https://github.com/user-attachments/assets/e54ed999-fa3d-4396-80d7-99b66f5c652b" />

评估算法和判断结束的算法

```c#
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
```

