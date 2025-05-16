using UnityEngine;
using UnityEngine.UI;

public class DetectorStasisObjects : MonoBehaviour
{
    [Header("Stasis Detection")]
    [Tooltip("Distancia del raycast para detectar objetos stasis")]
    public float detectionDistance = 100f;

    [Header("Reticle UI")]
    [Tooltip("Lista de imágenes de la mira; todas se animan")]
    public Image[] reticleImages;

    [Header("Animation Settings")]
    [Tooltip("Curva de easing para la animación (entrada/salida)")]
    public AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Tooltip("Velocidad de la progresión (1 = 1 segundo para ir de 0→1)")]
    public float animSpeed = 2f;

    [Header("Scale Settings")]
    [Tooltip("Escala en reposo")]
    public float normalScale = 1f;
    [Tooltip("Escala al highlight")]
    public float highlightScale = 1.2f;

    [Header("Color Settings")]
    [Tooltip("Color en reposo")]
    public Color normalColor = Color.white;
    [Tooltip("Color al highlight")]
    public Color highlightColor = Color.cyan;

    [Header("Rotation Settings")]
    [Tooltip("Velocidad de giro máxima (grados/segundo)")]
    public float rotationSpeed = 90f;

    [Header("Tick Sound")]
    [Tooltip("Nombre exacto del sonido en AudioManager (ambientSounds)")]
    public string tickingSoundName = "Ticking.Pulse";

    private bool isAiming = false;
    private float animProgress = 0f;
    private PhysicsObject _physicObject;

    [SerializeField] private Player _player;


    void Start()
    {
        // Inicializa todas las imágenes en estado reposo
        foreach (var img in reticleImages)
        {
            if (img == null) continue;
            img.rectTransform.localScale = Vector3.one * normalScale;
            img.color = normalColor;
            img.rectTransform.localEulerAngles = Vector3.zero;
        }
    }

    void Update()
    {
        DetectStasisObjects(Camera.main.transform);

        // Actualiza el progreso de animación (0→1 o 1→0)
        float target = isAiming ? 1f : 0f;
        animProgress = Mathf.MoveTowards(animProgress, target, Time.deltaTime * animSpeed);
        float t = animCurve.Evaluate(animProgress);

        // Anima todas las partes de la mira
        foreach (var img in reticleImages)
        {
            if (img == null) continue;

            // Escala
            float s = Mathf.Lerp(normalScale, highlightScale, t);
            img.rectTransform.localScale = Vector3.one * s;

            // Color
            img.color = Color.Lerp(normalColor, highlightColor, t);

            // Rotación
            float angle = img.rectTransform.localEulerAngles.z + rotationSpeed * t * Time.deltaTime;
            img.rectTransform.localEulerAngles = new Vector3(0, 0, angle);
        }
    }

    void DetectStasisObjects(Transform cam)
    {
        
        bool hitStasis = false;
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, detectionDistance))
        {
            var po = hit.collider.GetComponent<PhysicsObject>();
            if (po != null && po is IStasis)
            {
                hitStasis = true;
                if (!isAiming)
                {
                    // Empieza la animación y el sonido
                    isAiming = true;
                    AudioManager.Instance?.PlayAmbient(tickingSoundName);



                    if (!po._isFreezed &&
                        po.player?.playerInteractor._objectGrabbable == null)
                    {
                        _physicObject = po;
                        Color softGreen = new Color(0.7f, 1f, 0.7f, 1f);
                        _physicObject.SetColorOutline(softGreen, 0.3f);
                        _physicObject.SetOutlineThickness(1.05f);
                        AudioManager.Instance?.PlaySfx("SelectStasiable");
                    }
                }
            }
        }
        if(_player.playerInteractor._objectGrabbable != null)
        {
            AudioManager.Instance?.StopAmbient(tickingSoundName);
        }
        if (!hitStasis && isAiming)
        {
            // Termina la animación y detiene el sonido
            isAiming = false;
            AudioManager.Instance?.StopAmbient(tickingSoundName);

            if (_physicObject != null)
            {
                if (!_physicObject._isFreezed)
                {
                    _physicObject.SetColorOutline(Color.green, 1);
                    _physicObject.SetOutlineThickness(1f);
                }

                // _physicObject.Glow(false, 1);

                _physicObject = null;
            }
        }
    }
}
