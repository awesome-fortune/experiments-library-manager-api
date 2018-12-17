using Microsoft.AspNetCore.Mvc;
using LibraryManager.Api.Services;
using System.Collections.Generic;
using LibraryManager.Api.Models;
using System;
using LibraryManager.Api.Entities;
using Microsoft.AspNetCore.Http;
using LibraryManager.Api.Helpers;
using Newtonsoft.Json;
using System.Linq;

namespace LibraryManager.Api.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;

        public AuthorsController(ILibraryRepository libraryRepository,
            IUrlHelper urlHelper,
            IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _libraryRepository = libraryRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetAuthors")]
        public IActionResult GetAuthors([FromQuery] AuthorsResourceParameters authorsResourceParameters, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<AuthorDto, Author>(authorsResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<AuthorDto>(authorsResourceParameters.Fields))
            {
                return BadRequest();
            }

            var authorsFromRepo = _libraryRepository.GetAuthors(authorsResourceParameters);

            var authors = AutoMapper.Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);

            if (mediaType == CustomRequestHeaders.CUSTOM_HATEOAS_MEDIA_TYPE)
            {
                var paginationMetadata = new
                {
                    totalCount = authorsFromRepo.TotalCount,
                    pageSize = authorsFromRepo.PageSize,
                    currentPage = authorsFromRepo.CurrentPage,
                    totalsPages = authorsFromRepo.TotalPages,
                };

                Response.Headers.Add(CustomRequestHeaders.X_PAGINATION, JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateLinksForAuthors(
                    authorsResourceParameters,
                    authorsFromRepo.HasNext,
                    authorsFromRepo.HasPrevious
                );

                var shapedAuthors = authors.ShapeData(authorsResourceParameters.Fields);

                var shapedAuthorsWithLinks = shapedAuthors.Select(author =>
                {
                    var authorAsDictionary = author as IDictionary<string, object>;
                    // var authorLinks = CreateLinksForAuthor((Guid)authorAsDictionary["id"], authorsResourceParameters.Fields);

                    // authorAsDictionary.Add("links", authorLinks);

                    return authorAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedAuthorsWithLinks,
                    links = links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                var previousPageLink = authorsFromRepo.HasPrevious ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage) : null;
                var nextPageLink = authorsFromRepo.HasNext ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage) : null;
                var paginationMetadata = new
                {
                    previousPageLink = previousPageLink,
                    nextPageLink = nextPageLink,
                    toatlCount = authorsFromRepo.TotalCount,
                    pageSize = authorsFromRepo.PageSize,
                    currentPage = authorsFromRepo.CurrentPage,
                    toatlPages = authorsFromRepo.TotalPages
                };

                Response.Headers.Add(CustomRequestHeaders.X_PAGINATION, JsonConvert.SerializeObject(paginationMetadata));

                return Ok(authors.ShapeData(authorsResourceParameters.Fields));
            }
        }

        private string CreateAuthorsResourceUri(AuthorsResourceParameters authorsResourceParameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetAuthors",
                        new
                        {
                            fields = authorsResourceParameters.Fields,
                            orderBy = authorsResourceParameters.OrderBy,
                            searchQuery = authorsResourceParameters.SearchQuery,
                            genre = authorsResourceParameters.Genre,
                            pageNumber = authorsResourceParameters.PageNumber - 1,
                            pageSize = authorsResourceParameters.PageSize
                        }
                    );

                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetAuthors",
                        new
                        {
                            fields = authorsResourceParameters.Fields,
                            orderBy = authorsResourceParameters.OrderBy,
                            searchQuery = authorsResourceParameters.SearchQuery,
                            genre = authorsResourceParameters.Genre,
                            pageNumber = authorsResourceParameters.PageNumber + 1,
                            pageSize = authorsResourceParameters.PageSize
                        }
                    );

                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link("GetAuthors",
                        new
                        {
                            fields = authorsResourceParameters.Fields,
                            orderBy = authorsResourceParameters.OrderBy,
                            searchQuery = authorsResourceParameters.SearchQuery,
                            genre = authorsResourceParameters.Genre,
                            pageNumber = authorsResourceParameters.PageNumber,
                            pageSize = authorsResourceParameters.PageSize
                        }
                    );
            }
        }

        [HttpGet("{id}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid id, [FromQuery] string fields)
        {
            if (!_typeHelperService.TypeHasProperties<AuthorDto>(fields))
            {
                return BadRequest();
            }
            var authorFromRepo = _libraryRepository.GetAuthor(id);

            if (authorFromRepo == null)
            {
                return NotFound();
            }

            var author = AutoMapper.Mapper.Map<AuthorDto>(authorFromRepo);
            var links = CreateLinksForAuthor(id, fields);
            var linkedResourceToReturn = author.ShapeData(fields) as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorCreateDto author)
        {
            if (author == null)
            {
                return BadRequest();
            }

            var authorEntity = AutoMapper.Mapper.Map<Author>(author);

            _libraryRepository.AddAuthor(authorEntity);

            if (!_libraryRepository.Save())
            {
                throw new Exception("Creating an author failed on save.");
            }

            var authorToReturn = AutoMapper.Mapper.Map<AuthorDto>(authorEntity);

            var links = CreateLinksForAuthor(authorToReturn.Id, null);

            var linkedResourceToReturn = authorToReturn.ShapeData(null) as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetAuthor", new { id = linkedResourceToReturn["Id"] }, linkedResourceToReturn);
        }

        [HttpPost("{id}")]
        public IActionResult BlockAuthorCreation(Guid id)
        {
            if (_libraryRepository.AuthorExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpDelete("{id}", Name = "DeleteAuthor")]
        public IActionResult DeleteAuthor(Guid id)
        {
            var authorsFromRepo = _libraryRepository.GetAuthor(id);

            if (authorsFromRepo == null)
            {
                return NotFound();
            }

            _libraryRepository.DeleteAuthor(authorsFromRepo);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Deleting author {id} failed on save.");
            }

            return NoContent();
        }

        private IEnumerable<LinkDto> CreateLinksForAuthor(Guid id, string fields)
        {
            // HATEOAS for shaped data :D
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(_urlHelper.Link("GetAuthor", new { id = id }), "self", "GET"));
            }
            else
            {
                links.Add(new LinkDto(_urlHelper.Link("GetAuthor", new { id = id, fields = fields }), "self", "GET"));
            }

            links.Add(new LinkDto(_urlHelper.Link("DeleteAuthor", new { id = id }), "delete_author", "DELETE"));

            links.Add(new LinkDto(_urlHelper.Link("CreateBookForAuthor", new { authorId = id }), "create_book_for_author", "POST"));

            links.Add(new LinkDto(_urlHelper.Link("GetBooksForAuthor", new { authorId = id }), "books", "GET"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForAuthors(AuthorsResourceParameters authorsResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self
            links.Add(new LinkDto(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.Current), "self", "GET"));

            if (hasNext)
            {
                links.Add(new LinkDto(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage), "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage), "previousPage", "GET"));
            }

            return links;
        }
    }
}