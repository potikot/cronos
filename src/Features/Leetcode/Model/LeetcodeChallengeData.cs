namespace Cronos.Features.Leetcode.Model;

public struct LeetcodeChallengeData
{
    public string Title;
    public string Url;
    public LeetcodeChallengeDifficulty Difficulty;
    
    public override string ToString()
    {
        return $"{Title} ({Difficulty}) | URL: {Url}";
    }
}