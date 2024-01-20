using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

public class LogCntrl : NetworkBehaviour
{
    public float textShowTime = 3f;
    private TMP_Text txt;
    private string text = string.Empty;

    private void Start()
    {
        txt = GetComponent<TMP_Text>();
        txt.text = string.Empty;
    }

    [Server]
    public void ShowText(string text)
    {
        ShowTextRpc(text + '\n');
    }

    [ClientRpc]
    public void ShowTextRpc(string text)
    {
        txt.text += text;
        StartCoroutine(nameof(TextTimeout), text);
    }

    private IEnumerator TextTimeout(string text)
    {
        yield return new WaitForSeconds(textShowTime);
        txt.text = txt.text.Replace(text, string.Empty);
    }
}
