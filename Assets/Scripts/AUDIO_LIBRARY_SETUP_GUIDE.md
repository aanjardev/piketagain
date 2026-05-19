# 🎵 Audio Library Setup - Panduan Cepat

## ✅ Apa yang Sudah Dilakukan

Semua SFX kini **tersentralisasi** di **1 GameObject** saja! Anda tinggal assign file audio sekali.

---

## 🎯 Setup di Unity (3 Langkah Mudah)

### **Langkah 1: Buat GameObject "AudioLibrary"**

1. Buka scene `Gameplay.unity`
2. **Klik kanan** di Hierarchy → **Create Empty**
3. Rename menjadi `AudioLibrary`
4. Pastikan posisinya di **(0, 0, 0)** (opsional, tidak berpengaruh)

### **Langkah 2: Attach Script AudioLibrary**

1. Pilih GameObject `AudioLibrary`
2. Di Inspector → **Add Component** → Cari `AudioLibrary`
3. Attach script tersebut

### **Langkah 3: Assign Semua File Audio**

Di Inspector, di script `AudioLibrary`, Anda akan lihat section berikut:

```
=== SCHOOL & TIMER ===
  Bell School Sound: [ASSIGN FILE AUDIO BELL DI SINI]

=== BOOKS ===
  Book Pickup Sound: [ASSIGN FILE AUDIO PICKUP BUKU DI SINI]
  Book Shelve Sound: [ASSIGN FILE AUDIO SHELVE BUKU DI SINI]

=== TRASH ===
  Trash Pickup Sound: [ASSIGN FILE AUDIO PICKUP SAMPAH DI SINI]
  Trash Dispose Sound: [ASSIGN FILE AUDIO BUANG SAMPAH DI SINI]

=== CLEANING ===
  Dust Wipe Sound: [ASSIGN FILE AUDIO LAP DEBU DI SINI]

=== FURNITURE ===
  Furniture Slide Sound: [ASSIGN FILE AUDIO GESER FURNITURE DI SINI]
```

**Drag & drop file audio Anda ke masing-masing field!**

---

## 📋 Mapping SFX

| Field                     | Deskripsi                      | File Audio Yang Diperlukan    |
| ------------------------- | ------------------------------ | ----------------------------- |
| **Bell School Sound**     | Bunyi bell saat waktu habis    | Cari file bell sekolah Anda   |
| **Book Pickup Sound**     | Suara saat player ambil buku   | File SFX pickup book          |
| **Book Shelve Sound**     | Suara saat buku ditaruh di rak | File SFX shelve/put down book |
| **Trash Pickup Sound**    | Suara saat ambil sampah        | File SFX pickup trash         |
| **Trash Dispose Sound**   | Suara saat buang sampah        | File SFX dispose/throw trash  |
| **Dust Wipe Sound**       | Suara lap debu (looping)       | File SFX wipe/clean dust      |
| **Furniture Slide Sound** | Suara geser kursi/meja         | File SFX slide furniture      |

---

## 🔍 Verifikasi Setup

Setelah assign semua audio, **play scene** dan lakukan interaksi ini untuk test:

1. ✅ Tunggu sampai **timer habis** → Harus dengar **bell sound**
2. ✅ **Ambil buku** → Harus dengar **pickup sound**
3. ✅ **Taruh buku di rak** → Harus dengar **shelve sound**
4. ✅ **Ambil sampah** dengan kantong → Harus dengar **trash pickup sound**
5. ✅ **Buang sampah** ke tong → Harus dengar **trash dispose sound**
6. ✅ **Lap debu** → Harus dengar **dust wipe sound**
7. ✅ **Rapikan kursi/meja** → Harus dengar **slide sound**

---

## ⚙️ Technical Details

### Semua Script Sudah Diupdate:

- ✅ `Timer.cs`
- ✅ `BookItem.cs`
- ✅ `BookshelfSlot.cs`
- ✅ `TrashItem.cs`
- ✅ `TrashCan.cs`
- ✅ `DustItem.cs`
- ✅ `FurnitureTidy.cs`

### AudioLibrary Features:

- 🔄 **Singleton Pattern** → Hanya ada 1 instance di scene
- 🎯 **DontDestroyOnLoad** → Persist antar scene (jika diperlukan)
- 📍 **3D Spatial Audio** → Suara lebih keras dari dekat (via SoundManager)
- 🔊 **Respect SFX Volume** → Otomatis mengikuti pengaturan volume player

---

## 💡 Tips

- Semua file audio sebaiknya di-import ke folder **`Assets/SFX/`**
- Jika SFX tidak terdengar, cek:
  - Apakah file sudah ter-assign?
  - Apakah SFX Volume di settings game bukan 0%?
  - Apakah AudioListener ada di scene?
- Untuk mengubah volume SFX tertentu:
  - Edit file audio di Audacity/audio editor sebelum import
  - Atau adjust di AudioClip inspector properties

---

## 🎬 Contoh Scene Setup

```
Gameplay Scene Hierarchy:
├── Canvas
├── Player
├── Classroom
│   ├── Books
│   ├── Bookshelf
│   ├── Desks
│   └── ...
└── AudioLibrary ← TAMBAHKAN DI SINI
    └── [Script: AudioLibrary]
        ├── bellSchoolSound
        ├── bookPickupSound
        ├── bookShelveSound
        ├── trashPickupSound
        ├── trashDisposeSound
        ├── dustWipeSound
        └── furnitureSlideSound
```

---

**Selesai! Cukup itu saja, Anda tidak perlu edit script lagi.** 🎉
