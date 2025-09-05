using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OutburstQuestion
{
    public string title;
    public List<string> items;
}

[System.Serializable]
public class OutburstDatabase
{
    public List<OutburstQuestion> cards;
}
