using ACS_Reception.Application.CardUseCases.Commands;
using ACS_Reception.Application.CardUseCases.Queries;
using ACS_Reception.Application.RecordUseCases.Commands;
using ACS_Reception.Application.RecordUseCases.Queries;
using ACS_Reception.Domain.Entities;
using ACS_Reception.Domain.Entities.CardRecords;
using ACS_Reception.Persistence.Data;
using ACS_Reception.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ACS_Reception.Tests.DBTests
{
    public class DBTests
    {
        private readonly AppDbContext context;
        private readonly EfUnitOfWork unitOfWork;

        public DBTests()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017/");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseMongoDB(mongoClient, "ACS_Reception_Tests")
                .Options;

            context = new AppDbContext(options);
            unitOfWork = new(context);
        }

        [Fact]
        public async Task TestDB()
        {
            await unitOfWork.DeleteDatabaseAsync();
            await unitOfWork.CreateDatabaseAsync();

            var card1 = new Card("John", "Lazzer", DateTime.Now);
            var card2 = new Card("Mark", "Digher", DateTime.Now);

            await unitOfWork.CardRepository.AddAsync(card1);
            await unitOfWork.CardRepository.AddAsync(card2);
            await unitOfWork.SaveAllAsync();

            Assert.Equal(2, (await unitOfWork.CardRepository.ListAllAsync()).Count);
            Assert.Contains(card1, await unitOfWork.CardRepository.ListAllAsync());

            await unitOfWork.CardRepository.DeleteAsync(card1);
            await unitOfWork.SaveAllAsync();

            Assert.Single((await unitOfWork.CardRepository.ListAllAsync()));
            Assert.DoesNotContain(card1, await unitOfWork.CardRepository.ListAllAsync());
        }

        [Fact]
        public async Task TestDBwithCommands()
        {
            await unitOfWork.DeleteDatabaseAsync();
            await unitOfWork.CreateDatabaseAsync();

            var card1 = new Card("John", "Lazzer", DateTime.Now);
            var card2 = new Card("Mark", "Digher", DateTime.Now);


            var handler1 = new AddCardHandler(unitOfWork);
            var req = new AddCardCommand() { Card = card1 };
            await handler1.Handle(req, default);
            req = new AddCardCommand() { Card = card2 };
            await handler1.Handle(req, default);

            Assert.Equal(2, (await unitOfWork.CardRepository.ListAllAsync()).Count);
            Assert.Contains(card1, await unitOfWork.CardRepository.ListAllAsync());

            var handler2 = new DeleteCardHandler(unitOfWork);
            var req2 = new DeleteCardCommand(card1);
            await handler2.Handle(req2, default);

            Assert.Single((await unitOfWork.CardRepository.ListAllAsync()));
            Assert.DoesNotContain(card1, await unitOfWork.CardRepository.ListAllAsync());
        }

        [Fact]
        public async Task TestDBwithEdit()
        {
            await unitOfWork.DeleteDatabaseAsync();
            await unitOfWork.CreateDatabaseAsync();

            var card1 = new Card("John", "Lazzer", DateTime.Now);
            var card2 = new Card("Mark", "Digher", DateTime.Now);


            var handler1 = new AddCardHandler(unitOfWork);
            var req = new AddCardCommand() { Card = card1 };
            await handler1.Handle(req, default);
            req = new AddCardCommand() { Card = card2 };
            await handler1.Handle(req, default);

            Assert.Equal(2, (await unitOfWork.CardRepository.ListAllAsync()).Count);
            Assert.Contains(card1, await unitOfWork.CardRepository.ListAllAsync());

            var handler2 = new EditCardHandler(unitOfWork);
            card2.FirstName = "Test";
            var req2 = new EditCardCommand() { Card = card2 };
            await handler2.Handle(req2, default);

            var handler3 = new GetCardByIdQueryHandler(unitOfWork);
            var req3 = new GetCardByIdQuery(card2.Id);
            var card = await handler3.Handle(req3, default);

            var handler4 = new GetCardsQueryHandler(unitOfWork);
            var req4 = new GetCardsQuery();
            var cards = await handler4.Handle(req4, default);

            Assert.Equal("Test", card.FirstName);
            Assert.Equal(2, cards.Count());
        }

        [Fact]
        public async Task TestDBwithInteraction()
        {
            await unitOfWork.DeleteDatabaseAsync();
            await unitOfWork.CreateDatabaseAsync();

            var card1 = new Card("John", "Lazzer", DateTime.Now) { Id = ObjectId.GenerateNewId() };
            var card2 = new Card("Mark", "Digher", DateTime.Now) { Id = ObjectId.GenerateNewId() };

            var doc1 = new Doctor("Vella", "Kanner", 101, DoctorType.LabDiagnostic);
            var doc2 = new Doctor("Bella", "Manter", 102, DoctorType.Therapist);

            var rec1 = new Analyzis(card1.Id, DateTime.Now, $"{doc1.LastName} {doc1.FirstName}", AnalizysResults.Satisfactory);
            var rec2 = new Check(card2.Id, DateTime.Now, $"{doc2.LastName} {doc2.FirstName}", doc2.DoctorType, "test2");
            var rec3 = new Check(card1.Id, DateTime.Now, $"{doc2.LastName} {doc2.FirstName}", doc2.DoctorType, "test1");

            var handler1 = new AddCardHandler(unitOfWork);
            var req = new AddCardCommand() { Card = card1 };
            await handler1.Handle(req, default);
            req = new AddCardCommand() { Card = card2 };
            await handler1.Handle(req, default);

            var handler2 = new AddRecordHandler(unitOfWork);
            var req2 = new AddRecordCommand() { CardRecord = rec1 };
            await handler2.Handle(req2, default);
            req2 = new AddRecordCommand() { CardRecord = rec2 };
            await handler2.Handle(req2, default);
            req2 = new AddRecordCommand() { CardRecord = rec3 };
            await handler2.Handle(req2, default);

            Assert.Equal(3, (await unitOfWork.RecordRepository.ListAllAsync()).Count);
            Assert.Contains(rec2, await unitOfWork.RecordRepository.ListAllAsync());

            var handler3 = new GetRecordsByCardIdQueryHandler(unitOfWork);
            var req3 = new GetRecordsByCardIdQuery(card1.Id);

            Assert.Equal(2, (await handler3.Handle(req3, default)).Count());
        }
    }
}
