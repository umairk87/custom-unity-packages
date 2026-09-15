using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace Framework.UISequence
{
    public enum AnimationActionType
    {
        Delay,
        Enable,
        Disable,
        Move,
        Scale,
        Rotate,
        Fade,
        Sound,
        Callback
    }

    public enum MoveTweenType
    {
        Move,
        Punch,
        Shake,
        Jump
    }

    public enum ScaleTweenType
    {
        Scale,
        Punch,
        Shake
    }

    public enum RotateTweenType
    {
        Rotate,
        Punch,
        Shake
    }

    [Serializable]
    public class AnimationAction
    {
        public AnimationActionType type;

        [Header("Delay")]
        [Min(0f)]
        public float delay = 0.5f;

        [Header("Object")]
        public GameObject objectTarget;

        [Header("Transform")]
        public Transform transformTarget;

        public bool local = true;

        [Header("Move")]

        public MoveTweenType moveTween = MoveTweenType.Move;

        public RectTransform moveTarget;

        public RectTransform startPoint;

        public RectTransform endPoint;

        [Min(0f)]
        public float moveDuration = 0.5f;

        public Ease moveEase = Ease.OutQuad;
        [Header("Jump")]
        public float jumpPower = 1f;

        public int jumpCount = 1;

        [Header("Punch")]
        public Vector3 punch = Vector3.one;

        [Min(0.01f)]
        public float punchDuration = 0.5f;

        [Min(1)]
        public int punchVibrato = 10;

        [Range(0f, 1f)]
        public float punchElasticity = 1f;

        [Header("Shake")]
        public Vector3 shakeStrength = Vector3.one;

        [Min(0.01f)]
        public float shakeDuration = 0.5f;

        [Min(1)]
        public int shakeVibrato = 10;

        [Range(0f, 180f)]
        public float shakeRandomness = 90f;

        [Header("Scale")]
        public ScaleTweenType scaleTween = ScaleTweenType.Scale;

        public Vector3 scale = Vector3.one;

        [Min(0f)]
        public float scaleDuration = 0.5f;

        public Ease scaleEase = Ease.OutQuad;

        [Header("Rotate")]
        public RotateTweenType rotateTween = RotateTweenType.Rotate;

        public Vector3 rotation;

        [Min(0f)]
        public float rotateDuration = 0.5f;

        public Ease rotateEase = Ease.OutQuad;

        [Header("Fade")]
        public CanvasGroup canvasGroup;

        [Range(0f, 1f)]
        public float alpha = 1f;

        [Min(0f)]
        public float fadeDuration = 0.5f;

        public Ease fadeEase = Ease.OutQuad;

        [Header("Sound")]
        // public AudioDefinition sound;

        public AudioSource audioSource;
        public AudioClip audioClip;

        [Range(0f, 1f)]
        public float volume = 1f;

        [Header("Callback")]
        public UnityEvent callback;

        public Tween Build()
        {
            switch (type)
            {
                case AnimationActionType.Delay:
                    return BuildDelay();

                case AnimationActionType.Enable:
                    return BuildEnable();

                case AnimationActionType.Disable:
                    return BuildDisable();

                case AnimationActionType.Move:
                    return BuildMove();

                case AnimationActionType.Scale:
                    return BuildScale();

                case AnimationActionType.Rotate:
                    return BuildRotate();

                case AnimationActionType.Fade:
                    return BuildFade();

                case AnimationActionType.Sound:
                    return BuildSound();

                case AnimationActionType.Callback:
                    return BuildCallback();

                default:
                    return null;
            }
        }

        private Tween BuildDelay()
        {
            return DOVirtual.DelayedCall(
                delay,
                () => { });
        }

        private Tween BuildEnable()
        {
            return DOVirtual.DelayedCall(
                0f,
                () =>
                {
                    if (objectTarget != null)
                        objectTarget.SetActive(true);
                });
        }

        private Tween BuildDisable()
        {
            return DOVirtual.DelayedCall(
                0f,
                () =>
                {
                    if (objectTarget != null)
                        objectTarget.SetActive(false);
                });
        }

        private Tween BuildMove()
        {
            if (moveTarget == null)
                return EmptyTween();

            switch (moveTween)
            {
                case MoveTweenType.Move:
                    {
                        if (startPoint != null)
                        {
                            moveTarget.anchoredPosition =
                                GetCanvasPosition(startPoint);
                        }

                        if (endPoint == null)
                            return EmptyTween();

                        Vector2 endPosition =
                            GetCanvasPosition(endPoint);

                        return moveTarget
                            .DOAnchorPos(endPosition, moveDuration)
                            .SetEase(moveEase);
                    }

                case MoveTweenType.Punch:

                    return moveTarget.DOPunchAnchorPos(
                        punch,
                        punchDuration,
                        punchVibrato,
                        punchElasticity);

                case MoveTweenType.Shake:

                    return moveTarget.DOShakeAnchorPos(
                        shakeDuration,
                        shakeStrength,
                        shakeVibrato,
                        shakeRandomness);

                case MoveTweenType.Jump:
                    {
                        if (startPoint != null)
                        {
                            moveTarget.anchoredPosition =
                                GetCanvasPosition(startPoint);
                        }

                        if (endPoint == null)
                            return EmptyTween();

                        Vector2 endPosition =
                            GetCanvasPosition(endPoint);

                        return moveTarget
                            .DOJumpAnchorPos(
                                endPosition,
                                jumpPower,
                                jumpCount,
                                moveDuration)
                            .SetEase(moveEase);
                    }

                default:
                    return EmptyTween();
            }
        }
        private Vector2 GetCanvasPosition(
    RectTransform point)
        {
            if (point == null)
                return Vector2.zero;

            RectTransform parent =
                moveTarget.parent as RectTransform;

            if (parent == null)
                return point.anchoredPosition;

            Vector3 worldPosition =
                point.TransformPoint(point.rect.center);

            Vector3 localPosition =
                parent.InverseTransformPoint(worldPosition);

            return new Vector2(
                localPosition.x,
                localPosition.y);
        }


        private Tween BuildScale()
        {
            if (transformTarget == null)
                return EmptyTween();

            switch (scaleTween)
            {
                case ScaleTweenType.Scale:

                    return transformTarget
                        .DOScale(scale, scaleDuration)
                        .SetEase(scaleEase);

                case ScaleTweenType.Punch:

                    return transformTarget
                        .DOPunchScale(
                            punch,
                            punchDuration,
                            punchVibrato,
                            punchElasticity);

                case ScaleTweenType.Shake:

                    return transformTarget
                        .DOShakeScale(
                            shakeDuration,
                            shakeStrength,
                            shakeVibrato,
                            shakeRandomness);

                default:
                    return EmptyTween();
            }
        }

        private Tween BuildRotate()
        {
            if (transformTarget == null)
                return EmptyTween();

            switch (rotateTween)
            {
                case RotateTweenType.Rotate:

                    return local
                        ? transformTarget
                            .DOLocalRotate(
                                rotation,
                                rotateDuration)
                            .SetEase(rotateEase)
                        : transformTarget
                            .DORotate(
                                rotation,
                                rotateDuration)
                            .SetEase(rotateEase);

                case RotateTweenType.Punch:

                    return transformTarget
                        .DOPunchRotation(
                            rotation,
                            punchDuration,
                            punchVibrato,
                            punchElasticity);

                case RotateTweenType.Shake:

                    return transformTarget
                        .DOShakeRotation(
                            shakeDuration,
                            shakeStrength,
                            shakeVibrato,
                            shakeRandomness);

                default:
                    return EmptyTween();
            }
        }

        private Tween BuildFade()
        {
            if (canvasGroup == null)
                return EmptyTween();

            return canvasGroup
                .DOFade(alpha, fadeDuration)
                .SetEase(fadeEase);
        }

        private Tween BuildSound()
        {
            return DOVirtual.DelayedCall(
                0f,
                () =>
                {
                    if (audioSource != null &&
                        audioClip != null)
                    {
                        audioSource.PlayOneShot(
                            audioClip,
                            volume);
                    }
                });
        }


        // private Tween BuildSound()
        // {
        //     return DOVirtual.DelayedCall(0f, () =>
        //     {
        //         if (audioSource == null || clip == null)
        //             return;

        //         audioSource.PlayOneShot(clip, volume);
        //     });
        // }

        private Tween BuildCallback()
        {
            return DOVirtual.DelayedCall(
                0f,
                () =>
                {
                    callback?.Invoke();
                });
        }

        private Tween EmptyTween()
        {
            return DOVirtual.DelayedCall(
                0f,
                () => { });
        }
    }

    public class AnimationSequence : MonoBehaviour
    {
        [SerializeField]
        private List<AnimationAction> actions = new();

        [Header("Settings")]

        [SerializeField]
        private bool playOnEnable;

        [SerializeField]
        private bool killPreviousOnPlay = true;

        private Sequence sequence;

        public bool IsPlaying =>
            sequence != null &&
            sequence.IsActive() &&
            sequence.IsPlaying();

        public event Action Completed;

        private void OnEnable()
        {
            if (playOnEnable)
                Play();
        }

        public void Play()
        {
            if (killPreviousOnPlay)
                Stop();

            sequence = DOTween.Sequence();

            for (int i = 0; i < actions.Count; i++)
            {
                AnimationAction action = actions[i];

                if (action == null)
                    continue;

                Tween tween = action.Build();

                if (tween != null)
                    sequence.Append(tween);
            }

            sequence.OnComplete(() =>
            {
                sequence = null;
                Completed?.Invoke();
            });
        }

        public void Stop()
        {
            if (sequence == null)
                return;

            sequence.Kill();
            sequence = null;
        }

        public void Pause()
        {
            sequence?.Pause();
        }

        public void Resume()
        {
            sequence?.Play();
        }

        public void Restart()
        {
            Stop();
            Play();
        }

        public void Complete()
        {
            sequence?.Complete();
        }

        private void OnDestroy()
        {
            Stop();
        }

        [ContextMenu("Play")]
        private void PlayContext()
        {
            Play();
        }

        [ContextMenu("Stop")]
        private void StopContext()
        {
            Stop();
        }

        [ContextMenu("Restart")]
        private void RestartContext()
        {
            Restart();
        }
    }
}