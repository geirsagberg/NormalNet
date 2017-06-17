using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NormalNet.Test
{
    public class NormalizerTests_List
    {
        public class Order
        {
            public int Id { get; set; }
            public List<OrderLine> Lines { get; set; } = new List<OrderLine>();
        }

        public class OrderLine
        {
            public int Id { get; set; }
            public string Code { get; set; }
        }

        public class ViewModel
        {
            public Order Order { get; set; }
        }

        [Fact]
        public void Can_normalize_model_with_list()
        {
            var viewModel = new ViewModel {
                Order = new Order {
                    Id = 1,
                    Lines = {
                        new OrderLine {
                            Id = 1,
                            Code = "AB"
                        },
                        new OrderLine {
                            Id = 2,
                            Code = "XZ"
                        }
                    }
                }
            };

            var normalized = new Normalizer().Normalize(viewModel);

            normalized.ShouldBeEquivalentTo(new Dictionary<string, object> {
                {
                    "OrderId", 1
                }, {
                    "Entities", new Dictionary<string, object> {
                        {
                            "OrderLine", new Dictionary<string, object> {
                                {
                                    "1", new Dictionary<string, object> {
                                        {"Id", 1},
                                        {"Code", "AB"}
                                    }
                                }, {
                                    "2", new Dictionary<string, object> {
                                        {"Id", 2},
                                        {"Code", "XZ"}
                                    }
                                }
                            }
                        }, {
                            "Order", new Dictionary<string, object> {
                                {
                                    "1", new Dictionary<string, object> {
                                        {"Id", 1}, {
                                            "Lines", new[] {
                                                1, 2
                                            }
                                        }
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