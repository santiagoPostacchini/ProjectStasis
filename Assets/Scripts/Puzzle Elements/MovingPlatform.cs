using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float pauseDuration = 0.5f;

    private Rigidbody rb;
    private Vector3 target;
    private Coroutine moveCoroutine;
    private Player _player;

    public bool canMove;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        target = pointB.position;
        moveCoroutine = StartCoroutine(MovePlatform());
        canMove = true;
    }

    private IEnumerator MovePlatform()
    {
        while (true)
        {
            // Mover hacia el objetivo
            while (Vector3.Distance(rb.position, target) > 0.05f)
            {
                Vector3 newPosition = Vector3.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
                rb.MovePosition(newPosition);
                yield return new WaitForFixedUpdate();
            }

            // Llegó al límite, pausa
            yield return new WaitForSeconds(pauseDuration);

            // Cambiar dirección
            target = target == pointA.position ? pointB.position : pointA.position;
            if(_player != null)
            {
                Rigidbody rbPlayer = _player.GetComponent<Rigidbody>();
                if (rbPlayer != null)
                {
                    rbPlayer.velocity = rb.velocity;
                }
                Debug.Log("La inercia del player es " + _player.GetComponent<Rigidbody>().velocity);
            }

            

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player !=null)
        {
            player.transform.SetParent(transform);
            _player = player;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if(player!= null)
        {
            player.transform.SetParent(null);
            _player = null;
        }
        
    }
}