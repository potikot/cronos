using Cronos.Features.Leetcode.Model;

namespace Cronos.Features.Leetcode;

public interface ILeetcodeService
{
    Task<LeetcodeChallengeData> GetDailyChallengeAsync();
}