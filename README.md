# Adfnet

## Proje Yapısı
- Proje dokümanları "/doc" klasöründe,
- Projede kullanılan ham kodlar, muhtelif dosyalar ve materyaller "/res" klasöründe,
- Sunucu taraflı kaynak kodları "/src/server" klasöründe,
- İstemci taraflı kaynak kodları "/src/client" klasöründe
bulunmaktadır.
Projenin sunucu taraflı kodları ".NET Core" ile istemci-web taraflı kodları ise Angular ile hazırlanmaktadır.
## Projeyi Çalıştırma
Projeyi çalıştırmak için bilgisayarınızda ".Net Core", "git" ve "Node.js" kurulu olmalıdır. Eğer yoksa şu aşamaları takip edin:
1. https://git-scm.com/download adresinden kendi sisteminize uygun olan kurulum dosyasını indirin ve kurulumu başlatın. Kurulum sonrasında komut satırından aşağıdaki komutlarla git istemcisine kendinizi tanıtın.
```
git config --global user.name "Adınız ve Soyadınız"
git config --global user.email "epostaadresiniz@siteniz.com"
```
2. Komut satırından ```git --version``` komutuyla kurulumu test edin. Karşınızda git’in versiyonunu görüyorsanız git istemcisini başarılı bir şekilde kurdunuz demektir.
3. https://nodejs.org adresinden adresinden kendi sisteminize uygun olan kurulum dosyasını indirin ve kurulumu başlatın. Kurulum sonrasında komut satırından ```npm --version``` komutuyla kurulumu test edin. Karşınızda npm’in versiyonunu görüyorsanız kurulum başarılı demektir.
4. https://dotnet.microsoft.com/download/dotnet-core/current adresinden kendi sisteminize uygun olan ".NET Core SDK" kurulum dosyalarını indirin ve kurulumu başlatın. Kurulum sonrasında komut satırından ```dotnet --version``` komutuyla kurulumu test edin. Karşınızda dotnet’in versiyonunu görüyorsanız kurulum başarılı demektir.
5. https://datalust.co/download adresinden Seq uygulamasını indirin ve kurulumu başlatın.
6. Komut satırından ```git clone https://github.com/atifdag/adfnet.git``` komutuyla projeyi bilgisayarınıza indirin.
7. Komut satırından ```npm i -g @angular/cli``` komutuyla "Angular CLI" paketini indirip kurun.
8. Komut satırından "src/server/Adfnet.Setup" klasöründe iken ```dotnet run``` komutuyla veri tabanı kurulumunu yapın.
9. Komut satırından "src/server/Adfnet.Web.Api" klasöründe iken ```dotnet run``` komutuyla Web Api projesini çalıştırın.
10. Komut satırından "src/client/web/private" klasöründe iken ```npm i``` komutuyla bu projede kullanılan paketleri kurun.
11. Komut satırından "src/client/web/private" klasöründe iken ```ng s -o``` komutuyla Angular tabanlı web sitesi yönetim paneli projesini çalıştırın.
12. Komut satırından "src/client/web/public" klasöründe iken ```npm i``` komutuyla bu projede kullanılan paketleri kurun.
13. Komut satırından "src/client/web/public" klasöründe iken ```ng s -o``` komutuyla Angular tabanlı genel site projesini çalıştırın.

