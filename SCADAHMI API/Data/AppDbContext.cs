using System;
using Microsoft.EntityFrameworkCore;

namespace SCADAHMI_API.Data;

/*
    TODO: This is supposed to be for the SQLite database.
    Command/Event logging will have to be added here eventually. 
*/
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        
    }
}
