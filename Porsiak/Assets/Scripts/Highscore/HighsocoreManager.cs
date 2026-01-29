using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HighscoreManager : MonoBehaviour
{
    private const string PrefKey = "Highscores";
    public int maxEntries = 10;

    public void AddScore(int newScore)
    {
        List<int> scores = GetScores();
        scores.Add(newScore);

        // Ordenamos de mayor a menor y nos quedamos con los mejores 10
        var topScores = scores.OrderByDescending(s => s).Take(maxEntries).ToList();

        // Guardamos convirtiendo la lista a un string (puedes usar JSON o simplemente separando por comas)
        string scoreString = string.Join(",", topScores);
        PlayerPrefs.SetString(PrefKey, scoreString);
        PlayerPrefs.Save();
    }

    public List<int> GetScores()
    {
        string savedScores = PlayerPrefs.GetString(PrefKey, "");
        if (string.IsNullOrEmpty(savedScores)) return new List<int>();

        return savedScores.Split(',').Select(int.Parse).ToList();
    }
}
