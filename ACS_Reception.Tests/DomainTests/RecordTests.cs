using ACS_Reception.Domain.Entities;
using ACS_Reception.Domain.Entities.CardRecords;
using MongoDB.Bson;

namespace ACS_Reception.Tests.DomainTests
{
    public class RecordTests
    {
        [Fact]
        public void Constructor_Check_Available()
        {
            var r1 = new Check(ObjectId.GenerateNewId(), DateTime.Now, "doc1", DoctorType.Therapist, "test1");

            Assert.True(r1 is CardRecord);
        }

        [Fact]
        public void Constructor_Analyzis_Available()
        {
            var r2 = new Analyzis(ObjectId.GenerateNewId(), DateTime.Now, "doc2", AnalizysResults.Acceptable);

            Assert.True(r2 is CardRecord);
        }

        [Fact]
        public void Constructor_SickList_Available()
        {
            var r3 = new SickList(ObjectId.GenerateNewId(), DateTime.MinValue, "doc3");

            Assert.True(r3 is CardRecord);
        }

        [Fact]
        public void Constructor_Collection_Works()
        {
            List <CardRecord> list = [];
            var r1 = new Check(ObjectId.GenerateNewId(), DateTime.Now, "doc1", DoctorType.Therapist, "test1");
            var r2 = new Analyzis(ObjectId.GenerateNewId(), DateTime.Now, "doc2", AnalizysResults.Acceptable);
            var r3 = new SickList(ObjectId.GenerateNewId(), DateTime.MinValue, "doc3");

            list.Add(r1);
            list.Add(r2);
            list.Add(r3);

            Assert.True(list[0] is Check);
            Assert.True(list[1] is Analyzis);
            Assert.True(list[2] is SickList);
        }

        [Fact]
        public void Card_SettersAvailable()
        {
            var r1 = new Check(ObjectId.GenerateNewId(), DateTime.Now, "doc1", DoctorType.Therapist, "test1");

            r1.Date = DateTime.MaxValue;
            r1.Doctor = "doc2";
            r1.DoctorType = DoctorType.Psychologist;
            r1.CheckResponse = "test2";

            Assert.Equal(DateTime.MaxValue, r1.Date);
            Assert.Equal("doc2", r1.Doctor);
            Assert.Equal(DoctorType.Psychologist, r1.DoctorType);
            Assert.Equal("test2", r1.CheckResponse);
        }
    }
}
