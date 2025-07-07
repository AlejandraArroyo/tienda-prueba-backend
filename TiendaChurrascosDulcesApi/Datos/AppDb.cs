using Microsoft.EntityFrameworkCore;
using System;
using TiendaChurrascosDulcesApi.Modelos;

namespace TiendaChurrascosDulcesApi.Datos
{
    public class AppDb : DbContext
    {
        public AppDb(DbContextOptions<AppDb> options) : base(options) { }
        public DbSet<ChurrascoGuarnicion> ChurrascosGuarnicion { get; set; }



        public DbSet<TipoCarne> TiposCarne { get; set; }
        public DbSet<TerminoCoccion> TerminosCoccion { get; set; }
        public DbSet<Guarnicion> Guarniciones { get; set; }
        public DbSet<Churrasco> Churrascos { get; set; }
       


        public DbSet<Dulce> Dulces => Set<Dulce>();
        public DbSet<Presentacion> Presentaciones => Set<Presentacion>();



        /// inventarios 
       

        public DbSet<CarneInventario> CarnesInventario { get; set; }
        public DbSet<IngredienteGuarnicion> IngredientesGuarnicion { get; set; }
        public DbSet<DulceUnidad> DulcesUnidad { get; set; }
        public DbSet<Empaque> Empaques { get; set; }
        public DbSet<Combustible> Combustibles { get; set; }

        public DbSet<DulceCaja> DulcesCaja { get; set; }

        public DbSet<VentaDetalle> VentaDetalles { get; set; }
        /// combos


        public DbSet<Combo> Combos { get; set; }
        public DbSet<ComboChurrasco> CombosChurrasco { get; set; }
        public DbSet<ComboDulceUnidad> CombosDulceUnidad { get; set; }
        public DbSet<ComboDulceCaja> CombosDulceCaja { get; set; }


        public DbSet<Venta> Ventas { get; set; }
        


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dulce>()
                .HasMany(d => d.Presentaciones)
                .WithOne(p => p.Dulce)
                .HasForeignKey(p => p.DulceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChurrascoGuarnicion>()
          .HasKey(cg => cg.Id);

            modelBuilder.Entity<ChurrascoGuarnicion>()
                .HasOne(cg => cg.Churrasco)
                .WithMany(c => c.ChurrascosGuarnicion)
                .HasForeignKey(cg => cg.ChurrascoId);

            modelBuilder.Entity<ChurrascoGuarnicion>()
                .HasOne(cg => cg.Guarnicion)
                .WithMany()
                .HasForeignKey(cg => cg.GuarnicionId);


            modelBuilder.Entity<Combo>().ToTable("combo");
            modelBuilder.Entity<ComboChurrasco>().ToTable("combo_churrasco");
            modelBuilder.Entity<ComboDulceUnidad>().ToTable("combo_dulce_unidad");
            modelBuilder.Entity<ComboDulceCaja>().ToTable("combo_dulce_caja");
        }



    }
}
