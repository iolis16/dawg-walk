using UnityEngine;
using UnityEditor;
using UnityEngine.ProBuilder;

namespace DawgWalk.CampusBuilders
{
    public static class SuzzalloBuilder
    {
        const string StoneMat = "Assets/Materials/Mat_Stone.mat";
        const string RoofMat = "Assets/Materials/Mat_Roof.mat";
        const string GlassMat = "Assets/Materials/Mat_StainedGlass.mat";

        public static void Build()
        {
            var stone = AssetDatabase.LoadAssetAtPath<Material>(StoneMat);
            var roof = AssetDatabase.LoadAssetAtPath<Material>(RoofMat);
            var glass = AssetDatabase.LoadAssetAtPath<Material>(GlassMat);

            var oldGroup = GameObject.Find("Suzzallo_Details");
            if (oldGroup != null)
                Object.DestroyImmediate(oldGroup);

            var group = new GameObject("Suzzallo_Details");

            // 11 pointed-arch windows across the south facade (real Suzzallo has 11 x 36ft stained-glass windows)
            float[] windowXs = { -34f, -27.2f, -20.4f, -13.6f, -6.8f, 0f, 6.8f, 13.6f, 20.4f, 27.2f, 34f };
            const float windowWidth = 3f;
            const float windowHeight = 11f;
            const float windowBaseY = 3f;
            const float facadeZ = 60f;

            foreach (var x in windowXs)
            {
                var shaft = GameObject.CreatePrimitive(PrimitiveType.Cube);
                shaft.name = "Suzzallo_Window";
                shaft.transform.SetParent(group.transform);
                shaft.transform.position = new Vector3(x, windowBaseY + windowHeight * 0.5f, facadeZ - 0.3f);
                shaft.transform.localScale = new Vector3(windowWidth, windowHeight, 1f);
                shaft.GetComponent<MeshRenderer>().sharedMaterial = glass;

                float radius = windowWidth * 0.5f;
                var arch = ShapeGenerator.GenerateArch(PivotLocation.Center, 180f, radius, radius, 1f, 8, true, true, true, true, true);
                arch.gameObject.name = "Suzzallo_WindowArch";
                arch.GetComponent<MeshRenderer>().sharedMaterial = glass;
                arch.transform.SetParent(group.transform);
                arch.transform.position = new Vector3(x, windowBaseY + windowHeight + radius * 0.5f, facadeZ - 0.3f - 0.5f);
                arch.ToMesh();
                arch.Refresh();
            }

            // 12 buttress piers between/flanking the windows, each topped with a pinnacle
            float[] buttressXs = { -37.4f, -30.6f, -23.8f, -17f, -10.2f, -3.4f, 3.4f, 10.2f, 17f, 23.8f, 30.6f, 37.4f };
            const float buttressTopY = 22f;

            foreach (var x in buttressXs)
            {
                var pier = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pier.name = "Suzzallo_Buttress";
                pier.transform.SetParent(group.transform);
                pier.transform.position = new Vector3(x, buttressTopY * 0.5f, facadeZ + 0.4f);
                pier.transform.localScale = new Vector3(2f, buttressTopY, 2f);
                pier.GetComponent<MeshRenderer>().sharedMaterial = stone;

                var pinnacle = ShapeGenerator.GenerateCone(PivotLocation.Center, 1.2f, 3.5f, 6);
                pinnacle.gameObject.name = "Suzzallo_Pinnacle";
                pinnacle.GetComponent<MeshRenderer>().sharedMaterial = roof;
                pinnacle.transform.SetParent(group.transform);
                pinnacle.transform.position = new Vector3(x, buttressTopY + 1.75f, facadeZ + 0.4f);
                pinnacle.ToMesh();
                pinnacle.Refresh();
            }

            // Corner tower spires (replace flat caps with tapering cones)
            Vector3[] towerTops =
            {
                new Vector3(-36f, 35f, 86f), new Vector3(36f, 35f, 86f),
                new Vector3(-36f, 35f, 64f), new Vector3(36f, 35f, 64f),
            };
            foreach (var top in towerTops)
            {
                var spire = ShapeGenerator.GenerateCone(PivotLocation.Center, 2f, 6f, 8);
                spire.gameObject.name = "Suzzallo_TowerSpire";
                spire.GetComponent<MeshRenderer>().sharedMaterial = roof;
                spire.transform.SetParent(group.transform);
                spire.transform.position = top + new Vector3(0f, 3f, 0f);
                spire.ToMesh();
                spire.Refresh();
            }

            // Central nave (raised wall block the gable roof sits on)
            var nave = GameObject.CreatePrimitive(PrimitiveType.Cube);
            nave.name = "Suzzallo_Nave";
            nave.transform.SetParent(group.transform);
            nave.transform.position = new Vector3(0f, 29f, 75f);
            nave.transform.localScale = new Vector3(20f, 8f, 30f);
            nave.GetComponent<MeshRenderer>().sharedMaterial = stone;

            // Central nave gable roof
            var gable = ShapeGenerator.GeneratePrism(PivotLocation.Center, new Vector3(22f, 6f, 32f));
            gable.gameObject.name = "Suzzallo_NaveGable";
            gable.GetComponent<MeshRenderer>().sharedMaterial = roof;
            gable.transform.SetParent(group.transform);
            gable.transform.position = new Vector3(0f, 33f, 75f);
            gable.ToMesh();
            gable.Refresh();

            // Entrance steps facing Red Square
            var stairs = ShapeGenerator.GenerateStair(PivotLocation.Center, new Vector3(14f, 3f, 6f), 6, true);
            stairs.gameObject.name = "Suzzallo_EntranceStairs";
            stairs.GetComponent<MeshRenderer>().sharedMaterial = stone;
            stairs.transform.SetParent(group.transform);
            stairs.transform.position = new Vector3(0f, 1.5f, 56f);
            stairs.ToMesh();
            stairs.Refresh();

            Debug.Log("[SuzzalloBuilder] Build complete.");
        }
    }
}
