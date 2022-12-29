using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// The DebugConsole script takes string inputs and writes them to the UI slate above the
/// board for debugging purposes.
/// </summary>
public class DebugConsole : MonoBehaviour
{
    #region Fields
    private TextMeshPro _consoleText;
    #endregion

    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        GameObject consoleObject = GameObject.FindGameObjectWithTag("ConsoleText");
        _consoleText = consoleObject.GetComponent<TextMeshPro>();
        Clear();
    }

    public void Write(string input)
    {
        var sb = new StringBuilder();

        sb.Append(input);
        sb.Append(System.Environment.NewLine);
        sb.Append(_consoleText.text);

        _consoleText.text = sb.ToString();
    }

    public void Clear()
    {
        _consoleText.text = "";
    }
    #endregion
}
