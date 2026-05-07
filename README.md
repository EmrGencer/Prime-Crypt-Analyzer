# Prime Crypt Analyzer 🛡️

Bu proje, kriptografinin temel taşı olan asal sayı tespit algoritmalarının (deterministik ve olasılıksal) performans ve doğruluk analizini gerçekleştiren bir yazılım laboratuvarıdır.

## 🚀 Özellikler

Uygulama, aşağıdaki üç temel algoritmayı bizzat (saf kod ile) implement ederek karşılaştırmalı sonuçlar sunar:

* **Sieve of Eratosthenes:** Geleneksel eleme yöntemi ile düşük ölçekli sayılarda yüksek doğruluk.
* **Sieve of Atkin:** Modern modüler aritmetik yaklaşımıyla optimize edilmiş elek algoritması.
* **Miller-Rabin Primality Test:** Çok büyük sayılar (BigInteger ölçeği) için olasılıksal, hızlı ve kriptografik standartlara uygun test mekanizması.

## 📊 Performans Analizi

Yazılım, her bir algoritmanın çalışma süresini hem **Milisaniye (ms)** hem de işlemci hassasiyetinde **Ticks/Nanisaniye** bazında ölçer. Bu sayede algoritmik karmaşıklık (Time Complexity) teorik bilgisi, pratik verilerle doğrulanır.

## 🛠️ Kurulum ve Çalıştırma

1. Projeyi bilgisayarınıza indirin:
   ```bash
