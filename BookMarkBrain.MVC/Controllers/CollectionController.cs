using BookMarkBrain.MVC.Models.Collection;
using BookMarkBrain.MVC.Services.APIServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookMarkBrain.MVC.Controllers;
public class CollectionController : BaseController
{
    private readonly ICollectionApiService _collectionApiService;

    public CollectionController(
        ICollectionApiService collectionApiService,
        IApiService apiService,
        ILogger<CollectionController> logger)
        : base(apiService, logger)
    {
        _collectionApiService = collectionApiService;
    }

    // GET: Collection
    public async Task<IActionResult> Index(string searchTerm = null, int page = 1, int pageSize = 10)
    {
        try
        {
            var viewModel = new CollectionListViewModel
            {
                CurrentPage = page,
                PageSize = pageSize,
                SearchTerm = searchTerm
            };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                viewModel.Collections = await _collectionApiService.SearchCollectionsAsync(searchTerm);
                viewModel.TotalCount = viewModel.Collections.Count;
            }
            else
            {
                viewModel.Collections = await _collectionApiService.GetOrderedCollectionsAsync();
                viewModel.TotalCount = viewModel.Collections.Count;
            }

            // Apply pagination manually since we're already fetching all items
            viewModel.Collections = viewModel.Collections
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving collections");
            SetNotification("An error occurred while loading collections", "error");
            return View(new CollectionListViewModel());
        }
    }

    // GET: Collection/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var collection = await _collectionApiService.GetCollectionWithTweetsAsync(id);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving collection details for ID: {Id}", id);
            SetNotification("An error occurred while loading collection details", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Collection/Create
    public IActionResult Create()
    {
        return View(new CreateCollectionViewModel());
    }

    // POST: Collection/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCollectionViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var result = await _collectionApiService.CreateCollectionAsync(viewModel);
                SetNotification("Collection created successfully", "success");
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating collection");
                SetNotification("An error occurred while creating the collection", "error");
                ModelState.AddModelError("", "Failed to create collection. Please try again.");
            }
        }

        return View(viewModel);
    }

    // GET: Collection/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var collection = await _collectionApiService.GetCollectionByIdAsync(id);
            if (collection == null)
            {
                return NotFound();
            }

            var updateViewModel = new UpdateCollectionViewModel
            {
                Name = collection.Name,
                Description = collection.Description,
                IconUrl = collection.IconUrl,
                IsPublic = collection.IsPublic,
                DisplayOrder = collection.DisplayOrder
            };

            return View(updateViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving collection for edit with ID: {Id}", id);
            SetNotification("An error occurred while loading the collection for editing", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Collection/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateCollectionViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var result = await _collectionApiService.UpdateCollectionAsync(id, viewModel);
                SetNotification("Collection updated successfully", "success");
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating collection with ID: {Id}", id);
                SetNotification("An error occurred while updating the collection", "error");
                ModelState.AddModelError("", "Failed to update collection. Please try again.");
            }
        }

        return View(viewModel);
    }

    // GET: Collection/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var collection = await _collectionApiService.GetCollectionByIdAsync(id);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving collection for deletion with ID: {Id}", id);
            SetNotification("An error occurred while loading the collection for deletion", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Collection/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            await _collectionApiService.DeleteCollectionAsync(id);
            SetNotification("Collection deleted successfully", "success");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting collection with ID: {Id}", id);
            SetNotification("An error occurred while deleting the collection", "error");
            return RedirectToAction(nameof(Delete), new { id });
        }
    }

    // POST: Collection/UpdateDisplayOrder
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateDisplayOrder([FromBody] Dictionary<Guid, int> orderUpdates)
    {
        try
        {
            await _collectionApiService.UpdateCollectionDisplayOrderAsync(orderUpdates);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating collection display order");
            return Json(new { success = false, message = "Failed to update display order" });
        }
    }

    // GET: Collection/Public
    public async Task<IActionResult> Public()
    {
        try
        {
            var collections = await _collectionApiService.GetPublicCollectionsAsync();
            return View(collections);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving public collections");
            SetNotification("An error occurred while loading public collections", "error");
            return View(new List<CollectionViewModel>());
        }
    }
}