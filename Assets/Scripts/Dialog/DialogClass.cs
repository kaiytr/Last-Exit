using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[Serializable]
public class DialogClass
{
    public string Name;

    [TextArea]
    public string Dialogs;
    public float coolTime;

}
[Serializable]
public class Dialog
{
    public List<DialogClass> dialogs;
}
