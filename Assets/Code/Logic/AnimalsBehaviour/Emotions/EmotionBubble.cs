using UnityEngine;

namespace Logic.AnimalsBehaviour.Emotions
{
    public class EmotionBubble : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _icon;

        public void UpdateEmotion(Emotion emotion)
        {
            _icon.sprite = emotion.Sprite;
            Debug.Log($"Update emotion to {emotion.Name}");
        }
    }
}