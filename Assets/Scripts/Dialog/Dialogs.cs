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
        dialogsList.AddRange(dialog.dialogs);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ShowNextDialog();
        }
    }

    void ShowNextDialog()
    {
        if (dialogsList != null && dialogsList.Count > 0)
        {
            DialogClass currentDialog = dialogsList[0];
            dialogTxt.text = currentDialog.Dialog;
            dialogsList.RemoveAt(0);
        }
        else
        {
            dialogTxt.text = "대화가 종료되었습니다.";
        }
    }
}
