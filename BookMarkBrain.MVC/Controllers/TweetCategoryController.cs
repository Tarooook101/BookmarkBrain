using BookMarkBrain.MVC.Models.TweetCategory;
using BookMarkBrain.MVC.Services.APIServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookMarkBrain.MVC.Controllers;
public class TweetCategoryController : BaseController
{
    private readonly ITweetCategoryApiService _tweetCategoryApiService;
    private readonly ITweetApiService _tweetApiService;
    private readonly ICategoryApiService _categoryApiService;

    public TweetCategoryController(
        ITweetCategoryApiService tweetCategoryApiService,
        ITweetApiService tweetApiService,
        ICategoryApiService categoryApiService,
        ILogger<TweetCategoryController> logger) : base(null, logger)
    {
        _tweetCategoryApiService = tweetCategoryApiService ?? throw new ArgumentNullException(nameof(tweetCategoryApiService));
        _tweetApiService = tweetApiService ?? throw new ArgumentNullException(nameof(tweetApiService));
        _categoryApiService = categoryApiService ?? throw new ArgumentNullException(nameof(categoryApiService));
    }

    // GET: TweetCategory
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchTerm = null, Guid? categoryId = null, Guid? tweetId = null)
    {
        try
        {
            var (items, totalCount) = await _tweetCategoryApiService.GetPagedAsync(page, pageSize);

            // Apply filters if provided
            if (!string.IsNullOrEmpty(searchTerm) || categoryId.HasValue || tweetId.HasValue)
            {
                // For simplicity, we'll handle filtering client-side
                // In a production app, you'd probably want to implement server-side filtering
                if (categoryId.HasValue)
                {
                    items = items.FindAll(tc => tc.CategoryId == categoryId.Value);
                }

                if (tweetId.HasValue)
                {
                    items = items.FindAll(tc => tc.TweetId == tweetId.Value);
                }

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    items = items.FindAll(tc =>
                        (tc.TweetContent != null && tc.TweetContent.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (tc.CategoryName != null && tc.CategoryName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (tc.AuthorUsername != null && tc.AuthorUsername.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
                }
            }

            var viewModel = new TweetCategoryListViewModel
            {
                TweetCategories = items,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                SearchTerm = searchTerm,
                FilterByCategoryId = categoryId,
                FilterByTweetId = tweetId
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tweet categories");
            SetNotification("Failed to load tweet categories. Please try again later.", "error");
            return View(new TweetCategoryListViewModel());
        }
    }

    // GET: TweetCategory/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var tweetCategory = await _tweetCategoryApiService.GetByIdAsync(id);
            if (tweetCategory == null)
            {
                SetNotification("Tweet category not found.", "error");
                return RedirectToAction(nameof(Index));
            }

            return View(tweetCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tweet category details for ID: {Id}", id);
            SetNotification("Failed to load tweet category details. Please try again later.", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: TweetCategory/Create
    public async Task<IActionResult> Create()
    {
        try
        {
            var viewModel = new TweetCategoryCreateViewModel
            {
                AvailableTweets = await GetAvailableTweetsAsync(),
                AvailableCategories = await GetAvailableCategoriesAsync()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing tweet category creation form");
            SetNotification("Failed to load the create form. Please try again later.", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: TweetCategory/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TweetCategoryCreateViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                viewModel.AvailableTweets = await GetAvailableTweetsAsync();
                viewModel.AvailableCategories = await GetAvailableCategoriesAsync();
                return View(viewModel);
            }

            var result = await _tweetCategoryApiService.CreateAsync(viewModel);
            if (result != null)
            {
                SetNotification("Tweet category created successfully.", "success");
                return RedirectToAction(nameof(Index));
            }

            SetNotification("Failed to create tweet category.", "error");
            viewModel.AvailableTweets = await GetAvailableTweetsAsync();
            viewModel.AvailableCategories = await GetAvailableCategoriesAsync();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tweet category");
            SetNotification("An error occurred while creating the tweet category. Please try again.", "error");
            viewModel.AvailableTweets = await GetAvailableTweetsAsync();
            viewModel.AvailableCategories = await GetAvailableCategoriesAsync();
            return View(viewModel);
        }
    }

    // GET: TweetCategory/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var tweetCategory = await _tweetCategoryApiService.GetByIdAsync(id);
            if (tweetCategory == null)
            {
                SetNotification("Tweet category not found.", "error");
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new TweetCategoryUpdateViewModel
            {
                Id = tweetCategory.Id,
                TweetId = tweetCategory.TweetId,
                CategoryId = tweetCategory.CategoryId,
                AvailableTweets = await GetAvailableTweetsAsync(),
                AvailableCategories = await GetAvailableCategoriesAsync()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing tweet category edit form for ID: {Id}", id);
            SetNotification("Failed to load the edit form. Please try again later.", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: TweetCategory/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, TweetCategoryUpdateViewModel viewModel)
    {
        try
        {
            if (id != viewModel.Id)
            {
                SetNotification("Invalid request.", "error");
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                viewModel.AvailableTweets = await GetAvailableTweetsAsync();
                viewModel.AvailableCategories = await GetAvailableCategoriesAsync();
                return View(viewModel);
            }

            var result = await _tweetCategoryApiService.UpdateAsync(viewModel);
            if (result != null)
            {
                SetNotification("Tweet category updated successfully.", "success");
                return RedirectToAction(nameof(Index));
            }

            SetNotification("Failed to update tweet category.", "error");
            viewModel.AvailableTweets = await GetAvailableTweetsAsync();
            viewModel.AvailableCategories = await GetAvailableCategoriesAsync();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tweet category with ID: {Id}", id);
            SetNotification("An error occurred while updating the tweet category. Please try again.", "error");
            viewModel.AvailableTweets = await GetAvailableTweetsAsync();
            viewModel.AvailableCategories = await GetAvailableCategoriesAsync();
            return View(viewModel);
        }
    }

    // GET: TweetCategory/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var tweetCategory = await _tweetCategoryApiService.GetByIdAsync(id);
            if (tweetCategory == null)
            {
                SetNotification("Tweet category not found.", "error");
                return RedirectToAction(nameof(Index));
            }

            return View(tweetCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing tweet category delete confirmation for ID: {Id}", id);
            SetNotification("Failed to load the delete confirmation. Please try again later.", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: TweetCategory/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            var result = await _tweetCategoryApiService.DeleteAsync(id);
            if (result)
            {
                SetNotification("Tweet category deleted successfully.", "success");
                return RedirectToAction(nameof(Index));
            }

            SetNotification("Failed to delete tweet category.", "error");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tweet category with ID: {Id}", id);
            SetNotification("An error occurred while deleting the tweet category. Please try again.", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: TweetCategory/ManageTweetCategories/5
    public async Task<IActionResult> ManageTweetCategories(Guid tweetId)
    {
        try
        {
            // Get the tweet details
            var tweet = await _tweetApiService.GetTweetByIdAsync(tweetId);
            if (tweet == null)
            {
                SetNotification("Tweet not found.", "error");
                return RedirectToAction("Index", "Tweet");
            }

            // Get all categories
            var allCategories = await _categoryApiService.GetAllCategoriesAsync();
            var categoryItems = allCategories.Select(c => new CategorySelectItemViewModel
            {
                Value = c.Id,
                Text = c.Name,
                ColorHex = c.ColorHex,
                Selected = false
            }).ToList();

            // Get categories already assigned to this tweet
            var assignedTweetCategories = await _tweetCategoryApiService.GetByTweetIdAsync(tweetId);
            var assignedCategoryIds = assignedTweetCategories.Select(tc => tc.CategoryId).ToList();

            // Update the Selected property for assigned categories
            foreach (var category in categoryItems)
            {
                category.Selected = assignedCategoryIds.Contains(category.Value);
            }

            var viewModel = new TweetWithCategoriesViewModel
            {
                TweetId = tweet.Id,
                TweetContent = tweet.Content,
                AuthorUsername = tweet.AuthorUsername,
                OriginalUrl = tweet.OriginalUrl,
                AssignedCategories = categoryItems.Where(c => c.Selected).ToList(),
                AvailableCategories = categoryItems.Where(c => !c.Selected).ToList()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing category management form for tweet ID: {TweetId}", tweetId);
            SetNotification("Failed to load the category management form. Please try again later.", "error");
            return RedirectToAction("Index", "Tweet");
        }
    }

    // POST: TweetCategory/ManageTweetCategories/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageTweetCategories(Guid tweetId, List<Guid> categoryIds)
    {
        try
        {
            var result = await _tweetCategoryApiService.AssignCategoriesToTweetAsync(tweetId, categoryIds);
            if (result != null)
            {
                SetNotification("Categories updated successfully for tweet.", "success");
                return RedirectToAction("Details", "Tweet", new { id = tweetId });
            }

            SetNotification("Failed to update tweet categories.", "error");
            return RedirectToAction(nameof(ManageTweetCategories), new { tweetId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating categories for tweet ID: {TweetId}", tweetId);
            SetNotification("An error occurred while updating the tweet categories. Please try again.", "error");
            return RedirectToAction(nameof(ManageTweetCategories), new { tweetId });
        }
    }

    // POST: TweetCategory/RemoveCategory
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveCategory(Guid tweetId, Guid categoryId)
    {
        try
        {
            var result = await _tweetCategoryApiService.RemoveCategoryFromTweetAsync(tweetId, categoryId);
            if (result)
            {
                SetNotification("Category removed from tweet successfully.", "success");
            }
            else
            {
                SetNotification("Failed to remove category from tweet.", "error");
            }

            return RedirectToAction(nameof(ManageTweetCategories), new { tweetId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing category {CategoryId} from tweet {TweetId}", categoryId, tweetId);
            SetNotification("An error occurred while removing the category. Please try again.", "error");
            return RedirectToAction(nameof(ManageTweetCategories), new { tweetId });
        }
    }

    #region Helper Methods

    private async Task<List<SelectItemViewModel>> GetAvailableTweetsAsync()
    {
        var tweets = await _tweetApiService.GetAllTweetsAsync();
        return tweets.Select(t => new SelectItemViewModel
        {
            Value = t.Id,
            Text = $"{t.AuthorUsername}: {t.Content?.Substring(0, Math.Min(t.Content?.Length ?? 0, 30))}{(t.Content?.Length > 30 ? "..." : "")}"
        }).ToList();
    }

    private async Task<List<SelectItemViewModel>> GetAvailableCategoriesAsync()
    {
        var categories = await _categoryApiService.GetAllCategoriesAsync();
        return categories.Select(c => new SelectItemViewModel
        {
            Value = c.Id,
            Text = c.Name
        }).ToList();
    }

    #endregion
}