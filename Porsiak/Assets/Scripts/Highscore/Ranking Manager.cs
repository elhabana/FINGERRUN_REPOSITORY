using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class RankingManager : MonoBehaviour
{
    [SerializeField] private TMP_Text[] rankingTexts; // Arrastra aquí tus 10 textos de la UI

    private const string KeyRanking = "TopScores";

    void Start()
    {
        // Carga los puntos guardados en cuanto arranca el objeto
        UpdateUI(GetScores());
    }

    public void SaveScore(int newScore)
    {
        Debug.Log("Puntos recibidos: " + newScore);
        List<int> scores = GetScores();
        scores.Add(newScore);

        var top10 = scores.OrderByDescending(s => s).Take(10).ToList();

        // Guardamos
        PlayerPrefs.SetString(KeyRanking, string.Join(",", top10));
        PlayerPrefs.Save();

        Debug.Log("Ranking actualizado con " + top10.Count + " puntuaciones.");
        UpdateUI(top10);
    }

    public List<int> GetScores()
    {
        string data = PlayerPrefs.GetString(KeyRanking, "");
        if (string.IsNullOrEmpty(data)) return new List<int>();
        return data.Split(',').Select(int.Parse).ToList();
    }

    public void UpdateUI(List<int> scores)
    {
        for (int i = 0; i < rankingTexts.Length; i++)
        {
            if (i < scores.Count)
                rankingTexts[i].text = $"{i + 1}. {scores[i]:00000} PTS";
            else
                rankingTexts[i].text = $"{i + 1}. -----";
        }
    }
}