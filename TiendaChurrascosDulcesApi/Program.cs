using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Serialization;
using TiendaChurrascosDulcesApi.Datos;
using TiendaChurrascosDulcesApi.Modelos;
using Microsoft.AspNetCore.Http.Json;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDb>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
    );


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JsonOptions>(options => 
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000") 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowReactApp");


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();
app.MapGet("/", () => "API de Churrascos y Dulces");

// dulces tipicos



app.MapGet("/api/dulces", async (AppDb db) =>
    await db.Dulces.Include(d => d.Presentaciones).ToListAsync());

app.MapGet("/api/dulces/{id}", async (int id, AppDb db) =>
    await db.Dulces.Include(d => d.Presentaciones)
                   .FirstOrDefaultAsync(d => d.Id == id) is Dulce dulce
        ? Results.Ok(dulce)
        : Results.NotFound());


app.MapPost("/api/dulces", async (Dulce dulce, AppDb db) =>
{
    db.Dulces.Add(dulce);
    await db.SaveChangesAsync();
    return Results.Created($"/api/dulces/{dulce.Id}", dulce);
});

app.MapPut("/api/dulces/{id}", async (int id, Dulce input, AppDb db) =>
{
    var dulce = await db.Dulces.Include(d => d.Presentaciones).FirstOrDefaultAsync(d => d.Id == id);
    if (dulce is null) return Results.NotFound();

    dulce.Nombre = input.Nombre;
    dulce.Descripcion = input.Descripcion;
    dulce.Disponible = input.Disponible;

    db.Presentaciones.RemoveRange(dulce.Presentaciones);
    dulce.Presentaciones = input.Presentaciones;

    await db.SaveChangesAsync();
    return Results.Ok(dulce);
});


app.MapDelete("/api/dulces/{id}", async (int id, AppDb db) =>
{
    var dulce = await db.Dulces.FindAsync(id);
    if (dulce is null) return Results.NotFound();

    db.Dulces.Remove(dulce);
    await db.SaveChangesAsync();
    return Results.NoContent();
});



//// carnes


app.MapGet("/api/tiposcarne", async (AppDb db) => await db.TiposCarne.ToListAsync());
app.MapGet("/api/terminoscoccion", async (AppDb db) =>
    await db.TerminosCoccion.ToListAsync());
app.MapGet("/api/guarniciones", async (AppDb db) => await db.Guarniciones.ToListAsync());


app.MapGet("/api/churrascos", async (AppDb db) =>
{
    var churrascos = await db.Churrascos
        .Include(c => c.TipoCarne)
        .Include(c => c.TerminoCoccion)
        .Include(c => c.ChurrascosGuarnicion)
            .ThenInclude(cg => cg.Guarnicion)
        .ToListAsync();

    var resultado = churrascos.Select(c => new
    {
        c.Id,
        TipoCarne = c.TipoCarne.Nombre,
        TerminoCoccion = c.TerminoCoccion.Descripcion,
        c.Porciones,
        c.PorcionesExtra,
        Guarniciones = c.ChurrascosGuarnicion!
            .OrderBy(g => g.PorcionNumero)
            .GroupBy(g => g.PorcionNumero)
            .Select(g => new
            {
                Porcion = g.Key,
                Guarniciones = g.Select(x => x.Guarnicion!.Nombre).ToList()
            })
    });

    return Results.Ok(resultado);
});



app.MapPost("/api/churrascos/{id}/guarniciones", async (int id, List<int> guarnicionesIds, AppDb db) =>
{
    var churrasco = await db.Churrascos.FindAsync(id);
    if (churrasco is null) return Results.NotFound();

    foreach (var gid in guarnicionesIds)
    {
        db.ChurrascosGuarnicion.Add(new ChurrascoGuarnicion
        {
            ChurrascoId = id,
            GuarnicionId = gid
        });
    }

    await db.SaveChangesAsync();
    return Results.Ok();
});


app.MapPost("/api/churrascosGuardar", async (ChurrascoDto dto, AppDb db) =>
{
    var churrasco = new Churrasco
    {
        TipoCarneId = dto.TipoCarneId,
        TerminoCoccionId = dto.TerminoCoccionId,
        Porciones = dto.Porciones,
        PorcionesExtra = dto.PorcionesExtra
    };

    await db.Churrascos.AddAsync(churrasco);
    await db.SaveChangesAsync();

    var churrascoGuarniciones = new List<ChurrascoGuarnicion>();

    foreach (var porcion in dto.PorcionGuarniciones)
    {
        foreach (var guarnicionId in porcion.GuarnicionIds)
        {
            churrascoGuarniciones.Add(new ChurrascoGuarnicion
            {
                ChurrascoId = churrasco.Id,
                GuarnicionId = guarnicionId,
                PorcionNumero = porcion.PorcionNumero
            });
        }
    }

    await db.ChurrascosGuarnicion.AddRangeAsync(churrascoGuarniciones);
    await db.SaveChangesAsync();

    return Results.Ok(new { message = "Churrasco guardado exitosamente" });
});






/// ale inventarios



app.MapGet("/api/inventario/carnes", async (AppDb db) => await db.CarnesInventario.ToListAsync());

app.MapGet("/api/inventario/ingredientes", async (AppDb db) => await db.IngredientesGuarnicion.ToListAsync());

app.MapGet("/api/inventario/dulces_inventario", async (AppDb db) => await db.DulcesUnidad.ToListAsync());

app.MapGet("/api/inventario/dulcesunidad", async (AppDb db) =>
{
    var data = await db.DulcesUnidad
        .Include(du => du.Dulce)
        .ToListAsync();

    return Results.Ok(data);
});




app.MapGet("/api/inventario/empaques", async (AppDb db) => await db.Empaques.ToListAsync());

app.MapGet("/api/inventario/combustible", async (AppDb db) => await db.Combustibles.ToListAsync());

app.MapGet("/api/inventario/dulcescaja", async (AppDb db) =>
{
    var cajas = await db.DulcesCaja
                        .Include(dc => dc.Dulce)
                        .ToListAsync();
    return Results.Ok(cajas);
});
app.MapPost("/api/dulcescaja", async (DulceCaja dulceCaja, AppDb db) =>
{
    db.DulcesCaja.Add(dulceCaja);
    await db.SaveChangesAsync();
    return Results.Created($"/api/inventario/dulcescaja/{dulceCaja.Id}", dulceCaja);
});


// combos aqui
app.MapPost("/api/combos", async (ComboDto dto, AppDb db) =>
{
    var combo = new Combo
    {
        Nombre = dto.Nombre,
        Descripcion = dto.Descripcion,
        Precio = dto.Precio
    };

    await db.Combos.AddAsync(combo);
    await db.SaveChangesAsync(); 

    var churrascos = dto.Churrascos.Select(c => new ComboChurrasco
    {
        ComboId = combo.Id,
        ChurrascoId = c.ChurrascoId,
        Cantidad = c.Cantidad
    });

    var dulcesUnidad = dto.DulcesUnidad.Select(d => new ComboDulceUnidad
    {
        ComboId = combo.Id,
        DulceId = d.DulceId,
        Cantidad = d.Cantidad
    });

    var dulcesCaja = dto.DulcesCaja.Select(d => new ComboDulceCaja
    {
        ComboId = combo.Id,
        DulceId = d.DulceId,
        TamañoCaja = d.TamañoCaja,
        Cantidad = d.Cantidad
    });

    await db.CombosChurrasco.AddRangeAsync(churrascos);
    await db.CombosDulceUnidad.AddRangeAsync(dulcesUnidad);
    await db.CombosDulceCaja.AddRangeAsync(dulcesCaja);


    await db.SaveChangesAsync();

    return Results.Ok(new { message = "Combo guardado correctamente" });
});




app.MapPost("/api/ventas", async (VentaDto dto, AppDb db) =>
{
    if ((dto.Combos == null || !dto.Combos.Any()) && (dto.Churrascos == null || !dto.Churrascos.Any()))
    {
        return Results.BadRequest("La venta debe incluir al menos un combo o un churrasco.");
    }

    Console.WriteLine($"Combos recibidos: {dto.Combos.Count}");
    Console.WriteLine($"Churrascos recibidos: {dto.Churrascos.Count}");

    var venta = new Venta
    {
        FechaHora = DateTime.UtcNow,
        Total = dto.Total
    };

    await db.Ventas.AddAsync(venta);
    await db.SaveChangesAsync();

    var detalles = new List<VentaDetalle>();

    try
    {
  
        foreach (var combo in dto.Combos)
        {
            var comboCompleto = await db.Combos
                .Include(c => c.Churrascos).ThenInclude(cc => cc.Churrasco)
                .Include(c => c.DulcesUnidad)
                .Include(c => c.DulcesCaja)
                .FirstOrDefaultAsync(c => c.Id == combo.ComboId);

            if (comboCompleto == null)
                return Results.BadRequest($"Combo con ID {combo.ComboId} no existe.");

            detalles.Add(new VentaDetalle
            {
                VentaId = venta.Id,
                ComboId = combo.ComboId,
                ChurrascoId = null,
                Cantidad = combo.Cantidad
            });

            foreach (var cc in comboCompleto.Churrascos)
            {
                var carne = await db.CarnesInventario.FindAsync(cc.Churrasco.TipoCarneId);
                if (carne == null)
                    return Results.BadRequest($"No se encontró carne para tipo ID {cc.Churrasco.TipoCarneId}");

                var requerido = cc.Churrasco.Porciones * combo.Cantidad;
                if (carne.StockLibras < requerido)
                    return Results.BadRequest($"Stock insuficiente de carne para el combo {combo.ComboId}");

                carne.StockLibras -= requerido;
            }

            
            foreach (var du in comboCompleto.DulcesUnidad)
            {
                var inventarioUnidad = await db.DulcesUnidad.FirstOrDefaultAsync(d => d.DulceId == du.DulceId);
                if (inventarioUnidad == null || inventarioUnidad.StockUnidades < du.Cantidad * combo.Cantidad)
                    return Results.BadRequest($"Stock insuficiente de dulce unidad en combo {combo.ComboId}");

                inventarioUnidad.StockUnidades -= du.Cantidad * combo.Cantidad;
            }

            
            foreach (var dc in comboCompleto.DulcesCaja)
            {
                var inventarioCaja = await db.DulcesCaja.FirstOrDefaultAsync(d => d.DulceId == dc.DulceId);
                if (inventarioCaja == null || inventarioCaja.StockCajas < dc.Cantidad * combo.Cantidad)
                    return Results.BadRequest($"Stock insuficiente de dulce en caja en combo {combo.ComboId}");

                inventarioCaja.StockCajas -= dc.Cantidad * combo.Cantidad;
            }
        }

        foreach (var churrasco in dto.Churrascos)
        {
            var churrascoEntity = await db.Churrascos.FindAsync(churrasco.ChurrascoId);
            if (churrascoEntity == null)
                return Results.BadRequest($"Churrasco con ID {churrasco.ChurrascoId} no encontrado");

            var carne = await db.CarnesInventario.FindAsync(churrascoEntity.TipoCarneId);
            if (carne == null)
                return Results.BadRequest($"No se encontró carne para churrasco {churrasco.ChurrascoId}");

            var requerido = churrascoEntity.Porciones * churrasco.Cantidad;
            if (carne.StockLibras < requerido)
                return Results.BadRequest($"Stock insuficiente para churrasco {churrasco.ChurrascoId}");

            carne.StockLibras -= requerido;

            detalles.Add(new VentaDetalle
            {
                VentaId = venta.Id,
                ComboId = null,
                ChurrascoId = churrasco.ChurrascoId,
                Cantidad = churrasco.Cantidad
            });
        }

        if (!detalles.Any())
            return Results.BadRequest("No se generó ningún detalle válido para la venta.");

        await db.VentaDetalles.AddRangeAsync(detalles);
        await db.SaveChangesAsync();

        return Results.Ok(new { message = "Venta registrada correctamente", ventaId = venta.Id });
    }
    catch (Exception ex)
    {
        return Results.Problem("Error interno: " + ex.Message);
    }
});



app.MapGet("/api/getCombos", async (AppDb db) =>
{
    var combos = await db.Combos
        .Select(c => new
        {
            c.Id,
            c.Nombre,
            c.Descripcion,
            c.Precio
        })
        .ToListAsync();

    return Results.Ok(combos);
});


app.Run();