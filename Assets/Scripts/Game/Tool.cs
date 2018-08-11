using UnityEngine;

namespace Game {
    public abstract class Tool : MonoBehaviour {
        public bool canUse;
        
        public void Pickup(Vector3 playerPos) {
            transform.position = playerPos;
            gameObject.SetActive(false);
        }

        public void Equip() {
            gameObject.SetActive(true);
        }

        public void UnEquip() {
            gameObject.SetActive(false);
        }

        public abstract void UseItem();
        public abstract void SetUsable(bool usable, string type, GameObject interactable);
    }
}
