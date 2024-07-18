using Godot;
using System;

[Tool]
public partial class Planet : Node3D
{
    [Export(PropertyHint.Range, "2,256")]
    public int Resolution { get; set; } = 10;

    [Export]
    public bool AutoUpdate { get; set; } = true;

    [Export]
    public ShapeSettings ShapeSettings { get; set; }

    [Export]
    public ColorSettings ColorSettings { get; set; }

    private ShapeGenerator shapeGenerator;
    private MeshInstance3D[] meshInstances = new MeshInstance3D[6];
    private TerrainFace[] terrainFaces = new TerrainFace[6];

    // Add these fields to track changes
    private int lastResolution;
    private ShapeSettings lastShapeSettings;
    private ColorSettings lastColorSettings;

    public override void _Ready()
    {
        Initialize();
        GeneratePlanet();
        UpdateLastValues();
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint() && AutoUpdate && SettingsChanged())
        {
            GeneratePlanet();
            UpdateLastValues();
        }
    }

    private bool SettingsChanged()
    {
        return Resolution != lastResolution ||
               ShapeSettings != lastShapeSettings ||
               ColorSettings != lastColorSettings ||
               (ShapeSettings != null && !ShapeSettings.Equals(lastShapeSettings)) ||
               (ColorSettings != null && !ColorSettings.Equals(lastColorSettings));
    }

    private void UpdateLastValues()
    {
        lastResolution = Resolution;
        lastShapeSettings = ShapeSettings?.Clone() as ShapeSettings;
        lastColorSettings = ColorSettings?.Clone() as ColorSettings;
    }

    private void Initialize()
    {
        shapeGenerator = new ShapeGenerator(ShapeSettings);
        Vector3[] directions = { Vector3.Up, Vector3.Down, Vector3.Left, Vector3.Right, Vector3.Forward, Vector3.Back };

        for (int i = 0; i < 6; i++)
        {
            if (meshInstances[i] == null)
            {
                meshInstances[i] = new MeshInstance3D();
                AddChild(meshInstances[i]);
                meshInstances[i].Mesh = new ArrayMesh();
                meshInstances[i].MaterialOverride = new StandardMaterial3D();
            }

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshInstances[i].Mesh as ArrayMesh, Resolution, directions[i]);
        }
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    private void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }

    private void GenerateColors()
    {
        foreach (MeshInstance3D m in meshInstances)
        {
            (m.MaterialOverride as StandardMaterial3D).AlbedoColor = ColorSettings.PlanetColor;
        }
    }
}