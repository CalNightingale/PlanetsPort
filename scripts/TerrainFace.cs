using Godot;
using System;

public class TerrainFace
{
    private ArrayMesh _mesh;
    private Planet.PlanetSettings _settings;
    private Vector3 _localUp;
    private Vector3 _axisA;
    private Vector3 _axisB;

    public TerrainFace(ArrayMesh mesh, Planet.PlanetSettings settings, Vector3 localUp)
    {
        _mesh = mesh;
        _settings = settings;
        _localUp = localUp;

        _axisA = new Vector3(localUp.Y, localUp.Z, localUp.X);
        _axisB = _localUp.Cross(_axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[_settings.Resolution * _settings.Resolution];
        int[] indices = new int[(_settings.Resolution - 1) * (_settings.Resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < _settings.Resolution; y++)
        {
            for (int x = 0; x < _settings.Resolution; x++)
            {
                int i = x + y * _settings.Resolution;
                Vector2 percent = new Vector2(x, y) / (float)(_settings.Resolution - 1);
                Vector3 pointOnUnitCube = _localUp + (percent.X - 0.5f) * 2 * _axisA + (percent.Y - 0.5f) * 2 * _axisB;
                Vector3 pointOnSphere = pointOnUnitCube.Normalized() * _settings.Radius;
                vertices[i] = pointOnSphere;

                if (x != _settings.Resolution - 1 && y != _settings.Resolution - 1)
                {
                    indices[triIndex] = i;
                    indices[triIndex + 1] = i + _settings.Resolution + 1;
                    indices[triIndex + 2] = i + _settings.Resolution;

                    indices[triIndex + 3] = i;
                    indices[triIndex + 4] = i + 1;
                    indices[triIndex + 5] = i + _settings.Resolution + 1;
                    triIndex += 6;
                }
            }
        }

        _mesh.ClearSurfaces();
        var array = new Godot.Collections.Array();
        array.Resize((int)Mesh.ArrayType.Max);
        array[(int)Mesh.ArrayType.Vertex] = vertices;
        array[(int)Mesh.ArrayType.Index] = indices;
        _mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, array);
        UpdateColor(_settings.Color);
    }

    public void UpdateSettings(Planet.PlanetSettings newSettings)
    {
        _settings = newSettings;
        ConstructMesh();
    }

    public void UpdateColor(Color newColor)
    {
        _settings.Color = newColor;
        StandardMaterial3D material;

        if (_mesh.GetSurfaceCount() > 0 && _mesh.SurfaceGetMaterial(0) is StandardMaterial3D existingMaterial)
        {
            material = existingMaterial;
        }
        else
        {
            material = new StandardMaterial3D();
        }

        material.AlbedoColor = _settings.Color;
        _mesh.SurfaceSetMaterial(0, material);
    }
}