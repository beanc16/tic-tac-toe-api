using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;
using TicTacToeApi.Models.Enums;

namespace TicTacToeApi.Models
{
    public class Board
    {
        [BsonElement("rows")]
        public List<BoardRow> Rows { get; private set; }

        public Board()
        {
            Rows = new List<BoardRow>(new BoardRow[] {
                new BoardRow(),
                new BoardRow(),
                new BoardRow(),
            });
        }

        public Board(Board board)
        {
            Rows = board.Rows;
        }

        [JsonConstructor]   // Called on JsonConvert.DeserializeObject
        public Board(List<BoardRow> rows)
        {
            if (this.IsValidRowList(rows))
            {
                Rows = rows;
            }

            else
            {
                Rows = new List<BoardRow>(new BoardRow[] {
                    new BoardRow(),
                    new BoardRow(),
                    new BoardRow(),
                });
            }
        }

        public bool IsValidRowList(List<BoardRow> rows)
        {
            Predicate<BoardRow> isValidBoardRow = 
                (row) => (row != null && 
                          row.Columns != null && 
                          row.IsValidColumnList(row.Columns));
            
            /* Is not null, 
             * a list with exactly 3 values, 
             * and a list with all elements that all:
             * - are not null
             * - have non-null columns
             * - have a valid column list
             */
            return (rows != null && rows.Count == 3 && 
                    rows.TrueForAll(isValidBoardRow));
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