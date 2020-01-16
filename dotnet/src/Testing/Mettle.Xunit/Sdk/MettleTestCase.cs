using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Xunit.Sdk
{
    /// <summary>
    /// Default implementation of <see cref="IXunitTestCase"/> for xUnit v2 that supports tests decorated with
    /// both <see cref="FactAttribute"/> and <see cref="TheoryAttribute"/>.
    /// </summary>
    [DebuggerDisplay(@"\{ class = {TestMethod.TestClass.Class.Name}, method = {TestMethod.Method.Name}, display = {DisplayName}, skip = {SkipReason} \}")]
    [Serializable]
    public class MettleTestCase : TestMethodTestCase, IXunitTestCase, IMettleTestCase
    {
        static ConcurrentDictionary<string, IEnumerable<IAttributeInfo>> assemblyTraitAttributeCache = new ConcurrentDictionary<string, IEnumerable<IAttributeInfo>>(StringComparer.OrdinalIgnoreCase);
        static ConcurrentDictionary<string, IEnumerable<IAttributeInfo>> typeTraitAttributeCache = new ConcurrentDictionary<string, IEnumerable<IAttributeInfo>>(StringComparer.OrdinalIgnoreCase);
        int timeout;

        private IServiceProvider serviceProvider;

        /// <summary/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public MettleTestCase()
        {
            // No way for us to get access to the message sink on the execution deserialization path, but that should
            // be okay, because we assume all the issues were reported during discovery.
            DiagnosticMessageSink = new NullMessageSink();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XunitTestCase"/> class.
        /// </summary>
        /// <param name="diagnosticMessageSink">The message sink used to send diagnostic messages</param>
        /// <param name="defaultMethodDisplay">Default method display to use (when not customized).</param>
        /// <param name="testMethod">The test method this test case belongs to.</param>
        /// <param name="testMethodArguments">The arguments for the test method.</param>
        [Obsolete("Please call the constructor which takes TestMethodDisplayOptions")]
        public MettleTestCase(IMessageSink diagnosticMessageSink,
                             TestMethodDisplay defaultMethodDisplay,
                             ITestMethod testMethod,
                             IServiceProvider serviceProvider,
                             object[] testMethodArguments = null)
            : this(diagnosticMessageSink, defaultMethodDisplay, TestMethodDisplayOptions.None, testMethod, serviceProvider, testMethodArguments) 
            
        { 
            
        }


        internal protected new TestMethodDisplay DefaultMethodDisplay { get; }

        internal protected new TestMethodDisplayOptions DefaultMethodDisplayOptions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XunitTestCase"/> class.
        /// </summary>
        /// <param name="diagnosticMessageSink">The message sink used to send diagnostic messages</param>
        /// <param name="defaultMethodDisplay">Default method display to use (when not customized).</param>
        /// <param name="defaultMethodDisplayOptions">Default method display options to use (when not customized).</param>
        /// <param name="testMethod">The test method this test case belongs to.</param>
        /// <param name="testMethodArguments">The arguments for the test method.</param>
        public MettleTestCase(IMessageSink diagnosticMessageSink,
                             TestMethodDisplay defaultMethodDisplay,
                             TestMethodDisplayOptions defaultMethodDisplayOptions,
                             ITestMethod testMethod,
                             IServiceProvider serviceProvider,
                             object[] testMethodArguments = null)
            : base(defaultMethodDisplay, defaultMethodDisplayOptions, testMethod, testMethodArguments)
        {
            DefaultMethodDisplay = defaultMethodDisplay;
            DefaultMethodDisplayOptions = defaultMethodDisplayOptions;
            DiagnosticMessageSink = diagnosticMessageSink;
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the message sink used to report <see cref="IDiagnosticMessage"/> messages.
        /// </summary>
        protected IMessageSink DiagnosticMessageSink { get; }

        /// <inheritdoc/>
        public int Timeout
        {
            get
            {
                EnsureInitialized();
                return timeout;
            }
            protected set
            {
                EnsureInitialized();
                timeout = value;
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
            => factAttribute.GetNamedArgument<string>("SkipReason");

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
            var serviceProviderFactoryAttribute = TestMethod.Method
                .GetCustomAttributes(typeof(ServiceProviderFactoryAttribute))
                .SingleOrDefault();

            if(serviceProviderFactoryAttribute == null)
            {
                serviceProviderFactoryAttribute = TestMethod.TestClass.Class.Assembly
                    .GetCustomAttributes(typeof(ServiceProviderFactoryAttribute))
                    .SingleOrDefault();
            }

            if(serviceProviderFactoryAttribute != null)
            {
                var factoryType = serviceProviderFactoryAttribute.GetNamedArgument<Type>("FactoryType");

                // TODO: consider diagnostic message if the type exists but not the interface.
                if(factoryType != null && factoryType.GetInterface("IServiceProviderFactory") != null) 
                {
                    var serviceFactory = (IServiceProviderFactory)Activator.CreateInstance(factoryType);   
                    this.serviceProvider = serviceFactory.CreateProvider();
                }
            } else {
                this.serviceProvider = new SimpleServiceProvider();
            }
            
            bool isTestCase = true;
            var factAttribute =TestMethod.Method.GetCustomAttributes(typeof(TestCaseAttribute)).FirstOrDefault();
           
            if(factAttribute == null)
            {
                factAttribute = TestMethod.Method.GetCustomAttributes(typeof(FactAttribute)).FirstOrDefault();
                isTestCase = false;
            }

            var baseDisplayName = factAttribute.GetNamedArgument<string>("DisplayName") ?? BaseDisplayName;
            DisplayName = GetDisplayName(factAttribute, baseDisplayName);
            SkipReason = GetSkipReason(factAttribute);
            Timeout = GetTimeout(factAttribute);

            if(isTestCase)
            {
                string id = factAttribute.GetNamedArgument<string>("Id");
                string ticket= factAttribute.GetNamedArgument<string>("Ticket");
                string tags = factAttribute.GetNamedArgument<string>("Tags");

                if(!string.IsNullOrWhiteSpace(id))
                    Traits.Add("id", id);

                if(!string.IsNullOrWhiteSpace(ticket))
                    Traits.Add("ticket", ticket);

                if(!string.IsNullOrWhiteSpace(tags))
                {
                    var set = tags.Split(';').Select(o => o.Trim()).ToArray();
                    
                    foreach(var tag in set)
                    {
                        Traits.Add("Category", tag);
                        Traits.Add("tag", tag);
                    }  
                }  
            }

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

        static IEnumerable<IAttributeInfo> GetCachedTraitAttributes(IAssemblyInfo assembly)
            => assemblyTraitAttributeCache.GetOrAdd(assembly.Name, () => assembly.GetCustomAttributes(typeof(ITraitAttribute)));

        static IEnumerable<IAttributeInfo> GetCachedTraitAttributes(ITypeInfo type)
            => typeTraitAttributeCache.GetOrAdd(type.Name, () => type.GetCustomAttributes(typeof(ITraitAttribute)));

        static IEnumerable<IAttributeInfo> GetTraitAttributesData(ITestMethod testMethod)
        {
            return GetCachedTraitAttributes(testMethod.TestClass.Class.Assembly)
                  .Concat(testMethod.Method.GetCustomAttributes(typeof(ITraitAttribute)))
                  .Concat(GetCachedTraitAttributes(testMethod.TestClass.Class));
        }

          public virtual Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink,
                                                 IMessageBus messageBus,
                                                 object[] constructorArguments,
                                                 ExceptionAggregator aggregator,
                                                 CancellationTokenSource cancellationTokenSource,
                                                 IServiceProvider serviceProvider)
            => new MettleTestCaseRunner(this, DisplayName, SkipReason, constructorArguments, TestMethodArguments, messageBus, aggregator, cancellationTokenSource, this.serviceProvider ?? serviceProvider).RunAsync();

        /// <inheritdoc/>
        public virtual Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink,
                                                 IMessageBus messageBus,
                                                 object[] constructorArguments,
                                                 ExceptionAggregator aggregator,
                                                 CancellationTokenSource cancellationTokenSource)
            => new MettleTestCaseRunner(this, DisplayName, SkipReason, constructorArguments, TestMethodArguments, messageBus, aggregator, cancellationTokenSource, this.serviceProvider).RunAsync();

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