
/*
 
5 Çevrim - İçi Sepet – Türkçe Yüzdeler, Canlı Koleksiyon ve “Ücretsiz Kargo”

Yeni kampanya modülü, fiyatları anlık çeker; indirim yüzdesi “25,00 %” biçiminde 
(Türkçe ondalık) gelir.
 Front-end MVVM katmanı ObservableCollection<Product> ile çalışır.
 Cart arka planda API’yi poll eder; koleksiyon değişince UI animasyonlu 
güncellenir.
 Ücretsiz kargo limiti ″ 1 000: toplam altına düştüğünde rozet kaybolmalı.
 “Aynı marka” ürünlerin toplam indirimi sağ-üst köşede yüzde (%) formatlı 
görünmeli.
 İlk testte INotifyCollectionChanged ve INotifyPropertyChanged yanlış kullanıldı;
UI thread’i çakıldı, spinner dönüp kaldı.
 CEO mobil uygulamada fiyat kayarken “″ 1 234,567,89” gördü, Slack’te “O ne 
öyle?” diye patladı.
 BugFix günü: tüm güncellemeler gerçek-zamanlı olacak, kargo rozeti PadLeft ile 
tam ortalanacak.
Kodlama Görevleri 
1. class Cart – koleksiyona Subscribe olup UpdateTotals() içinde toplam fiyat,
indirim yüzdesi ve kargo durumunu güncellesin.
2. string GetBrandDiscount(string brand) – tek satır LINQ ile aynı markanın toplam 
indirimini P biçimli string olarak döndürsün.
3. event CargoStatusHandler OnCargo – toplam ≤ 1 000 olduğunda tetiklenip 
Console.WriteLine(msg.PadLeft(40)) yazsın.

*/


using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

namespace OnlineCartTR
{
    // Ürün modeli
    class Product
    {
        public string Brand { get; set; }        // marka
        public decimal Price { get; set; }       // fiyat (TL)
        public string DiscountText { get; set; } // ör: "25,00 %"

        public Product()
        {
            Brand = "";
            DiscountText = "0 %";
        }
    }

    // 3) event delegate
    delegate void CargoStatusHandler(string msg);

    class Cart
    {
        private readonly CultureInfo tr = new CultureInfo("tr-TR");
        private bool lastFreeCargoState = true; // ilk durumda rozet var gibi varsayalım

        public ObservableCollection<Product> Products { get; private set; }

        public decimal TotalPrice { get; private set; }
        public decimal TotalDiscountAmount { get; private set; } // TL olarak indirimin toplam etkisi
        public bool IsFreeCargo { get; private set; }

        public event CargoStatusHandler OnCargo;

        public Cart()
        {
            Products = new ObservableCollection<Product>();
            Products.CollectionChanged += Products_CollectionChanged;

            UpdateTotals(); // başlangıç
        }

        // 1) Koleksiyona subscribe olup UpdateTotals çağır
        private void Products_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateTotals();
        }

        // toplam fiyat, indirim ve kargo durumunu güncelle
        public void UpdateTotals()
        {
            decimal total = 0m;
            decimal discountTotal = 0m;

            for (int i = 0; i < Products.Count; i++)
            {
                var p = Products[i];
                if (p == null) continue;

                total += p.Price;

                decimal rate = ParseTurkishPercent(p.DiscountText); // 0.25 gibi
                // indirimin TL etkisi: Price * rate (basitçe)
                discountTotal += p.Price * rate;
            }

            TotalPrice = total;
            TotalDiscountAmount = discountTotal;

            // Ücretsiz kargo limiti: toplam <= 1000 ise rozet kaybolmalı => free cargo false
            bool freeCargoNow = TotalPrice > 1000m;
            IsFreeCargo = freeCargoNow;

            // rozet "var" iken "yok" durumuna düştüyse event tetikle
            if (lastFreeCargoState == true && freeCargoNow == false)
            {
                if (OnCargo != null)
                {
                    OnCargo("Ucretsiz kargo rozetiniz kayboldu (Toplam <= 1000 TL)");
                }
            }

            lastFreeCargoState = freeCargoNow;
        }

        // 2) Tek satır LINQ: aynı markanın toplam indirimini "P" formatında döndür
        // Burada toplam indirim oranını, aynı markadaki ürünlerin indirim oranlarının toplamı gibi aldık
        // (Soru "toplam indirimi" diyor; en basit yorum: oranları topla)
        public string GetBrandDiscount(string brand) =>
            Products.Where(p => p != null && p.Brand == brand)
                    .Sum(p => ParseTurkishPercent(p.DiscountText))
                    .ToString("P", tr);

        // "25,00 %" -> 0.25 gibi çevirir
        // "1 234,567,89" gibi bozuk formatları da temizlemeye çalışır
        private decimal ParseTurkishPercent(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0m;

            // yüzde işaretini kaldır
            string s = text.Replace("%", "").Trim();

            // bozuk format temizleme:
            // - boşlukları kaldır (1 234,56 -> 1234,56)
            s = s.Replace(" ", "");

            // Bazı bozuk veri "1 234,567,89" gibi gelebilir.
            // Bu durumda en sonda bulunan ',' ondalık olsun diye kaba bir düzeltme yapıyoruz:
            //  - sonda birden fazla virgül varsa, son virgülü ondalık kabul edip öncekileri sil
            int lastComma = s.LastIndexOf(',');
            if (lastComma >= 0)
            {
                // son virgülden önceki kısımda virgül varsa hepsini kaldır (binlik gibi davran)
                string before = s.Substring(0, lastComma).Replace(",", "");
                string after = s.Substring(lastComma + 1);

                // eğer after da virgül içeriyorsa onu da temizle
                after = after.Replace(",", "");

                s = before + "," + after;
            }

            decimal number;
            // tr-TR ile parse et
            if (decimal.TryParse(s, NumberStyles.Number, tr, out number))
            {
                // "25,00" -> 25 demek; yüzde oranı için 100'e böl
                return number / 100m;
            }

            return 0m;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Cart cart = new Cart();

            // 3) Event: toplam <= 1000 olduğunda tetiklenince PadLeft ile ortalı gibi yaz
            cart.OnCargo += (msg) =>
            {
                Console.WriteLine(msg.PadLeft(40));
            };

            // Ürün ekleyelim
            cart.Products.Add(new Product { Brand = "A", Price = 400m, DiscountText = "25,00 %" });
            cart.Products.Add(new Product { Brand = "A", Price = 350m, DiscountText = "10,00 %" });
            cart.Products.Add(new Product { Brand = "B", Price = 500m, DiscountText = "5,00 %" });

            PrintCart(cart);

            Console.WriteLine("\nA markasi toplam indirim: " + cart.GetBrandDiscount("A"));

            // Toplamı 1000 altına düşürelim (rozet kaybolmalı => event tetiklenir)
            Console.WriteLine("\nBir urun cikartiliyor (toplam <= 1000 olursa rozet gidecek)...");
            cart.Products.RemoveAt(2); // B markalı 500 TL gider => toplam 750 olur => event

            PrintCart(cart);

            // Bozuk yüzde format testi
            Console.WriteLine("\nBozuk format testi ekleniyor: \"1 234,567,89 %\"");
            cart.Products.Add(new Product { Brand = "C", Price = 100m, DiscountText = "1 234,567,89 %" });

            PrintCart(cart);
            Console.WriteLine("\nC markasi toplam indirim: " + cart.GetBrandDiscount("C"));

            Console.ReadLine();
        }

        static void PrintCart(Cart cart)
        {
            Console.WriteLine("\n--- CART ---");
            Console.WriteLine("Urun sayisi: " + cart.Products.Count);
            Console.WriteLine("Toplam: " + cart.TotalPrice.ToString("C", new CultureInfo("tr-TR")));
            Console.WriteLine("Indirim TL: " + cart.TotalDiscountAmount.ToString("C", new CultureInfo("tr-TR")));
            Console.WriteLine("Ucretsiz Kargo: " + (cart.IsFreeCargo ? "VAR" : "YOK (<= 1000)"));
        }
    }
}
