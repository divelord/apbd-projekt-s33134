namespace SystemUznawaniaPrzychodow.Services;

public class CurrencyService
{
    private readonly HttpClient _httpClient;

    public CurrencyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal?> GetExchangeRateAsync(string currency)
    {
        if (string.IsNullOrEmpty(currency))
        {
            return null;
        }

        if (currency.ToUpper() == "PLN")
        {
            return 1.0m;
        }

        decimal? rate = await GetApiNbpRateAsync("a", currency);

        if (rate == null)
        {
            rate = await GetApiNbpRateAsync("b", currency);
        }

        if (rate.HasValue && rate.Value != 0)
        {
            return rate.Value;
        }

        return null;
    }

    public async Task<decimal?> GetApiNbpRateAsync(string table, string currency)
    {
        string link = $"http://api.nbp.pl/api/exchangerates/rates/{table}/{currency.ToLower()}/?format=json";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<NbpResponse>(link);

            return response?.Rates?.FirstOrDefault()?.Mid;
        }
        catch (Exception)
        {
            return null;
        }
    }
}

public class Rate
{
    public decimal Mid { get; set; }
}

public class NbpResponse
{
    public Rate[] Rates { get; set; } = [];
}