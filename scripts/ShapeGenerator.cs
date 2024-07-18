using Godot;
using System;

public class ShapeGenerator
{
    private ShapeSettings settings;
    private NoiseFilter[] noiseFilters;

    public ShapeGenerator(ShapeSettings settings)
    {
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        if (settings.NoiseLayers == null)
        {
            noiseFilters = Array.Empty<NoiseFilter>();
        }
        else
        {
            noiseFilters = new NoiseFilter[settings.NoiseLayers.Length];
            for (int i = 0; i < noiseFilters.Length; i++)
            {
                if (settings.NoiseLayers[i]?.NoiseSettings != null)
                {
                    noiseFilters[i] = new NoiseFilter(settings.NoiseLayers[i].NoiseSettings);
                }
                else
                {
                    GD.PrintErr($"NoiseSettings is null for layer {i}");
                    noiseFilters[i] = new NoiseFilter(new NoiseSettings()); // Use default settings
                }
            }
        }
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0;

        if (noiseFilters.Length > 0)
        {
            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
            if (settings.NoiseLayers[0].Enabled)
            {
                elevation = firstLayerValue;
            }
        }

        for (int i = 1; i < noiseFilters.Length; i++)
        {
            if (settings.NoiseLayers[i].Enabled)
            {
                float mask = settings.NoiseLayers[i].UseFirstLayerAsMask ? firstLayerValue : 1;
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
            }
        }

        return pointOnUnitSphere * settings.PlanetRadius * (1 + elevation);
    }
}