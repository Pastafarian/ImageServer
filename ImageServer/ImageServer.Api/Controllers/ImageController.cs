using System.Threading;
using System.Threading.Tasks;
using ImageServer.Application.Handlers.Query.GetImage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ImageServer.Api.Controllers
{
    [Route("[controller]")]
    public class ImageController : Controller
    {
        private readonly IMediator _mediator;

        public ImageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(GetImageRequest request)
        {
            var result = await _mediator.Send(new GetImage.Query(request), CancellationToken.None);

            if (result.ResponseType == ResponseType.BadRequest) 
                return BadRequest(result.Message);

            if (result.ResponseType == ResponseType.NotFound) 
                return NotFound();

            return File(result.Content, result.ContentType);
        }
    }
}
