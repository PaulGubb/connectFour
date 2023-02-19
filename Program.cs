using System;
using System.Linq;


Exception? exception = null;


bool?[,] frame = new bool?[7, 8];
bool player1Go;
bool player1GoFirst = true;
Random random = new();


const int moveMinI = 5;
const int moveJ = 2;


try
{
    Console.CursorVisible = false;

PlayAgain:
    player1Go = player1GoFirst;
    player1GoFirst = !player1GoFirst;
    Resetframe();

    while (true)
    {
        (int I, int J) move = default;
        if (player1Go)
        {
            DrawFrame();
            int i = 0;
            Console.SetCursorPosition(moveMinI, moveJ);
            //Console.Write('O');
        GetPlayerInput:
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.LeftArrow:
                    Console.SetCursorPosition(i * 2 -1 + moveMinI, moveJ);
                    Console.Write(' ');
                    i = Math.Max(0, i - 1);
                    Console.SetCursorPosition(i * 2 -1 + moveMinI, moveJ);
                    Console.Write('O');
                    goto GetPlayerInput;
                case ConsoleKey.RightArrow:
                    Console.SetCursorPosition(i * 2 -1 + moveMinI, moveJ);
                    Console.Write(' ');
                    i = Math.Min(frame.GetLength(0) - 1, i + 1);
                    Console.SetCursorPosition(i * 2 -1 + moveMinI, moveJ);
                    Console.Write('O');
                    goto GetPlayerInput;
                case ConsoleKey.Enter:
                    if (frame[i, frame.GetLength(1) - 1] != null)
                    {
                        goto GetPlayerInput;
                    }

                    for (int j = frame.GetLength(1) - 1; ; j--)
                    {
                        if (j is 0 || frame[i, j - 1].HasValue)
                        {
                            frame[i, j] = true;
                            move = (i, j);
                            break;
                        }
                    }

                    break;
                case ConsoleKey.Escape:
                    Console.Clear();
                    return;
                default: goto GetPlayerInput;
            }
            if (CheckForConnect4(move.I, move.J))
            {
                DrawFrame();
                Console.WriteLine();
                Console.WriteLine("   You Won!");
                goto PlayAgainCheck;
            }
        }
        else
        {
            int[] moves = Enumerable.Range(0, frame.GetLength(0)).Where(i => !frame[i, frame.GetLength(1) - 1].HasValue).ToArray();
            int randomMove = moves[random.Next(moves.Length)];
            for (int j = frame.GetLength(1) - 1; ; j--)
            {
                if (j is 0 || frame[randomMove, j - 1].HasValue)
                {
                    frame[randomMove, j] = false;
                    move = (randomMove, j);
                    break;
                }
            }
            if (CheckForConnect4(move.I, move.J))
            {
                DrawFrame();
                Console.WriteLine();
                Console.WriteLine($"   You Lost!");
                goto PlayAgainCheck;
            }

        }

        if (CheckForADraw())
        {
            DrawFrame();
            Console.WriteLine();
            Console.WriteLine($"   A Draw!");
            goto PlayAgainCheck;
        }
        else
        {
            player1Go = !player1Go;
        }
    }

PlayAgainCheck:
    Console.WriteLine("   Play Again ENTER, or quit ESCAPE ?");
GetInput:

    switch (Console.ReadKey(true).Key)
    {
        case ConsoleKey.Enter: goto PlayAgain;
        case ConsoleKey.Escape: Console.Clear(); return;
        default: goto GetInput;
    }
}
catch (Exception e)
{
    exception = e;
    throw;
}
finally
{
    Console.CursorVisible = true;
    Console.Clear();
    Console.WriteLine(exception?.ToString() ?? "Connect 4 game over.");

}


void Resetframe()
{
    for (int i = 0; i < frame.GetLength(0); i++)
    {
        for (int j = 0; j < frame.GetLength(1); j++)
        {
            frame[i, j] = null;
        }
    }
}



void DrawFrame()
{
    Console.Clear();
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("  ╔" + new string('-', frame.GetLength(0) * 2 + 1) + "╗");
    Console.Write("  ║ ");
    int iOffset = Console.CursorLeft;
    int jOffset = Console.CursorTop;
    Console.WriteLine(new string(' ', frame.GetLength(0) * 2) + "║");
    for (int j = 1; j < frame.GetLength(1); j++)
    {
        Console.WriteLine("  ║" + new string(' ', frame.GetLength(0) * 2 + 1) + "║");
    }
    Console.WriteLine("  ╚" + new string('═', frame.GetLength(0) * 2 + 1) + "╝");
    //DrawCounter
    int iFinal = Console.CursorLeft;
    int jFinal = Console.CursorTop;
    for (int i = 0; i < frame.GetLength(0); i++)
    {
        for (int j = 0; j < frame.GetLength(1); j++)
        {
            //Console.SetCursorPosition(i * 2 + iOffset, (frame.GetLength(1) - j) * 2 + jOffset - 1);
            if (frame[i, j] == true)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(i * 2 + iOffset, j + jOffset);
                Console.Write('█');
                Console.SetCursorPosition(i * 2 + 1 + iOffset, j + jOffset);
                Console.Write('█');
                Console.ResetColor();
            }
            else if (frame[i, j] == false)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.SetCursorPosition(i * 2 + iOffset, j + jOffset);
                Console.Write('█');
                Console.SetCursorPosition(i * 2 + 1 + iOffset, j + jOffset);
                Console.Write('█');
                Console.ResetColor();
            }
            else
            {
                Console.ResetColor();
                Console.SetCursorPosition(i * 2 + iOffset, j + jOffset);
                Console.Write(' ');
                Console.SetCursorPosition(i * 2 + 1 + iOffset, j + jOffset);
                Console.Write(' ');
            }
        }
    }

    Console.SetCursorPosition(iFinal, jFinal);
}



bool CheckForConnect4(int i, int j)
{
    bool player = frame[i, j]!.Value;
    { // horizontal
        int inARow = 0;
        for (int _i = 0; _i < frame.GetLength(0); _i++)
        {
            inARow = frame[_i, j] == player ? inARow + 1 : 0;
            if (inARow >= 4) return true;
        }
    }

    { // vertical
        int inARow = 0;
        for (int _j = 0; _j < frame.GetLength(1); _j++)
        {
            inARow = frame[i, _j] == player ? inARow + 1 : 0;
            if (inARow >= 4) return true;
        }
    }

    { // postive slope diagonal
        int inARow = 0;
        int min = Math.Min(i, j);
        for (int _i = i - min, _j = j - min; _i < frame.GetLength(0) && _j < frame.GetLength(1); _i++, _j++)
        {
            inARow = frame[_i, _j] == player ? inARow + 1 : 0;
            if (inARow >= 4) return true;
        }
    }

    { // negative slope diagonal
        int inARow = 0;
        int offset = Math.Min(i, frame.GetLength(1) - (j + 1));
        for (int _i = i - offset, _j = j + offset; _i < frame.GetLength(0) && _j >= 0; _i++, _j--)
        {
            inARow = frame[_i, _j] == player ? inARow + 1 : 0;
            if (inARow >= 4) return true;
        }
    }
    return false;
}



bool CheckForADraw()
{
    for (int i = 0; i < frame.GetLength(0); i++)
    {
        if (!frame[i, frame.GetLength(1) - 1].HasValue)
        {
            return false;
        }
    }
    return true;
}

