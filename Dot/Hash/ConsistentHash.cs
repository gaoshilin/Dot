using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Dot.Extension;
using Dot.Util;

namespace Dot.Hash
{
    public class MurmurHash2
    {
        const uint m = 0x5bd1e995;
        const int r = 24;

        public static uint Hash(Byte[] data)
        {
            return Hash(data, 0xc58f1a7b);
        }
        
        [StructLayout(LayoutKind.Explicit)]
        struct BytetoUInt32Converter
        {
            [FieldOffset(0)]
            public Byte[] Bytes;

            [FieldOffset(0)]
            public UInt32[] UInts;
        }

        public static UInt32 Hash(Byte[] data, UInt32 seed)
        {
            Int32 length = data.Length;
            if (length == 0)
                return 0;
            UInt32 h = seed ^ (UInt32)length;
            Int32 currentIndex = 0;
            // array will be length of Bytes but contains Uints
            // therefore the currentIndex will jump with +1 while length will jump with +4
            UInt32[] hackArray = new BytetoUInt32Converter { Bytes = data }.UInts;
            while (length >= 4)
            {
                UInt32 k = hackArray[currentIndex++];
                k *= m;
                k ^= k >> r;
                k *= m;

                h *= m;
                h ^= k;
                length -= 4;
            }
            currentIndex *= 4; // fix the length
            switch (length)
            {
                case 3:
                    h ^= (UInt16)(data[currentIndex++] | data[currentIndex++] << 8);
                    h ^= (UInt32)data[currentIndex] << 16;
                    h *= m;
                    break;
                case 2:
                    h ^= (UInt16)(data[currentIndex++] | data[currentIndex] << 8);
                    h *= m;
                    break;
                case 1:
                    h ^= data[currentIndex];
                    h *= m;
                    break;
                default:
                    break;
            }

            h ^= h >> 13;
            h *= m;
            h ^= h >> 15;

            return h;
        }
    }

    public class ConsistentHash<T>
    {
        private SortedDictionary<int, T> _circle = new SortedDictionary<int, T>();
        private Dictionary<T, int> _weights = new Dictionary<T, int>();
        private int _replicate = 100;
        private int[] _keys = null;

        public ConsistentHash(List<T> nodes, int replicate = 100)
        {
            _replicate = replicate;

            nodes.ForEach(node => this.Add(node, false));

            _keys = _circle.Keys.ToArray();
        }

        public ConsistentHash(List<T> nodes, List<int> weights, int replicate = 100)
        {
            Ensure.True(nodes.Count() == weights.Count(), "nodes count must euqal than weights count.");

            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes.ElementAt(i);
                var weight = weights.ElementAt(i);
                this.Add(node, false, weight);
            }

            _keys = _circle.Keys.ToArray();
        } 

        private void Add(T node, bool updateKeyArray = true, int weight = 1)
        {
            _weights.Add(node, weight);
            for (int i = 0; i < _replicate * weight; i++) // 复制 (_replicate * weight) 个镜像
            {
                var hash = BetterHash(node.GetHashCode().ToString() + i);  // 对 (_replicate * weight) 个镜像计算 hash
                _circle[hash] = node; // 将镜像存入圆环
            }

            if (updateKeyArray)
                _keys = _circle.Keys.ToArray();
        }

        public void Remove(T node)
        {
            for (int i = 0; i < _replicate * _weights[node]; i++)
            {
                var hash = BetterHash(node.GetHashCode().ToString() + i);
                if (_circle.Remove(hash) == false)
                    throw new Exception("can not remove a node that not added");
            }

            _keys = _circle.Keys.ToArray();
            _weights.Remove(node);
        }

        public T GetNode(String key)
        {
            int hash = BetterHash(key);
            int first = this.GetFirstHash(_keys, hash);
            return _circle[_keys[first]];
        }

        private int GetFirstHash(int[] keys, int value)
        {
            int begin = 0;
            int end = keys.Length - 1;

            if (keys[end] < value || keys[0] > value)
                return 0;

            int mid = begin;
            while (end - begin > 1)
            {
                mid = (end + begin) / 2;
                if (keys[mid] >= value)
                    end = mid;
                else
                    begin = mid;
            }

            if (keys[begin] > value || keys[end] < value)
                throw new Exception("should not happen");

            return end;
        }

        public static int BetterHash(String key)
        {
            uint hash = MurmurHash2.Hash(Encoding.ASCII.GetBytes(key));
            return (int)hash;
        }
    }
}