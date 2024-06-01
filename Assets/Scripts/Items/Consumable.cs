using UnityEngine;

namespace Items
{
    public class Consumable : MonoBehaviour
    {
        public enum ItemType
        {
            HealthPotion,
            Fireball,
            ScrollOfConfusion
        }

        [SerializeField] private ItemType type;
        public ItemType Type { get => type; }

        private void Start()
        {
            GameManager.Get.AddItem(this);
        }
    }

    public class HealthPotion : Consumable
    {
        public int HealAmount;

        private void Start()
        {
            HealAmount = 20;
        }
    }

    public class Fireball : Consumable
    {
        public int Damage;

        private void Start()
        {
            Damage = 20;
        }
    }

    public class ScrollOfConfusion : Consumable
    {
        // Je kunt hier specifieke eigenschappen en methodes toevoegen voor ScrollOfConfusion

        private void Start()
        {
            // Initialiseer eventuele eigenschappen
        }
    }
}
