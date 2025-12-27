/*
 * 
4. Görev Kuyruğu Sunucusu – “Async void”in Sessiz Çığlığı

E-ticaret devinin “CheckoutQueue” mikroservisi, indirim gecesinde dakikada 900 000 
“Sipariş Tamam” mesajı almaya başladı.
 Kod tabanı eskiydi; mesaj dinleyicisi şöyleydi:
async void OnMessageReceived(Order o) {
    await ProcessAsync(o);
}
 02 : 35’te AWS alarmı: CPU %100, hafıza %95; Hiç istisna log’a düşmemişti.
 SRE ekibi Stack Trace peşine düştü; meğerse async void içindeki 
NullReferenceException top-level hatayı yutuyordu.
 Ayrıca stok sayacı static int TotalOrdersProcessed idi: dört worker thread aynı 
hücreye ++ basarken sayaç 76 482 yerine 62 019 oldu.
 Üstüne üstlük bankanın “3D-Secure yavaşladı” dediği dakikalarda 5 s üzerinde 
süren 14 000 istek iptal olamadı, kullanıcı ekranı dönüp durdu.
 CEO sabaha karşı Slack’te: “Şafak sökmeden fix istiyorum, yoksa alışveriş durdu!”
Kodlama Görevleri 
1. async Task OnMessageAsync(Order o, CancellationToken ct) – try / catch içinde 
LogError(ex) çağrısı ile hatayı kaydeden dinleyici yazın.
2. static class OrderCounter – static int Total’ü Interlocked.Increment ile artıran 
güvenli sayaç ekleyin.
3. Task ProcessWithTimeout(Order o, int ms, CancellationToken user) – 
CancellationTokenSource.CreateLinkedTokenSource ile kullanıcı iptali + 5 000 ms 
timeout zincirleyen metodu yazın.

*/

using System;
using System.Threading;
using System.Threading.Tasks;

namespace CheckoutQueueFix
{
    // Basit Order modeli (NET Framework uyumlu, nullable yok)
    class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }   // null değil
        public int SimulatedWorkMs { get; set; }

        public Order()
        {
            UserId = ""; // derleme hatası olmasın diye varsayılan
        }
    }

    // 2) Thread-safe sayaç
    static class OrderCounter
    {
        public static int Total = 0;

        public static int Increment()
        {
            return Interlocked.Increment(ref Total);
        }
    }

    class CheckoutQueue
    {
        // 1) async void yerine async Task + try/catch + LogError
        public async Task OnMessageAsync(Order o, CancellationToken ct)
        {
            try
            {
                await ProcessWithTimeout(o, 5000, ct);

                int current = OrderCounter.Increment();
                Console.WriteLine("OK -> OrderId: " + o.Id + " | Total: " + current);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("CANCEL/TIMEOUT -> OrderId: " + o.Id);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private void LogError(Exception ex)
        {
            Console.WriteLine("ERROR -> " + ex.GetType().Name + " : " + ex.Message);
        }

        // 3) kullanıcı iptali + timeout zinciri
        public async Task ProcessWithTimeout(Order o, int ms, CancellationToken user)
        {
            if (o == null) throw new ArgumentNullException("o");
            if (ms <= 0) throw new ArgumentException("ms 0'dan buyuk olmali");

            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(user))
            {
                cts.CancelAfter(ms); // ör: 5000 ms
                await ProcessAsync(o, cts.Token);
            }
        }

        // İş simülasyonu (iptal edilebilir + hata üretebilir)
        private async Task ProcessAsync(Order o, CancellationToken ct)
        {
            // Bozuk mesaj simülasyonu: UserId boşsa hata üret
            if (string.IsNullOrWhiteSpace(o.UserId))
                throw new NullReferenceException("UserId bos geldigi icin islem iptal edildi (simulasyon)");

            // İşlem süresi (iptal edilebilir)
            await Task.Delay(o.SimulatedWorkMs, ct);

            // Arada random hata gibi: örnek
            if (o.Id % 13 == 0)
                throw new Exception("Banka servis hatasi (simulasyon)");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var service = new CheckoutQueue();

            // Kullanıcı iptali token'ı gibi düşün
            var userCts = new CancellationTokenSource();

            // Örnek siparişler (null yok!)
            Order[] orders = new Order[]
            {
                new Order { Id = 1,  UserId = "u1",  SimulatedWorkMs = 1000 },
                new Order { Id = 2,  UserId = "u2",  SimulatedWorkMs = 7000 }, // timeout yiyecek
                new Order { Id = 3,  UserId = "",    SimulatedWorkMs = 500  }, // NullReference simülasyonu
                new Order { Id = 13, UserId = "u13", SimulatedWorkMs = 800  }, // Exception simülasyonu
                new Order { Id = 4,  UserId = "u4",  SimulatedWorkMs = 2000 }
            };

            // Worker thread gibi paralel çalıştır
            Task[] tasks = new Task[orders.Length];
            for (int i = 0; i < orders.Length; i++)
            {
                int idx = i;
                tasks[i] = service.OnMessageAsync(orders[idx], userCts.Token);
            }

            Task.WaitAll(tasks);

            Console.WriteLine("\nBitti. TotalOrdersProcessed: " + OrderCounter.Total);
            Console.ReadLine();
        }
    }
}
