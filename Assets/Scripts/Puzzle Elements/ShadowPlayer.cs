using UnityEngine;

public class ShadowPlayer : MonoBehaviour
{
    [SerializeField] private Player _player;
    private void Start()
    {
        var posInitial = new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z);
        transform.position = posInitial;
    }
    public void UpdateShadowPlayerPosition()
    {
        transform.position = new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z);
    }
}
