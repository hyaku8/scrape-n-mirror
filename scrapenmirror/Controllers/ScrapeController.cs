using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using scrapenmirror.Core;
using scrapenmirror.Models;

namespace scrapenmirror.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScrapeController : ControllerBase
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IRepository<string, Scrape> scrapes;

        public ScrapeController(IHttpClientFactory httpClientFactory, IRepository<string,Scrape> scrapes)
        {
            this.httpClientFactory = httpClientFactory;
            this.scrapes = scrapes;
        }

        [HttpPost]
        public async Task<IActionResult> Test([FromBody]ScrapeRequest scrapeRequest)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, scrapeRequest.Url);
            HttpClient httpClient = httpClientFactory.CreateClient();
            HttpResponseMessage response = await httpClient.SendAsync(request);

            Scrape scrape = new Scrape();
            scrape.HttpResponseMessage = response;

            int tries = 0;
            int seed = (int)DateTime.Now.Ticks;
            do
            {
                scrape.Id = Scrape.GenerateId(seed);
                tries++;

            }
            while (scrapes[scrape.Id] != null && tries < 20);
            scrapes.Add(scrape);

            return Ok(scrape);
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

        [HttpGet]
        public IActionResult GetScrapes()
        {
            return Ok(this.scrapes.Values);
        }

        public class ScrapeRequest
        {
            public string Url { get; set; }
        }
    }
}