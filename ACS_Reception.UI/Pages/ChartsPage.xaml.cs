using ACS_Reception.Application.CardUseCases.Queries;
using ACS_Reception.Application.DoctorUseCases.Queries;
using ACS_Reception.Application.RecordUseCases.Queries;
using Microcharts;
using SkiaSharp;

namespace ACS_Reception.UI.Pages;

public partial class ChartsPage : ContentPage
{
	public ChartsPage(IMediator mediator)
	{
		InitializeComponent();
        this.mediator = mediator;
        var task = Task.Run(() => GetLists());
        Task.WaitAll(task);
        ToChartEntries();
    }

    private readonly IMediator mediator;

    private List<Doctor> Doctors { get; set; } = [];
    private List<Card> Cards { get; set; } = [];
    private List<CardRecord> Records { get; set; } = [];

    public List<ChartEntry> DoctorEntries = [];
    public List<ChartEntry> CardEntries = [];
    public List<ChartEntry> RecordEntries = [];

    private void ToChartEntries()
    {
        Dictionary<string, int> docData = [];
        Dictionary<string, int> cardData = [];
        Dictionary<string, int> recData = [];

        foreach (var doc in Doctors)
        {
            if (!docData.ContainsKey(doc.DoctorType.ToString()))
            {
                docData.Add(doc.DoctorType.ToString(), 0);
            }
            docData[doc.DoctorType.ToString()]++;
        }

        foreach (var card in Cards)
        {
            string data = (card.BirthDate.Year / (int)10).ToString();
            data += 'X';

            if (!cardData.ContainsKey(data))
            {
                cardData.Add(data, 0);
            }

            cardData[data]++;
        }

        foreach (var rec in Records)
        {
            if (!recData.ContainsKey(rec.GetRecordName))
            {
                recData.Add(rec.GetRecordName, 0);
            }

            recData[rec.GetRecordName]++;
        }

        float h = 0;
        SKColor color = SKColor.FromHsv(h, 85, 90);

        foreach (var data in docData)
        {
            DoctorEntries.Add(new ChartEntry(data.Value) { Label = data.Key, Color = color, ValueLabel = data.Value.ToString() });
            h += 200;
            h = h > 360 ? h - 360 : h;
            color = SKColor.FromHsv(h, 85, 90);
        }

        foreach (var data in cardData)
        {
            CardEntries.Add(new ChartEntry(data.Value) { Label = data.Key, Color = color, ValueLabel = data.Value.ToString() });
            h += 200;
            h = h > 360 ? h - 360 : h;
            color = SKColor.FromHsv(h, 85, 90);
        }

        foreach (var data in recData)
        {
            RecordEntries.Add(new ChartEntry(data.Value) { Label = data.Key, Color = color, ValueLabel = data.Value.ToString() });
            h += 200;
            h = h > 360 ? h - 360 : h;
            color = SKColor.FromHsv(h, 85, 90);
        }

        CardChart.Chart = new BarChart() { Entries = CardEntries };
        CardChartDonut.Chart = new DonutChart() { Entries = CardEntries };
        DocChart.Chart = new BarChart() { Entries = DoctorEntries };
        DocChartDonut.Chart = new DonutChart() { Entries = DoctorEntries };
        RecChart.Chart = new BarChart() { Entries = RecordEntries };
        RecChartDonut.Chart = new DonutChart() { Entries = RecordEntries };
    }

    public async Task GetLists()
    {
        Doctors = [.. await mediator.Send(new GetDoctorsQuery())];
        Cards = [.. await mediator.Send(new GetCardsQuery())];
        Records = [.. await mediator.Send(new GetRecordsQuery())];
    }
}