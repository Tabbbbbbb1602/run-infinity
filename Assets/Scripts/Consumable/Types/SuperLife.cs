using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperLife : Consumable
{
    protected readonly Vector3 k_HalfExtentsBox = new Vector3(20.0f, 1.0f, 1.0f);
    protected const int k_LayerMask = 1 << 8;
    public override string GetConsumableName()
    {
        return "Super life";
    }

    public override ConsumableType GetConsumableType()
    {
        return ConsumableType.SUPERLIFE;
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

    protected Collider[] returnColls = new Collider[20];
    public override void Tick(CharacterInputController c)
    {
        base.Tick(c);

        /*int nb = Physics.OverlapBoxNonAlloc(c.characterCollider.transform.position, k_HalfExtentsBox, returnColls, c.characterCollider.transform.rotation, k_LayerMask);

        for (int i = 0; i < nb; ++i)
        {
            Coin returnCoin = returnColls[i].GetComponent<Coin>();

            if (returnCoin != null && !returnCoin.isPremium && !c.characterCollider.magnetCoins.Contains(returnCoin.gameObject))
            {
                returnColls[i].transform.SetParent(c.transform);
                c.characterCollider.magnetCoins.Add(returnColls[i].gameObject);
            }
        }*/
    }
}
