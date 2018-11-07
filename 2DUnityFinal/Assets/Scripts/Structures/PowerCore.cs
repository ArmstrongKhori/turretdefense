using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ah, yes... The Power Core.
/// This is the center of the game. Structures try to protect it, while creeps try to destroy it.
/// It does very little other than be an objective.
/// </summary>
public class PowerCore : Structure {

    protected override void Awake()
    {
        base.Awake();
        //
        ChangeType(StructureTypes.Core);
        //
        AssignSprite("Core");


        indestructible = false;
        cc.radius = 0.5f;
    }


    public override void OnSpawn(Actor by = null)
    {
        base.OnSpawn(by);
        //
        lite.intensity *= 3;
        lite.range *= 3;
        lite.color = Color.cyan;
    }

    public override void Die()
    {
        base.Die();
        //
        Director d = Director.Instance();
        d.DoDefeat();
    }

    /// <summary>
    /// The core has a special counter-attack that triggers whenever it is attacked.
    /// This minimizes the deadliness of a "swarm" of creeps finding the core.
    /// However, this also means ONE creep is just about as deadly as ten,
    /// since the number of counter-attacks are relative to the amount of attackers.
    /// </summary>
    /// <param name="c"></param>
    public override void OnAttacked(Creep c)
    {
        base.OnAttacked(c);
        //
        Director d = Director.Instance();
        EnergyBurst eb = d.CreateDamageArea<EnergyBurst>(Helper.DEADZONE);
        eb.pos = pos;
        eb.OnSpawn();
        // *** This does not get an object pool because there can be ANY number of creeps hitting a core-- And they won't all necessarily be hitting it at once.

        Sounder.Instance().PlaySound("core_damage");
    }


}
