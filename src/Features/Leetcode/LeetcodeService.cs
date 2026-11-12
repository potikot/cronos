using System.Text.Json;
using Cronos.Features.Leetcode.Model;
using Cronos.Infrastructure.Http;
using Cronos.Infrastructure.Http.Model;
using Cronos.Infrastructure.Logger;
using Cronos.Utilities;

namespace Cronos.Features.Leetcode;

public class LeetcodeService(
    IHttpService _httpService,
    ILoggerService _logger) : ILeetcodeService
{
    private const string Url = "https://leetcode.com/graphql";
    private const string Query = """
                                     query dailyChallenge {
                                         activeDailyCodingChallengeQuestion {
                                             link
                                             question {
                                                 title
                                                 difficulty
                                             }
                                         }
                                     }
                                 """;
    
    public async Task<LeetcodeChallengeData> GetDailyChallengeAsync()
    {
        RequestResult<string> response = await _httpService.PostStringAsync(Url, new { query = Query });
        if (!response.IsSuccess())
            return default;

        JsonDocument jsonDoc = JsonDocument.Parse(response.Result);

        if (!response.IsSuccess()
            || !jsonDoc.RootElement.TryGetProperty("data", out JsonElement data)
            || !data.TryGetProperty("activeDailyCodingChallengeQuestion", out JsonElement activeDailyCodingChallengeQuestion)
            || !activeDailyCodingChallengeQuestion.TryGetProperty("link", out JsonElement path))
            return default;

        JsonElement? question = activeDailyCodingChallengeQuestion
            .GetPropertyOrNull("question");
        
        string title = question?
            .GetPropertyOrNull("title")?
            .GetString() ?? "unknown";

        string difficulty = question?
            .GetPropertyOrNull("difficulty")?
            .GetString();
        
        return new LeetcodeChallengeData
        {
            Title = title,
            Url = $"https://leetcode.com{path}",
            Difficulty = difficulty switch
            {
                "Easy" => LeetcodeChallengeDifficulty.Easy,
                "Medium" => LeetcodeChallengeDifficulty.Medium,
                "Hard" => LeetcodeChallengeDifficulty.Hard,
                _ => LeetcodeChallengeDifficulty.None,
            }
        };
    }
}