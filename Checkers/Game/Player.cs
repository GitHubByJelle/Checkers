using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpaceC;
using BoardC;
using NNC;
using System.Windows.Forms;
using PieceC;

namespace PlayerC
{
    [Serializable]
    internal class Player
    {
        public int id;
        public Board B;

        public virtual void makeMove()
        {

        }

        public int getId() { return id; }
    }

    [Serializable]
    internal class Human : Player
    {
        public Human(int id)
        {
            this.id = id;
        }

        public override void makeMove()
        {
            // Get possible moves
            List<Space[]> moves = B.getPossibleMoves();

            // Get player move
            Space[] move = getPlayerMove();

            // Check if the move is a possible move
            bool cntinue = true;
            while (cntinue)
            {
                // If it is, make the move
                if (checkMove(move, moves))
                {
                    // Move piece
                    B.movePiece(move);
                    cntinue = false;
                }
                // Otherwise choose a different move
                else
                {
                    // Update user
                    Console.WriteLine("Your move hasn't been found, try again!");

                    // Get new move
                    move = getPlayerMove();
                }
            }  
        }

        private Space[] getPlayerMove()
        {
            // Get console inputs of user
            int[] inpts = getInput();

            // Convert to spaces
            Space[] move = convertToSpace(inpts);

            // Check for double jumps
            Space[] temp_move = new Space[2]; ;
            Array.Copy(move, move.Length - 2, temp_move, 0, 2);

            // As long as player is able to jump
            bool check = true;
            while (check)
            {
                // If it made a jump
                if (B.madeJump(temp_move))
                {
                    // Check if there are double jumps
                    if (checkForPossibleDoubleJumps(move))
                    {
                        // Get input double jump
                        inpts = getInputDoubleJump();

                        // Add to move
                        move = AddToSpace(move, inpts);

                        // Change temporary array
                        Array.Copy(move, move.Length - 2, temp_move, 0, 2);
                    }
                    else
                        check = false;
                }
                else
                    check = false;
            }

            // Add empy line for cosmetics
            Console.WriteLine();

            return move;
        }

        private int[] getInput()
        {
            Console.Write("From: ");
            int from = Convert.ToInt32(Console.ReadLine());

            while (from == 99)
            {
                B.printColoredIndex();
                Console.Write("From: ");
                from = Convert.ToInt32(Console.ReadLine());
            }

            Console.Write("To: ");
            int to = Convert.ToInt32(Console.ReadLine());

            while (to == 99)
            {
                B.printColoredIndex();
                Console.Write("From: ");
                to = Convert.ToInt32(Console.ReadLine());
            }

            return new int[] { from, to };
        }

        private int[] getInputDoubleJump()
        {
            Console.Write("To: ");
            int to = Convert.ToInt32(Console.ReadLine());

            return new int[] { to };
        }

        private Space[] convertToSpace(int[] inpts)
        {
            int[] coordsfrom = this.B.boardId2Coords(inpts[0]);
            int[] coordsto = this.B.boardId2Coords(inpts[1]);
            return new Space[] { this.B.Spaces[coordsfrom[0], coordsfrom[1]], this.B.Spaces[coordsto[0], coordsto[1]] };
        }

        private Space[] AddToSpace(Space[] move, int[] inpts)
        {
            int[] coordsto = this.B.boardId2Coords(inpts[0]);

            return B.mergeArrays(move, new Space[] { this.B.Spaces[coordsto[0], coordsto[1]] });
        }

        private bool checkMove(Space[] move, List<Space[]> moves)
        {
            // For every move
            for (int i = 0; i < moves.Count(); i++)
            {
                // Check all the Spaces if same length
                if (move.Length == moves[i].Length)
                {
                    int count = 0;
                    while (count < move.Length && move[count].getId() == moves[i][count].getId())
                        count++;

                    // If counter stopped by length limit, return true
                    if (count == move.Length)
                        return true;
                }
            }

            // If no move is the same, return false
            return false;
        }

        private bool checkForPossibleDoubleJumps(Space[] startmove)
        {
            List<int> jumpedOver = new List<int>();

            // Determine and add space that got jumped over
            for (int steps = 0; steps < startmove.Length - 1; steps++)
            {
                // Get coords
                int[] from = B.boardId2Coords(startmove[steps].getId());
                int[] to1 = B.boardId2Coords(startmove[steps + 1].getId());

                int row = from[0];
                int col = from[1];

                // Check which direction to check
                int i_dir = from[0] < to1[0] ? 1 : -1;
                int j_dir = from[1] < to1[1] ? 1 : -1;

                // Check every space on diagonal between steps if there is a piece
                for (int iter = 1; iter < Math.Abs(from[0] - to1[0]); iter++)
                {
                    if (B.Spaces[row + iter * i_dir, col + iter * j_dir].isOccupied())
                    {
                        // Remove piece that got jumped
                        jumpedOver.Add(B.boardCoords2Id(row + iter * i_dir, col + iter * j_dir));
                    }
                }
            }

            /// Get position of last jump
            int[] to = B.boardId2Coords(startmove[startmove.Length - 1].getId());
            int i = to[0];
            int j = to[1];

            ////// Check for new jump in new space ("to")
            // Get direction
            int dir = startmove[0].getPieces()[0].getDir(); // TODO link to currentPlayerId

            if (startmove[0].getPieceId() == B.currentPlayer.getId())
            {
                ///// Check for  forward jumps
                if (i + 2 * dir < 0 || i + 2 * dir > B.n - 1)
                { }
                // Else, only add if piece stays on the board
                else
                {
                    // If next square has an enemy piece, and the space after that is free
                    if (j + 2 < B.n)
                        if (B.Spaces[i + dir, j + 1].isOccupied() &
                                ((B.Spaces[i + dir, j + 1].getPieceId() != B.currentPlayer.getId() & B.Spaces[i + dir, j + 1].getPieceId() < 10) ||
                                (B.Spaces[i + dir, j + 1].getPieceId() == 11 & B.currentPlayer.getId() == 2) ||
                                (B.Spaces[i + dir, j + 1].getPieceId() == 22 & B.currentPlayer.getId() == 1)) &
                                !jumpedOver.Contains(B.Spaces[i + dir, j + 1].getId()) & !B.Spaces[i + 2 * dir, j + 2].isOccupied())
                        {
                            return true;
                        }

                    if (j - 2 > -1)
                        if (B.Spaces[i + dir, j - 1].isOccupied() &
                                ((B.Spaces[i + dir, j - 1].getPieceId() != B.currentPlayer.getId() & B.Spaces[i + dir, j - 1].getPieceId() < 10) ||
                                (B.Spaces[i + dir, j - 1].getPieceId() == 11 & B.currentPlayer.getId() == 2) ||
                                (B.Spaces[i + dir, j - 1].getPieceId() == 22 & B.currentPlayer.getId() == 1)) &
                                !jumpedOver.Contains(B.Spaces[i + dir, j - 1].getId()) & !B.Spaces[i + 2 * dir, j - 2].isOccupied())
                        {
                            return true;
                        }
                }

                ///// Check for backward jumps
                if (i - 2 * dir < 0 || i - 2 * dir > B.n - 1)
                { }
                // Else, only add if piece stays on the board
                else
                {
                    // If next square has an enemy piece, and the space after that is free
                    if (j + 2 < B.n)
                        if (B.Spaces[i - dir, j + 1].isOccupied() &
                                ((B.Spaces[i - dir, j + 1].getPieceId() != B.currentPlayer.getId() & B.Spaces[i - dir, j + 1].getPieceId() < 10) ||
                                (B.Spaces[i - dir, j + 1].getPieceId() == 11 & B.currentPlayer.getId() == 2) ||
                                (B.Spaces[i - dir, j + 1].getPieceId() == 22 & B.currentPlayer.getId() == 1)) &
                                !jumpedOver.Contains(B.Spaces[i - dir, j + 1].getId()) & !B.Spaces[i - 2 * dir, j + 2].isOccupied())
                        {
                            return true;
                        }

                    if (j - 2 > -1)
                        if (B.Spaces[i - dir, j - 1].isOccupied() &
                                ((B.Spaces[i - dir, j - 1].getPieceId() != B.currentPlayer.getId() & B.Spaces[i - dir, j - 1].getPieceId() < 10) ||
                                (B.Spaces[i - dir, j - 1].getPieceId() == 11 & B.currentPlayer.getId() == 2) ||
                                (B.Spaces[i - dir, j - 1].getPieceId() == 22 & B.currentPlayer.getId() == 1)) &
                                !jumpedOver.Contains(B.Spaces[i - dir, j - 1].getId()) & !B.Spaces[i - 2 * dir, j - 2].isOccupied())
                        {
                            return true;
                        }
                }
            }

            // If it is a King
            else if ((startmove[0].getPieceId() == 11 & B.currentPlayer.getId() == 1)
                        || startmove[0].getPieceId() == 22 & B.currentPlayer.getId() == 2)
            {
                // Left up. k -1, l -1
                int k = -1;
                int l = -1;
                bool jumpedPiece = false;

                while (i + k > -1) //|| i + k < n)
                {
                    if (j + l > -1)
                        if (!B.Spaces[i + k, j + l].isOccupied() & jumpedPiece)
                        {
                            return true;
                        }
                        else if (B.Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                        {
                            k *= B.n;
                            l *= B.n;
                        }
                        else if (B.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                            ((B.Spaces[i + k, j + l].getPieceId() != B.currentPlayer.getId() & B.currentPlayer.getId() < 10) ||
                            (B.Spaces[i + k, j + l].getPieceId() == 11 & B.currentPlayer.getId() == 2) ||
                            (B.Spaces[i + k, j + l].getPieceId() == 22 & B.currentPlayer.getId() == 1)) &
                            (B.Spaces[i, j].getPieceId() != B.Spaces[i + k, j + l].getPieceId()) &
                            !jumpedOver.Contains(B.Spaces[i + k, j + l].getId())) // If space is occupied, and jumpedPiece is false, make it true
                        {
                            jumpedOver.Add(B.Spaces[i + k, j + l].getId());
                            jumpedPiece = true;
                        }
                        else if (B.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                            ((B.Spaces[i + k, j + l].getPieceId() == B.currentPlayer.getId()) ||
                            (B.Spaces[i + k, j + l].getPieceId() == 11 & B.currentPlayer.getId() == 1) ||
                            (B.Spaces[i + k, j + l].getPieceId() == 22 & B.currentPlayer.getId() == 2) ||
                            (B.Spaces[i, j].getPieceId() == B.Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                        {
                            k *= B.n;
                            l *= B.n;
                        }


                    k -= 1;
                    l -= 1;
                }


                // Right up. k -1, l +1
                k = -1;
                l = 1;
                jumpedPiece = false;

                while (i + k > -1) // || i + k < n)
                {
                    if (j + l < B.n)
                        if (!B.Spaces[i + k, j + l].isOccupied() & jumpedPiece)
                        {
                            return true;
                        }
                        else if (B.Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                        {
                            k *= B.n;
                            l *= B.n;
                        }
                        else if (B.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                            ((B.Spaces[i + k, j + l].getPieceId() != B.currentPlayer.getId() & B.currentPlayer.getId() < 10) ||
                            (B.Spaces[i + k, j + l].getPieceId() == 11 & B.currentPlayer.getId() == 2) ||
                            (B.Spaces[i + k, j + l].getPieceId() == 22 & B.currentPlayer.getId() == 1)) &
                            (B.Spaces[i, j].getPieceId() != B.Spaces[i + k, j + l].getPieceId()) &
                            !jumpedOver.Contains(B.Spaces[i + k, j + l].getId())) // If space is occupied, and jumpedPiece is false, make it true
                        {
                            jumpedOver.Add(B.Spaces[i + k, j + l].getId());
                            jumpedPiece = true;
                        }
                        else if (B.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                            ((B.Spaces[i + k, j + l].getPieceId() == B.currentPlayer.getId()) ||
                            (B.Spaces[i + k, j + l].getPieceId() == 11 & B.currentPlayer.getId() == 1) ||
                            (B.Spaces[i + k, j + l].getPieceId() == 22 & B.currentPlayer.getId() == 2) ||
                            (B.Spaces[i, j].getPieceId() == B.Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                        {
                            k *= B.n;
                            l *= B.n;
                        }

                    k -= 1;
                    l += 1;
                }

                // Left down. k +1, l -1
                k = 1;
                l = -1;
                jumpedPiece = false;

                while (i + k < B.n) //(i + k > -1 || i + k < n)
                {
                    if (j + l > -1)
                        if (!B.Spaces[i + k, j + l].isOccupied() & jumpedPiece)
                        {
                            return true;
                        }
                        else if (B.Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                        {
                            k *= B.n;
                            l *= B.n;
                        }
                        else if (B.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                            ((B.Spaces[i + k, j + l].getPieceId() != B.currentPlayer.getId() & B.currentPlayer.getId() < 10) ||
                            (B.Spaces[i + k, j + l].getPieceId() == 11 & B.currentPlayer.getId() == 2) ||
                            (B.Spaces[i + k, j + l].getPieceId() == 22 & B.currentPlayer.getId() == 1)) &
                            (B.Spaces[i, j].getPieceId() != B.Spaces[i + k, j + l].getPieceId()) &
                            !jumpedOver.Contains(B.Spaces[i + k, j + l].getId())) // If space is occupied, and jumpedPiece is false, make it true
                        {
                            jumpedOver.Add(B.Spaces[i + k, j + l].getId());
                            jumpedPiece = true;
                        }
                        else if (B.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                            ((B.Spaces[i + k, j + l].getPieceId() == B.currentPlayer.getId()) ||
                            (B.Spaces[i + k, j + l].getPieceId() == 11 & B.currentPlayer.getId() == 1) ||
                            (B.Spaces[i + k, j + l].getPieceId() == 22 & B.currentPlayer.getId() == 2) ||
                            (B.Spaces[i, j].getPieceId() == B.Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                        {
                            k *= B.n;
                            l *= B.n;
                        }

                    k += 1;
                    l -= 1;
                }

                // Right down. k +1, l +1
                k = 1;
                l = 1;
                jumpedPiece = false;

                while (i + k < B.n) //(i + k > -1 || i + k < n)
                {
                    if (j + l < B.n)
                        if (!B.Spaces[i + k, j + l].isOccupied() & jumpedPiece)
                        {
                            return true;
                        }
                        else if (B.Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                        {
                            k *= B.n;
                            l *= B.n;
                        }
                        else if (B.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                            ((B.Spaces[i + k, j + l].getPieceId() != B.currentPlayer.getId() & B.currentPlayer.getId() < 10) ||
                            (B.Spaces[i + k, j + l].getPieceId() == 11 & B.currentPlayer.getId() == 2) ||
                            (B.Spaces[i + k, j + l].getPieceId() == 22 & B.currentPlayer.getId() == 1)) &
                            (B.Spaces[i, j].getPieceId() != B.Spaces[i + k, j + l].getPieceId()) &
                            !jumpedOver.Contains(B.Spaces[i + k, j + l].getId())) // If space is occupied, and jumpedPiece is false, make it true
                        {
                            jumpedOver.Add(B.Spaces[i + k, j + l].getId());
                            jumpedPiece = true;
                        }
                        else if (B.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                            ((B.Spaces[i + k, j + l].getPieceId() == B.currentPlayer.getId()) ||
                            (B.Spaces[i + k, j + l].getPieceId() == 11 & B.currentPlayer.getId() == 1) ||
                            (B.Spaces[i + k, j + l].getPieceId() == 22 & B.currentPlayer.getId() == 2) ||
                            (B.Spaces[i, j].getPieceId() == B.Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                        {
                            k *= B.n;
                            l *= B.n;
                        }

                    k += 1;
                    l += 1;
                }
            }

            return false;
        }
    }

    [Serializable]
    internal class RandomBot : Player
    {
        Random rnd = new Random();

        public RandomBot(int id)
        {
            this.id = id;
        }

        public override void makeMove()
        {
            List<Space[]> moves = B.getPossibleMoves();

            if (moves.Count() > 0)
            {
                Space[] move = moves[rnd.Next(moves.Count())];

                B.movePiece(move);
            }
            
        }
    }

    [Serializable]
    internal class NNBot : Player
    {
        NN nn;
        double[] scores;

        public NNBot(int id, int[] hidden, double[] weights)
        {
            this.id = id;
            this.nn = new NN(7, hidden, 1);
            this.nn.setWeights(weights);
        }

        public override void makeMove()
        {
            // Get Possible moves
            List<Space[]> moves = B.getPossibleMoves();

            // Make array for all moves
            scores = new double[moves.Count()];

            Space[] reset;
            // If there are moves available
            if (moves.Count() > 0)
            {
                // For every move
                for (int i = 0; i < moves.Count(); i++)
                {
                    // Simulate move
                    reset = this.B.simulateMove(moves[i]);

                    // Determine score & save score
                    scores[i] = this.nn.forward(calculateFeatures(this.B))[0];

                    // Undo move
                    this.B.UndoMove(moves[i], reset);
                }

                // Return best move
                Space[] best_move = moves[scores.ToList().IndexOf(scores.Max())];

                // Make best move
                B.movePiece(best_move);
            }

        }

        private double[] calculateFeatures(Board b)
        {
            double[] result = new double[7];

            // Pieces in danger (that could be jumped (own))
            result[0] = PiecesToBeJumped(b, b.currentPlayer.id == 1 ? 2 : 1);

            // Pieces that can be jumped (opponent)
            result[1] = PiecesToBeJumped(b, b.currentPlayer.id == 1 ? 1 : 2);

            // Amount of ((own) pieces on first row (home)
            result[2] = CountFirstRow(b);

            // Number of Kings (own)
            int[] cntKings = CountKings(b);
            result[3] = cntKings[0];

            // Number of Kings (opponent)
            result[3] = cntKings[0];

            // Pieces in Play (own)
            int[] cntPieces = b.CountPieces();
            result[5] = cntPieces[b.currentPlayer.id == 1 ? 0 : 1];

            // Pieces in Play (opponent)
            result[6] = cntPieces[b.currentPlayer.id == 1 ? 1 : 0];

            return result;
        }

        private int PiecesToBeJumped(Board b, int playerId)
        {
            int count = 0;

            for (int i = 0; i < b.n; i++)
            {
                for (int j = 0; j < b.n; j++)
                {
                    if (b.Spaces[i, j].isOccupied())
                    {
                        if (b.Spaces[i, j].getPieces()[0].getId() == playerId)
                        {
                            int dir = b.Spaces[i, j].getPieces()[0].getDir(); // TODO link to currentPlayerId

                            ///// Check for  forward jumps
                            if (i + 2 * dir < 0 || i + 2 * dir > b.n - 1)
                            { }
                            // Else, only add if piece stays on the board
                            else
                            {
                                // If next square has an enemy piece, and the space after that is free
                                if (j + 2 < b.n)
                                    if (b.Spaces[i + dir, j + 1].isOccupied() &
                                        ((b.Spaces[i + dir, j + 1].getPieceId() != playerId & playerId < 10) ||
                                        (b.Spaces[i + dir, j + 1].getPieceId() == 11 & playerId == 2) ||
                                        (b.Spaces[i + dir, j + 1].getPieceId() == 22 & playerId == 1))
                                        & !b.Spaces[i + 2 * dir, j + 2].isOccupied())
                                    {
                                        count++;
                                    }

                                if (j - 2 > -1)
                                    if (b.Spaces[i + dir, j - 1].isOccupied() &
                                        ((b.Spaces[i + dir, j - 1].getPieceId() != playerId & playerId < 10) ||
                                        (b.Spaces[i + dir, j - 1].getPieceId() == 11 & playerId == 2) ||
                                        (b.Spaces[i + dir, j - 1].getPieceId() == 22 & playerId == 1))
                                        & !b.Spaces[i + 2 * dir, j - 2].isOccupied())
                                    {
                                        count++;
                                    }
                            }

                            ///// Check for backward jumps
                            if (i - 2 * dir < 0 || i - 2 * dir > b.n - 1)
                            { }
                            // Else, only add if piece stays on the board
                            else
                            {
                                // If next square has an enemy piece, and the space after that is free
                                if (j + 2 < b.n)
                                    if (b.Spaces[i - dir, j + 1].isOccupied() &
                                        ((b.Spaces[i - dir, j + 1].getPieceId() != playerId & playerId < 10) ||
                                        (b.Spaces[i - dir, j + 1].getPieceId() == 11 & playerId == 2) ||
                                        (b.Spaces[i - dir, j + 1].getPieceId() == 22 & playerId == 1))
                                        & !b.Spaces[i - 2 * dir, j + 2].isOccupied())
                                    {
                                        count++;
                                    }

                                if (j - 2 > -1)
                                    if (b.Spaces[i - dir, j - 1].isOccupied() &
                                        ((b.Spaces[i - dir, j - 1].getPieceId() != playerId & playerId < 10) ||
                                        (b.Spaces[i - dir, j - 1].getPieceId() == 11 & playerId == 2) ||
                                        (b.Spaces[i - dir, j - 1].getPieceId() == 22 & playerId == 1))
                                        & !b.Spaces[i - 2 * dir, j - 2].isOccupied())
                                    {
                                        count++;
                                    }
                            }
                        }

                        // If it is a King
                        else if ((b.Spaces[i, j].getPieces()[0].getId() == 11 & playerId == 1)
                            || b.Spaces[i, j].getPieces()[0].getId() == 22 & playerId == 2)
                        {
                            // Left up. k -1, l -1
                            int k = -1;
                            int l = -1;
                            bool jumpedPiece = false;
                            bool alreadyJumped = false;

                            while (i + k > -1) //|| i + k < n)
                            {
                                if (j + l > -1)
                                    if (!b.Spaces[i + k, j + l].isOccupied() & jumpedPiece & !alreadyJumped)
                                    {
                                        count++;
                                        alreadyJumped = true;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                                    {
                                        k *= b.n;
                                        l *= b.n;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((b.Spaces[i + k, j + l].getPieceId() != playerId & playerId < 10) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 11 & playerId == 2) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 22 & playerId == 1)) &
                                        (b.Spaces[i, j].getPieceId() != b.Spaces[i + k, j + l].getPieceId())) // If space is occupied, and jumpedPiece is false, make it true
                                    {
                                        jumpedPiece = true;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((b.Spaces[i + k, j + l].getPieceId() == playerId) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 11 & playerId == 1) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 22 & playerId == 2) ||
                                        (b.Spaces[i, j].getPieceId() == b.Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                                    {
                                        k *= b.n;
                                        l *= b.n;
                                    }


                                k -= 1;
                                l -= 1;
                            }


                            // Right up. k -1, l +1
                            k = -1;
                            l = 1;
                            jumpedPiece = false;
                            alreadyJumped = false;

                            while (i + k > -1) // || i + k < n)
                            {
                                if (j + l < b.n)
                                    if (!b.Spaces[i + k, j + l].isOccupied() & jumpedPiece & !alreadyJumped)
                                    {
                                        count++;
                                        alreadyJumped = true;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                                    {
                                        k *= b.n;
                                        l *= b.n;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((b.Spaces[i + k, j + l].getPieceId() != playerId & playerId < 10) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 11 & playerId == 2) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 22 & playerId == 1)) &
                                        (b.Spaces[i, j].getPieceId() != b.Spaces[i + k, j + l].getPieceId())) // If space is occupied, and jumpedPiece is false, make it true
                                    {
                                        jumpedPiece = true;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((b.Spaces[i + k, j + l].getPieceId() == playerId) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 11 & playerId == 1) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 22 & playerId == 2) ||
                                        (b.Spaces[i, j].getPieceId() == b.Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                                    {
                                        k *= b.n;
                                        l *= b.n;
                                    }

                                k -= 1;
                                l += 1;
                            }

                            // Left down. k +1, l -1
                            k = 1;
                            l = -1;
                            jumpedPiece = false;
                            alreadyJumped = false;

                            while (i + k < b.n) //(i + k > -1 || i + k < n)
                            {
                                if (j + l > -1)
                                    if (!b.Spaces[i + k, j + l].isOccupied() & jumpedPiece & !alreadyJumped)
                                    {
                                        count++;
                                        alreadyJumped = true;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                                    {
                                        k *= b.n;
                                        l *= b.n;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((b.Spaces[i + k, j + l].getPieceId() != playerId & playerId < 10) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 11 & playerId == 2) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 22 & playerId == 1)) &
                                        (b.Spaces[i, j].getPieceId() != b.Spaces[i + k, j + l].getPieceId())) // If space is occupied, and jumpedPiece is false, make it true
                                    {
                                        jumpedPiece = true;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((b.Spaces[i + k, j + l].getPieceId() == playerId) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 11 & playerId == 1) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 22 & playerId == 2) ||
                                        (b.Spaces[i, j].getPieceId() == b.Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                                    {
                                        k *= b.n;
                                        l *= b.n;
                                    }

                                k += 1;
                                l -= 1;
                            }

                            // Right down. k +1, l +1
                            k = 1;
                            l = 1;
                            jumpedPiece = false;
                            alreadyJumped = false;

                            while (i + k < b.n) //(i + k > -1 || i + k < n)
                            {
                                if (j + l < b.n)
                                    if (!b.Spaces[i + k, j + l].isOccupied() & jumpedPiece & !alreadyJumped)
                                    {
                                        count++;
                                        alreadyJumped = true;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                                    {
                                        k *= b.n;
                                        l *= b.n;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((b.Spaces[i + k, j + l].getPieceId() != playerId & playerId < 10) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 11 & playerId == 2) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 22 & playerId == 1)) &
                                        (b.Spaces[i, j].getPieceId() != b.Spaces[i + k, j + l].getPieceId())) // If space is occupied, and jumpedPiece is false, make it true
                                    {
                                        jumpedPiece = true;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((b.Spaces[i + k, j + l].getPieceId() == playerId) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 11 & playerId == 1) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 22 & playerId == 2) ||
                                        (b.Spaces[i, j].getPieceId() == b.Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                                    {
                                        k *= b.n;
                                        l *= b.n;
                                    }

                                k += 1;
                                l += 1;
                            }
                        }
                    }
                }
            }

            return count;
        }

        private int[] CountKings(Board b)
        {
            int[] count = new int[] { 0, 0 };
            for (int i = 0; i < b.n; i++)
            {
                for (int j = 0; j < b.n; j++)
                {
                    if (b.Spaces[i, j].getPieceId() == 11)
                        count[b.currentPlayer.id == 1 ? 0 : 1]++;
                    else if (b.Spaces[i, j].getPieceId() == 22)
                        count[b.currentPlayer.id == 1 ? 1 : 0]++;
                }
            }

            return count;
        }

        private int CountFirstRow(Board b)
        {
            int firstRow = (b.currentPlayer.getId() == 1 ? b.n-1 : 0);
            int count = 0;
            for (int col = 0; col < b.n; col++)
            {
                if (b.Spaces[firstRow, col].getPieceId() == b.currentPlayer.getId() ||
                    (b.Spaces[firstRow, col].getPieceId() == 11 & b.currentPlayer.getId() == 1) ||
                    (b.Spaces[firstRow, col].getPieceId() == 22 & b.currentPlayer.getId() == 2))
                    count++;
            }

            return count;
        }
    }

    [Serializable]
    internal class AlphaBetaBot : Player
    {
        NN nn;
        private int max_depth;
        private Space[] best_move;
        private double alpha; // Minimum
        private double beta; // Maximum
        private double[] scores;

        public AlphaBetaBot(int id, int max_depth, int[] hidden, double[] weights)
        {
            this.id = id;
            this.nn = new NN(7, hidden, 1);
            this.nn.setWeights(weights);
            this.max_depth = max_depth;
        }

        public override void makeMove()
        {
            // Get Possible moves
            List<Space[]> moves = B.getPossibleMoves();

            // Make array for all moves
            //scores = new double[moves.Count()];

            // If there are moves available
            if (moves.Count() > 0)
            {
                AlphaBetaSearch();

                // Return best move
                Space[] best_move = moves[this.scores.ToList().IndexOf(this.scores.Max())];

                // Make best move
                B.movePiece(best_move);
            }
        }

        private void AlphaBetaSearch()
        {
            // Reset alpha and beta
            this.alpha = -10000;
            this.beta = 10000;

            // Start recursive search
            maxValue(this.B, 0);
        }

        private double maxValue(Board bd, int depth)
        {
            // Check for terminal state (max depth or end of game)
            if (depth == this.max_depth || !bd.inGame())
                return this.nn.forward(calculateFeatures(bd))[0];

            // Get possible moves
            List<Space[]> moves = bd.getPossibleMoves();

            // If new, reset scores
            if (depth == 0)
                this.scores = new double[moves.Count()];

            // Start min-max
            double v = -10000;
            for (int i = 0; i < moves.Count(); i++)
            {
                // Get move
                Space[] move = moves[i];

                // Copy board
                //Board temp_board = bd.DeepClone();

                // Move piece
                Space[] reset_move = bd.simulateMove(move);

                // Convert to King pieces
                Space reset_king = bd.simulateConvertToKing();

                // Change sides
                if (bd.currentPlayer.getId() == 1)
                    bd.currentPlayer = bd.player2;
                else
                    bd.currentPlayer = bd.player1;

                // Update v
                v = Math.Max(v,minValue(bd, depth+1));

                // Change sides back
                if (bd.currentPlayer.getId() == 1)
                    bd.currentPlayer = bd.player2;
                else
                    bd.currentPlayer = bd.player1;

                // Reset board with temp Board
                //bd = temp_board;
                bd.simulateConvertToPiece(reset_king);
                bd.UndoMove(move, reset_move);

                // If depth = 0, save score
                if (depth == 0)
                    this.scores[i] = v;

                //if (depth == 0 & i == 0)
                //{
                //    Console.WriteLine("Nr " + i);
                //    bd.printColored();
                //}

                // If v is higher than beta, return v
                if (v >= this.beta)
                    return v;

                // Update alpha
                this.alpha = Math.Max(this.alpha, v);
            }

            // If nothing has been returned return, return v
            return v;
        }

        private double minValue(Board bd, int depth)
        {
            // Check for terminal state (max depth or end of game)
            if (depth == this.max_depth || !bd.inGame())
                return this.nn.forward(calculateFeatures(bd))[0];

            // Get possible moves
            List<Space[]> moves = bd.getPossibleMoves();

            // Start min-max
            double v = 10000;
            for (int i = 0; i < moves.Count(); i++)
            {
                // Get move
                Space[] move = moves[i];

                // Copy board
                //Board temp_board = bd.DeepClone();

                // Move piece
                Space[] reset_move = bd.simulateMove(move);

                // Convert to King pieces
                Space reset_king = bd.simulateConvertToKing();

                // Change sides
                if (bd.currentPlayer.getId() == 1)
                    bd.currentPlayer = bd.player2;
                else
                    bd.currentPlayer = bd.player1;

                // Update v (and return)
                v = Math.Min(v, maxValue(bd, depth + 1));

                // Change sides back
                if (bd.currentPlayer.getId() == 1)
                    bd.currentPlayer = bd.player2;
                else
                    bd.currentPlayer = bd.player1;

                // Reset board with temp Board
                //bd = temp_board;
                bd.simulateConvertToPiece(reset_king);
                bd.UndoMove(move, reset_move);

                if (v <= this.alpha)
                    return v;

                // Update alpha
                this.beta = Math.Min(this.beta, v);
            }

            // If no return, return v
            return v;
        }

        private double[] calculateFeatures(Board b)
        {
            double[] result = new double[7];

            // Pieces in danger (that could be jumped (own))
            result[0] = PiecesToBeJumped(b, b.currentPlayer.id == 1 ? 2 : 1);

            // Pieces that can be jumped (opponent)
            result[1] = PiecesToBeJumped(b, b.currentPlayer.id == 1 ? 1 : 2);

            // Amount of ((own) pieces on first row (home)
            result[2] = CountFirstRow(b);

            // Number of Kings (own)
            int[] cntKings = CountKings(b);
            result[3] = cntKings[0];

            // Number of Kings (opponent)
            result[3] = cntKings[0];

            // Pieces in Play (own)
            int[] cntPieces = b.CountPieces();
            result[5] = cntPieces[b.currentPlayer.id == 1 ? 0 : 1];

            // Pieces in Play (opponent)
            result[6] = cntPieces[b.currentPlayer.id == 1 ? 1 : 0];

            return result;
        }

        private int PiecesToBeJumped(Board b, int playerId)
        {
            int count = 0;

            for (int i = 0; i < b.n; i++)
            {
                for (int j = 0; j < b.n; j++)
                {
                    if (b.Spaces[i, j].isOccupied())
                    {
                        if (b.Spaces[i, j].getPieces()[0].getId() == playerId)
                        {
                            int dir = b.Spaces[i, j].getPieces()[0].getDir(); // TODO link to currentPlayerId

                            ///// Check for  forward jumps
                            if (i + 2 * dir < 0 || i + 2 * dir > b.n - 1)
                            { }
                            // Else, only add if piece stays on the board
                            else
                            {
                                // If next square has an enemy piece, and the space after that is free
                                if (j + 2 < b.n)
                                    if (b.Spaces[i + dir, j + 1].isOccupied() &
                                        ((b.Spaces[i + dir, j + 1].getPieceId() != playerId & playerId < 10) ||
                                        (b.Spaces[i + dir, j + 1].getPieceId() == 11 & playerId == 2) ||
                                        (b.Spaces[i + dir, j + 1].getPieceId() == 22 & playerId == 1))
                                        & !b.Spaces[i + 2 * dir, j + 2].isOccupied())
                                    {
                                        count++;
                                    }

                                if (j - 2 > -1)
                                    if (b.Spaces[i + dir, j - 1].isOccupied() &
                                        ((b.Spaces[i + dir, j - 1].getPieceId() != playerId & playerId < 10) ||
                                        (b.Spaces[i + dir, j - 1].getPieceId() == 11 & playerId == 2) ||
                                        (b.Spaces[i + dir, j - 1].getPieceId() == 22 & playerId == 1))
                                        & !b.Spaces[i + 2 * dir, j - 2].isOccupied())
                                    {
                                        count++;
                                    }
                            }

                            ///// Check for backward jumps
                            if (i - 2 * dir < 0 || i - 2 * dir > b.n - 1)
                            { }
                            // Else, only add if piece stays on the board
                            else
                            {
                                // If next square has an enemy piece, and the space after that is free
                                if (j + 2 < b.n)
                                    if (b.Spaces[i - dir, j + 1].isOccupied() &
                                        ((b.Spaces[i - dir, j + 1].getPieceId() != playerId & playerId < 10) ||
                                        (b.Spaces[i - dir, j + 1].getPieceId() == 11 & playerId == 2) ||
                                        (b.Spaces[i - dir, j + 1].getPieceId() == 22 & playerId == 1))
                                        & !b.Spaces[i - 2 * dir, j + 2].isOccupied())
                                    {
                                        count++;
                                    }

                                if (j - 2 > -1)
                                    if (b.Spaces[i - dir, j - 1].isOccupied() &
                                        ((b.Spaces[i - dir, j - 1].getPieceId() != playerId & playerId < 10) ||
                                        (b.Spaces[i - dir, j - 1].getPieceId() == 11 & playerId == 2) ||
                                        (b.Spaces[i - dir, j - 1].getPieceId() == 22 & playerId == 1))
                                        & !b.Spaces[i - 2 * dir, j - 2].isOccupied())
                                    {
                                        count++;
                                    }
                            }
                        }

                        // If it is a King
                        else if ((b.Spaces[i, j].getPieces()[0].getId() == 11 & playerId == 1)
                            || b.Spaces[i, j].getPieces()[0].getId() == 22 & playerId == 2)
                        {
                            // Left up. k -1, l -1
                            int k = -1;
                            int l = -1;
                            bool jumpedPiece = false;
                            bool alreadyJumped = false;

                            while (i + k > -1) //|| i + k < n)
                            {
                                if (j + l > -1)
                                    if (!b.Spaces[i + k, j + l].isOccupied() & jumpedPiece & !alreadyJumped)
                                    {
                                        count++;
                                        alreadyJumped = true;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                                    {
                                        k *= b.n;
                                        l *= b.n;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((b.Spaces[i + k, j + l].getPieceId() != playerId & playerId < 10) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 11 & playerId == 2) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 22 & playerId == 1)) &
                                        (b.Spaces[i, j].getPieceId() != b.Spaces[i + k, j + l].getPieceId())) // If space is occupied, and jumpedPiece is false, make it true
                                    {
                                        jumpedPiece = true;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((b.Spaces[i + k, j + l].getPieceId() == playerId) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 11 & playerId == 1) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 22 & playerId == 2) ||
                                        (b.Spaces[i, j].getPieceId() == b.Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                                    {
                                        k *= b.n;
                                        l *= b.n;
                                    }


                                k -= 1;
                                l -= 1;
                            }


                            // Right up. k -1, l +1
                            k = -1;
                            l = 1;
                            jumpedPiece = false;
                            alreadyJumped = false;

                            while (i + k > -1) // || i + k < n)
                            {
                                if (j + l < b.n)
                                    if (!b.Spaces[i + k, j + l].isOccupied() & jumpedPiece & !alreadyJumped)
                                    {
                                        count++;
                                        alreadyJumped = true;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                                    {
                                        k *= b.n;
                                        l *= b.n;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((b.Spaces[i + k, j + l].getPieceId() != playerId & playerId < 10) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 11 & playerId == 2) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 22 & playerId == 1)) &
                                        (b.Spaces[i, j].getPieceId() != b.Spaces[i + k, j + l].getPieceId())) // If space is occupied, and jumpedPiece is false, make it true
                                    {
                                        jumpedPiece = true;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((b.Spaces[i + k, j + l].getPieceId() == playerId) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 11 & playerId == 1) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 22 & playerId == 2) ||
                                        (b.Spaces[i, j].getPieceId() == b.Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                                    {
                                        k *= b.n;
                                        l *= b.n;
                                    }

                                k -= 1;
                                l += 1;
                            }

                            // Left down. k +1, l -1
                            k = 1;
                            l = -1;
                            jumpedPiece = false;
                            alreadyJumped = false;

                            while (i + k < b.n) //(i + k > -1 || i + k < n)
                            {
                                if (j + l > -1)
                                    if (!b.Spaces[i + k, j + l].isOccupied() & jumpedPiece & !alreadyJumped)
                                    {
                                        count++;
                                        alreadyJumped = true;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                                    {
                                        k *= b.n;
                                        l *= b.n;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((b.Spaces[i + k, j + l].getPieceId() != playerId & playerId < 10) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 11 & playerId == 2) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 22 & playerId == 1)) &
                                        (b.Spaces[i, j].getPieceId() != b.Spaces[i + k, j + l].getPieceId())) // If space is occupied, and jumpedPiece is false, make it true
                                    {
                                        jumpedPiece = true;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((b.Spaces[i + k, j + l].getPieceId() == playerId) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 11 & playerId == 1) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 22 & playerId == 2) ||
                                        (b.Spaces[i, j].getPieceId() == b.Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                                    {
                                        k *= b.n;
                                        l *= b.n;
                                    }

                                k += 1;
                                l -= 1;
                            }

                            // Right down. k +1, l +1
                            k = 1;
                            l = 1;
                            jumpedPiece = false;
                            alreadyJumped = false;

                            while (i + k < b.n) //(i + k > -1 || i + k < n)
                            {
                                if (j + l < b.n)
                                    if (!b.Spaces[i + k, j + l].isOccupied() & jumpedPiece & !alreadyJumped)
                                    {
                                        count++;
                                        alreadyJumped = true;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                                    {
                                        k *= b.n;
                                        l *= b.n;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((b.Spaces[i + k, j + l].getPieceId() != playerId & playerId < 10) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 11 & playerId == 2) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 22 & playerId == 1)) &
                                        (b.Spaces[i, j].getPieceId() != b.Spaces[i + k, j + l].getPieceId())) // If space is occupied, and jumpedPiece is false, make it true
                                    {
                                        jumpedPiece = true;
                                    }
                                    else if (b.Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((b.Spaces[i + k, j + l].getPieceId() == playerId) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 11 & playerId == 1) ||
                                        (b.Spaces[i + k, j + l].getPieceId() == 22 & playerId == 2) ||
                                        (b.Spaces[i, j].getPieceId() == b.Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                                    {
                                        k *= b.n;
                                        l *= b.n;
                                    }

                                k += 1;
                                l += 1;
                            }
                        }
                    }
                }
            }

            return count;
        }

        private int[] CountKings(Board b)
        {
            int[] count = new int[] { 0, 0 };
            for (int i = 0; i < b.n; i++)
            {
                for (int j = 0; j < b.n; j++)
                {
                    if (b.Spaces[i, j].getPieceId() == 11)
                        count[b.currentPlayer.id == 1 ? 0 : 1]++;
                    else if (b.Spaces[i, j].getPieceId() == 22)
                        count[b.currentPlayer.id == 1 ? 1 : 0]++;
                }
            }

            return count;
        }

        private int CountFirstRow(Board b)
        {
            int firstRow = (b.currentPlayer.getId() == 1 ? b.n - 1 : 0);
            int count = 0;
            for (int col = 0; col < b.n; col++)
            {
                if (b.Spaces[firstRow, col].getPieceId() == b.currentPlayer.getId() ||
                    (b.Spaces[firstRow, col].getPieceId() == 11 & b.currentPlayer.getId() == 1) ||
                    (b.Spaces[firstRow, col].getPieceId() == 22 & b.currentPlayer.getId() == 2))
                    count++;
            }

            return count;
        }
    }
}
