using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Mylearningapi.Services;

namespace Mylearningapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly TransientService _transient1;
        private readonly TransientService _transient2;
        private readonly ScopedService _scoped1;
        private readonly ScopedService _scoped2;
        private readonly SingletonService _singleton1;
        private readonly SingletonService _singleton2;

        public OrdersController(
            OrderService orderService,
            TransientService transient1,
            TransientService transient2,
            ScopedService scoped1,
            ScopedService scoped2,
            SingletonService singleton1,
            SingletonService singleton2)
        {
            _orderService = orderService;
            _transient1 = transient1;
            _transient2 = transient2;
            _scoped1 = scoped1;
            _scoped2 = scoped2;
            _singleton1 = singleton1;
            _singleton2 = singleton2;
        }

        [HttpGet("lifetimes")]
        public IActionResult Getservicelifetime()
        {
            return Ok(new
            {
                Transient1 = _transient1.Id,
                Transient2 = _transient2.Id,
                Scoped1 = _scoped1.Id,
                Scoped2 = _scoped2.Id,
                Singleton1 = _singleton1.Id,
                Singleton2 = _singleton2.Id
            });
        }

        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return Ok(new
                {
                    Authenticated = true,
                    Name = User.Identity!.Name,
                    Roles = User.Claims
                        .Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)
                });
            }

            return Ok(new { Authenticated = false });
        }
    
        [HttpGet("admin-area")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAdminArea()
        {
            return Ok("Welcome to the admin area!");
        }
    }
}