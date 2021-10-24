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



        public void Clear()
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                for (int j = 0; j < Rows[i].Columns.Count; j++)
                {
                    Rows[i].Columns[j] = BoardMark.EMPTY;
                }
            }
        }

        public void ClearAllButWinningMarks()
        {
            if (HasTopRowMatch())
            {
                ClearRows(1, 2);
            }

            else if (HasMiddleRowMatch())
            {
                ClearRows(0, 2);
            }

            else if (HasBottomRowMatch())
            {
                ClearRows(0, 1);
            }

            else if (HasLeftColumnMatch())
            {
                ClearColumns(1, 2);
            }

            else if (HasMiddleColumnMatch())
            {
                ClearColumns(0, 2);
            }

            else if (HasRightColumnMatch())
            {
                ClearColumns(0, 1);
            }

            else if (HasLeftDiagonalMatch())
            {
                ClearAllButLeftDiagonal();
            }

            else if (HasRightDiagonalMatch())
            {
                ClearAllButRightDiagonal();
            }
        }

        private void ClearRows(int r1Index, int r2Index)
        {
            Rows[r1Index].Clear();
            Rows[r2Index].Clear();
        }

        private void ClearColumns(int c1Index, int c2Index)
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                Rows[i].Columns[c1Index] = BoardMark.EMPTY;
                Rows[i].Columns[c2Index] = BoardMark.EMPTY;
            }
        }

        private void ClearAllButLeftDiagonal()
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                for (int j = 0; j < Rows[i].Columns.Count; j++)
                {
                    // Skip left diagonal marks
                    if ((i == 0 && j == 0) ||   // Top left
                        (i == 1 && j == 1) ||   // Middle
                        (i == 2 && j == 2))     // Bottom right
                    {
                        continue;
                    }

                    Rows[i].Columns[j] = BoardMark.EMPTY;
                }
            }
        }

        private void ClearAllButRightDiagonal()
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                for (int j = 0; j < Rows[i].Columns.Count; j++)
                {
                    // Skip right diagonal marks
                    if ((i == 0 && j == 2) ||   // Top right
                        (i == 1 && j == 1) ||   // Middle
                        (i == 2 && j == 0))     // Bottom left
                    {
                        continue;
                    }

                    Rows[i].Columns[j] = BoardMark.EMPTY;
                }
            }
        }



        public string GetWinningMark()
        {
            // Center is part of winning three
            if (HasMiddleRowMatch() || HasMiddleColumnMatch() ||
                HasLeftDiagonalMatch() || HasRightDiagonalMatch())
            {
                return GetWinningMarkHelper(1, 1);
            }

            // Top left is part of winning three
            else if (HasTopRowMatch() || HasLeftColumnMatch())
            {
                return GetWinningMarkHelper(0, 0);
            }

            // Bottom right is part of winning three
            else if (HasBottomRowMatch() || HasRightColumnMatch())
            {
                return GetWinningMarkHelper(2, 2);
            }

            return BoardMark.EMPTY;
        }

        private string GetWinningMarkHelper(int rowIndex, int columnIndex)
        {
            if (Rows[rowIndex].Columns[columnIndex] == BoardMark.X)
            {
                return BoardMark.X;
            }

            else if (Rows[rowIndex].Columns[columnIndex] == BoardMark.O)
            {
                return BoardMark.O;
            }
            
            return BoardMark.EMPTY;
        }

        

        public bool HasNoMoreMoves()
        {
            foreach (BoardRow row in Rows)
            {
                // There's still at least one empty space
                if (!row.HasNoMoreMoves())
                {
                    return false;
                }
            }

            // There's no more empty spaces
            return true;
        }
        
        public bool HasMatch()
        {
            // Has a match in a row, column, or diagonal
            return (HasRowMatch() ||
                    HasColumnMatch() ||
                    HasDiagonalMatch());
        }

        public bool HasRowMatch()
        {
            foreach (BoardRow row in Rows)
            {
                if (row.HasMatch())
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasTopRowMatch()
        {
            return (Rows[0].HasMatch());
        }

        public bool HasMiddleRowMatch()
        {
            return (Rows[1].HasMatch());
        }

        public bool HasBottomRowMatch()
        {
            return (Rows[2].HasMatch());
        }

        public bool HasColumnMatch()
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                // All spaces in the current column match and are not empty
                if (Rows[0].Columns[i] != BoardMark.EMPTY &&
                    Rows[0].Columns[i] == Rows[1].Columns[i] &&
                    Rows[1].Columns[i] == Rows[2].Columns[i])
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasLeftColumnMatch()
        {
            // All spaces in left column match and are not empty
            return (Rows[0].Columns[0] != BoardMark.EMPTY &&
                    Rows[0].Columns[0] == Rows[1].Columns[0] &&
                    Rows[1].Columns[0] == Rows[2].Columns[0]);
        }

        public bool HasMiddleColumnMatch()
        {
            // All spaces in middle column match and are not empty
            return (Rows[0].Columns[1] != BoardMark.EMPTY &&
                    Rows[0].Columns[1] == Rows[1].Columns[1] &&
                    Rows[1].Columns[1] == Rows[2].Columns[1]);
        }

        public bool HasRightColumnMatch()
        {
            // All spaces in right column match and are not empty
            return (Rows[0].Columns[2] != BoardMark.EMPTY &&
                    Rows[0].Columns[2] == Rows[1].Columns[2] &&
                    Rows[1].Columns[2] == Rows[2].Columns[2]);
        }

        public bool HasDiagonalMatch()
        {
            return (HasLeftDiagonalMatch() || 
                    HasRightDiagonalMatch());
        }

        public bool HasLeftDiagonalMatch()
        {
            // All spaces in left diagonal match and are not empty
            return (Rows[0].Columns[0] != BoardMark.EMPTY &&
                    Rows[0].Columns[0] == Rows[1].Columns[1] &&     // Top left & middle
                    Rows[1].Columns[1] == Rows[2].Columns[2]);      // Middle & bottom right
        }

        public bool HasRightDiagonalMatch()
        {
            // All spaces in right diagonal match and are not empty
            return (Rows[0].Columns[2] != BoardMark.EMPTY &&
                    Rows[0].Columns[2] == Rows[1].Columns[1] &&     // Top right & middle
                    Rows[1].Columns[1] == Rows[2].Columns[0]);      // Middle & bottom left
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