using CodingTest;
using Microsoft.AspNetCore.Mvc;

namespace CodingTest
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoriesController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public StoriesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetBestStories(int n)
        {
            if (n <= 0)
            {
                return BadRequest("Invalid value for 'n'");
            }

            var client = _httpClientFactory.CreateClient("CodingTest");
            var bestStoryIds = await client.GetFromJsonAsync<int[]>("beststories.json");

            var stories = new List<StoryDisplay>();

            foreach (var storyId in bestStoryIds.Take(n))
            {
                var story = await client.GetFromJsonAsync<Story>($"item/{storyId}.json");
                if (story != null)
                {
                    StoryDisplay strDisp = new StoryDisplay();
                    strDisp.Title = story.Title;
                    strDisp.Uri = story.Uri;
                    strDisp.PostedBy = story.PostedBy;
                    strDisp.Time = DateTimeOffset.FromUnixTimeSeconds(story.Time).UtcDateTime;
                    strDisp.CommentCount = story.CommentCount;
                    strDisp.Score = story.Score;
                    stories.Add(strDisp);
                }
            }

            stories.Sort((a, b) => b.Score.CompareTo(a.Score));

            return Ok(stories);
        }
    }

}