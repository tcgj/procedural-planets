using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNoiseFilter : INoiseFilter {

    NoiseLayer settings;
    Noise noise = new Noise();

    public SimpleNoiseFilter(NoiseLayer settings) {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point) {
        float noiseValue = 0;
        float frequency = settings.baseFrequency;
        float amplitude = 1;

        for (int i = 0; i < settings.octaveCount; i++) {
            float val = noise.Evaluate(point * frequency + settings.centre);
            noiseValue += (val + 1) * 0.5f * amplitude;
            frequency *= settings.lacunarity;
            amplitude *= settings.persistence;
        }

        noiseValue -= settings.minNoise;
        return noiseValue * settings.strength;
    }
}
