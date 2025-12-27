

/*3 Byte-Şelalesi Sensörü – Endian Savaşları ve Ani Sel
Keban Barajı’nın gövdesine yeni nesil “Byte-Şelalesi” su-seviyesi sensörleri yerleştirildi.
 Kurulum günü: Mühendisler, vinçle 60 m yüksekliğe çıkıp sensörü beton perdeye 
çiviledi.
 Sensör, Big-Endian 16-bit sayıları LoRa-WAN üstünden beş saniyede bir 
gönderiyor.
 Bulut tarafında eski C# servisi, “Hele bir paket gelsin, ben Buffer.GetByte ile 
çözerim” diyerek Little-Endian varsaydı.
 Gövde üzerinde gece yarısı fırtına kopunca sensör, 01 : 17’de 0x66 0x00 (Big
Endian: 26 112 cm) paketledi; servis bunu 102 cm sanıp “seviyenin altında” diye 
alarmı susturdu.
 Kapaklar otomatik moddaydı — misreading yüzünden üç kapak tam açıldı; baraj 
altındaki köy hoparlörlerinden “Olası ani sel!” uyarısı verildi, vatandaş traktörle 
evi terk etti.
 Ertesi sabah medyada manşet: “Sensör Byte’ı Yanlış Yiyince Köy Uyandı!”
Şirket, mühendis ekibine bir gecede hot-fix yazma görevi verdi.
Kodlama Görevleri 
1. static short ReadInt16BigAware(byte high, byte low) – host endian’ı test edip 
doğru short döndürsün.
2. short[] ReadPacket(byte[] packet) – dizideki ardışık ikili baytların hepsini 
ReadInt16BigAware ile çözsün.
3. SensorSimulator – rastgele doğru seviyeler üretip sunucuya göndererek konsola 
her cm değerini doğru yazsın.
*/





using System;
using System.Collections.Generic;
using System.Threading;

namespace ByteSelalesiSensoru
{
    class Program
    {
        // 1) Host endian'ını test edip doğru short döndüren fonksiyon
        static short ReadInt16BigAware(byte high, byte low)
        {
            byte[] b = new byte[2];

            // BitConverter, host little-endian ise byte sırası little olmalı
            if (BitConverter.IsLittleEndian)
            {
                // Big-endian gelen (high,low) -> little-endian diziliş (low,high)
                b[0] = low;
                b[1] = high;
            }
            else
            {
                // Host zaten big-endian ise direkt
                b[0] = high;
                b[1] = low;
            }

            return BitConverter.ToInt16(b, 0);
        }

        // 2) Packet içindeki ardışık ikili baytları çöz
        static short[] ReadPacket(byte[] packet)
        {
            if (packet == null || packet.Length == 0)
                return new short[0];

            // tek sayıda byte varsa son byte çözülemez
            int pairCount = packet.Length / 2;

            short[] values = new short[pairCount];

            int index = 0;
            for (int i = 0; i < pairCount; i++)
            {
                byte high = packet[index];
                byte low = packet[index + 1];

                values[i] = ReadInt16BigAware(high, low);

                index += 2;
            }

            return values;
        }

        static void Main(string[] args)
        {
            // Örnek: 0x66 0x00 big-endian -> 26112
            byte[] sample = new byte[] { 0x66, 0x00 };
            short[] solved = ReadPacket(sample);
            Console.WriteLine("Ornek Paket: 0x66 0x00 => " + solved[0] + " cm");

            Console.WriteLine("\n--- Sensor Simulator Basliyor ---\n");

            SensorSimulator sim = new SensorSimulator();
            sim.Run(10); // 10 ölçüm üret ve yazdır

            Console.WriteLine("\nBitti. Enter...");
            Console.ReadLine();
        }
    }

    // 3) SensorSimulator: doğru seviyeler üretip Big-Endian paket gönderir,
    // sunucu tarafı ReadPacket ile çözer ve doğru cm yazar
    class SensorSimulator
    {
        private Random rnd = new Random();

        public void Run(int count)
        {
            for (int i = 0; i < count; i++)
            {
                // 0..30000 cm arası (short sınırını aşmamak için)
                short realCm = (short)rnd.Next(0, 30001);

                // Sensör Big-Endian paketler
                byte[] packet = BuildBigEndianPacket(realCm);

                // Sunucu paketi çözer (Program.ReadPacket ile)
                short decoded = DecodeOnServer(packet);

                Console.WriteLine("Gercek: " + realCm + " cm"
                    + " | Paket: " + ToHex(packet)
                    + " | Cozulen: " + decoded + " cm");

                Thread.Sleep(200); // görüntü için küçük bekleme
            }
        }

        // Sensör tarafı: 1 adet 16-bit değeri Big-Endian byte dizisine çevirir
        private byte[] BuildBigEndianPacket(short value)
        {
            // unsigned gibi davranmak için ushort'a çeviriyoruz
            ushort u = (ushort)value;

            byte high = (byte)(u >> 8);
            byte low = (byte)(u & 0xFF);

            return new byte[] { high, low };
        }

        // Sunucu tarafı: gelen packet'i çözer
        private short DecodeOnServer(byte[] packet)
        {
            // Program.ReadPacket static ama başka class'tan çağırmak için
            // ya public yapman gerekir ya da aynı class içine taşırsın.
            // Öğrenci işi basit olsun diye burada kopya çözüm yaptım:

            if (packet == null || packet.Length < 2)
                return 0;

            byte high = packet[0];
            byte low = packet[1];

            byte[] b = new byte[2];

            if (BitConverter.IsLittleEndian)
            {
                b[0] = low;
                b[1] = high;
            }
            else
            {
                b[0] = high;
                b[1] = low;
            }

            return BitConverter.ToInt16(b, 0);
        }

        private string ToHex(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return "";

            // "66 00" gibi yazdır
            List<string> parts = new List<string>();
            for (int i = 0; i < bytes.Length; i++)
                parts.Add(bytes[i].ToString("X2"));

            return string.Join(" ", parts);
        }
    }
}
