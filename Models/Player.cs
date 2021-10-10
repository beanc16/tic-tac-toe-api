using System;
using MongoDB.Bson.Serialization.Attributes;
using TicTacToeApi.Models.Enums;
using RandomNameGeneratorLibrary;

namespace TicTacToeApi.Models
{
    public class Player
    {
        [BsonElement("_id")]
        public string Id { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("mark")]
        public string Mark { get; set; }
        [BsonElement("isCpu")]
        public bool IsCpu { get; set; }


        // --> Static
        private static PersonNameGenerator _personGenerator { get; }

        static Player()
        {
            _personGenerator = new PersonNameGenerator();
        }
        // <-- Static


        // 4 parameters

        public Player(string id, string name, string mark, bool isCpu)
        {
            if (id == null)
            {
                id = GenerateRandomId();
            }
            if (name == null)
            {
                name = GenerateRandomName();
            }
            
            Id = id;
            Name = name;
            Mark = mark.ToString();
            IsCpu = isCpu;
        }

        // 3 parameters

        public Player(string id, string name, string mark)
            : this(id, name, mark, false)
        {
        }

        public Player(string id, string name, bool isCpu)
        {
            Id = id;
            Name = name;
            Mark = BoardMark.X;
            IsCpu = isCpu;
        }

        public static Player FromIdMarkAndIsCpu(string id, string mark, bool isCpu)
        {
            return new Player(id, GenerateRandomName(), mark, isCpu);
        }

        public static Player FromNameMarkAndIsCpu(string name, string mark, bool isCpu)
        {
            return new Player(GenerateRandomId(), name, mark, isCpu);
        }

        // 2 parameters

        public Player(string id, string name)
        {
            Id = id;
            Name = name;
            Mark = BoardMark.X;
            IsCpu = false;
        }

        public static Player FromIdAndMark(string id, string mark)
        {
            return new Player(id, GenerateRandomName(), mark);
        }

        public static Player FromIdAndIsCpu(string id, bool isCpu)
        {
            return new Player(id, GenerateRandomName(), isCpu);
        }

        public static Player FromNameAndMark(string name, string mark)
        {
            return new Player(GenerateRandomId(), name, mark);
        }

        public Player(string name, bool isCpu)
            : this(GenerateRandomId(), name, isCpu)
        {
        }

        public static Player FromMarkAndIsCpu(string mark, bool isCpu)
        {
            return new Player(GenerateRandomId(), GenerateRandomName(), mark, isCpu);
        }

        // 1 parameter

        public static Player FromId(string id)
        {
            return new Player(id, GenerateRandomName());
        }

        public Player(string name)
        {
            Id = GenerateRandomId();
            Name = name; 
            Mark = BoardMark.X;
            IsCpu = false;
        }

        public static Player FromBoardMark(string mark)
        {
            return new Player(GenerateRandomId(), GenerateRandomName(), mark);
        }

        public Player(bool isCpu)
        {
            Id = GenerateRandomId();
            Name = GenerateRandomName(); 
            Mark = BoardMark.X;
            IsCpu = isCpu;
        }

        // 0 parameters

        public Player()
        {
            Id = GenerateRandomId();
            Name = GenerateRandomName();
            Mark = BoardMark.X;
            IsCpu = false;
        }



        private static string GenerateRandomId()
        {
            return Guid.NewGuid().ToString();
        }

        private static string GenerateRandomName()
        {
            return _personGenerator.GenerateRandomFirstAndLastName();
        }
    }
}