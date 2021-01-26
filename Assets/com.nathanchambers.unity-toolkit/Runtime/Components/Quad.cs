using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {

    public class Quad : MonoBehaviourEx {
        private MeshRenderer meshRenderer = null;
        private MeshFilter meshFilter = null;
        private Mesh mesh = null;

        private Vector3 origin = Vector3.zero;
        private Vector3 normal = Vector3.up;
        private Vector2 size = Vector2.one;
        private Vector2Int tileSize = Vector2Int.one;

        private Vector3[] vertices = null;
        private Vector3[] normals = null;
        private Vector2[] uv = null;
        private int[] triangles = null;

        private bool isDirty = false;

        public static Quad Create(Vector3 _origin, Vector3 _normal, Vector2 _size, Material _material) {
            GameObject go = new GameObject("Quad");
            Quad quad = go.AddComponent<Quad>();
            quad.CreateInternal(_origin, _normal, _size, _material);
            return quad;
        }

        private void CreateInternal(Vector3 _origin, Vector3 _normal, Vector2 _size, Material _material) {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshFilter = gameObject.AddComponent<MeshFilter>();

            origin = _origin;
            normal = _normal;
            size = _size;

            meshRenderer.sharedMaterial = new Material(_material);
            mesh = new Mesh();

            Rebuild();
        }

        public override void Update() {
            base.Update();
            
            if (isDirty == true) {
                Rebuild();
                isDirty = false;
            }
        }

        private void Rebuild() {
            Vector3 left = Vector3.Cross(Vector3.forward, normal);
            Vector3 down = Vector3.Cross(left, normal);
            float halfX = size.x * 0.5f;
            float halfY = size.y * 0.5f;

            if (tileSize == Vector2.zero) {
                throw new CriticalErrorException("Tile size cannot be zero");
            }

            float tileSizeX = size.x / tileSize.x;
            float tileSizeY = size.y / tileSize.y;
            float tileUVSizeX = 1.0f / tileSize.x;
            float tileUVSizeY = 1.0f / tileSize.y;
            int tileCount = tileSize.x * tileSize.y;

            vertices = new Vector3[tileCount * 4];
            normals = new Vector3[tileCount * 4];
            uv = new Vector2[tileCount * 4];
            triangles = new int[tileCount * 6];

            int tileIdx = 0;
            for (int y = 0; y < tileSize.y; ++y) {
                for (int x = 0; x < tileSize.x; ++x) {
                    int vertIdx = tileIdx * 4;
                    int triangleIdx = tileIdx * 6;

                    Vector3 bottomLeft = (left * (halfX - (tileSizeX * x))) + (down * (halfY - (tileSizeY * y)));
                    vertices[vertIdx + 0] = bottomLeft;
                    vertices[vertIdx + 1] = bottomLeft - (left * tileSizeX);
                    vertices[vertIdx + 2] = bottomLeft - (left * tileSizeX) - (down * tileSizeY);
                    vertices[vertIdx + 3] = bottomLeft - (down * tileSizeY);

                    uv[vertIdx + 0].Set(0.0f, 0.0f);
                    uv[vertIdx + 1].Set(1.0f, 0.0f);
                    uv[vertIdx + 2].Set(1.0f, 1.0f);
                    uv[vertIdx + 3].Set(0.0f, 1.0f);

                    normals[vertIdx + 0] = normal;
                    normals[vertIdx + 1] = normal;
                    normals[vertIdx + 2] = normal;
                    normals[vertIdx + 3] = normal;

                    triangles[triangleIdx + 0] = vertIdx + 0;
                    triangles[triangleIdx + 1] = vertIdx + 3;
                    triangles[triangleIdx + 2] = vertIdx + 2;

                    triangles[triangleIdx + 3] = vertIdx + 0;
                    triangles[triangleIdx + 4] = vertIdx + 2;
                    triangles[triangleIdx + 5] = vertIdx + 1;

                    ++tileIdx;
                }
            }

            for (int i = 0; i < 4; ++i) {
                normals[i] = normal;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uv;

            meshFilter.mesh = mesh;
            transform.position = origin;
        }

        public void SetRect(Vector3 bottomLeft, Vector3 topRight) {
            Vector3 diff = topRight - bottomLeft;
            size.Set(Mathf.Abs(diff.x), Mathf.Abs(diff.z));
            origin = bottomLeft + (diff * 0.5f);
            SetAsDirty();
        }

        public void SetTilingSize(Vector2Int _tileSize) {
            SetTilingSize(_tileSize.x, _tileSize.y);
        }

        public void SetTilingSize(int sizeX, int sizeY) {
            tileSize.Set(sizeX, sizeY);
            SetAsDirty();
        }

        public void SetMaterial(Material material) {
            Requires.NotNull(material);
            Requires.NotNull(meshRenderer);
            meshRenderer.sharedMaterial = new Material(material);
        }

        public void SetAsDirty() {
            isDirty = true;
        }

        public void SetColor(Color color) {
            Requires.NotNull(meshRenderer);
            Material material = meshRenderer.material;

            Requires.NotNull(material);
            material.color = color;
        }

        public void Hide() {
            gameObject.SetActive(false);
            SetRect(Vector3.zero, Vector3.zero);
        }

        public void Show() {
            SetRect(Vector3.zero, Vector3.zero);
            gameObject.SetActive(true);
        }
    }

}