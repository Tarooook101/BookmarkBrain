using BookMarkBrain.Core.DTOs;
using BookMarkBrain.Core.DTOs.Category;
using BookMarkBrain.Core.ServiceInterfaces;
using BookMarkBrain.MVC.Models.Category;
using BookMarkBrain.MVC.Services.APIServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookMarkBrain.MVC.Controllers;

/// <summary>
/// Controller for managing categories in the MVC application
/// </summary>
public class CategoryController : BaseController
{
    private readonly ICategoryApiService _categoryService;

    public CategoryController(
        ICategoryApiService categoryService,
        IApiService apiService,
        ILogger<CategoryController> logger) : base(apiService, logger)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
    }

    /// <summary>
    /// Displays the index page with all categories
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories for index page");
            SetNotification("Failed to load categories. Please try again later.", "error");
            return RedirectToAction(nameof(Index), "Home");
        }
    }

    /// <summary>
    /// Displays a hierarchical tree view of categories
    /// </summary>
    public async Task<IActionResult> Tree()
    {
        try
        {
            var categoryTree = await _categoryService.GetCategoryTreeAsync();
            var viewModel = new CategoryTreeViewModel
            {
                Categories = categoryTree.ToList()
            };
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category tree");
            SetNotification("Failed to load category tree. Please try again later.", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Displays details for a specific category
    /// </summary>
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var category = await _categoryService.GetCategoryWithChildrenAsync(id);
            if (category == null)
            {
                SetNotification("Category not found.", "warning");
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category details for ID {CategoryId}", id);
            SetNotification("Failed to load category details. Please try again later.", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Displays the form for creating a new category
    /// </summary>
    public async Task<IActionResult> Create()
    {
        try
        {
            var viewModel = new CreateCategoryViewModel
            {
                AvailableParentCategories = await _categoryService.GetCategoryDropdownItemsAsync()
            };
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing create category form");
            SetNotification("Failed to prepare category creation form. Please try again later.", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Processes the form submission for creating a new category
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCategoryViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                viewModel.AvailableParentCategories = await _categoryService.GetCategoryDropdownItemsAsync();
                return View(viewModel);
            }

            var result = await _categoryService.CreateCategoryAsync(viewModel);
            if (result != null)
            {
                SetNotification($"Category '{result.Name}' created successfully.", "success");
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }

            SetNotification("Failed to create category. Please try again.", "error");
            viewModel.AvailableParentCategories = await _categoryService.GetCategoryDropdownItemsAsync();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            ModelState.AddModelError("", "An error occurred while creating the category. Please try again.");
            viewModel.AvailableParentCategories = await _categoryService.GetCategoryDropdownItemsAsync();
            return View(viewModel);
        }
    }

    /// <summary>
    /// Displays the form for editing an existing category
    /// </summary>
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                SetNotification("Category not found.", "warning");
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new UpdateCategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ColorHex = category.ColorHex,
                DisplayOrder = category.DisplayOrder,
                ParentCategoryId = category.ParentCategoryId,
                AvailableParentCategories = await _categoryService.GetCategoryDropdownItemsAsync()
            };

            // Remove the current category from available parents to prevent self-reference
            viewModel.AvailableParentCategories = viewModel.AvailableParentCategories
                .Where(c => c.Id != id)
                .ToList();

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing edit form for category ID {CategoryId}", id);
            SetNotification("Failed to prepare category edit form. Please try again later.", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Processes the form submission for updating an existing category
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateCategoryViewModel viewModel)
    {
        try
        {
            if (id != viewModel.Id)
            {
                SetNotification("Invalid category ID.", "error");
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                viewModel.AvailableParentCategories = await _categoryService.GetCategoryDropdownItemsAsync();
                // Remove the current category from available parents
                viewModel.AvailableParentCategories = viewModel.AvailableParentCategories
                    .Where(c => c.Id != id)
                    .ToList();
                return View(viewModel);
            }

            var result = await _categoryService.UpdateCategoryAsync(id, viewModel);
            if (result != null)
            {
                SetNotification($"Category '{result.Name}' updated successfully.", "success");
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }

            SetNotification("Failed to update category. Please try again.", "error");
            viewModel.AvailableParentCategories = await _categoryService.GetCategoryDropdownItemsAsync();
            // Remove the current category from available parents
            viewModel.AvailableParentCategories = viewModel.AvailableParentCategories
                .Where(c => c.Id != id)
                .ToList();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category with ID {CategoryId}", id);
            ModelState.AddModelError("", "An error occurred while updating the category. Please try again.");
            viewModel.AvailableParentCategories = await _categoryService.GetCategoryDropdownItemsAsync();
            // Remove the current category from available parents
            viewModel.AvailableParentCategories = viewModel.AvailableParentCategories
                .Where(c => c.Id != id)
                .ToList();
            return View(viewModel);
        }
    }

    /// <summary>
    /// Displays confirmation page for deleting a category
    /// </summary>
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                SetNotification("Category not found.", "warning");
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing delete confirmation for category ID {CategoryId}", id);
            SetNotification("Failed to prepare delete confirmation. Please try again later.", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Processes the deletion of a category
    /// </summary>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                SetNotification("Category not found.", "warning");
                return RedirectToAction(nameof(Index));
            }

            var result = await _categoryService.DeleteCategoryAsync(id);
            if (result)
            {
                SetNotification($"Category '{category.Name}' deleted successfully.", "success");
                return RedirectToAction(nameof(Index));
            }

            SetNotification("Failed to delete category. Please try again.", "error");
            return RedirectToAction(nameof(Delete), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category with ID {CategoryId}", id);
            SetNotification("An error occurred while deleting the category. Please try again.", "error");
            return RedirectToAction(nameof(Delete), new { id });
        }
    }

    /// <summary>
    /// Displays the interface for reordering categories
    /// </summary>
    public async Task<IActionResult> Reorder()
    {
        try
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories.OrderBy(c => c.DisplayOrder).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing category reorder interface");
            SetNotification("Failed to prepare category reordering interface. Please try again later.", "error");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Processes category reordering
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrder([FromBody] Dictionary<Guid, int> orderUpdates)
    {
        try
        {
            if (orderUpdates == null || !orderUpdates.Any())
            {
                return Json(new { success = false, message = "No order updates provided." });
            }

            var result = await _categoryService.UpdateCategoryDisplayOrderAsync(orderUpdates);
            if (result)
            {
                return Json(new { success = true, message = "Category order updated successfully." });
            }

            return Json(new { success = false, message = "Failed to update category order." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category display order");
            return Json(new { success = false, message = "An error occurred while updating category order." });
        }
    }
}