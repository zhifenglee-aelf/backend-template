using MongoDB.Driver;
using Volo.Abp.MongoDB;

namespace MyTemplate.Books;

public class BookStoreMongoDbContext : AbpMongoDbContext
{
    public IMongoCollection<Book> Books => Collection<Book>();
}
