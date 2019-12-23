using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidgedNoiseFilter : INoiseFilter {

    NoiseLayer settings;
    Noise noise = new Noise();

    public RidgedNoiseFilter(NoiseLayer settings) {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point) {
        float noiseValue = 0;
        float frequency = settings.baseFrequency;
        float amplitude = 1;
        float weight = 1;

        for (int i = 0; i < settings.octaveCount; i++) {
            float val = 1 - Mathf.Abs(noise.Evaluate(point * frequency + settings.centre));
            val *= val; // Make ridges more pronounced
            val *= weight;
            weight = Mathf.Clamp01(val * settings.weightMultiplier);
            noiseValue += val * amplitude;
            frequency *= settings.lacunarity;
            amplitude *= settings.persistence;
        }

        noiseValue -= settings.minNoise;
        return noiseValue * settings.strength;
    }
}
