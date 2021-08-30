using System;
using System.Collections.Generic;
using System.Linq;
using PieceC;

namespace SpaceC
{
    [Serializable]
    internal class Space
    {
        private List<PieceC.Piece> Pieces = new List<Piece>();
        public int id;

        public Space(int id)
        {
            this.id = id;
        }

        public List<Piece> getPieces() { return Pieces; }

        public int getId() { return this.id;  }

        public int getPieceId()
        {
            int num = 0;

            if (this.Pieces.Count() > 0) num = this.Pieces[0].getId();

            return num;
        }

        public void addPiece(Piece piece)
        {
            this.Pieces.Add(piece);
        }

        public bool isOccupied()
        {
            return this.Pieces.Count > 0;
        }
    }
}
