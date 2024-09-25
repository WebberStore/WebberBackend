using Webber.API.Attributes;
using Webber.Application.DTOs;
using Webber.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Webber.API.Controllers;

/// <summary>
/// Manages product-related operations including retrieval, creation, and updating of products.
/// </summary>
/// <param name="productService">Service for handling product operations.</param>
/// <param name="imageHelper">Helper for processing and uploading images.</param>
[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProductService productService, IImageHelper imageHelper) : ControllerBaseEx
{
    /// <summary>
    /// Retrieves all products with optional pagination.
    /// </summary>
    /// <param name="page">Page number for pagination (default is 1).</param>
    /// <param name="pageSize">Number of products per page (default is 10).</param>
    /// <returns>
    /// Returns an OK response with the list of products.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetAllProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var products = await productService.GetAllProductsAsync(page, pageSize);
        return Ok(products); // Return the list of products
    }

    /// <summary>
    /// Retrieves a product by its ID.
    /// </summary>
    /// <param name="id">The ID of the product to retrieve.</param>
    /// <returns>
    /// Returns an OK response with the product details or a NotFound response if the product does not exist.
    /// </returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await productService.GetProductDetailsAsync(id);
        if (product == null)
            return NotFound(); // Product not found

        return Ok(product); // Return the product details
    }

    /// <summary>
    /// Retrieves products by category ID with optional pagination.
    /// </summary>
    /// <param name="categoryId">The ID of the category to filter products by.</param>
    /// <param name="page">Page number for pagination (default is 1).</param>
    /// <param name="pageSize">Number of products per page (default is 10).</param>
    /// <returns>
    /// Returns an OK response with the list of products in the specified category.
    /// </returns>
    [HttpGet("category/{categoryId:int}")]
    public async Task<IActionResult> GetProductsByCategory(
        int categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var products = await productService.GetProductsByCategoryAsync(categoryId, page, pageSize);
        return Ok(products); // Return the list of products in the specified category
    }

    /// <summary>
    /// Searches for products based on various filter criteria.
    /// </summary>
    /// <param name="searchTerm">The search term to filter products.</param>
    /// <param name="page">Page number for pagination (default is 1).</param>
    /// <param name="pageSize">Number of products per page (default is 10).</param>
    /// <param name="categoryId">Optional category ID to filter by.</param>
    /// <param name="minPrice">Optional minimum price to filter by.</param>
    /// <param name="maxPrice">Optional maximum price to filter by.</param>
    /// <param name="tags">Optional list of tags to filter by.</param>
    /// <param name="seller">Optional seller ID to filter by.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortOrder">Optional order to sort (asc or desc).</param>
    /// <returns>
    /// Returns an OK response with the list of products that match the search criteria.
    /// </returns>
    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts(
        [FromQuery] string searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? categoryId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] List<string>? tags = null,
        [FromQuery] string? seller = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null
    )
    {
        var products = await productService.SearchProductsAsync(searchTerm, page, pageSize, categoryId, minPrice,
            maxPrice, tags, seller, sortBy, sortOrder);
        return Ok(products); // Return the list of products matching the search criteria
    }

    /// <summary>
    /// Retrieves the top-selling products with optional pagination.
    /// </summary>
    /// <param name="pageSize">Number of products to return (default is 10).</param>
    /// <returns>
    /// Returns an OK response with the list of top-selling products.
    /// </returns>
    [HttpGet("top-selling")]
    public async Task<IActionResult> GetTopSellingProducts([FromQuery] int pageSize = 10)
    {
        var products = await productService.GetTopSellingProductsAsync(pageSize);
        return Ok(products); // Return the list of top-selling products
    }

    /// <summary>
    /// Retrieves the latest products with optional pagination.
    /// </summary>
    /// <param name="pageSize">Number of products to return (default is 10).</param>
    /// <returns>
    /// Returns an OK response with the list of latest products.
    /// </returns>
    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestProducts([FromQuery] int pageSize = 10)
    {
        var products = await productService.GetLatestProductsAsync(pageSize);
        return Ok(products); // Return the list of latest products
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="productDto">Data transfer object containing the product details.</param>
    /// <returns>
    /// Returns a Created response with the new product details or a BadRequest response if creation fails.
    /// Returns a Forbid response if the seller ID does not match the current user.
    /// </returns>
    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
    {
        if (productDto.SellerId != CurrentUserId && CurrentUserRole != "Admin")
            return Forbid(); // Seller is not authorized to create products for other sellers

        if (CurrentUserRole == "Seller")
            productDto.SellerId = CurrentUserId;
        
        var newProduct = await productService.CreateProductAsync(productDto);
        return newProduct != null
            ? CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct)
            : BadRequest("Failed to create product"); // Creation failed
    }

    /// <summary>
    /// Updates an existing product by its ID.
    /// </summary>
    /// <param name="id">The ID of the product to update.</param>
    /// <param name="productDto">Data transfer object containing updated product details.</param>
    /// <returns>
    /// Returns NoContent if the update is successful.
    /// Returns a Forbid response if the user does not own the product.
    /// </returns>
    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto productDto)
    {
        var product = await productService.GetProductByIdAsync(id);
        if (product == null || (product.SellerId != CurrentUserId && CurrentUserRole != "Admin"))
            return Forbid(); // User is not authorized to update this product

        await productService.UpdateProductAsync(id, productDto);
        return NoContent(); // Update successful
    }

    /// <summary>
    /// Deletes a product by its ID.
    /// </summary>
    /// <param name="id">The ID of the product to delete.</param>
    /// <returns>
    /// Returns NoContent if the deletion is successful.
    /// Returns a Forbid response if the user does not own the product.
    /// </returns>
    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await productService.GetProductByIdAsync(id);
        if (product == null || (product.SellerId != CurrentUserId && CurrentUserRole != "Admin"))
            return Forbid(); // User is not authorized to delete this product

        await productService.DeleteProductAsync(id);
        return NoContent(); // Deletion successful
    }

    /// <summary>
    /// Creates a new variant for a product.
    /// </summary>
    /// <param name="productId">The ID of the product to which the variant belongs.</param>
    /// <param name="variantDto">Data transfer object containing variant details.</param>
    /// <returns>
    /// Returns a Created response with the new variant details or a BadRequest response if creation fails.
    /// </returns>
    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpPost("variants/{productId:int}")]
    public async Task<IActionResult> CreateProductVariant(int productId, [FromBody] CreateProductVariantDto variantDto)
    {
        var newVariant = await productService.CreateProductVariantAsync(productId, variantDto);
        return newVariant != null
            ? CreatedAtAction(nameof(GetProductById), new { id = newVariant.Id }, newVariant)
            : BadRequest("Failed to create product variant"); // Creation failed
    }

    /// <summary>
    /// Updates an existing variant by its ID.
    /// </summary>
    /// <param name="variantId">The ID of the variant to update.</param>
    /// <param name="variantDto">Data transfer object containing updated variant details.</param>
    /// <returns>
    /// Returns NoContent if the update is successful.
    /// Returns a Forbid response if the user does not own the product.
    /// </returns>
    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpPut("variants/{variantId:int}")]
    public async Task<IActionResult> UpdateProductVariant(int variantId, [FromBody] UpdateProductVariantDto variantDto)
    {
        var product = await productService.GetProductByVariantIdAsync(variantId);
        if (product == null || (product.SellerId != CurrentUserId && CurrentUserRole != "Admin"))
            return Forbid(); // User is not authorized to update this variant

        await productService.UpdateProductVariantAsync(variantId, variantDto);
        return NoContent(); // Update successful
    }

    /// <summary>
    /// Deletes a variant by its ID.
    /// </summary>
    /// <param name="variantId">The ID of the variant to delete.</param>
    /// <returns>
    /// Returns NoContent if the deletion is successful.
    /// Returns a Forbid response if the user does not own the product.
    /// </returns>
    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpDelete("variants/{variantId:int}")]
    public async Task<IActionResult> DeleteProductVariant(int variantId)
    {
        var product = await productService.GetProductByVariantIdAsync(variantId);
        if (product == null || (product.SellerId != CurrentUserId && CurrentUserRole != "Admin"))
            return Forbid(); // User is not authorized to delete this variant

        await productService.DeleteProductVariantAsync(variantId);
        return NoContent(); // Deletion successful
    }

    /// <summary>
    /// Uploads an image for a product.
    /// </summary>
    /// <param name="imageFile">The image file to upload.</param>
    /// <returns>
    /// Returns an OK response with the URL of the uploaded image.
    /// Returns a BadRequest response if no image file is provided.
    /// </returns>
    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadProductImage([FromForm] IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return BadRequest("Image file is required."); // Image file is mandatory
        }

        var imageUrl =
            await imageHelper.ProcessAndUploadImageAsync(imageFile.OpenReadStream(), imageFile.FileName,
                "products-images");
        return Ok(new { ImageUrl = imageUrl }); // Return the URL of the uploaded image
    }
}