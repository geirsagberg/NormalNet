using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NormalNet.Test
{
    public class NormalizerTests_Nested
    {
        public class OfficeViewModel
        {
            public Address Address { get; set; }
            public Person Owner { get; set; }
        }

        public class Person
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public Address HomeAddress { get; set; }
        }

        public class Address
        {
            public int Id { get; set; }
            public string Street { get; set; }
            public string PostalCode { get; set; }
        }

        [Fact]
        public void Can_normalize_nested_model()
        {
            var viewModel = new OfficeViewModel {
                Address = new Address {
                    Id = 1,
                    PostalCode = "7044",
                    Street = "Bortegaten 4"
                },
                Owner = new Person {
                    Id = 3,
                    FirstName = "Eiermann",
                    LastName = "Eiersen",
                    HomeAddress = new Address {
                        Id = 5,
                        PostalCode = "7052",
                        Street = "Hjemmegaten 9"
                    }
                }
            };

            var normalized = new Normalizer().Normalize(viewModel);

            normalized.ShouldBeEquivalentTo(new Dictionary<string, object> {
                {"AddressId", 1},
                {"OwnerId", 3}, {
                    "Entities", new Dictionary<string, object> {
                        {
                            "Address", new Dictionary<string, object> {
                                {
                                    "1", new Dictionary<string, object> {
                                        {"Id", 1},
                                        {"PostalCode", "7044"},
                                        {"Street", "Bortegaten 4"}
                                    }
                                }, {
                                    "5", new Dictionary<string, object> {
                                        {"Id", "5"},
                                        {"PostalCode", "7052"},
                                        {"Street", "Hjemmegaten 9"}
                                    }
                                }
                            }
                        }, {
                            "Person", new Dictionary<string, object> {
                                {
                                    "3", new Dictionary<string, object> {
                                        {"Id", "3"},
                                        {"FirstName", "Eiermann"},
                                        {"LastName", "Eiersen"},
                                        {"HomeAddressId", "5"}
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}