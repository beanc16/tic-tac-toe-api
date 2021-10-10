using MongoDB.Bson.Serialization.Attributes;
using TicTacToeApi.Models.Enums;

namespace TicTacToeApi.Models
{
    public class Board
    {
        [BsonElement("rows")]
        public BoardRow[] Rows { get; private set; }

        public Board()
        {
            Rows = new BoardRow[] {
                new BoardRow(),
                new BoardRow(),
                new BoardRow(),
            };
        }

        public Board(Board board)
        {
            Rows = board.Rows;
        }



        public void MarkColumn(BoardMark mark, int rowNum, int columnNum)
        {
            Rows[rowNum].MarkColumn(mark, columnNum);
        }

        public Board Clone()
        {
            return new Board(this);
        }
    }
}