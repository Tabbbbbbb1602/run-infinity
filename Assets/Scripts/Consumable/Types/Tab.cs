using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tab : Consumable
{
    public override string GetConsumableName()
    {
        return "tab";
    }

    public override ConsumableType GetConsumableType()
    {
        return ConsumableType.TAB;
    }

    public override int GetPrice()
    {
        return 750;
    }

    public override int GetPremiumCost()
    {
        return 0;
    }

    public override IEnumerator Started(CharacterInputController c)
    {
        yield return base.Started(c);

        m_SinceStart = 0;

        c.trackManager.modifyMultiply += MultiplyModify;
    }

    public override void Ended(CharacterInputController c)
    {
        base.Ended(c);

        c.trackManager.modifyMultiply -= MultiplyModify;
    }

    protected int MultiplyModify(int multi)
    {
        return multi * 4;
    }
}
