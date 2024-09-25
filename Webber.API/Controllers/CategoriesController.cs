using Webber.API.Attributes;
using Webber.Application.DTOs;
using Webber.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Webber.API.Controllers;

/// <summary>
/// Handles operations related to product categories.
/// </summary>
/// <param name="categoryService">Service for managing category operations.</param>

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ICategoryService categoryService) : ControllerBaseEx
{
    /// <summary>
    /// Retrieves all categories.
    /// </summary>
    /// <returns>
    /// Returns an OK response with a list of all categories.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Retrieves a category by its ID.
    /// </summary>
    /// <param name="id">The ID of the category to retrieve.</param>
    /// <returns>
    /// Returns an OK response with the category details if found.
    /// Returns a NotFound response if no category with the specified ID exists.
    /// </returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await categoryService.GetCategoryByIdAsync(id);
        return category == null ? NotFound() : Ok(category);
    }

    /// <summary>
    /// Retrieves subcategories for a specified category.
    /// </summary>
    /// <param name="id">The ID of the parent category.</param>
    /// <returns>
    /// Returns an OK response with a list of subcategories.
    /// </returns>
    [HttpGet("{id:int}/subcategories")]
    public async Task<IActionResult> GetSubcategories(int id)
    {
        var subcategories = await categoryService.GetSubcategoriesAsync(id);
        return Ok(subcategories);
    }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    /// <param name="categoryDto">The details of the category to create.</param>
    /// <returns>
    /// Returns a Created response with the new category's details if creation is successful.
    /// Returns a BadRequest response if creation fails.
    /// </returns>
    [AuthorizeMiddleware(["Admin"])]
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto)
    {
        var newCategory = await categoryService.CreateCategoryAsync(categoryDto);
        return newCategory != null
            ? CreatedAtAction(nameof(GetCategoryById), new { id = newCategory.Id }, newCategory)
            : BadRequest("Failed to create category");
    }

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    /// <param name="id">The ID of the category to update.</param>
    /// <param name="categoryDto">The updated category details.</param>
    /// <returns>
    /// Returns a NoContent response upon successful update.
    /// Returns a BadRequest response if the category ID in the request body does not match the ID in the URL.
    /// </returns>
    [AuthorizeMiddleware(["Admin"])]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto categoryDto)
    {
        if (categoryDto.Id != id)
        {
            return BadRequest("Category ID in request body does not match ID in URL.");
        }

        await categoryService.UpdateCategoryAsync(id, categoryDto);
        return NoContent();
    }

    /// <summary>
    /// Deletes a category by its ID.
    /// </summary>
    /// <param name="id">The ID of the category to delete.</param>
    /// <returns>
    /// Returns a NoContent response upon successful deletion.
    /// </returns>
    [AuthorizeMiddleware(["Admin"])]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }
}
