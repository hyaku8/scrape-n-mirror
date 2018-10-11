using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using scrapenmirror.Core;
using scrapenmirror.Models;
using System.IO;
using System.Text;
using System.Net.Http;

namespace scrapenmirror.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MirrorController : ControllerBase
    {
        private readonly IRepository<string, Scrape> scrapes;
        public MirrorController(IRepository<string, Scrape> scrapes)
        {
            this.scrapes = scrapes;
        }

        [HttpGet]
        public IActionResult GetScrapes()
        {
            return Ok(this.scrapes.Values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetScrape([FromRoute]string id)
        {
            Scrape scrape = this.scrapes.Where(x => x.Id == id).FirstOrDefault();
            if (scrape == null)
            {
                return NotFound();
            }
            else
            {
                Response.StatusCode = (int)scrape.HttpResponseMessage.StatusCode;
                foreach (var header in scrape.HttpResponseMessage.Headers)
                {
                    Response.Headers[header.Key] = header.Value.ToArray();
                }
                foreach (var header in scrape.HttpResponseMessage.Content.Headers)
                {
                    Response.Headers[header.Key] = header.Value.ToArray();
                }
                await scrape.HttpResponseMessage.Content.CopyToAsync(Response.Body);
            }
            return Ok();
        }
    }
}