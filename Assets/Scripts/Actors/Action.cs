using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Action
{
    public static void MoveOrHit(Actor actor, Vector3 direction)
    {
        Vector3 targetPosition = actor.transform.position + direction;
        Actor target = GameManager.Get.GetActorAtLocation(targetPosition);

        if (target == null)
        {
            Move(actor, direction);
        }
        else
        {
            Hit(actor, target);
        }
    }

    public static void Move(Actor actor, Vector3 direction)
    {
        actor.Move(direction);
        actor.UpdateFieldOfView();
    }

    public static void Hit(Actor actor, Actor target)
    {
        int damage = actor.Power - target.Defense;
        if (damage > 0)
        {
            target.DoDamage(damage);
            UIManager.Instance.AddMessage($"{actor.name} hits {target.name} for {damage} damage!", actor.GetComponent<Player>() ? Color.white : Color.red);
        }
        else
        {
            UIManager.Instance.AddMessage($"{actor.name} hits {target.name} but does no damage.", actor.GetComponent<Player>() ? Color.white : Color.red);
        }
    }
}
