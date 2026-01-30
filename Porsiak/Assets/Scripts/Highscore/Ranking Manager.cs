using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class RankingManager : MonoBehaviour
{
    [SerializeField] private TMP_Text[] rankingTexts;
    [SerializeField] private TMP_InputField nameInputField; // Arrastra el InputField del Main Menu aquí

    [System.Serializable]
    public class RankingEntry
    {
        public string playerName;
        public int score;
    }

    [System.Serializable]
    private class RankingDataWrapper
    {
        public List<RankingEntry> entries = new List<RankingEntry>();
    }

    private const string KeyRanking = "TopScoresData";

    void Start()
    {
        UpdateUI();
    }

    public void SaveScore(int newScore)
    {
        // Si el nombre está vacío, ponemos "Vaca Pro" o "Anónimo"
        string nameToSave = "Anonimo";
        if (nameInputField != null && !string.IsNullOrEmpty(nameInputField.text))
        {
            nameToSave = nameInputField.text;
        }

        RankingDataWrapper data = LoadData();
        data.entries.Add(new RankingEntry { playerName = nameToSave, score = newScore });

        // Ordenar y tomar top 10
        data.entries = data.entries.OrderByDescending(r => r.score).Take(10).ToList();

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(KeyRanking, json);
        PlayerPrefs.Save();

        UpdateUI();
    }

    private RankingDataWrapper LoadData()
    {
        string json = PlayerPrefs.GetString(KeyRanking, "");
        if (string.IsNullOrEmpty(json)) return new RankingDataWrapper();
        return JsonUtility.FromJson<RankingDataWrapper>(json);
    }

    // Ahora no recibe argumentos para evitar el error CS1501
    public void UpdateUI()
    {
        RankingDataWrapper data = LoadData();
        for (int i = 0; i < rankingTexts.Length; i++)
        {
            if (i < data.entries.Count)
            {
                rankingTexts[i].text = $"{i + 1}. {data.entries[i].playerName} - {data.entries[i].score:00000}";
            }
            else
            {
                rankingTexts[i].text = $"{i + 1}. ---";
            }
        }
    }
}