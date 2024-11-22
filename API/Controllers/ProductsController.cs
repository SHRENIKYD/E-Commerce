using System;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ProductsController(IProductRepository repository) : ControllerBase
{
    

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
    {
        return Ok(await repository.GetProductAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProductById(int id)
    {
        var product = await repository.GetProductByIdAsync(id);
        if(product == null) return NotFound();
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        repository.AddProduct(product);
        if(await repository.SaveChangesAsync()){
            return CreatedAtAction("GetProductById", new {id = product.Id},product);
        }
        return BadRequest("Problem creating product");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
    {
        if(product.Id != id || !ProductExists(id)) 
            return BadRequest("Cannot update this product");
        
        repository.UpdateProduct(product);
        
        if(await repository.SaveChangesAsync()){
            return NoContent();
        }

        return BadRequest("Problem updating the product");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Product>> DeleteProduct(int id)
    {
        var product = await repository.GetProductByIdAsync(id);

        if(product == null) return NotFound();
        repository.DeleteProduct(product);
         if(await repository.SaveChangesAsync()){
            return NoContent();
        }

        return BadRequest("Problem deleting the product");
    }

      private bool ProductExists(int id){
        return repository.ProductExists(id);
    }
}
