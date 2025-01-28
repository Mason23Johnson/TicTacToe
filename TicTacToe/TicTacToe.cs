using System;
using System.Threading;

class Program
{
    static void Main()
    {
        while (true)
        {
            PlayGame(); // Start game

            Console.WriteLine("Would you like to play again? (y/n):");
            string response;
            while (true)
            {
                response = Console.ReadLine().ToLower();
                if (response == "y" || response == "n") // Check input
                {
                    break;
                }
                Console.WriteLine("Invalid input. Please enter 'y' to play again or 'n' to exit:");
            }

            if (response == "n")
            {
                Console.WriteLine("Thanks for playing! Goodbye!");
                break; // End program
            }
        }
    }

    static void PlayGame()
    {
        // Create game board with numbered positions
        string[,] board = new string[,]
        {
            { " 1 ", "|", " 2 ", "|", " 3 " },
            { "---", "+", "---", "+", "---" },
            { " 4 ", "|", " 5 ", "|", " 6 " },
            { "---", "+", "---", "+", "---" },
            { " 7 ", "|", " 8 ", "|", " 9 " }
        };

        Console.WriteLine("Do you want to play as X or O? (Enter X or O):");
        string player = "";
        while (true)
        {
            player = Console.ReadLine().ToUpper();
            if (player == "X" || player == "O") // Check input
                break;
            Console.WriteLine("Invalid choice. Please enter X or O:");
        }

        string computer = player == "X" ? "O" : "X"; // Assign computer the opposite symbol
        Console.WriteLine($"You are {player}. The computer is {computer}.");
        Console.WriteLine("Press Enter to start the game.");
        Console.ReadLine();

        // Show board
        PrintBoard(board);

        string currentPlayer = "X"; // X always starts the game
        string[] flatBoard = new string[9]; // 1D array for internal board tracking
        for (int i = 0; i < flatBoard.Length; i++) flatBoard[i] = (i + 1).ToString(); // Fill with numbers 1-9

        while (true)
        {
            if (currentPlayer == player) // Player's turn
            {
                Console.WriteLine("Your turn! Choose a position (1-9):");
                int position = -1;
                while (true)
                {
                    // Check input (1-9 and not already occupied)
                    if (int.TryParse(Console.ReadLine(), out position) && position >= 1 && position <= 9 && flatBoard[position - 1] != "X" && flatBoard[position - 1] != "O")
                    {
                        break;
                    }
                    Console.WriteLine("Invalid input. Try again:");
                }
                UpdateBoard(flatBoard, position - 1, currentPlayer); // Update board with player's move
            }
            else // Computer's turn
            {
                Console.WriteLine("Computer's turn...");
                Thread.Sleep(1000); // Short delay for realism
                int move = GetComputerMove(flatBoard, computer, player); // Computer's move
                UpdateBoard(flatBoard, move, currentPlayer); // Update board with computer's move
            }

            UpdateVisualBoard(flatBoard, board); // Update board
            PrintBoard(board); // Print board

            string winner = CheckWinner(flatBoard); // Check if there's a winner
            if (winner != null) // If there's a winner or tie
            {
                Console.WriteLine(winner == "T" ? "It's a tie!" : $"{winner} wins!");
                break;
            }

            currentPlayer = currentPlayer == "X" ? "O" : "X"; // Switch turns
        }
    }

    static void PrintBoard(string[,] board)
    {
        Console.Clear(); // Clear console
        for (int i = 0; i < board.GetLength(0); i++) // Iterate through rows
        {
            for (int j = 0; j < board.GetLength(1); j++) // Iterate through columns
            {
                Console.Write(board[i, j]); // Print each cell
            }
            Console.WriteLine(); // Move to the next line
        }
    }

    static void UpdateVisualBoard(string[] flatBoard, string[,] board)
    {
        // Map the flatBoard array to the visual board for display
        board[0, 0] = $" {flatBoard[0]} ";
        board[0, 2] = $" {flatBoard[1]} ";
        board[0, 4] = $" {flatBoard[2]} ";
        board[2, 0] = $" {flatBoard[3]} ";
        board[2, 2] = $" {flatBoard[4]} ";
        board[2, 4] = $" {flatBoard[5]} ";
        board[4, 0] = $" {flatBoard[6]} ";
        board[4, 2] = $" {flatBoard[7]} ";
        board[4, 4] = $" {flatBoard[8]} ";
    }

    static void UpdateBoard(string[] flatBoard, int position, string player)
    {
        flatBoard[position] = player; // Place the player's symbol at the specified position
    }

    static string CheckWinner(string[] board)
    {
        // All winning combinations
        int[,] winningCombos = new int[,]
        {
            { 0, 1, 2 },
            { 3, 4, 5 },
            { 6, 7, 8 },
            { 0, 3, 6 },
            { 1, 4, 7 },
            { 2, 5, 8 },
            { 0, 4, 8 },
            { 2, 4, 6 }
        };

        for (int i = 0; i < winningCombos.GetLength(0); i++) // Check each combination
        {
            if (board[winningCombos[i, 0]] != " " &&
                board[winningCombos[i, 0]] == board[winningCombos[i, 1]] &&
                board[winningCombos[i, 1]] == board[winningCombos[i, 2]])
            {
                return board[winningCombos[i, 0]]; // Return the winner (X or O)
            }
        }

        // Check for empty spots; if none, Tie
        foreach (string spot in board)
        {
            if (spot != "X" && spot != "O")
                return null; // Game is still in progress
        }

        return "T"; // Tie
    }

    static int GetComputerMove(string[] board, string computer, string player)
    {
        // Check for a winning move
        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] != "X" && board[i] != "O")
            {
                string original = board[i];
                board[i] = computer;
                if (CheckWinner(board) == computer)
                {
                    board[i] = original;
                    return i; // Return winning move
                }
                board[i] = original;
            }
        }

        // Check for a blocking move
        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] != "X" && board[i] != "O")
            {
                string original = board[i];
                board[i] = player;
                if (CheckWinner(board) == player)
                {
                    board[i] = original;
                    return i; // Return blocking move
                }
                board[i] = original;
            }
        }

        // Pick a random available spot
        Random rand = new Random();
        int move;
        do
        {
            move = rand.Next(0, board.Length);
        } while (board[move] == "X" || board[move] == "O");

        return move; // Return random move
    }
}
