using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NormalNet.Test
{
    public class NormalizerTests_Simple
    {
        public class Person
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class SimpleViewModel
        {
            public Person Owner { get; set; }
            public Person Employee { get; set; }
        }

        [Fact]
        public void Can_normalize_simple_model()
        {
            var viewModel = new SimpleViewModel {
                Owner = new Person {
                    Id = 1,
                    FirstName = "Geir",
                    LastName = "Sagberg"
                },
                Employee = new Person {
                    Id = 2,
                    FirstName = "Ola",
                    LastName = "Normann"
                }
            };

            var normalized = new Normalizer().Normalize(viewModel);

            var expectation = new Dictionary<string, object> {
                {"OwnerId", 1},
                {"EmployeeId", 2}, {
                    "Entities", new Dictionary<string, object> {
                        {
                            "Person", new Dictionary<string, object> {
                                {
                                    "1", new Dictionary<string, object> {
                                        {"Id", 1},
                                        {"FirstName", "Geir"},
                                        {"LastName", "Sagberg"}
                                    }
                                }, {
                                    "2", new Dictionary<string, object> {
                                        {"Id", 2},
                                        {"FirstName", "Ola"},
                                        {"LastName", "Normann"}
                                    }
                                }
                            }
                        }
                    }
                }
            };
            normalized.ShouldBeEquivalentTo(expectation);
        }
    }
}