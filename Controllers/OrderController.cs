using Microsoft.AspNetCore.Http.HttpResults; // for Ok()
using Microsoft.AspNetCore.Mvc; // for IActionResult
using Mylearningapi.Services;

namespace Mylearningapi.Controllers
{
    // ControllerBase is a base class in ASP.NET Core that provides the fundamental 
    // features needed to build Web API controllers. It’s what gives you access to 
    // helper methods like Ok(), BadRequest(), NotFound(), and model binding — without 
    // pulling in the extra overhead of MVC views.
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderservice;
        public OrderController(OrderService orderservice)
        {
            _orderservice = orderservice;
        }
        [HttpPost("place")]
        public IActionResult Placeorder()
        {
            var message = _orderservice.PlaceOrder();
            // Console.WriteLine($"message : {message}");
            // Ok() is a method on ControllerBase that returns an OkObjectResult.
            return Ok(new {message});
        }
    }
}