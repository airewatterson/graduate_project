using System.Collections.Generic;
using General;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class Dialog : SingletonMonoBehavior<Dialog>
{
    [Header("UI Plugin")]
    [SerializeField] private Text _textLabel;
    [SerializeField] private Image _imageLabel;
    
    [FormerlySerializedAs("_textAsset")]
    [Header("Plain Text Asset")]
    [SerializeField] private TextAsset textAsset;
    [SerializeField] private int index;

    private readonly List<string> _text = new List<string>();
    private void Start()
    {
        GetText(textAsset);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && index == _text.Count)
        {
            gameObject.SetActive(false);
            index = 0;
            return;
        }
        
        if (!Input.GetKeyDown(KeyCode.R)) return;
        _textLabel.text = _text[index];
        index++;
    }

    private void GetText(TextAsset file)
    {
        _text.Clear();
        index = 0;

        var lineData = file.text.Split('\n');

        foreach (var line in lineData)
        {
            _text.Add(line);
        }
    }
}
