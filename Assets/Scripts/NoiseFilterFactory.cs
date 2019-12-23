using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseFilterFactory {
    public static INoiseFilter CreateNoiseFilter(NoiseLayer settings) {
        switch(settings.filterType) {
            case NoiseLayer.FilterType.Simple:
                return new SimpleNoiseFilter(settings);
            case NoiseLayer.FilterType.Ridged:
                return new RidgedNoiseFilter(settings);
            default:
                return null;
        }
    }
}