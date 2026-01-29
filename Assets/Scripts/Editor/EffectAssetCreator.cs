using UnityEngine;
using UnityEditor;

namespace ProjectFoundPhone.EditorTools
{
    public static class EffectAssetCreator
    {
        [MenuItem("Tools/FoundPhone/Create Sparkle Effect")]
        public static void CreateSparkleEffect()
        {
            // Create GameObject
            GameObject go = new GameObject("Sparkle");
            
            // Add Particle System
            ParticleSystem ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = 1.0f;
            main.startLifetime = 1.0f;
            main.startSpeed = 5.0f;
            main.startSize = 0.5f;
            main.loop = false;
            
            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, 30) });
            
            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.5f;

            // Save as Prefab
            string path = "Assets/Resources/Effects/Sparkle.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, path);
            
            // Cleanup
            GameObject.DestroyImmediate(go);
            
            Debug.Log($"[EffectCreator] Created Sparkle prefab at {path}");
        }
    }
}
