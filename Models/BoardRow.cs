using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;
using TicTacToeApi.Models.Enums;

namespace TicTacToeApi.Models
{
    public class BoardRow
    {
        [BsonElement("columns")]
        public List<string> Columns { get; private set; }

        public int NumOfMarks
        {
            get
            {
                int numOfMarks = 0;

                foreach (string column in Columns)
                {
                    if (column != BoardMark.EMPTY)
                    {
                        numOfMarks++;
                    }
                }

                return numOfMarks;
            }
        }

        public BoardRow()
        {
            Columns = new List<string>(new string[] {
                BoardMark.EMPTY,
                BoardMark.EMPTY,
                BoardMark.EMPTY,
            });
        }

        [JsonConstructor]   // Called on JsonConvert.DeserializeObject
        public BoardRow(List<string> columns)
        {
            if (this.IsValidColumnList(columns))
            {
                Columns = columns;
            }

            else
            {
                Columns = new List<string>(new string[] {
                    BoardMark.EMPTY,
                    BoardMark.EMPTY,
                    BoardMark.EMPTY,
                });
            }
        }



        public bool IsValidColumnList(List<string> columns)
        {
            Predicate<string> isValidBoardValue = 
                (col) => (col == null || 
                          col.ToLower() == "null" || 
                          col.ToUpper() == "X" || 
                          col.ToUpper() == "O");
            
            /* Is not null, 
             * a list with exactly 3 values, 
             * and a list with all elements that are either:
             * - null
             * - "null"
             * - "X"
             * - "O"
             */
            return (columns != null && columns.Count == 3 && 
                    columns.TrueForAll(isValidBoardValue));
        }



        public void MarkColumn(BoardMark mark, int columnNum)
        {
            Columns[columnNum] = mark.ToString();
        }

        public void Clear()
        {
            for (int i = 0; i < Columns.Count; i++)
            {
                Columns[i] = BoardMark.EMPTY;
            }
        }



        public bool HasNoMoreMoves()
        {
            foreach (string column in Columns)
            {
                // There's still an empty space
                if (column == BoardMark.EMPTY)
                {
                    return false;
                }
            }

            // There's no more empty spaces
            return true;
        }
        
        public bool HasMatch()
        {
            // All columns match and are not empty
            return (Columns[0] != BoardMark.EMPTY &&
                    Columns[0] == Columns[1] &&
                    Columns[1] == Columns[2]);
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
                if (column == null || column == "null")
                {
                    col = "null";
                }
                columnStr += "\t\t" + col + ",\n";
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
                columnStr += "\t\t\t\t" + col + "\n";
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
                columnStr += "\t\t\t\t\t\t" + col + "\n";
            }
            columnStr += "\t\t\t\t\t]";

            return string.Format(template, columnStr);
        }
    }
}