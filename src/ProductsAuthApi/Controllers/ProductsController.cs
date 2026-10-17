using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsAuthApi.Models;

namespace ProductsAuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private static readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Ноутбук Dell XPS 15", Price = 850000, Category = "Электроника", AddedBy = "System" },
        new Product { Id = 2, Name = "Смартфон iPhone 15 Pro", Price = 590000, Category = "Электроника", AddedBy = "System" },
        new Product { Id = 3, Name = "Офисное эргономичное кресло", Price = 95000, Category = "Мебель", AddedBy = "System" },
        new Product { Id = 4, Name = "Беспроводные наушники Sony WH-1000XM5", Price = 160000, Category = "Аудио", AddedBy = "System" }
    };

    /// <summary>
    /// Просмотр списка товаров (доступен без авторизации)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetAll()
    {
        return Ok(new
        {
            total = _products.Count,
            products = _products
        });
    }

    /// <summary>
    /// Просмотр одного товара по ID (доступен без авторизации)
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public IActionResult GetById(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound(new { message = $"Товар с идентификатором {id} не найден." });
        }
        return Ok(product);
    }

    /// <summary>
    /// Добавление нового товара (только с JWT токеном).
    /// Дополнительное задание: извлечение имени пользователя через HttpContext.User.Claims.
    /// </summary>
    [HttpPost]
    [Authorize]
    public IActionResult Create([FromBody] Product product)
    {
        if (product == null || string.IsNullOrWhiteSpace(product.Name))
        {
            return BadRequest(new { message = "Название товара не может быть пустым." });
        }

        // Извлечение Claim имени пользователя из контекста JWT
        var userName = HttpContext.User.Claims
                           .FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "unique_name")?.Value
                       ?? HttpContext.User.Identity?.Name
                       ?? "Аноним";

        var newId = _products.Any() ? _products.Max(p => p.Id) + 1 : 1;
        product.Id = newId;
        product.AddedBy = userName;
        _products.Add(product);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, new
        {
            message = "Товар успешно добавлен авторизованным пользователем!",
            authorClaim = userName,
            product = product
        });
    }

    /// <summary>
    /// Удаление товара (только с JWT токеном)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult Delete(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound(new { message = $"Товар с идентификатором {id} не найден." });
        }

        var deletedBy = HttpContext.User.Claims
                            .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                        ?? HttpContext.User.Identity?.Name
                        ?? "Пользователь";

        _products.Remove(product);

        return Ok(new
        {
            message = $"Товар с ID {id} успешно удален.",
            deletedBy = deletedBy,
            deletedProduct = product
        });
    }
}
