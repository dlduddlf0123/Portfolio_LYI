using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.InventoryEngine;

public class PickTester : MonoBehaviour
{
    public ItemPicker item;

    private void Start()
    {
       MoreMountains.Tools.MMGameEvent.Trigger("Load");
    }

    public void Pick()
    {
        item.Quantity = 1;
        item.Pick();
    }

    public void Save()
    {
        MoreMountains.Tools.MMGameEvent.Trigger("Save");
    }

}
