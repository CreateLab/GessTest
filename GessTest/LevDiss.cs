namespace GessTest;

public class LevClass
{
    public static IEnumerable<string> GetPrediction(string word, IEnumerable<string> collection, bool isDem = false)
    {
        var result = new List<(int, string)>();
        foreach (var distWord in collection)
        {
            var levDistance = 0;
            levDistance = isDem ? DLevDistance(word, distWord) : LevDistance(word, distWord);

            if (result.Count < 5)
            {
                result.Add((levDistance, distWord));
            }
            else
            {
                var findPosMin = FindPosMin(result);
                if (result[findPosMin].Item1 > levDistance)
                {
                    result[findPosMin] = new ValueTuple<int, string>(levDistance, distWord);
                }
            }
        }

        return result.OrderBy(t => t.Item1).Select(t => t.Item2);
    }

    private static int FindPosMin(IReadOnlyList<(int, string)> list)
    {
        var value = -1;
        var pos = -1;
        for (var i = 0; i < list.Count; i++)
        {
            if (value == -1)
            {
                value = list[i].Item1;
                pos = 0;
            }
            else
            {
                if (list[i].Item1 >= value) continue;
                value = list[i].Item1;
                pos = i;
            }
        }

        return pos;
    }

    private static int LevDistance(string firstWord, string secondWord)
    {
        int diff;
        var m = new int[firstWord.Length + 1, secondWord.Length + 1];

        for (var i = 0; i < firstWord.Length + 1; i++)
        {
            m[i, 0] = i;
        }

        for (var j = 0; j < secondWord.Length + 1; j++)
        {
            m[0, j] = j;
        }

        for (var i = 1; i <= firstWord.Length; i++)
        {
            for (var j = 1; j <= secondWord.Length; j++)
            {
                diff = firstWord[i - 1] == secondWord[j - 1] ? 0 : 1;

                m[i, j] = Math.Min(Math.Min(m[i - 1, j] + 1, m[i, j - 1] + 1), m[i - 1, j - 1] + diff);
            }
        }

        return m[firstWord.Length, secondWord.Length];
    }

    public static int DLevDistance(string firstWord, string secondWord)
    {
        int diff;
        var m = new int[firstWord.Length + 1, secondWord.Length + 1];

        for (var i = 0; i < firstWord.Length + 1; i++)
        {
            m[i, 0] = i;
        }

        for (var j = 0; j < secondWord.Length + 1; j++)
        {
            m[0, j] = j;
        }

        for (var i = 1; i <= firstWord.Length; i++)
        {
            for (var j = 1; j <= secondWord.Length; j++)
            {
                diff = firstWord[i - 1] == secondWord[j - 1] ? 0 : 1;

                m[i, j] = Math.Min(Math.Min(m[i - 1, j] + 1, m[i, j - 1] + 1), m[i - 1, j - 1] + diff);
                if (i > 1 && j > 1 && firstWord[i - 1] == secondWord[j - 2] &&
                    firstWord[i - 2] == secondWord[j - 1])
                {
                    m[i, j] = Math.Min(m[i, j], m[i - 2, j - 2] + diff);
                }
            }
        }

        return m[firstWord.Length, secondWord.Length];
    }
}