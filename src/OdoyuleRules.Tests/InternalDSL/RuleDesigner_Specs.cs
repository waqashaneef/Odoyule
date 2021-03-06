﻿// Copyright 2011 Chris Patterson
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace OdoyuleRules.Tests.InternalDSL
{
    using System.Collections.Generic;
    using System.Linq;
    using Designer;
    using NUnit.Framework;
    using Visualizer;

    [TestFixture]
    public class Designing_a_single_fact_rule
    {
        [Test]
        public void Should_match_the_alpha_node()
        {
            _result = null;

            using (StatefulSession session = _engine.CreateSession())
            {
                session.Add(new Order {OrderId = "123", Amount = 10001.0m});
                session.Run();

                List<FactHandle<Violation>> violations = session.Facts<Violation>().ToList();

                Assert.AreEqual(1, violations.Count);
                Assert.IsNotNull(_result);
                Assert.AreEqual("123", _result.Value);
            }
        }


        [Test]
        [Explicit]
        public void Show_me()
        {
            _engine.ShowVisualizer();
        }

        RulesEngine _engine;
        static Violation _result;

        [TestFixtureSetUp]
        public void Setup()
        {
            _engine = RulesEngineFactory.New(x =>
                {
                    // add our rule
                    x.Rule<SingleFactRule>();
                    x.Rule<ViolationRule>();
                });
        }

        class SingleFactRule :
            RuleDefinition
        {
            public SingleFactRule()
            {
                Fact<Order>()
                    .When(o => o.Amount > 10000.0m)
                    .Then(x => x.Add(o => new Violation(o.OrderId, "Large Order Hold")));
            }
        }

        class ViolationRule :
            RuleDefinition
        {
            public ViolationRule()
            {
                Fact<Violation>()
                    .When(x => x.Value == "123")
                    .Then(x => x.Delegate(v => _result = v));
            }
        }

        class Order
        {
            public string OrderId { get; set; }
            public decimal Amount { get; set; }
        }

        class Violation
        {
            public Violation(string value, string description)
            {
                Value = value;
                Description = description;
            }

            public string Value { get; set; }
            public string Description { get; set; }
        }
    }
}