# 🔊 SFX Allocation Guide

Dokumentasi lengkap untuk memasukkan file Sound Effect ke setiap interaksi player.

## Daftar SFX yang Diperlukan

| No | Interaksi | Script | Nama Field | Deskripsi |
|----|-----------|--------|-----------|-----------|
| 1 | **Bell Sekolah** | `Timer.cs` | `bellSound` | Bunyi bell saat waktu habis/game over |
| 2 | **Pick Up Buku** | `BookItem.cs` | `pickupSound` | Suara saat player mengambil/mengangkat buku |
| 3 | **Menaruh Buku di Rak** | `BookshelfSlot.cs` | `shelveSound` | Suara saat player menaruh buku di rak |
| 4 | **Pick Up Sampah** | `TrashItem.cs` | `pickupSound` | Suara saat sampah dimasukkan ke kantong sampah |
| 5 | **Buang Sampah** | `TrashCan.cs` | `emptyBagSound` | Suara saat sampah dari kantong dibuang ke tong |
| 6 | **Bersihkan Debu** | `DustItem.cs` | `wipeSound` | Suara lap/gosok saat membersihkan debu |
| 7 | **Pindahkan Meja/Kursi** | `FurnitureTidy.cs` | `slideSound` | Suara gesek/dorong saat merapikan furniture |

## Cara Mengalokasikan SFX

### Langkah 1: Buka Scene di Unity
Buka scene `Gameplay.unity` dan masuk ke Game view.

### Langkah 2: Cari Object yang Sesuai

#### 1️⃣ Timer (Bell Sekolah)
- Cari object bernama `Timer` di Hierarchy (biasanya ada di Canvas atau sebagai GameObject terpisah)
- Di Inspector, cari komponen `Timer` script
- Assign file audio bell ke field `Bell Sound`

#### 2️⃣ BookItem (Pick Up Buku)
- Cari salah satu Book prefab di scene atau di folder `Assets/Prefabs/`
- Expand prefab dan lihat GameObject dengan script `BookItem`
- Di Inspector → Script `BookItem` → field `Pickup Sound`
- Assign file audio ke field tersebut
- **Catatan:** Jika menggunakan Prefab, editing prefab akan berlaku ke semua instance

#### 3️⃣ BookshelfSlot (Menaruh Buku)
- Cari Bookshelf object di scene
- Lihat child objects bernama seperti "Slot_1", "Slot_2", dll
- Setiap slot memiliki script `BookshelfSlot`
- Assign file audio ke field `Shelve Sound`

#### 4️⃣ TrashItem (Pick Up Sampah)
- Buka Scene atau cari TrashItem prefab di `Assets/Prefabs/`
- Di script `TrashItem`, ada field `Pickup Sound` (baris atas)
- Assign file audio untuk suara mengambil sampah

#### 5️⃣ TrashCan (Buang Sampah)
- Cari object bernama `TrashCan` atau tong sampah di scene
- Script `TrashCan` memiliki field `Empty Bag Sound`
- Assign file audio untuk suara buang sampah

#### 6️⃣ DustItem (Bersihkan Debu)
- Cari Dust objects di scene (biasanya ada di meja atau area tertentu)
- Script `DustItem` memiliki field `Wipe Sound` di bagian atas
- Assign file audio untuk suara pembersihan

#### 7️⃣ FurnitureTidy (Pindahkan Meja/Kursi)
- Cari Chair atau Desk objects yang perlu dibereskan
- Setiap memiliki script `FurnitureTidy`
- Field `Slide Sound` → Assign audio untuk gesek/dorong

## Tips & Best Practices

✅ **DO:**
- Gunakan file audio format `.mp3` atau `.wav` yang sudah di-import ke folder `Assets/SFX/`
- Pastikan volume audio sudah sesuai (sekitar -3 dB hingga -6 dB untuk SFX)
- Test dengan memutar scene dan berinteraksi dengan object

❌ **DON'T:**
- Jangan assign null/kosong - akan menyebabkan warning tapi game tetap berjalan
- Jangan menggunakan file BGM untuk SFX (akan terasa tidak sesuai)
- Jangan lupa menyimpan scene setelah assign SFX

## File SFX Anda Saat Ini

Lokasi: `Assets/SFX/`

File yang ada:
- `backgroundmusicforvideos-roblox-minecraft-fortnite-video-game-music-358426.mp3` (BGM)
- `doorknock.mp3`
- `doorknock1.mp3`
- `bgm_main.prefab`

**Anda perlu menambahkan file untuk:**
1. Bell sekolah
2. Pick up book
3. Shelve/drop book
4. Pick up trash
5. Empty trash/dispose
6. Wipe dust
7. Slide furniture

## Troubleshooting

**Q: Suara tidak terdengar?**
A: 
- Pastikan file audio sudah ter-import dengan benar di Unity
- Cek volumenya di AudioClip properties (Volume Envelope)
- Pastikan SFX Volume setting di game tidak di-mute (0%)

**Q: Suara terlalu keras/lemah?**
A: 
- Sesuaikan volume di field `volume` di script (defaultnya 1f = 100%)
- Edit file audio di Audacity atau software audio lain sebelum assign

**Q: Ingin menggunakan prefab existing untuk semua trash/books?**
A:
- Edit prefab di folder `Assets/Prefabs/`
- Assign SFX ke prefab tersebut
- Semua instance akan otomatis mendapat SFX

---

**Questions?** Cek semua script yang dimention di sini untuk melihat field dan keterangan lengkapnya.
