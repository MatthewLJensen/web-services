using Microsoft.AspNetCore.JsonPatch;
﻿using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/companies")]
    [Authorize]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    //[ResponseCache(CacheProfileName = "120SecondsDuration")]
    //I'm not sure if Dr. A wants the authorize header for the entire controller or not...
    public class CompaniesController : ControllerBase
    {
        private readonly IServiceManager _service;
        public CompaniesController(IServiceManager service) => _service = service;

        /// <summary>
        /// Gets the list of all companies
        /// </summary>
        /// <returns>The companies list</returns>
        [HttpGet(Name = "GetCompanies")]
        //[Authorize(Roles = "Manager")]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public async Task<IActionResult> GetCompanies([FromQuery] CompanyParameters companyParameters)
        {
            //throw new Exception("Exception");
            var linkParams = new CompanyLinkParameters(companyParameters, HttpContext);
            var result = await _service.CompanyService.GetAllCompaniesAsync(companyParameters, linkParams, trackChanges: false);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.metaData));
            return result.linkResponse.HasLinks ? Ok(result.linkResponse.LinkedEntities) : Ok(result.linkResponse.ShapedEntities);
        }

        /// <summary>
        /// Gets a specific company, based on the ID that was passed in
        /// </summary>
        /// <returns>The specified company</returns>
        [HttpGet("{id:guid}", Name = "CompanyById")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _service.CompanyService.GetCompanyAsync(id, trackChanges: false);
            return Ok(company);
        }

        /// <summary>
        /// Creates a newly created company
        /// </summary>
        /// <param name="company"></param>
        /// <returns>A newly created company</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        /// <response code="422">If the model is invalid</response>
        [HttpPost(Name = "CreateCompany")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            var createdCompany = await _service.CompanyService.CreateCompanyAsync(company);
            return CreatedAtRoute("CompanyById", new { id = createdCompany.Id },
            createdCompany);
        }

        /// <summary>
        /// Gets a collection of companies
        /// </summary>
        /// <returns>A collection of companies</returns>
        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection ([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            var companies = await _service.CompanyService.GetByIdsAsync(ids, trackChanges: false);
            return Ok(companies);
        }

        /// <summary>
        /// Creates a collection of companies
        /// </summary>
        /// <returns>The companies that were created by the action</returns>
        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection ([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            var result = await _service.CompanyService.CreateCompanyCollectionAsync(companyCollection);
            return CreatedAtRoute("CompanyCollection", new { result.ids },
            result.companies);
        }

        /// <summary>
        /// Deletes the company with the specificed ID
        /// </summary>
        /// <returns>204 No Content</returns>
        /// <response code="204">If the company is deleted</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            await _service.CompanyService.DeleteCompanyAsync(id, trackChanges: false);
            return NoContent();
        }

        /// <summary>
        /// Updates company
        /// </summary>
        /// <returns>204 No Content</returns>
        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
        {
            await _service.CompanyService.UpdateCompanyAsync(id, company, trackChanges: true);
            return NoContent();
        }

        /// <summary>
        /// Patches company
        /// </summary>
        /// <returns>204 No Content</returns>
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> PartiallyUpdateCompany(Guid id, [FromBody] JsonPatchDocument<CompanyForUpdateDto> patchDoc)
        {
            if (patchDoc is null)
                return BadRequest("patchDoc sent from client is null.");
            var result = await _service.CompanyService.GetCompanyForPatchAsync(id, trackChanges: true);
            patchDoc.ApplyTo(result.companyToPatch);
            TryValidateModel(result.companyToPatch);
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            await _service.CompanyService.SaveChangesForPatchAsync(result.companyToPatch, result.companyEntity);
            return NoContent();
        }

        /// <summary>
        /// Returns API options
        /// </summary>
        /// <returns>Response Headers</returns>
        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST"); // do I need to add Delete and Put?
            return Ok();
        }
    }
}