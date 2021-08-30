using System;
using System.Collections.Generic;
using System.Linq;
using PieceC;
using SpaceC;
using PlayerC;

namespace BoardC
{
    internal class Board
    {
        public int n = 8;
        public Space[,] Spaces = new Space[8, 8];
        public Player player1;
        public Player player2;
        public Player currentPlayer;

        public Board()
        {
            createSpaces();
            AddPieces();
            printColored();
        }

        // Add All pieces to the board
        public void AddPieces()
        {
            for (int id = 1; id < 3; id++) AddPiecesPlayer(id);
        }

        // Add pieces to the board for one player
        public void AddPiecesPlayer(int player_id)
        {
            // Player one at the bottom
            if (player_id == 1)
            {
                for (int i = this.n - 1; i > this.n - 4; i--)
                {
                    for (int j = 0; j < this.n; j++)
                    {
                        if (i % 2 != 0 & j % 2 == 0){ this.Spaces[i, j].addPiece(new Piece(player_id)); }
                        else if (i % 2 == 0 & j % 2 != 0) { this.Spaces[i, j].addPiece(new Piece(player_id)); }
                    }
                }
            }
            // Player two at the top
            else if (player_id == 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i % 2 != 0 & j % 2 == 0) { this.Spaces[i, j].addPiece(new Piece(player_id)); }
                        else if (i % 2 == 0 & j % 2 != 0) { this.Spaces[i, j].addPiece(new Piece(player_id)); }

                        //if (boardCoords2Id(i, j) == 14)
                        //{
                        //    this.Spaces[i, j].getPieces().RemoveAt(0);
                        //    this.Spaces[i + 3, j - 3].addPiece(new Piece(player_id));
                        //}
                    }
                }
            }
        }

        public Space simulateConvertToKing()
        {
            if (currentPlayer.id == 1)
            {
                for (int j = 0; j < n; j++)
                {
                    if (Spaces[0, j].isOccupied())
                    {
                        if (Spaces[0, j].getPieceId() == currentPlayer.id)
                        {
                            // Change id of piece
                            Spaces[0, j].getPieces()[0].id = 11;
                            return Spaces[0, j];
                        }
                    }
                }
            }

            else if (currentPlayer.id == 2)
            {
                for (int j = 0; j < n; j++)
                {
                    if (Spaces[n - 1, j].isOccupied())
                    {
                        if (Spaces[n - 1, j].getPieceId() == currentPlayer.id)
                        {
                            // Change id of piece
                            Spaces[n - 1, j].getPieces()[0].id = 22;
                            return Spaces[n - 1, j];
                        }
                    }
                }
            }

            return new Space(99);
        }

        public void simulateConvertToPiece(Space reset)
        {
            if (reset.getId() != 99)
            {
                if (reset.getPieceId() == 11)
                {
                    reset.getPieces()[0].id = 1;
                }
                else if (reset.getPieceId() == 22)
                {
                    reset.getPieces()[0].id = 2;
                }
            }
        }

        public void convertToKing()
        {
            if (currentPlayer.id == 1)
            {
                for (int j = 0; j < n; j++)
                {
                    if (Spaces[0, j].isOccupied())
                    {
                        if (Spaces[0, j].getPieceId() == currentPlayer.id)
                        {
                            // Change id of piece
                            Spaces[0, j].getPieces()[0].id = 11;
                        }
                    }
                }
            }

            else if (currentPlayer.id == 2)
            {
                for (int j = 0; j < n; j++)
                {
                    if (Spaces[n-1, j].isOccupied())
                    {
                        if (Spaces[n-1, j].getPieceId() == currentPlayer.id)
                        {
                            // Change id of piece
                            Spaces[n-1, j].getPieces()[0].id = 22;
                        }
                    }
                }
            }
        }

        public List<Space[]> getPossibleMoves()
        {
            List<Space[]> moves = getPossibleJumps();

            // Check if double jumps are possible
            if (moves.Count() > 0)
            { 
                int nxt = countSpaces(moves);
                int old = nxt - 1;

                while (nxt != old)
                {
                    moves = getPossibleDoubleJumps(moves);
                    old = nxt;
                    nxt = countSpaces(moves);
                }
            }

            else
                moves = getPossibleSteps();

            return moves;
        }

        private int countSpaces(List<Space[]> lst)
        {
            int count = 0;
            for (int i = 0; i < lst.Count(); i++)
            {
                count += lst[i].Length;
            }

            return count;
        }

        private List<Space[]> getPossibleSteps()
        {
            List<Space[]> moves = new List<Space[]>();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (Spaces[i, j].isOccupied())
                    {
                        if (Spaces[i, j].getPieces()[0].getId() == currentPlayer.getId())
                        {
                            int dir = Spaces[i, j].getPieces()[0].getDir();

                            // [5,5] -> [4,3] + [4,6] -> [i+dir,j+-1] ---- Normal
                            // [5,7] -> [4,6] -> if j+1 > n or j-1 < 0
                            // [0,5] -> if i+dir < 0 or i+dir > n

                            ///// Check normal steps
                            // If out of reach, don't add
                            if (i + dir < 0 || i + dir > n - 1)
                                { }
                            // Else, only add if piece stays on the board
                            else
                            {
                                if (j + 1 < n)
                                    if (!Spaces[i + dir, j + 1].isOccupied())
                                    {
                                        moves.Add(new Space[2] { Spaces[i, j], Spaces[i + dir, j + 1] });
                                    }

                                if (j - 1 > -1)
                                    if (!Spaces[i + dir, j - 1].isOccupied())
                                    {
                                        moves.Add(new Space[2] { Spaces[i, j], Spaces[i + dir, j - 1] });
                                    }
                            }
                        }
                        
                        // If it is a King
                        else if ((Spaces[i, j].getPieces()[0].getId() == 11 & currentPlayer.getId() == 1)
                            || Spaces[i, j].getPieces()[0].getId() == 22 & currentPlayer.getId() == 2)
                        {
                            // Left up. k -1, l -1
                            int k = -1;
                            int l = -1;

                            while (i + k > -1) //|| i + k < n)
                            {
                                if (j + l > -1)
                                    if (!Spaces[i + k, j + l].isOccupied())
                                    {
                                        moves.Add(new Space[2] { Spaces[i, j], Spaces[i + k, j + l] });
                                    }
                                    else // Stop by increasing k and l
                                    {
                                        k *= n;
                                        l *= n;
                                    }

                                k -= 1;
                                l -= 1; 
                            }


                            // Right up. k -1, l +1
                            k = -1;
                            l = 1;

                            while (i + k > -1) // || i + k < n)
                            {
                                if (j + l < n)
                                    if (!Spaces[i + k, j + l].isOccupied())
                                    {
                                        moves.Add(new Space[2] { Spaces[i, j], Spaces[i + k, j + l] });
                                    }
                                    else // Stop by increasing k and l
                                    {
                                        k *= n;
                                        l *= n;
                                    }

                                k -= 1;
                                l += 1;
                            }

                            // Left down. k +1, l -1
                            k = 1;
                            l = -1;

                            while (i + k < n) //(i + k > -1 || i + k < n)
                            {
                                if (j + l > -1)
                                    if (!Spaces[i + k, j + l].isOccupied())
                                    {
                                        moves.Add(new Space[2] { Spaces[i, j], Spaces[i + k, j + l] });
                                    }
                                    else // Stop by increasing k and l
                                    {
                                        k *= n;
                                        l *= n;
                                    }

                                k += 1;
                                l -= 1;
                            }

                            // Right down. k +1, l +1
                            k = 1;
                            l = 1;

                            while (i + k < n) //(i + k > -1 || i + k < n)
                            {
                                if (j + l < n)
                                    if (!Spaces[i + k, j + l].isOccupied())
                                    {
                                        moves.Add(new Space[2] { Spaces[i, j], Spaces[i + k, j + l] });
                                    }
                                    else // Stop by increasing k and l
                                    {
                                        k *= n;
                                        l *= n;
                                    }

                                k += 1;
                                l += 1;
                            }
                        }
                    }
                }
            }

            return moves;
        }

        private List<Space[]> getPossibleJumps()
        {
            List<Space[]> moves = new List<Space[]>();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (Spaces[i, j].isOccupied())
                    {
                        if (Spaces[i, j].getPieces()[0].getId() == currentPlayer.getId())
                        {
                            int dir = Spaces[i, j].getPieces()[0].getDir(); // TODO link to currentPlayerId

                            ///// Check for  forward jumps
                            if (i + 2 * dir < 0 || i + 2 * dir > n - 1)
                                { }
                            // Else, only add if piece stays on the board
                            else
                            {
                                // If next square has an enemy piece, and the space after that is free
                                if (j + 2 < n)
                                    if (Spaces[i + dir, j + 1].isOccupied() &
                                        ((Spaces[i + dir, j + 1].getPieceId() != currentPlayer.getId() & Spaces[i + dir, j + 1].getPieceId() < 10) ||
                                        (Spaces[i + dir, j + 1].getPieceId() == 11 & currentPlayer.getId() == 2) ||
                                        (Spaces[i + dir, j + 1].getPieceId() == 22 & currentPlayer.getId() == 1)) 
                                        & !Spaces[i + 2 * dir, j + 2].isOccupied())
                                    {
                                        moves.Add(new Space[2] { Spaces[i, j], Spaces[i + 2 * dir, j + 2] });
                                    }

                                if (j - 2 > -1)
                                    if (Spaces[i + dir, j - 1].isOccupied() &
                                        ((Spaces[i + dir, j - 1].getPieceId() != currentPlayer.getId() & Spaces[i + dir, j - 1].getPieceId() < 10) ||
                                        (Spaces[i + dir, j - 1].getPieceId() == 11 & currentPlayer.getId() == 2) ||
                                        (Spaces[i + dir, j - 1].getPieceId() == 22 & currentPlayer.getId() == 1)) 
                                        & !Spaces[i + 2 * dir, j - 2].isOccupied())
                                    {
                                        moves.Add(new Space[2] { Spaces[i, j], Spaces[i + 2 * dir, j - 2] });
                                    }
                            }

                            ///// Check for backward jumps
                            if (i - 2 * dir < 0 || i - 2 * dir > n - 1)
                                { }
                            // Else, only add if piece stays on the board
                            else
                            {
                                // If next square has an enemy piece, and the space after that is free
                                if (j + 2 < n)
                                    if (Spaces[i - dir, j + 1].isOccupied() & 
                                        ((Spaces[i - dir, j + 1].getPieceId() != currentPlayer.getId() & Spaces[i - dir, j + 1].getPieceId() < 10) ||
                                        (Spaces[i - dir, j + 1].getPieceId() == 11 & currentPlayer.getId() == 2) ||
                                        (Spaces[i - dir, j + 1].getPieceId() == 22 & currentPlayer.getId() == 1)) 
                                        & !Spaces[i - 2 * dir, j + 2].isOccupied())
                                    {
                                        moves.Add(new Space[2] { Spaces[i, j], Spaces[i - 2 * dir, j + 2] });
                                    }

                                if (j - 2 > -1)
                                    if (Spaces[i - dir, j - 1].isOccupied() &
                                        ((Spaces[i - dir, j - 1].getPieceId() != currentPlayer.getId() & Spaces[i - dir, j - 1].getPieceId() < 10) ||
                                        (Spaces[i - dir, j - 1].getPieceId() == 11 & currentPlayer.getId() == 2) ||
                                        (Spaces[i - dir, j - 1].getPieceId() == 22 & currentPlayer.getId() == 1))
                                        & !Spaces[i - 2 * dir, j - 2].isOccupied())
                                    {
                                        moves.Add(new Space[2] { Spaces[i, j], Spaces[i - 2 * dir, j - 2] });
                                    }
                            }
                        }

                        // If it is a King
                        else if ((Spaces[i, j].getPieces()[0].getId() == 11 & currentPlayer.getId() == 1)
                            || Spaces[i, j].getPieces()[0].getId() == 22 & currentPlayer.getId() == 2)
                        {
                            // Left up. k -1, l -1
                            int k = -1;
                            int l = -1;
                            bool jumpedPiece = false;

                            while (i + k > -1) //|| i + k < n)
                            {
                                if (j + l > -1)
                                    if (!Spaces[i + k, j + l].isOccupied() & jumpedPiece)
                                    {
                                        moves.Add(new Space[2] { Spaces[i, j], Spaces[i + k, j + l] });
                                        //Console.WriteLine("Maybe here");
                                    }
                                    else if (Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                                    {
                                        k *= n;
                                        l *= n;
                                    }
                                    else if (Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((Spaces[i + k, j + l].getPieceId() != currentPlayer.getId() & Spaces[i + k, j + l].getPieceId() < 10) ||
                                        (Spaces[i + k, j + l].getPieceId() == 11 & currentPlayer.getId() == 2) ||
                                        (Spaces[i + k, j + l].getPieceId() == 22 & currentPlayer.getId() == 1)) &
                                        (Spaces[i, j].getPieceId() != Spaces[i + k, j + l].getPieceId())) // If space is occupied, and jumpedPiece is false, make it true
                                    {
                                        jumpedPiece = true;
                                    }
                                    else if (Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((Spaces[i + k, j + l].getPieceId() == currentPlayer.getId()) ||
                                        (Spaces[i + k, j + l].getPieceId() == 11 & currentPlayer.getId() == 1) ||
                                        (Spaces[i + k, j + l].getPieceId() == 22 & currentPlayer.getId() == 2) ||
                                        (Spaces[i, j].getPieceId() == Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                                    {
                                        k *= n;
                                        l *= n;
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
                                if (j + l < n)
                                    if (!Spaces[i + k, j + l].isOccupied() & jumpedPiece)
                                    {
                                        moves.Add(new Space[2] { Spaces[i, j], Spaces[i + k, j + l] });
                                        //Console.WriteLine("Maybe here");
                                    }
                                    else if (Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                                    {
                                        k *= n;
                                        l *= n;
                                    }
                                    else if (Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((Spaces[i + k, j + l].getPieceId() != currentPlayer.getId() & Spaces[i + k, j + l].getPieceId() < 10) ||
                                        (Spaces[i + k, j + l].getPieceId() == 11 & currentPlayer.getId() == 2) ||
                                        (Spaces[i + k, j + l].getPieceId() == 22 & currentPlayer.getId() == 1)) &
                                        (Spaces[i, j].getPieceId() != Spaces[i + k, j + l].getPieceId())) // If space is occupied, and jumpedPiece is false, make it true
                                    {
                                        jumpedPiece = true;
                                    }
                                    else if (Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((Spaces[i + k, j + l].getPieceId() == currentPlayer.getId()) ||
                                        (Spaces[i + k, j + l].getPieceId() == 11 & currentPlayer.getId() == 1) ||
                                        (Spaces[i + k, j + l].getPieceId() == 22 & currentPlayer.getId() == 2) ||
                                        (Spaces[i, j].getPieceId() == Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                                    {
                                        k *= n;
                                        l *= n;
                                    }

                                k -= 1;
                                l += 1;
                            }

                            // Left down. k +1, l -1
                            k = 1;
                            l = -1;
                            jumpedPiece = false;

                            while (i + k < n) //(i + k > -1 || i + k < n)
                            {
                                if (j + l > -1)
                                    if (!Spaces[i + k, j + l].isOccupied() & jumpedPiece)
                                    {
                                        moves.Add(new Space[2] { Spaces[i, j], Spaces[i + k, j + l] });
                                        //Console.WriteLine("Maybe here");
                                    }
                                    else if (Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                                    {
                                        k *= n;
                                        l *= n;
                                    }
                                    else if (Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((Spaces[i + k, j + l].getPieceId() != currentPlayer.getId() & Spaces[i + k, j + l].getPieceId() < 10) ||
                                        (Spaces[i + k, j + l].getPieceId() == 11 & currentPlayer.getId() == 2) ||
                                        (Spaces[i + k, j + l].getPieceId() == 22 & currentPlayer.getId() == 1)) &
                                        (Spaces[i, j].getPieceId() != Spaces[i + k, j + l].getPieceId())) // If space is occupied, and jumpedPiece is false, make it true
                                    {
                                        jumpedPiece = true;
                                    }
                                    else if (Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((Spaces[i + k, j + l].getPieceId() == currentPlayer.getId()) ||
                                        (Spaces[i + k, j + l].getPieceId() == 11 & currentPlayer.getId() == 1) ||
                                        (Spaces[i + k, j + l].getPieceId() == 22 & currentPlayer.getId() == 2) ||
                                        (Spaces[i, j].getPieceId() == Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                                    {
                                        k *= n;
                                        l *= n;
                                    }

                                k += 1;
                                l -= 1;
                            }

                            // Right down. k +1, l +1
                            k = 1;
                            l = 1;
                            jumpedPiece = false;

                            while (i + k < n) //(i + k > -1 || i + k < n)
                            {
                                if (j + l < n)
                                    if (!Spaces[i + k, j + l].isOccupied() & jumpedPiece)
                                    {
                                        moves.Add(new Space[2] { Spaces[i, j], Spaces[i + k, j + l] });
                                        //Console.WriteLine("Maybe here");
                                    }
                                    else if (Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                                    {
                                        k *= n;
                                        l *= n;
                                    }
                                    else if (Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((Spaces[i + k, j + l].getPieceId() != currentPlayer.getId() & Spaces[i + k, j + l].getPieceId() < 10) ||
                                        (Spaces[i + k, j + l].getPieceId() == 11 & currentPlayer.getId() == 2) ||
                                        (Spaces[i + k, j + l].getPieceId() == 22 & currentPlayer.getId() == 1)) &
                                        (Spaces[i, j].getPieceId() != Spaces[i + k, j + l].getPieceId())) // If space is occupied, and jumpedPiece is false, make it true
                                    {
                                        jumpedPiece = true;
                                    }
                                    else if (Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                        ((Spaces[i + k, j + l].getPieceId() == currentPlayer.getId()) ||
                                        (Spaces[i + k, j + l].getPieceId() == 11 & currentPlayer.getId() == 1) ||
                                        (Spaces[i + k, j + l].getPieceId() == 22 & currentPlayer.getId() == 2) ||
                                        (Spaces[i, j].getPieceId() == Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                                    {
                                        k *= n;
                                        l *= n;
                                    }

                                k += 1;
                                l += 1;
                            }
                        }
                    }
                }
            }

            return moves;
        }

        private List<Space[]> getPossibleDoubleJumps(List<Space[]> startmoves)
        {
            List<Space[]> moves = new List<Space[]>();

            for (int indx = 0; indx < startmoves.Count(); indx++)
            {
                Space[] startmove = startmoves[indx];
                List<int> jumpedOver = new List<int>();
                bool added = false;

                // Determine and add space that got jumped over
                for (int steps = 0; steps < startmove.Length-1; steps++)
                {
                    // Get coords
                    int[] from = boardId2Coords(startmove[steps].getId());
                    int[] to1 = boardId2Coords(startmove[steps + 1].getId());

                    int row = from[0];
                    int col = from[1];

                    // Check which direction to check
                    int i_dir = from[0] < to1[0] ? 1 : -1;
                    int j_dir = from[1] < to1[1] ? 1 : -1;

                    // Check every space on diagonal between steps if there is a piece
                    for (int iter = 1; iter < Math.Abs(from[0] - to1[0]); iter++)
                    {
                        if (this.Spaces[row + iter * i_dir, col + iter * j_dir].isOccupied())
                        {
                            // Remove piece that got jumped
                            jumpedOver.Add(boardCoords2Id(row + iter * i_dir, col + iter * j_dir));
                        }
                    }
                }

                /// Get position of last jump
                int[] to = boardId2Coords(startmove[startmove.Length - 1].getId());
                int i = to[0];
                int j = to[1];

                ////// Check for new jump in new space ("to")
                // Get direction
                int dir = startmove[0].getPieces()[0].getDir(); // TODO link to currentPlayerId

                if (startmove[0].getPieceId() == currentPlayer.getId())
                {
                    ///// Check for  forward jumps
                    if (i + 2 * dir < 0 || i + 2 * dir > n - 1)
                    { }
                    // Else, only add if piece stays on the board
                    else
                    {
                        // If next square has an enemy piece, and the space after that is free
                        if (j + 2 < n)
                            if (Spaces[i + dir, j + 1].isOccupied() & 
                                ((Spaces[i + dir, j + 1].getPieceId() != currentPlayer.getId() & Spaces[i + dir, j + 1].getPieceId() < 10) ||
                                (Spaces[i + dir, j + 1].getPieceId() == 11 & currentPlayer.getId() == 2) ||
                                (Spaces[i + dir, j + 1].getPieceId() == 22 & currentPlayer.getId() == 1)) &
                                !jumpedOver.Contains(Spaces[i + dir, j + 1].getId()) & !Spaces[i + 2 * dir, j + 2].isOccupied())
                            {
                                jumpedOver.Add(Spaces[i + dir, j + 1].getId());
                                moves.Add(mergeArrays(startmove, new Space[] { Spaces[i + 2 * dir, j + 2] }));
                                added = true;
                            }

                        if (j - 2 > -1)
                            if (Spaces[i + dir, j - 1].isOccupied() &
                                ((Spaces[i + dir, j - 1].getPieceId() != currentPlayer.getId() & Spaces[i + dir, j - 1].getPieceId() < 10) ||
                                (Spaces[i + dir, j - 1].getPieceId() == 11 & currentPlayer.getId() == 2) ||
                                (Spaces[i + dir, j - 1].getPieceId() == 22 & currentPlayer.getId() == 1)) &
                                !jumpedOver.Contains(Spaces[i + dir, j - 1].getId()) & !Spaces[i + 2 * dir, j - 2].isOccupied())
                            {
                                jumpedOver.Add(Spaces[i + dir, j - 1].getId());
                                moves.Add(mergeArrays(startmove, new Space[] { Spaces[i + 2 * dir, j - 2] }));
                                added = true;
                            }
                    }

                    ///// Check for backward jumps
                    if (i - 2 * dir < 0 || i - 2 * dir > n - 1)
                    { }
                    // Else, only add if piece stays on the board
                    else
                    {
                        // If next square has an enemy piece, and the space after that is free
                        if (j + 2 < n)
                            if (Spaces[i - dir, j + 1].isOccupied() & 
                                ((Spaces[i - dir, j + 1].getPieceId() != currentPlayer.getId() & Spaces[i - dir, j + 1].getPieceId() < 10) ||
                                (Spaces[i - dir, j + 1].getPieceId() == 11 & currentPlayer.getId() == 2) ||
                                (Spaces[i - dir, j + 1].getPieceId() == 22 & currentPlayer.getId() == 1)) &
                                !jumpedOver.Contains(Spaces[i - dir, j + 1].getId()) & !Spaces[i - 2 * dir, j + 2].isOccupied())
                            {
                                jumpedOver.Add(Spaces[i - dir, j + 1].getId());
                                moves.Add(mergeArrays(startmove, new Space[] { Spaces[i - 2 * dir, j + 2] }));
                                added = true;
                            }

                        if (j - 2 > -1)
                            if (Spaces[i - dir, j - 1].isOccupied() & 
                                ((Spaces[i - dir, j - 1].getPieceId() != currentPlayer.getId() & Spaces[i - dir, j - 1].getPieceId() < 10) ||
                                (Spaces[i - dir, j - 1].getPieceId() == 11 & currentPlayer.getId() == 2) ||
                                (Spaces[i - dir, j - 1].getPieceId() == 22 & currentPlayer.getId() == 1)) &
                                !jumpedOver.Contains(Spaces[i - dir, j - 1].getId()) & !Spaces[i - 2 * dir, j - 2].isOccupied())
                            {
                                jumpedOver.Add(Spaces[i - dir, j - 1].getId());
                                moves.Add(mergeArrays(startmove, new Space[] { Spaces[i - 2 * dir, j - 2] }));
                                added = true;
                            }
                    }
                }
                    
                // If it is a King
                else if ((startmove[0].getPieceId() == 11 & currentPlayer.getId() == 1)
                            || startmove[0].getPieceId() == 22 & currentPlayer.getId() == 2)
                {
                    // Left up. k -1, l -1
                    int k = -1;
                    int l = -1;
                    bool jumpedPiece = false;

                    while (i + k > -1) //|| i + k < n)
                    {
                        if (j + l > -1)
                            if (!Spaces[i + k, j + l].isOccupied() & jumpedPiece)
                            {
                                moves.Add(mergeArrays(startmove, new Space[] { Spaces[i + k, j + l] }));
                                added = true;
                                //Console.WriteLine("Maybe here 2");
                            }
                            else if (Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                            {
                                k *= n;
                                l *= n;
                            }
                            else if (Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                ((Spaces[i + k, j + l].getPieceId() != currentPlayer.getId() & Spaces[i + k, j + l].getPieceId() < 10) ||
                                (Spaces[i + k, j + l].getPieceId() == 11 & currentPlayer.getId() == 2) ||
                                (Spaces[i + k, j + l].getPieceId() == 22 & currentPlayer.getId() == 1)) &
                                (Spaces[i, j].getPieceId() != Spaces[i + k, j + l].getPieceId()) &
                                !jumpedOver.Contains(Spaces[i + k, j + l].getId())) // If space is occupied, and jumpedPiece is false, make it true
                            {
                                jumpedOver.Add(Spaces[i + k, j + l].getId());
                                jumpedPiece = true;
                            }
                            else if (Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                ((Spaces[i + k, j + l].getPieceId() == currentPlayer.getId()) ||
                                (Spaces[i + k, j + l].getPieceId() == 11 & currentPlayer.getId() == 1) ||
                                (Spaces[i + k, j + l].getPieceId() == 22 & currentPlayer.getId() == 2) ||
                                (Spaces[i, j].getPieceId() == Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                            {
                                k *= n;
                                l *= n;
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
                        if (j + l < n)
                            if (!Spaces[i + k, j + l].isOccupied() & jumpedPiece)
                            {
                                moves.Add(mergeArrays(startmove, new Space[] { Spaces[i + k, j + l] }));
                                added = true;
                                //Console.WriteLine("Maybe here 2");
                            }
                            else if (Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                            {
                                k *= n;
                                l *= n;
                            }
                            else if (Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                ((Spaces[i + k, j + l].getPieceId() != currentPlayer.getId() & Spaces[i + k, j + l].getPieceId() < 10) ||
                                (Spaces[i + k, j + l].getPieceId() == 11 & currentPlayer.getId() == 2) ||
                                (Spaces[i + k, j + l].getPieceId() == 22 & currentPlayer.getId() == 1)) &
                                (Spaces[i, j].getPieceId() != Spaces[i + k, j + l].getPieceId()) &
                                !jumpedOver.Contains(Spaces[i + k, j + l].getId())) // If space is occupied, and jumpedPiece is false, make it true
                            {
                                jumpedOver.Add(Spaces[i + k, j + l].getId());
                                jumpedPiece = true;
                            }
                            else if (Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                ((Spaces[i + k, j + l].getPieceId() == currentPlayer.getId()) ||
                                (Spaces[i + k, j + l].getPieceId() == 11 & currentPlayer.getId() == 1) ||
                                (Spaces[i + k, j + l].getPieceId() == 22 & currentPlayer.getId() == 2) ||
                                (Spaces[i, j].getPieceId() == Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                            {
                                k *= n;
                                l *= n;
                            }

                        k -= 1;
                        l += 1;
                    }

                    // Left down. k +1, l -1
                    k = 1;
                    l = -1;
                    jumpedPiece = false;

                    while (i + k < n) //(i + k > -1 || i + k < n)
                    {
                        if (j + l > -1)
                            if (!Spaces[i + k, j + l].isOccupied() & jumpedPiece)
                            {
                                moves.Add(mergeArrays(startmove, new Space[] { Spaces[i + k, j + l] }));
                                added = true;
                                //Console.WriteLine("Maybe here 2");
                            }
                            else if (Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                            {
                                k *= n;
                                l *= n;
                            }
                            else if (Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                ((Spaces[i + k, j + l].getPieceId() != currentPlayer.getId() & Spaces[i + k, j + l].getPieceId() < 10) ||
                                (Spaces[i + k, j + l].getPieceId() == 11 & currentPlayer.getId() == 2) ||
                                (Spaces[i + k, j + l].getPieceId() == 22 & currentPlayer.getId() == 1)) &
                                (Spaces[i, j].getPieceId() != Spaces[i + k, j + l].getPieceId()) &
                                !jumpedOver.Contains(Spaces[i + k, j + l].getId())) // If space is occupied, and jumpedPiece is false, make it true
                            {
                                jumpedOver.Add(Spaces[i + k, j + l].getId());
                                jumpedPiece = true;
                            }
                            else if (Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                ((Spaces[i + k, j + l].getPieceId() == currentPlayer.getId()) ||
                                (Spaces[i + k, j + l].getPieceId() == 11 & currentPlayer.getId() == 1) ||
                                (Spaces[i + k, j + l].getPieceId() == 22 & currentPlayer.getId() == 2) ||
                                (Spaces[i, j].getPieceId() == Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                            {
                                k *= n;
                                l *= n;
                            }

                        k += 1;
                        l -= 1;
                    }

                    // Right down. k +1, l +1
                    k = 1;
                    l = 1;
                    jumpedPiece = false;

                    while (i + k < n) //(i + k > -1 || i + k < n)
                    {
                        if (j + l < n)
                            if (!Spaces[i + k, j + l].isOccupied() & jumpedPiece)
                            {
                                moves.Add(mergeArrays(startmove, new Space[] { Spaces[i + k, j + l] }));
                                added = true;
                                //Console.WriteLine("Maybe here 2");
                            }
                            else if (Spaces[i + k, j + l].isOccupied() & jumpedPiece) // Stop by increasing k and l, when already one piece has been jumped
                            {
                                k *= n;
                                l *= n;
                            }
                            else if (Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                ((Spaces[i + k, j + l].getPieceId() != currentPlayer.getId() & Spaces[i + k, j + l].getPieceId() < 10) ||
                                (Spaces[i + k, j + l].getPieceId() == 11 & currentPlayer.getId() == 2) ||
                                (Spaces[i + k, j + l].getPieceId() == 22 & currentPlayer.getId() == 1)) &
                                (Spaces[i, j].getPieceId() != Spaces[i + k, j + l].getPieceId()) &
                                !jumpedOver.Contains(Spaces[i + k, j + l].getId())) // If space is occupied, and jumpedPiece is false, make it true
                            {
                                jumpedOver.Add(Spaces[i + k, j + l].getId());
                                jumpedPiece = true;
                            }
                            else if (Spaces[i + k, j + l].isOccupied() & !jumpedPiece &
                                ((Spaces[i + k, j + l].getPieceId() == currentPlayer.getId()) ||
                                (Spaces[i + k, j + l].getPieceId() == 11 & currentPlayer.getId() == 1) ||
                                (Spaces[i + k, j + l].getPieceId() == 22 & currentPlayer.getId() == 2) ||
                                (Spaces[i, j].getPieceId() == Spaces[i + k, j + l].getPieceId()))) // If space consist friendly piece stop as well
                            {
                                k *= n;
                                l *= n;
                            }

                        k += 1;
                        l += 1;
                    }
                }

                if (!added)
                    moves.Add(startmove);

            }
            return moves;
        }

        public Space[] mergeArrays(Space[] x, Space[] y)
        {
            Space[] result = new Space[x.Length + 1];
            x.CopyTo(result, 0);
            y.CopyTo(result, x.Length);

            return result;
        }

        public void createSpaces()
        {
            for (int i = 0; i < this.n; i++)
            {
                for (int j = 0; j < this.n; j++)
                {
                    this.Spaces[i, j] = new Space(boardCoords2Id(i,j));
                }
            }
        }

        //override
        //public string ToString()
        //{
        //    string str = "";
        //    for (int i = 0; i < this.n; i++)
        //    {
        //        for (int j = 0; j < this.n; j++)
        //        { 
        //            str += string.Format("{0} ", this.Spaces[i,j].getPieceId());
        //        }
        //        str += "\n";
        //    }
        //    str += "\n";
        //
        //    return str;
        //}

        public void printColored()
        {
            for (int i = 0; i < this.n; i++)
            {
                for (int j = 0; j < this.n; j++)
                {
                    if (this.Spaces[i, j].isOccupied())
                    {
                        if (this.Spaces[i, j].getPieceId() == 1)
                        {
                            Console.ResetColor();
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }
                        else if (this.Spaces[i, j].getPieceId() == 2)
                        {
                            Console.ResetColor();
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        else if (this.Spaces[i, j].getPieceId() == 11)
                        {
                            Console.ResetColor();
                            Console.BackgroundColor = ConsoleColor.Yellow;
                        }
                        else if (this.Spaces[i, j].getPieceId() == 22)
                        {
                            Console.ResetColor();
                            Console.BackgroundColor = ConsoleColor.Red;
                        }
                    }
                    else
                    {
                        Console.ResetColor();
                    }


                    if (this.Spaces[i, j].getPieceId() < 10)
                        Console.Write(string.Format("{0} ", this.Spaces[i, j].getPieceId()));
                    else
                        Console.Write(string.Format("{0} ", this.Spaces[i, j].getPieceId()));

                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.ResetColor();
        }


        public void printColoredIndex()
        {
            for (int i = 0; i < this.n; i++)
            {
                for (int j = 0; j < this.n; j++)
                {
                    if (this.Spaces[i, j].isOccupied())
                    {
                        if (this.Spaces[i, j].getPieceId() == 1)
                        {
                            Console.ResetColor();
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }
                        else if (this.Spaces[i, j].getPieceId() == 2)
                        {
                            Console.ResetColor();
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        else if (this.Spaces[i, j].getPieceId() == 11)
                        {
                            Console.ResetColor();
                            Console.BackgroundColor = ConsoleColor.Yellow;
                        }
                        else if (this.Spaces[i, j].getPieceId() == 22)
                        {
                            Console.ResetColor();
                            Console.BackgroundColor = ConsoleColor.Red;
                        }
                    }
                    else
                    {
                        Console.ResetColor();
                    }
                        

                    if (boardCoords2Id(i, j) < 10)
                        Console.Write(string.Format("{0}  ", boardCoords2Id(i, j)));
                    else
                        Console.Write(string.Format("{0} ", boardCoords2Id(i, j)));

                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        public int[] CountPieces()
        {
            int[] count = { 0, 0 };
            for (int i = 0; i < this.n; i++)
            {
                for (int j = 0; j < this.n; j++)
                {
                    if (Spaces[i, j].isOccupied())
                    {
                        if (Spaces[i, j].getPieceId() == 1 || Spaces[i, j].getPieceId() == 11)
                            count[0]++;
                        else count[1]++;
                    } 
                }

            }

            return count;
        }

        public int getWinner()
        {
            int[] cntPieces = this.CountPieces();

            if (cntPieces[0] == 0 || (getPossibleMoves().Count() == 0 & currentPlayer.getId() == 1))
                return 2;
            else if (cntPieces[1] == 0 || (getPossibleMoves().Count() == 0 & currentPlayer.getId() == 2))
                return 1;
            else 
                return 0;
        }

        public Space[] simulateMove(Space[] moves)
        {
            Space[] reset = new Space[moves.Length - 1];
            for (int i = 0; i < moves.Length - 1; i++)
            {
                // Determine submove
                Space[] move = new Space[2];
                Array.Copy(moves, i, move, 0, 2);

                // Get piece on "from" space
                Piece p = move[0].getPieces()[0];

                // Remove piece on "from" space
                move[0].getPieces().Remove(p);

                // Add piece on "to" space
                move[1].getPieces().Add(p);

                // Check is piece got jumped
                // If yes, remove piece
                if (madeJump(move))
                    reset[i] = simulateRemove(move);
                else
                    reset[i] = new Space(99);
            }

            return reset;
        }

        public void UndoMove(Space[] moves, Space[] reset)
        {
            // Get piece on last space
            Piece p = moves[moves.Length - 1].getPieces()[0];

            // Remove piece on "to" space
            moves[moves.Length - 1].getPieces().Remove(p);

            // Add piece on "from" space
            moves[0].getPieces().Add(p);

            // Place back the removed pieces
            int[] coords;
            for (int i = 0; i < reset.Length; i++)
            {
                if (reset[i].getId() != 99)
                {
                    coords = boardId2Coords(reset[i].getId());
                    this.Spaces[coords[0], coords[1]].getPieces().Add(reset[i].getPieces()[0]);
                }
            }
        }

        public void movePiece(Space[] moves)
        { 
            for (int i = 0; i < moves.Length-1; i++)
            {
                // Determine submove
                Space[] move = new Space[2];
                Array.Copy(moves, i, move, 0, 2);

                // Get piece on "from" space
                Piece p = move[0].getPieces()[0];

                // Remove piece on "from" space
                move[0].getPieces().Remove(p);

                // Add piece on "to" space
                move[1].getPieces().Add(p);

                // Check is piece got jumped
                // If yes, remove piece
                if (madeJump(move))
                    removeJumpedPiece(move);
            }
            
            printPiecesInPlay();
            printMove(moves);
        }

        private Space simulateRemove(Space[] move)
        {
            Space reset = new Space(99);

            // Get coords
            int[] coordsfrom = boardId2Coords(move[0].getId());
            int[] coordsto = boardId2Coords(move[1].getId());

            int i = coordsfrom[0];
            int j = coordsfrom[1];

            // Check which direction to check
            int i_dir = coordsfrom[0] < coordsto[0] ? 1 : -1;
            int j_dir = coordsfrom[1] < coordsto[1] ? 1 : -1;

            // Check every space on diagonal between steps if there is a piece
            for (int iter = 1; iter < Math.Abs(coordsfrom[0] - coordsto[0]); iter++)
            {
                if (this.Spaces[i + iter * i_dir, j + iter * j_dir].isOccupied())
                {
                    // Remove piece that got jumped
                    reset = this.Spaces[i + iter * i_dir, j + iter * j_dir].DeepClone();
                    this.Spaces[i + iter * i_dir, j + iter * j_dir].getPieces().RemoveAt(0);
                }
            }

            return reset;
        }

        private void removeJumpedPiece(Space[] move)
        {
            // Get coords
            int[] coordsfrom = boardId2Coords(move[0].getId());
            int[] coordsto = boardId2Coords(move[1].getId());

            int i = coordsfrom[0];
            int j = coordsfrom[1];

            // Check which direction to check
            int i_dir = coordsfrom[0] < coordsto[0] ? 1 : -1;
            int j_dir = coordsfrom[1] < coordsto[1] ? 1 : -1;

            // Check every space on diagonal between steps if there is a piece
            //for (int i = coordsfrom[0] + i_dir; i * i_dir < coordsto[0] * i_dir; i += i_dir)
            //{
            //    for (int j = coordsfrom[1] + j_dir; j * j_dir < coordsto[1] * j_dir; j += j_dir) // Checks whole rows, should only check diagonal (moves as well)
            //    {
            //        if (this.Spaces[i, j].isOccupied())
            //        {
            // Remove piece that got jumped
            //            this.Spaces[i, j].getPieces().RemoveAt(0);
            //        }
            //    }
            //}

            // Check every space on diagonal between steps if there is a piece
            for (int iter = 1; iter < Math.Abs(coordsfrom[0]-coordsto[0]); iter++)
            {
                if (this.Spaces[i+iter*i_dir, j+iter*j_dir].isOccupied())
                {
                    // Remove piece that got jumped
                    this.Spaces[i + iter * i_dir, j + iter * j_dir].getPieces().RemoveAt(0);
                }
            }
        }

        public bool madeJump(Space[] move)
        {
            int[] coordsfrom = boardId2Coords(move[0].getId());
            int[] coordsto = boardId2Coords(move[1].getId());

            // Check if step is bigger than 1 (otherwise no jump)
            if (Math.Abs(coordsfrom[0] - coordsto[0]) > 1)
            {
                // Check which direction to check
                int i_dir = coordsfrom[0] < coordsto[0] ? 1 : -1;
                int j_dir = coordsfrom[1] < coordsto[1] ? 1 : -1;

                // Check every space on diagonal between steps if there is a piece
                for (int i = coordsfrom[0]+i_dir; i*i_dir < coordsto[0]*i_dir; i += i_dir)
                {
                    for (int j = coordsfrom[1]+j_dir; j*j_dir < coordsto[1]*j_dir; j += j_dir)
                    {
                        if (this.Spaces[i, j].isOccupied())
                            return true;
                    }
                }
            }

            return false;
        }

        public int[] boardId2Coords(int Id)
        {
            return new int[] { Id / this.n, Id % this.n };
        }

        public int boardCoords2Id(int i, int j)
        {
            return i * this.n + j;
        }

        public void printMove(Space[] move)
        {
            string strMove = "Player " + currentPlayer.getId() + ": ";
            for (int i = 0; i < move.Length; i++)
            {
                if (i == move.Length - 1)
                    strMove += move[i].getId() + ".\n";
                else
                    strMove += move[i].getId() + " -> ";
            }
            Console.WriteLine(strMove);
        }

        public void printPiecesInPlay()
        {
            int[] numberOfPieces = CountPieces();
            Console.WriteLine(numberOfPieces[0] + " Player 1, " + numberOfPieces[1] + " Player 2.");
        }

        public bool inGame()
        {
            return this.getWinner() == 0 || getPossibleMoves().Count() > 0;
        }
    }
}