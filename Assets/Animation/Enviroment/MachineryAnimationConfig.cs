using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Machinery/AnimationConfig", fileName = "NewMachineryConfig")]
public class MachineryAnimationConfig : ScriptableObject
{
    [Tooltip("Lista de pasos de la animación de montaje")]
    public List<AnimationStep> steps = new List<AnimationStep>();

    [System.Serializable]
    public class AnimationStep
    {
        [Tooltip("El transform del objeto a animar")]
        public Transform target;

        [Tooltip("Posición local o global desde donde parte (si está vacío, se usa la posición actual)")]
        public Vector3 fromPosition;
        [Tooltip("Posición local o global de destino")]
        public Vector3 toPosition;

        [Tooltip("Rotación desde la cual parte (euler)")]
        public Vector3 fromRotation;
        [Tooltip("Rotación objetivo (euler)")]
        public Vector3 toRotation;

        [Tooltip("Tiempo que tarda en completar este paso (segundos)")]
        public float duration = 1f;
        [Tooltip("Retraso antes de comenzar este paso (segundos)")]
        public float delay = 0f;

        [Tooltip("Curva de interpolación (0→1)")]
        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }
}
