using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using System.Text.Json;

class Program
{
    private static readonly string botToken = "7823798459:AAGixDtXPxpZQaYmDZM3CqIO4p6Oi2NE2To";
    private static readonly long[] channelIds = { -1002393692883, -1002311980353 };
    private static readonly string weatherApiKey = "c7a39cc2518c499dd0157c1d151644f8";

    static async Task Main()
    {
        var botClient = new TelegramBotClient(botToken);
        Console.WriteLine("Successful...");

        while (true)
        {
            try
            {
                string weather = await GetWeatherAsync("Tashkent");
                foreach (var channelId in channelIds)
                {
                    await botClient.SendTextMessageAsync(channelId, $"{weather}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}");
            }

            await Task.Delay(TimeSpan.FromMinutes(2));
        }
    }
    static async Task<string> GetWeatherAsync(string city)
    {
        using var client = new HttpClient();
        string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={weatherApiKey}&units=metric&lang=en";
        var response = await client.GetStringAsync(url);

        using JsonDocument doc = JsonDocument.Parse(response);
        var temp = doc.RootElement.GetProperty("main").GetProperty("temp").GetDouble();
        var condition = doc.RootElement.GetProperty("weather")[0].GetProperty("description").GetString();

        Dictionary<string, string> descriptions = new()
        {
            {"clear sky", "ochiq osmon"},
            {"few clouds", "kam bulutli"},
            {"scattered clouds", "tarqoq bulutlar"},
            {"broken clouds", "bulutli"},
            {"shower rain", "kuchli yomg‘ir"},
            {"rain", "yomg‘ir"},
            {"thunderstorm", "chaqmoq"},
            {"snow", "qor"},
            {"mist", "tuman"},
            {"light intensity shower rain", "yengil yomg'ir" }
        };
        if(descriptions.ContainsKey(condition))
        {
            condition = descriptions[condition];
        }

        return $"{city} shahridagi ob-havo: {temp}°C, {condition}";
    }
}