using Godot;
using System;

public partial class NoiseSettings : Resource
{
    [Export]
    public float Strength { get; set; } = 1;

    [Export(PropertyHint.Range, "1,8")]
    public int NumLayers { get; set; } = 1;

    [Export]
    public float BaseRoughness { get; set; } = 1;

    [Export]
    public float Roughness { get; set; } = 2;

    [Export]
    public float Persistence { get; set; } = 0.5f;

    [Export]
    public Vector3 Centre { get; set; } = Vector3.Zero;

    [Export]
    public float MinValue { get; set; } = 0;

    public override bool Equals(object obj)
    {
        if (obj is NoiseSettings other)
        {
            return Strength == other.Strength &&
                   NumLayers == other.NumLayers &&
                   BaseRoughness == other.BaseRoughness &&
                   Roughness == other.Roughness &&
                   Persistence == other.Persistence &&
                   Centre == other.Centre &&
                   MinValue == other.MinValue;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Strength, NumLayers, BaseRoughness, Roughness, Persistence, Centre, MinValue);
    }

    public NoiseSettings Clone()
    {
        return new NoiseSettings
        {
            Strength = this.Strength,
            NumLayers = this.NumLayers,
            BaseRoughness = this.BaseRoughness,
            Roughness = this.Roughness,
            Persistence = this.Persistence,
            Centre = this.Centre,
            MinValue = this.MinValue
        };
    }
}