using System;

namespace Bowling
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bowling();
        }
        /*
         * This method starts a bowling gameplay
         */
        public static void Bowling()
        {
            const int MIN_PLAYER = 1;
            int numberOfPlayers = GetNumber("Please enter the number of Players ", MIN_PLAYER);
            string[] players = new string[numberOfPlayers];

            for (int i = 0; i < numberOfPlayers; i++)
            {
                Console.Write($"Please enter player {i + 1} name ");
                players[i] = Console.ReadLine();
            }

            int[,] throws = new int[numberOfPlayers, 20];
            int[,] scores = new int[numberOfPlayers, 10];

            GamePlay(throws, scores, players);

        }

        /*
        * This method starts scanning the throws TwoDArray colum-wise 
        * In every frame, every player throws twice by calling the Throw method
        * The score is updated after a player plays a round
        * After every player plays a frame round, the throws and scores arrays are displayed 
        * After the 10 frames are played CheckExtraRounds is called to check if extra rounds should be given to certain array depending on their last frame throws
        */
        public static void GamePlay(int[,] throws, int[,] scores, string[] players)
        {
            int frame = 1;
            int pinsFirstThrow, pinsSecondThrow;


            for (int throwNumber = 0; throwNumber <= throws.GetLength(1) - 2; throwNumber += 2) // incrementing by 2 since a player throws twice in a frame
            {
                Console.WriteLine($"Frame {frame} ");

                for (int player = 0; player < throws.GetLength(0); player++)
                {
                    //first throw
                    Console.Write($"{players[player]}'s 1st throw is ");
                    Thread.Sleep(2000);
                    pinsFirstThrow = Throw();
                    throws[player, throwNumber] = pinsFirstThrow; //notice throwNumber
                    Console.Write(pinsFirstThrow);


                    Console.WriteLine();

                    //second throw
                    Console.Write($"{players[player]}'s 2nd throw is ");
                    Thread.Sleep(2000);
                    pinsSecondThrow = Throw(10 - throws[player, throwNumber]);
                    throws[player, throwNumber + 1] = pinsSecondThrow; //notice throwNumber+1
                    Console.Write(pinsSecondThrow);

                    Console.WriteLine();

                    //Score Update
                    UpdateScore(player, frame - 1, pinsFirstThrow, pinsSecondThrow, scores, throws);

                }

                Console.WriteLine();

                // printing the two arrays
                PrintTwoDArray(throws, $"Throws After Frame {frame} ", players);
                PrintTwoDArray(scores, $"Scores After Frame {frame} ", players);
                RearrangeAndPrintBoard(throws, players, scores, frame - 1);

                Thread.Sleep(9000);
                frame++;

            }
            //checking if extra throws should be awarded
            CheckExtraRounds(throws, scores, players);
        }

        /*
         * This method checks if the players should be awarded extra throws based on throw number 10
         */
        public static void CheckExtraRounds(int[,] throws, int[,] scores, string[] players)
        {
            bool jaggedCreated = false;
            const int LAST_THROW = 19, NEXT_TO_LAST_THROW = 18;
            int[][] jaggedThrows = new int[players.Length][];
            int[][] jaggedScore = new int[players.Length][];
            int throwNumber = 18;
            const int EXTRA_THROW = 1, TWO_EXTRA_THROWS = 2, STRIKE_OR_SPARE = 10;

            for (int player = 0; player < players.Length; player++) //checking for every player if we need to give extra throws
            {
                //placing the last two throws in variables
                int pinsFirstThrow = throws[player, NEXT_TO_LAST_THROW];
                int pinsSecondThrow = throws[player, LAST_THROW];


                if (pinsFirstThrow == STRIKE_OR_SPARE) // Strike, user gets 2 extra throws
                {
                    if (!jaggedCreated) //checking if we craeted a jagged array, meaning another player already got extra rounds
                    {
                        //create jagged arrays
                        jaggedThrows = TwoDToJagged(throws, player, TWO_EXTRA_THROWS);
                        jaggedScore = TwoDToJagged(scores, player, 1);
                        jaggedCreated = true;
                    }
                    else
                    {
                        //increase the columns of the already existing jagged array
                        ModifyJagged(jaggedThrows, player, TWO_EXTRA_THROWS);
                        ModifyJagged(jaggedScore, player, 1);
                    }

                    Console.Write($"{players[player]}'s 3rd throw is ");
                    Thread.Sleep(2000);
                    int pinsThirdThrow = Throw();
                    jaggedThrows[player][throwNumber + 2] = pinsThirdThrow;
                    Console.Write(pinsThirdThrow);


                    Console.WriteLine();


                    Console.Write($"{players[player]}'s 4th throw is ");
                    Thread.Sleep(2000);
                    int pinsFourthThrow = Throw(STRIKE_OR_SPARE - pinsThirdThrow);
                    jaggedThrows[player][throwNumber + 3] = pinsFourthThrow;
                    Console.Write(pinsFourthThrow);

                    Console.WriteLine();

                    //update the previous and current score
                    jaggedScore[player][jaggedScore[player].Length - 2] += pinsThirdThrow + pinsFourthThrow;
                    jaggedScore[player][jaggedScore[player].Length - 1] = jaggedScore[player][jaggedScore[player].Length - 2] + pinsThirdThrow + pinsFourthThrow;


                }
                else if (pinsFirstThrow + pinsSecondThrow == STRIKE_OR_SPARE)// Spare, user gets 1 extra throw
                {
                    if (!jaggedCreated)//checking if we craeted a jagged array, meaning another player already got extra rounds
                    {
                        //create jagged arrays
                        jaggedThrows = TwoDToJagged(throws, player, EXTRA_THROW);
                        jaggedScore = TwoDToJagged(scores, player, 1);
                        jaggedCreated = true;
                    }
                    else
                    {
                        //increase the columns of the already existing jagged array
                        ModifyJagged(jaggedThrows, player, EXTRA_THROW);
                        ModifyJagged(jaggedScore, player, 1);
                    }
                    Console.Write($"{players[player]}'s 3rd throw is ");
                    //Thread.Sleep(2000);
                    int pinsThirdThrow = Throw();
                    jaggedThrows[player][throwNumber + 2] = pinsThirdThrow;
                    Console.Write(pinsThirdThrow);

                    Console.WriteLine();

                    //update the previous and current score
                    jaggedScore[player][jaggedScore[player].Length - 2] += pinsThirdThrow;
                    jaggedScore[player][jaggedScore[player].Length - 1] = jaggedScore[player][jaggedScore[player].Length - 2] + pinsThirdThrow;

                }

                if (jaggedCreated) // if we didn't give extra throws no need to print anything -- there is a bug to be fixed here
                {
                    PrintJaggedArray(jaggedThrows, $"Throws After EXTRA Frame ", players);
                    PrintJaggedArray(jaggedScore, $"Scores After EXTRA Frame ", players);
                    RearrangeAndPrintBoardJagged(jaggedThrows, players, jaggedScore);

                }
            }
        }

        /*
     * This method prints a 2D array
     * It takes a 2D array a 1D array and a message as parameters
     */
        public static void PrintTwoDArray(int[,] TwoDArray, string message, string[] array)
        {
            Console.WriteLine($"{message}");

            for (int i = 0; i < TwoDArray.GetLength(0); i++)
            {
                Console.Write($"{array[i],10}|");
                for (int j = 0; j < TwoDArray.GetLength(1); j++)
                {
                    Console.Write($"{TwoDArray[i, j],5}|");
                }
                Console.WriteLine();
                Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------");

            }
        }
        /*
       * This method updates the scores
       * It takes into consideration spare and strikes in the previous frame and updates scores
       * In frame=0, we do not have a previous frame
       */

        public static void UpdateScore(int player, int frame, int pinsFirstThrow, int pinsSecondThrow, int[,] scores, int[,] throws)
        {
            int previousRoundFirstThrowPins, previousRoundSecondThrowPins, indexOfPreviousRoundFirstThrow, indexOfPreviousRoundSecondThrow;


            if (frame > 0) //check if need to update previous frame score
            {
                indexOfPreviousRoundFirstThrow = (frame - 1) * 2;
                indexOfPreviousRoundSecondThrow = (frame - 1) * 2 + 1;
                previousRoundFirstThrowPins = throws[player, indexOfPreviousRoundFirstThrow];
                previousRoundSecondThrowPins = throws[player, indexOfPreviousRoundSecondThrow];

                if (previousRoundFirstThrowPins == 10) //strike
                {
                    scores[player, frame - 1] += pinsFirstThrow + pinsSecondThrow;
                }
                else if (previousRoundFirstThrowPins + previousRoundSecondThrowPins == 10)//spare
                {
                    scores[player, frame - 1] += pinsFirstThrow;
                }

                scores[player, frame] = scores[player, frame - 1] + pinsFirstThrow + pinsSecondThrow;
            }

            else
                scores[player, frame] = pinsFirstThrow + pinsSecondThrow; //first frame we don't add the score thus far
        }
        public static int Throw(int max = 10)
        {
            const int MIN_PIN = 0;
            Random randomNumber = new Random();

            return randomNumber.Next(MIN_PIN, max + 1);
        }

        /*
         * This method creates copies of the arrays its receives as parameters
         * finds the max player row index using the frame argument
         * Places the max player row on the first row by swapping with the first row
         */
        public static void RearrangeAndPrintBoard(int[,] throws, string[] players, int[,] scores, int frame)
        {
            string[] rearrangedPlayers = new string[players.Length];
            int[,] rearrangedThrows = new int[throws.GetLength(0), throws.GetLength(1)];
            int[,] rearrangedScores = new int[scores.GetLength(0), scores.GetLength(1)];

            int maxIndex = 0;

            //copy the arrays

            CopyArray(rearrangedPlayers, players);
            CopyArray(rearrangedThrows, throws);
            CopyArray(rearrangedScores, scores);

            // find the player with the highest score 
            for (int row = 1; row < scores.GetLength(0); row++)
            {
                if (scores[maxIndex, frame] < scores[row, frame])
                    maxIndex = row;
            }


            //if the max score player is not already on the first row swap
            if (maxIndex != 0)
            {
                SwapRows(rearrangedThrows, maxIndex);
                SwapRows(rearrangedPlayers, maxIndex);
                SwapRows(rearrangedScores, maxIndex);
            }
            PrintLeaderBoard(rearrangedThrows, rearrangedPlayers, rearrangedScores, frame);
        }

        /*
        * This method copies the content of one 2D int array to another
        */
        public static void CopyArray(int[,] newArray, int[,] anArray)
        {
            for (int row = 0; row < anArray.GetLength(0); row++)
            {
                for (int column = 0; column < anArray.GetLength(1); column++)
                {
                    newArray[row, column] = anArray[row, column];
                }
            }
        }

        /*
         * This method copies the content of one string array to another
         */
        public static void CopyArray(string[] newArray, string[] anArray)
        {
            for (int i = 0; i < anArray.Length; i++)
            {
                newArray[i] = anArray[i];
            }
        }

        /*
         * This method swaps the content of 2 rows (index zero and maxIndex) in a 2D array
         */
        public static void SwapRows(int[,] anArray, int maxIndex)
        {
            int[] temp = new int[anArray.GetLength(1)];

            for (int column = 0; column < anArray.GetLength(1); column++)
            {
                temp[column] = anArray[maxIndex, column];
                anArray[maxIndex, column] = anArray[0, column];
                anArray[0, column] = temp[column];
            }

        }

        /*
         * This method swaps the content of 2 indexes (zero and maxIndex) in a string array
         */
        public static void SwapRows(string[] anArray, int maxIndex)
        {
            string temp = anArray[0];
            anArray[0] = anArray[maxIndex];
            anArray[maxIndex] = temp;
        }

        /*
        * This method prints the Leaderboard
        * No need to calculate score, use frame argument and display score at that index
        */
        public static void PrintLeaderBoard(int[,] throws, string[] players, int[,] scores, int frame)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"LEADERBORAD AFTER FRAME {frame + 1}");

            for (int player = 0; player < throws.GetLength(0); player++)
            {
                Console.Write($"{players[player],10}|");
                for (int column = 0; column < throws.GetLength(1); column++)
                {
                    Console.Write($"{throws[player, column],2}|");

                }
                Console.Write($"  Score: {scores[player, frame],8}|");
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        /*
         * This method takes and validates an interger user input
         */

        public static int GetNumber(string msg, int min, int max = int.MaxValue)
        {
            int number;

            Console.Write(msg);
            bool valid = int.TryParse(Console.ReadLine(), out number);

            while (!valid || number < min || number > max)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"Error invalid number: {msg}");
                Console.ForegroundColor = ConsoleColor.White;
                valid = int.TryParse(Console.ReadLine(), out number);

            }

            return number;
        }
        #region jagged

        /*
        * This method creates copies of the arrays its receives
        * finds the player with max score by scanning the last score column of every row
        * places the max player row on the first row by swapping with the first row
        */
        public static void RearrangeAndPrintBoardJagged(int[][] throws, string[] players, int[][] scores)
        {
            // create jagged arrays of number of players rows
            string[] rearrangedPlayers = new string[players.Length];
            int[][] rearrangedThrows = new int[throws.Length][];
            int[][] rearrangedScores = new int[scores.Length][];

            int maxIndex = 0;

            // create each row in a jagged array
            for (int i = 0; i < throws.Length; i++)
                rearrangedThrows[i] = new int[throws[i].Length];

            for (int i = 0; i < scores.Length; i++)
                rearrangedScores[i] = new int[scores[i].Length];

            // copy the array content
            CopyArray(rearrangedPlayers, players);
            CopyJaggedArray(rearrangedThrows, throws);
            CopyJaggedArray(rearrangedScores, scores);

            // find the max score player
            for (int row = 1; row < rearrangedScores.Length; row++)
            {
                if (rearrangedScores[maxIndex][rearrangedScores[maxIndex].Length - 1] < rearrangedScores[row][rearrangedScores[row].Length - 1])
                    maxIndex = row;
            }

            //if the max score player is not already on the first row swap
            if (maxIndex != 0)
            {
                SwapRowsJagged(rearrangedThrows, maxIndex);
                SwapRows(rearrangedPlayers, maxIndex);
                SwapRowsJagged(rearrangedScores, maxIndex);
            }

            PrintLeaderBoardJagged(rearrangedThrows, rearrangedPlayers, rearrangedScores);
        }
        /*
        * 
        * This method copies the content of one jagged array to another
        */
        public static void CopyJaggedArray(int[][] newArray, int[][] anArray)
        {
            for (int row = 0; row < anArray.Length; row++)
            {
                for (int column = 0; column < anArray[row].Length; column++)
                {
                    newArray[row][column] = anArray[row][column];
                }
            }
        }

        /*
     * 
     * This method prints the leaderboard, to get the score use the index of the last column of every row
     */
        public static void PrintLeaderBoardJagged(int[][] throws, string[] players, int[][] scores)
        {

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"LEADERBORAD AFTER FRAME {10}");

            for (int player = 0; player < throws.Length; player++)
            {
                Console.Write($"{players[player],10}|");
                for (int column = 0; column < throws[player].Length; column++)
                {
                    Console.Write($"{throws[player][column],2}|");

                }
                Console.Write($"  Score: {scores[player][scores[player].Length - 1],8}|");
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.White;
        }


        /*
       * 
       * This method swaps 2 rows of jagged array
       */
        public static void SwapRowsJagged(int[][] anArray, int maxIndex)
        {
            int[] temp = new int[anArray[maxIndex].Length];

            anArray[maxIndex] = anArray[0];
            anArray[0] = temp;

        }

        /*
        * 
        * This method increases the size of one row in a jagged array by sizeIncrease and copies the content
        */
        public static void ModifyJagged(int[][] jaggedArray, int row, int cells)
        {
            int[] newArray = new int[jaggedArray[row].Length + cells];

            for (int i = 0; i < jaggedArray[row].Length; i++)
            {
                newArray[i] = jaggedArray[row][i];
            }
            jaggedArray[row] = newArray;
        }

        /*
        * 
        * This method converts a 2D array to jagged Array. The row at index r has original  size + sizeIncrease columns
        */

        public static int[][] TwoDToJagged(int[,] TwoDArray, int r, int sizeIncrease)
        {
            int[][] jaggedArray = new int[TwoDArray.GetLength(0)][];
            int numberOfColumns = TwoDArray.GetLength(1);
            for (int row = 0; row < TwoDArray.GetLength(0); row++)
            {
                if (row == r)
                    jaggedArray[row] = new int[numberOfColumns + sizeIncrease];
                else
                    jaggedArray[row] = new int[numberOfColumns];
            }

            for (int row = 0; row < TwoDArray.GetLength(0); row++)
            {
                for (int column = 0; column < TwoDArray.GetLength(1); column++)
                {
                    jaggedArray[row][column] = TwoDArray[row, column];
                }
            }
            return jaggedArray;
        }

        /*
         * 
         * This method prints a jagged array
         */

        public static void PrintJaggedArray(int[][] TwoDArray, string message, string[] array)
        {
            Console.WriteLine($"{message}");

            for (int i = 0; i < TwoDArray.Length; i++)
            {
                Console.Write($"{array[i],10}|");
                for (int j = 0; j < TwoDArray[i].Length; j++)
                {
                    Console.Write($"{TwoDArray[i][j],5}|");
                }
                Console.WriteLine();
                Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------");

            }
        }

        #endregion

    }
}