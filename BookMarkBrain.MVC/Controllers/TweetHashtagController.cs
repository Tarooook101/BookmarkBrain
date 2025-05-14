using BookMarkBrain.Core.DTOs.TweetHashtag;
using BookMarkBrain.MVC.Models.TweetHashtag;
using BookMarkBrain.MVC.Services.APIServiceInterfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace BookMarkBrain.MVC.Controllers;

public class TweetHashtagController : BaseController
{
    private readonly ITweetHashtagApiService _tweetHashtagApiService;
    private readonly ITweetApiService _tweetApiService;
    private readonly IHashtagApiService _hashtagApiService;
    private readonly ILogger<TweetHashtagController> _logger;

    public TweetHashtagController(
        IApiService apiService,
        ITweetHashtagApiService tweetHashtagApiService,
        ITweetApiService tweetApiService,
        IHashtagApiService hashtagApiService,
        ILogger<TweetHashtagController> logger)
        : base(apiService, logger)
    {
        _tweetHashtagApiService = tweetHashtagApiService ?? throw new ArgumentNullException(nameof(tweetHashtagApiService));
        _tweetApiService = tweetApiService ?? throw new ArgumentNullException(nameof(tweetApiService));
        _hashtagApiService = hashtagApiService ?? throw new ArgumentNullException(nameof(hashtagApiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // GET: TweetHashtag/Index
    public async Task<IActionResult> Index()
    {
        try
        {
            var result = await _tweetHashtagApiService.GetAllWithDetailsAsync();

            if (!result.Success)
            {
                SetNotification(result.Message, "error");
                return View(new List<TweetHashtagDetailsViewModel>());
            }

            var viewModels = result.Data.Adapt<List<TweetHashtagDetailsViewModel>>();
            return View(viewModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tweet hashtag relationships");
            SetNotification("An error occurred while retrieving tweet hashtag relationships", "error");
            return View(new List<TweetHashtagDetailsViewModel>());
        }
    }

    // GET: TweetHashtag/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var result = await _tweetHashtagApiService.GetTweetHashtagByIdAsync(id);

            if (!result.Success || result.Data == null)
            {
                SetNotification("Tweet hashtag relationship not found", "warning");
                return RedirectToAction(nameof(Index));
            }

            // Since this is just a relationship entity, we might want the detailed view instead of the basic one
            // Let's get the tweets for this hashtag with details
            var detailsResult = await _tweetHashtagApiService.GetAllWithDetailsAsync();

            if (!detailsResult.Success)
            {
                SetNotification(detailsResult.Message, "error");
                return RedirectToAction(nameof(Index));
            }

            var detailedViewModel = detailsResult.Data
                .FirstOrDefault(th => th.Id == id)
                .Adapt<TweetHashtagDetailsViewModel>();

            if (detailedViewModel == null)
            {
                SetNotification("Could not retrieve detailed information", "warning");
                return RedirectToAction(nameof(Index));
            }

            return View(detailedViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tweet hashtag relationship with ID {Id}", id);
            SetNotification("An error occurred while retrieving tweet hashtag details", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: TweetHashtag/ByTweet/5
    public async Task<IActionResult> ByTweet(Guid id)
    {
        try
        {
            var result = await _tweetHashtagApiService.GetByTweetIdWithDetailsAsync(id);

            if (!result.Success)
            {
                SetNotification(result.Message, "error");
                return View(new List<TweetHashtagDetailsViewModel>());
            }

            var viewModels = result.Data.Adapt<List<TweetHashtagDetailsViewModel>>();

            // Get the tweet details to display in the view header
            var tweet = await _tweetApiService.GetTweetByIdAsync(id);
            if (tweet != null)
            {
                ViewBag.TweetContent = tweet.Content;
                ViewBag.TweetId = id;
            }

            return View(viewModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hashtags for tweet with ID {Id}", id);
            SetNotification("An error occurred while retrieving hashtags for this tweet", "error");
            return View(new List<TweetHashtagDetailsViewModel>());
        }
    }

    // GET: TweetHashtag/ByHashtag/5
    public async Task<IActionResult> ByHashtag(Guid id)
    {
        try
        {
            var result = await _tweetHashtagApiService.GetByHashtagIdWithDetailsAsync(id);

            if (!result.Success)
            {
                SetNotification(result.Message, "error");
                return View(new List<TweetHashtagDetailsViewModel>());
            }

            var viewModels = result.Data.Adapt<List<TweetHashtagDetailsViewModel>>();

            // Get the hashtag details to display in the view header
            var hashtagResult = await _hashtagApiService.GetHashtagByIdAsync(id);
            if (hashtagResult != null && hashtagResult.Success && hashtagResult.Data != null)
            {
                ViewBag.HashtagName = hashtagResult.Data.Name;
                ViewBag.HashtagId = id;
            }

            return View(viewModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tweets for hashtag with ID {Id}", id);
            SetNotification("An error occurred while retrieving tweets for this hashtag", "error");
            return View(new List<TweetHashtagDetailsViewModel>());
        }
    }

    // GET: TweetHashtag/Create
    public async Task<IActionResult> Create()
    {
        try
        {
            var viewModel = new CreateTweetHashtagViewModel();

            // Populate available tweets and hashtags for dropdown lists
            var tweets = await _tweetApiService.GetAllTweetsAsync();
            if (tweets != null)
            {
                viewModel.AvailableTweets = tweets.Select(t => new TweetSelectListItem
                {
                    Id = t.Id,
                    Content = t.Content.Length > 50 ? t.Content.Substring(0, 47) + "..." : t.Content,
                    AuthorUsername = t.AuthorUsername
                }).ToList();
            }

            var hashtagsResult = await _hashtagApiService.GetAllHashtagsAsync();
            if (hashtagsResult != null && hashtagsResult.Success && hashtagsResult.Data != null)
            {
                viewModel.AvailableHashtags = hashtagsResult.Data.Select(h => new HashtagSelectListItem
                {
                    Id = h.Id,
                    Name = h.Name
                }).ToList();
            }

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing tweet hashtag creation form");
            SetNotification("An error occurred while preparing the form", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: TweetHashtag/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTweetHashtagViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                // Repopulate available tweets and hashtags for dropdown lists
                var tweets = await _tweetApiService.GetAllTweetsAsync();
                if (tweets != null)
                {
                    viewModel.AvailableTweets = tweets.Select(t => new TweetSelectListItem
                    {
                        Id = t.Id,
                        Content = t.Content.Length > 50 ? t.Content.Substring(0, 47) + "..." : t.Content,
                        AuthorUsername = t.AuthorUsername
                    }).ToList();
                }

                var hashtagsResult = await _hashtagApiService.GetAllHashtagsAsync();
                if (hashtagsResult != null && hashtagsResult.Success && hashtagsResult.Data != null)
                {
                    viewModel.AvailableHashtags = hashtagsResult.Data.Select(h => new HashtagSelectListItem
                    {
                        Id = h.Id,
                        Name = h.Name
                    }).ToList();
                }

                return View(viewModel);
            }

            var createDto = new CreateTweetHashtagDto
            {
                TweetId = viewModel.TweetId,
                HashtagId = viewModel.HashtagId
            };

            var result = await _tweetHashtagApiService.CreateTweetHashtagAsync(createDto);

            if (!result.Success)
            {
                SetNotification(result.Message, "error");
                return View(viewModel);
            }

            // Try to update the usage count for the hashtag
            await _hashtagApiService.IncrementUsageCountAsync(viewModel.HashtagId);

            SetNotification("Tweet hashtag relationship created successfully", "success");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tweet hashtag relationship");
            SetNotification("An error occurred while creating the tweet hashtag relationship", "error");

            // Repopulate available tweets and hashtags for dropdown lists
            try
            {
                var tweets = await _tweetApiService.GetAllTweetsAsync();
                if (tweets != null)
                {
                    viewModel.AvailableTweets = tweets.Select(t => new TweetSelectListItem
                    {
                        Id = t.Id,
                        Content = t.Content.Length > 50 ? t.Content.Substring(0, 47) + "..." : t.Content,
                        AuthorUsername = t.AuthorUsername
                    }).ToList();
                }

                var hashtagsResult = await _hashtagApiService.GetAllHashtagsAsync();
                if (hashtagsResult != null && hashtagsResult.Success && hashtagsResult.Data != null)
                {
                    viewModel.AvailableHashtags = hashtagsResult.Data.Select(h => new HashtagSelectListItem
                    {
                        Id = h.Id,
                        Name = h.Name
                    }).ToList();
                }
            }
            catch
            {
                // Ignore errors when repopulating lists in this error case
            }

            return View(viewModel);
        }
    }

    // GET: TweetHashtag/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _tweetHashtagApiService.GetTweetHashtagByIdAsync(id);

            if (!result.Success || result.Data == null)
            {
                SetNotification("Tweet hashtag relationship not found", "warning");
                return RedirectToAction(nameof(Index));
            }

            var viewModel = result.Data.Adapt<TweetHashtagViewModel>();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tweet hashtag relationship for deletion with ID {Id}", id);
            SetNotification("An error occurred while retrieving tweet hashtag details", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: TweetHashtag/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            // First get the hashtag ID so we can decrement its usage count
            var tweetHashtagResult = await _tweetHashtagApiService.GetTweetHashtagByIdAsync(id);
            var hashtagId = tweetHashtagResult.Success && tweetHashtagResult.Data != null
                ? tweetHashtagResult.Data.HashtagId
                : Guid.Empty;

            var result = await _tweetHashtagApiService.DeleteTweetHashtagAsync(id);

            if (!result.Success)
            {
                SetNotification(result.Message, "error");
                return RedirectToAction(nameof(Index));
            }

            SetNotification("Tweet hashtag relationship deleted successfully", "success");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tweet hashtag relationship with ID {Id}", id);
            SetNotification("An error occurred while deleting the tweet hashtag relationship", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: TweetHashtag/RemoveFromTweet
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFromTweet(Guid tweetId, Guid hashtagId, string returnUrl = null)
    {
        try
        {
            var result = await _tweetHashtagApiService.RemoveTweetHashtagAsync(tweetId, hashtagId);

            if (!result.Success)
            {
                SetNotification(result.Message, "error");
            }
            else
            {
                SetNotification("Hashtag removed from tweet successfully", "success");
            }

            // Return to the referring page if provided, otherwise to the tweet details
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("ByTweet", new { id = tweetId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing hashtag ID {HashtagId} from tweet ID {TweetId}", hashtagId, tweetId);
            SetNotification("An error occurred while removing the hashtag from the tweet", "error");

            // Return to the referring page if provided, otherwise to the tweet details
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("ByTweet", new { id = tweetId });
        }
    }

    // POST: TweetHashtag/AddToTweet
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToTweet(Guid tweetId, Guid hashtagId)
    {
        try
        {
            var createDto = new CreateTweetHashtagDto
            {
                TweetId = tweetId,
                HashtagId = hashtagId
            };

            var result = await _tweetHashtagApiService.CreateTweetHashtagAsync(createDto);

            if (!result.Success)
            {
                SetNotification(result.Message, "error");
            }
            else
            {
                // Try to update the usage count for the hashtag
                await _hashtagApiService.IncrementUsageCountAsync(hashtagId);
                SetNotification("Hashtag added to tweet successfully", "success");
            }

            return RedirectToAction("ByTweet", new { id = tweetId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding hashtag ID {HashtagId} to tweet ID {TweetId}", hashtagId, tweetId);
            SetNotification("An error occurred while adding the hashtag to the tweet", "error");
            return RedirectToAction("ByTweet", new { id = tweetId });
        }
    }
}