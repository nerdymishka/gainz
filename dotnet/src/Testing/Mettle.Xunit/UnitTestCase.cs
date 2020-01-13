
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;

namespace Mettle
{


  
    public class UnitTestCase : Xunit.Sdk.TestMethodTestCase, IXunitTestCase
    {
        private static ConcurrentDictionary<string, IEnumerable<IAttributeInfo>> assemblyTraitAttributeCache = 
            new ConcurrentDictionary<string, IEnumerable<IAttributeInfo>>(StringComparer.OrdinalIgnoreCase);
        private static ConcurrentDictionary<string, IEnumerable<IAttributeInfo>> typeTraitAttributeCache = 
            new ConcurrentDictionary<string, IEnumerable<IAttributeInfo>>(StringComparer.OrdinalIgnoreCase);

        private int timeout;


         public UnitTestCase(IMessageSink diagnosticMessageSink,
                             TestMethodDisplay defaultMethodDisplay,
                             TestMethodDisplayOptions defaultMethodDisplayOptions,
                             ITestMethod testMethod,
                             object[] testMethodArguments = null)
            : base(defaultMethodDisplay, defaultMethodDisplayOptions, testMethod, testMethodArguments)
        {
           DiagnosticMessageSink = diagnosticMessageSink;
        }
  
        protected IMessageSink DiagnosticMessageSink { get; }

        /// <inheritdoc/>
        public int Timeout
        {
            get
            {
                EnsureInitialized();
                return this.timeout;
            }
            protected set
            {
                EnsureInitialized();
                this.timeout = value;
            }
        }

        /// <summary>
        /// Gets the display name for the test case. Calls <see cref="TypeUtility.GetDisplayNameWithArguments"/>
        /// with the given base display name (which is itself either derived from <see cref="FactAttribute.DisplayName"/>,
        /// falling back to <see cref="TestMethodTestCase.BaseDisplayName"/>.
        /// </summary>
        /// <param name="factAttribute">The fact attribute the decorated the test case.</param>
        /// <param name="displayName">The base display name from <see cref="TestMethodTestCase.BaseDisplayName"/>.</param>
        /// <returns>The display name for the test case.</returns>
        protected virtual string GetDisplayName(IAttributeInfo factAttribute, string displayName)
            => TestMethod.Method.GetDisplayNameWithArguments(displayName, TestMethodArguments, MethodGenericTypes);

        /// <summary>
        /// Gets the skip reason for the test case. By default, pulls the skip reason from the
        /// <see cref="FactAttribute.Skip"/> property.
        /// </summary>
        /// <param name="factAttribute">The fact attribute the decorated the test case.</param>
        /// <returns>The skip reason, if skipped; <c>null</c>, otherwise.</returns>
        protected virtual string GetSkipReason(IAttributeInfo factAttribute)
            => factAttribute.GetNamedArgument<string>("Skip");

        /// <summary>
        /// Gets the timeout for the test case. By default, pulls the skip reason from the
        /// <see cref="FactAttribute.Timeout"/> property.
        /// </summary>
        /// <param name="factAttribute">The fact attribute the decorated the test case.</param>
        /// <returns>The timeout in milliseconds, if set; 0, if unset.</returns>
        protected virtual int GetTimeout(IAttributeInfo factAttribute)
            => factAttribute.GetNamedArgument<int>("Timeout");

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();

            var factAttribute = TestMethod.Method.GetCustomAttributes(typeof(TestCaseAttribute)).First();
            var baseDisplayName = factAttribute.GetNamedArgument<string>("DisplayName") ?? BaseDisplayName;

            this.DisplayName = this.GetDisplayName(factAttribute, baseDisplayName);
            this.SkipReason = this.GetSkipReason(factAttribute);
            this.Timeout = this.GetTimeout(factAttribute);

            foreach (var traitAttribute in GetTraitAttributesData(TestMethod))
            {
                var discovererAttribute = traitAttribute.GetCustomAttributes(typeof(TraitDiscovererAttribute)).FirstOrDefault();
                if (discovererAttribute != null)
                {
                    var discoverer = ExtensibilityPointFactory.GetTraitDiscoverer(DiagnosticMessageSink, discovererAttribute);
                    if (discoverer != null)
                        foreach (var keyValuePair in discoverer.GetTraits(traitAttribute))
                            Traits.Add(keyValuePair.Key, keyValuePair.Value);
                }
                else
                    DiagnosticMessageSink.OnMessage(new DiagnosticMessage($"Trait attribute on '{DisplayName}' did not have [TraitDiscoverer]"));

                
            }
        }

        private static IEnumerable<IAttributeInfo> GetCachedTraitAttributes(IAssemblyInfo assembly)
        {
            return assemblyTraitAttributeCache.GetOrAdd(assembly.Name,
                (k) => assembly.GetCustomAttributes(typeof(ITraitAttribute)) );
        }

        static IEnumerable<IAttributeInfo> GetCachedTraitAttributes(ITypeInfo type)
            => typeTraitAttributeCache.GetOrAdd(type.Name, (k) => type.GetCustomAttributes(typeof(ITraitAttribute)));

        static IEnumerable<IAttributeInfo> GetTraitAttributesData(ITestMethod testMethod)
        {
            return GetCachedTraitAttributes(testMethod.TestClass.Class.Assembly)
                  .Concat(testMethod.Method.GetCustomAttributes(typeof(ITraitAttribute)))
                  .Concat(GetCachedTraitAttributes(testMethod.TestClass.Class));
        }

        /// <inheritdoc/>
        public virtual Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink,
                                                 IMessageBus messageBus,
                                                 object[] constructorArguments,
                                                 ExceptionAggregator aggregator,
                                                 CancellationTokenSource cancellationTokenSource)
            => new XunitTestCaseRunner(this, DisplayName, SkipReason, constructorArguments, TestMethodArguments, messageBus, aggregator, cancellationTokenSource).RunAsync();

        /// <inheritdoc/>
        public override void Serialize(IXunitSerializationInfo data)
        {
            base.Serialize(data);

            data.AddValue("Timeout", Timeout);
        }

        /// <inheritdoc/>
        public override void Deserialize(IXunitSerializationInfo data)
        {
            base.Deserialize(data);

            Timeout = data.GetValue<int>("Timeout");
        }
    }
}