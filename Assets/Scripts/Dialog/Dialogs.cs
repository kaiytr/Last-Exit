using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.IO;
public class Dialogs : MonoBehaviour
{
    [SerializeField] TextAsset textAsset;
    public List<DialogClass> dialogsList;
    [SerializeField] Text dialogTxt;

    private Dialog dialog;

    void Awake()
    {
        dialog = JsonUtility.FromJson<Dialog>(textAsset.text);
        dialogsList = dialog.dialogs;
    }
}
