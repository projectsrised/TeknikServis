using Microsoft.EntityFrameworkCore;
using TeknikServisApp.Application.Interfaces;
using TeknikServisApp.Domain.Entities;

namespace TeknikServisApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    private readonly ITenantProvider? _tenantProvider;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider? tenantProvider = null) : base(options)
    {
        _tenantProvider = tenantProvider;
        
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Bayi> Bayiler => Set<Bayi>();
    public DbSet<Kullanici> Kullanicilar => Set<Kullanici>();
    public DbSet<Marka> Markalar => Set<Marka>();
    public DbSet<Model> Modeller => Set<Model>();
    public DbSet<Kategori> Kategoriler => Set<Kategori>();
    public DbSet<Urun> Urunler => Set<Urun>();
    public DbSet<SeriNumarasi> SeriNumaralari => Set<SeriNumarasi>();
    public DbSet<Depo> Depolar => Set<Depo>();
    public DbSet<Kasa> Kasalar => Set<Kasa>();
    public DbSet<KasaHareket> KasaHareketleri => Set<KasaHareket>();
    public DbSet<StokHareket> StokHareketleri => Set<StokHareket>();
    public DbSet<Musteri> Musteriler => Set<Musteri>();
    public DbSet<Satis> Satislar => Set<Satis>();
    public DbSet<SatisKalem> SatisKalemleri => Set<SatisKalem>();
    public DbSet<TeknikServisKayit> TeknikServisler => Set<TeknikServisKayit>();
    public DbSet<TeknikServisKalem> TeknikServisKalemleri => Set<TeknikServisKalem>();
    public DbSet<Transfer> Transferler => Set<Transfer>();
    public DbSet<TransferKalem> TransferKalemleri => Set<TransferKalem>();
    public DbSet<Iade> Iadeler => Set<Iade>();
    public DbSet<IadeKalem> IadeKalemleri => Set<IadeKalem>();
    public DbSet<Fatura> Faturalar => Set<Fatura>();
    public DbSet<FaturaKalem> FaturaKalemleri => Set<FaturaKalem>();
    public DbSet<Sayim> Sayimlar => Set<Sayim>();
    public DbSet<SayimKalem> SayimKalemleri => Set<SayimKalem>();
    public DbSet<PersonelIzin> PersonelIzinleri => Set<PersonelIzin>();
    public DbSet<PersonelAvans> PersonelAvanslari => Set<PersonelAvans>();
    public DbSet<PersonelMesai> PersonelMesaileri => Set<PersonelMesai>();
    public DbSet<PersonelMaasOdeme> PersonelMaasOdemeleri => Set<PersonelMaasOdeme>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global tenant filter
        if (_tenantProvider != null)
        {
            modelBuilder.Entity<Bayi>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<Kullanici>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<Marka>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<Model>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            // Kategori is global - no tenant filter (only Silindi filter)
            modelBuilder.Entity<Kategori>().HasQueryFilter(e => !e.Silindi);
            modelBuilder.Entity<Urun>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<Depo>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<Kasa>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<Musteri>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<Satis>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<TeknikServisKayit>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<Transfer>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<Iade>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<Fatura>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<Sayim>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<SeriNumarasi>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<StokHareket>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<KasaHareket>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<PersonelIzin>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<PersonelAvans>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<PersonelMesai>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
            modelBuilder.Entity<PersonelMaasOdeme>().HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId && !e.Silindi);
        }

        // Indexes
        modelBuilder.Entity<Kullanici>().HasIndex(k => k.Email).IsUnique();
        modelBuilder.Entity<Marka>().HasIndex(m => m.Ad);
        modelBuilder.Entity<Model>().HasIndex(m => m.Ad);
        modelBuilder.Entity<SeriNumarasi>().HasIndex(s => s.SeriNo);
        modelBuilder.Entity<Urun>().HasIndex(u => u.Barkod);
        modelBuilder.Entity<Satis>().HasIndex(s => s.SatisNo).IsUnique();
        modelBuilder.Entity<TeknikServisKayit>().HasIndex(t => t.ServisNo).IsUnique();
        modelBuilder.Entity<Transfer>().HasIndex(t => t.TransferNo).IsUnique();
        modelBuilder.Entity<Fatura>().HasIndex(f => f.FaturaNo).IsUnique();

        // Marka - Model relationship
        modelBuilder.Entity<Model>()
            .HasOne(m => m.Marka)
            .WithMany(m => m.Modeller)
            .HasForeignKey(m => m.MarkaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Urun - Marka/Model relationships
        modelBuilder.Entity<Urun>()
            .HasOne(u => u.UrunMarka)
            .WithMany()
            .HasForeignKey(u => u.MarkaId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Urun>()
            .HasOne(u => u.UrunModel)
            .WithMany()
            .HasForeignKey(u => u.ModelId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relationships
        modelBuilder.Entity<Kategori>()
            .HasOne(k => k.UstKategori)
            .WithMany(k => k.AltKategoriler)
            .HasForeignKey(k => k.UstKategoriId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transfer>()
            .HasOne(t => t.KaynakDepo)
            .WithMany()
            .HasForeignKey(t => t.KaynakDepoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transfer>()
            .HasOne(t => t.HedefDepo)
            .WithMany()
            .HasForeignKey(t => t.HedefDepoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Decimal precision
        modelBuilder.Entity<Urun>().Property(u => u.AlisFiyat).HasPrecision(18, 2);
        modelBuilder.Entity<Urun>().Property(u => u.SatisFiyat).HasPrecision(18, 2);
        modelBuilder.Entity<Urun>().Property(u => u.KdvOran).HasPrecision(5, 2);
        modelBuilder.Entity<SeriNumarasi>().Property(s => s.AlisFiyati).HasPrecision(18, 2);
        modelBuilder.Entity<SeriNumarasi>().Property(s => s.SatisFiyati).HasPrecision(18, 2);
        modelBuilder.Entity<Satis>().Property(s => s.AraToplam).HasPrecision(18, 2);
        modelBuilder.Entity<Satis>().Property(s => s.KdvToplam).HasPrecision(18, 2);
        modelBuilder.Entity<Satis>().Property(s => s.IndirimTutar).HasPrecision(18, 2);
        modelBuilder.Entity<Satis>().Property(s => s.GenelToplam).HasPrecision(18, 2);
        modelBuilder.Entity<Satis>().Property(s => s.NakitTutar).HasPrecision(18, 2);
        modelBuilder.Entity<Satis>().Property(s => s.KartTutar).HasPrecision(18, 2);
        modelBuilder.Entity<SatisKalem>().Property(k => k.BirimFiyat).HasPrecision(18, 2);
        modelBuilder.Entity<SatisKalem>().Property(k => k.KdvOran).HasPrecision(5, 2);
        modelBuilder.Entity<SatisKalem>().Property(k => k.KdvTutar).HasPrecision(18, 2);
        modelBuilder.Entity<SatisKalem>().Property(k => k.IndirimTutar).HasPrecision(18, 2);
        modelBuilder.Entity<SatisKalem>().Property(k => k.Toplam).HasPrecision(18, 2);
        modelBuilder.Entity<Kasa>().Property(k => k.Bakiye).HasPrecision(18, 2);
        modelBuilder.Entity<KasaHareket>().Property(h => h.Tutar).HasPrecision(18, 2);
        modelBuilder.Entity<KasaHareket>().Property(h => h.OncekiBakiye).HasPrecision(18, 2);
        modelBuilder.Entity<KasaHareket>().Property(h => h.SonrakiBakiye).HasPrecision(18, 2);
        modelBuilder.Entity<TeknikServisKayit>().Property(t => t.IscilikUcreti).HasPrecision(18, 2);
        modelBuilder.Entity<TeknikServisKayit>().Property(t => t.ParcaToplam).HasPrecision(18, 2);
        modelBuilder.Entity<TeknikServisKayit>().Property(t => t.ToplamTutar).HasPrecision(18, 2);
        modelBuilder.Entity<TeknikServisKayit>().Property(t => t.OdenenTutar).HasPrecision(18, 2);
        modelBuilder.Entity<TeknikServisKalem>().Property(k => k.BirimFiyat).HasPrecision(18, 2);
        modelBuilder.Entity<TeknikServisKalem>().Property(k => k.Toplam).HasPrecision(18, 2);
        modelBuilder.Entity<Fatura>().Property(f => f.AraToplam).HasPrecision(18, 2);
        modelBuilder.Entity<Fatura>().Property(f => f.KdvToplam).HasPrecision(18, 2);
        modelBuilder.Entity<Fatura>().Property(f => f.IndirimTutar).HasPrecision(18, 2);
        modelBuilder.Entity<Fatura>().Property(f => f.GenelToplam).HasPrecision(18, 2);
        modelBuilder.Entity<FaturaKalem>().Property(k => k.BirimFiyat).HasPrecision(18, 2);
        modelBuilder.Entity<FaturaKalem>().Property(k => k.KdvOran).HasPrecision(5, 2);
        modelBuilder.Entity<FaturaKalem>().Property(k => k.KdvTutar).HasPrecision(18, 2);
        modelBuilder.Entity<FaturaKalem>().Property(k => k.IndirimTutar).HasPrecision(18, 2);
        modelBuilder.Entity<FaturaKalem>().Property(k => k.Toplam).HasPrecision(18, 2);
        modelBuilder.Entity<Iade>().Property(i => i.ToplamTutar).HasPrecision(18, 2);
        modelBuilder.Entity<Iade>().Property(i => i.IadeEdilen).HasPrecision(18, 2);
        modelBuilder.Entity<IadeKalem>().Property(k => k.IadeTutar).HasPrecision(18, 2);
        modelBuilder.Entity<Kullanici>().Property(k => k.Maas).HasPrecision(18, 2);
        modelBuilder.Entity<PersonelAvans>().Property(a => a.Tutar).HasPrecision(18, 2);
        modelBuilder.Entity<PersonelMesai>().Property(m => m.ToplamSaat).HasPrecision(10, 2);
        modelBuilder.Entity<PersonelMesai>().Property(m => m.SaatUcreti).HasPrecision(18, 2);
        modelBuilder.Entity<PersonelMesai>().Property(m => m.ToplamUcret).HasPrecision(18, 2);
        modelBuilder.Entity<PersonelMaasOdeme>().Property(o => o.BrutMaas).HasPrecision(18, 2);
        modelBuilder.Entity<PersonelMaasOdeme>().Property(o => o.NetMaas).HasPrecision(18, 2);
        modelBuilder.Entity<PersonelMaasOdeme>().Property(o => o.EkMesaiUcreti).HasPrecision(18, 2);
        modelBuilder.Entity<PersonelMaasOdeme>().Property(o => o.Kesinti).HasPrecision(18, 2);
        modelBuilder.Entity<PersonelMaasOdeme>().Property(o => o.AvansKesinti).HasPrecision(18, 2);
        modelBuilder.Entity<PersonelMaasOdeme>().Property(o => o.ToplamOdeme).HasPrecision(18, 2);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.GuncellemeTarihi = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
