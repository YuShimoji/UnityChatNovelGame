using UnityEngine;

namespace ProjectFoundPhone.Effects
{
    public class MetaEffectController : MonoBehaviour
    {
        public static MetaEffectController Instance { get; private set; }

        [SerializeField] private GlitchEffect m_GlitchEffect;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void TriggerGlitch()
        {
            if (m_GlitchEffect != null)
            {
                m_GlitchEffect.SetIntensity(1.0f);
            }
        }
    }
}
