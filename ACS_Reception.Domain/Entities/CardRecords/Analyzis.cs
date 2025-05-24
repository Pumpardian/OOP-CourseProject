using MongoDB.Bson;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;

namespace ACS_Reception.Domain.Entities.CardRecords
{
    public enum AnalizysResults
    {
        Satisfactory,
        Norm,
        Acceptable,
        Unsatisfactory
    }

    public class Analyzis : CardRecord
    {
        public AnalizysResults AnalizysResults { get; set; } = default;

        public Analyzis() : base(default, DateTime.MinValue, "doctor") {}

        public Analyzis(ObjectId cardId, DateTime date, string doctor, AnalizysResults analizysResults)
            : base(cardId, date, doctor)
        {
            AnalizysResults = analizysResults;
        }

        public override string GetRecordName
        {
            get => "Analyzis";
        }

        public override string GetInfo
        {
            get
            {
                switch (AnalizysResults)
                {
                    case AnalizysResults.Satisfactory:
                        return "Analyzis results are satisfactory.";
                    case AnalizysResults.Norm:
                        return "Analyzis results are in norm.";
                    case AnalizysResults.Acceptable:
                        return "Analyzis results are within acceptable range.";
                    case AnalizysResults.Unsatisfactory:
                        return "Analyzis results are unsatisfactory.";
                    default:
                        return "Wrong results";
                }
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Analyzis)
            {
                return false;
            }

            if (!base.Equals(obj) ||
                AnalizysResults != (obj as Analyzis)!.AnalizysResults)
            {
                return false;
            }

            return true;
        }
    }
}
