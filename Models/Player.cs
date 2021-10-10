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
        
        public Player()
            : this(null, null, BoardMark.X, false, false)
        {
        }


        // --> Static
        private static PersonNameGenerator _personGenerator { get; }

        static Player()
        {
            _personGenerator = new PersonNameGenerator();
        }
        // <-- Static



        private static string GenerateRandomId()
        {
            return Guid.NewGuid().ToString();
        }

        private static string GenerateRandomName()
        {
            return _personGenerator.GenerateRandomFirstAndLastName();
        }



        public override string ToString()
        {
            string template = "Player: {{\n" +
                "\t\"ID\": {0}\n" +
                "\t\"Name\": {1}\n" +
                "\t\"Mark\": {2}\n" +
                "\t\"IsCpu\": {3}\n" +
                "\t\"IsTurn\": {4}\n" +
            "}}";

            return string.Format(template, Id, Name, Mark, IsCpu, IsTurn);
        }

        public string ToStringTabbed()
        {
            string template = "Player: {{\n" +
                "\t\t\t\"ID\": {0}\n" +
                "\t\t\t\"Name\": {1}\n" +
                "\t\t\t\"Mark\": {2}\n" +
                "\t\t\t\"IsCpu\": {3}\n" +
                "\t\t\t\"IsTurn\": {4}\n" +
            "\t\t}}";

            return string.Format(template, Id, Name, Mark, IsCpu, IsTurn);
        }
    }
}