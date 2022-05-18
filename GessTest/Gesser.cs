using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Net;

namespace GessTest;

public class Gesser
{
    public async Task<IEnumerable<(string, object)>> Gess(string word, bool state)
    {
        if (IsSimilar(word, out var pair))
        {
            return new[] { (pair.Value.Item1, pair.Value.Item2) };
        }
        else
        {
            var closestDistances = await GetDistances(word);
            var suggestions = closestDistances.Take(5);
            var valueTuples = suggestions as (int, string)[] ?? suggestions.ToArray();
            if (!valueTuples.Any()) return await CheckDifferentLangs(word);
            var lastSuggestion = valueTuples.Last();
            return lastSuggestion.Item1 < 4
                ? valueTuples.Select(x => (x.Item2, DataMoq.Datas[x.Item2]))
                : await CheckDifferentLangs(word);
        }
    }

    private async Task<IEnumerable<(string, object)>> CheckDifferentLangs(string word)
    {
        var result = Translates(word);
        var tasks = result.Select(GetDistances);
        var resultsCollection = await Task.WhenAll(tasks);
        return resultsCollection.SelectMany(x => x).OrderBy(x => x.Item1).Take(5)
            .Select(x => (x.Item2, DataMoq.Datas[x.Item2]));
    }

    private IEnumerable<string> Translates(string word)
    {
        //TODO: change realisation for special data
        return Enumerable.Range(0, 9).Select(x => word);
    }

    private async Task<IEnumerable<(int, string)>> GetDistances(string word)
    {
        var sortedData = await GetSortedData(word);
        var bag = new ConcurrentBag<(int, string)>();

        Parallel.ForEach(sortedData, data =>
        {
            var distance = GetDistance(word, data);
            bag.Add((distance, data));
        });

        return bag.OrderBy(x => x.Item1);
    }

    private int GetDistance(string word, string data)
    {
        return LevClass.DLevDistance(word, data);
    }

    private Task<IEnumerable<string>> GetSortedData(string word)
    {
        return Task.Run(() => GetSortedDictionary(word));
    }

    private IEnumerable<string> GetSortedDictionary(string word)
    {
        if (word.Length > 5)
        {
            var trigram = word[..3];
            return DataMoq.SetDatas.Where(x =>
                x.StartsWith(trigram) && x.Length > word.Length - 2 && x.Length < word.Length + 2);
        }
        else
        {
            return DataMoq.SetDatas.Where(x =>
                x.Length > word.Length - 2 && x.Length < word.Length + 2);
        }
    }

    private bool IsSimilar(string word, out (string, object)? o)
    {
        var result = DataMoq.Datas.TryGetValue(word, out var data);
        if (result)
        {
            o = (word, data);
        }
        else
        {
            o = null;
        }

        return result;
    }
}