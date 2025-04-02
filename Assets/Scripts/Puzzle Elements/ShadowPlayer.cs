using UnityEngine;

public class ShadowPlayer : MonoBehaviour
{
    [SerializeField] private Player _player;

    public void UpdateShadowPlayerPosition()
    {
        transform.position = new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z);
    }
}
