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




//dash
app.MapGet("/api/dashboard/getCombos", async (AppDb db) =>
{
    return await db.Combos.ToListAsync();
});


app.MapGet("/api/dashboard/carnes", async (AppDb db) =>
{
    return await db.CarnesInventario.ToListAsync();
});

app.MapGet("/api/dashboard/dulcesunidad", async (AppDb db) =>
{
    return await db.DulcesUnidad
        .Include(d => d.Dulce)
        .ToListAsync();
});


app.MapGet("/api/dashboard/total-churrascos", async (AppDb db) =>
{
    var total = await db.Churrascos.CountAsync();
    return Results.Ok(total);
});



app.MapGet("/api/dashboard/carne-mas-usada", async (AppDb db) =>
{
    var carneMasUsada = await db.Churrascos
        .GroupBy(c => c.TipoCarneId)
        .Select(g => new
        {
            TipoCarneId = g.Key,
            Usos = g.Count()
        })
        .OrderByDescending(g => g.Usos)
        .Join(db.TiposCarne,
              g => g.TipoCarneId,
              tc => tc.Id,
              (g, tc) => new { tc.Nombre, g.Usos })
        .FirstOrDefaultAsync();

    return Results.Ok(carneMasUsada);
});


app.MapPost("/api/ventas", async (VentaDto request, AppDb db) =>
{
    var venta = new Venta
    {
        FechaHora = DateTime.UtcNow,
        Total = request.Total,
        Detalles = new List<VentaDetalle>()
    };


    foreach (var combo in request.Combos)
    {
        var comboEntity = await db.Combos.FindAsync(combo.ComboId);
        if (comboEntity is null) continue;

        venta.Detalles.Add(new VentaDetalle
        {
            TipoProducto = "combo",
            ProductoId = combo.ComboId,
            Cantidad = combo.Cantidad,
            PrecioUnitario = comboEntity.Precio * combo.Cantidad
        });
    }


    foreach (var churrasco in request.Churrascos)
    {
        var churrascoEntity = await db.Churrascos.FindAsync(churrasco.ChurrascoId);
        if (churrascoEntity is null) continue;

        venta.Detalles.Add(new VentaDetalle
        {
            TipoProducto = "churrasco",
            ProductoId = churrasco.ChurrascoId,
            Cantidad = churrasco.Cantidad,
            PrecioUnitario = 40 * churrasco.Cantidad
        });
    }

    db.Ventas.Add(venta);
    await db.SaveChangesAsync();

    return Results.Ok(new { mensaje = "Venta registrada correctamente", venta.Id });
});



app.MapGet("/api/dashboard/ventas-mensuales", async (AppDb db) =>
{
    var resumenMensual = await db.Ventas
        .GroupBy(v => new { v.FechaHora.Year, v.FechaHora.Month })
        .Select(g => new
        {
            Anio = g.Key.Year,
            Mes = g.Key.Month,
            Total = g.Sum(v => v.Total)
        })
        .ToListAsync();

    var resultado = resumenMensual
        .OrderBy(r => r.Anio).ThenBy(r => r.Mes)
        .Select(r => new
        {
            Mes = $"{r.Mes:D2}/{r.Anio}",  
            Total = r.Total
        });

    return Results.Ok(resultado);
});



app.MapGet("/api/dashboard/combos-mas-vendidos", async (AppDb db) =>
{
    var topCombos = await db.VentaDetalles
        .Where(v => v.TipoProducto == "combo")
        .GroupBy(v => v.ProductoId)
        .Select(g => new {
            ComboId = g.Key,
            Nombre = db.Combos.Where(c => c.Id == g.Key).Select(c => c.Nombre).FirstOrDefault(),
            TotalVendidos = g.Sum(v => v.Cantidad)
        })
        .OrderByDescending(c => c.TotalVendidos)
        .Take(3)
        .ToListAsync();

    return Results.Ok(topCombos);
});




app.MapGet("/api/proveedores", async (AppDb db) =>
    await db.Proveedores.ToListAsync());

app.MapPost("/api/proveedores", async (Proveedor proveedor, AppDb db) =>
{
    db.Proveedores.Add(proveedor);
    await db.SaveChangesAsync();
    return Results.Ok(proveedor);
});

app.MapPut("/api/churrascos/{id}/tiempo", async (int id, TiempoPreparacionDto dto, AppDb db) =>
{
    var churrasco = await db.Churrascos.FindAsync(id);
    if (churrasco is null) return Results.NotFound();

    churrasco.TiempoPreparacionMinutos = dto.Minutos;
    await db.SaveChangesAsync();
    return Results.Ok(churrasco);
});
app.Run();