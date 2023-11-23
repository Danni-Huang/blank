using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using QuotesApp.Models;
using System.Collections.Generic;
using System.Xml;


namespace QuotesApp.Controllers
{
    [ApiController]
    public class QuoteApiController : Controller
    {
        public QuoteApiController(QuoteContext quoteContext) 
        {
            _quoteContext = quoteContext;
        }

        [HttpGet("/quote-api")]
        public async Task<IActionResult> GetApiHomeResult()
        {
            QuoteApiViewModel viewModel = new QuoteApiViewModel()
            {
                Links = new Dictionary<string, Link>()
                {
                    { "self", new Link() { Rel = "self", Href = GnerateFullUrl("/quote-api") } },
                    { "quotes", new Link() { Rel = "quotes", Href = GnerateFullUrl("/quotes") } },
                    { "quoteById", new Link() { Rel = "quotes", Href = GnerateFullUrl("/quotes/{id}") } },
                    { "tags", new Link() { Rel = "tags", Href = GnerateFullUrl("/tags") } },
                    { "like", new Link() { Rel = "like", Href = GnerateFullUrl("/quote/{id}/like"), Method = "POST" } },
                    { "quotesByRank", new Link() { Rel = "quotes", Href = GnerateFullUrl("/quotes/rank") } },
                    { "tagQuote", new Link() { Rel = "quotes", Href = GnerateFullUrl("/quote/{quoteId}/tag/{tagId}"), Method = "POST" } },
                    { "removeTagWithQuote", new Link() { Rel = "quotes", Href = GnerateFullUrl("/quote/{quoteId}/tag/{tagName}"), Method = "DELETE" } }
                },
                Version = "1.0",
                Creator = "Danni Huang"           
            };
            return Ok(viewModel);
        }


        [HttpGet("/quotes")]
        public async Task<IActionResult> GetAllQuotes([FromQuery(Name = "tagId")] int? tagId)
        {
            if (tagId == null)
            {
                // get all quotes without filtering by tag
                var quotes = await _quoteContext.Quotes
                    .Include(t => t.Likes)
                    .Include(t => t.TagAssignments)
                    .ThenInclude(ta => ta.Tag)                 
                    .ToListAsync();

                List<QuotesResponse> quotesResponse = quotes.Select(q => new QuotesResponse
                {
                    QuoteId = q.QuoteId,
                    Content = q.Content,
                    Author = q.Author,
                    Likes = q.Likes.ToList().Count(),
                    Tags = q.TagAssignments != null
                        ? q.TagAssignments.Where(ta => ta != null && ta.Tag != null && ta.Tag.Name != null).Select(ta => ta.Tag.Name).ToList()
                        : new List<string>()
                }).ToList();

                var tags = await _quoteContext.Tags.Select(t => t.Name).ToListAsync();

                DateTime? quoteLastModified = new DateTime(1970, 1, 1);
                if (quotesResponse.Count > 0)
                {
                    quoteLastModified = await _quoteContext.Quotes.MaxAsync(q => q.LastModified);
                }

                QuoteViewModel viewModel = new QuoteViewModel()
                {
                    Quotes = quotesResponse,
                    QuotesLastModified = quoteLastModified,
                    Tags = tags
                };

                // add last modified to the HTTP last-modified header
                Response.Headers.LastModified = quoteLastModified?.ToUniversalTime().ToString("R");

                return Ok(viewModel);   
            }
            else
            {
                // get quotes by a specific tag
                var quotesByTag = await _quoteContext.Quotes
                    .Include(q => q.TagAssignments)
                    .ThenInclude(ta => ta.Tag)
                    .Include (q => q.Likes)
                    .Where(q => q.TagAssignments.Any(ta => ta.TagId == tagId))
                    .ToListAsync();

                List<QuotesResponse> quotesResponse = quotesByTag.Select(q => new QuotesResponse
                {
                    QuoteId = q.QuoteId,
                    Content = q.Content,
                    Author = q.Author,
                    Likes = q.Likes.ToList().Count(),
                    Tags = q.TagAssignments != null
                        ? q.TagAssignments.Where(ta => ta != null && ta.Tag != null && ta.Tag.Name != null).Select(ta => ta.Tag.Name).ToList()
                        : new List<string>()
                }).ToList();

                var tags = await _quoteContext.Tags.Select(t => t.Name).ToListAsync();
                DateTime? quoteLastModified = new DateTime(1970, 1, 1);
                if (quotesResponse.Count > 0)
                {
                    quoteLastModified = await _quoteContext.Quotes.MaxAsync(q => q.LastModified);
                }

                QuoteViewModel viewModel = new QuoteViewModel()
                {
                    Quotes = quotesResponse,
                    QuotesLastModified = quoteLastModified,
                    Tags = tags
                };

                // add last modified to the HTTP last-modified header
                Response.Headers.LastModified = quoteLastModified?.ToUniversalTime().ToString("R");

                return Ok(viewModel);                  
            }         
        }

        [HttpGet("/quotes/{id}")]
        public async Task<IActionResult> GetQuoteById(int id)
        {
            // Using the Where method to filter to the specific quote by ID...
            QuotesResponse quote = await _quoteContext.Quotes
                    .Include(t => t.Likes)
                    .Include(t => t.TagAssignments)
                    .Where(t => t.QuoteId == id)
                    .Select(q => new QuotesResponse
                    {
                        QuoteId = q.QuoteId,
                        Content = q.Content,
                        Author = q.Author,
                        Likes = q.Likes.ToList().Count(),
                        Tags = q.TagAssignments.Select(ta => ta.Tag.Name).ToList()
                    })
                    .FirstOrDefaultAsync();

            if (quote == null)
                return NotFound();

            return Ok(quote);
        }

        [HttpPost("/quotes")]
        public async Task<IActionResult> AddNewQuote([FromBody] NewQuoteRequest newQuoteRequest)
        {
            if (newQuoteRequest == null)
            {
                return BadRequest("Invalid request data.");
            }

            if (newQuoteRequest.Author == null || String.IsNullOrEmpty(newQuoteRequest.Author))
            {
                newQuoteRequest.Author = "Anonymous";
            }

            Quote existingQuote = _quoteContext.Quotes.FirstOrDefault(q => q.Content == newQuoteRequest.Content);
            if (existingQuote != null)
            {
                return BadRequest("The quote is already exist.");
            }

            Quote quote = new Quote()
            {
                Content = newQuoteRequest.Content,
                Author = newQuoteRequest.Author
            };

            _quoteContext.Quotes.Add(quote);
            await _quoteContext.SaveChangesAsync();

            QuotesResponse quoteResponse = new QuotesResponse
            {
                QuoteId = quote.QuoteId,
                Content = quote.Content,
                Author = quote.Author,
                Likes = _quoteContext.Like.Where(l => l.QuoteId == quote.QuoteId).ToList().Count(),
                Tags = _quoteContext.TagAssignments.Where(ta => ta.QuoteId == quote.QuoteId).Select(ta => ta.Tag.Name).ToList()
            };

            return CreatedAtAction(nameof(GetQuoteById), new {id = quoteResponse.QuoteId}, quoteResponse);
        }

        [HttpPut("/quotes/{id}")]
        public async Task<IActionResult> EditQuoteById(int id, [FromBody] NewQuoteRequest newQuoteRequest)
        {
            Quote existingQuote = await _quoteContext.Quotes.FirstOrDefaultAsync(a => a.QuoteId == id);

            if (existingQuote == null)
            {
                return NotFound("The Quote is not found.");
            }

            existingQuote.Content = newQuoteRequest.Content;
            existingQuote.Author = newQuoteRequest.Author;
            existingQuote.LastModified = DateTime.Now;

            await _quoteContext.SaveChangesAsync();

            QuotesResponse quoteResponse = new QuotesResponse
            {
                QuoteId = existingQuote.QuoteId,
                Content = existingQuote.Content,
                Author = existingQuote.Author,
                Likes = await _quoteContext.Like.CountAsync(l => l.QuoteId == existingQuote.QuoteId),
                Tags = await _quoteContext.TagAssignments.Where(ta => ta.QuoteId == existingQuote.QuoteId).Select(ta => ta.Tag.Name).ToListAsync()
            };

            return Ok(quoteResponse);
        }

        [HttpGet("/tags")]
        public async Task<IActionResult> GetAllTags()
        {
            List<TagsResponse> tags = await _quoteContext.Tags
                .Select(t => new TagsResponse()
                {
                    TagId = t.TagId,
                    Name = t.Name
                }).ToListAsync();

            return Ok(tags);
        }

        [HttpPost("/tags")]
        public async Task<IActionResult> AddNewTag([FromBody] NewTagRequest newTagRequest)
        {
            Tag existingTag = _quoteContext.Tags.FirstOrDefault(t => t.Name == newTagRequest.Name);
            if (existingTag != null)
            {
                return BadRequest("The tag is already exist.");
            }

            Tag tag = new Tag()
            {
                Name = newTagRequest.Name
            };

            _quoteContext.Tags.Add(tag);
            await _quoteContext.SaveChangesAsync();

            TagsResponse tagResponse = new TagsResponse
            {
                TagId = tag.TagId,
                Name = tag.Name
            };

            return CreatedAtAction(nameof(GetAllTags), new { id = tagResponse.TagId }, tagResponse);
        }

        [HttpPost("/quote/{id}/like")]
        public async Task<IActionResult> AddLike(int id)
        {
            Quote quote = await _quoteContext.Quotes.FirstOrDefaultAsync(q => q.QuoteId == id);

            if (quote == null)
            {
                return NotFound("Quote not found.");
            }

            // TODO: replace with the actual userID in assignment 4
            int userId = 123;

            // create a new Like
            Like newLike = new Like
            {
                UserId = userId,
                QuoteId = id
            };

            _quoteContext.Like.Add(newLike);
            await _quoteContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuoteRank), new { id = newLike.LikeId }, newLike);
        }

        // this function is return the most liked quotes, default is 10
        [HttpGet("/quotes/rank")]
        public async Task<IActionResult> GetQuoteRank([FromQuery] int limit = 10)
        {
            List<QuotesResponse> topLikedQuotes = await _quoteContext.Quotes
                .GroupJoin(
                    _quoteContext.Like,
                    quote => quote.QuoteId,
                    like => like.QuoteId,
                    (quote, likes) => new { Quote = quote, LikeCount = likes.Count() }
                )
                .OrderByDescending(result => result.LikeCount)
                .Take(limit)
                .Select(result => new QuotesResponse
                {
                    QuoteId = result.Quote.QuoteId,
                    Content = result.Quote.Content,
                    Author = result.Quote.Author,
                    Likes = result.LikeCount,
                    Tags = _quoteContext.TagAssignments.Where(ta => ta.QuoteId == result.Quote.QuoteId).Select(ta => ta.Tag.Name).ToList()
                })
                .ToListAsync();

            var tags = await _quoteContext.Tags.Select(t => t.Name).ToListAsync();

            DateTime? quoteLastModified = new DateTime(1970, 1, 1);
            if (topLikedQuotes.Count > 0)
            {
                quoteLastModified = await _quoteContext.Quotes.MaxAsync(q => q.LastModified);
            }

            QuoteViewModel viewModel = new QuoteViewModel()
            {
                Quotes = topLikedQuotes,
                QuotesLastModified = quoteLastModified,
                Tags = tags
            };

            // add last modified to the HTTP last-modified header
            Response.Headers.LastModified = quoteLastModified?.ToUniversalTime().ToString("R");

            return Ok(viewModel);
        }

        [HttpPost("/quote/{quoteId}/tag/{tagId}")]
        public async Task<IActionResult> AddTagWithQuote(int quoteId, int tagId)
        {
            // find the quote
            Quote quote = await _quoteContext.Quotes.FirstOrDefaultAsync(q => q.QuoteId == quoteId);

            if (quote == null)
            {
                return NotFound("Quote not found.");
            }

            // find the tag
            Tag tag = await _quoteContext.Tags.FirstOrDefaultAsync(q => q.TagId == tagId);

            if (tag == null)
            {
                return NotFound("Tag not found.");
            }

            // check if the tag is already assigned to Quote
            bool tagAlreadyAssigned = await _quoteContext.TagAssignments.AnyAsync(ta => ta.QuoteId == quoteId && ta.TagId == tagId);

            if (tagAlreadyAssigned) 
            {
                return BadRequest("Tag is already assigned to the quote");
            }

            // create a new TagAssignment
            TagAssignment tagAssignment = new TagAssignment
            {
                QuoteId = quoteId,
                TagId = tagId
            };

            quote.LastModified = DateTime.Now;

            // add TagAssignment to the context
            _quoteContext.TagAssignments.Add(tagAssignment);
            await _quoteContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("/quote/{quoteId}/tag/{tagName}")]
        public async Task<IActionResult> RemoveTagWithQuote(int quoteId, string tagName)
        {
            // find the quote
            Quote quote = await _quoteContext.Quotes.FirstOrDefaultAsync(q => q.QuoteId == quoteId);

            if (quote == null)
            {
                return NotFound("Quote not found.");
            }

            // find the tagId based on the tagName
            int tagId = await _quoteContext.Tags
                .Where(t => t.Name == tagName)
                .Select(t => t.TagId)
                .FirstOrDefaultAsync();

            // find the tag
            // Tag tag = await _quoteContext.Tags.FirstOrDefaultAsync(q => q.TagId == tagId);

            if (tagId == 0)
            {
                return NotFound("Tag not found.");
            }

            // check if the tag is already assigned to Quote
            bool tagAlreadyAssigned = await _quoteContext.TagAssignments.AnyAsync(ta => ta.QuoteId == quoteId && ta.TagId == tagId);

            if (!tagAlreadyAssigned)
            {
                return BadRequest("Tag is not assigned to the quote, and you cannot remove it from quote.");
            }

            // find tag with quote in TagAssignment
            TagAssignment tagAssignment = _quoteContext.TagAssignments.FirstOrDefault(ta => ta.QuoteId == quoteId && ta.TagId == tagId);           

            // remove TagAssignment to the context
            _quoteContext.TagAssignments.Remove(tagAssignment);
            await _quoteContext.SaveChangesAsync();

            return Ok();
        }

        private string GnerateFullUrl(string path)
        {
            return $"{Request.Scheme}://{Request.Host}{path}";
        }

        private QuoteContext _quoteContext;
    }
}