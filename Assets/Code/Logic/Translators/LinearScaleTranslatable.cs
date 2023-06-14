using UnityEngine;

namespace Logic.Translators
{
    public class LinearScaleTranslatable : Translatable<Vector3>
    {
        protected override void OnInit()
        {
        }

        protected override void ApplyTranslation(Vector3 vector)
        {
            transform.localScale = vector;
        }
    }
}