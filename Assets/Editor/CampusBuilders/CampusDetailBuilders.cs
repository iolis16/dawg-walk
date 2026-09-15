using UnityEngine;
using UnityEditor;
using UnityEngine.ProBuilder;

namespace DawgWalk.CampusBuilders
{
    public static class CampusDetailBuilders
    {
        const string StoneMat = "Assets/Materials/Mat_Stone.mat";
        const string ConcreteMat = "Assets/Materials/Mat_Concrete.mat";
        const string RoofMat = "Assets/Materials/Mat_Roof.mat";
        const string GlassMat = "Assets/Materials/Mat_Glass.mat";
        const string StainedGlassMat = "Assets/Materials/Mat_StainedGlass.mat";

        static GameObject FreshGroup(string name)
        {
            var old = GameObject.Find(name);
            if (old != null) Object.DestroyImmediate(old);
            return new GameObject(name);
        }

        static GameObject Cube(Transform parent, string name, Vector3 pos, Vector3 scale, Material mat)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent);
            go.transform.position = pos;
            go.transform.localScale = scale;
            go.GetComponent<MeshRenderer>().sharedMaterial = mat;
            return go;
        }

        // ---------------- Odegaard Library: Brutalist, vertical concrete fins + recessed glass, flat roof ----------------
        public static void BuildOdegaard()
        {
            var concrete = AssetDatabase.LoadAssetAtPath<Material>(ConcreteMat);
            var glass = AssetDatabase.LoadAssetAtPath<Material>(GlassMat);
            var roof = AssetDatabase.LoadAssetAtPath<Material>(RoofMat);

            var group = FreshGroup("Odegaard_Details");

            // North face (facing Red Square, Z = -50) fin + glass rhythm
            const float faceZ = -50f;
            float[] finXs = { -28f, -21f, -14f, -7f, 0f, 7f, 14f, 21f, 28f };
            foreach (var x in finXs)
                Cube(group.transform, "Odegaard_Fin", new Vector3(x, 6f, faceZ + 0.3f), new Vector3(1f, 12f, 1f), concrete);

            for (int i = 0; i < finXs.Length - 1; i++)
            {
                float midX = (finXs[i] + finXs[i + 1]) * 0.5f;
                Cube(group.transform, "Odegaard_Glass", new Vector3(midX, 6.5f, faceZ - 0.2f), new Vector3(5f, 10f, 0.6f), glass);
            }

            // Projecting central volume (flat-roofed)
            Cube(group.transform, "Odegaard_CentralBlock", new Vector3(0f, 8f, -47f), new Vector3(16f, 4f, 8f), concrete);

            // Flat roof overhang + entrance canopy
            Cube(group.transform, "Odegaard_RoofOverhang", new Vector3(0f, 12.4f, -60f), new Vector3(64f, 0.8f, 22f), roof);
            Cube(group.transform, "Odegaard_Canopy", new Vector3(0f, 9f, -44f), new Vector3(16f, 0.6f, 4f), roof);

            Debug.Log("[CampusDetailBuilders] Odegaard build complete.");
        }

        // ---------------- Kane Hall: Brutalist, heavy concrete piers/loggia, tall window band, flat roof ----------------
        public static void BuildKane()
        {
            var concrete = AssetDatabase.LoadAssetAtPath<Material>(ConcreteMat);
            var glass = AssetDatabase.LoadAssetAtPath<Material>(GlassMat);
            var roof = AssetDatabase.LoadAssetAtPath<Material>(RoofMat);

            var group = FreshGroup("Kane_Details");

            // West face (facing Red Square, X = 35): ground-level loggia of heavy piers
            const float faceX = 35f;
            float[] pierZs = { -16f, -8f, 0f, 8f, 16f };
            foreach (var z in pierZs)
                Cube(group.transform, "Kane_Pier", new Vector3(faceX - 0.5f, 3f, z), new Vector3(2f, 6f, 2f), concrete);

            // Tall window band above the loggia
            for (int i = 0; i < pierZs.Length - 1; i++)
            {
                float midZ = (pierZs[i] + pierZs[i + 1]) * 0.5f;
                Cube(group.transform, "Kane_Window", new Vector3(faceX - 0.3f, 10f, midZ), new Vector3(0.8f, 7f, 5f), glass);
            }

            Cube(group.transform, "Kane_RoofCap", new Vector3(45f, 15.5f, 0f), new Vector3(21f, 1f, 41f), roof);

            Debug.Log("[CampusDetailBuilders] Kane Hall build complete.");
        }

        // ---------------- Gerberding Hall: Collegiate Gothic (cast stone), pointed arches, gabled entrance ----------------
        public static void BuildGerberding()
        {
            var stone = AssetDatabase.LoadAssetAtPath<Material>(StoneMat);
            var roof = AssetDatabase.LoadAssetAtPath<Material>(RoofMat);
            var glass = AssetDatabase.LoadAssetAtPath<Material>(StainedGlassMat);

            var group = FreshGroup("Gerberding_Details");

            // East face (facing Red Square, X = -35): pointed-arch windows
            const float faceX = -35f;
            float[] windowZs = { -12f, -4f, 4f, 12f };
            const float winHeight = 6f;
            const float winWidth = 2f;
            foreach (var z in windowZs)
            {
                Cube(group.transform, "Gerberding_Window", new Vector3(faceX - 0.3f, 5f, z), new Vector3(1f, winHeight, winWidth), glass);

                float radius = winWidth * 0.5f;
                var arch = ShapeGenerator.GenerateArch(PivotLocation.Center, 180f, radius, radius, 1f, 8, true, true, true, true, true);
                arch.gameObject.name = "Gerberding_WindowArch";
                arch.GetComponent<MeshRenderer>().sharedMaterial = glass;
                arch.transform.SetParent(group.transform);
                // Arch's local X/Y plane becomes world Z/Y here, so rotate to align its span with Z
                arch.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                arch.transform.position = new Vector3(faceX - 0.3f - 0.5f, 5f + winHeight * 0.5f + radius * 0.5f, z);
                arch.ToMesh();
                arch.Refresh();
            }

            // Slim corner pinnacles echoing Suzzallo's Gothic language (smaller scale)
            (float x, float z)[] cornerXZ = { (-55f, 16f), (-55f, -16f) };
            foreach (var (x, z) in cornerXZ)
            {
                Cube(group.transform, "Gerberding_Turret", new Vector3(x, 17f, z), new Vector3(2.5f, 4f, 2.5f), stone);
                var spire = ShapeGenerator.GenerateCone(PivotLocation.Center, 1.4f, 3f, 6);
                spire.gameObject.name = "Gerberding_Spire";
                spire.GetComponent<MeshRenderer>().sharedMaterial = roof;
                spire.transform.SetParent(group.transform);
                spire.transform.position = new Vector3(x, 20.5f, z);
                spire.ToMesh();
                spire.Refresh();
            }

            // Gabled entrance porch (central, facing Red Square)
            Cube(group.transform, "Gerberding_EntryBlock", new Vector3(faceX + 2f, 9f, 0f), new Vector3(6f, 6f, 8f), stone);
            var gable = ShapeGenerator.GeneratePrism(PivotLocation.Center, new Vector3(8f, 3f, 9f));
            gable.gameObject.name = "Gerberding_EntryGable";
            gable.GetComponent<MeshRenderer>().sharedMaterial = roof;
            gable.transform.SetParent(group.transform);
            gable.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            gable.transform.position = new Vector3(faceX + 2f, 13.5f, 0f);
            gable.ToMesh();
            gable.Refresh();

            Cube(group.transform, "Gerberding_RoofCap", new Vector3(-45f, 15.5f, 0f), new Vector3(21f, 1f, 41f), roof);

            Debug.Log("[CampusDetailBuilders] Gerberding build complete.");
        }
    }
}
