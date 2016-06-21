using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Dot.Dubbo.Registery;
using Dot.LoadBalance;
using Dot.LoadBalance.Weight;
using Dot.ServiceModel;
using Dot.ServiceModel.Channels;
using Dot.Threading.Atomic;

namespace Dot.Dubbo.Rpc
{
    public abstract class ServiceInvokerBase<TService> where TService : class
    {
        protected string _serviceIdentity;
        protected IRegistery _registery;
        protected string _groupPath;
        protected NotifyListener _listener;
        protected List<ServiceMetadata> _metadatas;
        protected ILoadBalance _loadBalance;
        protected IWeightCalculator<ServiceMetadata> _weightCalculator;

        public ServiceInvokerBase(IRegistery registery, string groupPath, ILoadBalance loadBalance, IWeightCalculator<ServiceMetadata> weightCalculator)
        {
            _serviceIdentity = typeof(TService).Name;
            _loadBalance = loadBalance;
            _weightCalculator = weightCalculator;
            _groupPath = groupPath;

            _listener = new NotifyListener();
            _listener.OnMetadataChanged += (metas) => _metadatas = metas;

            _registery = registery;
            _registery.Subscribe(_groupPath, _listener, true);
        }

        protected virtual ServiceProxy<TService> Open()
        {
            var meta = _loadBalance.Select<ServiceMetadata>(_weightCalculator, _metadatas, _serviceIdentity);
            if (meta == null)
                throw new ServiceMetadataNotFoundException(typeof(TService));

            var binding = this.GetBinding(meta);
            var address = new EndpointAddress(meta.Address);
            return new ServiceProxy<TService>(binding, address);
        }

        protected virtual IEnumerable<ServiceProxy<TService>> OpenAll()
        {
            foreach (var meta in _metadatas)
            {
                var binding = this.GetBinding(meta);
                var address = new EndpointAddress(meta.Address);
                yield return new ServiceProxy<TService>(binding, address);
            }
        }

        protected virtual Binding GetBinding(ServiceMetadata meta)
        {
            var bindingType = Type.GetType(meta.Binding);
            return BindingFactory.Create(bindingType);
        }
    }
}