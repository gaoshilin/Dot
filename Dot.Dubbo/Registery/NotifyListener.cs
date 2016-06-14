using System;
using System.Collections.Generic;
using Dot.ServiceModel;

namespace Dot.Dubbo.Registery
{
    public class NotifyListener : INotifyListener
    {
        public delegate void OnMetadataChangedHandler(List<ServiceMetadata> metadatas);
        public event OnMetadataChangedHandler OnMetadataChanged;

        public void Notify(List<ServiceMetadata> metadatas)
        {
            this.OnMetadataChangedHandle(metadatas);
        }

        private void OnMetadataChangedHandle(List<ServiceMetadata> metadatas)
        {
            if (this.OnMetadataChanged != null)
                this.OnMetadataChanged(metadatas);
        }
    }
}