using System;
using System.Collections.Generic;
using System.Linq;
using LibraryManager.Api.Entities;
using LibraryManager.Api.Helpers;
using LibraryManager.Api.Models;

namespace LibraryManager.Api.Services
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly LibraryContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public LibraryRepository(LibraryContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public void AddAuthor(Author author)
        {
            author.Id = Guid.NewGuid();
            _context.Authors.Add(author);

            // The repository fills the id (instead of using identity columns)
            if (author.Books.Any())
            {
                foreach (Book book in author.Books)
                {
                    book.Id = Guid.NewGuid();
                }
            }
        }

        public void AddBookForAuthor(Guid authorId, Book book)
        {
            Author author = GetAuthor(authorId);

            if (author != null)
            {
                /* If there isn't an id filled out (i.e : we're not upserting), we 
                should generate one */
                if (book.Id == Guid.Empty)
                {
                    book.Id = Guid.NewGuid();
                }

                author.Books.Add(book);
            }
        }

        public bool AuthorExists(Guid authorId)
        {
            return _context.Authors
                .Any(x => x.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            _context.Authors.Remove(author);
        }

        public void DeleteBook(Book book)
        {
            _context.Books.Remove(book);
        }

        public Author GetAuthor(Guid authorId)
        {
            return _context.Authors
                .FirstOrDefault(x => x.Id == authorId);
        }

        public PagedList<Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            // var collectionBeforePaging =  _context.Authors
            //     .OrderBy(x => x.FirstName)
            //     .ThenBy(x => x.LastName).AsQueryable();

            var collectionBeforePaging = _context.Authors
                .ApplySort(authorsResourceParameters.OrderBy,
                 _propertyMappingService.GetPropertyMapping<AuthorDto, Author>());

            if (!string.IsNullOrEmpty(authorsResourceParameters.Genre))
            {
                // trim & ignore casing
                var genreForWhereClause = authorsResourceParameters.Genre
                    .Trim()
                    .ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(x => x.Genre.ToLowerInvariant() == genreForWhereClause);
            }

            if (!string.IsNullOrEmpty(authorsResourceParameters.SearchQuery))
            {
                // Trim & ignore casing
                var searchQueryForWhereClause = authorsResourceParameters.SearchQuery
                    .Trim()
                    .ToLowerInvariant();

                    collectionBeforePaging = collectionBeforePaging
                        .Where(x => x.Genre.ToLowerInvariant().Contains(searchQueryForWhereClause)
                        || x.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                        || x.LastName.ToLowerInvariant().Contains(searchQueryForWhereClause));

            }

            return PagedList<Author>.Create(
                collectionBeforePaging,
                authorsResourceParameters.PageNumber,
                authorsResourceParameters.PageSize
            );
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            return _context.Authors.Where(x => authorIds.Contains(x.Id))
                .OrderBy(x => x.FirstName)
                .OrderBy(x => x.LastName)
                .ToList();
        }

        public Book GetBookForAuthor(Guid authorId, Guid bookId)
        {
            return _context.Books
                .Where(x => x.AuthorId == authorId && x.Id == bookId)
                .FirstOrDefault();
        }

        public IEnumerable<Book> GetBooksForAuthor(Guid authorId)
        {
            return _context.Books
                .Where(x => x.AuthorId == authorId)
                .OrderBy(x => x.Title)
                .ToList();
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateAuthor(Author author)
        {
        }

        public void UpdateBookForAuthor(Book book)
        {
        }
    }
}