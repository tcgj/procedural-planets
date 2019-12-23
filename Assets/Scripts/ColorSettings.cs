using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ColorSettings : ScriptableObject {

    public Material planetMaterial;
    public BiomeColorSettings biomeColorSettings;
    public Gradient oceanColor;

    [System.Serializable]
    public class BiomeColorSettings {

        public Biome[] biomes;
        public NoiseLayer noise;
        public float noiseOffset;
        public float noiseStrength;
        [Range(0, 1)]
        public float blendAmount;

        [System.Serializable]
        public class Biome {

            public Gradient gradient;
            public Color tint;
            [Range(0, 1)]
            public float tintPercentage;
            [Range(0, 1)]
            public float startHeight;
        }
    }
}
