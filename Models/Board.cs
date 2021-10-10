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



        public override string ToString()
        {
            string template = "Board: {{\n" +
                "\t\"Rows\": {0}\n" +
            "}}";

            string rowStr = "[\n";
            foreach (BoardRow row in Rows)
            {
                rowStr += "\t" + row.ToStringTabbed() + "\n";
            }
            rowStr += "\t]";

            return string.Format(template, rowStr);
        }

        public string ToStringTabbed()
        {
            string template = "\t\tBoard: {{\n" +
                "\t\t\t\"Rows\": {0}\n" +
            "\t\t}}";

            string rowStr = "[\n";
            foreach (BoardRow row in Rows)
            {
                rowStr += "\t\t\t" + row.ToStringDoubleTabbed() + "\n";
            }
            rowStr += "\t\t\t]";

            return string.Format(template, rowStr);
        }
    }
}