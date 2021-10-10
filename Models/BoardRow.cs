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
    }
}