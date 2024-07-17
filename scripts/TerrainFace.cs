using Godot;
using System;

public class TerrainFace
{
    private ArrayMesh _mesh;
    private int _resolution;
    private Color _color;
    private float _radius;
    private Vector3 _localUp;
    private Vector3 _axisA;
    private Vector3 _axisB;

    public TerrainFace(ArrayMesh mesh, int resolution, Color color, float radius, Vector3 localUp)
    {
        _mesh = mesh;
        _resolution = resolution;
        _radius = radius;
        _color = color;
        _localUp = localUp;

        _axisA = new Vector3(localUp.Y, localUp.Z, localUp.X);
        _axisB = _localUp.Cross(_axisA);
    }

        public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[_resolution * _resolution];
        int[] indices = new int[(_resolution - 1) * (_resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < _resolution; y++)
        {
            for (int x = 0; x < _resolution; x++)
            {
                int i = x + y * _resolution;
                Vector2 percent = new Vector2(x, y) / (float)(_resolution - 1);
                Vector3 pointOnUnitCube = _localUp + (percent.X - 0.5f) * 2 * _axisA + (percent.Y - 0.5f) * 2 * _axisB;
                Vector3 pointOnSphere = pointOnUnitCube.Normalized() * _radius;
                vertices[i] = pointOnSphere;

                if (x != _resolution - 1 && y != _resolution - 1)
                {
                    indices[triIndex] = i;
                    indices[triIndex + 1] = i + _resolution + 1;
                    indices[triIndex + 2] = i + _resolution;

                    indices[triIndex + 3] = i;
                    indices[triIndex + 4] = i + 1;
                    indices[triIndex + 5] = i + _resolution + 1;
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
        UpdateColor(_color);
    }

    public void UpdateColor(Color newColor)
    {
        _color = newColor;
        StandardMaterial3D material;

        if (_mesh.GetSurfaceCount() > 0 && _mesh.SurfaceGetMaterial(0) is StandardMaterial3D existingMaterial)
        {
            material = existingMaterial;
        }
        else
        {
            material = new StandardMaterial3D();
        }

        material.AlbedoColor = _color;
        _mesh.SurfaceSetMaterial(0, material);
    }
}
