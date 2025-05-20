using UnityEngine;

namespace Puzzle_Elements
{
    public class EmancipationField : MonoBehaviour
    {
        [SerializeField] private Transform t;
    
    
        private void OnTriggerEnter(Collider other)
        {
            PhysicsObject physicObject = other.GetComponent<PhysicsObject>();
            if (physicObject)
            {
                Debug.Log("A");
                if (!physicObject.player.GetComponent<PlayerInteractor>()._objectGrabbable) return;
                PhysicsBox box = physicObject as PhysicsBox;
                if (box && box.IsFreezed) box.UnfreezeObject();
                physicObject.player.GetComponent<PlayerInteractor>()._objectGrabbable.Drop();
                physicObject.player.GetComponent<PlayerInteractor>()._objectGrabbable.gameObject.transform.position = t.position;
                physicObject.player.GetComponent<PlayerInteractor>().ClearHands();

            }
        }
    }
}
