using UnityEngine;
using System.Collections.Generic;

public class ClassroomMessManager : MonoBehaviour
{
    [Header("--- Target Objek ---")]
    [Tooltip("Masukkan semua objek meja yang ingin diacak ke sini.")]
    public DeskMessy[] allDesks;
    
    [Tooltip("Masukkan semua objek kursi yang ingin diacak ke sini.")]
    public ChairMessy[] allChairs;

    [Header("--- Pengaturan Jumlah Berantakan ---")]
    [Range(0, 50)] public int totalMessyDesks = 5;
    [Range(0, 50)] public int totalMessyChairs = 8;

    void Start()
    {
        // 1. Acak Meja
        ShuffleAndMessUpDesks();
        
        // 2. Acak Kursi
        ShuffleAndMessUpChairs();
    }

    void ShuffleAndMessUpDesks()
    {
        if (allDesks == null || allDesks.Length == 0) return;

        // Bikin list sementara dari array
        List<DeskMessy> deskList = new List<DeskMessy>(allDesks);
        
        // Batasi jumlah maksimal supaya tidak lebih dari total meja yang ada
        int amountToMess = Mathf.Min(totalMessyDesks, deskList.Count);

        for (int i = 0; i < amountToMess; i++)
        {
            // Ambil satu meja secara acak
            int randomIndex = Random.Range(0, deskList.Count);
            DeskMessy selectedDesk = deskList[randomIndex];
            
            // Suruh meja tersebut berantakan
            selectedDesk.MakeItMessy();
            
            // Hapus dari list supaya tidak terpilih 2 kali
            deskList.RemoveAt(randomIndex);
        }
    }

    void ShuffleAndMessUpChairs()
    {
        if (allChairs == null || allChairs.Length == 0) return;

        List<ChairMessy> chairList = new List<ChairMessy>(allChairs);
        int amountToMess = Mathf.Min(totalMessyChairs, chairList.Count);

        for (int i = 0; i < amountToMess; i++)
        {
            int randomIndex = Random.Range(0, chairList.Count);
            ChairMessy selectedChair = chairList[randomIndex];
            
            selectedChair.DropAndSettle();
            chairList.RemoveAt(randomIndex);
        }
    }
}