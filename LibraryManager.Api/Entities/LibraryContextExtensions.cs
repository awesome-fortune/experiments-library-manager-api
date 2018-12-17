using System;
using System.Collections.Generic;

namespace LibraryManager.Api.Entities
{
    public static class LibraryContextExtensions
    {
        public static void EnsureSeedDataForContext(this LibraryContext context)
        {
            context.Authors.RemoveRange(context.Authors);
            context.SaveChanges();

            // Init seed data
            List<Author> authors = new List<Author>()
            {
                new Author()
                {
                    Id = new Guid("afe2a092-ccd3-4847-ada5-54f3ec0268e2"),
                    FirstName = "Stephen",
                    LastName = "King",
                    Genre = "Horror",
                    DateOfBirth = new DateTimeOffset(new DateTime(1947, 9, 21)),
                    Books = new List<Book>
                    {
                        new Book()
                         {
                             Id = new Guid("12c56c8a-107c-4e13-904f-cd83d0385799"),
                             Title = "The Shining",
                             Description = "The Shining is a horror novel by American author Stephen King. Published in 1977, it is King's third published novel and first hardback bestseller: the success of the book firmly established King as a preeminent author in the horror genre. "
                         },
                         new Book()
                         {
                             Id = new Guid("85821592-656d-4b54-b5b2-1e85f323bf11"),
                             Title = "Misery",
                             Description = "Misery is a 1987 psychological horror novel by Stephen King. This novel was nominated for the World Fantasy Award for Best Novel in 1988, and was later made into a Hollywood film and an off-Broadway play of the same name."
                         },
                         new Book()
                         {
                             Id = new Guid("142a5b9f-3d90-4dac-9b64-29b24a21b7f9"),
                             Title = "It",
                             Description = "It is a 1986 horror novel by American author Stephen King. The story follows the exploits of seven children as they are terrorized by the eponymous being, which exploits the fears and phobias of its victims in order to disguise itself while hunting its prey. 'It' primarily appears in the form of a clown in order to attract its preferred prey of young children."
                         },
                         new Book()
                         {
                             Id = new Guid("d8a1402e-92c8-4946-91d6-1fa453252174"),
                             Title = "The Stand",
                             Description = "The Stand is a post-apocalyptic horror/fantasy novel by American author Stephen King. It expands upon the scenario of his earlier short story 'Night Surf' and outlines the total breakdown of society after the accidental release of a strain of influenza that had been modified for biological warfare causes an apocalyptic pandemic which kills off the majority of the world's human population."
                         }
                    }
                },
                new Author()
                {
                     Id = new Guid("cae1a7eb-fa88-4b46-bdfd-b3a44af0cb31"),
                     FirstName = "George",
                     LastName = "RR Martin",
                     Genre = "Fantasy",
                     DateOfBirth = new DateTimeOffset(new DateTime(1948, 9, 20)),
                     Books = new List<Book>()
                     {
                         new Book()
                         {
                             Id = new Guid("136c1fe0-2994-4823-a08a-602c80e11900"),
                             Title = "A Game of Thrones",
                             Description = "A Game of Thrones is the first novel in A Song of Ice and Fire, a series of fantasy novels by American author George R. R. Martin. It was first published on August 1, 1996."
                         },
                         new Book()
                         {
                             Id = new Guid("a30e8ede-6d0b-4768-8241-e026d5e0f8c0"),
                             Title = "The Winds of Winter",
                             Description = "Forthcoming 6th novel in A Song of Ice and Fire."
                         },
                         new Book()
                         {
                             Id = new Guid("2cda8ee3-4403-4551-8423-66d525787024"),
                             Title = "A Dance with Dragons",
                             Description = "A Dance with Dragons is the fifth of seven planned novels in the epic fantasy series A Song of Ice and Fire by American author George R. R. Martin."
                         }
                     }
                },
                new Author()
                {
                     Id = new Guid("ac80633d-438d-40b2-b1ab-cb5b4bf1bcd2"),
                     FirstName = "Neil",
                     LastName = "Gaiman",
                     Genre = "Fantasy",
                     DateOfBirth = new DateTimeOffset(new DateTime(1960, 11, 10)),
                     Books = new List<Book>()
                     {
                         new Book()
                         {
                             Id = new Guid("be171a97-50be-4cb0-a8b4-cd13604001e4"),
                             Title = "American Gods",
                             Description = "American Gods is a Hugo and Nebula Award-winning novel by English author Neil Gaiman. The novel is a blend of Americana, fantasy, and various strands of ancient and modern mythology, all centering on the mysterious and taciturn Shadow."
                         }
                     }
                },
                new Author()
                {
                     Id = new Guid("9553e44e-6f68-4c05-b269-a0867f6c34ac"),
                     FirstName = "Tom",
                     LastName = "Lanoye",
                     Genre = "Various",
                     DateOfBirth = new DateTimeOffset(new DateTime(1958, 8, 27)),
                     Books = new List<Book>()
                     {
                         new Book()
                         {
                             Id = new Guid("cc85270e-6059-4549-a58a-9708662573e5"),
                             Title = "Speechless",
                             Description = "Good-natured and often humorous, Speechless is at times a 'song of curses', as Lanoye describes the conflicts with his beloved diva of a mother and her brave struggle with decline and death."
                         }
                     }
                },
                new Author()
                {
                     Id = new Guid("337d2e4a-8e36-48a0-8378-ebf112dc0605"),
                     FirstName = "Douglas",
                     LastName = "Adams",
                     Genre = "Science fiction",
                     DateOfBirth = new DateTimeOffset(new DateTime(1952, 3, 11)),
                     Books = new List<Book>()
                     {
                         new Book()
                         {
                             Id = new Guid("114712c2-4085-43b5-8450-9cce9434215f"),
                             Title = "The Hitchhiker's Guide to the Galaxy",
                             Description = "The Hitchhiker's Guide to the Galaxy is the first of five books in the Hitchhiker's Guide to the Galaxy comedy science fiction 'trilogy' by Douglas Adams."
                         }
                     }
                },
                new Author()
                {
                     Id = new Guid("feda72b1-b399-41dd-bbc4-2f41e2efe8d6"),
                     FirstName = "Jens",
                     LastName = "Lapidus",
                     Genre = "Thriller",
                     DateOfBirth = new DateTimeOffset(new DateTime(1974, 5, 24)),
                     Books = new List<Book>()
                     {
                         new Book()
                         {
                             Id = new Guid("a38656a8-585d-4d77-84bc-319fd6842c65"),
                             Title = "Easy Money",
                             Description = "Easy Money or Snabba cash is a novel from 2006 by Jens Lapidus. It has been a success in term of sales, and the paperback was the fourth best seller of Swedish novels in 2007."
                         }
                     }
                }
            };

            context.Authors.AddRange(authors);
            context.SaveChanges();
        }
    }
}