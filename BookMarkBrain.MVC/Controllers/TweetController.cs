using BookMarkBrain.MVC.Models.Tweet;
using Microsoft.AspNetCore.Mvc;
using BookMarkBrain.MVC.Services.APIServiceInterfaces;
using BookMarkBrain.MVC.Models.Common;
using BookMarkBrain.Core.DTOs.Hashtag;
using BookMarkBrain.Core.ServiceInterfaces;
using BookMarkBrain.Core.DTOs.Category;
using BookMarkBrain.Core.DTOs.TweetHashtag;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookMarkBrain.MVC.Controllers;
public class TweetController : BaseController
{
    private readonly ITweetApiService _tweetApiService;
    private readonly ICategoryApiService _categoryApiService;
    private readonly IHashtagApiService _hashtagApiService;
    private readonly ITweetHashtagApiService _tweetHashtagApiService;

    public TweetController(
        ITweetApiService tweetApiService,
        ICategoryApiService categoryApiService,
        IHashtagApiService hashtagApiService,
        ITweetHashtagApiService tweetHashtagApiService,
        IApiService apiService,
        ILogger<TweetController> logger)
        : base(apiService, logger)
    {
        _tweetApiService = tweetApiService ?? throw new ArgumentNullException(nameof(tweetApiService));
        _categoryApiService = categoryApiService ?? throw new ArgumentNullException(nameof(categoryApiService));
        _hashtagApiService = hashtagApiService ?? throw new ArgumentNullException(nameof(hashtagApiService));
        _tweetHashtagApiService = tweetHashtagApiService ?? throw new ArgumentNullException(nameof(tweetHashtagApiService));
    }

    [HttpGet]
    public async Task<IActionResult> Index(TweetSearchViewModel searchModel, int page = 1, int pageSize = 10)
    {
        var viewModel = new TweetListViewModel
        {
            SearchCriteria = searchModel,
            Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                PageSize = pageSize
            }
        };

        try
        {
            List<TweetViewModel> tweets = new List<TweetViewModel>();

            // Apply filters based on search criteria
            if (!string.IsNullOrEmpty(searchModel.SearchTerm))
            {
                tweets = await _tweetApiService.SearchTweetsAsync(searchModel.SearchTerm);
            }
            else if (searchModel.CategoryId.HasValue)
            {
                tweets = await _tweetApiService.GetTweetsByCategoryAsync(searchModel.CategoryId.Value);
            }
            else if (!string.IsNullOrEmpty(searchModel.PlatformName))
            {
                tweets = await _tweetApiService.GetTweetsByPlatformAsync(searchModel.PlatformName);
            }
            else if (!string.IsNullOrEmpty(searchModel.HashtagName))
            {
                // Get hashtag by name
                var hashtagResult = await _hashtagApiService.GetHashtagByNameAsync(searchModel.HashtagName);

                if (hashtagResult.Success && hashtagResult.Data != null)
                {
                    // Get tweets with this hashtag
                    var tweetHashtagsResult = await _tweetHashtagApiService.GetByHashtagIdWithDetailsAsync(hashtagResult.Data.Id);

                    if (tweetHashtagsResult.Success && tweetHashtagsResult.Data != null)
                    {
                        // Get all the tweets
                        List<TweetViewModel> hashtagTweets = new List<TweetViewModel>();
                        foreach (var tweetHashtag in tweetHashtagsResult.Data)
                        {
                            var tweet = await _tweetApiService.GetTweetByIdAsync(tweetHashtag.TweetId);
                            if (tweet != null)
                            {
                                hashtagTweets.Add(tweet);
                            }
                        }
                        tweets = hashtagTweets;
                    }
                }
            }
            else
            {
                tweets = await _tweetApiService.GetAllTweetsAsync();
            }

            // Apply seen status filter if specified
            if (searchModel.SeenStatus.HasValue)
            {
                tweets = tweets.Where(t => t.IsSeen == searchModel.SeenStatus.Value).ToList();
            }

            // Get total count for pagination
            var totalItems = tweets.Count;

            // Apply pagination
            viewModel.Tweets = tweets
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            viewModel.Pagination.TotalItems = totalItems;

            // Load category names for each tweet
            foreach (var tweet in viewModel.Tweets)
            {
                if (tweet.CategoryId.HasValue)
                {
                    var category = await _categoryApiService.GetCategoryByIdAsync(tweet.CategoryId.Value);
                    if (category != null)
                    {
                        tweet.CategoryName = category.Name;
                    }
                }

                // Load hashtags for each tweet
                var hashtagResult = await _tweetHashtagApiService.GetByTweetIdAsync(tweet.Id);
                tweet.Hashtags = new List<string>();

                if (hashtagResult.Success && hashtagResult.Data != null)
                {
                    foreach (var tweetHashtag in hashtagResult.Data)
                    {
                        var hashtag = await _hashtagApiService.GetHashtagByIdAsync(tweetHashtag.HashtagId);
                        if (hashtag.Success && hashtag.Data != null)
                        {
                            tweet.Hashtags.Add(hashtag.Data.Name);
                        }
                    }
                }
            }

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching tweets");
            SetNotification("Error occurred while fetching tweets. Please try again.");
            return View(viewModel);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var tweet = await _tweetApiService.GetTweetByIdAsync(id);
            if (tweet == null)
            {
                SetNotification("Tweet not found");
                return RedirectToAction(nameof(Index));
            }

            // Get category information if available
            if (tweet.CategoryId.HasValue)
            {
                var category = await _categoryApiService.GetCategoryByIdAsync(tweet.CategoryId.Value);
                if (category != null)
                {
                    tweet.CategoryName = category.Name;
                }
            }

            // Get hashtags for this tweet
            var hashtagResult = await _tweetHashtagApiService.GetByTweetIdWithDetailsAsync(id);
            tweet.Hashtags = new List<string>();

            if (hashtagResult.Success && hashtagResult.Data != null)
            {
                foreach (var tweetHashtag in hashtagResult.Data)
                {
                    tweet.Hashtags.Add(tweetHashtag.HashtagName);
                }
            }

            var viewModel = new TweetDetailsViewModel
            {
                Tweet = tweet
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching tweet details for ID: {TweetId}", id);
            SetNotification("Error occurred while fetching tweet details. Please try again.");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        try
        {
            var viewModel = new CreateTweetViewModel();

            // Get categories for dropdown
            var categories = await _categoryApiService.GetAllCategoriesAsync();
            ViewBag.Categories = categories;

            // Get popular hashtags for suggestions
            var popularHashtags = await _hashtagApiService.GetPopularHashtagsAsync();
            if (popularHashtags.Success && popularHashtags.Data != null)
            {
                ViewBag.PopularHashtags = popularHashtags.Data.Select(h => h.Name).ToList();
            }

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while preparing tweet creation form");
            SetNotification("Error occurred while preparing the creation form. Please try again.");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTweetViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryApiService.GetAllCategoriesAsync();
                ViewBag.Categories = categories;

                var popularHashtags = await _hashtagApiService.GetPopularHashtagsAsync();
                if (popularHashtags.Success && popularHashtags.Data != null)
                {
                    ViewBag.PopularHashtags = popularHashtags.Data.Select(h => h.Name).ToList();
                }

                return View(viewModel);
            }

            // Create the tweet
            var createdTweet = await _tweetApiService.CreateTweetAsync(viewModel);
            if (createdTweet == null)
            {
                SetNotification("Failed to create tweet. Please try again.");
                return View(viewModel);
            }

            // Process and add hashtags
            if (viewModel.HashtagNames != null && viewModel.HashtagNames.Any())
            {
                foreach (var hashtagName in viewModel.HashtagNames)
                {
                    if (string.IsNullOrWhiteSpace(hashtagName))
                        continue;

                    // Check if hashtag exists, create if not
                    var trimmedName = hashtagName.Trim().StartsWith("#") ? hashtagName.Trim() : $"#{hashtagName.Trim()}";
                    var hashtagResult = await _hashtagApiService.GetHashtagByNameAsync(trimmedName);

                    Guid hashtagId;
                    if (!hashtagResult.Success || hashtagResult.Data == null)
                    {
                        // Create new hashtag
                        var createResult = await _hashtagApiService.CreateHashtagAsync(new BookMarkBrain.Core.DTOs.Hashtag.CreateHashtagDto
                        {
                            Name = trimmedName,
                            Description = $"Hashtag for {trimmedName}",
                            IsPopular = false
                        });

                        if (!createResult.Success || createResult.Data == null)
                            continue;

                        hashtagId = createResult.Data.Id;
                    }
                    else
                    {
                        hashtagId = hashtagResult.Data.Id;
                        // Increment usage count
                        await _hashtagApiService.IncrementUsageCountAsync(hashtagId);
                    }

                    // Associate hashtag with tweet
                    await _tweetHashtagApiService.CreateTweetHashtagAsync(new CreateTweetHashtagDto
                    {
                        TweetId = createdTweet.Id,
                        HashtagId = hashtagId
                    });
                }
            }

            SetNotification("Tweet created successfully");
            return RedirectToAction(nameof(Details), new { id = createdTweet.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating tweet");
            ModelState.AddModelError("", "An error occurred while creating the tweet. Please try again.");

            var categories = await _categoryApiService.GetAllCategoriesAsync();
            ViewBag.Categories = categories;

            return View(viewModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> ExtractTweet()
    {
        return View(new ExtractTweetViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExtractTweet(ExtractTweetViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var extractedTweet = await _tweetApiService.ExtractTweetFromUrlAsync(viewModel.Url);
            if (extractedTweet == null)
            {
                SetNotification("Failed to extract tweet from the provided URL. Please check the URL and try again.");
                return View(viewModel);
            }

            SetNotification("Tweet extracted successfully");
            return RedirectToAction(nameof(Details), new { id = extractedTweet.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while extracting tweet from URL: {Url}", viewModel.Url);
            ModelState.AddModelError("", "An error occurred while extracting the tweet. Please try again.");
            return View(viewModel);
        }
    }

    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var tweet = await _tweetApiService.GetTweetByIdAsync(id);
            if (tweet == null)
            {
                SetNotification("Tweet not found");
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new UpdateTweetViewModel
            {
                Id = tweet.Id,
                Content = tweet.Content,
                AuthorUsername = tweet.AuthorUsername,
                OriginalUrl = tweet.OriginalUrl,
                TweetDate = tweet.TweetDate,
                ImageUrl = tweet.ImageUrl,
                IsSeen = tweet.IsSeen,
                PlatformName = tweet.PlatformName,
                CategoryId = tweet.CategoryId
            };

            // Get hashtags for this tweet
            var hashtagResult = await _tweetHashtagApiService.GetByTweetIdWithDetailsAsync(id);
            viewModel.HashtagNames = new List<string>();

            if (hashtagResult.Success && hashtagResult.Data != null)
            {
                foreach (var tweetHashtag in hashtagResult.Data)
                {
                    viewModel.HashtagNames.Add(tweetHashtag.HashtagName);
                }
            }

            // Get categories for dropdown
            var categories = await _categoryApiService.GetAllCategoriesAsync();
            ViewBag.Categories = categories;

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while preparing tweet edit form for ID: {TweetId}", id);
            SetNotification("Error occurred while preparing the edit form. Please try again.");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost("{id:guid}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateTweetViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryApiService.GetAllCategoriesAsync();
                ViewBag.Categories = categories;
                return View(viewModel);
            }

            // Update the tweet
            var updatedTweet = await _tweetApiService.UpdateTweetAsync(viewModel);
            if (updatedTweet == null)
            {
                SetNotification("Failed to update tweet. Please try again.");
                return View(viewModel);
            }

            // Get existing hashtags
            var existingHashtagsResult = await _tweetHashtagApiService.GetByTweetIdWithDetailsAsync(id);
            var existingHashtags = existingHashtagsResult.Success ? existingHashtagsResult.Data?.ToList() : new List<TweetHashtagWithDetailsDto>();
            var existingHashtagNames = existingHashtags.Select(h => h.HashtagName).ToList();

            // Process hashtags changes
            if (viewModel.HashtagNames != null)
            {
                // Add new hashtags
                foreach (var hashtagName in viewModel.HashtagNames)
                {
                    if (string.IsNullOrWhiteSpace(hashtagName))
                        continue;

                    var trimmedName = hashtagName.Trim().StartsWith("#") ? hashtagName.Trim() : $"#{hashtagName.Trim()}";

                    // Skip if hashtag already exists
                    if (existingHashtagNames.Contains(trimmedName))
                        continue;

                    // Check if hashtag exists, create if not
                    var hashtagResult = await _hashtagApiService.GetHashtagByNameAsync(trimmedName);

                    Guid hashtagId;
                    if (!hashtagResult.Success || hashtagResult.Data == null)
                    {
                        // Create new hashtag
                        var createResult = await _hashtagApiService.CreateHashtagAsync(new BookMarkBrain.Core.DTOs.Hashtag.CreateHashtagDto
                        {
                            Name = trimmedName,
                            Description = $"Hashtag for {trimmedName}",
                            IsPopular = false
                        });

                        if (!createResult.Success || createResult.Data == null)
                            continue;

                        hashtagId = createResult.Data.Id;
                    }
                    else
                    {
                        hashtagId = hashtagResult.Data.Id;
                        // Increment usage count
                        await _hashtagApiService.IncrementUsageCountAsync(hashtagId);
                    }

                    // Associate hashtag with tweet
                    await _tweetHashtagApiService.CreateTweetHashtagAsync(new CreateTweetHashtagDto
                    {
                        TweetId = id,
                        HashtagId = hashtagId
                    });
                }

                // Remove hashtags that were deleted
                foreach (var existingHashtag in existingHashtags)
                {
                    var hashtagStillExists = viewModel.HashtagNames.Any(h =>
                    {
                        var trimmedName = h.Trim().StartsWith("#") ? h.Trim() : $"#{h.Trim()}";
                        return trimmedName == existingHashtag.HashtagName;
                    });

                    if (!hashtagStillExists)
                    {
                        // Remove association
                        await _tweetHashtagApiService.RemoveTweetHashtagAsync(id, existingHashtag.HashtagId);
                    }
                }
            }
            else
            {
                // All hashtags were removed, delete all associations
                foreach (var existingHashtag in existingHashtags)
                {
                    await _tweetHashtagApiService.RemoveTweetHashtagAsync(id, existingHashtag.HashtagId);
                }
            }

            SetNotification("Tweet updated successfully");
            return RedirectToAction(nameof(Details), new { id = updatedTweet.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating tweet with ID: {TweetId}", id);
            ModelState.AddModelError("", "An error occurred while updating the tweet. Please try again.");

            var categories = await _categoryApiService.GetAllCategoriesAsync();
            ViewBag.Categories = categories;

            return View(viewModel);
        }
    }

    [HttpPost("{id:guid}/toggle-seen")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleSeen(Guid id)
    {
        try
        {
            var updatedTweet = await _tweetApiService.ToggleTweetSeenStatusAsync(id);
            if (updatedTweet == null)
            {
                SetNotification("Failed to toggle seen status. Please try again.");
            }
            else
            {
                SetNotification($"Tweet marked as {(updatedTweet.IsSeen ? "seen" : "unseen")}");
            }

            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while toggling seen status for tweet with ID: {TweetId}", id);
            SetNotification("An error occurred while toggling the seen status. Please try again.");
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [HttpPost("{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            // First delete all associated hashtag relationships
            var hashtagsResult = await _tweetHashtagApiService.GetByTweetIdAsync(id);
            if (hashtagsResult.Success && hashtagsResult.Data != null)
            {
                foreach (var tweetHashtag in hashtagsResult.Data)
                {
                    await _tweetHashtagApiService.DeleteTweetHashtagAsync(tweetHashtag.Id);
                }
            }

            // Then delete the tweet
            var success = await _tweetApiService.DeleteTweetAsync(id);
            if (success)
            {
                SetNotification("Tweet deleted successfully");
            }
            else
            {
                SetNotification("Failed to delete tweet. Please try again.");
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting tweet with ID: {TweetId}", id);
            SetNotification("An error occurred while deleting the tweet. Please try again.");
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}