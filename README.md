# NormalNet

.NET Standard Library providing normalization of entities for easier API consumption.

Example:

```csharp
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
```

See [NormalNet.Test](NormalNet.Test) for more examples.
