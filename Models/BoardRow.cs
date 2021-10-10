using MongoDB.Bson.Serialization.Attributes;
using TicTacToeApi.Models.Enums;

namespace TicTacToeApi.Models
{
    public class BoardRow
    {
        [BsonElement("columns")]
        public string[] Columns { get; private set; }

        public BoardRow()
        {
            Columns = new string[] {
                BoardMark.EMPTY,
                BoardMark.EMPTY,
                BoardMark.EMPTY,
            };
        }



        public void MarkColumn(BoardMark mark, int columnNum)
        {
            Columns[columnNum] = mark.ToString();
        }



        public override string ToString()
        {
            string template = "BoardRow: {{\n" +
                "\t\"Columns\": {0}\n" +
            "}}";

            string columnStr = "[\n";
            foreach (string column in Columns)
            {
                string col = column;
                if (column == null)
                {
                    col = "null,";
                }
                columnStr += "\t" + col + "\n";
            }
            columnStr += "\t]";

            return string.Format(template, columnStr);
        }

        public string ToStringTabbed()
        {
            string template = "\tBoardRow: {{\n" +
                "\t\t\t\"Columns\": {0}\n" +
            "\t\t}}";

            string columnStr = "[\n";
            foreach (string column in Columns)
            {
                string col = column;
                if (column == null)
                {
                    col = "null,";
                }
                columnStr += "\t\t\t" + col + "\n";
            }
            columnStr += "\t\t\t]";

            return string.Format(template, columnStr);
        }

        public string ToStringDoubleTabbed()
        {
            string template = "\tBoardRow: {{\n" +
                "\t\t\t\t\t\"Columns\": {0}\n" +
            "\t\t\t\t}}";

            string columnStr = "[\n";
            foreach (string column in Columns)
            {
                string col = column;
                if (column == null)
                {
                    col = "null,";
                }
                columnStr += "\t\t\t\t\t" + col + "\n";
            }
            columnStr += "\t\t\t\t\t]";

            return string.Format(template, columnStr);
        }
    }
}