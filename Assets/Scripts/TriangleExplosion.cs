using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriangleExplosion : MonoBehaviour {

    public int fragmentSize = 12;

    public IEnumerator SplitMesh(bool destroy, float explodeAmount) {

        if (GetComponent<MeshFilter>() == null || GetComponent<SkinnedMeshRenderer>() == null) {
            yield return null;
        }

        if (GetComponent<Collider>()) {
            GetComponent<Collider>().enabled = false;
        }

        Mesh M = new Mesh();
        if (GetComponent<MeshFilter>()) {
            M = GetComponent<MeshFilter>().mesh;
        } else if (GetComponent<SkinnedMeshRenderer>()) {
            M = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        }

        Material[] materials = new Material[0];
        if (GetComponent<MeshRenderer>()) {
            materials = GetComponent<MeshRenderer>().materials;
        } else if (GetComponent<SkinnedMeshRenderer>()) {
            materials = GetComponent<SkinnedMeshRenderer>().materials;
        }

        Vector3[] verts = M.vertices;
        Vector3[] normals = M.normals;
        Vector2[] uvs = M.uv;
        for (int submesh = 0; submesh < M.subMeshCount; submesh++) {

            int[] indices = M.GetTriangles(submesh);

            int sz = fragmentSize;
               
            for (int i = 0; i < indices.Length; i += 3 * sz) {
                Vector3[] newVerts = new Vector3[3 * sz];
                Vector3[] newNormals = new Vector3[3 * sz];
                Vector2[] newUvs = new Vector2[3 * sz];
                for (int n = 0; n < 3 * sz; n++) {
                    int index = indices[i + n];
                    newVerts[n] = verts[index];
                    newUvs[n] = uvs[index];
                    newNormals[n] = normals[index];
                }

                Mesh mesh = new Mesh();
                mesh.vertices = newVerts;
                mesh.normals = newNormals;
                mesh.uv = newUvs;

                //mesh.triangles = new int[6]; //new int[] { 0, 1, 2, 2, 1, 0 };

                List<int> _tri = new List<int>(6 * sz);
                for (int j = 0; j < 3 * sz; ++j) {
                    _tri.Add(j);
                }

                for (int j = 3 * sz - 1; j >= 0; --j) {
                    _tri.Add(j);
                }

                /*mesh.triangles[0] = 0;
                mesh.triangles[1] = 1;
                mesh.triangles[2] = 2;
                mesh.triangles[3] = 2;
                mesh.triangles[4] = 1;
                mesh.triangles[5] = 0;*/
                mesh.triangles = _tri.ToArray();

                GameObject GO = new GameObject("Triangle " + (i / 3));
                //GO.layer = LayerMask.NameToLayer("Particle");
                GO.transform.position = transform.position;
                GO.transform.rotation = transform.rotation;
                GO.AddComponent<MeshRenderer>().material = materials[submesh];
                GO.AddComponent<MeshFilter>().mesh = mesh;
                GO.AddComponent<BoxCollider>();
                Vector3 explosionPos = new Vector3(transform.position.x + Random.Range(-0.5f, 0.5f), transform.position.y + Random.Range(0f, 0.5f), transform.position.z + Random.Range(-0.5f, 0.5f));
                GO.AddComponent<Rigidbody>().AddExplosionForce(explodeAmount * Random.Range(300, 500), explosionPos, 5);
                Destroy(GO, 5 + Random.Range(0.0f, 5.0f));
            }
        }

        GetComponent<Renderer>().enabled = false;

        yield return new WaitForSeconds(1.0f);
        if (destroy == true) {
            Destroy(gameObject);
        }

    }


}