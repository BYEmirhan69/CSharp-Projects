

/*
1.Kuzey Gözlem İstasyonu – Buzun ve Belleğin İnce Çizgisi
Doğu Anadolu’nun 3 190 m’lik zirvesindeki ahşap kulübede tek güç kaynağı, yelkovanı 
düşmüş bir rüzgâr türbinidir. İçeride yalnızca 512 MB RAM’e sahip, yıllanmış bir 
Raspberry Pi bulunur. –35 °C’ye kadar inen tipide, antene çarpan kuşlar yüzünden ADC 
bazen bit kaydırır, ölçümler NaN ya da +∞ olur. Pi, 10 s’de bir double[] rawData dizisini 
şişirir; SD kartı %97 doludur. Ertesi sabah akademik ekip kulübeye çıkamadığında hatalı 
veriler, şehirde “sıcaklık +98 °C” alarmı yaratır, valilik okulları tatil eder.
Görevler 
1. StationData adında bir sınıf tanımlayın.
o const double HumidityRatio = 0.62197
o readonly DateTime StartTime
o double[] RawData alanları içersin.
2. static double[] FilterCold(double[] src) fonksiyonunda LINQ kullanarak sadece –10 °C’den düşük değerleri döndürün.
3. public static double[] FilterInvalid(this double[] src) extension metodunu yazın;
NaN, PositiveInfinity, NegativeInfinity elemanları ayıklayıp yeni dizi döndürsün.
*/


using System;
using System.Linq;

namespace KuzeyGozlemIstasyonu
{
    // 1) StationData sınıfı
    class StationData
    {
        public const double HumidityRatio = 0.62197;

        public readonly DateTime StartTime;

        public double[] RawData;

        public StationData(double[] rawData)
        {
            StartTime = DateTime.Now;
            RawData = rawData;
        }
    }

    // 3) Extension method
    static class ArrayExtensions
    {
        public static double[] FilterInvalid(this double[] src)
        {
            if (src == null)
                return new double[0];

            // NaN ve Infinity olanları çıkar
            return src.Where(x =>
                !double.IsNaN(x) &&
                x != double.PositiveInfinity &&
                x != double.NegativeInfinity
            ).ToArray();
        }
    }

    class Program
    {
        // 2) LINQ ile -10'dan düşükleri seç
        static double[] FilterCold(double[] src)
        {
            if (src == null)
                return new double[0];

            return src.Where(x => x < -10).ToArray();
        }

        static void Main(string[] args)
        {
            double[] raw = new double[]
            {
                -35, -12, -9, 5,
                double.NaN,
                double.PositiveInfinity,
                double.NegativeInfinity,
                -25, 98
            };

            StationData station = new StationData(raw);

            Console.WriteLine("Baslangic: " + station.StartTime);
            Console.WriteLine("HumidityRatio: " + StationData.HumidityRatio);

            // önce bozukları temizle
            double[] temiz = station.RawData.FilterInvalid();

            // sonra soğukları al
            double[] soguklar = FilterCold(temiz);

            Console.WriteLine("\nHam Veri:");
            Yazdir(station.RawData);

            Console.WriteLine("\nTemiz Veri (NaN ve Infinity yok):");
            Yazdir(temiz);

            Console.WriteLine("\n-10'dan dusuk olanlar:");
            Yazdir(soguklar);

            Console.ReadLine();
        }

        static void Yazdir(double[] dizi)
        {
            if (dizi == null || dizi.Length == 0)
            {
                Console.WriteLine("Bos");
                return;
            }

            for (int i = 0; i < dizi.Length; i++)
            {
                double v = dizi[i];

                if (double.IsNaN(v))
                    Console.Write("NaN");
                else if (v == double.PositiveInfinity)
                    Console.Write("+Inf");
                else if (v == double.NegativeInfinity)
                    Console.Write("-Inf");
                else
                    Console.Write(v);

                if (i != dizi.Length - 1)
                    Console.Write(", ");
            }

            Console.WriteLine();
        }
    }
}
