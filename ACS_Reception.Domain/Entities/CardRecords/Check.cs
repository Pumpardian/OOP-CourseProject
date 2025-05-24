using MongoDB.Bson;

namespace ACS_Reception.Domain.Entities.CardRecords
{
    public class Check : CardRecord
    {
        public DoctorType DoctorType { get; set; } = default;
        public string CheckResponse { get; set; } = "response";
        public Check() : base(default, DateTime.MinValue, "doctor") { }

        public Check(ObjectId cardId, DateTime date, string doctor, DoctorType doctorType, string checkResponse) 
            : base(cardId, date, doctor)
        {
            DoctorType = doctorType;
            CheckResponse = checkResponse;
        }

        public override string GetRecordName => "Check";

        public override string GetInfo => $"{Enum.GetName(DoctorType)}\n{CheckResponse}";

        public override bool Equals(object? obj)
        {
            if (obj is not Check)
            {
                return false;
            }

            if (!base.Equals(obj) ||
                DoctorType != (obj as Check)!.DoctorType ||
                CheckResponse != (obj as Check)!.CheckResponse)
            {
                return false;
            }

            return true;
        }
    }
}
