using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Model;
using System;
using System.Threading.Tasks;

namespace OrderProducer.Controllers
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
                //Nome que ira aparecer no RabbitMq da filas
                Uri uri = new Uri("rabbitmq://localhost/QueueOfName"); 

                /*Resolve um ISendEndPoint que representa o destino (a fila).
                MassTransit cria/resolve internamente como enviar para esse endereço*/
                var endPoint = await _bus.GetSendEndpoint(uri);

                await endPoint.Send(ticket);
                return Ok();
            }
            return BadRequest();
        }

    }
}
