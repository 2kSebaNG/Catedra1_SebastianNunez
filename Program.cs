using ebooks_dotnet8_api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("ebooks"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

var ebooks = app.MapGroup("api/ebook");

// TODO: Add more routes
ebooks.MapPost("/", CreateEBookAsync);
ebooks.MapGet("/",GetAllEbooks);
ebooks.MapPut("{id}",UpdateEbook);
ebooks.MapPut("{id}/change-availability", ChangeAvailaBility);
ebooks.MapPut("{id}/increment-stock",IncrementStock);
ebooks.MapPost("purchase",PurchaseEbook);
ebooks.MapDelete("{id}",DeleteEbook);

app.Run();

//1 Funcionando (validar)
async Task<IResult> CreateEBookAsync(CreateEbookDto createEbookDto,DataContext context)
{
    var existingEbook = await context.EBooks.Where(e => e.Title == createEbookDto.Title && e.Author == createEbookDto.Author).FirstOrDefaultAsync();

    if(existingEbook != null){
        return TypedResults.Conflict("Ya existe un libro con ese titulo y autor.");
    }

    var ebook = new EBook{
        Title = createEbookDto.Title,
        Author = createEbookDto.Author,
        Genre = createEbookDto.Genre,
        Format = createEbookDto.Format,
        IsAvailable = createEbookDto.IsAvailable,
        Price = createEbookDto.Price,
        Stock = createEbookDto.Stock  
    };

    await context.AddAsync(ebook);
    await context.SaveChangesAsync();

    return TypedResults.Created($"/ebooks",ebook);
}

//2 (Falta busqueda)
async Task<IResult> GetAllEbooks([FromQuery] string? genre, [FromQuery] string? format, [FromQuery] string? author, DataContext context){

    var ebooks = await context.EBooks.Where(e => e.IsAvailable == true).OrderBy(e => e.Title).ToListAsync();

    if(ebooks is null){
        return TypedResults.BadRequest("La lista de libros no existe");
    }

    if(!string.IsNullOrEmpty(genre)){
      return TypedResults.Ok(ebooks.Where(e => e.Genre == genre));
    }

    return TypedResults.Ok(ebooks);
}

//3 Funcionando (validar)
async Task<IResult> UpdateEbook(int id,UpdateEbookDto updateEbookDto,DataContext context)
{
    var ebook = await context.EBooks.FindAsync(id);

    if(ebook is null){
        return TypedResults.NotFound("El libro a actualizar no existe");
    }

    ebook.Title = updateEbookDto.Title ?? ebook.Title;
    ebook.Author = updateEbookDto.Author ?? ebook.Author;
    ebook.Genre = updateEbookDto.Genre ?? ebook.Genre;
    ebook.Format = updateEbookDto.Format ?? ebook.Format;
    ebook.Price = updateEbookDto.Price ?? ebook.Price;

    await context.SaveChangesAsync();

    return TypedResults.Ok(ebook);
}

//4 Funcionando
async Task<IResult> ChangeAvailaBility(int id,DataContext context)
{
    var ebook = await context.EBooks.FindAsync(id);

    if(ebook is null){
        return TypedResults.NotFound("El libro a actualizar no existe");
    }

    ebook.IsAvailable = !ebook.IsAvailable;

    await context.SaveChangesAsync();

    return TypedResults.Ok(ebook);
}

//5 
async Task<IResult> IncrementStock(int id, IncrementStockDto incrementStockDto,DataContext context)
{
    var ebook = await context.EBooks.FindAsync(id);

    if(ebook is null){
        return TypedResults.NotFound("El libro a actualizar no existe");
    }

    if(incrementStockDto is null){
        return TypedResults.BadRequest("El stock a agregar es nulo");
    }

    if(incrementStockDto.Stock < 0){
        return TypedResults.BadRequest("El stock no puede ser menor a 0");
    }

    ebook.Stock += incrementStockDto.Stock;

    await context.SaveChangesAsync();

    return TypedResults.Ok(ebook);
}

//6 Funcionando (validar)
async Task<IResult> PurchaseEbook(Purchase purchase,DataContext context)
{

    if(purchase is null){
        return TypedResults.BadRequest("La compra no puede ser nula");
    }

    var ebook = await context.EBooks.FindAsync(purchase.EbookId);

    if(ebook is null){
        return TypedResults.NotFound("El libro a comprar no existe");
    }

    if(!ebook.IsAvailable){
        return TypedResults.BadRequest("El libro no se encuentra disponible");
    }

    if(ebook.Stock < purchase.Quantity){
        return TypedResults.BadRequest("No hay suficiente stock del libro a comprar");
    }

    var realTotalPrice = ebook.Price*purchase.Quantity;

    if(realTotalPrice != purchase.TotalPrice){
        return TypedResults.BadRequest("El precio total ingresado es incorrecto");
    }

    ebook.Stock -= purchase.Quantity;

    await context.SaveChangesAsync();

    return TypedResults.Ok(ebook);
}

//7
async Task<IResult> DeleteEbook(int id,DataContext context)
{
    var ebook = await context.EBooks.FindAsync(id);

    if(ebook is null){
        return TypedResults.NotFound("El libro a eliminar no existe");
    }

    context.EBooks.Remove(ebook);
    await context.SaveChangesAsync();

    return TypedResults.Ok(ebook);
}