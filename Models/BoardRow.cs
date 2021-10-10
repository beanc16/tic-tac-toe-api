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
        // public string[] Columns { get; private set; }
        public List<string> Columns { get; private set; }

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
            Predicate<string> isValidBoardValue = 
                (col) => (col == null || 
                          col.ToLower() == "null" || 
                          col.ToUpper() == "X" || 
                          col.ToUpper() == "O");
            
            /* Is not null, 
             * a list with exactly 3 values, 
             * and a list with all elements that are either:
             * - "X"
             * - "O"
             * - "null"
             * - null
             */
            if (columns != null && columns.Count == 3 && 
                columns.TrueForAll(isValidBoardValue))
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