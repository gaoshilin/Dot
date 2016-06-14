using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Dot.ServiceModel
{
    public class ServiceMetadata
    {
        public string Address { get; set; }
        public string Binding { get; set; }
        public string RootPath { get; set; }
        public string FullPath { get; set; }
        public string Path { get; set; }
        public bool CheckOnStart { get; set; }

        public ServiceMetadata()
        {
        }

        public ServiceMetadata(string rootPath, string address, string binding, bool checkOnStart = true)
        {
            this.Address = address;
            this.Binding = binding;
            this.RootPath = rootPath;
            this.Path = this.Address.GetHashCode().ToString();
            this.FullPath = rootPath + "/" + this.Path;
            this.CheckOnStart = checkOnStart;
        }

        public ServiceMetadata(Type contractType, string rootPath, EndpointAddress address, Binding binding, bool checkOnStart = true)
        {
            this.Address = address.Uri.AbsoluteUri;
            this.Binding = string.Format("{0}, {1}", binding.GetType().FullName, binding.GetType().Assembly.FullName);
            this.RootPath = rootPath;
            this.Path = this.Address.GetHashCode().ToString();
            this.FullPath = rootPath + "/" + this.Path;
            this.CheckOnStart = checkOnStart;
        }

        public override string ToString()
        {
            return this.FullPath;
        }
    }
}