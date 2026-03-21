using CommunityToolkit.Datasync.Server;
using CommunityToolkit.Datasync.Server.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Models;

namespace API.Controllers
{
    [Route("tables/[controller]")]
    public class GroceryItemController : TableController<GroceryItem>
    {
        public GroceryItemController(AppDbContext context, ILogger<GroceryItemController> logger)
        {
            Repository = new EntityTableRepository<GroceryItem>(context);
            Options = new TableControllerOptions { EnableSoftDelete = true, PageSize = 100 };
            Logger = logger;
        }
    }
}
