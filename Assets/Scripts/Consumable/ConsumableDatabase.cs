using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The consumable database is an asset in the project where designers can drag'n'drop the prefab for the Consumable. This allows explicit
/// definition (you can leave one out of the database to not appear in game) contrary to automatic population of the database like the Character one does.
/// </summary>
[CreateAssetMenu(fileName="Consumables", menuName = "Trash Dash/Consumables Database", order = 100)]
public class ConsumableDatabase : ScriptableObject
{
    //lưu consumbales by array
    public Consumable[] consumbales;

    //Lưu _consumablesDict by Dictionary
    static protected Dictionary<Consumable.ConsumableType, Consumable> _consumablesDict;

    public void Load()
    {
        if (_consumablesDict == null)
        {
            _consumablesDict = new Dictionary<Consumable.ConsumableType, Consumable>();
            for (int i = 0; i < consumbales.Length; ++i)
            {
                /*Debug.Log(consumbales.Length);
                Debug.Log(_consumablesDict);*/
                //Debug.Log(consumbales[i]);
                _consumablesDict.Add(consumbales[i].GetConsumableType(), consumbales[i]);
            }
        }
    }

    static public Consumable GetConsumbale(Consumable.ConsumableType type)
    {
        Consumable c;
        //Debug.Log(_consumablesDict.TryGetValue(type, out c) ? c : null);
        return _consumablesDict.TryGetValue (type, out c) ? c : null;
    }
}
