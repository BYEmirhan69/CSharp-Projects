/*
2 Deprem Erken-Uyarı Ağı – Dinamik Çok-Boyutlu Hafıza Tetrisi
Doğu Anadolu Fay Hattı’nda 3 × 5 ivmeölçer ızgarası 50 ms aralıkla yer sarsıntısı yayınlar. 
Kışın iki istasyon karla göçer, diğerleri “hot swap” edilip koordinatları değişir. Anlık veri 
haritası televizyona canlı akar; yanlış kopyalama 2017’deki gibi toplu SMS paniği 
doğurur. Yeni yazılım, ızgarayı veri kaybı olmadan genişletip daraltmalı, on-saniyede bir 
2 × 2 “yama” patch’ini de yerleştirmelidir; aksi hâlde Kandilli sunucuları çöküp sosyal 
medya “megadeprem” etiketine boğulur.
Kodlama Görevleri 
1. class GridBuffer<T> – ctor’da T[,] grid alacak.
2. void ResizePreserve(int newRows, int newCols) – eski veriyi koruyarak boyut 
değiştirin. Bir değişkenin içerisinde bir matrisin tüm satırları nasıl tutulur?
3. void ApplyPatch(int r,int c, T[,] patch) – yamayı verilen köşeden kopyalayın
*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_Deprem_Erken_Uyarı_Ağı___Dinamik_Çok_Boyutlu_Hafıza_Tetrisi
{
    // 1) GridBuffer<T> sınıfı
    class GridBuffer<T>
    {
        private T[,] grid;

        public GridBuffer(T[,] grid)
        {
            // null gelmesin diye basit kontrol
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));

            this.grid = grid;
        }

        public int Rows
        {
            get { return grid.GetLength(0); }
        }

        public int Cols
        {
            get { return grid.GetLength(1); }
        }

        public T[,] GetGrid()
        {
            return grid;
        }

        // 2) Eski veriyi koruyarak resize
        public void ResizePreserve(int newRows, int newCols)
        {
            if (newRows <= 0 || newCols <= 0)
                throw new ArgumentException("Yeni satır ve sütun sayısı 0'dan büyük olmalı.");

            T[,] newGrid = new T[newRows, newCols];

            int copyRows = Math.Min(Rows, newRows);
            int copyCols = Math.Min(Cols, newCols);

            for (int r = 0; r < copyRows; r++)
            {
                for (int c = 0; c < copyCols; c++)
                {
                    newGrid[r, c] = grid[r, c];
                }
            }

            grid = newGrid;
        }

        // 3) Patch'i belirtilen köşeden kopyala
        public void ApplyPatch(int r, int c, T[,] patch)
        {
            if (patch == null)
                throw new ArgumentNullException(nameof(patch));

            int patchRows = patch.GetLength(0);
            int patchCols = patch.GetLength(1);

            // sınır kontrol (taşma olmasın)
            if (r < 0 || c < 0 || r + patchRows > Rows || c + patchCols > Cols)
                throw new ArgumentException("Patch grid sınırlarını aşıyor.");

            for (int i = 0; i < patchRows; i++)
            {
                for (int j = 0; j < patchCols; j++)
                {
                    grid[r + i, c + j] = patch[i, j];
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Başlangıç: 3x5 grid (örnek veriler)
            int[,] start = new int[3, 5]
            {
                { 1,  2,  3,  4,  5 },
                { 6,  7,  8,  9, 10 },
                {11, 12, 13, 14, 15 }
            };

            GridBuffer<int> buffer = new GridBuffer<int>(start);

            Console.WriteLine("ILK GRID (3x5):");
            Yazdir(buffer.GetGrid());

            // Resize: büyüt (6x7)
            buffer.ResizePreserve(6, 7);
            Console.WriteLine("\nRESIZE SONRASI (6x7) - eski veri korunur:");
            Yazdir(buffer.GetGrid());

            // Patch: 2x2 yama
            int[,] patch = new int[2, 2]
            {
                { 99, 99 },
                { 99, 99 }
            };

            // Patch'i (1,2) noktasından uygula
            buffer.ApplyPatch(1, 2, patch);
            Console.WriteLine("\nPATCH SONRASI (r=1,c=2) 2x2 99 basildi:");
            Yazdir(buffer.GetGrid());

            // Resize: küçült (2x4)
            buffer.ResizePreserve(2, 4);
            Console.WriteLine("\nKUCULTME (2x4) - sigan kadar veri kalir:");
            Yazdir(buffer.GetGrid());

            Console.ReadLine();
        }

        static void Yazdir(int[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Console.Write(grid[r, c].ToString().PadLeft(4));
                }
                Console.WriteLine();
            }
        }
    }
}