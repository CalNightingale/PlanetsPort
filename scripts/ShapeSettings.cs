using System.Linq;
using Godot;

public partial class ShapeSettings : Resource
{
    [Export]
    public float PlanetRadius { get; set; } = 1;

    [Export]
    public NoiseLayer[] NoiseLayers { get; set; } = new NoiseLayer[0];

    public override bool Equals(object obj)
    {
        if (obj is ShapeSettings other)
        {
            return PlanetRadius == other.PlanetRadius &&
                   NoiseLayers.Length == other.NoiseLayers.Length &&
                   NoiseLayers.Zip(other.NoiseLayers, (a, b) => a.Equals(b)).All(equal => equal);
        }
        return false;
    }

    public ShapeSettings Clone()
    {
        return new ShapeSettings
        {
            PlanetRadius = this.PlanetRadius,
            NoiseLayers = this.NoiseLayers.Select(layer => layer.Clone()).ToArray()
        };
    }
}

public partial class NoiseLayer : Resource
{
    [Export]
    public bool Enabled { get; set; } = true;

    [Export]
    public bool UseFirstLayerAsMask { get; set; }

    [Export]
    public NoiseSettings NoiseSettings { get; set; }

    public override bool Equals(object obj)
    {
        if (obj is NoiseLayer other)
        {
            return Enabled == other.Enabled &&
                   UseFirstLayerAsMask == other.UseFirstLayerAsMask &&
                   NoiseSettings.Equals(other.NoiseSettings);
        }
        return false;
    }

    public NoiseLayer Clone()
    {
        return new NoiseLayer
        {
            Enabled = this.Enabled,
            UseFirstLayerAsMask = this.UseFirstLayerAsMask,
            NoiseSettings = this.NoiseSettings.Clone()
        };
    }
}