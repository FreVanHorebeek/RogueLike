using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    static public void MoveOrHit(Actor actor, Vector2 direction)
    {
        Vector3 direction3D = new Vector3(direction.x, direction.y, 0); // Convert Vector2 to Vector3
        if (MapManager.Get.IsWalkable(actor.transform.position + direction3D))
        {
            Move(actor, direction);
        }
        else
        {
            Hit(actor, GameManager.Get.GetActorAtLocation(actor.transform.position + direction3D));
        }
    }

    static private void Move(Actor actor, Vector2 direction)
    {
        actor.Move(direction);
        actor.UpdateFieldOfView();
    }

    static public void Hit(Actor actor, Actor target)
    {
        int damage = actor.Power - target.Defense;
        string message = $"{actor.name} hits {target.name}";

        if (damage > 0)
        {
            target.DoDamage(damage);
            message += $" for {damage} damage";
        }
        else
        {
            message += " but does no damage";
        }

        Color color = (actor.GetComponent<Player>() != null) ? Color.white : Color.red;
        UIManager.Instance.AddMessage(message, color);
    }
}
