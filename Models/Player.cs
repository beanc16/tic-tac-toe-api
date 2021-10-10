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
        [BsonElement("isTurn")]
        public bool IsTurn { get; set; }


        // --> Static
        private static PersonNameGenerator _personGenerator { get; }

        static Player()
        {
            _personGenerator = new PersonNameGenerator();
        }
        // <-- Static


        public Player(string id, string name, string mark, bool isCpu, bool isTurn)
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
            IsTurn = isTurn;
        }

        public static Player FromBoardMarkAndIsTurn(string mark, bool isTurn)
        {
            return new Player(GenerateRandomId(), GenerateRandomName(), mark, false, isTurn);
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