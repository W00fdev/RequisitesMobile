using UnityEngine;

using TMPro;
using Assets.Scripts.Core;

[RequireComponent(typeof(TMP_Text))]
public class HintText : MonoBehaviour
{
    private TMP_Text _text;

    public string Text 
    {
        get => _text.text;
        set => _text.text = value;
    }

    private void Awake()
    {
        if (_text == null)
            _text = GetComponent<TMP_Text>();
        _text.text = "";
    }

    public void UpdateHint(ParserResponse response)
    {
        Text = response.MakeHint(response.Response);
    }

    public void UpdateHint(string text)
    {
        Text = text;
    }
}
