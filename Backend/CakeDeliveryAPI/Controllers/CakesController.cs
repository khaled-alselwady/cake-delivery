﻿using Business_Layer.Cake;
using Business_Layer.Order;
using CakeDeliveryAPI.Controllers;
using CakeDeliveryDTO.CakeDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[Route("api/cakes")]
[ApiController]
public class CakesController : BaseController
{
    private readonly clsCakeValidator _validator = new clsCakeValidator();

    // GET: api/cakes/all
    [HttpGet("All", Name = "GetAllCakes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<CakeDTO>> GetAllCakes()
        => GetAllEntities(() => clsCake.All());

    
    // GET: api/cakes/{id}
    [HttpGet("{id}", Name = "GetCakeById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<CakeDTO> GetCakeById(int id)
        => GetEntityByIdentifier(id, clsCake.FindCakeById, cake => Ok(cake));

  
    // GET: api/cakes/name/{cakeName}
    [HttpGet("name/{cakeName}", Name = "GetCakeByName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<CakeDTO> GetCakeByName(string cakeName)
        => GetEntityByIdentifier(cakeName, clsCake.FindCakeByName, cake => Ok(cake));


    // POST: api/cakes
    [HttpPost(Name = "AddCake")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<CakeDTO> AddCake([FromBody] CakeCreateDto newCakeDTO)
    {
        if (newCakeDTO == null || string.IsNullOrEmpty(newCakeDTO.CakeName))
        {
            return BadRequest("Invalid cake data.");
        }

        clsCake cakeInstance = new clsCake(
            new CakeDTO(null, newCakeDTO.CakeName, newCakeDTO.Description, newCakeDTO.Price, newCakeDTO.StockQuantity, newCakeDTO.CategoryID, newCakeDTO.ImageUrl),
            clsCake.enMode.AddNew
        );

        var validationResult = _validator.Validate(cakeInstance);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                Success = false,
                Errors = validationResult.Errors
            });
        }

        if (cakeInstance.Save())
        {
            return CreatedAtRoute("GetCakeById", new { id = cakeInstance.CakeID }, newCakeDTO);
        }

        return BadRequest("Unable to create cake.");
    }

   
    // PUT: api/cakes/{id}
    [HttpPut("{id}", Name = "UpdateCake")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<CakeDTO> UpdateCake(int id, CakeDTO updatedCake)
    {
        if (id < 1 || updatedCake == null || string.IsNullOrEmpty(updatedCake.CakeName))
        {
            return BadRequest("Invalid cake data.");
        }

        CakeDTO? existingCake = clsCake.FindCakeById(id);
        if (existingCake == null)
        {
            return NotFound($"Cake with ID {id} not found.");
        }

        clsCake cakeInstance = new clsCake(updatedCake, clsCake.enMode.Update);

        var validationResult = _validator.Validate(cakeInstance);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                Success = false,
                Errors = validationResult.Errors
            });
        }

        if (cakeInstance.Save())
        {
            return Ok(cakeInstance.ToCakeDto());
        }

        return StatusCode(500, new { message = "Error updating cake." });
    }

   
    // DELETE: api/cakes/{id}
    [HttpDelete("{id}", Name = "DeleteCake")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    public ActionResult DeleteCake(int id)
      => DeleteEntity<clsCake>(id, clsCake.Delete, "Cake");

   
    // GET: api/cakes/category/{category}
    [HttpGet("category/{category}", Name = "GetCakesByCategory")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<CakeDTO>> GetCakesByCategory(int category)
        => GetAllEntitiesBy<int, CakeDTO, int>(category, clsCake.AllByCategoryID, "Cake");


    // GET: api/cakes/category/name/{categoryName}
    [HttpGet("category/name/{categoryName}", Name = "GetCakesByCategoryName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<CakeDTO>> GetCakesByCategoryName(string categoryName)
        => GetAllEntitiesBy<string, CakeDTO, string>(categoryName, clsCake.AllByCategoryName, "Cake");
}
