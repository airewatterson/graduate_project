using System;
using System.Collections;
using System.Collections.Generic;
using General;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class Dialog : SingletonMonoBehavior<Dialog>
{
    [FormerlySerializedAs("_textLabel")]
    [Header("UI Plugin")]
    [SerializeField] private Text textLabel;
    [FormerlySerializedAs("_imageLabel")] [SerializeField] private Image imageLabel;
    
    [FormerlySerializedAs("_textAsset")]
    [Header("Plain Text Asset")]
    [SerializeField] private TextAsset textAsset;
    [SerializeField] private  int index;
    private bool _textFinished;
    private bool _cancelTyping;

    [FormerlySerializedAs("_av1")]
    [Header("Avatar")] 
    [SerializeField] private Sprite av1;
    [FormerlySerializedAs("_av2")] [SerializeField] private Sprite av2;

    [Header("Dialog Block")]
    [SerializeField] private GameObject block;
    
    
    [Header("Clock")]
    [SerializeField] private float textClock;

    [Header("Dialog Dictionary")] private  readonly List<string> _text = new List<string>();

    public override void Awake()
    {
        GetText(textAsset);
    }

    private void OnEnable()
    {
        _textFinished = true;
        StartCoroutine(nameof(SetText));
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && index == _text.Count)
        {
            index = 0;
            block.SetActive(false);
            return;
        }
        
        /*if (Input.GetKeyDown(KeyCode.R) && _textFinished)
        {
            StartCoroutine(nameof(SetText));
        }*/
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (_textFinished && !_cancelTyping)
            {
                StartCoroutine(nameof(SetText));
            }
            else if (!_textFinished && !_cancelTyping)
            {
                _cancelTyping = true;
            }
        }
        
        //_textLabel.text = _text[index];
        //index++;
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
    private IEnumerator SetText()
    {
        _textFinished = false;
        textLabel.text = "";

        switch (_text[index])
        {
            case "PLAYER":
                imageLabel.sprite = av1;
                index++; 
                break;
            case "NPC":
                imageLabel.sprite = av2;
                index++; 
                break;
        }

        /*for (var i = 0; i < _text[index].Length; i++)
        {
            _textLabel.text += _text[index][i];
            yield return new WaitForSeconds(textClock);
        }*/
        var letter = 0;
        while (!_cancelTyping && letter < _text[index].Length - 1)
        {
            textLabel.text += _text[index][letter];
            letter++;
            yield return new WaitForSeconds(textClock);
        }
        textLabel.text = _text[index];
        _cancelTyping = false;
        _textFinished = true;
        index++;
    }
}


