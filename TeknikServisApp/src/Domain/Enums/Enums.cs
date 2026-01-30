namespace TeknikServisApp.Domain.Enums;

public enum UserRole
{
    SuperAdmin = 1,
    TenantAdmin = 2,
    BayiAdmin = 3,
    SatisSorumlusu = 4,
    TeknikServisPersoneli = 5
}

public enum KategoriTip
{
    Telefon = 1,
    Tablet = 2,
    Aksesuar = 3,
    YedekParca = 4,
    Diger = 5
}

public enum StokHareketTip
{
    Giris = 1,
    Cikis = 2,
    Transfer = 3,
    Satis = 4,
    Iade = 5,
    Sayim = 6,
    Fire = 7
}

public enum KasaHareketTip
{
    Giris = 1,
    Cikis = 2,
    NakitSatis = 3,
    KartSatis = 4,
    TeknikServisOdeme = 5,
    MaasOdeme = 6,
    AvansOdeme = 7,
    Gider = 8
}

public enum OdemeTipi
{
    Nakit = 1,
    KrediKarti = 2,
    Havale = 3,
    Taksit = 4,
    Karisik = 5
}

public enum TeknikServisDurum
{
    Beklemede = 1,
    Inceleniyor = 2,
    ParcaBekleniyor = 3,
    Onariliyor = 4,
    Tamamlandi = 5,
    TeslimEdildi = 6,
    IptalEdildi = 7
}

public enum TransferDurum
{
    Beklemede = 1,
    Onaylandi = 2,
    YoldaGidiyor = 3,
    TeslimAlindi = 4,
    Tamamlandi = 5,
    IptalEdildi = 6
}

public enum SayimDurum
{
    Basladi = 1,
    DevamEdiyor = 2,
    Tamamlandi = 3,
    IptalEdildi = 4
}

public enum FaturaTip
{
    Alis = 1,
    Satis = 2,
    Iade = 3,
    TeknikServis = 4
}

public enum IadeDurum
{
    Beklemede = 1,
    Inceleniyor = 2,
    Onaylandi = 3,
    Reddedildi = 4,
    Tamamlandi = 5
}

public enum PersonelIzinTip
{
    YillikIzin = 1,
    HastalikIzni = 2,
    MazeretIzni = 3,
    UcretsizIzin = 4,
    DogumIzni = 5,
    EvlilikIzni = 6,
    OlumIzni = 7
}

public enum IzinDurum
{
    Beklemede = 1,
    Onaylandi = 2,
    Reddedildi = 3,
    IptalEdildi = 4
}
