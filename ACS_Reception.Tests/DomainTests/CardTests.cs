using ACS_Reception.Domain.Entities;
using ACS_Reception.Domain.Entities.CardRecords;
using System.Collections.Generic;

namespace ACS_Reception.Tests.DomainTests
{
    public class CardTests
    {
        [Fact]
        public void Constructor_Card_ValidData()
        {
            string firstName = "Name";
            string lastName = "Surname";
            DateTime dateTime = DateTime.Now;
            var card = new Card(firstName, lastName, dateTime);

            Assert.Equal(firstName, card.FirstName);
            Assert.Equal(lastName, card.LastName);
            Assert.Equal(dateTime, card.BirthDate);
        }

        [Fact]
        public void Card_SettersAvailable()
        {
            string firstName = "Name";
            string lastName = "Surname";
            DateTime dateTime = DateTime.MinValue;
            var card = new Card(firstName, lastName, dateTime);

            string firstName2 = "Name2";
            string lastName2 = "Surname2";
            DateTime dateTime2 = DateTime.MaxValue;

            card.FirstName = firstName2;
            card.LastName = lastName2;
            card.BirthDate = dateTime2;

            Assert.Equal(firstName2, card.FirstName);
            Assert.Equal(lastName2, card.LastName);
            Assert.Equal(dateTime2, card.BirthDate);
        }

        [Fact]
        public void Card_RecordsReadOnly()
        {
            string firstName = "Name";
            string lastName = "Surname";
            DateTime dateTime = DateTime.Now;
            var card = new Card(firstName, lastName, dateTime);

            Assert.True(card.CardRecords is IReadOnlyList<CardRecord>);
        }
    }
}
