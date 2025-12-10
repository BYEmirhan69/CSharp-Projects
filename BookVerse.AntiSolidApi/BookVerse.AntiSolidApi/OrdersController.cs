using Microsoft.AspNetCore.Mvc;
using System;


[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderManager _orderManager = new OrderManager();

    [HttpPost]
    public IActionResult CreateOrder([FromBody] Order order)
    {
        _orderManager.ProcessOrder(order);
        return Ok("Sipariþ baþarýyla iþlendi.");
    }
}
