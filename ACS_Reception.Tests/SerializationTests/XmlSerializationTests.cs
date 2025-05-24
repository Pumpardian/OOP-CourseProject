using ACS_Reception.Domain.Entities;
using ACS_Reception.SerializerLib;

namespace ACS_Reception.Tests.SerializationTests
{
    public class XmlSerializationTests
    {
        [Fact]
        public async Task Xml_Serialize_and_Deserialize_correct_with_structs()
        {
            string filename = "test.xml";

            List<Card> cards =
            [
                new Card("John", "Salazar", DateTime.Now),
                new Card("Marie", "Vemnant", DateTime.Now),
                new Card("Anne", "Lock", DateTime.Now),
            ];

            Serializer serializer = new();
            await serializer.SerializeXml(cards, filename);

            List<Card> cards2 = await serializer.DeserializeXml<List<Card>>(filename);

            Assert.NotEmpty(cards2);
            foreach (var card in cards)
            {
                Assert.Contains(card, cards2);
            }
        }

        [Fact]
        public async Task Xml_Serialize_and_Deserialize_correct_with_enums()
        {
            string filename = "test2.xml";

            List<Doctor> doctors =
            [
                new Doctor("John", "Salazar", 105, DoctorType.Therapist),
                new Doctor("Marie", "Vemnant", 106, DoctorType.LabDiagnostic),
                new Doctor("Anne", "Lock", 107, DoctorType.Psychologist),
            ];

            Serializer serializer = new();
            await serializer.SerializeXml(doctors, filename);

            List<Doctor> doctors2 = await serializer.DeserializeXml<List<Doctor>>(filename);

            Assert.NotEmpty(doctors2);
            foreach (var doc in doctors)
            {
                Assert.Contains(doc, doctors2);
            }
        }

        [Fact]
        public async Task Xml_Serialize_and_Deserialize_correct_with_solo_classes()
        {
            string filename = "test3.xml";
            string filename2 = "test3_2.xml";

            var card = new Card("John", "Salazar", DateTime.Now);
            var doc = new Doctor("Marie", "Vemnant", 106, DoctorType.LabDiagnostic);

            Serializer serializer = new();
            await serializer.SerializeXml(card, filename);
            await serializer.SerializeXml(doc, filename2);

            Card card2 = await serializer.DeserializeXml<Card>(filename);
            Doctor doc2 = await serializer.DeserializeXml<Doctor>(filename2);

            Assert.Equal(card, card2);
            Assert.Equal(doc, doc2);
        }

        /*[Fact]
        public async Task Xml_Serialize_and_Deserialize_correct_with_polymorphism()
        {
            string filename = "test4.xml";

            List<CardRecord> recs =
            [
                new Analyzis(ObjectId.GenerateNewId(), DateTime.Now, "doc1", AnalizysResults.Norm),
                new Check(ObjectId.GenerateNewId(), DateTime.Now, "doc2", DoctorType.Psychologist, "response2"),
                new Prescription(ObjectId.GenerateNewId(), DateTime.Now, "doc3", "testicine", 0.25),
            ];

            Serializer serializer = new();
            await serializer.SerializeXml(recs, filename);

            List<CardRecord> recs2 = await serializer.DeserializeXml<List<CardRecord>>(filename);

            Assert.All(recs, (c) => recs2.Contains(c));
            Assert.True(recs2.ElementAt(0) is Analyzis);
            Assert.True(recs2.ElementAt(1) is Check);
            Assert.True(recs2.ElementAt(2) is Prescription);
        }*/
    }
}
