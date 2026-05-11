using UnityEngine;
using System.Collections;

public class MessyChairPhysics : MonoBehaviour
{
    [Header("Pengaturan Jatuh")]
    [Tooltip("Seberapa tinggi kursi diangkat sebelum dijatuhkan")]
    public float liftHeight = 0.5f; 
    
    [Tooltip("Kekuatan putaran/bantingan biar posisinya acak")]
    public float tumbleForce = 15f; 
    
    [Tooltip("Berapa detik nunggu sampai kursi diam sebelum dibekukan")]
    public float settleTime = 2.5f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Rigidbody rb;

    void Start()
    {
        // [cite_start]// Simpan posisi rapi untuk fungsi Luruskan() [cite: 19]
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        rb = GetComponent<Rigidbody>();

        // Jalankan proses menjatuhkan kursi
        StartCoroutine(DropAndSettle());
    }

    IEnumerator DropAndSettle()
    {
        // 1. Pastikan fisika aktif dan bisa jatuh
        rb.isKinematic = false;
        rb.useGravity = true;

        // 2. Angkat sedikit dari lantai agar punya ruang untuk jatuh
        transform.position += new Vector3(0, liftHeight, 0);

        // 3. Kasih gaya putar (Torque) secara acak di semua sumbu (X, Y, Z)
        Vector3 randomTumble = new Vector3(
            Random.Range(-tumbleForce, tumbleForce),
            Random.Range(-tumbleForce, tumbleForce),
            Random.Range(-tumbleForce, tumbleForce)
        );
        rb.AddTorque(randomTumble, ForceMode.Impulse);

        // 4. Biarkan gravitasi bekerja dan tunggu beberapa detik sampai kursinya mapan (jatuh/nyender)
        yield return new WaitForSeconds(settleTime);

        // 5. Bekukan posisinya (Kinematic) agar pemain gak sengaja nendang kursinya saat jalan-jalan
        rb.isKinematic = true;
    }

    // [cite_start]// Panggil fungsi ini saat pemain tekan 'E' [cite: 19]
    public void Luruskan()
    {
        // Pastikan fisika tetap mati, lalu kembalikan ke posisi awal
        rb.isKinematic = true;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
}   