using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class TextClickSystem : MonoBehaviour
{
    public static TextClickSystem Instance { get; private set; }

    [System.Serializable]
    public class TextData
    {
        public int GroupID;
        public int UIID;
        public int LineNumber;
        public string Character;
        public string Text;
        public int Tachie;
    }

    // UI要素
    public TextMeshProUGUI textTMP;
    public TextMeshProUGUI textChara;
    public GameObject image_text;
    public GameObject image_serihu;
    public GameObject image;
    public GameObject[] image_tachie;

    private int selectedGroupID;
    private int currentLineIndex = 0;
    private bool isDisplayingText = false;
    private int i;

    // CSVデータを保持する辞書
    private Dictionary<int, List<TextData>> textDataDictionary = new Dictionary<int, List<TextData>>();

    private void Awake()
    {
        // シングルトンパターンの実装
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 既にインスタンスが存在する場合、重複しないように破棄する
        }
    }

    private void Start()
    {
        image_text.SetActive(false);
        image_serihu.SetActive(false);
        textTMP.gameObject.SetActive(false);
        textChara.gameObject.SetActive(false);
        image.SetActive(false);
        for (i = 0; i < image_tachie.Length; i++)
        {
            image_tachie[i].SetActive(false);
        }

        // ゲーム開始時にCSVを読み込む
        LoadTextData();
    }

    // ゲーム開始時にCSVファイルを一度だけ読み込む
    private void LoadTextData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "text_data.csv");
        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] fields = line.Split(',');
            if (fields.Length < 4)
            {
                Debug.LogError($"無効な行の形式: {line}");
                continue;
            }

            if (int.TryParse(fields[0].Trim(), out int groupID) &&
                int.TryParse(fields[1].Trim(), out int uiID) &&
                int.TryParse(fields[2].Trim(), out int lineNumber)&&
                int.TryParse(fields[5].Trim(), out int tachie))
            {
                TextData data = new TextData
                {
                    GroupID = groupID,
                    UIID = uiID,
                    LineNumber = lineNumber,
                    Character = fields[3].Trim('\"'),
                    Text = fields[4].Trim('\"'),
                    Tachie = tachie
                };

                // GroupIDごとにデータを格納
                if (!textDataDictionary.ContainsKey(groupID))
                {
                    textDataDictionary[groupID] = new List<TextData>();
                }

                textDataDictionary[groupID].Add(data);
            }
            else
            {
                Debug.LogError($"行の整数を解析できませんでした: {line}");
            }
        }

        // 各GroupIDのリストをLineNumberでソート
        foreach (var group in textDataDictionary)
        {
            group.Value.Sort((a, b) => a.LineNumber.CompareTo(b.LineNumber));
        }
    }

    public void OnImageClicked()
    {
        Debug.Log("imageがクリックされました");
        if (currentLineIndex < textDataDictionary[selectedGroupID].Count - 1)
        {
            currentLineIndex++;
            ShowText();
        }
        else
        {
            HideTextUI();
        }
    }

    // GroupIDが渡されたときに、すでに読み込んだデータから選択されたグループのデータを取得
    public void OnObjectClicked(int groupID)
    {
        selectedGroupID = groupID;
        Debug.Log("GroupIDをTextClickSystemに受け渡しました！");

        // 最初のテキストを表示
        currentLineIndex = 0;
        ShowText();
    }

    private void ShowText()
    {
        if (currentLineIndex < textDataDictionary[selectedGroupID].Count)
        {
            for (i = 0; i < image_tachie.Length; i++)
            {
                image_tachie[i].SetActive(false);
            }
            TextData currentText = textDataDictionary[selectedGroupID][currentLineIndex];
            image_serihu.SetActive(currentText.UIID == 1);
            image_text.SetActive(currentText.UIID == 2);
            image.SetActive(true);
            image_tachie[currentText.Tachie].SetActive(true);
            textTMP.text = currentText.Text;
            textChara.text = currentText.Character;
            textTMP.gameObject.SetActive(true);
            textChara.gameObject.SetActive(true);
            isDisplayingText = true;
        }
        else
        {
            HideTextUI();
        }
    }

    private void HideTextUI()
    {
        image_serihu.SetActive(false);
        image_text.SetActive(false);
        image.SetActive(false);
        textTMP.gameObject.SetActive(false);
        textChara.gameObject.SetActive(false);
        for (i = 0; i < image_tachie.Length; i++)
        {
            image_tachie[i].SetActive(false);
        }
        isDisplayingText = false;
        currentLineIndex = 0;
    }
}