using BookMarkBrain.Core.DTOs.Hashtag;
using BookMarkBrain.MVC.Services.APIServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookMarkBrain.MVC.Controllers;


public class HashtagController : BaseController
{
    private readonly IHashtagApiService _hashtagApiService;

    public HashtagController(IApiService apiService, IHashtagApiService hashtagApiService, ILogger<HashtagController> logger)
        : base(apiService, logger)
    {
        _hashtagApiService = hashtagApiService ?? throw new ArgumentNullException(nameof(hashtagApiService));
    }

    // GET: Hashtag
    public async Task<IActionResult> Index()
    {
        try
        {
            var result = await _hashtagApiService.GetAllHashtagsAsync();
            if (!result.Success)
            {
                SetNotification(result.Message, "error");
                return View(Array.Empty<HashtagDto>());
            }

            return View(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all hashtags");
            SetNotification("Failed to load hashtags. Please try again later.", "error");
            return View(Array.Empty<HashtagDto>());
        }
    }

    // GET: Hashtag/Popular
    public async Task<IActionResult> Popular()
    {
        try
        {
            var result = await _hashtagApiService.GetPopularHashtagsAsync();
            if (!result.Success)
            {
                SetNotification(result.Message, "error");
                return View("Index", Array.Empty<HashtagDto>());
            }

            ViewBag.IsPopular = true;
            return View("Index", result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting popular hashtags");
            SetNotification("Failed to load popular hashtags. Please try again later.", "error");
            return View("Index", Array.Empty<HashtagDto>());
        }
    }

    // GET: Hashtag/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var result = await _hashtagApiService.GetHashtagByIdAsync(id);
            if (!result.Success)
            {
                SetNotification(result.Message, "error");
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting hashtag details for ID: {HashtagId}", id);
            SetNotification("Failed to load hashtag details. Please try again later.", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Hashtag/Create
    public IActionResult Create()
    {
        return View(new CreateHashtagDto());
    }

    // POST: Hashtag/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateHashtagDto createHashtagDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(createHashtagDto);
            }

            var result = await _hashtagApiService.CreateHashtagAsync(createHashtagDto);
            if (!result.Success)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View(createHashtagDto);
            }

            SetNotification("Hashtag created successfully!", "success");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating hashtag: {HashtagName}", createHashtagDto.Name);
            SetNotification("Failed to create hashtag. Please try again later.", "error");
            return View(createHashtagDto);
        }
    }

    // GET: Hashtag/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var result = await _hashtagApiService.GetHashtagByIdAsync(id);
            if (!result.Success)
            {
                SetNotification(result.Message, "error");
                return RedirectToAction(nameof(Index));
            }

            var updateDto = new UpdateHashtagDto
            {
                Name = result.Data.Name,
                Description = result.Data.Description,
                IsPopular = result.Data.IsPopular
            };

            ViewBag.HashtagId = id;
            return View(updateDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while preparing edit form for hashtag ID: {HashtagId}", id);
            SetNotification("Failed to load hashtag for editing. Please try again later.", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Hashtag/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateHashtagDto updateHashtagDto)
    {
        ViewBag.HashtagId = id;

        try
        {
            if (!ModelState.IsValid)
            {
                return View(updateHashtagDto);
            }

            var result = await _hashtagApiService.UpdateHashtagAsync(id, updateHashtagDto);
            if (!result.Success)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View(updateHashtagDto);
            }

            SetNotification("Hashtag updated successfully!", "success");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating hashtag ID: {HashtagId}", id);
            SetNotification("Failed to update hashtag. Please try again later.", "error");
            return View(updateHashtagDto);
        }
    }

    // GET: Hashtag/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _hashtagApiService.GetHashtagByIdAsync(id);
            if (!result.Success)
            {
                SetNotification(result.Message, "error");
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while preparing delete confirmation for hashtag ID: {HashtagId}", id);
            SetNotification("Failed to load hashtag for deletion. Please try again later.", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Hashtag/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            var result = await _hashtagApiService.DeleteHashtagAsync(id);
            if (!result.Success)
            {
                SetNotification(result.Message, "error");
                return RedirectToAction(nameof(Index));
            }

            SetNotification("Hashtag deleted successfully!", "success");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting hashtag ID: {HashtagId}", id);
            SetNotification("Failed to delete hashtag. Please try again later.", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Hashtag/IncrementUsage/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> IncrementUsage(Guid id)
    {
        try
        {
            var result = await _hashtagApiService.IncrementUsageCountAsync(id);
            if (!result.Success)
            {
                SetNotification(result.Message, "error");
                return RedirectToAction(nameof(Index));
            }

            SetNotification($"Hashtag usage count incremented to {result.Data}!", "success");
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while incrementing usage count for hashtag ID: {HashtagId}", id);
            SetNotification("Failed to increment hashtag usage count. Please try again later.", "error");
            return RedirectToAction(nameof(Index));
        }
    }
}