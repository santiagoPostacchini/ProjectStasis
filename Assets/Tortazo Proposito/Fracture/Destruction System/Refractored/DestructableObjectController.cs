using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObjectController : MonoBehaviour
{
    public GameObject[] roots = new GameObject[4];
    [HideInInspector] public DestroyedPieceController[] root_dest_pieces = new DestroyedPieceController[4];

    private List<DestroyedPieceController> destroyed_pieces = new List<DestroyedPieceController>();


    public float initialDelay = 2f;
    public float timeBetweenDrops = 1f;
    public float forceMagnitude = 5f;

    public float percentDestroy = 0.45f;
    public Material stasisMaterial;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var _dpc = child.gameObject.AddComponent<DestroyedPieceController>();
            _dpc.matStasis = stasisMaterial;
            var _rigidbody = child.gameObject.AddComponent<Rigidbody>();
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = false;

            var _mc = child.gameObject.AddComponent<MeshCollider>();
            _mc.convex = true;
            _dpc.meshCollider = _mc;
            var _mct = child.gameObject.AddComponent<MeshCollider>();
            _mct.convex = true;
            _mct.isTrigger = true;
            _dpc.meshColliderTrigger = _mct;
            destroyed_pieces.Add(_dpc);
        }

        for (int _i = 0; _i < 4; _i++)
        {
            root_dest_pieces[_i] = roots[_i].GetComponent<DestroyedPieceController>();
        }
        StartCoroutine(run_physics_steps(10));
    }
    

    public void InitDestruction()
    {
        StartCoroutine(AutoCollapse());
    }
    public IEnumerator InstantCollapse()
    {
        yield return new WaitForSeconds(initialDelay); // Espera inicial

        // Clonamos y barajamos las piezas
        List<DestroyedPieceController> piecesToDrop = new List<DestroyedPieceController>(destroyed_pieces);
        Shuffle(piecesToDrop);

        foreach (var piece in piecesToDrop)
        {
            if (piece != null)
            {
                Vector3 randomDirection = Random.onUnitSphere;
                piece.cause_damage(randomDirection * forceMagnitude); // Fuerza aleatoria

            }
        }
    }
    //private IEnumerator AutoCollapse()
    //{
    //    yield return new WaitForSeconds(initialDelay); // Initial delay

    //    List<DestroyedPieceController> piecesToDrop = new List<DestroyedPieceController>(destroyed_pieces);
    //    Shuffle(piecesToDrop);

    //    int dropCount = Mathf.CeilToInt(piecesToDrop.Count * percentDestroy); 

    //    for (int i = 0; i < dropCount; i++)
    //    {
    //        var piece = piecesToDrop[i];
    //        if (piece != null)
    //        {
    //            Vector3 randomDirection = Random.onUnitSphere;
    //            piece.cause_damage(randomDirection * forceMagnitude);

    //            yield return new WaitForSeconds(timeBetweenDrops); // Delay between each piece
    //        }

    //    }
    //}
    private IEnumerator AutoCollapse()
    {
        yield return new WaitForSeconds(initialDelay); // Initial delay

        // Filtramos nulls antes de hacer cualquier cosa
        List<DestroyedPieceController> piecesToDrop = new List<DestroyedPieceController>();
        foreach (var piece in destroyed_pieces)
        {
            if (piece != null)
                piecesToDrop.Add(piece);
        }

        Shuffle(piecesToDrop); // Mezcla aleatoria

        int dropCount = Mathf.Min(
            Mathf.CeilToInt(piecesToDrop.Count * percentDestroy),
            piecesToDrop.Count); // Evita pasarte del límite

        for (int i = 0; i < dropCount; i++)
        {
            var piece = piecesToDrop[i];
            if (piece != null)
            {
                Vector3 randomDirection = Random.onUnitSphere;
                piece.cause_damage(randomDirection * forceMagnitude);

                yield return new WaitForSeconds(timeBetweenDrops); // Delay entre piezas
            }
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
    private void Update()
    {

        if (DestroyedPieceController.is_dirty)
        {

            foreach (var destroyed_piece in destroyed_pieces)
            {
                destroyed_piece.visited = false;
            }


            // do a breadth first search to find all connected pieces
            for (int _i = 0; _i < 4; _i++)
                find_all_connected_pieces(root_dest_pieces[_i]);

            // drop all pieces not reachable from root
            foreach (var piece in destroyed_pieces)
            {
                if (piece && !piece.visited)
                {
                    piece.drop();
                }
            }
        }
    }

    private void find_all_connected_pieces(DestroyedPieceController destroyed_piece)
    {
        if (!destroyed_piece.visited)
        {
            if (!destroyed_piece.is_connected)
                return;
            destroyed_piece.visited = true;

            foreach (var _pdc in destroyed_piece.connected_to)
            {
                find_all_connected_pieces(_pdc);
            }
        }
        else
            return;
    }

    private IEnumerator run_physics_steps(int step_count)
    {
        for (int i = 0; i < step_count; i++)
            yield return new WaitForFixedUpdate();

        foreach (var piece in destroyed_pieces)
        {
            piece.make_static();
        }
    }
}
