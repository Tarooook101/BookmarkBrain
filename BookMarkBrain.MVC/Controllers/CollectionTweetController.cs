using BookMarkBrain.MVC.Models.CollectionTweet;
using BookMarkBrain.MVC.Services.APIServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookMarkBrain.MVC.Controllers;

public class CollectionTweetController : BaseController
{
    private readonly ICollectionTweetApiService _collectionTweetApiService;
    private readonly ICollectionApiService _collectionApiService;

    public CollectionTweetController(
        ICollectionTweetApiService collectionTweetApiService,
        ICollectionApiService collectionApiService,
        IApiService apiService,
        ILogger<CollectionTweetController> logger)
        : base(apiService, logger)
    {
        _collectionTweetApiService = collectionTweetApiService;
        _collectionApiService = collectionApiService;
    }

    // GET: CollectionTweet/Index/5 - Lists all tweets in a collection
    public async Task<IActionResult> Index(Guid collectionId, int page = 1, int pageSize = 10)
    {
        try
        {
            var collection = await _collectionApiService.GetCollectionByIdAsync(collectionId);
            if (collection == null)
            {
                return NotFound();
            }

            var collectionTweets = await _collectionTweetApiService.GetCollectionTweetsByCollectionIdOrderedAsync(collectionId);

            var viewModel = new CollectionTweetsViewModel
            {
                CollectionId = collectionId,
                CollectionName = collection.Name,
                CollectionDescription = collection.Description
            };

            // Apply pagination manually since we have all the collection tweets
            var pagedCollectionTweets = collectionTweets
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            foreach (var collectionTweet in pagedCollectionTweets)
            {
                if (collectionTweet is CollectionTweetDetailViewModel detailViewModel && detailViewModel.Tweet != null)
                {
                    viewModel.Tweets.Add(detailViewModel.Tweet);
                }
            }

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tweets for collection: {CollectionId}", collectionId);
            SetNotification("An error occurred while loading tweets for this collection", "error");
            return RedirectToAction("Index", "Collection");
        }
    }

    // GET: CollectionTweet/Assign/5 - Shows UI to assign tweets to a collection
    public async Task<IActionResult> Assign(Guid collectionId)
    {
        try
        {
            var collection = await _collectionApiService.GetCollectionByIdAsync(collectionId);
            if (collection == null)
            {
                return NotFound();
            }

            // In a real implementation, you would get available tweets from a TweetApiService
            // This is a placeholder - you'll need to implement the actual service method
            var availableTweets = new List<TweetSelectionViewModel>();
            // availableTweets = await _tweetApiService.GetAvailableTweetsForSelection();

            var viewModel = new TweetAssignmentViewModel
            {
                CollectionId = collectionId,
                CollectionName = collection.Name,
                AvailableTweets = availableTweets
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing tweet assignment for collection: {CollectionId}", collectionId);
            SetNotification("An error occurred while preparing the tweet assignment page", "error");
            return RedirectToAction("Details", "Collection", new { id = collectionId });
        }
    }

    // POST: CollectionTweet/Assign
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(TweetAssignmentViewModel viewModel)
    {
        if (ModelState.IsValid && viewModel.SelectedTweetIds?.Count > 0)
        {
            try
            {
                await _collectionTweetApiService.AssignTweetsToCollectionAsync(
                    viewModel.CollectionId, viewModel.SelectedTweetIds);

                SetNotification("Tweets successfully assigned to collection", "success");
                return RedirectToAction("Details", "Collection", new { id = viewModel.CollectionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning tweets to collection: {CollectionId}", viewModel.CollectionId);
                SetNotification("An error occurred while assigning tweets to the collection", "error");
                ModelState.AddModelError("", "Failed to assign tweets. Please try again.");
            }
        }

        return View(viewModel);
    }

    // POST: CollectionTweet/Remove
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(Guid collectionId, Guid tweetId)
    {
        try
        {
            await _collectionTweetApiService.RemoveTweetFromCollectionAsync(collectionId, tweetId);
            SetNotification("Tweet removed from collection successfully", "success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing tweet {TweetId} from collection {CollectionId}",
                tweetId, collectionId);
            SetNotification("An error occurred while removing the tweet from the collection", "error");
        }

        return RedirectToAction("Details", "Collection", new { id = collectionId });
    }

    // POST: CollectionTweet/UpdateOrder
    [HttpPost]
    public async Task<IActionResult> UpdateOrder(Guid collectionId, [FromBody] Dictionary<Guid, int> tweetOrderUpdates)
    {
        try
        {
            await _collectionTweetApiService.UpdateTweetDisplayOrderInCollectionAsync(collectionId, tweetOrderUpdates);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tweet order in collection {CollectionId}", collectionId);
            return Json(new { success = false, message = "Failed to update tweet order" });
        }
    }

    // GET: CollectionTweet/Create/5
    public async Task<IActionResult> Create(Guid collectionId)
    {
        try
        {
            var collection = await _collectionApiService.GetCollectionByIdAsync(collectionId);
            if (collection == null)
            {
                return NotFound();
            }

            var viewModel = new CreateCollectionTweetViewModel
            {
                CollectionId = collectionId,
                DisplayOrder = 0 // Default value, you might want to get the next display order from the service
            };

            ViewBag.CollectionName = collection.Name;
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing create tweet for collection: {CollectionId}", collectionId);
            SetNotification("An error occurred while preparing the create page", "error");
            return RedirectToAction("Details", "Collection", new { id = collectionId });
        }
    }

    // POST: CollectionTweet/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCollectionTweetViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var result = await _collectionTweetApiService.CreateCollectionTweetAsync(viewModel);
                SetNotification("Tweet added to collection successfully", "success");
                return RedirectToAction("Details", "Collection", new { id = viewModel.CollectionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating collection tweet: {CollectionId}, {TweetId}",
                    viewModel.CollectionId, viewModel.TweetId);
                SetNotification("An error occurred while adding the tweet to the collection", "error");
                ModelState.AddModelError("", "Failed to add tweet to collection. Please try again.");
            }
        }

        // If we got here, something went wrong
        try
        {
            var collection = await _collectionApiService.GetCollectionByIdAsync(viewModel.CollectionId);
            ViewBag.CollectionName = collection?.Name ?? "Collection";
        }
        catch
        {
            ViewBag.CollectionName = "Collection";
        }

        return View(viewModel);
    }

    // GET: CollectionTweet/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var collectionTweet = await _collectionTweetApiService.GetCollectionTweetByIdAsync(id);
            if (collectionTweet == null)
            {
                return NotFound();
            }

            var viewModel = new UpdateCollectionTweetViewModel
            {
                DisplayOrder = collectionTweet.DisplayOrder
            };

            ViewBag.CollectionId = collectionTweet.CollectionId;
            ViewBag.TweetId = collectionTweet.TweetId;
            ViewBag.CollectionName = collectionTweet.CollectionName ?? "Collection";
            ViewBag.TweetContent = collectionTweet.TweetContent ?? "Tweet";

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving collection tweet for edit: {Id}", id);
            SetNotification("An error occurred while preparing the edit page", "error");
            return RedirectToAction("Index", "Collection");
        }
    }

    // POST: CollectionTweet/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateCollectionTweetViewModel viewModel, Guid collectionId)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Implement the update logic to update just the display order
                // This would typically be:
                // await _collectionTweetApiService.UpdateCollectionTweetAsync(id, viewModel);

                // For now, we'll use a workaround since you probably haven't implemented that specific method
                var tweetOrderUpdates = new Dictionary<Guid, int> { { id, viewModel.DisplayOrder } };
                await _collectionTweetApiService.UpdateTweetDisplayOrderInCollectionAsync(collectionId, tweetOrderUpdates);

                SetNotification("Collection tweet updated successfully", "success");
                return RedirectToAction("Details", "Collection", new { id = collectionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating collection tweet: {Id}", id);
                SetNotification("An error occurred while updating the collection tweet", "error");
                ModelState.AddModelError("", "Failed to update. Please try again.");
            }
        }

        // If we got here, something went wrong
        try
        {
            var collectionTweet = await _collectionTweetApiService.GetCollectionTweetByIdAsync(id);
            ViewBag.CollectionId = collectionTweet?.CollectionId;
            ViewBag.TweetId = collectionTweet?.TweetId;
            ViewBag.CollectionName = collectionTweet?.CollectionName ?? "Collection";
            ViewBag.TweetContent = collectionTweet?.TweetContent ?? "Tweet";
        }
        catch
        {
            ViewBag.CollectionId = collectionId;
            ViewBag.CollectionName = "Collection";
            ViewBag.TweetContent = "Tweet";
        }

        return View(viewModel);
    }
}
