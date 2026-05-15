using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawn books, trash, and dust randomly in a classroom
/// while preventing overlap.
/// </summary>
public class ClassroomSpawner : MonoBehaviour
{
    #region INSPECTOR

    [Header("=== PREFABS ===")]
    public GameObject[] bookPrefabs;
    public GameObject[] trashPrefabs;
    public GameObject[] dustPrefabs;

    [Header("=== TABLE SETTINGS ===")]
    public string tableTag = "Table";
    public int tablesToPopulate = -1;

    [Header("=== COUNTS ===")]
    [Range(1, 50)] public int totalTrashCount = 15;
    [Range(1, 20)] public int totalDustCount = 10;

    [Header("=== FLOOR SETTINGS ===")]
    public LayerMask floorLayer;
    public Vector2 floorSpawnArea = new Vector2(8f, 6f);

    [Header("=== POSITION SETTINGS ===")]
    public float minSeparation = 0.35f;
    public float surfaceOffset = 0.02f;
    public int maxPlacementAttempts = 30;

    [Header("=== BOOK ROTATION ===")]
    public float rotationX = 0f;
    public float rotationZ = 90f;

    #endregion

    // Track occupied positions
    private readonly List<Vector3> occupiedPositions = new();

    // =========================================================

    void Start()
    {
        SpawnBooks();
        SpawnTrash();
        SpawnDust();
    }

    // =========================================================
    #region BOOKS

    void SpawnBooks()
    {
        if (bookPrefabs == null || bookPrefabs.Length == 0)
        {
            Debug.LogWarning("[Spawner] No book prefabs assigned.");
            return;
        }

        GameObject[] tables = GameObject.FindGameObjectsWithTag(tableTag);

        if (tables.Length == 0)
        {
            Debug.LogWarning($"[Spawner] No tables found with tag '{tableTag}'.");
            return;
        }

        int spawnCount = tablesToPopulate < 0
            ? tables.Length
            : Mathf.Min(tablesToPopulate, tables.Length);

        List<GameObject> selectedTables = PickRandom(tables, spawnCount);

        int success = 0;

        foreach (GameObject table in selectedTables)
        {
            if (TrySpawnBook(table))
                success++;
        }

        Debug.Log($"[Spawner] Spawned {success}/{spawnCount} books.");
    }

    bool TrySpawnBook(GameObject table)
    {
        Bounds bounds = GetBounds(table);

        float marginX = bounds.size.x * 0.1f;
        float marginZ = bounds.size.z * 0.1f;

        for (int i = 0; i < maxPlacementAttempts; i++)
        {
            float x = Random.Range(bounds.min.x + marginX, bounds.max.x - marginX);
            float z = Random.Range(bounds.min.z + marginZ, bounds.max.z - marginZ);

            Vector3 pos = new(
                x,
                bounds.max.y + surfaceOffset,
                z
            );

            if (!IsPositionFree(pos))
                continue;

            GameObject prefab = GetRandom(bookPrefabs);

            Quaternion rot = Quaternion.Euler(
                rotationX,
                Random.Range(0f, 360f),
                rotationZ
            );

            GameObject book = Instantiate(prefab, pos, rot);

            book.name = $"Book_{prefab.name}";
            book.tag = "Book";

            occupiedPositions.Add(pos);
            return true;
        }

        return false;
    }

    #endregion

    // =========================================================
    #region TRASH

    void SpawnTrash()
    {
        if (trashPrefabs == null || trashPrefabs.Length == 0)
        {
            Debug.LogWarning("[Spawner] No trash prefabs assigned.");
            return;
        }

        int spawned = 0;

        for (int i = 0; i < totalTrashCount * maxPlacementAttempts; i++)
        {
            if (spawned >= totalTrashCount)
                break;

            Vector3 randomPos = transform.position + new Vector3(
                Random.Range(-floorSpawnArea.x / 2f, floorSpawnArea.x / 2f),
                2f,
                Random.Range(-floorSpawnArea.y / 2f, floorSpawnArea.y / 2f)
            );

            if (!Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, 10f, floorLayer))
                continue;

            Vector3 spawnPos = hit.point + Vector3.up * surfaceOffset;

            if (!IsPositionFree(spawnPos))
                continue;

            Quaternion rot = Quaternion.Euler(
                Random.Range(-15f, 15f),
                Random.Range(0f, 360f),
                Random.Range(-15f, 15f)
            );

            GameObject trash = Instantiate(
                GetRandom(trashPrefabs),
                spawnPos,
                rot
            );

            trash.tag = "Trash";

            occupiedPositions.Add(spawnPos);
            spawned++;
        }

        Debug.Log($"[Spawner] Spawned {spawned}/{totalTrashCount} trash.");
    }

    #endregion

    // =========================================================
    #region DUST

    void SpawnDust()
    {
        if (dustPrefabs == null || dustPrefabs.Length == 0)
        {
            Debug.LogWarning("[Spawner] Dust prefabs kosong.");
            return;
        }

        // 1. OTOMATIS CARI SEMUA MEJA PAKAI TAG (Biar nggak usah manual)
        GameObject[] tables = GameObject.FindGameObjectsWithTag(tableTag);

        if (tables.Length == 0)
        {
            Debug.LogWarning($"[Spawner] Tidak ada meja dengan tag '{tableTag}' untuk debu.");
            return;
        }

        int spawned = 0;

        for (int i = 0; i < totalDustCount * maxPlacementAttempts; i++)
        {
            if (spawned >= totalDustCount)
                break;

            // Pilih meja acak
            GameObject randomTable = GetRandom(tables);
            Bounds bounds = GetBounds(randomTable);

            // Beri sedikit margin agar debu tidak *spawn* mengambang di luar tepi meja
            float marginX = bounds.size.x * 0.1f;
            float marginZ = bounds.size.z * 0.1f;

            float x = Random.Range(bounds.min.x + marginX, bounds.max.x - marginX);
            float z = Random.Range(bounds.min.z + marginZ, bounds.max.z - marginZ);

            Vector3 pos = new(
                x,
                bounds.max.y + surfaceOffset,
                z
            );

            if (!IsPositionFree(pos))
                continue;

            // 2. LOGIKA ROTASI: Menempel rata di meja + Putaran acak
            Quaternion rot = Quaternion.identity;
            
            // Tembak raycast ke bawah untuk mendeteksi permukaan kayu meja
            if (Physics.Raycast(pos + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 1f))
            {
                // Bikin debunya mengikuti kemiringan permukaan meja
                rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            }

            // --- BAGIAN INI YANG DIUBAH ---
            // Ubah sumbu X menjadi 90f agar Quad tiduran, sumbu Y tetap acak (0-360)
            rot *= Quaternion.Euler(90f, Random.Range(0f, 360f), 0f);
            // ------------------------------

            // Spawn Debu
            GameObject prefabAcak = GetRandom(dustPrefabs);
            GameObject dust = Instantiate(prefabAcak, pos, rot);
            dust.tag = "Dust";

            // 3. LOGIKA SKALA: Bikin ukuran debunya beda-beda (0.5x sampai 1.5x dari ukuran asli)
            float randomScale = Random.Range(0.5f, 1.5f);
            dust.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            occupiedPositions.Add(pos);
            spawned++;
        }

        Debug.Log($"[Spawner] Berhasil menaruh {spawned}/{totalDustCount} debu di atas meja.");
    }
    #endregion

    // =========================================================
    #region HELPERS

    bool IsPositionFree(Vector3 candidate)
    {
        foreach (Vector3 used in occupiedPositions)
        {
            if (Vector3.Distance(candidate, used) < minSeparation)
                return false;
        }

        return true;
    }

    Bounds GetBounds(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;

            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds;
        }

        Collider col = go.GetComponentInChildren<Collider>();

        if (col != null)
            return col.bounds;

        return new Bounds(go.transform.position, Vector3.one * 0.5f);
    }

    T GetRandom<T>(T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    List<GameObject> PickRandom(GameObject[] source, int count)
    {
        List<GameObject> pool = new(source);

        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        return pool.GetRange(0, Mathf.Min(count, pool.Count));
    }

    #endregion

    // =========================================================
    #region GIZMOS

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);

        Gizmos.DrawCube(
            transform.position + Vector3.up * 0.05f,
            new Vector3(floorSpawnArea.x, 0.1f, floorSpawnArea.y)
        );

        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(
            transform.position + Vector3.up * 0.05f,
            new Vector3(floorSpawnArea.x, 0.1f, floorSpawnArea.y)
        );
    }

    #endregion
}