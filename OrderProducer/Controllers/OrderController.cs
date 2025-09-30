using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Model;
using System;
using System.Threading.Tasks;

namespace OrderConsumer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IBus _bus;

        public OrderController(IBus bus)
        {
            _bus = bus;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket(Ticket ticket)
        {
            if (ticket != null)
            {
                ticket.Booked = DateTime.Now;
                Uri uri = new Uri("rabbitmq://localhost/QueueOfName"); //Nome que ira aparecer no RabbitMq da filas
                var endPoint = await _bus.GetSendEndpoint(uri);
                await endPoint.Send(ticket);
                return Ok();
            }
            return BadRequest();
        }

    }
}
