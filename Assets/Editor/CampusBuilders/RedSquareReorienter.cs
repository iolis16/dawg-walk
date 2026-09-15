using UnityEngine;

namespace DawgWalk.CampusBuilders
{
    public static class RedSquareReorienter
    {
        // Real UW Red Square: Suzzallo = east, Kane = north, Gerberding = south,
        // Odegaard = northwest (approximated here as west, since our plaza only
        // models four cardinal edges). Each entry rotates that building's base
        // block + its detail group rigidly around Red Square's center (world
        // origin) by the given Y angle, which simultaneously relocates it to the
        // correct side AND reorients its facade to keep facing the plaza.
        static readonly (string baseName, string detailsName, float angle)[] Moves =
        {
            ("Suzzallo_Library", "Suzzallo_Details", 90f),
            ("Odegaard_Library", "Odegaard_Details", 90f),
            ("Kane_Hall", "Kane_Details", -90f),
            ("Gerberding_Hall", "Gerberding_Details", -90f),
        };

        public static void Reorient()
        {
            foreach (var (baseName, detailsName, angle) in Moves)
            {
                var baseObj = GameObject.Find(baseName);
                var detailsObj = GameObject.Find(detailsName);
                if (baseObj == null || detailsObj == null)
                {
                    Debug.LogWarning($"[RedSquareReorienter] Missing {baseName} or {detailsName}, skipping.");
                    continue;
                }

                var pivot = new GameObject("TempPivot_" + baseName);
                pivot.transform.position = Vector3.zero;
                pivot.transform.rotation = Quaternion.identity;

                baseObj.transform.SetParent(pivot.transform, true);
                detailsObj.transform.SetParent(pivot.transform, true);

                pivot.transform.Rotate(Vector3.up, angle, Space.World);

                baseObj.transform.SetParent(null, true);
                detailsObj.transform.SetParent(null, true);

                Object.DestroyImmediate(pivot);
            }

            Debug.Log("[RedSquareReorienter] Reorient complete.");
        }
    }
}
