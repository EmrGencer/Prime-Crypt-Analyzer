using System;
using System.Diagnostics;
using System.Numerics;

namespace PrimeAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("======================================================");
            Console.WriteLine("    Kapsamlı Asal Sayı Test ve Analiz Programı        ");
            Console.WriteLine("======================================================");
            
            while (true)
            {
                Console.WriteLine("\nLütfen test etmek istediğiniz sayıyı girin (Çıkış için 'q'):");
                string? input = Console.ReadLine();
                if (input?.ToLower() == "q") break;

                if (!long.TryParse(input, out long number) || number < 0)
                {
                    Console.WriteLine("Hata: Lütfen 0 veya daha büyük geçerli bir pozitif tam sayı girin.");
                    continue;
                }

                Console.WriteLine("\nKullanmak istediğiniz algoritmayı seçin:");
                Console.WriteLine("1. Sieve of Eratosthenes (Eratosten Eleği)");
                Console.WriteLine("2. Sieve of Atkin (Atkin Eleği)");
                Console.WriteLine("3. Miller-Rabin Primality Test");
                Console.WriteLine("4. Hepsini Karşılaştır");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RunSieveOfEratosthenes(number, true);
                        break;
                    case "2":
                        RunSieveOfAtkin(number, true);
                        break;
                    case "3":
                        RunMillerRabin(number, true);
                        break;
                    case "4":
                        CompareAll(number);
                        break;
                    default:
                        Console.WriteLine("Geçersiz seçim. Lütfen tekrar deneyin.");
                        break;
                }
            }
        }

        /* 
         * 1. Sieve of Eratosthenes (Eratosten Eleği)
         * Belirli bir üst sınıra (limit) kadar olan asal sayıları bulmak için kullanılır.
         * En bilinen ve temel eleme yöntemidir. Küçük sayılarda hızlıdır, ancak çok
         * büyük sayılarda dizi tahsisatından (array allocation) dolayı yüksek bellek tüketir.
         */
        static (bool isPrime, long elapsedMs, long elapsedTicks, string? error) RunSieveOfEratosthenes(long limit, bool printResult = false)
        {
            // Negatif ve küçük sayılar için kontrol
            if (limit < 2) 
            {
                if (printResult) PrintResult("Sieve of Eratosthenes", limit, false, 0, 0);
                return (false, 0, 0, null);
            }

            // Dizi boyutu sınırlarını aşmayı önlemek için güvenlik kontrolü (~2 Milyar)
            if (limit > int.MaxValue - 56) 
            {
                string err = "Sayı çok büyük. Dizi boyutu (Array Limit) aşıldı.";
                if (printResult) Console.WriteLine($"\n[Hata] Eratosthenes: {err}");
                return (false, 0, 0, err);
            }

            int n = (int)limit;
            Stopwatch sw = Stopwatch.StartNew();
            bool isPrime = false;
            
            try
            {
                // isComposite dizisi: false durumu asal (prime), true durumu asal değil (composite) anlamına gelir.
                // Diziyi false (varsayılan) başlatmak zaman kazandırır.
                bool[] isComposite = new bool[n + 1];
                isComposite[0] = true;
                isComposite[1] = true;

                // Eleme işlemi: p*p <= n olana kadar
                for (int p = 2; p * p <= n; p++)
                {
                    if (!isComposite[p]) // Eğer p asal ise
                    {
                        // p'nin katlarını asal değil olarak işaretle
                        for (int i = p * p; i <= n; i += p)
                        {
                            isComposite[i] = true;
                        }
                    }
                }
                
                isPrime = !isComposite[n];
                sw.Stop();

                if (printResult)
                    PrintResult("Sieve of Eratosthenes", limit, isPrime, sw.ElapsedMilliseconds, sw.ElapsedTicks);

                return (isPrime, sw.ElapsedMilliseconds, sw.ElapsedTicks, null);
            }
            catch (OutOfMemoryException)
            {
                sw.Stop();
                string err = "Yetersiz bellek (OutOfMemory). Bu algoritma büyük sayılar için uygun değildir.";
                if (printResult) Console.WriteLine($"\n[Hata] Eratosthenes: {err}");
                return (false, 0, 0, err);
            }
        }

        /* 
         * 2. Sieve of Atkin (Atkin Eleği)
         * Eratosten eleğinin daha modern ve belirli şartlarda daha optimize edilmiş halidir.
         * Kareler ve modulo aritmetiğini kullanarak potansiyel asalları eler.
         * Karmaşık mantığı sayesinde asimptotik olarak biraz daha verimli olabilir, 
         * ancak bellek kısıtlaması (OutOfMemory) aynen devam eder.
         */
        static (bool isPrime, long elapsedMs, long elapsedTicks, string? error) RunSieveOfAtkin(long limit, bool printResult = false)
        {
            if (limit < 2) 
            {
                if (printResult) PrintResult("Sieve of Atkin", limit, false, 0, 0);
                return (false, 0, 0, null);
            }

            if (limit > int.MaxValue - 56)
            {
                string err = "Sayı çok büyük. Dizi boyutu (Array Limit) aşıldı.";
                if (printResult) Console.WriteLine($"\n[Hata] Atkin: {err}");
                return (false, 0, 0, err);
            }

            int n = (int)limit;
            Stopwatch sw = Stopwatch.StartNew();
            bool isPrime = false;

            try
            {
                // Eleme dizisi: true durumu asal olduğunu belirtir
                bool[] sieve = new bool[n + 1];
                
                if (n >= 2) sieve[2] = true;
                if (n >= 3) sieve[3] = true;

                // Modulo 12, 60 gibi değerler üzerinden kare toplamları analizi
                for (int x = 1; x * x <= n; x++)
                {
                    for (int y = 1; y * y <= n; y++)
                    {
                        int n1 = (4 * x * x) + (y * y);
                        if (n1 <= n && (n1 % 12 == 1 || n1 % 12 == 5))
                            sieve[n1] ^= true;

                        int n2 = (3 * x * x) + (y * y);
                        if (n2 <= n && n2 % 12 == 7)
                            sieve[n2] ^= true;

                        int n3 = (3 * x * x) - (y * y);
                        if (x > y && n3 <= n && n3 % 12 == 11)
                            sieve[n3] ^= true;
                    }
                }

                // Tam kare asalların katlarını eleme
                for (int r = 5; r * r <= n; r++)
                {
                    if (sieve[r])
                    {
                        for (int i = r * r; i <= n; i += r * r)
                            sieve[i] = false;
                    }
                }

                isPrime = sieve[n];
                sw.Stop();

                if (printResult)
                    PrintResult("Sieve of Atkin", limit, isPrime, sw.ElapsedMilliseconds, sw.ElapsedTicks);

                return (isPrime, sw.ElapsedMilliseconds, sw.ElapsedTicks, null);
            }
            catch (OutOfMemoryException)
            {
                sw.Stop();
                string err = "Yetersiz bellek (OutOfMemory). Bu algoritma büyük sayılar için uygun değildir.";
                if (printResult) Console.WriteLine($"\n[Hata] Atkin: {err}");
                return (false, 0, 0, err);
            }
        }

        /* 
         * 3. Miller-Rabin Primality Test
         * Çok büyük sayılar (kriptografi vb.) için kullanılan olasılıksal bir yaklaşımdır.
         * Sayıyı elemek yerine, belirli iterasyonlarla test eder. 
         * Eğer testten geçemezse kesinlikle asal değildir, geçerse büyük olasılıkla asaldır.
         * Bellek sınırına takılmaz (sadece test edilen sayı kadar işlem yapar).
         */
        static (bool isPrime, long elapsedMs, long elapsedTicks, string? error) RunMillerRabin(long n, bool printResult = false)
        {
            Stopwatch sw = Stopwatch.StartNew();
            
            // Temel Durumlar (Base Cases)
            if (n < 2) 
            {
                sw.Stop();
                if (printResult) PrintResult("Miller-Rabin", n, false, sw.ElapsedMilliseconds, sw.ElapsedTicks);
                return (false, sw.ElapsedMilliseconds, sw.ElapsedTicks, null);
            }
            if (n == 2 || n == 3)
            {
                sw.Stop();
                if (printResult) PrintResult("Miller-Rabin", n, true, sw.ElapsedMilliseconds, sw.ElapsedTicks);
                return (true, sw.ElapsedMilliseconds, sw.ElapsedTicks, null);
            }
            if (n % 2 == 0)
            {
                sw.Stop();
                if (printResult) PrintResult("Miller-Rabin", n, false, sw.ElapsedMilliseconds, sw.ElapsedTicks);
                return (false, sw.ElapsedMilliseconds, sw.ElapsedTicks, null);
            }

            // n-1 sayısını (2^s * d) formunda yazıyoruz.
            long d = n - 1;
            int s = 0;
            while (d % 2 == 0)
            {
                d /= 2;
                s++;
            }

            int k = 10; // İterasyon sayısı (Hassasiyeti artırır, önerilen kriptografik değerler daha yüksektir)
            bool isPrime = true;
            Random rand = new Random();

            for (int i = 0; i < k; i++)
            {
                // [2, n-2] aralığında rastgele bir a tabanı seç
                long a = LongRandom(2, n - 2, rand);
                
                // BigInteger kullanarak yüksek sayılarda bellek/overflow taşmasını engelliyoruz
                BigInteger bigA = new BigInteger(a);
                BigInteger bigD = new BigInteger(d);
                BigInteger bigN = new BigInteger(n);
                
                // x = (a^d) % n
                long x = (long)BigInteger.ModPow(bigA, bigD, bigN);

                if (x == 1 || x == n - 1)
                    continue;

                bool compositeForm = true;
                for (int r = 1; r < s; r++)
                {
                    BigInteger bigX = new BigInteger(x);
                    x = (long)BigInteger.ModPow(bigX, 2, bigN);
                    
                    if (x == n - 1)
                    {
                        compositeForm = false;
                        break;
                    }
                }

                if (compositeForm)
                {
                    isPrime = false;
                    break;
                }
            }

            sw.Stop();
            if (printResult) 
                PrintResult("Miller-Rabin Testi", n, isPrime, sw.ElapsedMilliseconds, sw.ElapsedTicks);

            return (isPrime, sw.ElapsedMilliseconds, sw.ElapsedTicks, null);
        }

        // --- Karşılaştırmalı Analiz Menüsü ---
        static void CompareAll(long number)
        {
            Console.WriteLine($"\n[{number}] sayısı için karşılaştırmalı analiz başlatılıyor...\n");

            var resEratosthenes = RunSieveOfEratosthenes(number);
            var resAtkin = RunSieveOfAtkin(number);
            var resMillerRabin = RunMillerRabin(number);

            Console.WriteLine(new string('-', 100));
            Console.WriteLine($"{"Algoritma",-25} | {"Sonuç",-15} | {"Süre (ms)",-10} | {"Süre (Ticks)",-12} | {"Durum / Notlar"}");
            Console.WriteLine(new string('-', 100));

            PrintComparisonRow("Sieve of Eratosthenes", resEratosthenes);
            PrintComparisonRow("Sieve of Atkin", resAtkin);
            PrintComparisonRow("Miller-Rabin Testi", resMillerRabin);

            Console.WriteLine(new string('-', 100));
            Console.WriteLine("* Not: Miller-Rabin testinin sonucu 'Asal' ise, bu olasılıksal (probabilistic) bir sonuçtur.\n");
        }

        // --- Yardımcı Konsol Çıktı Fonksiyonları ---
        static void PrintResult(string algoName, long number, bool isPrime, long ms, long ticks)
        {
            Console.WriteLine($"\n--- {algoName} ---");
            Console.WriteLine($"Sayı : {number}");
            Console.WriteLine($"Sonuç: {(isPrime ? "ASALDIR" : "ASAL DEĞİLDİR")}");
            Console.WriteLine($"Süre : {ms} ms ({ticks} ticks)");
        }

        static void PrintComparisonRow(string algoName, (bool isPrime, long elapsedMs, long elapsedTicks, string? error) result)
        {
            string primeStr = result.error != null ? "N/A" : (result.isPrime ? "Asal" : "Asal Değil");
            string msStr = result.error != null ? "-" : result.elapsedMs.ToString();
            string ticksStr = result.error != null ? "-" : result.elapsedTicks.ToString();
            string statusStr = result.error != null ? result.error : "Başarılı";

            Console.WriteLine($"{algoName,-25} | {primeStr,-15} | {msStr,-10} | {ticksStr,-12} | {statusStr}");
        }

        // --- Rastgele UZUN (long) Sayı Üretici (Miller-Rabin İçin) ---
        static long LongRandom(long min, long max, Random rand)
        {
            if (max <= min) return min; // edge case için
            
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);
            
            return (Math.Abs(longRand % (max - min)) + min);
        }
    }
}
