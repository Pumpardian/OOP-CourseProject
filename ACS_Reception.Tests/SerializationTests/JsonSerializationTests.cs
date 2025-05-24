using ACS_Reception.Domain.Entities;
using ACS_Reception.Domain.Entities.CardRecords;
using ACS_Reception.SerializerLib;

namespace ACS_Reception.Tests.SerializationTests
{
    public class JsonSerializationTests
    {
        [Fact]
        public async Task Json_Serialize_and_Deserialize_correct_with_structs()
        {
            string filename = "test.json";

            List<Card> cards =
            [
                new Card("John", "Salazar", DateTime.Now),
                new Card("Marie", "Vemnant", DateTime.Now),
                new Card("Anne", "Lock", DateTime.Now),
            ];

            Serializer serializer = new();
            await serializer.SerializeJson(cards, filename);

            List<Card> cards2 = await serializer.DeserializeJson<List<Card>>(filename);

            Assert.All(cards, (c) => cards2.Contains(c));
        }

        [Fact]
        public async Task Json_Serialize_and_Deserialize_correct_with_enums()
        {
            string filename = "test2.json";

            List<Doctor> doctors =
            [
                new Doctor("John", "Salazar", 105, DoctorType.Therapist),
                new Doctor("Marie", "Vemnant", 106, DoctorType.LabDiagnostic),
                new Doctor("Anne", "Lock", 107, DoctorType.Psychologist),
            ];

            Serializer serializer = new();
            await serializer.SerializeJson(doctors, filename);

            List<Doctor> doctors2 = await serializer.DeserializeJson<List<Doctor>>(filename);

            Assert.All(doctors, (c) => doctors2.Contains(c));
        }

        [Fact]
        public async Task Json_Serialize_and_Deserialize_correct_with_solo_classes()
        {
            string filename = "test3.json";
            string filename2 = "test3_2.json";

            var card = new Card("John", "Salazar", DateTime.Now);
            var doc = new Doctor("Marie", "Vemnant", 106, DoctorType.LabDiagnostic);

            Serializer serializer = new();
            await serializer.SerializeJson(card, filename);
            await serializer.SerializeJson(doc, filename2);

            Card card2 = await serializer.DeserializeJson<Card>(filename);
            Doctor doc2 = await serializer.DeserializeJson<Doctor>(filename2);

            Assert.Equal(card, card2);
            Assert.Equal(doc, doc2);
        }
    }
}
