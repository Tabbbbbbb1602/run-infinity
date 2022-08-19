using UnityEngine;
using System;
using System.Collections;

public class Star : Consumable
{
    public override string GetConsumableName()
    {
        return "Star";
    }

    public override ConsumableType GetConsumableType()
    {
        return ConsumableType.STAR;
    }

    public override int GetPrice()
    {
        return 1500;
    }

	public override int GetPremiumCost()
	{
		return 5;
	}

	public override void Tick(CharacterInputController c)
    {
        base.Tick(c);

        c.characterCollider.SetInvincibleExplicit(true);
    }

    public override IEnumerator Started(CharacterInputController c)
    {
        yield return base.Started(c);
        c.characterCollider.SetInvincible(duration);
    }

    public override void Ended(CharacterInputController c)
    {
        base.Ended(c);
        c.characterCollider.SetInvincibleExplicit(false);
    }
}
